import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/dine_in_repository.dart';
import 'session_orders_state.dart';

part 'session_orders_notifier.g.dart';

@riverpod
class SessionOrdersNotifier extends _$SessionOrdersNotifier {
  late DineInRepository _repository;

  @override
  SessionOrdersState build(String sessionId) {
    _repository = ref.watch(dineInRepositoryProvider);
    loadOrders();
    return const SessionOrdersState.initial();
  }

  Future<void> loadOrders() async {
    state = const SessionOrdersState.loading();
    final result = await _repository.getSessionOrders(sessionId: sessionId);
    if (result.failure != null) {
      state = SessionOrdersState.error(failure: result.failure!);
    } else {
      state = SessionOrdersState.loaded(orders: result.data!);
    }
  }

  Future<bool> placeOrder({
    required List<Map<String, dynamic>> items,
    String? specialInstructions,
  }) async {
    final result = await _repository.placeDineInOrder(
      sessionId: sessionId,
      items: items,
      specialInstructions: specialInstructions,
    );
    if (result.failure != null) return false;
    await loadOrders();
    return true;
  }
}
