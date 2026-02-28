import 'package:freezed_annotation/freezed_annotation.dart';

part 'order_item_model.freezed.dart';
part 'order_item_model.g.dart';

@freezed
class OrderItemModel with _$OrderItemModel {
  const factory OrderItemModel({
    required String id,
    required String menuItemId,
    required String itemName,
    String? variantId,
    required int quantity,
    required int unitPrice,
    required int totalPrice,
    String? specialInstructions,
    @Default([]) List<OrderItemAddonModel> addons,
  }) = _OrderItemModel;

  factory OrderItemModel.fromJson(Map<String, dynamic> json) =>
      _$OrderItemModelFromJson(json);
}

@freezed
class OrderItemAddonModel with _$OrderItemAddonModel {
  const factory OrderItemAddonModel({
    required String addonId,
    required String addonName,
    required int quantity,
    required int price,
  }) = _OrderItemAddonModel;

  factory OrderItemAddonModel.fromJson(Map<String, dynamic> json) =>
      _$OrderItemAddonModelFromJson(json);
}
