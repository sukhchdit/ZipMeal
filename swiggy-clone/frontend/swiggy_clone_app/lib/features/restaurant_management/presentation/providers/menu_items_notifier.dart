import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/restaurant_repository.dart';
import 'menu_items_state.dart';

part 'menu_items_notifier.g.dart';

@riverpod
class MenuItemsNotifier extends _$MenuItemsNotifier {
  late RestaurantRepository _repository;

  @override
  MenuItemsState build(String restaurantId, String categoryId) {
    _repository = ref.watch(restaurantRepositoryProvider);
    loadItems();
    return const MenuItemsState.initial();
  }

  Future<void> loadItems() async {
    state = const MenuItemsState.loading();
    final result = await _repository.getMenuItems(
      restaurantId: restaurantId,
      categoryId: categoryId,
    );
    if (result.failure != null) {
      state = MenuItemsState.error(failure: result.failure!);
    } else {
      state = MenuItemsState.loaded(items: result.data!);
    }
  }

  Future<bool> createItem(Map<String, dynamic> data) async {
    final result = await _repository.createMenuItem(
      restaurantId: restaurantId,
      data: data,
    );
    if (result.failure != null) return false;
    await loadItems();
    return true;
  }

  Future<bool> updateItem(String itemId, Map<String, dynamic> data) async {
    final result = await _repository.updateMenuItem(
      restaurantId: restaurantId,
      itemId: itemId,
      data: data,
    );
    if (result.failure != null) return false;
    await loadItems();
    return true;
  }

  Future<bool> deleteItem(String itemId) async {
    final failure = await _repository.deleteMenuItem(
      restaurantId: restaurantId,
      itemId: itemId,
    );
    if (failure != null) return false;
    await loadItems();
    return true;
  }
}
