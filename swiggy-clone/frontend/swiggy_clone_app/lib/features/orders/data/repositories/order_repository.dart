import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/order_remote_data_source.dart';
import '../models/order_model.dart';
import '../models/order_summary_model.dart';

part 'order_repository.g.dart';

@riverpod
OrderRepository orderRepository(Ref ref) {
  final remoteDataSource = ref.watch(orderRemoteDataSourceProvider);
  return OrderRepository(remoteDataSource: remoteDataSource);
}

class OrderRepository {
  OrderRepository({required OrderRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final OrderRemoteDataSource _remote;

  Future<({OrderModel? data, Failure? failure})> placeOrder({
    required String deliveryAddressId,
    required int paymentMethod,
    String? specialInstructions,
    String? couponCode,
    String? scheduledDeliveryTime,
  }) async {
    try {
      final result = await _remote.placeOrder(
        deliveryAddressId: deliveryAddressId,
        paymentMethod: paymentMethod,
        specialInstructions: specialInstructions,
        couponCode: couponCode,
        scheduledDeliveryTime: scheduledDeliveryTime,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<
      ({
        List<OrderSummaryModel>? items,
        String? nextCursor,
        bool? hasMore,
        Failure? failure,
      })> getMyOrders({String? cursor, int pageSize = 20}) async {
    try {
      final data =
          await _remote.getMyOrders(cursor: cursor, pageSize: pageSize);
      final items = (data['items'] as List<dynamic>)
          .map((e) => OrderSummaryModel.fromJson(e as Map<String, dynamic>))
          .toList();
      return (
        items: items,
        nextCursor: data['nextCursor'] as String?,
        hasMore: data['hasMore'] as bool?,
        failure: null,
      );
    } on DioException catch (e) {
      return (items: null, nextCursor: null, hasMore: null, failure: _mapDioError(e));
    }
  }

  Future<({OrderModel? data, Failure? failure})> getOrderDetail({
    required String orderId,
  }) async {
    try {
      final result = await _remote.getOrderDetail(orderId: orderId);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> cancelOrder({
    required String orderId,
    String? cancellationReason,
  }) async {
    try {
      await _remote.cancelOrder(
        orderId: orderId,
        cancellationReason: cancellationReason,
      );
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
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
