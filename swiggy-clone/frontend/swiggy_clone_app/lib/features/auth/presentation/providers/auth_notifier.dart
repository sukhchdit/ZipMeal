import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/models/auth_response_model.dart';
import '../../data/repositories/auth_repository.dart';
import 'auth_state.dart';

part 'auth_notifier.g.dart';

/// Global auth state notifier.
///
/// Marked with `keepAlive: true` so that the auth state is never disposed
/// while the app is running -- it acts as the single source of truth for
/// whether the current user is authenticated.
@Riverpod(keepAlive: true)
class AuthNotifier extends _$AuthNotifier {
  late AuthRepository _repository;

  @override
  AuthState build() {
    _repository = ref.watch(authRepositoryProvider);
    _checkAuthStatus();
    return const AuthState.initial();
  }

  /// Checks local token storage and, if a token exists, fetches the user
  /// profile to confirm the session is still valid.
  Future<void> _checkAuthStatus() async {
    final isAuthenticated = await _repository.isAuthenticated();
    if (isAuthenticated) {
      final result = await _repository.getProfile();
      if (result.data != null) {
        state = AuthState.authenticated(user: result.data!);
      } else {
        state = const AuthState.unauthenticated();
      }
    } else {
      state = const AuthState.unauthenticated();
    }
  }

  // ─────────────────────── Registration ──────────────────────────────

  Future<void> registerByPhone({
    required String phoneNumber,
    required String otp,
    required String fullName,
  }) async {
    state = const AuthState.authenticating();
    final result = await _repository.registerByPhone(
      phoneNumber: phoneNumber,
      otp: otp,
      fullName: fullName,
    );
    if (result.failure != null) {
      state = AuthState.error(failure: result.failure!);
    } else {
      state = AuthState.authenticated(user: result.data!.user);
    }
  }

  Future<void> registerByEmail({
    required String email,
    required String password,
    required String fullName,
    required String phoneNumber,
  }) async {
    state = const AuthState.authenticating();
    final result = await _repository.registerByEmail(
      email: email,
      password: password,
      fullName: fullName,
      phoneNumber: phoneNumber,
    );
    if (result.failure != null) {
      state = AuthState.error(failure: result.failure!);
    } else {
      state = AuthState.authenticated(user: result.data!.user);
    }
  }

  // ─────────────────────── Login ─────────────────────────────────────

  Future<void> loginByPhone({
    required String phoneNumber,
    required String otp,
  }) async {
    state = const AuthState.authenticating();
    final result = await _repository.loginByPhone(
      phoneNumber: phoneNumber,
      otp: otp,
    );
    if (result.failure != null) {
      state = AuthState.error(failure: result.failure!);
    } else {
      state = AuthState.authenticated(user: result.data!.user);
    }
  }

  Future<void> loginByEmail({
    required String email,
    required String password,
  }) async {
    state = const AuthState.authenticating();
    final result = await _repository.loginByEmail(
      email: email,
      password: password,
    );
    if (result.failure != null) {
      state = AuthState.error(failure: result.failure!);
    } else {
      state = AuthState.authenticated(user: result.data!.user);
    }
  }

  // ─────────────────────── OTP ───────────────────────────────────────

  /// Convenience method that delegates to the repository.
  /// The [OtpNotifier] handles the full OTP send/resend lifecycle.
  Future<void> sendOtp({required String phoneNumber}) async {
    await _repository.sendOtp(phoneNumber: phoneNumber);
  }

  // ─────────────────────── Profile ──────────────────────────────────

  /// Updates the cached user in the auth state without re-fetching from API.
  void updateUser(UserModel user) {
    state = AuthState.authenticated(user: user);
  }

  // ─────────────────────── Logout ────────────────────────────────────

  Future<void> logout() async {
    await _repository.logout();
    state = const AuthState.unauthenticated();
  }

  Future<void> logoutAll() async {
    await _repository.logoutAll();
    state = const AuthState.unauthenticated();
  }
}
