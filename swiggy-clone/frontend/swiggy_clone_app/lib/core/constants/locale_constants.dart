import 'package:flutter/material.dart';

/// Supported locales and their display metadata.
abstract final class LocaleConstants {
  /// All locales the app supports.
  static const List<Locale> supportedLocales = [
    Locale('en'),
    Locale('hi'),
  ];

  /// Human-readable display names for each locale (in English).
  static const Map<String, String> localeDisplayNames = {
    'en': 'English',
    'hi': 'Hindi',
  };

  /// Native-script display names for each locale.
  static const Map<String, String> localeNativeNames = {
    'en': 'English',
    'hi': 'हिन्दी',
  };
}
