import 'package:freezed_annotation/freezed_annotation.dart';

part 'menu_item_model.freezed.dart';
part 'menu_item_model.g.dart';

@freezed
class MenuItemModel with _$MenuItemModel {
  const factory MenuItemModel({
    required String id,
    required String categoryId,
    required String name,
    String? description,
    required int price,
    int? discountedPrice,
    String? imageUrl,
    required bool isVeg,
    required bool isAvailable,
    required bool isBestseller,
    required int preparationTimeMin,
    required int sortOrder,
    @Default(0) int spiceLevel,
    @Default([]) List<int> allergens,
    @Default([]) List<int> dietaryTags,
    int? calorieCount,
    @Default([]) List<MenuItemVariantModel> variants,
    @Default([]) List<MenuItemAddonModel> addons,
  }) = _MenuItemModel;

  factory MenuItemModel.fromJson(Map<String, dynamic> json) =>
      _$MenuItemModelFromJson(json);
}

@freezed
class MenuItemVariantModel with _$MenuItemVariantModel {
  const factory MenuItemVariantModel({
    required String id,
    required String name,
    required int priceAdjustment,
    required bool isDefault,
    required bool isAvailable,
    required int sortOrder,
  }) = _MenuItemVariantModel;

  factory MenuItemVariantModel.fromJson(Map<String, dynamic> json) =>
      _$MenuItemVariantModelFromJson(json);
}

@freezed
class MenuItemAddonModel with _$MenuItemAddonModel {
  const factory MenuItemAddonModel({
    required String id,
    required String name,
    required int price,
    required bool isVeg,
    required bool isAvailable,
    required int maxQuantity,
    required int sortOrder,
  }) = _MenuItemAddonModel;

  factory MenuItemAddonModel.fromJson(Map<String, dynamic> json) =>
      _$MenuItemAddonModelFromJson(json);
}
