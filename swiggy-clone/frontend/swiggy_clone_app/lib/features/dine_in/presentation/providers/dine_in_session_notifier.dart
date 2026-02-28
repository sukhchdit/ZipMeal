import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/storage/secure_storage_service.dart';
import '../../data/repositories/dine_in_repository.dart';
import 'active_session_notifier.dart';
import 'dine_in_session_state.dart';

part 'dine_in_session_notifier.g.dart';

@riverpod
class DineInSessionNotifier extends _$DineInSessionNotifier {
  late DineInRepository _repository;

  @override
  DineInSessionState build(String sessionId) {
    _repository = ref.watch(dineInRepositoryProvider);
    loadSession();
    return const DineInSessionState.initial();
  }

  Future<void> loadSession() async {
    state = const DineInSessionState.loading();
    final result = await _repository.getSessionDetail(sessionId: sessionId);
    if (result.failure != null) {
      state = DineInSessionState.error(failure: result.failure!);
      return;
    }
    final session = result.data!;
    final secureStorage = ref.read(secureStorageServiceProvider);
    final currentUserId = await secureStorage.getUserId();

    // Host is the member with role == 1 whose userId matches
    final isHost = session.members.any(
      (m) => m.userId == currentUserId && m.role == 1,
    );

    state = DineInSessionState.loaded(session: session, isHost: isHost);
  }

  Future<bool> requestBill() async {
    final failure = await _repository.requestBill(sessionId: sessionId);
    if (failure != null) return false;
    await loadSession();
    return true;
  }

  Future<bool> leaveSession() async {
    final failure = await _repository.leaveSession(sessionId: sessionId);
    if (failure != null) return false;
    ref.read(activeSessionNotifierProvider.notifier).clearSession();
    state = const DineInSessionState.ended();
    return true;
  }

  Future<bool> endSession() async {
    final failure = await _repository.endSession(sessionId: sessionId);
    if (failure != null) return false;
    ref.read(activeSessionNotifierProvider.notifier).clearSession();
    state = const DineInSessionState.ended();
    return true;
  }
}
