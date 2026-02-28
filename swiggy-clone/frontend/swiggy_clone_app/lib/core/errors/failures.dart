import 'package:equatable/equatable.dart';

/// Base class for domain-layer failures.
///
/// Each subclass maps to a specific category of error (server, cache, network)
/// and carries a human-readable [message] plus an optional [statusCode].
///
/// Failures are value objects -- two failures with the same properties are
/// considered equal thanks to [Equatable].
sealed class Failure extends Equatable {
  const Failure({
    required this.message,
    this.statusCode,
  });

  /// A user-friendly description of what went wrong.
  final String message;

  /// HTTP or internal status code associated with this failure, if any.
  final int? statusCode;

  @override
  List<Object?> get props => [message, statusCode];
}

/// A failure originating from the remote API (HTTP 4xx/5xx, parse errors, etc.).
final class ServerFailure extends Failure {
  const ServerFailure({
    required super.message,
    super.statusCode,
    this.errors,
  });

  /// Optional map of field-level validation errors returned by the server.
  final Map<String, List<String>>? errors;

  @override
  List<Object?> get props => [message, statusCode, errors];
}

/// A failure originating from local storage (Hive, secure storage, etc.).
final class CacheFailure extends Failure {
  const CacheFailure({
    required super.message,
    super.statusCode,
  });
}

/// A failure caused by a connectivity problem (airplane mode, DNS, etc.).
final class NetworkFailure extends Failure {
  const NetworkFailure({
    super.message = 'No internet connection. Please check your network and try again.',
    super.statusCode,
  });
}

/// A failure caused by an authentication or authorization issue.
final class AuthFailure extends Failure {
  const AuthFailure({
    super.message = 'Session expired. Please log in again.',
    super.statusCode = 401,
  });
}

/// A failure caused by invalid input that was caught before reaching the API.
final class ValidationFailure extends Failure {
  const ValidationFailure({
    required super.message,
    this.fieldErrors,
  });

  /// Per-field validation messages, e.g. `{'email': ['Invalid format']}`.
  final Map<String, List<String>>? fieldErrors;

  @override
  List<Object?> get props => [message, fieldErrors];
}
