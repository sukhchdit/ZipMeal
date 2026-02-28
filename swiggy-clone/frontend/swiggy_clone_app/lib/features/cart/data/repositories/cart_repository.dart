import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/cart_remote_data_source.dart';
import '../models/cart_model.dart';

part 'cart_repository.g.dart';

@riverpod
CartRepository cartRepository(Ref ref) {
  final remoteDataSource = ref.watch(cartRemoteDataSourceProvider);
  return CartRepository(remoteDataSource: remoteDataSource);
}

class CartRepository {
  CartRepository({required CartRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final CartRemoteDataSource _remote;

  Future<({CartModel? data, Failure? failure})> getCart() async {
    try {
      final result = await _remote.getCart();
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({CartModel? data, Failure? failure, String? errorCode})> addToCart({
    required String restaurantId,
    required String menuItemId,
    String? variantId,
    required int quantity,
    String? specialInstructions,
    required List<Map<String, dynamic>> addons,
  }) async {
    try {
      final result = await _remote.addToCart(
        restaurantId: restaurantId,
        menuItemId: menuItemId,
        variantId: variantId,
        quantity: quantity,
        specialInstructions: specialInstructions,
        addons: addons,
      );
      return (data: result, failure: null, errorCode: null);
    } on DioException catch (e) {
      final errorCode = _extractErrorCode(e);
      return (data: null, failure: _mapDioError(e), errorCode: errorCode);
    }
  }

  Future<({CartModel? data, Failure? failure})> updateQuantity({
    required String cartItemId,
    required int quantity,
  }) async {
    try {
      final result = await _remote.updateQuantity(
        cartItemId: cartItemId,
        quantity: quantity,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({CartModel? data, Failure? failure})> removeItem({
    required String cartItemId,
  }) async {
    try {
      final result = await _remote.removeItem(cartItemId: cartItemId);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> clearCart() async {
    try {
      await _remote.clearCart();
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  String? _extractErrorCode(DioException e) {
    final data = e.response?.data;
    if (data is Map<String, dynamic>) {
      return data['errorCode'] as String?;
    }
    return null;
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
