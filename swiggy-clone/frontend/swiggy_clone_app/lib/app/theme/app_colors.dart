import 'package:flutter/material.dart';

/// Centralised color palette for the Swiggy Clone application.
///
/// Colours are derived from Swiggy's brand guidelines and extended to
/// cover surface, background, semantic, and neutral tones required by
/// Material 3.
abstract final class AppColors {
  // ─────────────────────────── Brand ───────────────────────────
  /// Primary Swiggy orange.
  static const Color primary = Color(0xFFFC8019);

  /// Darker variant used for pressed / active states.
  static const Color primaryDark = Color(0xFFE06D00);

  /// Lighter tint for containers and chips.
  static const Color primaryLight = Color(0xFFFFF3E0);

  /// On-primary content colour (text/icons sitting on the primary colour).
  static const Color onPrimary = Color(0xFFFFFFFF);

  // ─────────────────────────── Secondary ───────────────────────
  static const Color secondary = Color(0xFF60B246);
  static const Color secondaryDark = Color(0xFF3E8E2C);
  static const Color onSecondary = Color(0xFFFFFFFF);

  // ─────────────────────────── Surfaces ────────────────────────
  static const Color surfaceLight = Color(0xFFFFFFFF);
  static const Color surfaceDark = Color(0xFF1E1E1E);

  static const Color backgroundLight = Color(0xFFF5F5F5);
  static const Color backgroundDark = Color(0xFF121212);

  static const Color cardLight = Color(0xFFFFFFFF);
  static const Color cardDark = Color(0xFF2C2C2C);

  // ─────────────────────────── Text ────────────────────────────
  static const Color textPrimaryLight = Color(0xFF282C3F);
  static const Color textSecondaryLight = Color(0xFF7E808C);
  static const Color textTertiaryLight = Color(0xFF93959F);
  static const Color textDisabledLight = Color(0xFFBDBDBD);

  static const Color textPrimaryDark = Color(0xFFE0E0E0);
  static const Color textSecondaryDark = Color(0xFFAAAAAA);
  static const Color textTertiaryDark = Color(0xFF888888);
  static const Color textDisabledDark = Color(0xFF555555);

  // ─────────────────────────── Semantic ────────────────────────
  static const Color error = Color(0xFFE23744);
  static const Color onError = Color(0xFFFFFFFF);
  static const Color success = Color(0xFF60B246);
  static const Color warning = Color(0xFFFFC107);
  static const Color info = Color(0xFF2196F3);

  // ─────────────────────────── Borders / Dividers ──────────────
  static const Color dividerLight = Color(0xFFE8E8E8);
  static const Color dividerDark = Color(0xFF3A3A3A);
  static const Color borderLight = Color(0xFFD4D5D9);
  static const Color borderDark = Color(0xFF444444);

  // ─────────────────────────── Misc ────────────────────────────
  static const Color shimmerBase = Color(0xFFE0E0E0);
  static const Color shimmerHighlight = Color(0xFFF5F5F5);
  static const Color shadow = Color(0x1A000000);
  static const Color scrim = Color(0x80000000);
  static const Color rating = Color(0xFF48C479);
}
