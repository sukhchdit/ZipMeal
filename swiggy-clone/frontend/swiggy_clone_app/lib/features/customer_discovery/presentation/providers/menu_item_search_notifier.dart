import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/discovery_repository.dart';
import 'menu_item_search_state.dart';

part 'menu_item_search_notifier.g.dart';

@riverpod
class MenuItemSearchNotifier extends _$MenuItemSearchNotifier {
  late DiscoveryRepository _repository;

  @override
  MenuItemSearchState build() {
    _repository = ref.watch(discoveryRepositoryProvider);
    return const MenuItemSearchState.initial();
  }

  Future<void> search(String term, {String? city}) async {
    if (term.trim().length < 2) {
      state = const MenuItemSearchState.initial();
      return;
    }

    state = const MenuItemSearchState.loading();

    final result = await _repository.searchMenuItems(term: term, city: city);

    if (result.failure != null) {
      state = MenuItemSearchState.error(failure: result.failure!);
    } else if (result.data!.isEmpty) {
      state = const MenuItemSearchState.empty();
    } else {
      state = MenuItemSearchState.loaded(results: result.data!);
    }
  }

  void clear() => state = const MenuItemSearchState.initial();
}
