import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/order_repository.dart';
import 'place_order_state.dart';

part 'place_order_notifier.g.dart';

@riverpod
class PlaceOrderNotifier extends _$PlaceOrderNotifier {
  late OrderRepository _repository;

  @override
  PlaceOrderState build() {
    _repository = ref.watch(orderRepositoryProvider);
    return const PlaceOrderState.initial();
  }

  Future<void> placeOrder({
    required String deliveryAddressId,
    required int paymentMethod,
    String? specialInstructions,
    String? couponCode,
  }) async {
    state = const PlaceOrderState.placing();
    final result = await _repository.placeOrder(
      deliveryAddressId: deliveryAddressId,
      paymentMethod: paymentMethod,
      specialInstructions: specialInstructions,
      couponCode: couponCode,
    );
    if (result.failure != null) {
      state = PlaceOrderState.error(failure: result.failure!);
    } else {
      state = PlaceOrderState.placed(order: result.data!);
    }
  }
}
