import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/personalized_recommendations_model.dart';
import '../models/recommended_menu_item_model.dart';
import '../models/recommended_restaurant_model.dart';
import '../models/trending_item_model.dart';

part 'recommendations_remote_data_source.g.dart';

class RecommendationsRemoteDataSource {
  RecommendationsRemoteDataSource(this._dio);

  final Dio _dio;

  Future<PersonalizedRecommendationsModel> getPersonalized({
    String? city,
  }) async {
    final response = await _dio.get(
      ApiConstants.recommendationsPersonalized,
      queryParameters: {if (city != null) 'city': city},
    );
    return PersonalizedRecommendationsModel.fromJson(
      response.data as Map<String, dynamic>,
    );
  }

  Future<List<TrendingItemModel>> getTrending({
    String? city,
    int count = 20,
  }) async {
    final response = await _dio.get(
      ApiConstants.recommendationsTrending,
      queryParameters: {
        if (city != null) 'city': city,
        'count': count,
      },
    );
    return (response.data as List)
        .map((e) => TrendingItemModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<List<RecommendedRestaurantModel>> getSimilarRestaurants({
    required String restaurantId,
    int count = 10,
  }) async {
    final response = await _dio.get(
      ApiConstants.recommendationsSimilarRestaurants(restaurantId),
      queryParameters: {'count': count},
    );
    return (response.data as List)
        .map((e) =>
            RecommendedRestaurantModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<List<RecommendedMenuItemModel>> getSimilarItems({
    required String menuItemId,
    int count = 10,
  }) async {
    final response = await _dio.get(
      ApiConstants.recommendationsSimilarItems(menuItemId),
      queryParameters: {'count': count},
    );
    return (response.data as List)
        .map((e) =>
            RecommendedMenuItemModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<void> trackInteraction({
    required int entityType,
    required String entityId,
    required int interactionType,
  }) async {
    await _dio.post(
      ApiConstants.recommendationsInteractions,
      data: {
        'entityType': entityType,
        'entityId': entityId,
        'interactionType': interactionType,
      },
    );
  }
}

@riverpod
RecommendationsRemoteDataSource recommendationsRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return RecommendationsRemoteDataSource(dio);
}
