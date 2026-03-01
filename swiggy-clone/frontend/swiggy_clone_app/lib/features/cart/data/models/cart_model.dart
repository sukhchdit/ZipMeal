import 'package:freezed_annotation/freezed_annotation.dart';

part 'cart_model.freezed.dart';
part 'cart_model.g.dart';

@freezed
class CartModel with _$CartModel {
  const factory CartModel({
    required String restaurantId,
    required String restaurantName,
    @Default([]) List<CartItemModel> items,
    required int subtotal,
  }) = _CartModel;

  factory CartModel.fromJson(Map<String, dynamic> json) =>
      _$CartModelFromJson(json);
}

@freezed
class CartItemModel with _$CartItemModel {
  const factory CartItemModel({
    required String cartItemId,
    required String menuItemId,
    String? variantId,
    required String itemName,
    String? variantName,
    required int quantity,
    required int unitPrice,
    required int totalPrice,
    @Default([]) List<CartItemAddonModel> addons,
    String? specialInstructions,
    @Default([]) List<int> allergens,
  }) = _CartItemModel;

  factory CartItemModel.fromJson(Map<String, dynamic> json) =>
      _$CartItemModelFromJson(json);
}

@freezed
class CartItemAddonModel with _$CartItemAddonModel {
  const factory CartItemAddonModel({
    required String addonId,
    required String addonName,
    required int price,
    required int quantity,
  }) = _CartItemAddonModel;

  factory CartItemAddonModel.fromJson(Map<String, dynamic> json) =>
      _$CartItemAddonModelFromJson(json);
}
