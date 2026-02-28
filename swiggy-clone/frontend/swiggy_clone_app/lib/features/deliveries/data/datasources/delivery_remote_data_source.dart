import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';

part 'delivery_remote_data_source.g.dart';

@riverpod
DeliveryRemoteDataSource deliveryRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return DeliveryRemoteDataSource(dio: dio);
}

class DeliveryRemoteDataSource {
  DeliveryRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  Future<void> toggleOnlineStatus({
    required bool isOnline,
    double? latitude,
    double? longitude,
  }) async {
    await _dio.put(
      ApiConstants.deliveryOnlineStatus,
      data: {
        'isOnline': isOnline,
        if (latitude != null) 'latitude': latitude,
        if (longitude != null) 'longitude': longitude,
      },
    );
  }

  Future<Map<String, dynamic>> getMyDeliveries({
    String? cursor,
    int pageSize = 20,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.deliveries,
      queryParameters: {
        if (cursor != null) 'cursor': cursor,
        'pageSize': pageSize,
      },
    );
    return response.data!;
  }

  Future<Map<String, dynamic>?> getActiveDelivery() async {
    final response = await _dio.get<Map<String, dynamic>?>(
      ApiConstants.deliveryActive,
    );
    return response.data;
  }

  Future<Map<String, dynamic>> getDashboard() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.deliveryDashboard,
    );
    return response.data!;
  }

  Future<void> acceptDelivery({required String assignmentId}) async {
    await _dio.put(ApiConstants.deliveryAccept(assignmentId));
  }

  Future<void> updateDeliveryStatus({
    required String assignmentId,
    required int newStatus,
  }) async {
    await _dio.put(
      ApiConstants.deliveryStatus(assignmentId),
      data: {'newStatus': newStatus},
    );
  }

  Future<void> updateLocation({
    required double latitude,
    required double longitude,
    double? heading,
    double? speed,
  }) async {
    await _dio.put(
      ApiConstants.deliveryLocation,
      data: {
        'latitude': latitude,
        'longitude': longitude,
        if (heading != null) 'heading': heading,
        if (speed != null) 'speed': speed,
      },
    );
  }

  Future<Map<String, dynamic>> getDeliveryTracking({
    required String orderId,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.deliveryTracking(orderId),
    );
    return response.data!;
  }
}
