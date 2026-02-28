import 'package:freezed_annotation/freezed_annotation.dart';

part 'order_summary_model.freezed.dart';
part 'order_summary_model.g.dart';

@freezed
class OrderSummaryModel with _$OrderSummaryModel {
  const factory OrderSummaryModel({
    required String id,
    required String orderNumber,
    required String restaurantName,
    String? restaurantLogoUrl,
    required int status,
    required int totalAmount,
    required int itemCount,
    required String createdAt,
    String? scheduledDeliveryTime,
  }) = _OrderSummaryModel;

  factory OrderSummaryModel.fromJson(Map<String, dynamic> json) =>
      _$OrderSummaryModelFromJson(json);
}
