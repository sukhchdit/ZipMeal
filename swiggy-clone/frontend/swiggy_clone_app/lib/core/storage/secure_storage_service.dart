import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

part 'secure_storage_service.g.dart';

/// Provides a singleton [SecureStorageService] across the application.
@riverpod
SecureStorageService secureStorageService(Ref ref) => SecureStorageService();

/// Thin wrapper around [FlutterSecureStorage] that exposes a typed API
/// for persisting and retrieving sensitive data such as auth tokens.
///
/// All values are stored as encrypted key-value pairs using the platform's
/// native keychain / keystore implementation.
class SecureStorageService {
  SecureStorageService({FlutterSecureStorage? storage})
      : _storage = storage ??
            const FlutterSecureStorage(
              aOptions: AndroidOptions(encryptedSharedPreferences: true),
              iOptions: IOSOptions(
                accessibility: KeychainAccessibility.first_unlock,
              ),
            );

  final FlutterSecureStorage _storage;

  // ─────────────────────── Storage Keys ─────────────────────────

  static const String _accessTokenKey = 'access_token';
  static const String _refreshTokenKey = 'refresh_token';
  static const String _userIdKey = 'user_id';
  static const String _fcmTokenKey = 'fcm_token';

  // ─────────────────────── Access Token ─────────────────────────

  /// Retrieves the current access token, or `null` if none is stored.
  Future<String?> getAccessToken() => _storage.read(key: _accessTokenKey);

  /// Persists the given [token] as the current access token.
  Future<void> setAccessToken(String token) =>
      _storage.write(key: _accessTokenKey, value: token);

  /// Deletes the stored access token.
  Future<void> deleteAccessToken() => _storage.delete(key: _accessTokenKey);

  // ─────────────────────── Refresh Token ────────────────────────

  /// Retrieves the current refresh token, or `null` if none is stored.
  Future<String?> getRefreshToken() => _storage.read(key: _refreshTokenKey);

  /// Persists the given [token] as the current refresh token.
  Future<void> setRefreshToken(String token) =>
      _storage.write(key: _refreshTokenKey, value: token);

  /// Deletes the stored refresh token.
  Future<void> deleteRefreshToken() => _storage.delete(key: _refreshTokenKey);

  // ─────────────────────── User ID ──────────────────────────────

  /// Retrieves the current user ID, or `null` if none is stored.
  Future<String?> getUserId() => _storage.read(key: _userIdKey);

  /// Persists the given [userId].
  Future<void> setUserId(String userId) =>
      _storage.write(key: _userIdKey, value: userId);

  // ─────────────────────── FCM Token ────────────────────────────

  /// Retrieves the stored FCM device token.
  Future<String?> getFcmToken() => _storage.read(key: _fcmTokenKey);

  /// Persists the given FCM [token].
  Future<void> setFcmToken(String token) =>
      _storage.write(key: _fcmTokenKey, value: token);

  // ─────────────────────── Bulk Operations ──────────────────────

  /// Clears both access and refresh tokens (e.g. on logout or forced re-auth).
  Future<void> clearTokens() async {
    await Future.wait([
      deleteAccessToken(),
      deleteRefreshToken(),
    ]);
  }

  /// Removes **all** entries managed by this service.
  Future<void> clearAll() => _storage.deleteAll();

  // ─────────────────────── Generic Helpers ──────────────────────

  /// Reads an arbitrary [key] from secure storage.
  Future<String?> read(String key) => _storage.read(key: key);

  /// Writes an arbitrary [key]-[value] pair to secure storage.
  Future<void> write({required String key, required String value}) =>
      _storage.write(key: key, value: value);

  /// Deletes an arbitrary [key] from secure storage.
  Future<void> delete(String key) => _storage.delete(key: key);

  /// Returns `true` when the given [key] exists in secure storage.
  Future<bool> containsKey(String key) => _storage.containsKey(key: key);
}
