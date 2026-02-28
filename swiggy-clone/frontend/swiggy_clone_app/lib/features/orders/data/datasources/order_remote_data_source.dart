import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/fee_config_model.dart';
import '../models/order_model.dart';
import '../models/order_summary_model.dart';

part 'order_remote_data_source.g.dart';

@riverpod
OrderRemoteDataSource orderRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return OrderRemoteDataSource(dio: dio);
}

class OrderRemoteDataSource {
  OrderRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  Future<OrderModel> placeOrder({
    required String deliveryAddressId,
    required int paymentMethod,
    String? specialInstructions,
    String? couponCode,
    String? scheduledDeliveryTime,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.orders,
      data: {
        'deliveryAddressId': deliveryAddressId,
        'paymentMethod': paymentMethod,
        if (specialInstructions != null)
          'specialInstructions': specialInstructions,
        if (couponCode != null) 'couponCode': couponCode,
        if (scheduledDeliveryTime != null)
          'scheduledDeliveryTime': scheduledDeliveryTime,
      },
    );
    return OrderModel.fromJson(response.data!);
  }

  Future<Map<String, dynamic>> getMyOrders({
    String? cursor,
    int pageSize = 20,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.orders,
      queryParameters: {
        if (cursor != null) 'cursor': cursor,
        'pageSize': pageSize,
      },
    );
    return response.data!;
  }

  Future<OrderModel> getOrderDetail({required String orderId}) async {
    final response = await _dio.get<Map<String, dynamic>>(
      '${ApiConstants.orders}/$orderId',
    );
    return OrderModel.fromJson(response.data!);
  }

  Future<void> cancelOrder({
    required String orderId,
    String? cancellationReason,
  }) async {
    await _dio.put(
      '${ApiConstants.orders}/$orderId/cancel',
      data: {
        if (cancellationReason != null) 'cancellationReason': cancellationReason,
      },
    );
  }

  Future<FeeConfigModel> getFeeConfig() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.configFees,
    );
    return FeeConfigModel.fromJson(response.data!);
  }
}
