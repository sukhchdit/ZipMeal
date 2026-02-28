import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/discovery_repository.dart';
import 'public_restaurant_detail_state.dart';

part 'public_restaurant_detail_notifier.g.dart';

@riverpod
class PublicRestaurantDetailNotifier extends _$PublicRestaurantDetailNotifier {
  late DiscoveryRepository _repository;

  @override
  PublicRestaurantDetailState build(String restaurantId) {
    _repository = ref.watch(discoveryRepositoryProvider);
    loadDetail();
    return const PublicRestaurantDetailState.initial();
  }

  Future<void> loadDetail() async {
    state = const PublicRestaurantDetailState.loading();

    final result = await _repository.getRestaurantDetail(id: restaurantId);
    if (result.failure != null) {
      state = PublicRestaurantDetailState.error(failure: result.failure!);
      return;
    }

    // Check favourite status (fails silently if not authenticated)
    bool isFavourited = false;
    try {
      final favResult =
          await _repository.checkFavourite(restaurantId: restaurantId);
      isFavourited = favResult.data ?? false;
    } catch (_) {
      // Not authenticated or error — just show not favourited
    }

    state = PublicRestaurantDetailState.loaded(
      restaurant: result.data!,
      isFavourited: isFavourited,
    );
  }

  Future<void> toggleFavourite() async {
    final current = state;
    if (current is! PublicRestaurantDetailLoaded) return;

    if (current.isFavourited) {
      await _repository.removeFavourite(restaurantId: restaurantId);
    } else {
      await _repository.addFavourite(restaurantId: restaurantId);
    }

    state = current.copyWith(isFavourited: !current.isFavourited);
  }
}
