/// Base class for all application-specific exceptions.
///
/// These exceptions are thrown in the data layer and converted to [Failure]
/// objects at the repository boundary before being surfaced to the domain
/// or presentation layers.
sealed class AppException implements Exception {
  const AppException({
    required this.message,
    this.statusCode,
  });

  /// A developer-facing description of the error.
  final String message;

  /// HTTP or internal status code associated with this exception, if any.
  final int? statusCode;

  @override
  String toString() => '$runtimeType(message: $message, statusCode: $statusCode)';
}

/// Thrown when the remote API returns an error response (4xx / 5xx).
final class ServerException extends AppException {
  const ServerException({
    required super.message,
    super.statusCode,
    this.errors,
  });

  /// Optional map of field-level validation errors from the API body.
  final Map<String, List<String>>? errors;

  @override
  String toString() =>
      'ServerException(message: $message, statusCode: $statusCode, errors: $errors)';
}

/// Thrown when a local storage operation fails (read/write to Hive or secure
/// storage).
final class CacheException extends AppException {
  const CacheException({
    required super.message,
    super.statusCode,
  });
}

/// Thrown when a request fails due to a connectivity problem before any
/// HTTP response is received.
final class NetworkException extends AppException {
  const NetworkException({
    super.message = 'No internet connection.',
    super.statusCode,
  });
}

/// Thrown when an authentication / authorisation check fails (e.g. expired
/// token that could not be refreshed).
final class AuthException extends AppException {
  const AuthException({
    super.message = 'Authentication failed.',
    super.statusCode = 401,
  });
}

/// Thrown when a request exceeds the configured timeout.
final class TimeoutException extends AppException {
  const TimeoutException({
    super.message = 'The request timed out. Please try again.',
    super.statusCode,
  });
}

/// Thrown when a response body cannot be parsed into the expected model.
final class ParseException extends AppException {
  const ParseException({
    super.message = 'Failed to parse the server response.',
    super.statusCode,
  });
}
