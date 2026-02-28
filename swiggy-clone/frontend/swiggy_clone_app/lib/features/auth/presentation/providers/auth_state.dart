import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/auth_response_model.dart';

part 'auth_state.freezed.dart';

/// Sealed union representing every possible authentication state.
///
/// Use Dart 3 pattern matching (`switch (state) { case AuthAuthenticated(): }`)
/// in the UI to react to state transitions exhaustively.
@freezed
sealed class AuthState with _$AuthState {
  /// App just launched; auth status has not been determined yet.
  const factory AuthState.initial() = AuthInitial;

  /// User is confirmed to be unauthenticated (no valid token).
  const factory AuthState.unauthenticated() = AuthUnauthenticated;

  /// A login / register request is currently in flight.
  const factory AuthState.authenticating() = AuthAuthenticating;

  /// User is authenticated; [user] contains their profile data.
  const factory AuthState.authenticated({required UserModel user}) =
      AuthAuthenticated;

  /// An authentication attempt failed with [failure].
  const factory AuthState.error({required Failure failure}) = AuthError;
}
