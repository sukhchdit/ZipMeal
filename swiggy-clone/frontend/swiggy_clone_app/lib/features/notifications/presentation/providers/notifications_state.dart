import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/notification_model.dart';

part 'notifications_state.freezed.dart';

@freezed
sealed class NotificationsState with _$NotificationsState {
  const factory NotificationsState.initial() = NotificationsInitial;
  const factory NotificationsState.loading() = NotificationsLoading;
  const factory NotificationsState.loaded({
    required List<NotificationModel> notifications,
    required bool hasMore,
    required String? nextCursor,
    @Default(false) bool isLoadingMore,
  }) = NotificationsLoaded;
  const factory NotificationsState.error({required Failure failure}) =
      NotificationsError;
}
