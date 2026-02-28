import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/restaurant_remote_data_source.dart';
import '../models/cuisine_type_model.dart';
import '../models/file_upload_result_model.dart';
import '../models/menu_category_model.dart';
import '../models/menu_item_model.dart';
import '../models/operating_hours_model.dart';
import '../models/owner_dine_in_order_model.dart';
import '../models/owner_session_model.dart';
import '../models/restaurant_dashboard_model.dart';
import '../models/restaurant_model.dart';
import '../models/restaurant_summary_model.dart';
import '../models/restaurant_table_model.dart';

part 'restaurant_repository.g.dart';

@riverpod
RestaurantRepository restaurantRepository(Ref ref) {
  final remoteDataSource = ref.watch(restaurantRemoteDataSourceProvider);
  return RestaurantRepository(remoteDataSource: remoteDataSource);
}

/// Repository that mediates between the restaurant data source and the
/// presentation layer, handling error mapping.
///
/// Every public method returns a record with nullable [data] and [Failure]
/// fields, following the "result" pattern to avoid throwing exceptions into
/// the UI layer.
class RestaurantRepository {
  RestaurantRepository({
    required RestaurantRemoteDataSource remoteDataSource,
  }) : _remoteDataSource = remoteDataSource;

  final RestaurantRemoteDataSource _remoteDataSource;

  // ─────────────────────── Restaurant CRUD ──────────────────────────────

