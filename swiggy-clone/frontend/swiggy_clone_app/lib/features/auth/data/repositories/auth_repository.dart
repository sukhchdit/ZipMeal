import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../../../core/storage/secure_storage_service.dart';
import '../datasources/auth_remote_data_source.dart';
import '../models/auth_response_model.dart';
import '../models/session_model.dart';

part 'auth_repository.g.dart';

@riverpod
AuthRepository authRepository(Ref ref) {
  final remoteDataSource = ref.watch(authRemoteDataSourceProvider);
  final secureStorage = ref.watch(secureStorageServiceProvider);
  return AuthRepository(
    remoteDataSource: remoteDataSource,
    secureStorage: secureStorage,
  );
}

/// Repository that mediates between the auth data source and the presentation
/// layer, handling error mapping and token persistence.
///
/// Every public method returns a record with nullable [data] and [Failure]
/// fields, following the "result" pattern to avoid throwing exceptions into
/// the UI layer.
class AuthRepository {
  AuthRepository({
    required AuthRemoteDataSource remoteDataSource,
    required SecureStorageService secureStorage,
  })  : _remoteDataSource = remoteDataSource,
        _secureStorage = secureStorage;

  final AuthRemoteDataSource _remoteDataSource;
  final SecureStorageService _secureStorage;

  // ─────────────────────── Registration ──────────────────────────────

  Future<({AuthResponseModel? data, Failure? failure})> registerByPhone({
    required String phoneNumber,
    required String otp,
    required String fullName,
    String? referralCode,
  }) async {
    try {
      final response = await _remoteDataSource.registerByPhone(
        phoneNumber: phoneNumber,
        otp: otp,
        fullName: fullName,
        referralCode: referralCode,
      );
      await _persistTokens(response);
      return (data: response, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({AuthResponseModel? data, Failure? failure})> registerByEmail({
    required String email,
    required String password,
    required String fullName,
    required String phoneNumber,
    String? referralCode,
  }) async {
    try {
      final response = await _remoteDataSource.registerByEmail(
        email: email,
        password: password,
        fullName: fullName,
        phoneNumber: phoneNumber,
        referralCode: referralCode,
      );
      await _persistTokens(response);
      return (data: response, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Login ─────────────────────────────────────

  Future<({AuthResponseModel? data, Failure? failure})> loginByPhone({
    required String phoneNumber,
    required String otp,
  }) async {
    try {
      final response = await _remoteDataSource.loginByPhone(
        phoneNumber: phoneNumber,
        otp: otp,
      );
      await _persistTokens(response);
      return (data: response, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({AuthResponseModel? data, Failure? failure})> loginByEmail({
    required String email,
    required String password,
  }) async {
    try {
      final response = await _remoteDataSource.loginByEmail(
        email: email,
        password: password,
      );
      await _persistTokens(response);
      return (data: response, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── OTP ───────────────────────────────────────

  Future<Failure?> sendOtp({required String phoneNumber}) async {
    try {
      await _remoteDataSource.sendOtp(phoneNumber: phoneNumber);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  // ─────────────────────── Logout ────────────────────────────────────

  Future<Failure?> logout() async {
    try {
      final refreshToken = await _secureStorage.getRefreshToken();
      if (refreshToken != null) {
        await _remoteDataSource.logout(refreshToken: refreshToken);
      }
      await _secureStorage.clearTokens();
      return null;
    } on DioException catch (e) {
      // Always clear local tokens even if the server call fails.
      await _secureStorage.clearTokens();
      return _mapDioError(e);
    }
  }

  Future<Failure?> logoutAll() async {
    try {
      await _remoteDataSource.logoutAll();
      await _secureStorage.clearTokens();
      return null;
    } on DioException catch (e) {
      await _secureStorage.clearTokens();
      return _mapDioError(e);
    }
  }

  // ─────────────────────── Profile ───────────────────────────────────

  Future<({UserModel? data, Failure? failure})> getProfile() async {
    try {
      final user = await _remoteDataSource.getProfile();
      return (data: user, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({UserModel? data, Failure? failure})> updateProfile({
    String? fullName,
    String? email,
    String? avatarUrl,
  }) async {
    try {
      final user = await _remoteDataSource.updateProfile(
        fullName: fullName,
        email: email,
        avatarUrl: avatarUrl,
      );
      return (data: user, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Sessions ──────────────────────────────────

  Future<({List<SessionModel>? data, Failure? failure})> getSessions() async {
    try {
      final sessions = await _remoteDataSource.getSessions();
      return (data: sessions, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> revokeSession({required String sessionId}) async {
    try {
      await _remoteDataSource.revokeSession(sessionId: sessionId);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  // ─────────────────────── Password ─────────────────────────────────

  Future<Failure?> changePassword({
    required String currentPassword,
    required String newPassword,
  }) async {
    try {
      await _remoteDataSource.changePassword(
        currentPassword: currentPassword,
        newPassword: newPassword,
      );
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  // ─────────────────────── Account Deletion ───────────────────────────

  Future<Failure?> deleteAccount() async {
    try {
      await _remoteDataSource.deleteAccount();
      await _secureStorage.clearTokens();
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  // ─────────────────────── Auth Status ───────────────────────────────

  /// Returns `true` if a non-empty access token is stored locally.
  Future<bool> isAuthenticated() async {
    final token = await _secureStorage.getAccessToken();
    return token != null && token.isNotEmpty;
  }

  // ─────────────────────── Private Helpers ───────────────────────────

  /// Persists access token, refresh token, and user ID to secure storage.
  Future<void> _persistTokens(AuthResponseModel response) async {
    await Future.wait([
      _secureStorage.setAccessToken(response.accessToken),
      _secureStorage.setRefreshToken(response.refreshToken),
      _secureStorage.setUserId(response.user.id),
    ]);
  }

  /// Maps a [DioException] to the appropriate [Failure] subclass.
  Failure _mapDioError(DioException e) {
    if (e.type == DioExceptionType.connectionError ||
        e.type == DioExceptionType.connectionTimeout) {
      return const NetworkFailure();
    }

    final statusCode = e.response?.statusCode;
    final data = e.response?.data;
    String message = 'An unexpected error occurred.';

    if (data is Map<String, dynamic>) {
      message = (data['errorMessage'] as String?) ?? message;
    }

    if (statusCode == 401) {
      return AuthFailure(message: message);
    }

    return ServerFailure(message: message, statusCode: statusCode);
  }
}
