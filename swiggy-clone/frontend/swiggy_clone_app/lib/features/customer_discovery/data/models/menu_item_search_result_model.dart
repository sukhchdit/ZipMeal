import 'package:freezed_annotation/freezed_annotation.dart';

part 'menu_item_search_result_model.freezed.dart';
part 'menu_item_search_result_model.g.dart';

@freezed
class MenuItemSearchGroupedResultModel with _$MenuItemSearchGroupedResultModel {
  const factory MenuItemSearchGroupedResultModel({
    required String restaurantId,
    required String restaurantName,
    required String restaurantSlug,
    String? restaurantLogoUrl,
    String? restaurantCity,
    required double restaurantAverageRating,
    required int restaurantTotalRatings,
    required bool restaurantIsAcceptingOrders,
    @Default([]) List<MenuItemSearchHitModel> items,
  }) = _MenuItemSearchGroupedResultModel;

  factory MenuItemSearchGroupedResultModel.fromJson(Map<String, dynamic> json) =>
      _$MenuItemSearchGroupedResultModelFromJson(json);
}

@freezed
class MenuItemSearchHitModel with _$MenuItemSearchHitModel {
  const factory MenuItemSearchHitModel({
    required String id,
    required String name,
    String? description,
    required int price,
    int? discountedPrice,
    String? imageUrl,
    required bool isVeg,
    required bool isBestseller,
  }) = _MenuItemSearchHitModel;

  factory MenuItemSearchHitModel.fromJson(Map<String, dynamic> json) =>
      _$MenuItemSearchHitModelFromJson(json);
}
