import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';

part 'analytics_remote_data_source.g.dart';

@riverpod
AnalyticsRemoteDataSource analyticsRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return AnalyticsRemoteDataSource(dio: dio);
}

class AnalyticsRemoteDataSource {
  AnalyticsRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  Future<Map<String, dynamic>> getPlatformAnalytics({
    String period = 'daily',
    int days = 30,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.adminAnalytics,
      queryParameters: {'period': period, 'days': days},
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> getRestaurantAnalytics(
    String restaurantId, {
    String period = 'daily',
    int days = 30,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.restaurantAnalytics(restaurantId),
      queryParameters: {'period': period, 'days': days},
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> getPartnerAnalytics({
    String period = 'daily',
    int days = 30,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.deliveryAnalytics,
      queryParameters: {'period': period, 'days': days},
    );
    return response.data!;
  }
}
