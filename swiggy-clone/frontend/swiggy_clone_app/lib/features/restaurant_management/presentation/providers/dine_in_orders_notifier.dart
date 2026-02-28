import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/restaurant_repository.dart';
import 'dine_in_orders_state.dart';

part 'dine_in_orders_notifier.g.dart';

@riverpod
class DineInOrdersNotifier extends _$DineInOrdersNotifier {
  late RestaurantRepository _repository;

  @override
  DineInOrdersState build(String restaurantId) {
    _repository = ref.watch(restaurantRepositoryProvider);
    loadOrders();
    return const DineInOrdersState.initial();
  }

  Future<void> loadOrders({int? statusFilter}) async {
    state = const DineInOrdersState.loading();
    final result = await _repository.getDineInOrders(
      restaurantId: restaurantId,
      statusFilter: statusFilter,
    );
    if (result.failure != null) {
      state = DineInOrdersState.error(failure: result.failure!);
    } else {
      state = DineInOrdersState.loaded(orders: result.data!);
    }
  }

  Future<bool> updateOrderStatus(
    String orderId,
    int newStatus, {
    String? notes,
  }) async {
    final failure = await _repository.updateDineInOrderStatus(
      restaurantId: restaurantId,
      orderId: orderId,
      data: {
        'newStatus': newStatus,
        if (notes != null) 'notes': notes,
      },
    );
    if (failure != null) return false;
    await loadOrders();
    return true;
  }
}
