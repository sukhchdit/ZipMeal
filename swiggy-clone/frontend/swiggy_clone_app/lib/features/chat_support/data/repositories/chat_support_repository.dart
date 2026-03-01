import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/chat_support_remote_data_source.dart';
import '../models/canned_response_model.dart';
import '../models/support_message_model.dart';
import '../models/support_ticket_model.dart';

part 'chat_support_repository.g.dart';

@riverpod
ChatSupportRepository chatSupportRepository(Ref ref) {
  return ChatSupportRepository(
    remoteDataSource: ref.watch(chatSupportRemoteDataSourceProvider),
  );
}

class ChatSupportRepository {
  final ChatSupportRemoteDataSource _remoteDataSource;

  ChatSupportRepository({
    required ChatSupportRemoteDataSource remoteDataSource,
  }) : _remoteDataSource = remoteDataSource;

  // ─────────────────── Tickets ─────────────────────────────────────

  Future<({Map<String, dynamic>? data, Failure? failure})> getMyTickets({
    String? cursor,
    int pageSize = 20,
  }) async {
    try {
      final data = await _remoteDataSource.getMyTickets(
        cursor: cursor,
        pageSize: pageSize,
      );
      return (data: data, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({SupportTicketModel? data, Failure? failure})> createTicket({
    required String subject,
    required int category,
    String? orderId,
    String? initialMessage,
  }) async {
    try {
      final ticket = await _remoteDataSource.createTicket(
        subject: subject,
        category: category,
        orderId: orderId,
        initialMessage: initialMessage,
      );
      return (data: ticket, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({int? data, Failure? failure})> getUnreadCount() async {
    try {
      final count = await _remoteDataSource.getUnreadCount();
      return (data: count, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────── Messages ────────────────────────────────────

  Future<({Map<String, dynamic>? data, Failure? failure})> getTicketMessages(
    String ticketId, {
    String? cursor,
    int pageSize = 30,
  }) async {
    try {
      final data = await _remoteDataSource.getTicketMessages(
        ticketId,
        cursor: cursor,
        pageSize: pageSize,
      );
      return (data: data, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({SupportMessageModel? data, Failure? failure})> sendMessage(
    String ticketId, {
    required String content,
    int messageType = 0,
  }) async {
    try {
      final message = await _remoteDataSource.sendMessage(
        ticketId,
        content: content,
        messageType: messageType,
      );
      return (data: message, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({bool? data, Failure? failure})> markMessagesRead(
      String ticketId) async {
    try {
      await _remoteDataSource.markMessagesRead(ticketId);
      return (data: true, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({bool? data, Failure? failure})> closeTicket(
      String ticketId) async {
    try {
      await _remoteDataSource.closeTicket(ticketId);
      return (data: true, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────── Canned Responses ────────────────────────────

  Future<({List<CannedResponseModel>? data, Failure? failure})>
      getCannedResponses({int? category}) async {
    try {
      final responses =
          await _remoteDataSource.getCannedResponses(category: category);
      return (data: responses, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────── Helpers ─────────────────────────────────────

  Failure _mapDioError(DioException e) {
    final statusCode = e.response?.statusCode;
    final data = e.response?.data;
    String message = 'Something went wrong. Please try again.';

    if (data is Map<String, dynamic>) {
      message = data['errorMessage'] as String? ?? message;
    }

    if (statusCode == 401) {
      return const AuthFailure();
    }

    if (e.type == DioExceptionType.connectionError ||
        e.type == DioExceptionType.connectionTimeout) {
      return const NetworkFailure();
    }

    return ServerFailure(message: message, statusCode: statusCode);
  }
}
