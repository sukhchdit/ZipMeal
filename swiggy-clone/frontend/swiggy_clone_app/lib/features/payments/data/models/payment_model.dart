import 'package:freezed_annotation/freezed_annotation.dart';

part 'payment_model.freezed.dart';
part 'payment_model.g.dart';

@freezed
class PaymentModel with _$PaymentModel {
  const factory PaymentModel({
    required String id,
    required String orderId,
    required String paymentGateway,
    String? gatewayPaymentId,
    String? gatewayOrderId,
    required int amount,
    required String currency,
    required int status,
    String? method,
    int? refundAmount,
    String? refundReason,
    String? refundedAt,
    required String createdAt,
  }) = _PaymentModel;

  factory PaymentModel.fromJson(Map<String, dynamic> json) =>
      _$PaymentModelFromJson(json);
}
