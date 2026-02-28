import 'dart:async';
import 'dart:io';

import 'package:dio/dio.dart';
import 'package:logger/logger.dart';

import '../constants/api_constants.dart';
import '../storage/secure_storage_service.dart';

final Logger _logger = Logger(
  printer: PrettyPrinter(
    methodCount: 0,
    printEmojis: false,
    dateTimeFormat: DateTimeFormat.onlyTimeAndSinceStart,
  ),
);

// ────────────────────────────────────────────────────────────────────────────
// Auth Interceptor
// ────────────────────────────────────────────────────────────────────────────

/// Attaches the Bearer token to every outgoing request and handles
/// transparent token refresh on HTTP 401 responses.
///
/// While a refresh is in flight, all subsequent requests are queued and
/// replayed once the new token is available.
class AuthInterceptor extends QueuedInterceptor {
  AuthInterceptor({
    required Dio dio,
    required SecureStorageService secureStorage,
  })  : _dio = dio,
        _secureStorage = secureStorage;

  final Dio _dio;
  final SecureStorageService _secureStorage;

  bool _isRefreshing = false;
  final List<_QueuedRequest> _pendingRequests = [];

  @override
  Future<void> onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    // Skip auth header for public endpoints.
    if (_isPublicEndpoint(options.path)) {
      return handler.next(options);
    }

    final token = await _secureStorage.getAccessToken();
    if (token != null && token.isNotEmpty) {
      options.headers['Authorization'] = 'Bearer $token';
    }
    return handler.next(options);
  }

  @override
  Future<void> onError(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    if (err.response?.statusCode != HttpStatus.unauthorized) {
      return handler.next(err);
    }

    // If a refresh is already in progress, queue this request.
    if (_isRefreshing) {
      _pendingRequests.add(
        _QueuedRequest(
          options: err.requestOptions,
          handler: handler,
        ),
      );
      return;
    }

    _isRefreshing = true;

    try {
      final newToken = await _refreshAccessToken();

      if (newToken != null) {
        // Retry the original failed request with the new token.
        err.requestOptions.headers['Authorization'] = 'Bearer $newToken';
        final response = await _dio.fetch<dynamic>(err.requestOptions);
        handler.resolve(response);

        // Replay all queued requests.
        _replayPendingRequests(newToken);
      } else {
        // Refresh failed -- force logout / redirect to login.
        await _secureStorage.clearTokens();
        handler.next(err);
        _rejectPendingRequests(err);
      }
    } on DioException catch (refreshError) {
      await _secureStorage.clearTokens();
      handler.next(refreshError);
      _rejectPendingRequests(refreshError);
    } finally {
      _isRefreshing = false;
    }
  }

  /// Attempts to obtain a new access token using the stored refresh token.
  Future<String?> _refreshAccessToken() async {
    final refreshToken = await _secureStorage.getRefreshToken();
    if (refreshToken == null || refreshToken.isEmpty) return null;

    // Use a fresh Dio instance to avoid interceptor recursion.
    final refreshDio = Dio(
      BaseOptions(
        baseUrl: ApiConstants.baseUrl,
        headers: <String, dynamic>{
          'Content-Type': 'application/json',
        },
      ),
    );

    final response = await refreshDio.post<Map<String, dynamic>>(
      ApiConstants.authRefreshToken,
      data: <String, dynamic>{'refreshToken': refreshToken},
    );

    if (response.statusCode == HttpStatus.ok && response.data != null) {
      final data = response.data!;
      final newAccessToken = data['accessToken'] as String?;
      final newRefreshToken = data['refreshToken'] as String?;

      if (newAccessToken != null) {
        await _secureStorage.setAccessToken(newAccessToken);
      }
      if (newRefreshToken != null) {
        await _secureStorage.setRefreshToken(newRefreshToken);
      }

      return newAccessToken;
    }

    return null;
  }

  /// Replays all queued requests with the fresh [token].
  void _replayPendingRequests(String token) {
    for (final queued in _pendingRequests) {
      queued.options.headers['Authorization'] = 'Bearer $token';
      _dio.fetch<dynamic>(queued.options).then(
            queued.handler.resolve,
            onError: (Object e) {
              if (e is DioException) {
                queued.handler.reject(e);
              }
            },
          );
    }
    _pendingRequests.clear();
  }

  /// Rejects all queued requests when refresh fails.
  void _rejectPendingRequests(DioException error) {
    for (final queued in _pendingRequests) {
      queued.handler.reject(
        DioException(
          requestOptions: queued.options,
          error: error.error,
          response: error.response,
          type: error.type,
        ),
      );
    }
    _pendingRequests.clear();
  }

  /// Returns `true` for endpoints that do not require an auth token.
  bool _isPublicEndpoint(String path) {
    const publicPaths = <String>[
      ApiConstants.authLoginPhone,
      ApiConstants.authLoginEmail,
      ApiConstants.authRegister,
      ApiConstants.authRegisterEmail,
      ApiConstants.authSendOtp,
      ApiConstants.authVerifyOtp,
      ApiConstants.authRefreshToken,
    ];
    return publicPaths.any(path.contains);
  }
}

