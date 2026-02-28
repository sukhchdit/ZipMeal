import 'package:freezed_annotation/freezed_annotation.dart';

part 'admin_order_detail_model.freezed.dart';
part 'admin_order_detail_model.g.dart';

@freezed
class AdminOrderDetailModel with _$AdminOrderDetailModel {
  const factory AdminOrderDetailModel({
    required String id,
    required String orderNumber,
    required String userId,
    required String customerName,
    required String customerPhone,
    required String restaurantId,
    required String restaurantName,
    required int orderType,
    required int status,
    required int subtotal,
    required int taxAmount,
    required int deliveryFee,
    required int packagingCharge,
    required int discountAmount,
    required int totalAmount,
    required int paymentStatus,
    int? paymentMethod,
    String? specialInstructions,
    String? cancellationReason,
    DateTime? estimatedDeliveryTime,
    DateTime? actualDeliveryTime,
    required DateTime createdAt,
    @Default([]) List<AdminOrderItemModel> items,
  }) = _AdminOrderDetailModel;

  factory AdminOrderDetailModel.fromJson(Map<String, dynamic> json) =>
      _$AdminOrderDetailModelFromJson(json);
}

@freezed
class AdminOrderItemModel with _$AdminOrderItemModel {
  const factory AdminOrderItemModel({
    required String id,
    required String menuItemId,
    required String itemName,
    String? variantId,
    required int quantity,
    required int unitPrice,
    required int totalPrice,
    String? specialInstructions,
    @Default([]) List<AdminOrderItemAddonModel> addons,
  }) = _AdminOrderItemModel;

  factory AdminOrderItemModel.fromJson(Map<String, dynamic> json) =>
      _$AdminOrderItemModelFromJson(json);
}

@freezed
class AdminOrderItemAddonModel with _$AdminOrderItemAddonModel {
  const factory AdminOrderItemAddonModel({
    required String addonId,
    required String addonName,
    required int quantity,
    required int price,
  }) = _AdminOrderItemAddonModel;

  factory AdminOrderItemAddonModel.fromJson(Map<String, dynamic> json) =>
      _$AdminOrderItemAddonModelFromJson(json);
}
