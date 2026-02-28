import 'package:flutter/material.dart';
import 'package:shimmer/shimmer.dart';

import '../../app/theme/app_colors.dart';

/// A reusable shimmer-based loading placeholder.
///
/// Use [AppLoadingWidget] when you need a full-screen or section-level
/// loading indicator (e.g. while a network request is in-flight).
///
/// For inline skeleton loaders (e.g. restaurant cards) use
/// [ShimmerBox] directly.
class AppLoadingWidget extends StatelessWidget {
  const AppLoadingWidget({
    super.key,
    this.message,
    this.itemCount = 3,
    this.useShimmer = true,
  });

  /// Optional message displayed beneath the shimmer effect.
  final String? message;

  /// Number of shimmer placeholder rows to show.
  final int itemCount;

  /// When `false`, shows a simple [CircularProgressIndicator] instead
  /// of the shimmer skeleton.
  final bool useShimmer;

  @override
  Widget build(BuildContext context) {
    if (!useShimmer) {
      return Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            const CircularProgressIndicator(color: AppColors.primary),
            if (message != null) ...[
              const SizedBox(height: 16),
              Text(
                message!,
                style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                      color: AppColors.textSecondaryLight,
                    ),
              ),
            ],
          ],
        ),
      );
    }

    return Padding(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: List.generate(
          itemCount,
          (index) => Padding(
            padding: const EdgeInsets.only(bottom: 16),
            child: _ShimmerCard(index: index),
          ),
        ),
      ),
    );
  }
}

/// A single shimmer skeleton card that mimics a typical list-item layout.
class _ShimmerCard extends StatelessWidget {
  const _ShimmerCard({required this.index});

  final int index;

  @override
  Widget build(BuildContext context) => Shimmer.fromColors(
        baseColor: AppColors.shimmerBase,
        highlightColor: AppColors.shimmerHighlight,
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Thumbnail placeholder
            Container(
              width: 80,
              height: 80,
              decoration: BoxDecoration(
                color: Colors.white,
                borderRadius: BorderRadius.circular(12),
              ),
            ),
            const SizedBox(width: 12),
            // Text line placeholders
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const ShimmerBox(width: double.infinity, height: 16),
                  const SizedBox(height: 8),
                  ShimmerBox(width: index.isEven ? 200 : 150, height: 12),
                  const SizedBox(height: 8),
                  const ShimmerBox(width: 100, height: 12),
                ],
              ),
            ),
          ],
        ),
      );
}

/// A simple rectangular shimmer placeholder.
///
/// Wrap this inside a [Shimmer.fromColors] if used standalone; when placed
/// inside [AppLoadingWidget] the parent already provides the shimmer effect.
class ShimmerBox extends StatelessWidget {
  const ShimmerBox({
    required this.width,
    required this.height,
    super.key,
    this.borderRadius = 8,
  });

  final double width;
  final double height;
  final double borderRadius;

  @override
  Widget build(BuildContext context) => Container(
        width: width,
        height: height,
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(borderRadius),
        ),
      );
}

/// A full-screen loading overlay useful for blocking interactions during
/// async operations such as placing an order or processing payment.
class FullScreenLoader extends StatelessWidget {
  const FullScreenLoader({
    super.key,
    this.message,
  });

  final String? message;

  @override
  Widget build(BuildContext context) => ColoredBox(
        color: AppColors.scrim,
        child: Center(
          child: Card(
            elevation: 4,
            shape: RoundedRectangleBorder(
              borderRadius: BorderRadius.circular(16),
            ),
            child: Padding(
              padding: const EdgeInsets.symmetric(
                horizontal: 32,
                vertical: 24,
              ),
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  const CircularProgressIndicator(color: AppColors.primary),
                  if (message != null) ...[
                    const SizedBox(height: 16),
                    Text(
                      message!,
                      style: Theme.of(context).textTheme.bodyMedium,
                      textAlign: TextAlign.center,
                    ),
                  ],
                ],
              ),
            ),
          ),
        ),
      );
}
