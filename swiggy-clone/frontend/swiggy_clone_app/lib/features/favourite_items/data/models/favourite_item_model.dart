import 'package:freezed_annotation/freezed_annotation.dart';

part 'favourite_item_model.freezed.dart';
part 'favourite_item_model.g.dart';

@freezed
class FavouriteItemModel with _$FavouriteItemModel {
  const factory FavouriteItemModel({
    required String menuItemId,
    required String itemName,
    String? imageUrl,
    required int price,
    int? discountedPrice,
    required bool isVeg,
    required bool isAvailable,
    required String restaurantId,
    required String restaurantName,
    required String favouritedAt,
  }) = _FavouriteItemModel;

  factory FavouriteItemModel.fromJson(Map<String, dynamic> json) =>
      _$FavouriteItemModelFromJson(json);
}
