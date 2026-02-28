import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/favourite_item_model.dart';

part 'favourite_items_state.freezed.dart';

@freezed
sealed class FavouriteItemsState with _$FavouriteItemsState {
  const factory FavouriteItemsState.initial() = FavouriteItemsInitial;
  const factory FavouriteItemsState.loading() = FavouriteItemsLoading;
  const factory FavouriteItemsState.loaded({
    required List<FavouriteItemModel> items,
  }) = FavouriteItemsLoaded;
  const factory FavouriteItemsState.error({required Failure failure}) =
      FavouriteItemsError;
}
