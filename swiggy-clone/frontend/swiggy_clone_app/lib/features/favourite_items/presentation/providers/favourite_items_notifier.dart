import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/favourite_items_repository.dart';
import 'favourite_items_state.dart';

part 'favourite_items_notifier.g.dart';

@riverpod
class FavouriteItemsNotifier extends _$FavouriteItemsNotifier {
  late FavouriteItemsRepository _repository;

  @override
  FavouriteItemsState build() {
    _repository = ref.watch(favouriteItemsRepositoryProvider);
    loadItems();
    return const FavouriteItemsState.initial();
  }

  Future<void> loadItems() async {
    state = const FavouriteItemsState.loading();
    final result = await _repository.getFavouriteItems();
    if (result.failure != null) {
      state = FavouriteItemsState.error(failure: result.failure!);
    } else {
      state = FavouriteItemsState.loaded(items: result.data!);
    }
  }

  Future<bool> removeItem(String menuItemId) async {
    final failure =
        await _repository.removeFavouriteItem(menuItemId: menuItemId);
    if (failure != null) return false;
    await loadItems();
    return true;
  }
}
