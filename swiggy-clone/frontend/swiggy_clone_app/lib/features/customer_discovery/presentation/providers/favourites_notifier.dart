import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/discovery_repository.dart';
import 'favourites_state.dart';

part 'favourites_notifier.g.dart';

@riverpod
class FavouritesNotifier extends _$FavouritesNotifier {
  late DiscoveryRepository _repository;

  @override
  FavouritesState build() {
    _repository = ref.watch(discoveryRepositoryProvider);
    loadFavourites();
    return const FavouritesState.initial();
  }

  Future<void> loadFavourites() async {
    state = const FavouritesState.loading();
    final result = await _repository.getFavourites();
    if (result.failure != null) {
      state = FavouritesState.error(failure: result.failure!);
    } else {
      state = FavouritesState.loaded(restaurants: result.data!);
    }
  }

  Future<bool> removeFavourite(String restaurantId) async {
    final failure =
        await _repository.removeFavourite(restaurantId: restaurantId);
    if (failure != null) return false;
    await loadFavourites();
    return true;
  }
}
