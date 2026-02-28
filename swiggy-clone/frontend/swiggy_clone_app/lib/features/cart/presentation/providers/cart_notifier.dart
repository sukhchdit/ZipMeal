import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/cart_repository.dart';
import 'cart_state.dart';

part 'cart_notifier.g.dart';

@Riverpod(keepAlive: true)
class CartNotifier extends _$CartNotifier {
  late CartRepository _repository;

  @override
  CartState build() {
    _repository = ref.watch(cartRepositoryProvider);
    loadCart();
    return const CartState.initial();
  }

  Future<void> loadCart() async {
    state = const CartState.loading();
    final result = await _repository.getCart();
    if (result.failure != null) {
      state = CartState.error(failure: result.failure!);
    } else if (result.data!.items.isEmpty) {
      state = const CartState.empty();
    } else {
      state = CartState.loaded(cart: result.data!);
    }
  }

  /// Returns true on success, false on failure.
  /// If errorCode is 'DIFFERENT_RESTAURANT', the UI should show a confirmation dialog.
  Future<({bool success, String? errorCode})> addToCart({
    required String restaurantId,
    required String menuItemId,
    String? variantId,
    required int quantity,
    String? specialInstructions,
    required List<Map<String, dynamic>> addons,
  }) async {
    final result = await _repository.addToCart(
      restaurantId: restaurantId,
      menuItemId: menuItemId,
      variantId: variantId,
      quantity: quantity,
      specialInstructions: specialInstructions,
      addons: addons,
    );

    if (result.failure != null) {
      return (success: false, errorCode: result.errorCode);
    }

    if (result.data!.items.isEmpty) {
      state = const CartState.empty();
    } else {
      state = CartState.loaded(cart: result.data!);
    }
    return (success: true, errorCode: null);
  }

  Future<void> updateQuantity({
    required String cartItemId,
    required int quantity,
  }) async {
    final result = await _repository.updateQuantity(
      cartItemId: cartItemId,
      quantity: quantity,
    );
    if (result.failure != null) return;

    if (result.data!.items.isEmpty) {
      state = const CartState.empty();
    } else {
      state = CartState.loaded(cart: result.data!);
    }
  }

  Future<void> removeItem({required String cartItemId}) async {
    final result = await _repository.removeItem(cartItemId: cartItemId);
    if (result.failure != null) return;

    if (result.data!.items.isEmpty) {
      state = const CartState.empty();
    } else {
      state = CartState.loaded(cart: result.data!);
    }
  }

  Future<void> clearCart() async {
    await _repository.clearCart();
    state = const CartState.empty();
  }
}
