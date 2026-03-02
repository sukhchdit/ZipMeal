import 'package:freezed_annotation/freezed_annotation.dart';

part 'recommended_menu_item_model.freezed.dart';
part 'recommended_menu_item_model.g.dart';

@freezed
class RecommendedMenuItemModel with _$RecommendedMenuItemModel {
  const factory RecommendedMenuItemModel({
    required String id,
    required String name,
    String? description,
    required int price,
    int? discountedPrice,
    String? imageUrl,
    required bool isVeg,
    required bool isBestseller,
    required String restaurantId,
    required String restaurantName,
    required String restaurantSlug,
    String? recommendationReason,
    required double score,
  }) = _RecommendedMenuItemModel;

  factory RecommendedMenuItemModel.fromJson(Map<String, dynamic> json) =>
      _$RecommendedMenuItemModelFromJson(json);
}
