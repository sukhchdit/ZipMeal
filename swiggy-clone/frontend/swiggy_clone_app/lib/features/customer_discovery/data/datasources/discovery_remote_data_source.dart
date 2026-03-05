import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/browse_result_model.dart';
import '../models/customer_restaurant_model.dart';
import '../models/home_feed_model.dart';
import '../models/menu_item_search_result_model.dart';
import '../models/public_restaurant_detail_model.dart';
import '../models/search_suggestion_model.dart';

part 'discovery_remote_data_source.g.dart';

@riverpod
DiscoveryRemoteDataSource discoveryRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return DiscoveryRemoteDataSource(dio: dio);
}

class DiscoveryRemoteDataSource {
  DiscoveryRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;
  static const _base = '${ApiConstants.apiPrefix}/discovery';

  // ─────────────────────── Home Feed ──────────────────────────

  Future<HomeFeedModel> getHomeFeed({String? city}) async {
    final response = await _dio.get(
      '$_base/home',
      queryParameters: {if (city != null) 'city': city},
    );
    return HomeFeedModel.fromJson(response.data as Map<String, dynamic>);
  }

  // ─────────────────────── Browse ─────────────────────────────

  Future<BrowseResultModel> browseRestaurants({
    String? city,
    String? cuisineId,
    bool? isVegOnly,
    double? minRating,
    int? maxCostForTwo,
    String? sortBy,
    String? cursor,
    int pageSize = 20,
  }) async {
    final response = await _dio.get(
      '$_base/restaurants',
      queryParameters: {
        if (city != null) 'city': city,
        if (cuisineId != null) 'cuisineId': cuisineId,
        if (isVegOnly != null) 'isVegOnly': isVegOnly,
        if (minRating != null) 'minRating': minRating,
        if (maxCostForTwo != null) 'maxCostForTwo': maxCostForTwo,
        if (sortBy != null) 'sortBy': sortBy,
        if (cursor != null) 'cursor': cursor,
        'pageSize': pageSize,
      },
    );
    return BrowseResultModel.fromJson(response.data as Map<String, dynamic>);
  }

  // ─────────────────────── Search ─────────────────────────────

  Future<List<CustomerRestaurantModel>> searchRestaurants({
    required String term,
    String? city,
    int pageSize = 20,
  }) async {
    final response = await _dio.get(
      '$_base/restaurants/search',
      queryParameters: {
        'term': term,
        if (city != null) 'city': city,
        'pageSize': pageSize,
      },
    );
    return (response.data as List<dynamic>)
        .map((e) => CustomerRestaurantModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  // ─────────────────────── Suggestions ─────────────────────────

  Future<List<SearchSuggestionModel>> getSuggestions({
    required String prefix,
    String? city,
    int limit = 10,
  }) async {
    final response = await _dio.get(
      '$_base/suggestions',
      queryParameters: {
        'prefix': prefix,
        if (city != null) 'city': city,
        'limit': limit,
      },
    );
    return (response.data as List<dynamic>)
        .map((e) =>
            SearchSuggestionModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  // ─────────────────────── Menu Item Search ──────────────────────

  Future<List<MenuItemSearchGroupedResultModel>> searchMenuItems({
    required String term,
    String? city,
    int pageSize = 20,
  }) async {
    final response = await _dio.get(
      '$_base/menu-items/search',
      queryParameters: {
        'term': term,
        if (city != null) 'city': city,
        'pageSize': pageSize,
      },
    );
    return (response.data as List<dynamic>)
        .map((e) => MenuItemSearchGroupedResultModel.fromJson(
            e as Map<String, dynamic>))
        .toList();
  }

  // ─────────────────────── Detail ─────────────────────────────

  Future<PublicRestaurantDetailModel> getRestaurantDetail({
    required String id,
  }) async {
    final response = await _dio.get('$_base/restaurants/$id');
    return PublicRestaurantDetailModel.fromJson(
        response.data as Map<String, dynamic>);
  }

  // ─────────────────────── Favourites ─────────────────────────

  Future<List<CustomerRestaurantModel>> getFavourites() async {
    final response =
        await _dio.get('${ApiConstants.apiPrefix}/favourites');
    return (response.data as List<dynamic>)
        .map((e) => CustomerRestaurantModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<bool> checkFavourite({required String restaurantId}) async {
    final response = await _dio
        .get('${ApiConstants.apiPrefix}/favourites/$restaurantId');
    return (response.data as Map<String, dynamic>)['isFavourited'] as bool;
  }

  Future<void> addFavourite({required String restaurantId}) async {
    await _dio.post('${ApiConstants.apiPrefix}/favourites/$restaurantId');
  }

  Future<void> removeFavourite({required String restaurantId}) async {
    await _dio.delete('${ApiConstants.apiPrefix}/favourites/$restaurantId');
  }
}
