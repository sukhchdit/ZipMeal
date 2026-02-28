import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/search_suggestion_model.dart';

part 'search_suggestions_state.freezed.dart';

@freezed
sealed class SearchSuggestionsState with _$SearchSuggestionsState {
  const factory SearchSuggestionsState.initial() = SearchSuggestionsInitial;
  const factory SearchSuggestionsState.loading() = SearchSuggestionsLoading;
  const factory SearchSuggestionsState.loaded({
    required List<SearchSuggestionModel> suggestions,
  }) = SearchSuggestionsLoaded;
  const factory SearchSuggestionsState.empty() = SearchSuggestionsEmpty;
  const factory SearchSuggestionsState.error({required Failure failure}) =
      SearchSuggestionsError;
}
