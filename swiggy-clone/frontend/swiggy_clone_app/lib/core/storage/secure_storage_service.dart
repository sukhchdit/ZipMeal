import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:hive_flutter/hive_flutter.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

part 'secure_storage_service.g.dart';

/// Provides a singleton [SecureStorageService] across the application.
@riverpod
SecureStorageService secureStorageService(Ref ref) => SecureStorageService();

/// Thin wrapper around [FlutterSecureStorage] that exposes a typed API
/// for persisting and retrieving sensitive data such as auth tokens.
///
/// On mobile, uses the platform's native keychain / keystore via
/// [FlutterSecureStorage]. On web, falls back to a Hive box since
/// the Web Crypto API can throw OperationError on non-HTTPS origins.
class SecureStorageService {
  SecureStorageService({FlutterSecureStorage? storage})
      : _storage = kIsWeb
            ? null
            : (storage ??
                const FlutterSecureStorage(
                  aOptions: AndroidOptions(encryptedSharedPreferences: true),
                  iOptions: IOSOptions(
                    accessibility: KeychainAccessibility.first_unlock,
                  ),
                ));

  final FlutterSecureStorage? _storage;

  static const String _webBoxName = 'secure_tokens';
  static Box<String>? _webBox;

  Future<Box<String>> _getWebBox() async {
    _webBox ??= await Hive.openBox<String>(_webBoxName);
    return _webBox!;
  }

  Future<String?> _webRead(String key) async {
    final box = await _getWebBox();
    return box.get(key);
  }

  Future<void> _webWrite(String key, String value) async {
    final box = await _getWebBox();
    await box.put(key, value);
  }

  Future<void> _webDelete(String key) async {
    final box = await _getWebBox();
    await box.delete(key);
  }

  Future<void> _webDeleteAll() async {
    final box = await _getWebBox();
    await box.clear();
  }

  Future<bool> _webContainsKey(String key) async {
    final box = await _getWebBox();
    return box.containsKey(key);
  }

  // ─────────────────────── Storage Keys ─────────────────────────

  static const String _accessTokenKey = 'access_token';
  static const String _refreshTokenKey = 'refresh_token';
  static const String _userIdKey = 'user_id';
  static const String _fcmTokenKey = 'fcm_token';

  // ─────────────────────── Access Token ─────────────────────────

  /// Retrieves the current access token, or `null` if none is stored.
  Future<String?> getAccessToken() =>
      kIsWeb ? _webRead(_accessTokenKey) : _storage!.read(key: _accessTokenKey);

  /// Persists the given [token] as the current access token.
  Future<void> setAccessToken(String token) => kIsWeb
      ? _webWrite(_accessTokenKey, token)
      : _storage!.write(key: _accessTokenKey, value: token);

  /// Deletes the stored access token.
  Future<void> deleteAccessToken() => kIsWeb
      ? _webDelete(_accessTokenKey)
      : _storage!.delete(key: _accessTokenKey);

  // ─────────────────────── Refresh Token ────────────────────────

  /// Retrieves the current refresh token, or `null` if none is stored.
  Future<String?> getRefreshToken() => kIsWeb
      ? _webRead(_refreshTokenKey)
      : _storage!.read(key: _refreshTokenKey);

  /// Persists the given [token] as the current refresh token.
  Future<void> setRefreshToken(String token) => kIsWeb
      ? _webWrite(_refreshTokenKey, token)
      : _storage!.write(key: _refreshTokenKey, value: token);

  /// Deletes the stored refresh token.
  Future<void> deleteRefreshToken() => kIsWeb
      ? _webDelete(_refreshTokenKey)
      : _storage!.delete(key: _refreshTokenKey);

  // ─────────────────────── User ID ──────────────────────────────

  /// Retrieves the current user ID, or `null` if none is stored.
  Future<String?> getUserId() =>
      kIsWeb ? _webRead(_userIdKey) : _storage!.read(key: _userIdKey);

  /// Persists the given [userId].
  Future<void> setUserId(String userId) => kIsWeb
      ? _webWrite(_userIdKey, userId)
      : _storage!.write(key: _userIdKey, value: userId);

  // ─────────────────────── FCM Token ────────────────────────────

  /// Retrieves the stored FCM device token.
  Future<String?> getFcmToken() =>
      kIsWeb ? _webRead(_fcmTokenKey) : _storage!.read(key: _fcmTokenKey);

  /// Persists the given FCM [token].
  Future<void> setFcmToken(String token) => kIsWeb
      ? _webWrite(_fcmTokenKey, token)
      : _storage!.write(key: _fcmTokenKey, value: token);

  // ─────────────────────── Bulk Operations ──────────────────────

  /// Clears both access and refresh tokens (e.g. on logout or forced re-auth).
  Future<void> clearTokens() async {
    await Future.wait([
      deleteAccessToken(),
      deleteRefreshToken(),
    ]);
  }

  /// Removes **all** entries managed by this service.
  Future<void> clearAll() =>
      kIsWeb ? _webDeleteAll() : _storage!.deleteAll();

  // ─────────────────────── Generic Helpers ──────────────────────

  /// Reads an arbitrary [key] from secure storage.
  Future<String?> read(String key) =>
      kIsWeb ? _webRead(key) : _storage!.read(key: key);

  /// Writes an arbitrary [key]-[value] pair to secure storage.
  Future<void> write({required String key, required String value}) =>
      kIsWeb ? _webWrite(key, value) : _storage!.write(key: key, value: value);

  /// Deletes an arbitrary [key] from secure storage.
  Future<void> delete(String key) =>
      kIsWeb ? _webDelete(key) : _storage!.delete(key: key);

  /// Returns `true` when the given [key] exists in secure storage.
  Future<bool> containsKey(String key) =>
      kIsWeb ? _webContainsKey(key) : _storage!.containsKey(key: key);
}
