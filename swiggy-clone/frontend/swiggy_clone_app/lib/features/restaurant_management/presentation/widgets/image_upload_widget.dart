import 'package:cached_network_image/cached_network_image.dart';
import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';

/// A widget showing current image (via CachedNetworkImage or placeholder)
/// with an overlay camera button. Tapping invokes [onPickImage].
class ImageUploadWidget extends StatelessWidget {
  const ImageUploadWidget({
    required this.label,
    required this.onPickImage,
    this.currentUrl,
    this.isUploading = false,
    this.height = 160,
    super.key,
  });

  /// Label displayed on the placeholder when no image is present.
  final String label;

  /// Current image URL. When null or empty, shows a placeholder.
  final String? currentUrl;

  /// Callback invoked when the user taps the upload/camera button.
  final VoidCallback onPickImage;

  /// Whether an upload is currently in progress.
  final bool isUploading;

  /// Height of the widget.
  final double height;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final hasImage = currentUrl != null && currentUrl!.isNotEmpty;

    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: theme.textTheme.titleSmall?.copyWith(
            fontWeight: FontWeight.w600,
          ),
        ),
        const SizedBox(height: 8),
        Stack(
          children: [
            // Image or placeholder
            ClipRRect(
              borderRadius: BorderRadius.circular(12),
              child: hasImage
                  ? CachedNetworkImage(
                      imageUrl: currentUrl!,
                      width: double.infinity,
                      height: height,
                      fit: BoxFit.cover,
                      placeholder: (_, __) => Container(
                        width: double.infinity,
                        height: height,
                        color: AppColors.shimmerBase,
                        child: const Center(
                          child: CircularProgressIndicator(
                            color: AppColors.primary,
                          ),
                        ),
                      ),
                      errorWidget: (_, __, ___) => _Placeholder(
                        height: height,
                        label: label,
                      ),
                    )
                  : _Placeholder(height: height, label: label),
            ),

            // Upload overlay button
            Positioned(
              bottom: 8,
              right: 8,
              child: Material(
                color: AppColors.primary,
                borderRadius: BorderRadius.circular(24),
                elevation: 2,
                child: InkWell(
                  onTap: isUploading ? null : onPickImage,
                  borderRadius: BorderRadius.circular(24),
                  child: Padding(
                    padding: const EdgeInsets.all(10),
                    child: isUploading
                        ? const SizedBox(
                            width: 20,
                            height: 20,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              color: Colors.white,
                            ),
                          )
                        : const Icon(
                            Icons.camera_alt_rounded,
                            color: Colors.white,
                            size: 20,
                          ),
                  ),
                ),
              ),
            ),
          ],
        ),
      ],
    );
  }
}

class _Placeholder extends StatelessWidget {
  const _Placeholder({required this.height, required this.label});

  final double height;
  final String label;

  @override
  Widget build(BuildContext context) {
    return Container(
      width: double.infinity,
      height: height,
      decoration: BoxDecoration(
        color: AppColors.backgroundLight,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: AppColors.borderLight,
          style: BorderStyle.solid,
        ),
      ),
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(
            Icons.cloud_upload_outlined,
            size: 40,
            color: AppColors.textTertiaryLight,
          ),
          const SizedBox(height: 8),
          Text(
            'Upload $label',
            style: Theme.of(context).textTheme.bodySmall?.copyWith(
                  color: AppColors.textTertiaryLight,
                ),
          ),
        ],
      ),
    );
  }
}
