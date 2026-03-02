import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/analytics_remote_data_source.dart';
import '../models/customer_funnel_model.dart';
import '../models/partner_analytics_model.dart';
import '../models/platform_analytics_model.dart';
import '../models/restaurant_analytics_model.dart';
import '../models/restaurant_insights_model.dart';
import '../models/revenue_forecast_model.dart';

part 'analytics_repository.g.dart';

@riverpod
AnalyticsRepository analyticsRepository(Ref ref) {
  final remoteDataSource = ref.watch(analyticsRemoteDataSourceProvider);
  return AnalyticsRepository(remoteDataSource: remoteDataSource);
}

class AnalyticsRepository {
  AnalyticsRepository({required AnalyticsRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final AnalyticsRemoteDataSource _remote;

  Future<({PlatformAnalyticsModel? data, Failure? failure})>
      getPlatformAnalytics({
    String period = 'daily',
    int days = 30,
  }) async {
    try {
      final json = await _remote.getPlatformAnalytics(
        period: period,
        days: days,
      );
      return (data: PlatformAnalyticsModel.fromJson(json), failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({RestaurantAnalyticsModel? data, Failure? failure})>
      getRestaurantAnalytics(
    String restaurantId, {
    String period = 'daily',
    int days = 30,
  }) async {
    try {
      final json = await _remote.getRestaurantAnalytics(
        restaurantId,
        period: period,
        days: days,
      );
      return (data: RestaurantAnalyticsModel.fromJson(json), failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({PartnerAnalyticsModel? data, Failure? failure})>
      getPartnerAnalytics({
    String period = 'daily',
    int days = 30,
  }) async {
    try {
      final json = await _remote.getPartnerAnalytics(
        period: period,
        days: days,
      );
      return (data: PartnerAnalyticsModel.fromJson(json), failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────── Advanced Analytics (Module 47) ─────────────────

  Future<({RestaurantInsightsModel? data, Failure? failure})>
      getRestaurantInsights(
    String restaurantId, {
    String period = 'daily',
    int days = 30,
  }) async {
    try {
      final json = await _remote.getRestaurantInsights(
        restaurantId,
        period: period,
        days: days,
      );
      return (data: RestaurantInsightsModel.fromJson(json), failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({CustomerFunnelModel? data, Failure? failure})> getCustomerFunnel({
    int days = 30,
  }) async {
    try {
      final json = await _remote.getCustomerFunnel(days: days);
      return (data: CustomerFunnelModel.fromJson(json), failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({RevenueForecastModel? data, Failure? failure})> getRevenueForecast({
    int days = 30,
    int forecastDays = 14,
  }) async {
    try {
      final json = await _remote.getRevenueForecast(
        days: days,
        forecastDays: forecastDays,
      );
      return (data: RevenueForecastModel.fromJson(json), failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({RevenueForecastModel? data, Failure? failure})>
      getRestaurantForecast(
    String restaurantId, {
    int days = 30,
    int forecastDays = 14,
  }) async {
    try {
      final json = await _remote.getRestaurantForecast(
        restaurantId,
        days: days,
        forecastDays: forecastDays,
      );
      return (data: RevenueForecastModel.fromJson(json), failure: null);
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
    var message = 'An unexpected error occurred.';
    if (data is Map<String, dynamic>) {
      message = (data['errorMessage'] as String?) ?? message;
    }
    if (statusCode == 401) return AuthFailure(message: message);
    return ServerFailure(message: message, statusCode: statusCode);
  }
}
