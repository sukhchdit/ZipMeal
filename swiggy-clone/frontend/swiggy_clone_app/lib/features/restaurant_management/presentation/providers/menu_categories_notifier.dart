import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/restaurant_repository.dart';
import 'menu_categories_state.dart';

part 'menu_categories_notifier.g.dart';

@riverpod
class MenuCategoriesNotifier extends _$MenuCategoriesNotifier {
  late RestaurantRepository _repository;

  @override
  MenuCategoriesState build(String restaurantId) {
    _repository = ref.watch(restaurantRepositoryProvider);
    loadCategories();
    return const MenuCategoriesState.initial();
  }

  Future<void> loadCategories() async {
    state = const MenuCategoriesState.loading();
    final result =
        await _repository.getMenuCategories(restaurantId: restaurantId);
    if (result.failure != null) {
      state = MenuCategoriesState.error(failure: result.failure!);
    } else {
      state = MenuCategoriesState.loaded(categories: result.data!);
    }
  }

  Future<bool> createCategory(Map<String, dynamic> data) async {
    final result = await _repository.createMenuCategory(
      restaurantId: restaurantId,
      data: data,
    );
    if (result.failure != null) return false;
    await loadCategories();
    return true;
  }

  Future<bool> updateCategory(
      String categoryId, Map<String, dynamic> data) async {
    final result = await _repository.updateMenuCategory(
      restaurantId: restaurantId,
      categoryId: categoryId,
      data: data,
    );
    if (result.failure != null) return false;
    await loadCategories();
    return true;
  }

  Future<bool> deleteCategory(String categoryId) async {
    final failure = await _repository.deleteMenuCategory(
      restaurantId: restaurantId,
      categoryId: categoryId,
    );
    if (failure != null) return false;
    await loadCategories();
    return true;
  }
}
