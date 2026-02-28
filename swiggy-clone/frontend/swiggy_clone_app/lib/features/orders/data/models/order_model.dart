import 'package:freezed_annotation/freezed_annotation.dart';

import 'order_item_model.dart';

part 'order_model.freezed.dart';
part 'order_model.g.dart';

@freezed
class OrderModel with _$OrderModel {
  const factory OrderModel({
    required String id,
    required String orderNumber,
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
    String? estimatedDeliveryTime,
    required String createdAt,
    @Default([]) List<OrderItemModel> items,
    @Default(false) bool hasReview,
    @Default(0) int tipAmount,
    @Default(false) bool hasTipped,
  }) = _OrderModel;

  factory OrderModel.fromJson(Map<String, dynamic> json) =>
      _$OrderModelFromJson(json);
}
