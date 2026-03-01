import 'package:flutter/material.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../storage/local_storage_service.dart';

part 'locale_notifier.g.dart';

/// Manages the user's preferred locale, persisted via [LocalStorageService].
///
/// On first launch the default locale is English (`en`). Changing locale
/// via [setLocale] immediately updates the UI and persists the choice so
/// it survives app restarts.
@Riverpod(keepAlive: true)
class LocaleNotifier extends _$LocaleNotifier {
  @override
  Locale build() {
    final storage = LocalStorageService.instance;
    final stored =
        storage.get<String>(LocalStorageKeys.selectedLocale) ?? 'en';
    return Locale(stored);
  }

  /// Switches the app locale and persists the choice.
  Future<void> setLocale(Locale locale) async {
    final storage = LocalStorageService.instance;
    await storage.put(LocalStorageKeys.selectedLocale, locale.languageCode);
    state = locale;
  }
}
