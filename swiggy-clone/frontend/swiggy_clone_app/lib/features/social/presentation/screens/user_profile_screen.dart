import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/extensions/l10n_extensions.dart';
import '../../../../routing/route_names.dart';
import '../providers/user_profile_notifier.dart';
import '../widgets/activity_feed_card.dart';
import '../widgets/follow_button_widget.dart';
import '../widgets/share_bottom_sheet.dart';

class UserProfileScreen extends ConsumerStatefulWidget {
  const UserProfileScreen({super.key, required this.userId});

  final String userId;

  @override
  ConsumerState<UserProfileScreen> createState() => _UserProfileScreenState();
}

class _UserProfileScreenState extends ConsumerState<UserProfileScreen> {
  bool _isToggling = false;

  @override
  Widget build(BuildContext context) {
    final profileState = ref.watch(userProfileNotifierProvider(widget.userId));
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: Text(context.l10n.userProfile),
        actions: [
          IconButton(
            icon: const Icon(Icons.share_outlined),
            onPressed: () => ShareBottomSheet.show(
              context,
              shareUrl: 'https://zipmeal.com/profile/${widget.userId}',
              shareText:
                  '${context.l10n.viewProfile} https://zipmeal.com/profile/${widget.userId}',
            ),
          ),
        ],
      ),
      body: switch (profileState) {
        UserProfileLoaded(:final profile) => RefreshIndicator(
            onRefresh: () => ref
                .read(userProfileNotifierProvider(widget.userId).notifier)
                .loadProfile(),
            child: ListView(
              children: [
                // Profile header
                Padding(
                  padding: const EdgeInsets.all(24),
                  child: Column(
                    children: [
                      CircleAvatar(
                        radius: 48,
                        backgroundColor:
                            AppColors.primary.withValues(alpha: 0.15),
                        backgroundImage: profile.avatarUrl != null
                            ? NetworkImage(profile.avatarUrl!)
                            : null,
                        child: profile.avatarUrl == null
                            ? Text(
                                _initials(profile.fullName),
                                style:
                                    theme.textTheme.headlineMedium?.copyWith(
                                  color: AppColors.primary,
                                  fontWeight: FontWeight.bold,
                                ),
                              )
                            : null,
                      ),
                      const SizedBox(height: 16),
                      Text(
                        profile.fullName,
                        style: theme.textTheme.titleLarge?.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                      const SizedBox(height: 16),

                      // Stats row
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                        children: [
                          _StatItem(
                            label: context.l10n.followersCount,
                            count: profile.followerCount,
                            onTap: () => context.push(
                                RouteNames.followersPath(widget.userId)),
                          ),
                          _StatItem(
                            label: context.l10n.followingCount,
                            count: profile.followingCount,
                            onTap: () => context.push(
                                RouteNames.followingPath(widget.userId)),
                          ),
                          _StatItem(
                            label: context.l10n.reviewsCount,
                            count: profile.reviewCount,
                          ),
                        ],
                      ),
                      const SizedBox(height: 16),

                      // Follow button
                      FollowButton(
                        isFollowing: profile.isFollowedByCurrentUser,
                        isLoading: _isToggling,
                        onPressed: _handleToggleFollow,
                      ),
                    ],
                  ),
                ),
                const Divider(height: 1),

                // Recent activity
                if (profile.recentActivity.isNotEmpty) ...[
                  Padding(
                    padding:
                        const EdgeInsets.fromLTRB(16, 16, 16, 8),
                    child: Text(
                      context.l10n.recentActivity,
                      style: theme.textTheme.titleMedium?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                  ...profile.recentActivity.map(
                    (item) => ActivityFeedCard(item: item),
                  ),
                  const SizedBox(height: 24),
                ],
              ],
            ),
          ),
        UserProfileState() when profileState == const UserProfileState.loading() ||
            profileState == const UserProfileState.initial() =>
          const Center(child: CircularProgressIndicator()),
        _ => Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                const Icon(Icons.error_outline, size: 48, color: Colors.grey),
                const SizedBox(height: 16),
                Text(context.l10n.somethingWentWrong),
                const SizedBox(height: 16),
                OutlinedButton(
                  onPressed: () => ref
                      .read(
                          userProfileNotifierProvider(widget.userId).notifier)
                      .loadProfile(),
                  child: Text(context.l10n.retry),
                ),
              ],
            ),
          ),
      },
    );
  }

  Future<void> _handleToggleFollow() async {
    setState(() => _isToggling = true);
    await ref
        .read(userProfileNotifierProvider(widget.userId).notifier)
        .toggleFollow();
    if (mounted) setState(() => _isToggling = false);
  }

  String _initials(String name) {
    final parts = name.trim().split(' ');
    if (parts.length >= 2) {
      return '${parts[0][0]}${parts[1][0]}'.toUpperCase();
    }
    return parts[0].isNotEmpty ? parts[0][0].toUpperCase() : '?';
  }
}

class _StatItem extends StatelessWidget {
  const _StatItem({
    required this.label,
    required this.count,
    this.onTap,
  });

  final String label;
  final int count;
  final VoidCallback? onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return GestureDetector(
      onTap: onTap,
      child: Column(
        children: [
          Text(
            '$count',
            style: theme.textTheme.titleLarge?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            label,
            style: theme.textTheme.bodySmall?.copyWith(
              color: AppColors.textSecondaryLight,
            ),
          ),
        ],
      ),
    );
  }
}
