import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/recommendations_remote_data_source.dart';
import '../models/personalized_recommendations_model.dart';
import '../models/recommended_menu_item_model.dart';
import '../models/recommended_restaurant_model.dart';
import '../models/trending_item_model.dart';

part 'recommendations_repository.g.dart';

class RecommendationsRepository {
  RecommendationsRepository(this._dataSource);

  final RecommendationsRemoteDataSource _dataSource;

  Future<({PersonalizedRecommendationsModel? data, Failure? failure})>
      getPersonalized({String? city}) async {
    try {
      final data = await _dataSource.getPersonalized(city: city);
      return (data: data, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({List<TrendingItemModel>? data, Failure? failure})> getTrending({
    String? city,
    int count = 20,
  }) async {
    try {
      final data = await _dataSource.getTrending(city: city, count: count);
      return (data: data, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({List<RecommendedRestaurantModel>? data, Failure? failure})>
      getSimilarRestaurants({
    required String restaurantId,
    int count = 10,
  }) async {
    try {
      final data = await _dataSource.getSimilarRestaurants(
        restaurantId: restaurantId,
        count: count,
      );
      return (data: data, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({List<RecommendedMenuItemModel>? data, Failure? failure})>
      getSimilarItems({
    required String menuItemId,
    int count = 10,
  }) async {
    try {
      final data = await _dataSource.getSimilarItems(
        menuItemId: menuItemId,
        count: count,
      );
      return (data: data, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({bool success, Failure? failure})> trackInteraction({
    required int entityType,
    required String entityId,
    required int interactionType,
  }) async {
    try {
      await _dataSource.trackInteraction(
        entityType: entityType,
        entityId: entityId,
        interactionType: interactionType,
      );
      return (success: true, failure: null);
    } on DioException catch (e) {
      return (success: false, failure: _mapDioError(e));
    }
  }

  Failure _mapDioError(DioException e) {
    if (e.type == DioExceptionType.connectionError ||
        e.type == DioExceptionType.connectionTimeout) {
      return const NetworkFailure();
    }
    final statusCode = e.response?.statusCode;
    if (statusCode == 401) {
      return const AuthFailure();
    }
    final data = e.response?.data;
    final message = data is Map<String, dynamic>
        ? data['errorMessage'] as String? ?? 'An error occurred'
        : 'An error occurred';
    return ServerFailure(message: message, statusCode: statusCode);
  }
}

@riverpod
RecommendationsRepository recommendationsRepository(Ref ref) {
  final dataSource = ref.watch(recommendationsRemoteDataSourceProvider);
  return RecommendationsRepository(dataSource);
}
