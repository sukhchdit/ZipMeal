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

  // ─────────────────── Advanced Analytics (Module 47) ─────────────────

  Future<Map<String, dynamic>> getRestaurantInsights(
    String restaurantId, {
    String period = 'daily',
    int days = 30,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.restaurantInsights(restaurantId),
      queryParameters: {'period': period, 'days': days},
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> getCustomerFunnel({
    int days = 30,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.adminAnalyticsFunnel,
      queryParameters: {'days': days},
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> getRevenueForecast({
    int days = 30,
    int forecastDays = 14,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.adminAnalyticsForecast,
      queryParameters: {'days': days, 'forecastDays': forecastDays},
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> getRestaurantForecast(
    String restaurantId, {
    int days = 30,
    int forecastDays = 14,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.restaurantForecast(restaurantId),
      queryParameters: {'days': days, 'forecastDays': forecastDays},
    );
    return response.data!;
  }
}
