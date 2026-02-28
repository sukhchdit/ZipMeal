import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';

part 'admin_remote_data_source.g.dart';

@riverpod
AdminRemoteDataSource adminRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return AdminRemoteDataSource(dio: dio);
}

class AdminRemoteDataSource {
  AdminRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  // ─────────────────────── Dashboard ─────────────────────────

  Future<Map<String, dynamic>> getDashboard() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.adminDashboard,
    );
    return response.data!;
  }

  // ─────────────────────── Users ─────────────────────────────

  Future<Map<String, dynamic>> getUsers({
    String? search,
    int? role,
    int page = 1,
    int pageSize = 20,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.adminUsers,
      queryParameters: {
        if (search != null) 'search': search,
        if (role != null) 'role': role,
        'page': page,
        'pageSize': pageSize,
      },
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> getUserDetail(String id) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.adminUserById(id),
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> toggleUserActive(
    String id, {
    required bool isActive,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.adminUserActive(id),
      data: {'isActive': isActive},
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> changeUserRole(
    String id, {
    required int newRole,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.adminUserRole(id),
      data: {'newRole': newRole},
    );
    return response.data!;
  }

  // ─────────────────────── Restaurants ───────────────────────

  Future<Map<String, dynamic>> getRestaurants({
    int? status,
    String? search,
    int page = 1,
    int pageSize = 20,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.adminRestaurants,
      queryParameters: {
        if (status != null) 'status': status,
        if (search != null) 'search': search,
        'page': page,
        'pageSize': pageSize,
      },
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> getRestaurantDetail(String id) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.adminRestaurantById(id),
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> approveRestaurant(String id) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.adminRestaurantApprove(id),
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> rejectRestaurant(
    String id, {
    required String reason,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.adminRestaurantReject(id),
      data: {'reason': reason},
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> suspendRestaurant(
    String id, {
    required String reason,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.adminRestaurantSuspend(id),
      data: {'reason': reason},
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> reactivateRestaurant(String id) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.adminRestaurantReactivate(id),
    );
    return response.data!;
  }

  // ─────────────────────── Orders ────────────────────────────

  Future<Map<String, dynamic>> getOrders({
    int? status,
    DateTime? fromDate,
    DateTime? toDate,
    int page = 1,
    int pageSize = 20,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.adminOrders,
      queryParameters: {
        if (status != null) 'status': status,
        if (fromDate != null) 'fromDate': fromDate.toIso8601String(),
        if (toDate != null) 'toDate': toDate.toIso8601String(),
        'page': page,
        'pageSize': pageSize,
      },
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> getOrderDetail(String id) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.adminOrderById(id),
    );
    return response.data!;
  }

  // ─────────────────────── Banners ─────────────────────────

  Future<Map<String, dynamic>> getBanners({
    String? search,
    bool? isActive,
    int page = 1,
    int pageSize = 20,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.adminBanners,
      queryParameters: {
        if (search != null) 'search': search,
        if (isActive != null) 'isActive': isActive,
        'page': page,
        'pageSize': pageSize,
      },
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> createBanner({
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.adminBanners,
      data: data,
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> updateBanner(
    String id, {
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.adminBannerById(id),
      data: data,
    );
    return response.data!;
  }

  Future<void> toggleBanner(
    String id, {
    required bool isActive,
  }) async {
    await _dio.put<void>(
      ApiConstants.adminBannerToggle(id),
      data: {'isActive': isActive},
    );
  }

  // ─────────────────────── Platform Config ─────────────────

  Future<Map<String, dynamic>> getPlatformConfig() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.adminConfig,
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> updatePlatformConfig({
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.adminConfig,
      data: data,
    );
    return response.data!;
  }
}
