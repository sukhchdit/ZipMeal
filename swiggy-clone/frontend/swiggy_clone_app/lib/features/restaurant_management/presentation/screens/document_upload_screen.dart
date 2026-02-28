import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:image_picker/image_picker.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/repositories/restaurant_repository.dart';
import '../providers/restaurant_detail_notifier.dart';
import '../providers/restaurant_detail_state.dart';

/// Screen for uploading restaurant documents and images:
/// logo, banner, FSSAI license, and GST certificate.
///
/// Uses [ImagePicker] for file selection and the restaurant upload endpoint.
class DocumentUploadScreen extends ConsumerStatefulWidget {
  const DocumentUploadScreen({
    required this.restaurantId,
    super.key,
  });

  final String restaurantId;

  @override
  ConsumerState<DocumentUploadScreen> createState() =>
      _DocumentUploadScreenState();
}

class _DocumentUploadScreenState extends ConsumerState<DocumentUploadScreen> {
  final _picker = ImagePicker();
  final Map<String, bool> _uploading = {};

  static const _uploadTypes = <_UploadTypeInfo>[
    _UploadTypeInfo(
      key: 'logo',
      title: 'Restaurant Logo',
      subtitle: 'Square image, recommended 512x512',
      icon: Icons.image_outlined,
    ),
    _UploadTypeInfo(
      key: 'banner',
      title: 'Banner Image',
      subtitle: 'Wide image for header, recommended 1200x400',
      icon: Icons.panorama_outlined,
    ),
    _UploadTypeInfo(
      key: 'fssai',
      title: 'FSSAI License',
      subtitle: 'FSSAI food safety license document',
      icon: Icons.verified_outlined,
    ),
    _UploadTypeInfo(
      key: 'gst',
      title: 'GST Certificate',
      subtitle: 'GST registration certificate',
      icon: Icons.description_outlined,
    ),
  ];

  Future<void> _pickAndUpload(String fileType) async {
    final source = await showModalBottomSheet<ImageSource>(
      context: context,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (ctx) => SafeArea(
        child: Padding(
          padding: const EdgeInsets.symmetric(vertical: 16),
          child: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Text(
                'Choose Source',
                style: Theme.of(ctx).textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
              ),
              const SizedBox(height: 12),
              ListTile(
                leading: const Icon(Icons.camera_alt_outlined),
                title: const Text('Camera'),
                onTap: () => Navigator.of(ctx).pop(ImageSource.camera),
              ),
              ListTile(
                leading: const Icon(Icons.photo_library_outlined),
                title: const Text('Gallery'),
                onTap: () => Navigator.of(ctx).pop(ImageSource.gallery),
              ),
            ],
          ),
        ),
      ),
    );

    if (source == null || !mounted) return;

    final file = await _picker.pickImage(
      source: source,
      maxWidth: 2048,
      maxHeight: 2048,
      imageQuality: 85,
    );
    if (file == null || !mounted) return;

    setState(() => _uploading[fileType] = true);

    final repository = ref.read(restaurantRepositoryProvider);
    final result = await repository.uploadFile(
      id: widget.restaurantId,
      fileType: fileType,
      filePath: file.path,
      fileName: file.name,
    );

    if (!mounted) return;
    setState(() => _uploading[fileType] = false);

    if (result.failure != null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Upload failed. Please try again.'),
          backgroundColor: Colors.red,
        ),
      );
    } else {
      // Reload restaurant detail to reflect new image URLs
      ref
          .read(restaurantDetailNotifierProvider(widget.restaurantId).notifier)
          .loadRestaurant();
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('${_displayName(fileType)} uploaded successfully!'),
          backgroundColor: AppColors.success,
        ),
      );
    }
  }

  String _displayName(String fileType) {
    switch (fileType) {
      case 'logo':
        return 'Logo';
      case 'banner':
        return 'Banner';
      case 'fssai':
        return 'FSSAI License';
      case 'gst':
        return 'GST Certificate';
      default:
        return fileType;
    }
  }

  String? _currentUrl(String fileType, RestaurantDetailState detailState) {
    if (detailState is! RestaurantDetailLoaded) return null;
    final r = detailState.restaurant;
    switch (fileType) {
      case 'logo':
        return r.logoUrl;
      case 'banner':
        return r.bannerUrl;
      case 'fssai':
        return r.fssaiLicense;
      case 'gst':
        return r.gstNumber;
      default:
        return null;
    }
  }

  @override
  Widget build(BuildContext context) {
    final detailState =
        ref.watch(restaurantDetailNotifierProvider(widget.restaurantId));
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Documents & Images'),
      ),
      body: ListView.separated(
        padding: const EdgeInsets.all(16),
        itemCount: _uploadTypes.length,
        separatorBuilder: (_, __) => const SizedBox(height: 12),
        itemBuilder: (context, index) {
          final type = _uploadTypes[index];
          final isUploading = _uploading[type.key] ?? false;
          final url = _currentUrl(type.key, detailState);

          return Card(
            shape:
                RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Row(
                    children: [
                      Container(
                        padding: const EdgeInsets.all(10),
                        decoration: BoxDecoration(
                          color: AppColors.primaryLight,
                          borderRadius: BorderRadius.circular(10),
                        ),
                        child: Icon(type.icon, color: AppColors.primary),
                      ),
                      const SizedBox(width: 12),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              type.title,
                              style: theme.textTheme.titleSmall?.copyWith(
                                fontWeight: FontWeight.w600,
                              ),
                            ),
                            const SizedBox(height: 2),
                            Text(
                              type.subtitle,
                              style: theme.textTheme.bodySmall?.copyWith(
                                color: AppColors.textSecondaryLight,
                              ),
                            ),
                          ],
                        ),
                      ),
                    ],
                  ),
                  const SizedBox(height: 12),

                  // Status row
                  Row(
                    children: [
                      Icon(
                        url != null && url.isNotEmpty
                            ? Icons.check_circle
                            : Icons.cancel_outlined,
                        size: 16,
                        color: url != null && url.isNotEmpty
                            ? AppColors.success
                            : AppColors.textTertiaryLight,
                      ),
                      const SizedBox(width: 6),
                      Expanded(
                        child: Text(
                          url != null && url.isNotEmpty
                              ? 'Uploaded'
                              : 'Not uploaded',
                          style: theme.textTheme.bodySmall?.copyWith(
                            color: url != null && url.isNotEmpty
                                ? AppColors.success
                                : AppColors.textTertiaryLight,
                          ),
                        ),
                      ),
                      if (isUploading)
                        const SizedBox(
                          height: 20,
                          width: 20,
                          child: CircularProgressIndicator(
                            strokeWidth: 2,
                            color: AppColors.primary,
                          ),
                        )
                      else
                        OutlinedButton.icon(
                          onPressed: () => _pickAndUpload(type.key),
                          icon: Icon(
                            url != null && url.isNotEmpty
                                ? Icons.refresh
                                : Icons.upload_outlined,
                            size: 18,
                          ),
                          label: Text(
                            url != null && url.isNotEmpty
                                ? 'Replace'
                                : 'Upload',
                          ),
                          style: OutlinedButton.styleFrom(
                            foregroundColor: AppColors.primary,
                            side: const BorderSide(color: AppColors.primary),
                          ),
                        ),
                    ],
                  ),
                ],
              ),
            ),
          );
        },
      ),
    );
  }
}

class _UploadTypeInfo {
  const _UploadTypeInfo({
    required this.key,
    required this.title,
    required this.subtitle,
    required this.icon,
  });

  final String key;
  final String title;
  final String subtitle;
  final IconData icon;
}
