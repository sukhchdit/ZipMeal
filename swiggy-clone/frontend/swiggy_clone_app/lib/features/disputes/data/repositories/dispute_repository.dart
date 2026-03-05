import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/dispute_remote_data_source.dart';
import '../models/dispute_message_model.dart';
import '../models/dispute_model.dart';

part 'dispute_repository.g.dart';

@riverpod
DisputeRepository disputeRepository(Ref ref) {
  final remoteDataSource = ref.watch(disputeRemoteDataSourceProvider);
  return DisputeRepository(remoteDataSource: remoteDataSource);
}

class DisputeRepository {
  DisputeRepository({required DisputeRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final DisputeRemoteDataSource _remote;

  // ─────────────────── Customer ─────────────────────────────────

  Future<({DisputeModel? data, Failure? failure})> createDispute({
    required String orderId,
    required int issueType,
    required String description,
  }) async {
    try {
      final result = await _remote.createDispute(
        orderId: orderId,
        issueType: issueType,
        description: description,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<
      ({
        List<DisputeSummaryModel>? data,
        String? nextCursor,
        bool hasMore,
        Failure? failure,
      })> getMyDisputes({String? cursor, int pageSize = 20}) async {
    try {
      final result =
          await _remote.getMyDisputes(cursor: cursor, pageSize: pageSize);
      return (
        data: result.items,
        nextCursor: result.nextCursor,
        hasMore: result.hasMore,
        failure: null,
      );
    } on DioException catch (e) {
      return (
        data: null,
        nextCursor: null,
        hasMore: false,
        failure: _mapDioError(e),
      );
    }
  }

  Future<({DisputeModel? data, Failure? failure})> getDisputeDetail(
    String id,
  ) async {
    try {
      final result = await _remote.getDisputeDetail(id);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<
      ({
        List<DisputeMessageModel>? data,
        String? nextCursor,
        bool hasMore,
        Failure? failure,
      })> getDisputeMessages(
    String disputeId, {
    String? cursor,
    int pageSize = 30,
  }) async {
    try {
      final result = await _remote.getDisputeMessages(
        disputeId,
        cursor: cursor,
        pageSize: pageSize,
      );
      return (
        data: result.items,
        nextCursor: result.nextCursor,
        hasMore: result.hasMore,
        failure: null,
      );
    } on DioException catch (e) {
      return (
        data: null,
        nextCursor: null,
        hasMore: false,
        failure: _mapDioError(e),
      );
    }
  }

  Future<({DisputeMessageModel? data, Failure? failure})> addMessage({
    required String disputeId,
    required String content,
  }) async {
    try {
      final result = await _remote.addMessage(
        disputeId: disputeId,
        content: content,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  // ─────────────────── Helpers ──────────────────────────────────

  Failure _mapDioError(DioException e) {
    if (e.type == DioExceptionType.connectionError ||
        e.type == DioExceptionType.connectionTimeout) {
      return const NetworkFailure();
    }
    final statusCode = e.response?.statusCode;
    final data = e.response?.data;
    var message = 'An unexpected error occurred.';
    if (data is Map<String, dynamic>) {
      message = (data['errorMessage'] as String?) ?? message;
    }
    if (statusCode == 401) return AuthFailure(message: message);
    return ServerFailure(message: message, statusCode: statusCode);
  }
}
