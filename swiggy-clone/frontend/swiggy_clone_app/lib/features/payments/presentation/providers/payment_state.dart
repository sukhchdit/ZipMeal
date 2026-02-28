import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/payment_model.dart';
import '../../data/models/payment_order_model.dart';

part 'payment_state.freezed.dart';

@freezed
sealed class PaymentState with _$PaymentState {
  const factory PaymentState.initial() = PaymentInitial;
  const factory PaymentState.creatingOrder() = PaymentCreatingOrder;
  const factory PaymentState.orderCreated(
      {required PaymentOrderModel paymentOrder}) = PaymentOrderCreated;
  const factory PaymentState.processing() = PaymentProcessing;
  const factory PaymentState.verifying() = PaymentVerifying;
  const factory PaymentState.success({required PaymentModel payment}) =
      PaymentSuccess;
  const factory PaymentState.failed({required Failure failure}) =
      PaymentFailed;
}
