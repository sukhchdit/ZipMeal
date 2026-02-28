import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../constants/api_constants.dart';
import '../storage/secure_storage_service.dart';
import 'api_interceptors.dart';

part 'api_client.g.dart';

/// Provides a configured [Dio] instance as a singleton throughout the app.
///
/// Interceptor order matters:
///   1. [AuthInterceptor] -- injects Bearer token & handles 401 refresh.
///   2. [RetryInterceptor] -- retries transient failures (5xx, timeouts).
///   3. [LoggingInterceptor] -- logs request/response for debugging.
@riverpod
Dio apiClient(Ref ref) {
  final secureStorage = ref.watch(secureStorageServiceProvider);

  final dio = Dio(
    BaseOptions(
      baseUrl: ApiConstants.baseUrl,
      connectTimeout: const Duration(
        milliseconds: ApiConstants.connectTimeoutMs,
      ),
      receiveTimeout: const Duration(
        milliseconds: ApiConstants.receiveTimeoutMs,
      ),
      sendTimeout: const Duration(
        milliseconds: ApiConstants.sendTimeoutMs,
      ),
      headers: <String, dynamic>{
        'Content-Type': 'application/json',
        'Accept': 'application/json',
      },
      responseType: ResponseType.json,
      validateStatus: (int? status) => status != null && status < 500,
    ),
  );

  dio.interceptors.addAll([
    AuthInterceptor(dio: dio, secureStorage: secureStorage),
    RetryInterceptor(dio: dio),
    LoggingInterceptor(),
  ]);

  return dio;
}
