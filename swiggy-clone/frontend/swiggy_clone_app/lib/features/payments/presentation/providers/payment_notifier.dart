import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/models/payment_order_model.dart';
import '../../data/repositories/payment_repository.dart';
import 'payment_state.dart';

part 'payment_notifier.g.dart';

@riverpod
class PaymentNotifier extends _$PaymentNotifier {
  late PaymentRepository _repository;

  @override
  PaymentState build() {
    _repository = ref.watch(paymentRepositoryProvider);
    return const PaymentState.initial();
  }

  /// For delivery orders: create payment order, then simulate processing.
  Future<void> initiatePayment({
    required String orderId,
    required int paymentMethod,
  }) async {
    state = const PaymentState.creatingOrder();

    final result = await _repository.createPaymentOrder(
      orderId: orderId,
      paymentMethod: paymentMethod,
    );

    if (result.failure != null) {
      state = PaymentState.failed(failure: result.failure!);
      return;
    }

    state = PaymentState.orderCreated(paymentOrder: result.data!);
    await _simulateDevPayment(result.data!);
  }

  /// For dine-in session: create payment order for session total.
  Future<void> initiateDineInPayment({
    required String sessionId,
    required int paymentMethod,
  }) async {
    state = const PaymentState.creatingOrder();

    final result = await _repository.payDineInSession(
      sessionId: sessionId,
      paymentMethod: paymentMethod,
    );

    if (result.failure != null) {
      state = PaymentState.failed(failure: result.failure!);
      return;
    }

    state = PaymentState.orderCreated(paymentOrder: result.data!);
    await _simulateDevPayment(result.data!);
  }

  Future<void> _simulateDevPayment(PaymentOrderModel order) async {
    state = const PaymentState.processing();

    // Simulate 2-second payment processing in dev mode
    await Future.delayed(const Duration(seconds: 2));

    state = const PaymentState.verifying();

    final now = DateTime.now().millisecondsSinceEpoch;
    final result = await _repository.verifyPayment(
      gatewayOrderId: order.gatewayOrderId,
      gatewayPaymentId: 'dev_pay_$now',
      gatewaySignature: 'dev_sig_$now',
    );

    if (result.failure != null) {
      state = PaymentState.failed(failure: result.failure!);
    } else {
      state = PaymentState.success(payment: result.data!);
    }
  }
}
