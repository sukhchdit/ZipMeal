import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/admin_remote_data_source.dart';
import '../models/admin_dashboard_model.dart';
import '../models/admin_user_model.dart';
import '../models/admin_restaurant_model.dart';
import '../models/admin_banner_model.dart';
import '../models/admin_order_detail_model.dart';
import '../models/admin_platform_config_model.dart';

part 'admin_repository.g.dart';

@riverpod
AdminRepository adminRepository(Ref ref) {
  final remoteDataSource = ref.watch(adminRemoteDataSourceProvider);
  return AdminRepository(remoteDataSource: remoteDataSource);
}

class AdminRepository {
  AdminRepository({required AdminRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final AdminRemoteDataSource _remote;

  // ─────────────────────── Dashboard ─────────────────────────

  Future<({AdminDashboardModel? data, Failure? failure})>
      getDashboard() async {
    try {
      final raw = await _remote.getDashboard();
      return (
        data: AdminDashboardModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Users ─────────────────────────────

  Future<
      ({
        List<AdminUserModel>? items,
        int? totalCount,
        int? page,
        int? totalPages,
        Failure? failure,
      })> getUsers({
    String? search,
    int? role,
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final data = await _remote.getUsers(
        search: search,
        role: role,
        page: page,
        pageSize: pageSize,
      );
      final items = (data['items'] as List<dynamic>)
          .map((e) => AdminUserModel.fromJson(e as Map<String, dynamic>))
          .toList();
      return (
        items: items,
        totalCount: data['totalCount'] as int?,
        page: data['page'] as int?,
        totalPages: data['totalPages'] as int?,
        failure: null,
      );
    } on DioException catch (e) {
      return (
        items: null,
        totalCount: null,
        page: null,
        totalPages: null,
        failure: _mapDioError(e),
      );
    }
  }

  Future<({AdminUserModel? data, Failure? failure})> getUserDetail(
    String id,
  ) async {
    try {
      final raw = await _remote.getUserDetail(id);
      return (
        data: AdminUserModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({AdminUserModel? data, Failure? failure})> toggleUserActive(
    String id,
    bool isActive,
  ) async {
    try {
      final raw = await _remote.toggleUserActive(id, isActive: isActive);
      return (
        data: AdminUserModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({AdminUserModel? data, Failure? failure})> changeUserRole(
    String id,
    int newRole,
  ) async {
    try {
      final raw = await _remote.changeUserRole(id, newRole: newRole);
      return (
        data: AdminUserModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Restaurants ───────────────────────

  Future<
      ({
        List<AdminRestaurantModel>? items,
        int? totalCount,
        int? page,
        int? totalPages,
        Failure? failure,
      })> getRestaurants({
    int? status,
    String? search,
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final data = await _remote.getRestaurants(
        status: status,
        search: search,
        page: page,
        pageSize: pageSize,
      );
      final items = (data['items'] as List<dynamic>)
          .map((e) =>
              AdminRestaurantModel.fromJson(e as Map<String, dynamic>))
          .toList();
      return (
        items: items,
        totalCount: data['totalCount'] as int?,
        page: data['page'] as int?,
        totalPages: data['totalPages'] as int?,
        failure: null,
      );
    } on DioException catch (e) {
      return (
        items: null,
        totalCount: null,
        page: null,
        totalPages: null,
        failure: _mapDioError(e),
      );
    }
  }

  Future<({AdminRestaurantModel? data, Failure? failure})>
      getRestaurantDetail(String id) async {
    try {
      final raw = await _remote.getRestaurantDetail(id);
      return (
        data: AdminRestaurantModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({AdminRestaurantModel? data, Failure? failure})>
      approveRestaurant(String id) async {
    try {
      final raw = await _remote.approveRestaurant(id);
      return (
        data: AdminRestaurantModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({AdminRestaurantModel? data, Failure? failure})>
      rejectRestaurant(String id, String reason) async {
    try {
      final raw = await _remote.rejectRestaurant(id, reason: reason);
      return (
        data: AdminRestaurantModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({AdminRestaurantModel? data, Failure? failure})>
      suspendRestaurant(String id, String reason) async {
    try {
      final raw = await _remote.suspendRestaurant(id, reason: reason);
      return (
        data: AdminRestaurantModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({AdminRestaurantModel? data, Failure? failure})>
      reactivateRestaurant(String id) async {
    try {
      final raw = await _remote.reactivateRestaurant(id);
      return (
        data: AdminRestaurantModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Orders ────────────────────────────

  Future<
      ({
        List<AdminOrderSummaryModel>? items,
        int? totalCount,
        int? page,
        int? totalPages,
        Failure? failure,
      })> getOrders({
    int? status,
    DateTime? fromDate,
    DateTime? toDate,
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final data = await _remote.getOrders(
        status: status,
        fromDate: fromDate,
        toDate: toDate,
        page: page,
        pageSize: pageSize,
      );
      final items = (data['items'] as List<dynamic>)
          .map((e) =>
              AdminOrderSummaryModel.fromJson(e as Map<String, dynamic>))
          .toList();
      return (
        items: items,
        totalCount: data['totalCount'] as int?,
        page: data['page'] as int?,
        totalPages: data['totalPages'] as int?,
        failure: null,
      );
    } on DioException catch (e) {
      return (
        items: null,
        totalCount: null,
        page: null,
        totalPages: null,
        failure: _mapDioError(e),
      );
    }
  }

  Future<({AdminOrderDetailModel? data, Failure? failure})> getOrderDetail(
    String id,
  ) async {
    try {
      final raw = await _remote.getOrderDetail(id);
      return (
        data: AdminOrderDetailModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Banners ──────────────────────────

  Future<
      ({
        List<AdminBannerModel>? items,
        int? totalCount,
        int? page,
        int? totalPages,
        Failure? failure,
      })> getBanners({
    String? search,
    bool? isActive,
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final data = await _remote.getBanners(
        search: search,
        isActive: isActive,
        page: page,
        pageSize: pageSize,
      );
      final items = (data['items'] as List<dynamic>)
          .map((e) => AdminBannerModel.fromJson(e as Map<String, dynamic>))
          .toList();
      return (
        items: items,
        totalCount: data['totalCount'] as int?,
        page: data['page'] as int?,
        totalPages: data['totalPages'] as int?,
        failure: null,
      );
    } on DioException catch (e) {
      return (
        items: null,
        totalCount: null,
        page: null,
        totalPages: null,
        failure: _mapDioError(e),
      );
    }
  }

  Future<({AdminBannerModel? data, Failure? failure})> createBanner(
    Map<String, dynamic> data,
  ) async {
    try {
      final raw = await _remote.createBanner(data: data);
      return (
        data: AdminBannerModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({AdminBannerModel? data, Failure? failure})> updateBanner(
    String id,
    Map<String, dynamic> data,
  ) async {
    try {
      final raw = await _remote.updateBanner(id, data: data);
      return (
        data: AdminBannerModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> toggleBanner(String id, bool isActive) async {
    try {
      await _remote.toggleBanner(id, isActive: isActive);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  // ─────────────────────── Platform Config ─────────────────

  Future<({AdminPlatformConfigModel? data, Failure? failure})>
      getPlatformConfig() async {
    try {
      final raw = await _remote.getPlatformConfig();
      return (
        data: AdminPlatformConfigModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({AdminPlatformConfigModel? data, Failure? failure})>
      updatePlatformConfig(Map<String, dynamic> data) async {
    try {
      final raw = await _remote.updatePlatformConfig(data: data);
      return (
        data: AdminPlatformConfigModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Helpers ───────────────────────────

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
