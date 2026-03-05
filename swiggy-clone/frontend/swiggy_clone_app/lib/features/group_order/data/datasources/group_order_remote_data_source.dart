import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../../../cart/data/models/cart_model.dart';
import '../models/group_cart_model.dart';
import '../models/group_order_model.dart';

part 'group_order_remote_data_source.g.dart';

@riverpod
GroupOrderRemoteDataSource groupOrderRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return GroupOrderRemoteDataSource(dio: dio);
}

class GroupOrderRemoteDataSource {
  GroupOrderRemoteDataSource({required Dio dio}) : _dio = dio;
  final Dio _dio;

  Future<GroupOrderModel> createGroupOrder({
    required String restaurantId,
    int paymentSplitType = 0,
    String? deliveryAddressId,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.groupOrders,
      data: {
        'restaurantId': restaurantId,
        'paymentSplitType': paymentSplitType,
        if (deliveryAddressId != null) 'deliveryAddressId': deliveryAddressId,
      },
    );
    return GroupOrderModel.fromJson(response.data!);
  }

  Future<GroupOrderModel> joinGroupOrder({required String inviteCode}) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.groupOrderJoin,
      data: {'inviteCode': inviteCode},
    );
    return GroupOrderModel.fromJson(response.data!);
  }

  Future<GroupOrderModel?> getActiveGroupOrder() async {
    final response = await _dio.get<Map<String, dynamic>?>(
      ApiConstants.groupOrderActive,
    );
    if (response.data == null) return null;
    return GroupOrderModel.fromJson(response.data!);
  }

  Future<GroupOrderModel> getGroupOrderDetail({
    required String groupOrderId,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.groupOrderById(groupOrderId),
    );
    return GroupOrderModel.fromJson(response.data!);
  }

  Future<void> markReady({required String groupOrderId}) async {
    await _dio.put<void>(ApiConstants.groupOrderReady(groupOrderId));
  }

  Future<Map<String, dynamic>> finalizeGroupOrder({
    required String groupOrderId,
    required String deliveryAddressId,
    required int paymentMethod,
    String? couponCode,
    String? specialInstructions,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.groupOrderFinalize(groupOrderId),
      data: {
        'deliveryAddressId': deliveryAddressId,
        'paymentMethod': paymentMethod,
        if (couponCode != null) 'couponCode': couponCode,
        if (specialInstructions != null)
          'specialInstructions': specialInstructions,
      },
    );
    return response.data!;
  }

  Future<void> cancelGroupOrder({required String groupOrderId}) async {
    await _dio.put<void>(ApiConstants.groupOrderCancel(groupOrderId));
  }

  Future<void> leaveGroupOrder({required String groupOrderId}) async {
    await _dio.put<void>(ApiConstants.groupOrderLeave(groupOrderId));
  }

  Future<CartModel> addToCart({
    required String groupOrderId,
    required String menuItemId,
    String? variantId,
    int quantity = 1,
    String? specialInstructions,
    List<Map<String, dynamic>>? addons,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.groupOrderCartItems(groupOrderId),
      data: {
        'menuItemId': menuItemId,
        if (variantId != null) 'variantId': variantId,
        'quantity': quantity,
        if (specialInstructions != null)
          'specialInstructions': specialInstructions,
        if (addons != null) 'addons': addons,
      },
    );
    return CartModel.fromJson(response.data!);
  }

  Future<CartModel> updateCartItem({
    required String groupOrderId,
    required String cartItemId,
    required int quantity,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.groupOrderCartItem(groupOrderId, cartItemId),
      data: {'quantity': quantity},
    );
    return CartModel.fromJson(response.data!);
  }

  Future<CartModel> removeCartItem({
    required String groupOrderId,
    required String cartItemId,
  }) async {
    final response = await _dio.delete<Map<String, dynamic>>(
      ApiConstants.groupOrderCartItem(groupOrderId, cartItemId),
    );
    return CartModel.fromJson(response.data!);
  }

  Future<GroupCartModel> getGroupCart({required String groupOrderId}) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.groupOrderCart(groupOrderId),
    );
    return GroupCartModel.fromJson(response.data!);
  }
}
