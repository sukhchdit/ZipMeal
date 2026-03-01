import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/extensions/l10n_extensions.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/activity_feed_item_model.dart';

class ActivityFeedCard extends StatelessWidget {
  const ActivityFeedCard({super.key, required this.item});

  final ActivityFeedItemModel item;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final metadata = _parseMetadata(item.metadata);

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      child: InkWell(
        borderRadius: BorderRadius.circular(12),
        onTap: () => _onTap(context),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              GestureDetector(
                onTap: () =>
                    context.push(RouteNames.userProfilePath(item.userId)),
                child: CircleAvatar(
                  radius: 22,
                  backgroundColor:
                      AppColors.primary.withValues(alpha: 0.15),
                  backgroundImage: item.userAvatarUrl != null
                      ? NetworkImage(item.userAvatarUrl!)
                      : null,
                  child: item.userAvatarUrl == null
                      ? Text(
                          _initials(item.userName),
                          style: theme.textTheme.bodyMedium?.copyWith(
                            color: AppColors.primary,
                            fontWeight: FontWeight.bold,
                          ),
                        )
                      : null,
                ),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    RichText(
                      text: TextSpan(
                        style: theme.textTheme.bodyMedium,
                        children: [
                          TextSpan(
                            text: item.userName,
                            style: const TextStyle(
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                          TextSpan(
                            text: ' ${_activityDescription(context, metadata)}',
                          ),
                        ],
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      _formatTime(item.createdAt),
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppColors.textSecondaryLight,
                      ),
                    ),
                  ],
                ),
              ),
              const SizedBox(width: 8),
              Icon(
                _activityIcon,
                size: 20,
                color: AppColors.textSecondaryLight,
              ),
            ],
          ),
        ),
      ),
    );
  }

  Map<String, dynamic> _parseMetadata(String? metadata) {
    if (metadata == null || metadata.isEmpty) return {};
    try {
      return jsonDecode(metadata) as Map<String, dynamic>;
    } catch (_) {
      return {};
    }
  }

  String _activityDescription(
      BuildContext context, Map<String, dynamic> metadata) {
    switch (item.activityType) {
      case 'ReviewSubmitted':
        return context.l10n.reviewedRestaurant;
      case 'RestaurantFavourited':
        final name = metadata['RestaurantName'] as String? ?? '';
        return '${context.l10n.favouritedRestaurant}${name.isNotEmpty ? ' $name' : ''}';
      case 'OrderPlaced':
        return context.l10n.placedOrder;
      case 'UserFollowed':
        final name = metadata['FollowingName'] as String? ?? '';
        return '${context.l10n.startedFollowing}${name.isNotEmpty ? ' $name' : ''}';
      default:
        return '';
    }
  }

  IconData get _activityIcon {
    switch (item.activityType) {
      case 'ReviewSubmitted':
        return Icons.rate_review_outlined;
      case 'RestaurantFavourited':
        return Icons.favorite_outline;
      case 'OrderPlaced':
        return Icons.receipt_long_outlined;
      case 'UserFollowed':
        return Icons.person_add_outlined;
      default:
        return Icons.circle_outlined;
    }
  }

  void _onTap(BuildContext context) {
    if (item.targetEntityId == null) return;
    switch (item.activityType) {
      case 'ReviewSubmitted':
      case 'RestaurantFavourited':
        context.push(
            RouteNames.restaurantDetailPath(item.targetEntityId!));
        break;
      case 'UserFollowed':
        context.push(RouteNames.userProfilePath(item.targetEntityId!));
        break;
      default:
        break;
    }
  }

  String _initials(String name) {
    final parts = name.trim().split(' ');
    if (parts.length >= 2) {
      return '${parts[0][0]}${parts[1][0]}'.toUpperCase();
    }
    return parts[0].isNotEmpty ? parts[0][0].toUpperCase() : '?';
  }

  String _formatTime(String dateStr) {
    try {
      final date = DateTime.parse(dateStr);
      final diff = DateTime.now().difference(date);
      if (diff.inMinutes < 1) return 'Just now';
      if (diff.inMinutes < 60) return '${diff.inMinutes}m ago';
      if (diff.inHours < 24) return '${diff.inHours}h ago';
      if (diff.inDays < 7) return '${diff.inDays}d ago';
      return '${date.day}/${date.month}/${date.year}';
    } catch (_) {
      return '';
    }
  }
}
