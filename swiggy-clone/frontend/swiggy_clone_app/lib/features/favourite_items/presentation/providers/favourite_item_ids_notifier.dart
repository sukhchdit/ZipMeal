import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/favourite_items_repository.dart';

part 'favourite_item_ids_notifier.g.dart';

@Riverpod(keepAlive: true)
class FavouriteItemIdsNotifier extends _$FavouriteItemIdsNotifier {
  late FavouriteItemsRepository _repository;

  @override
  Set<String> build() {
    _repository = ref.watch(favouriteItemsRepositoryProvider);
    _loadIds();
    return {};
  }

  Future<void> _loadIds() async {
    final result = await _repository.getFavouriteItems();
    if (result.data != null) {
      state = result.data!.map((e) => e.menuItemId).toSet();
    }
  }

  Future<void> toggle(String menuItemId) async {
    final isFav = state.contains(menuItemId);

    // Optimistic update
    if (isFav) {
      state = {...state}..remove(menuItemId);
    } else {
      state = {...state, menuItemId};
    }

    final failure = isFav
        ? await _repository.removeFavouriteItem(menuItemId: menuItemId)
        : await _repository.addFavouriteItem(menuItemId: menuItemId);

    // Revert on failure
    if (failure != null) {
      if (isFav) {
        state = {...state, menuItemId};
      } else {
        state = {...state}..remove(menuItemId);
      }
    }
  }
}
