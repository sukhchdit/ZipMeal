import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../../../orders/data/models/order_model.dart';
import '../models/dine_in_session_model.dart';

part 'dine_in_remote_data_source.g.dart';

@riverpod
DineInRemoteDataSource dineInRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return DineInRemoteDataSource(dio: dio);
}

class DineInRemoteDataSource {
  DineInRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  /// POST /api/v1/dine-in/sessions — Start session by scanning QR
  Future<DineInSessionModel> startSession({
    required String qrCodeData,
    int guestCount = 1,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.dineInSessions,
      data: {
        'qrCodeData': qrCodeData,
        'guestCount': guestCount,
      },
    );
    return DineInSessionModel.fromJson(response.data!);
  }

  /// POST /api/v1/dine-in/sessions/join — Join session by code
  Future<DineInSessionModel> joinSession({
    required String sessionCode,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      '${ApiConstants.dineInSessions}/join',
      data: {'sessionCode': sessionCode},
    );
    return DineInSessionModel.fromJson(response.data!);
  }

  /// GET /api/v1/dine-in/sessions/active — Get user's active session
  Future<DineInSessionSummaryModel?> getActiveSession() async {
    final response = await _dio.get(
      '${ApiConstants.dineInSessions}/active',
    );
    if (response.data == null) return null;
    return DineInSessionSummaryModel.fromJson(
        response.data as Map<String, dynamic>);
  }

  /// GET /api/v1/dine-in/sessions/{id} — Session detail
  Future<DineInSessionModel> getSessionDetail({
    required String sessionId,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      '${ApiConstants.dineInSessions}/$sessionId',
    );
    return DineInSessionModel.fromJson(response.data!);
  }

  /// GET /api/v1/dine-in/sessions/{id}/orders — Session orders
  Future<List<OrderModel>> getSessionOrders({
    required String sessionId,
  }) async {
    final response = await _dio.get<List<dynamic>>(
      '${ApiConstants.dineInSessions}/$sessionId/orders',
    );
    return response.data!
        .map((e) => OrderModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  /// POST /api/v1/dine-in/sessions/{id}/orders — Place dine-in order
  Future<OrderModel> placeDineInOrder({
    required String sessionId,
    required List<Map<String, dynamic>> items,
    String? specialInstructions,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      '${ApiConstants.dineInSessions}/$sessionId/orders',
      data: {
        'items': items,
        if (specialInstructions != null)
          'specialInstructions': specialInstructions,
      },
    );
    return OrderModel.fromJson(response.data!);
  }

  /// PUT /api/v1/dine-in/sessions/{id}/request-bill — Request bill (host only)
  Future<void> requestBill({required String sessionId}) async {
    await _dio.put('${ApiConstants.dineInSessions}/$sessionId/request-bill');
  }

  /// PUT /api/v1/dine-in/sessions/{id}/leave — Leave session (guest)
  Future<void> leaveSession({required String sessionId}) async {
    await _dio.put('${ApiConstants.dineInSessions}/$sessionId/leave');
  }

  /// PUT /api/v1/dine-in/sessions/{id}/end — End session (host)
  Future<void> endSession({required String sessionId}) async {
    await _dio.put('${ApiConstants.dineInSessions}/$sessionId/end');
  }
}
