import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/chat_support_repository.dart';

part 'unread_chat_count_notifier.g.dart';

@Riverpod(keepAlive: true)
class UnreadChatCountNotifier extends _$UnreadChatCountNotifier {
  @override
  int build() {
    _fetchCount();
    return 0;
  }

  Future<void> _fetchCount() async {
    final repository = ref.read(chatSupportRepositoryProvider);
    final result = await repository.getUnreadCount();
    if (result.data != null) {
      state = result.data!;
    }
  }

  Future<void> refresh() async {
    await _fetchCount();
  }

  void decrement(int amount) {
    state = (state - amount).clamp(0, state);
  }

  void increment() {
    state = state + 1;
  }
}
