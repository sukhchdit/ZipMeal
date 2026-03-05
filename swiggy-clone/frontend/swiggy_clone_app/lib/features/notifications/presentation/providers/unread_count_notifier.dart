import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../auth/presentation/providers/auth_notifier.dart';
import '../../../auth/presentation/providers/auth_state.dart';
import '../../data/repositories/notification_repository.dart';

part 'unread_count_notifier.g.dart';

@riverpod
class UnreadCountNotifier extends _$UnreadCountNotifier {
  late NotificationRepository _repository;

  @override
  int build() {
    _repository = ref.watch(notificationRepositoryProvider);
    final authState = ref.watch(authNotifierProvider);
    if (authState is AuthAuthenticated) {
      fetchCount();
    }
    return 0;
  }

  Future<void> fetchCount() async {
    final result = await _repository.getUnreadCount();
    if (result.failure == null && result.data != null) {
      state = result.data!;
    }
  }

  void decrement() {
    if (state > 0) state = state - 1;
  }

  void reset() {
    state = 0;
  }
}
