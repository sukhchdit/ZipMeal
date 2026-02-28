import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/cart_model.dart';

part 'cart_remote_data_source.g.dart';

@riverpod
CartRemoteDataSource cartRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return CartRemoteDataSource(dio: dio);
}

class CartRemoteDataSource {
  CartRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  Future<CartModel> getCart() async {
    final response = await _dio.get(ApiConstants.cart);
    return CartModel.fromJson(response.data as Map<String, dynamic>);
  }

  Future<CartModel> addToCart({
    required String restaurantId,
    required String menuItemId,
    String? variantId,
    required int quantity,
    String? specialInstructions,
    required List<Map<String, dynamic>> addons,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      '${ApiConstants.cart}/items',
      data: {
        'restaurantId': restaurantId,
        'menuItemId': menuItemId,
        if (variantId != null) 'variantId': variantId,
        'quantity': quantity,
        if (specialInstructions != null)
          'specialInstructions': specialInstructions,
        'addons': addons,
      },
    );
    return CartModel.fromJson(response.data!);
  }

  Future<CartModel> updateQuantity({
    required String cartItemId,
    required int quantity,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      '${ApiConstants.cart}/items/$cartItemId',
      data: {'quantity': quantity},
    );
    return CartModel.fromJson(response.data!);
  }

  Future<CartModel> removeItem({required String cartItemId}) async {
    final response = await _dio.delete<Map<String, dynamic>>(
      '${ApiConstants.cart}/items/$cartItemId',
    );
    return CartModel.fromJson(response.data!);
  }

  Future<void> clearCart() async {
    await _dio.delete(ApiConstants.cart);
  }
}
