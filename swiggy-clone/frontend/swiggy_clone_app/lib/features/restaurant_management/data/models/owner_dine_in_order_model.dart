import 'package:freezed_annotation/freezed_annotation.dart';

part 'owner_dine_in_order_model.freezed.dart';
part 'owner_dine_in_order_model.g.dart';

@freezed
class OwnerDineInOrderModel with _$OwnerDineInOrderModel {
  const factory OwnerDineInOrderModel({
    required String id,
    required String orderNumber,
    required String tableNumber,
    required String customerName,
    required int status,
    required int itemCount,
    required int totalAmount,
    String? specialInstructions,
    required String createdAt,
    @Default([]) List<OwnerDineInOrderItemModel> items,
  }) = _OwnerDineInOrderModel;

  factory OwnerDineInOrderModel.fromJson(Map<String, dynamic> json) =>
      _$OwnerDineInOrderModelFromJson(json);
}

@freezed
class OwnerDineInOrderItemModel with _$OwnerDineInOrderItemModel {
  const factory OwnerDineInOrderItemModel({
    required String itemName,
    String? variantName,
    required int quantity,
    required int price,
    String? specialInstructions,
  }) = _OwnerDineInOrderItemModel;

  factory OwnerDineInOrderItemModel.fromJson(Map<String, dynamic> json) =>
      _$OwnerDineInOrderItemModelFromJson(json);
}
