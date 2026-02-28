import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/auth_response_model.dart';
import '../models/session_model.dart';

part 'auth_remote_data_source.g.dart';

@riverpod
AuthRemoteDataSource authRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return AuthRemoteDataSource(dio: dio);
}

/// Remote data source that handles all authentication-related API calls.
///
/// Every method communicates with the backend auth endpoints and either
/// returns the parsed response model or throws a [DioException] on failure.
class AuthRemoteDataSource {
  AuthRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  /// Registers a new user via phone number + OTP verification.
  Future<AuthResponseModel> registerByPhone({
    required String phoneNumber,
    required String otp,
    required String fullName,
    String? referralCode,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.authRegister,
      data: {
        'phoneNumber': phoneNumber,
        'otp': otp,
        'fullName': fullName,
        if (referralCode != null) 'referralCode': referralCode,
      },
    );
    return AuthResponseModel.fromJson(response.data!);
  }

  /// Registers a new user via email and password.
  Future<AuthResponseModel> registerByEmail({
    required String email,
    required String password,
    required String fullName,
    required String phoneNumber,
    String? referralCode,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.authRegisterEmail,
      data: {
        'email': email,
        'password': password,
        'fullName': fullName,
        'phoneNumber': phoneNumber,
        if (referralCode != null) 'referralCode': referralCode,
      },
    );
    return AuthResponseModel.fromJson(response.data!);
  }

  /// Logs in a user with phone number + OTP.
  Future<AuthResponseModel> loginByPhone({
    required String phoneNumber,
    required String otp,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.authLoginPhone,
      data: {
        'phoneNumber': phoneNumber,
        'otp': otp,
      },
    );
    return AuthResponseModel.fromJson(response.data!);
  }

  /// Logs in a user with email and password.
  Future<AuthResponseModel> loginByEmail({
    required String email,
    required String password,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.authLoginEmail,
      data: {
        'email': email,
        'password': password,
      },
    );
    return AuthResponseModel.fromJson(response.data!);
  }

  /// Sends an OTP to the given phone number.
  Future<void> sendOtp({required String phoneNumber}) async {
    await _dio.post<dynamic>(
      ApiConstants.authSendOtp,
      data: {'phoneNumber': phoneNumber},
    );
  }

  /// Refreshes the access token using the given refresh token.
  Future<AuthResponseModel> refreshToken({
    required String refreshToken,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.authRefreshToken,
      data: {'refreshToken': refreshToken},
    );
    return AuthResponseModel.fromJson(response.data!);
  }

  /// Logs out the current session by invalidating the refresh token.
  Future<void> logout({required String refreshToken}) async {
    await _dio.post<dynamic>(
      ApiConstants.authLogout,
      data: {'refreshToken': refreshToken},
    );
  }

  /// Logs out all sessions for the current user.
  Future<void> logoutAll() async {
    await _dio.post<dynamic>(ApiConstants.authLogoutAll);
  }

  /// Fetches the authenticated user's profile.
  Future<UserModel> getProfile() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.authMe,
    );
    return UserModel.fromJson(response.data!);
  }

  /// Updates the authenticated user's profile fields.
  Future<UserModel> updateProfile({
    String? fullName,
    String? email,
    String? avatarUrl,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.authMe,
      data: {
        if (fullName != null) 'fullName': fullName,
        if (email != null) 'email': email,
        if (avatarUrl != null) 'avatarUrl': avatarUrl,
      },
    );
    return UserModel.fromJson(response.data!);
  }

  /// Fetches all active sessions for the current user.
  Future<List<SessionModel>> getSessions() async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.authSessions,
    );
    return response.data!
        .map((e) => SessionModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  /// Revokes (deletes) a specific session by its ID.
  Future<void> revokeSession({required String sessionId}) async {
    await _dio.delete<dynamic>(
      '${ApiConstants.authSessions}/$sessionId',
    );
  }

  /// Changes the password for the current authenticated user.
  Future<void> changePassword({
    required String currentPassword,
    required String newPassword,
  }) async {
    await _dio.put<dynamic>(
      ApiConstants.authChangePassword,
      data: {
        'currentPassword': currentPassword,
        'newPassword': newPassword,
      },
    );
  }

  /// Deletes (soft-deletes) the current authenticated user's account.
  Future<void> deleteAccount() async {
    await _dio.delete<dynamic>(ApiConstants.authMe);
  }
}
