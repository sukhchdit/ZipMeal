import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/menu_item_model.dart';

part 'menu_items_state.freezed.dart';

@freezed
sealed class MenuItemsState with _$MenuItemsState {
  const factory MenuItemsState.initial() = MenuItemsInitial;
  const factory MenuItemsState.loading() = MenuItemsLoading;
  const factory MenuItemsState.loaded({
    required List<MenuItemModel> items,
  }) = MenuItemsLoaded;
  const factory MenuItemsState.error({required Failure failure}) =
      MenuItemsError;
}
