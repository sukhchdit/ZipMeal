import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/group_order_repository.dart';
import 'group_cart_state.dart';

part 'group_cart_notifier.g.dart';

@riverpod
class GroupCartNotifier extends _$GroupCartNotifier {
  late GroupOrderRepository _repository;

  @override
  GroupCartState build(String groupOrderId) {
    _repository = ref.watch(groupOrderRepositoryProvider);
    loadCart();
    return const GroupCartState.initial();
  }

  Future<void> loadCart() async {
    state = const GroupCartState.loading();
    final result = await _repository.getGroupCart(
      groupOrderId: groupOrderId,
    );
    if (result.failure != null) {
      state = GroupCartState.error(failure: result.failure!);
      return;
    }
    final cart = result.data!;
    if (cart.participantCarts.isEmpty) {
      state = const GroupCartState.empty();
      return;
    }
    state = GroupCartState.loaded(cart: cart);
  }

  Future<bool> addItem({
    required String menuItemId,
    String? variantId,
    int quantity = 1,
    String? specialInstructions,
    List<Map<String, dynamic>>? addons,
  }) async {
    final result = await _repository.addToCart(
      groupOrderId: groupOrderId,
      menuItemId: menuItemId,
      variantId: variantId,
      quantity: quantity,
      specialInstructions: specialInstructions,
      addons: addons,
    );
    if (result.failure != null) return false;
    await loadCart();
    return true;
  }

  Future<bool> updateQuantity({
    required String cartItemId,
    required int quantity,
  }) async {
    final result = await _repository.updateCartItem(
      groupOrderId: groupOrderId,
      cartItemId: cartItemId,
      quantity: quantity,
    );
    if (result.failure != null) return false;
    await loadCart();
    return true;
  }

  Future<bool> removeItem({required String cartItemId}) async {
    final result = await _repository.removeCartItem(
      groupOrderId: groupOrderId,
      cartItemId: cartItemId,
    );
    if (result.failure != null) return false;
    await loadCart();
    return true;
  }
}
