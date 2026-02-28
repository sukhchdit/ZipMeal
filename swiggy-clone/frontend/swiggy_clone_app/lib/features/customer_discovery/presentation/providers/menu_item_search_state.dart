import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/menu_item_search_result_model.dart';

part 'menu_item_search_state.freezed.dart';

@freezed
sealed class MenuItemSearchState with _$MenuItemSearchState {
  const factory MenuItemSearchState.initial() = MenuItemSearchInitial;
  const factory MenuItemSearchState.loading() = MenuItemSearchLoading;
  const factory MenuItemSearchState.loaded({
    required List<MenuItemSearchGroupedResultModel> results,
  }) = MenuItemSearchLoaded;
  const factory MenuItemSearchState.empty() = MenuItemSearchEmpty;
  const factory MenuItemSearchState.error({required Failure failure}) =
      MenuItemSearchError;
}
