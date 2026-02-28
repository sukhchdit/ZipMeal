import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/payment_model.dart';
import '../models/payment_order_model.dart';

part 'payment_remote_data_source.g.dart';

@riverpod
PaymentRemoteDataSource paymentRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return PaymentRemoteDataSource(dio: dio);
}

class PaymentRemoteDataSource {
  PaymentRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  /// POST /api/v1/payments — Create payment order
  Future<PaymentOrderModel> createPaymentOrder({
    required String orderId,
    required int paymentMethod,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.payments,
      data: {
        'orderId': orderId,
        'paymentMethod': paymentMethod,
      },
    );
    return PaymentOrderModel.fromJson(response.data!);
  }

  /// POST /api/v1/payments/verify — Verify payment
  Future<PaymentModel> verifyPayment({
    required String gatewayOrderId,
    required String gatewayPaymentId,
    required String gatewaySignature,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.paymentVerify,
      data: {
        'gatewayOrderId': gatewayOrderId,
        'gatewayPaymentId': gatewayPaymentId,
        'gatewaySignature': gatewaySignature,
      },
    );
    return PaymentModel.fromJson(response.data!);
  }

  /// GET /api/v1/payments/{orderId} — Get payment by order
  Future<PaymentModel> getPaymentByOrderId({
    required String orderId,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.paymentByOrder(orderId),
    );
    return PaymentModel.fromJson(response.data!);
  }

  /// POST /api/v1/payments/dine-in-session — Pay for dine-in session
  Future<PaymentOrderModel> payDineInSession({
    required String sessionId,
    required int paymentMethod,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.paymentDineInSession,
      data: {
        'sessionId': sessionId,
        'paymentMethod': paymentMethod,
      },
    );
    return PaymentOrderModel.fromJson(response.data!);
  }
}
