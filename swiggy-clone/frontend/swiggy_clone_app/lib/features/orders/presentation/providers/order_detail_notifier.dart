import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/order_repository.dart';
import 'order_detail_state.dart';

part 'order_detail_notifier.g.dart';

@riverpod
class OrderDetailNotifier extends _$OrderDetailNotifier {
  late OrderRepository _repository;

  @override
  OrderDetailState build(String orderId) {
    _repository = ref.watch(orderRepositoryProvider);
    loadDetail();
    return const OrderDetailState.initial();
  }

  Future<void> loadDetail() async {
    state = const OrderDetailState.loading();
    final result = await _repository.getOrderDetail(orderId: orderId);
    if (result.failure != null) {
      state = OrderDetailState.error(failure: result.failure!);
    } else {
      state = OrderDetailState.loaded(order: result.data!);
    }
  }

  Future<bool> cancelOrder({String? reason}) async {
    final failure = await _repository.cancelOrder(
      orderId: orderId,
      cancellationReason: reason,
    );
    if (failure != null) return false;
    await loadDetail();
    return true;
  }
}
