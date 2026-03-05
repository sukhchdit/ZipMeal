import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/dispute_message_model.dart';
import '../models/dispute_model.dart';

part 'dispute_remote_data_source.g.dart';

@riverpod
DisputeRemoteDataSource disputeRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return DisputeRemoteDataSource(dio: dio);
}

class DisputeRemoteDataSource {
  DisputeRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  // ─────────────────── Customer ─────────────────────────────────

  /// POST /api/v1/disputes
  Future<DisputeModel> createDispute({
    required String orderId,
    required int issueType,
    required String description,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.disputes,
      data: {
        'orderId': orderId,
        'issueType': issueType,
        'description': description,
      },
    );
    return DisputeModel.fromJson(response.data!);
  }

  /// GET /api/v1/disputes
  Future<({List<DisputeSummaryModel> items, String? nextCursor, bool hasMore})>
      getMyDisputes({String? cursor, int pageSize = 20}) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.disputes,
      queryParameters: {
        if (cursor != null) 'cursor': cursor,
        'pageSize': pageSize,
      },
    );
    final data = response.data!;
    final items = (data['items'] as List<dynamic>)
        .map((e) => DisputeSummaryModel.fromJson(e as Map<String, dynamic>))
        .toList();
    return (
      items: items,
      nextCursor: data['nextCursor'] as String?,
      hasMore: data['hasMore'] as bool? ?? false,
    );
  }

  /// GET /api/v1/disputes/{id}
  Future<DisputeModel> getDisputeDetail(String id) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.disputeById(id),
    );
    return DisputeModel.fromJson(response.data!);
  }

  /// GET /api/v1/disputes/{id}/messages
  Future<({List<DisputeMessageModel> items, String? nextCursor, bool hasMore})>
      getDisputeMessages(
    String disputeId, {
    String? cursor,
    int pageSize = 30,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.disputeMessages(disputeId),
      queryParameters: {
        if (cursor != null) 'cursor': cursor,
        'pageSize': pageSize,
      },
    );
    final data = response.data!;
    final items = (data['items'] as List<dynamic>)
        .map(
          (e) => DisputeMessageModel.fromJson(e as Map<String, dynamic>),
        )
        .toList();
    return (
      items: items,
      nextCursor: data['nextCursor'] as String?,
      hasMore: data['hasMore'] as bool? ?? false,
    );
  }

  /// POST /api/v1/disputes/{id}/messages
  Future<DisputeMessageModel> addMessage({
    required String disputeId,
    required String content,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.disputeMessages(disputeId),
      data: {'content': content},
    );
    return DisputeMessageModel.fromJson(response.data!);
  }
}
