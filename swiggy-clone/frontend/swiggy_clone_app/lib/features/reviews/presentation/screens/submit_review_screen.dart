import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:image_picker/image_picker.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/repositories/review_repository.dart';
import '../providers/review_submit_notifier.dart';
import '../providers/review_submit_state.dart';
import '../widgets/review_photo_picker.dart';

class SubmitReviewScreen extends ConsumerStatefulWidget {
  const SubmitReviewScreen({
    required this.orderId,
    required this.restaurantName,
    super.key,
  });

  final String orderId;
  final String restaurantName;

  @override
  ConsumerState<SubmitReviewScreen> createState() => _SubmitReviewScreenState();
}

class _SubmitReviewScreenState extends ConsumerState<SubmitReviewScreen> {
  int _rating = 0;
  int _deliveryRating = 0;
  bool _isAnonymous = false;
  final _reviewController = TextEditingController();
  final List<XFile> _selectedPhotos = [];
  bool _isUploading = false;

  @override
  void dispose() {
    _reviewController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(reviewSubmitNotifierProvider);
    final theme = Theme.of(context);

    ref.listen<ReviewSubmitState>(reviewSubmitNotifierProvider, (prev, next) {
      if (next is ReviewSubmitSubmitted) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Review submitted successfully!')),
        );
        context.pop(true);
      }
    });

    return Scaffold(
      appBar: AppBar(title: const Text('Write a Review')),
      body: switch (state) {
        ReviewSubmitSubmitting() =>
          const AppLoadingWidget(message: 'Submitting review...'),
        _ => ListView(
            padding: const EdgeInsets.all(16),
            children: [
              Text(
                widget.restaurantName,
                style: theme.textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 24),

              // Food rating
              Text(
                'How was the food?',
                style: theme.textTheme.titleSmall?.copyWith(
                  fontWeight: FontWeight.w600,
                ),
              ),
              const SizedBox(height: 8),
              _StarRatingRow(
                rating: _rating,
                onChanged: (value) => setState(() => _rating = value),
              ),

              const SizedBox(height: 20),

              // Delivery rating
              Text(
                'How was the delivery?',
                style: theme.textTheme.titleSmall?.copyWith(
                  fontWeight: FontWeight.w600,
                ),
              ),
              const SizedBox(height: 8),
              _StarRatingRow(
                rating: _deliveryRating,
                onChanged: (value) =>
                    setState(() => _deliveryRating = value),
              ),

              const SizedBox(height: 20),

              // Review text
              TextField(
                controller: _reviewController,
                maxLines: 5,
                maxLength: 2000,
                decoration: const InputDecoration(
                  hintText: 'Share your experience (optional)',
                  border: OutlineInputBorder(),
                ),
              ),

              const SizedBox(height: 16),
              ReviewPhotoPicker(
                photos: _selectedPhotos,
                onAdd: (file) => setState(() => _selectedPhotos.add(file)),
                onRemove: (index) =>
                    setState(() => _selectedPhotos.removeAt(index)),
              ),

              const SizedBox(height: 16),

              // Anonymous toggle
              SwitchListTile(
                title: const Text('Post anonymously'),
                subtitle: const Text('Your name will be hidden'),
                value: _isAnonymous,
                onChanged: (value) =>
                    setState(() => _isAnonymous = value),
                contentPadding: EdgeInsets.zero,
                activeColor: AppColors.primary,
              ),

              if (state is ReviewSubmitError) ...[
                const SizedBox(height: 16),
                Container(
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: AppColors.error.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Text(
                    state.failure.message,
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.error,
                    ),
                  ),
                ),
              ],
            ],
          ),
      },
      bottomNavigationBar: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: FilledButton(
            onPressed:
                _rating == 0 || state is ReviewSubmitSubmitting || _isUploading
                    ? null
                    : _submit,
            style: FilledButton.styleFrom(
              backgroundColor: AppColors.primary,
              minimumSize: const Size.fromHeight(52),
            ),
            child: const Text(
              'Submit Review',
              style: TextStyle(fontWeight: FontWeight.bold, fontSize: 16),
            ),
          ),
        ),
      ),
    );
  }

  Future<void> _submit() async {
    final List<String> photoUrls = [];

    if (_selectedPhotos.isNotEmpty) {
      setState(() => _isUploading = true);
      final repository = ref.read(reviewRepositoryProvider);
      for (final photo in _selectedPhotos) {
        final result = await repository.uploadReviewPhoto(
          filePath: photo.path,
          fileName: photo.name,
        );
        if (result.failure != null) {
          setState(() => _isUploading = false);
          if (mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(
                  content: Text(
                      'Failed to upload photo: ${result.failure!.message}')),
            );
          }
          return;
        }
        photoUrls.add(result.data!);
      }
      setState(() => _isUploading = false);
    }

    ref.read(reviewSubmitNotifierProvider.notifier).submitReview(
          orderId: widget.orderId,
          rating: _rating,
          reviewText: _reviewController.text.isNotEmpty
              ? _reviewController.text
              : null,
          deliveryRating: _deliveryRating > 0 ? _deliveryRating : null,
          isAnonymous: _isAnonymous,
          photoUrls: photoUrls,
        );
  }
}

class _StarRatingRow extends StatelessWidget {
  const _StarRatingRow({
    required this.rating,
    required this.onChanged,
  });

  final int rating;
  final ValueChanged<int> onChanged;

  @override
  Widget build(BuildContext context) => Row(
        children: List.generate(5, (index) {
          final starValue = index + 1;
          return GestureDetector(
            onTap: () => onChanged(starValue),
            child: Padding(
              padding: const EdgeInsets.only(right: 8),
              child: Icon(
                starValue <= rating ? Icons.star : Icons.star_border,
                size: 36,
                color: starValue <= rating
                    ? AppColors.rating
                    : AppColors.textTertiaryLight,
              ),
            ),
          );
        }),
      );
}