  Future<({RestaurantModel? data, Failure? failure})> registerRestaurant({
    required Map<String, dynamic> data,
  }) async {
    try {
      final result = await _remoteDataSource.registerRestaurant(data: data);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({List<RestaurantSummaryModel>? data, Failure? failure})>
      getMyRestaurants() async {
    try {
      final result = await _remoteDataSource.getMyRestaurants();
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({RestaurantModel? data, Failure? failure})> getRestaurantById({
    required String id,
  }) async {
    try {
      final result = await _remoteDataSource.getRestaurantById(id: id);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({RestaurantModel? data, Failure? failure})> updateRestaurant({
    required String id,
    required Map<String, dynamic> data,
  }) async {
    try {
      final result = await _remoteDataSource.updateRestaurant(
        id: id,
        data: data,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({bool? data, Failure? failure})> toggleAcceptingOrders({
    required String id,
    required bool value,
  }) async {
    try {
      final result = await _remoteDataSource.toggleAcceptingOrders(
        id: id,
        value: value,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({bool? data, Failure? failure})> toggleDineIn({
    required String id,
    required bool value,
  }) async {
    try {
      final result = await _remoteDataSource.toggleDineIn(
        id: id,
        value: value,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({RestaurantDashboardModel? data, Failure? failure})> getDashboard({
    required String id,
  }) async {
    try {
      final result = await _remoteDataSource.getDashboard(id: id);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({FileUploadResultModel? data, Failure? failure})> uploadFile({
    required String id,
    required String fileType,
    required String filePath,
    required String fileName,
  }) async {
    try {
      final result = await _remoteDataSource.uploadFile(
        id: id,
        fileType: fileType,
        filePath: filePath,
        fileName: fileName,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Menu Categories ─────────────────────────────

  Future<({List<MenuCategoryModel>? data, Failure? failure})>
      getMenuCategories({
    required String restaurantId,
  }) async {
    try {
      final result = await _remoteDataSource.getMenuCategories(
        restaurantId: restaurantId,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({MenuCategoryModel? data, Failure? failure})> createMenuCategory({
    required String restaurantId,
    required Map<String, dynamic> data,
  }) async {
    try {
      final result = await _remoteDataSource.createMenuCategory(
        restaurantId: restaurantId,
        data: data,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({MenuCategoryModel? data, Failure? failure})> updateMenuCategory({
    required String restaurantId,
    required String categoryId,
    required Map<String, dynamic> data,
  }) async {
    try {
      final result = await _remoteDataSource.updateMenuCategory(
        restaurantId: restaurantId,
        categoryId: categoryId,
        data: data,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> deleteMenuCategory({
    required String restaurantId,
    required String categoryId,
  }) async {
    try {
      await _remoteDataSource.deleteMenuCategory(
        restaurantId: restaurantId,
        categoryId: categoryId,
      );
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  // ─────────────────────── Menu Items ──────────────────────────────────

  Future<({List<MenuItemModel>? data, Failure? failure})> getMenuItems({
    required String restaurantId,
    required String categoryId,
  }) async {
    try {
      final result = await _remoteDataSource.getMenuItems(
        restaurantId: restaurantId,
        categoryId: categoryId,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({MenuItemModel? data, Failure? failure})> getMenuItemById({
    required String restaurantId,
    required String itemId,
  }) async {
    try {
      final result = await _remoteDataSource.getMenuItemById(
        restaurantId: restaurantId,
        itemId: itemId,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({MenuItemModel? data, Failure? failure})> createMenuItem({
    required String restaurantId,
    required Map<String, dynamic> data,
  }) async {
    try {
      final result = await _remoteDataSource.createMenuItem(
        restaurantId: restaurantId,
        data: data,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({MenuItemModel? data, Failure? failure})> updateMenuItem({
    required String restaurantId,
    required String itemId,
    required Map<String, dynamic> data,
  }) async {
    try {
      final result = await _remoteDataSource.updateMenuItem(
        restaurantId: restaurantId,
        itemId: itemId,
        data: data,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> deleteMenuItem({
    required String restaurantId,
    required String itemId,
  }) async {
    try {
      await _remoteDataSource.deleteMenuItem(
        restaurantId: restaurantId,
        itemId: itemId,
      );
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  // ─────────────────────── Variants ────────────────────────────────────

  Future<({MenuItemVariantModel? data, Failure? failure})> addVariant({
    required String restaurantId,
    required String itemId,
    required Map<String, dynamic> data,
  }) async {
    try {
      final result = await _remoteDataSource.addVariant(
        restaurantId: restaurantId,
        itemId: itemId,
        data: data,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({MenuItemVariantModel? data, Failure? failure})> updateVariant({
    required String restaurantId,
    required String itemId,
    required String variantId,
    required Map<String, dynamic> data,
  }) async {
    try {
      final result = await _remoteDataSource.updateVariant(
        restaurantId: restaurantId,
        itemId: itemId,
        variantId: variantId,
        data: data,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> deleteVariant({
    required String restaurantId,
    required String itemId,
    required String variantId,
  }) async {
    try {
      await _remoteDataSource.deleteVariant(
        restaurantId: restaurantId,
        itemId: itemId,
        variantId: variantId,
      );
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  // ─────────────────────── Addons ──────────────────────────────────────

  Future<({MenuItemAddonModel? data, Failure? failure})> addAddon({
    required String restaurantId,
    required String itemId,
    required Map<String, dynamic> data,
  }) async {
    try {
      final result = await _remoteDataSource.addAddon(
        restaurantId: restaurantId,
        itemId: itemId,
        data: data,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({MenuItemAddonModel? data, Failure? failure})> updateAddon({
    required String restaurantId,
    required String itemId,
    required String addonId,
    required Map<String, dynamic> data,
  }) async {
    try {
      final result = await _remoteDataSource.updateAddon(
        restaurantId: restaurantId,
        itemId: itemId,
        addonId: addonId,
        data: data,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> deleteAddon({
    required String restaurantId,
    required String itemId,
    required String addonId,
  }) async {
    try {
      await _remoteDataSource.deleteAddon(
        restaurantId: restaurantId,
        itemId: itemId,
        addonId: addonId,
      );
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  // ─────────────────────── Operating Hours ─────────────────────────────

  Future<({List<OperatingHoursModel>? data, Failure? failure})>
      getOperatingHours({
    required String restaurantId,
  }) async {
    try {
      final result = await _remoteDataSource.getOperatingHours(
        restaurantId: restaurantId,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({List<OperatingHoursModel>? data, Failure? failure})>
      upsertOperatingHours({
    required String restaurantId,
    required Map<String, dynamic> data,
  }) async {
    try {
      final result = await _remoteDataSource.upsertOperatingHours(
        restaurantId: restaurantId,
        data: data,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Cuisines ────────────────────────────────────

  Future<({List<CuisineTypeModel>? data, Failure? failure})>
      getCuisineTypes() async {
    try {
      final result = await _remoteDataSource.getCuisineTypes();
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Table Management ────────────────────────────

  Future<({List<RestaurantTableModel>? data, Failure? failure})> getTables({
    required String restaurantId,
  }) async {
    try {
      final result =
          await _remoteDataSource.getTables(restaurantId: restaurantId);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({RestaurantTableModel? data, Failure? failure})> createTable({
    required String restaurantId,
    required Map<String, dynamic> data,
  }) async {
    try {
      final result = await _remoteDataSource.createTable(
        restaurantId: restaurantId,
        data: data,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({RestaurantTableModel? data, Failure? failure})> updateTable({
    required String restaurantId,
    required String tableId,
    required Map<String, dynamic> data,
  }) async {
    try {
      final result = await _remoteDataSource.updateTable(
        restaurantId: restaurantId,
        tableId: tableId,
        data: data,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> deleteTable({
    required String restaurantId,
    required String tableId,
  }) async {
    try {
      await _remoteDataSource.deleteTable(
        restaurantId: restaurantId,
        tableId: tableId,
      );
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  // ─────────────────────── Dine-In Sessions ───────────────────────────

  Future<({List<OwnerSessionModel>? data, Failure? failure})>
      getDineInSessions({
    required String restaurantId,
  }) async {
    try {
      final result =
          await _remoteDataSource.getDineInSessions(restaurantId: restaurantId);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────────── Dine-In Orders ─────────────────────────────

  Future<({List<OwnerDineInOrderModel>? data, Failure? failure})>
      getDineInOrders({
    required String restaurantId,
    int? statusFilter,
  }) async {
    try {
      final result = await _remoteDataSource.getDineInOrders(
        restaurantId: restaurantId,
        statusFilter: statusFilter,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> updateDineInOrderStatus({
    required String restaurantId,
    required String orderId,
    required Map<String, dynamic> data,
  }) async {
    try {
      await _remoteDataSource.updateDineInOrderStatus(
        restaurantId: restaurantId,
        orderId: orderId,
        data: data,
      );
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  // ─────────────────────── Private Helpers ─────────────────────────────

  /// Maps a [DioException] to the appropriate [Failure] subclass.
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

    if (statusCode == 401) {
      return AuthFailure(message: message);
    }

    return ServerFailure(message: message, statusCode: statusCode);
  }
}
