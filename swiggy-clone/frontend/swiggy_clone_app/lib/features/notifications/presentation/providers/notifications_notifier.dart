import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/notification_repository.dart';
import 'notifications_state.dart';

part 'notifications_notifier.g.dart';

@riverpod
class NotificationsNotifier extends _$NotificationsNotifier {
  late NotificationRepository _repository;

  @override
  NotificationsState build() {
    _repository = ref.watch(notificationRepositoryProvider);
    loadNotifications();
    return const NotificationsState.initial();
  }

  Future<void> loadNotifications() async {
    state = const NotificationsState.loading();
    final result = await _repository.getMyNotifications();
    if (result.failure != null) {
      state = NotificationsState.error(failure: result.failure!);
    } else if (result.items!.isEmpty) {
      state = const NotificationsState.loaded(
        notifications: [],
        hasMore: false,
        nextCursor: null,
      );
    } else {
      state = NotificationsState.loaded(
        notifications: result.items!,
        hasMore: result.hasMore ?? false,
        nextCursor: result.nextCursor,
      );
    }
  }

  Future<void> loadMore() async {
    final current = state;
    if (current is! NotificationsLoaded ||
        !current.hasMore ||
        current.isLoadingMore) {
      return;
    }

    state = current.copyWith(isLoadingMore: true);
    final result =
        await _repository.getMyNotifications(cursor: current.nextCursor);

    if (result.failure != null) {
      state = current.copyWith(isLoadingMore: false);
      return;
    }

    state = NotificationsLoaded(
      notifications: [...current.notifications, ...result.items!],
      hasMore: result.hasMore ?? false,
      nextCursor: result.nextCursor,
    );
  }

  Future<void> markAsRead(String id) async {
    final failure = await _repository.markAsRead(id: id);
    if (failure != null) return;

    final current = state;
    if (current is NotificationsLoaded) {
      state = current.copyWith(
        notifications: current.notifications
            .map((n) => n.id == id ? n.copyWith(isRead: true) : n)
            .toList(),
      );
    }
  }

  Future<void> markAllAsRead() async {
    final failure = await _repository.markAllAsRead();
    if (failure != null) return;

    final current = state;
    if (current is NotificationsLoaded) {
      state = current.copyWith(
        notifications: current.notifications
            .map((n) => n.copyWith(isRead: true))
            .toList(),
      );
    }
  }
}
