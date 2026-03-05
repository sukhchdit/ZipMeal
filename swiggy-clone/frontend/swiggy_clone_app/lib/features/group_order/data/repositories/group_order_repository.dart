import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../../cart/data/models/cart_model.dart';
import '../datasources/group_order_remote_data_source.dart';
import '../models/group_cart_model.dart';
import '../models/group_order_model.dart';

part 'group_order_repository.g.dart';

@riverpod
GroupOrderRepository groupOrderRepository(Ref ref) {
  final remoteDataSource = ref.watch(groupOrderRemoteDataSourceProvider);
  return GroupOrderRepository(remoteDataSource: remoteDataSource);
}

class GroupOrderRepository {
  GroupOrderRepository({
    required GroupOrderRemoteDataSource remoteDataSource,
  }) : _remote = remoteDataSource;
  final GroupOrderRemoteDataSource _remote;

  Future<({GroupOrderModel? data, Failure? failure})> createGroupOrder({
    required String restaurantId,
    int paymentSplitType = 0,
    String? deliveryAddressId,
  }) async {
    try {
      final result = await _remote.createGroupOrder(
        restaurantId: restaurantId,
        paymentSplitType: paymentSplitType,
        deliveryAddressId: deliveryAddressId,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({GroupOrderModel? data, Failure? failure})> joinGroupOrder({
    required String inviteCode,
  }) async {
    try {
      final result = await _remote.joinGroupOrder(inviteCode: inviteCode);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({GroupOrderModel? data, Failure? failure})> getActiveGroupOrder() async {
    try {
      final result = await _remote.getActiveGroupOrder();
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({GroupOrderModel? data, Failure? failure})> getGroupOrderDetail({
    required String groupOrderId,
  }) async {
    try {
      final result = await _remote.getGroupOrderDetail(
        groupOrderId: groupOrderId,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> markReady({required String groupOrderId}) async {
    try {
      await _remote.markReady(groupOrderId: groupOrderId);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<({Map<String, dynamic>? data, Failure? failure})> finalizeGroupOrder({
    required String groupOrderId,
    required String deliveryAddressId,
    required int paymentMethod,
    String? couponCode,
    String? specialInstructions,
  }) async {
    try {
      final result = await _remote.finalizeGroupOrder(
        groupOrderId: groupOrderId,
        deliveryAddressId: deliveryAddressId,
        paymentMethod: paymentMethod,
        couponCode: couponCode,
        specialInstructions: specialInstructions,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> cancelGroupOrder({required String groupOrderId}) async {
    try {
      await _remote.cancelGroupOrder(groupOrderId: groupOrderId);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<Failure?> leaveGroupOrder({required String groupOrderId}) async {
    try {
      await _remote.leaveGroupOrder(groupOrderId: groupOrderId);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<({CartModel? data, Failure? failure})> addToCart({
    required String groupOrderId,
    required String menuItemId,
    String? variantId,
    int quantity = 1,
    String? specialInstructions,
    List<Map<String, dynamic>>? addons,
  }) async {
    try {
      final result = await _remote.addToCart(
        groupOrderId: groupOrderId,
        menuItemId: menuItemId,
        variantId: variantId,
        quantity: quantity,
        specialInstructions: specialInstructions,
        addons: addons,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({CartModel? data, Failure? failure})> updateCartItem({
    required String groupOrderId,
    required String cartItemId,
    required int quantity,
  }) async {
    try {
      final result = await _remote.updateCartItem(
        groupOrderId: groupOrderId,
        cartItemId: cartItemId,
        quantity: quantity,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({CartModel? data, Failure? failure})> removeCartItem({
    required String groupOrderId,
    required String cartItemId,
  }) async {
    try {
      final result = await _remote.removeCartItem(
        groupOrderId: groupOrderId,
        cartItemId: cartItemId,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({GroupCartModel? data, Failure? failure})> getGroupCart({
    required String groupOrderId,
  }) async {
    try {
      final result =
          await _remote.getGroupCart(groupOrderId: groupOrderId);
      return (data: result, failure: null);
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
    String message = 'An unexpected error occurred.';
    if (data is Map<String, dynamic>) {
      message = (data['errorMessage'] as String?) ?? message;
    }
    if (statusCode == 401) return AuthFailure(message: message);
    return ServerFailure(message: message, statusCode: statusCode);
  }
}
