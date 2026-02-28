import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/favourite_item_model.dart';

part 'favourite_items_remote_data_source.g.dart';

@riverpod
FavouriteItemsRemoteDataSource favouriteItemsRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return FavouriteItemsRemoteDataSource(dio: dio);
}

class FavouriteItemsRemoteDataSource {
  FavouriteItemsRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  Future<List<FavouriteItemModel>> getFavouriteItems() async {
    final response = await _dio.get<List<dynamic>>(ApiConstants.favouriteItems);
    return response.data!
        .map((e) => FavouriteItemModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<bool> checkFavouriteItem({required String menuItemId}) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.favouriteItemById(menuItemId),
    );
    return response.data!['isFavourited'] as bool;
  }

  Future<void> addFavouriteItem({required String menuItemId}) async {
    await _dio.post<void>(ApiConstants.favouriteItemById(menuItemId));
  }

  Future<void> removeFavouriteItem({required String menuItemId}) async {
    await _dio.delete<void>(ApiConstants.favouriteItemById(menuItemId));
  }
}