/// Internal helper representing a request waiting for token refresh.
class _QueuedRequest {
  const _QueuedRequest({
    required this.options,
    required this.handler,
  });

  final RequestOptions options;
  final ErrorInterceptorHandler handler;
}

// ────────────────────────────────────────────────────────────────────────────
// Retry Interceptor
// ────────────────────────────────────────────────────────────────────────────

/// Retries failed requests caused by transient network issues or server errors.
///
/// Uses exponential back-off with a configurable [maxRetries] count.
class RetryInterceptor extends Interceptor {
  RetryInterceptor({
    required Dio dio,
    this.maxRetries = 3,
  }) : _dio = dio;

  final Dio _dio;
  final int maxRetries;

  static const String _retryCountKey = 'retryCount';

  @override
  Future<void> onError(
    DioException err,
    ErrorInterceptorHandler handler,
  ) async {
    if (!_shouldRetry(err)) {
      return handler.next(err);
    }

    final currentRetry =
        (err.requestOptions.extra[_retryCountKey] as int?) ?? 0;

    if (currentRetry >= maxRetries) {
      return handler.next(err);
    }

    final nextRetry = currentRetry + 1;
    err.requestOptions.extra[_retryCountKey] = nextRetry;

    // Exponential back-off: 1s, 2s, 4s ...
    final delay = Duration(seconds: 1 << (nextRetry - 1));
    _logger.w(
      'Retrying request (attempt $nextRetry/$maxRetries) '
      'after ${delay.inSeconds}s -- ${err.requestOptions.uri}',
    );

    await Future<void>.delayed(delay);

    try {
      final response = await _dio.fetch<dynamic>(err.requestOptions);
      return handler.resolve(response);
    } on DioException catch (retryError) {
      return handler.next(retryError);
    }
  }

  bool _shouldRetry(DioException err) {
    // Retry on connection errors or 5xx server errors.
    if (err.type == DioExceptionType.connectionTimeout ||
        err.type == DioExceptionType.sendTimeout ||
        err.type == DioExceptionType.receiveTimeout ||
        err.type == DioExceptionType.connectionError) {
      return true;
    }

    final statusCode = err.response?.statusCode;
    return statusCode != null && statusCode >= 500;
  }
}

// ────────────────────────────────────────────────────────────────────────────
// Logging Interceptor
// ────────────────────────────────────────────────────────────────────────────

/// Logs HTTP traffic for debugging purposes.
///
/// In release builds this interceptor intentionally suppresses response body
/// output to avoid leaking sensitive data into production logs.
class LoggingInterceptor extends Interceptor {
  @override
  void onRequest(RequestOptions options, RequestInterceptorHandler handler) {
    _logger.d(
      '--> ${options.method.toUpperCase()} ${options.uri}\n'
      '    Headers: ${options.headers}\n'
      '    Data: ${options.data}',
    );
    handler.next(options);
  }

  @override
  void onResponse(
    Response<dynamic> response,
    ResponseInterceptorHandler handler,
  ) {
    _logger.d(
      '<-- ${response.statusCode} ${response.requestOptions.uri}\n'
      '    Data: ${response.data}',
    );
    handler.next(response);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) {
    _logger.e(
      '<-- ERROR ${err.response?.statusCode ?? 'N/A'} '
      '${err.requestOptions.uri}\n'
      '    ${err.message}',
    );
    handler.next(err);
  }
}
