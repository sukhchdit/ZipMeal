import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../../orders/data/models/order_model.dart';
import '../datasources/dine_in_remote_data_source.dart';
import '../models/dine_in_session_model.dart';

part 'dine_in_repository.g.dart';

@riverpod
DineInRepository dineInRepository(Ref ref) {
  final remoteDataSource = ref.watch(dineInRemoteDataSourceProvider);
  return DineInRepository(remoteDataSource: remoteDataSource);
}

class DineInRepository {
  DineInRepository({required DineInRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final DineInRemoteDataSource _remote;

  Future<({DineInSessionModel? data, Failure? failure})> startSession({
    required String qrCodeData,
    int guestCount = 1,
  }) async {
    try {
      final result = await _remote.startSession(
        qrCodeData: qrCodeData,
        guestCount: guestCount,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({DineInSessionModel? data, Failure? failure})> joinSession({
    required String sessionCode,
  }) async {
    try {
      final result = await _remote.joinSession(sessionCode: sessionCode);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({DineInSessionSummaryModel? data, Failure? failure})>
      getActiveSession() async {
    try {
      final result = await _remote.getActiveSession();
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({DineInSessionModel? data, Failure? failure})> getSessionDetail({
    required String sessionId,
  }) async {
    try {
      final result = await _remote.getSessionDetail(sessionId: sessionId);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({List<OrderModel>? data, Failure? failure})> getSessionOrders({
    required String sessionId,
  }) async {
    try {
      final result = await _remote.getSessionOrders(sessionId: sessionId);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({OrderModel? data, Failure? failure})> placeDineInOrder({
    required String sessionId,
    required List<Map<String, dynamic>> items,
    String? specialInstructions,
  }) async {
    try {
      final result = await _remote.placeDineInOrder(
        sessionId: sessionId,
        items: items,
        specialInstructions: specialInstructions,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> requestBill({required String sessionId}) async {
    try {
      await _remote.requestBill(sessionId: sessionId);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<Failure?> leaveSession({required String sessionId}) async {
    try {
      await _remote.leaveSession(sessionId: sessionId);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<Failure?> endSession({required String sessionId}) async {
    try {
      await _remote.endSession(sessionId: sessionId);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

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
