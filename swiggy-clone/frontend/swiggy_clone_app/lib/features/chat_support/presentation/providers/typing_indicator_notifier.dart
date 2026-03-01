import 'dart:async';

import 'package:riverpod_annotation/riverpod_annotation.dart';

part 'typing_indicator_notifier.g.dart';

@riverpod
class TypingIndicatorNotifier extends _$TypingIndicatorNotifier {
  Timer? _hideTimer;

  @override
  bool build(String ticketId) {
    ref.onDispose(() => _hideTimer?.cancel());
    return false;
  }

  void setTyping(bool isTyping) {
    _hideTimer?.cancel();
    state = isTyping;

    if (isTyping) {
      // Auto-hide after 5 seconds if no update
      _hideTimer = Timer(const Duration(seconds: 5), () {
        state = false;
      });
    }
  }
}
