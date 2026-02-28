import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/discovery_remote_data_source.dart';
import '../models/browse_result_model.dart';
import '../models/customer_restaurant_model.dart';
import '../models/home_feed_model.dart';
import '../models/menu_item_search_result_model.dart';
import '../models/public_restaurant_detail_model.dart';
import '../models/search_suggestion_model.dart';

part 'discovery_repository.g.dart';

@riverpod
DiscoveryRepository discoveryRepository(Ref ref) {
  final remoteDataSource = ref.watch(discoveryRemoteDataSourceProvider);
  return DiscoveryRepository(remoteDataSource: remoteDataSource);
}

class DiscoveryRepository {
  DiscoveryRepository({required DiscoveryRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final DiscoveryRemoteDataSource _remote;

  // ─────────────────────── Home Feed ──────────────────────────

  Future<({HomeFeedModel? data, Failure? failure})> getHomeFeed({
    String? city,
  }) async {
    try {
      final result = await _remote.getHomeFeed(city: city);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Browse ─────────────────────────────

  Future<({BrowseResultModel? data, Failure? failure})> browseRestaurants({
    String? city,
    String? cuisineId,
    bool? isVegOnly,
    double? minRating,
    int? maxCostForTwo,
    String? sortBy,
    String? cursor,
    int pageSize = 20,
  }) async {
    try {
      final result = await _remote.browseRestaurants(
        city: city,
        cuisineId: cuisineId,
        isVegOnly: isVegOnly,
        minRating: minRating,
        maxCostForTwo: maxCostForTwo,
        sortBy: sortBy,
        cursor: cursor,
        pageSize: pageSize,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Search ─────────────────────────────

  Future<({List<CustomerRestaurantModel>? data, Failure? failure})>
      searchRestaurants({
    required String term,
    String? city,
    int pageSize = 20,
  }) async {
    try {
      final result = await _remote.searchRestaurants(
          term: term, city: city, pageSize: pageSize);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Suggestions ─────────────────────────

  Future<({List<SearchSuggestionModel>? data, Failure? failure})>
      getSuggestions({
    required String prefix,
    String? city,
    int limit = 10,
  }) async {
    try {
      final result = await _remote.getSuggestions(
          prefix: prefix, city: city, limit: limit);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Menu Item Search ──────────────────────

  Future<({List<MenuItemSearchGroupedResultModel>? data, Failure? failure})>
      searchMenuItems({
    required String term,
    String? city,
    int pageSize = 20,
  }) async {
    try {
      final result = await _remote.searchMenuItems(
          term: term, city: city, pageSize: pageSize);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Detail ─────────────────────────────

  Future<({PublicRestaurantDetailModel? data, Failure? failure})>
      getRestaurantDetail({required String id}) async {
    try {
      final result = await _remote.getRestaurantDetail(id: id);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Favourites ─────────────────────────

  Future<({List<CustomerRestaurantModel>? data, Failure? failure})>
      getFavourites() async {
    try {
      final result = await _remote.getFavourites();
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({bool? data, Failure? failure})> checkFavourite({
    required String restaurantId,
  }) async {
    try {
      final result =
          await _remote.checkFavourite(restaurantId: restaurantId);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> addFavourite({required String restaurantId}) async {
    try {
      await _remote.addFavourite(restaurantId: restaurantId);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<Failure?> removeFavourite({required String restaurantId}) async {
    try {
      await _remote.removeFavourite(restaurantId: restaurantId);
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
