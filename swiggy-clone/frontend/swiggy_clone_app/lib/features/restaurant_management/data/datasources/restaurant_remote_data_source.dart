import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
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

part 'restaurant_remote_data_source.g.dart';

@riverpod
RestaurantRemoteDataSource restaurantRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return RestaurantRemoteDataSource(dio: dio);
}

/// Remote data source that handles all restaurant-management-related API calls.
///
/// Every method communicates with the backend restaurant endpoints and either
/// returns the parsed response model or throws a [DioException] on failure.
class RestaurantRemoteDataSource {
  RestaurantRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  // ─────────────────────── Restaurant CRUD ──────────────────────────────

  /// Registers a new restaurant with the given data.
  Future<RestaurantModel> registerRestaurant({
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.restaurants,
      data: data,
    );
    return RestaurantModel.fromJson(response.data!);
  }

  /// Fetches all restaurants owned by the authenticated user.
  Future<List<RestaurantSummaryModel>> getMyRestaurants() async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.restaurantsMy,
    );
    return response.data!
        .map((e) => RestaurantSummaryModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  /// Fetches a single restaurant by its [id].
  Future<RestaurantModel> getRestaurantById({required String id}) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.restaurantById(id),
    );
    return RestaurantModel.fromJson(response.data!);
  }

  /// Updates a restaurant identified by [id] with the given [data].
  Future<RestaurantModel> updateRestaurant({
    required String id,
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.restaurantById(id),
      data: data,
    );
    return RestaurantModel.fromJson(response.data!);
  }

  /// Toggles whether the restaurant is accepting orders.
  Future<bool> toggleAcceptingOrders({
    required String id,
    required bool value,
  }) async {
    final response = await _dio.put<dynamic>(
      ApiConstants.restaurantAcceptingOrders(id),
      data: {'value': value},
    );
    return response.data as bool;
  }

  /// Toggles whether dine-in is enabled for the restaurant.
  Future<bool> toggleDineIn({
    required String id,
    required bool value,
  }) async {
    final response = await _dio.put<dynamic>(
      ApiConstants.restaurantDineIn(id),
      data: {'value': value},
    );
    return response.data as bool;
  }

  /// Fetches the dashboard summary for a restaurant.
  Future<RestaurantDashboardModel> getDashboard({required String id}) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.restaurantDashboard(id),
    );
    return RestaurantDashboardModel.fromJson(response.data!);
  }

  /// Uploads a file (logo, banner, etc.) for the restaurant.
  Future<FileUploadResultModel> uploadFile({
    required String id,
    required String fileType,
    required String filePath,
    required String fileName,
  }) async {
    final formData = FormData.fromMap({
      'file': await MultipartFile.fromFile(filePath, filename: fileName),
    });
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.restaurantUpload(id, fileType),
      data: formData,
    );
    return FileUploadResultModel.fromJson(response.data!);
  }

  // ─────────────────────── Menu Categories ─────────────────────────────

  /// Fetches all menu categories for a restaurant.
  Future<List<MenuCategoryModel>> getMenuCategories({
    required String restaurantId,
  }) async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.restaurantMenuCategories(restaurantId),
    );
    return response.data!
        .map((e) => MenuCategoryModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  /// Creates a new menu category for a restaurant.
  Future<MenuCategoryModel> createMenuCategory({
    required String restaurantId,
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.restaurantMenuCategories(restaurantId),
      data: data,
    );
    return MenuCategoryModel.fromJson(response.data!);
  }

  /// Updates an existing menu category.
  Future<MenuCategoryModel> updateMenuCategory({
    required String restaurantId,
    required String categoryId,
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.restaurantMenuCategory(restaurantId, categoryId),
      data: data,
    );
    return MenuCategoryModel.fromJson(response.data!);
  }

  /// Deletes a menu category.
  Future<void> deleteMenuCategory({
    required String restaurantId,
    required String categoryId,
  }) async {
    await _dio.delete<dynamic>(
      ApiConstants.restaurantMenuCategory(restaurantId, categoryId),
    );
  }

  // ─────────────────────── Menu Items ──────────────────────────────────

  /// Fetches all menu items for a given category.
  Future<List<MenuItemModel>> getMenuItems({
    required String restaurantId,
    required String categoryId,
  }) async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.restaurantMenuCategoryItems(restaurantId, categoryId),
    );
    return response.data!
        .map((e) => MenuItemModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  /// Fetches a single menu item by its [itemId].
  Future<MenuItemModel> getMenuItemById({
    required String restaurantId,
    required String itemId,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.restaurantMenuItem(restaurantId, itemId),
    );
    return MenuItemModel.fromJson(response.data!);
  }

  /// Creates a new menu item for a restaurant.
  Future<MenuItemModel> createMenuItem({
    required String restaurantId,
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.restaurantMenuItems(restaurantId),
      data: data,
    );
    return MenuItemModel.fromJson(response.data!);
  }

  /// Updates an existing menu item.
  Future<MenuItemModel> updateMenuItem({
    required String restaurantId,
    required String itemId,
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.restaurantMenuItem(restaurantId, itemId),
      data: data,
    );
    return MenuItemModel.fromJson(response.data!);
  }

  /// Deletes a menu item.
  Future<void> deleteMenuItem({
    required String restaurantId,
    required String itemId,
  }) async {
    await _dio.delete<dynamic>(
      ApiConstants.restaurantMenuItem(restaurantId, itemId),
    );
  }

  // ─────────────────────── Variants ────────────────────────────────────

  /// Adds a variant to a menu item.
  Future<MenuItemVariantModel> addVariant({
    required String restaurantId,
    required String itemId,
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.restaurantMenuItemVariants(restaurantId, itemId),
      data: data,
    );
    return MenuItemVariantModel.fromJson(response.data!);
  }

  /// Updates an existing variant.
  Future<MenuItemVariantModel> updateVariant({
    required String restaurantId,
    required String itemId,
    required String variantId,
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.restaurantMenuItemVariant(restaurantId, itemId, variantId),
      data: data,
    );
    return MenuItemVariantModel.fromJson(response.data!);
  }

  /// Deletes a variant from a menu item.
  Future<void> deleteVariant({
    required String restaurantId,
    required String itemId,
    required String variantId,
  }) async {
    await _dio.delete<dynamic>(
      ApiConstants.restaurantMenuItemVariant(restaurantId, itemId, variantId),
    );
  }

  // ─────────────────────── Addons ──────────────────────────────────────

  /// Adds an addon to a menu item.
  Future<MenuItemAddonModel> addAddon({
    required String restaurantId,
    required String itemId,
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.restaurantMenuItemAddons(restaurantId, itemId),
      data: data,
    );
    return MenuItemAddonModel.fromJson(response.data!);
  }

  /// Updates an existing addon.
  Future<MenuItemAddonModel> updateAddon({
    required String restaurantId,
    required String itemId,
    required String addonId,
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.restaurantMenuItemAddon(restaurantId, itemId, addonId),
      data: data,
    );
    return MenuItemAddonModel.fromJson(response.data!);
  }

  /// Deletes an addon from a menu item.
  Future<void> deleteAddon({
    required String restaurantId,
    required String itemId,
    required String addonId,
  }) async {
    await _dio.delete<dynamic>(
      ApiConstants.restaurantMenuItemAddon(restaurantId, itemId, addonId),
    );
  }

  // ─────────────────────── Operating Hours ─────────────────────────────

  /// Fetches all operating hours for a restaurant.
  Future<List<OperatingHoursModel>> getOperatingHours({
    required String restaurantId,
  }) async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.restaurantOperatingHours(restaurantId),
    );
    return response.data!
        .map((e) => OperatingHoursModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  /// Creates or updates operating hours for a restaurant.
  Future<List<OperatingHoursModel>> upsertOperatingHours({
    required String restaurantId,
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.put<List<dynamic>>(
      ApiConstants.restaurantOperatingHours(restaurantId),
      data: data,
    );
    return response.data!
        .map((e) => OperatingHoursModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  // ─────────────────────── Table Management ───────────────────────────

  /// Fetches all tables for a restaurant.
  Future<List<RestaurantTableModel>> getTables({
    required String restaurantId,
  }) async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.restaurantTables(restaurantId),
    );
    return response.data!
        .map((e) => RestaurantTableModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  /// Creates a new table.
  Future<RestaurantTableModel> createTable({
    required String restaurantId,
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.restaurantTables(restaurantId),
      data: data,
    );
    return RestaurantTableModel.fromJson(response.data!);
  }

  /// Updates an existing table.
  Future<RestaurantTableModel> updateTable({
    required String restaurantId,
    required String tableId,
    required Map<String, dynamic> data,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.restaurantTable(restaurantId, tableId),
      data: data,
    );
    return RestaurantTableModel.fromJson(response.data!);
  }

  /// Soft-deletes a table.
  Future<void> deleteTable({
    required String restaurantId,
    required String tableId,
  }) async {
    await _dio.delete<dynamic>(
      ApiConstants.restaurantTable(restaurantId, tableId),
    );
  }

  // ─────────────────────── Dine-In Sessions ──────────────────────────

  /// Fetches active dine-in sessions for a restaurant.
  Future<List<OwnerSessionModel>> getDineInSessions({
    required String restaurantId,
  }) async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.restaurantDineInSessions(restaurantId),
    );
    return response.data!
        .map((e) => OwnerSessionModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  // ─────────────────────── Dine-In Orders ────────────────────────────

  /// Fetches dine-in orders for a restaurant with optional status filter.
  Future<List<OwnerDineInOrderModel>> getDineInOrders({
    required String restaurantId,
    int? statusFilter,
  }) async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.restaurantDineInOrders(restaurantId),
      queryParameters: {
        if (statusFilter != null) 'status': statusFilter,
      },
    );
    return response.data!
        .map(
            (e) => OwnerDineInOrderModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  /// Updates a dine-in order's status.
  Future<void> updateDineInOrderStatus({
    required String restaurantId,
    required String orderId,
    required Map<String, dynamic> data,
  }) async {
    await _dio.put<dynamic>(
      ApiConstants.restaurantDineInOrderStatus(restaurantId, orderId),
      data: data,
    );
  }

  // ─────────────────────── Cuisines ────────────────────────────────────

  /// Fetches all available cuisine types.
  Future<List<CuisineTypeModel>> getCuisineTypes() async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.cuisines,
    );
    return response.data!
        .map((e) => CuisineTypeModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }
}
