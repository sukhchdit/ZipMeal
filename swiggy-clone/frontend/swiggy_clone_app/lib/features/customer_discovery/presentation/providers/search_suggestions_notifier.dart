import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/discovery_repository.dart';
import 'search_suggestions_state.dart';

part 'search_suggestions_notifier.g.dart';

@riverpod
class SearchSuggestionsNotifier extends _$SearchSuggestionsNotifier {
  late DiscoveryRepository _repository;

  @override
  SearchSuggestionsState build() {
    _repository = ref.watch(discoveryRepositoryProvider);
    return const SearchSuggestionsState.initial();
  }

  Future<void> fetchSuggestions(String prefix, {String? city}) async {
    if (prefix.trim().isEmpty) {
      state = const SearchSuggestionsState.initial();
      return;
    }

    state = const SearchSuggestionsState.loading();

    final result = await _repository.getSuggestions(
      prefix: prefix,
      city: city,
    );

    if (result.failure != null) {
      state = SearchSuggestionsState.error(failure: result.failure!);
    } else if (result.data!.isEmpty) {
      state = const SearchSuggestionsState.empty();
    } else {
      state = SearchSuggestionsState.loaded(suggestions: result.data!);
    }
  }

  void clear() => state = const SearchSuggestionsState.initial();
}
