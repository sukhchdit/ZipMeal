import 'package:freezed_annotation/freezed_annotation.dart';

part 'payment_order_model.freezed.dart';
part 'payment_order_model.g.dart';

@freezed
class PaymentOrderModel with _$PaymentOrderModel {
  const factory PaymentOrderModel({
    required String paymentId,
    required String gatewayOrderId,
    required int amount,
    required String currency,
    required String gateway,
  }) = _PaymentOrderModel;

  factory PaymentOrderModel.fromJson(Map<String, dynamic> json) =>
      _$PaymentOrderModelFromJson(json);
}
