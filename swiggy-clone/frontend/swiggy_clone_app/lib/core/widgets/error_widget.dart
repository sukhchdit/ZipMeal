import 'package:flutter/material.dart';

import '../../app/theme/app_colors.dart';
import '../constants/app_strings.dart';
import '../errors/failures.dart';

/// A reusable error display with an optional retry callback.
///
/// Adapts its messaging and icon depending on the [Failure] subtype it
/// receives, while also supporting a plain [message] string for simple cases.
///
/// ```dart
/// AppErrorWidget(
///   failure: NetworkFailure(),
///   onRetry: () => ref.invalidate(restaurantsProvider),
/// )
/// ```
class AppErrorWidget extends StatelessWidget {
  const AppErrorWidget({
    super.key,
    this.failure,
    this.message,
    this.onRetry,
    this.compact = false,
  }) : assert(
          failure != null || message != null,
          'Either failure or message must be provided.',
        );

  /// Typed failure from which the widget derives icon and text.
  final Failure? failure;

  /// Plain text fallback when no [Failure] object is available.
  final String? message;

  /// Called when the user taps the retry button. When `null` the button is
  /// hidden.
  final VoidCallback? onRetry;

  /// When `true` renders a compact inline variant instead of the
  /// centred full-size layout.
  final bool compact;

  @override
  Widget build(BuildContext context) {
    final effectiveMessage = message ?? _messageFromFailure(failure!);
    final icon = _iconFromFailure(failure);

    if (compact) {
      return _CompactError(
        icon: icon,
        message: effectiveMessage,
        onRetry: onRetry,
      );
    }

    return Center(
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 32),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(icon, size: 64, color: AppColors.textTertiaryLight),
            const SizedBox(height: 16),
            Text(
              _titleFromFailure(failure),
              style: Theme.of(context).textTheme.headlineSmall,
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 8),
            Text(
              effectiveMessage,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    color: AppColors.textSecondaryLight,
                  ),
              textAlign: TextAlign.center,
            ),
            if (onRetry != null) ...[
              const SizedBox(height: 24),
              SizedBox(
                width: 200,
                child: ElevatedButton.icon(
                  onPressed: onRetry,
                  icon: const Icon(Icons.refresh_rounded, size: 20),
                  label: const Text(AppStrings.retry),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }

  // ──────────────────── Helpers ─────────────────────────────────

  static String _titleFromFailure(Failure? failure) => switch (failure) {
        NetworkFailure() => AppStrings.noInternetConnection,
        ServerFailure() => AppStrings.somethingWentWrong,
        AuthFailure() => AppStrings.sessionExpired,
        _ => AppStrings.somethingWentWrong,
      };

  static String _messageFromFailure(Failure failure) => switch (failure) {
        NetworkFailure() => AppStrings.checkConnection,
        ServerFailure(:final message) => message,
        AuthFailure() => AppStrings.sessionExpired,
        CacheFailure(:final message) => message,
        ValidationFailure(:final message) => message,
      };

  static IconData _iconFromFailure(Failure? failure) => switch (failure) {
        NetworkFailure() => Icons.wifi_off_rounded,
        ServerFailure() => Icons.cloud_off_rounded,
        AuthFailure() => Icons.lock_outline_rounded,
        CacheFailure() => Icons.storage_rounded,
        _ => Icons.error_outline_rounded,
      };
}

/// Compact inline error row for use inside lists or smaller containers.
class _CompactError extends StatelessWidget {
  const _CompactError({
    required this.icon,
    required this.message,
    this.onRetry,
  });

  final IconData icon;
  final String message;
  final VoidCallback? onRetry;

  @override
  Widget build(BuildContext context) => Padding(
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
        child: Row(
          children: [
            Icon(icon, size: 24, color: AppColors.error),
            const SizedBox(width: 12),
            Expanded(
              child: Text(
                message,
                style: Theme.of(context).textTheme.bodySmall?.copyWith(
                      color: AppColors.textSecondaryLight,
                    ),
              ),
            ),
            if (onRetry != null)
              TextButton(
                onPressed: onRetry,
                child: const Text(AppStrings.retry),
              ),
          ],
        ),
      );
}
