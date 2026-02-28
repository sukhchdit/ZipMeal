import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/payment_remote_data_source.dart';
import '../models/payment_model.dart';
import '../models/payment_order_model.dart';

part 'payment_repository.g.dart';

@riverpod
PaymentRepository paymentRepository(Ref ref) {
  final remoteDataSource = ref.watch(paymentRemoteDataSourceProvider);
  return PaymentRepository(remoteDataSource: remoteDataSource);
}

class PaymentRepository {
  PaymentRepository({required PaymentRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final PaymentRemoteDataSource _remote;

  Future<({PaymentOrderModel? data, Failure? failure})> createPaymentOrder({
    required String orderId,
    required int paymentMethod,
  }) async {
    try {
      final result = await _remote.createPaymentOrder(
        orderId: orderId,
        paymentMethod: paymentMethod,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({PaymentModel? data, Failure? failure})> verifyPayment({
    required String gatewayOrderId,
    required String gatewayPaymentId,
    required String gatewaySignature,
  }) async {
    try {
      final result = await _remote.verifyPayment(
        gatewayOrderId: gatewayOrderId,
        gatewayPaymentId: gatewayPaymentId,
        gatewaySignature: gatewaySignature,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({PaymentModel? data, Failure? failure})> getPaymentByOrderId({
    required String orderId,
  }) async {
    try {
      final result = await _remote.getPaymentByOrderId(orderId: orderId);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({PaymentOrderModel? data, Failure? failure})> payDineInSession({
    required String sessionId,
    required int paymentMethod,
  }) async {
    try {
      final result = await _remote.payDineInSession(
        sessionId: sessionId,
        paymentMethod: paymentMethod,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Failure _mapDioError(DioException e) {
    if (e.type == DioExceptionType.connectionError ||
        e.type == DioExceptionType.connectionTimeout) {
      return const NetworkFailure();
    }
    final statusCode = e.response?.statusCode;
    final data = e.response?.data;
    String message = 'An unexpected error occurred.';
    if (data is Map<String, dynamic>) {
      message = (data['errorMessage'] as String?) ?? message;
    }
    if (statusCode == 401) return AuthFailure(message: message);
    return ServerFailure(message: message, statusCode: statusCode);
  }
}
