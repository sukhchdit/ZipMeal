import 'package:hive_flutter/hive_flutter.dart';
import 'package:logger/logger.dart';

final Logger _logger = Logger(
  printer: PrettyPrinter(
    methodCount: 0,
    printEmojis: false,
    dateTimeFormat: DateTimeFormat.onlyTimeAndSinceStart,
  ),
);

/// Wrapper around [Hive] for non-sensitive local key-value storage.
///
/// Uses the singleton pattern so the same [LocalStorageService] instance is
/// shared across the entire application. Call [init] once during app
/// bootstrap (in `main.dart`) before reading or writing any data.
///
/// ## Usage
///
/// ```dart
/// final storage = LocalStorageService.instance;
/// await storage.put<bool>(LocalStorageKeys.onboardingCompleted, true);
/// final done = storage.get<bool>(LocalStorageKeys.onboardingCompleted);
/// ```
class LocalStorageService {
  LocalStorageService._();

  /// Global singleton accessor.
  static final LocalStorageService instance = LocalStorageService._();

  late Box<dynamic> _generalBox;
  late Box<dynamic> _searchHistoryBox;
  late Box<dynamic> _cacheBox;

  bool _isInitialised = false;

  // ─────────────────────── Box Names ────────────────────────────

  static const String _generalBoxName = 'general';
  static const String _searchHistoryBoxName = 'search_history';
  static const String _cacheBoxName = 'api_cache';

  // ─────────────────────── Initialisation ───────────────────────

  /// Opens all required Hive boxes.
  ///
  /// Must be called **once** before any read/write operation (typically in
  /// `main.dart` after `Hive.initFlutter()`).
  Future<void> init() async {
    if (_isInitialised) return;

    try {
      _generalBox = await Hive.openBox<dynamic>(_generalBoxName);
      _searchHistoryBox = await Hive.openBox<dynamic>(_searchHistoryBoxName);
      _cacheBox = await Hive.openBox<dynamic>(_cacheBoxName);
      _isInitialised = true;
      _logger.i('LocalStorageService initialised successfully.');
    } on HiveError catch (e, s) {
      _logger.e('Failed to initialise Hive boxes', error: e, stackTrace: s);
      rethrow;
    }
  }

  void _assertInitialised() {
    assert(_isInitialised, 'LocalStorageService.init() has not been called.');
  }

  // ─────────────────────── General Box ──────────────────────────

  /// Retrieves a value of type [T] from the general box, or [defaultValue] if
  /// the [key] does not exist.
  T? get<T>(String key, {T? defaultValue}) {
    _assertInitialised();
    return _generalBox.get(key, defaultValue: defaultValue) as T?;
  }

  /// Writes a [value] associated with [key] to the general box.
  Future<void> put<T>(String key, T value) async {
    _assertInitialised();
    await _generalBox.put(key, value);
  }

  /// Removes the entry for [key] from the general box.
  Future<void> remove(String key) async {
    _assertInitialised();
    await _generalBox.delete(key);
  }

  /// Returns `true` when [key] exists in the general box.
  bool containsKey(String key) {
    _assertInitialised();
    return _generalBox.containsKey(key);
  }

  // ─────────────────────── Search History ───────────────────────

  /// Returns the stored search history list.
  List<String> getSearchHistory() {
    _assertInitialised();
    return _searchHistoryBox.values.cast<String>().toList();
  }

  /// Adds a [query] to the top of search history (most-recent-first).
  ///
  /// Duplicates are removed and the history is capped at [maxEntries].
  Future<void> addSearchQuery(String query, {int maxEntries = 20}) async {
    _assertInitialised();
    final history = getSearchHistory()..remove(query);
    history.insert(0, query);
    if (history.length > maxEntries) {
      history.removeRange(maxEntries, history.length);
    }
    await _searchHistoryBox.clear();
    await _searchHistoryBox.addAll(history);
  }

  /// Removes a specific [query] from search history.
  Future<void> removeSearchQuery(String query) async {
    _assertInitialised();
    final history = getSearchHistory()..remove(query);
    await _searchHistoryBox.clear();
    await _searchHistoryBox.addAll(history);
  }

  /// Clears the entire search history.
  Future<void> clearSearchHistory() async {
    _assertInitialised();
    await _searchHistoryBox.clear();
  }

  // ─────────────────────── API Response Cache ───────────────────

  /// Stores a cached API response [data] for the given [key] along with a
  /// [ttl] (time-to-live). Reading back the value after the TTL has elapsed
  /// returns `null`.
  Future<void> cacheResponse(
    String key,
    dynamic data, {
    Duration ttl = const Duration(minutes: 5),
  }) async {
    _assertInitialised();
    final expiresAt = DateTime.now().add(ttl).millisecondsSinceEpoch;
    await _cacheBox.put(key, <String, dynamic>{
      'data': data,
      'expiresAt': expiresAt,
    });
  }

  /// Retrieves a previously cached response for [key], or `null` if the entry
  /// does not exist or has expired.
  dynamic getCachedResponse(String key) {
    _assertInitialised();
    final raw = _cacheBox.get(key);
    if (raw == null) return null;

    final map = Map<String, dynamic>.from(raw as Map<dynamic, dynamic>);
    final expiresAt = map['expiresAt'] as int;

    if (DateTime.now().millisecondsSinceEpoch > expiresAt) {
      // Expired -- remove eagerly and return nothing.
      _cacheBox.delete(key);
      return null;
    }

    return map['data'];
  }

  /// Evicts all cached API responses.
  Future<void> clearCache() async {
    _assertInitialised();
    await _cacheBox.clear();
  }

  // ─────────────────────── Teardown ─────────────────────────────

  /// Closes all open Hive boxes. Call during app shutdown if desired.
  Future<void> dispose() async {
    if (!_isInitialised) return;
    await _generalBox.close();
    await _searchHistoryBox.close();
    await _cacheBox.close();
    _isInitialised = false;
  }
}

// ─────────────────────── Key Constants ────────────────────────────────────

/// Keys used with [LocalStorageService.get] / [LocalStorageService.put].
abstract final class LocalStorageKeys {
  static const String onboardingCompleted = 'onboarding_completed';
  static const String selectedLocale = 'selected_locale';
  static const String themeMode = 'theme_mode';
  static const String lastKnownLatitude = 'last_known_lat';
  static const String lastKnownLongitude = 'last_known_lng';
  static const String lastSelectedAddressId = 'last_selected_address_id';
  static const String recentRestaurants = 'recent_restaurants';
  static const String cartData = 'cart_data';
}
