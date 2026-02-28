import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/notification_model.dart';
import '../providers/notifications_notifier.dart';
import '../providers/notifications_state.dart';
import '../providers/unread_count_notifier.dart';

class NotificationsScreen extends ConsumerWidget {
  const NotificationsScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(notificationsNotifierProvider);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Notifications'),
        actions: [
          if (state is NotificationsLoaded &&
              state.notifications.any((n) => !n.isRead))
            TextButton(
              onPressed: () {
                ref
                    .read(notificationsNotifierProvider.notifier)
                    .markAllAsRead();
                ref.read(unreadCountNotifierProvider.notifier).reset();
              },
              child: Text(
                'Mark all read',
                style: TextStyle(
                  color: AppColors.primary,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ),
        ],
      ),
      body: switch (state) {
        NotificationsInitial() || NotificationsLoading() =>
          const AppLoadingWidget(message: 'Loading notifications...'),
        NotificationsError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(notificationsNotifierProvider.notifier)
                .loadNotifications(),
          ),
        NotificationsLoaded(:final notifications, :final isLoadingMore) =>
          notifications.isEmpty
              ? _EmptyState()
              : RefreshIndicator(
                  color: AppColors.primary,
                  onRefresh: () async {
                    ref
                        .read(notificationsNotifierProvider.notifier)
                        .loadNotifications();
                    ref
                        .read(unreadCountNotifierProvider.notifier)
                        .fetchCount();
                  },
                  child: NotificationListener<ScrollNotification>(
                    onNotification: (scrollInfo) {
                      if (scrollInfo.metrics.pixels >=
                          scrollInfo.metrics.maxScrollExtent - 200) {
                        ref
                            .read(notificationsNotifierProvider.notifier)
                            .loadMore();
                      }
                      return false;
                    },
                    child: ListView.builder(
                      itemCount:
                          notifications.length + (isLoadingMore ? 1 : 0),
                      itemBuilder: (context, index) {
                        if (index == notifications.length) {
                          return const Padding(
                            padding: EdgeInsets.all(16),
                            child:
                                Center(child: CircularProgressIndicator()),
                          );
                        }
                        return _NotificationTile(
                          notification: notifications[index],
                          onTap: () => _onTap(context, ref, notifications[index]),
                        );
                      },
                    ),
                  ),
                ),
      },
    );
  }

  void _onTap(
      BuildContext context, WidgetRef ref, NotificationModel notification) {
    if (!notification.isRead) {
      ref
          .read(notificationsNotifierProvider.notifier)
          .markAsRead(notification.id);
      ref.read(unreadCountNotifierProvider.notifier).decrement();
    }

    // Navigate based on notification data
    if (notification.data != null) {
      try {
        final data = jsonDecode(notification.data!) as Map<String, dynamic>;
        final orderId = data['orderId'] as String?;
        if (orderId != null) {
          if (notification.type == 1) {
            // OrderUpdate → tracking screen
            context.push(RouteNames.orderTrackingPath(orderId));
          } else {
            context.push(RouteNames.orderDetailPath(orderId));
          }
          return;
        }
      } catch (_) {}
    }
  }
}

class _EmptyState extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Center(
      child: Column(
        mainAxisAlignment: MainAxisAlignment.center,
        children: [
          Icon(Icons.notifications_none_rounded,
              size: 64, color: AppColors.textTertiaryLight),
          const SizedBox(height: 16),
          Text(
            'No notifications yet',
            style: theme.textTheme.titleMedium?.copyWith(
              color: AppColors.textSecondaryLight,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            'We\'ll let you know when something arrives',
            style: theme.textTheme.bodySmall?.copyWith(
              color: AppColors.textTertiaryLight,
            ),
          ),
        ],
      ),
    );
  }
}

class _NotificationTile extends StatelessWidget {
  const _NotificationTile({
    required this.notification,
    required this.onTap,
  });

  final NotificationModel notification;
  final VoidCallback onTap;

  static const _typeIcons = {
    1: Icons.receipt_long_outlined, // OrderUpdate
    2: Icons.local_offer_outlined, // Promotion
    3: Icons.restaurant_outlined, // DineIn
    4: Icons.info_outline, // System
  };

  static const _typeColors = {
    1: AppColors.primary,
    2: Colors.purple,
    3: Colors.teal,
    4: AppColors.info,
  };

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final icon = _typeIcons[notification.type] ?? Icons.notifications_outlined;
    final iconColor = _typeColors[notification.type] ?? AppColors.info;

    return InkWell(
      onTap: onTap,
      child: Container(
        color: notification.isRead ? null : AppColors.primaryLight,
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
        child: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Container(
              width: 40,
              height: 40,
              decoration: BoxDecoration(
                color: iconColor.withValues(alpha: 0.12),
                shape: BoxShape.circle,
              ),
              child: Icon(icon, color: iconColor, size: 20),
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    notification.title,
                    style: theme.textTheme.bodyMedium?.copyWith(
                      fontWeight:
                          notification.isRead ? FontWeight.normal : FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 2),
                  Text(
                    notification.body,
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textSecondaryLight,
                    ),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                  const SizedBox(height: 4),
                  Text(
                    _formatTimeAgo(notification.createdAt),
                    style: theme.textTheme.labelSmall?.copyWith(
                      color: AppColors.textTertiaryLight,
                    ),
                  ),
                ],
              ),
            ),
            if (!notification.isRead)
              Padding(
                padding: const EdgeInsets.only(top: 4, left: 8),
                child: Container(
                  width: 8,
                  height: 8,
                  decoration: const BoxDecoration(
                    color: AppColors.primary,
                    shape: BoxShape.circle,
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }

  String _formatTimeAgo(String isoDate) {
    try {
      final date = DateTime.parse(isoDate);
      final now = DateTime.now();
      final diff = now.difference(date);

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
