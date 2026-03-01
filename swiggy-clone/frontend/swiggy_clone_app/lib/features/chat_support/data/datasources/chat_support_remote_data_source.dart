import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/canned_response_model.dart';
import '../models/support_message_model.dart';
import '../models/support_ticket_model.dart';

part 'chat_support_remote_data_source.g.dart';

@riverpod
ChatSupportRemoteDataSource chatSupportRemoteDataSource(Ref ref) {
  return ChatSupportRemoteDataSource(dio: ref.watch(apiClientProvider));
}

class ChatSupportRemoteDataSource {
  final Dio _dio;

  ChatSupportRemoteDataSource({required Dio dio}) : _dio = dio;

  // ─────────────────── Tickets ─────────────────────────────────────

  Future<Map<String, dynamic>> getMyTickets({
    String? cursor,
    int pageSize = 20,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.supportTickets,
      queryParameters: {
        if (cursor != null) 'cursor': cursor,
        'pageSize': pageSize,
      },
    );
    return response.data!;
  }

  Future<SupportTicketModel> createTicket({
    required String subject,
    required int category,
    String? orderId,
    String? initialMessage,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.supportTickets,
      data: {
        'subject': subject,
        'category': category,
        if (orderId != null) 'orderId': orderId,
        if (initialMessage != null) 'initialMessage': initialMessage,
      },
    );
    return SupportTicketModel.fromJson(response.data!);
  }

  Future<int> getUnreadCount() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.supportUnreadCount,
    );
    return response.data!['unreadCount'] as int? ?? 0;
  }

  // ─────────────────── Messages ────────────────────────────────────

  Future<Map<String, dynamic>> getTicketMessages(
    String ticketId, {
    String? cursor,
    int pageSize = 30,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.supportTicketMessages(ticketId),
      queryParameters: {
        if (cursor != null) 'cursor': cursor,
        'pageSize': pageSize,
      },
    );
    return response.data!;
  }

  Future<SupportMessageModel> sendMessage(
    String ticketId, {
    required String content,
    int messageType = 0,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.supportTicketMessages(ticketId),
      data: {
        'content': content,
        'messageType': messageType,
      },
    );
    return SupportMessageModel.fromJson(response.data!);
  }

  Future<void> markMessagesRead(String ticketId) async {
    await _dio.put<void>(ApiConstants.supportTicketMessagesRead(ticketId));
  }

  // ─────────────────── Admin ───────────────────────────────────────

  Future<void> closeTicket(String ticketId) async {
    await _dio.put<void>(ApiConstants.supportTicketClose(ticketId));
  }

  // ─────────────────── Canned Responses ────────────────────────────

  Future<List<CannedResponseModel>> getCannedResponses({int? category}) async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.cannedResponses,
      queryParameters: {
        if (category != null) 'category': category,
      },
    );
    return response.data!
        .map((e) => CannedResponseModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }
}
