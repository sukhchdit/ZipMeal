import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/restaurant_repository.dart';
import 'active_sessions_state.dart';

part 'active_sessions_notifier.g.dart';

@riverpod
class ActiveSessionsNotifier extends _$ActiveSessionsNotifier {
  late RestaurantRepository _repository;

  @override
  ActiveSessionsState build(String restaurantId) {
    _repository = ref.watch(restaurantRepositoryProvider);
    loadSessions();
    return const ActiveSessionsState.initial();
  }

  Future<void> loadSessions() async {
    state = const ActiveSessionsState.loading();
    final result =
        await _repository.getDineInSessions(restaurantId: restaurantId);
    if (result.failure != null) {
      state = ActiveSessionsState.error(failure: result.failure!);
    } else {
      state = ActiveSessionsState.loaded(sessions: result.data!);
    }
  }
}
