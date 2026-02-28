import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/favourite_items_remote_data_source.dart';
import '../models/favourite_item_model.dart';

part 'favourite_items_repository.g.dart';

@riverpod
FavouriteItemsRepository favouriteItemsRepository(Ref ref) {
  final remoteDataSource = ref.watch(favouriteItemsRemoteDataSourceProvider);
  return FavouriteItemsRepository(remoteDataSource: remoteDataSource);
}

class FavouriteItemsRepository {
  FavouriteItemsRepository({
    required FavouriteItemsRemoteDataSource remoteDataSource,
  }) : _remote = remoteDataSource;

  final FavouriteItemsRemoteDataSource _remote;

  Future<({List<FavouriteItemModel>? data, Failure? failure})>
      getFavouriteItems() async {
    try {
      final result = await _remote.getFavouriteItems();
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> addFavouriteItem({required String menuItemId}) async {
    try {
      await _remote.addFavouriteItem(menuItemId: menuItemId);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<Failure?> removeFavouriteItem({required String menuItemId}) async {
    try {
      await _remote.removeFavouriteItem(menuItemId: menuItemId);
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
    var message = 'An unexpected error occurred.';
    if (data is Map<String, dynamic>) {
      message = (data['errorMessage'] as String?) ?? message;
    }
    if (statusCode == 401) return AuthFailure(message: message);
    return ServerFailure(message: message, statusCode: statusCode);
  }
}
