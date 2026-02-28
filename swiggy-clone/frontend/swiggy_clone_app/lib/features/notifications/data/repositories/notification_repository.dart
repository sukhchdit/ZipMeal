import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/notification_remote_data_source.dart';
import '../models/notification_model.dart';

part 'notification_repository.g.dart';

@riverpod
NotificationRepository notificationRepository(Ref ref) {
  final remoteDataSource = ref.watch(notificationRemoteDataSourceProvider);
  return NotificationRepository(remoteDataSource: remoteDataSource);
}

class NotificationRepository {
  NotificationRepository(
      {required NotificationRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final NotificationRemoteDataSource _remote;

  Future<
      ({
        List<NotificationModel>? items,
        String? nextCursor,
        bool? hasMore,
        Failure? failure,
      })> getMyNotifications({String? cursor, int pageSize = 20}) async {
    try {
      final data =
          await _remote.getMyNotifications(cursor: cursor, pageSize: pageSize);
      final items = (data['items'] as List<dynamic>)
          .map((e) => NotificationModel.fromJson(e as Map<String, dynamic>))
          .toList();
      return (
        items: items,
        nextCursor: data['nextCursor'] as String?,
        hasMore: data['hasMore'] as bool?,
        failure: null,
      );
    } on DioException catch (e) {
      return (
        items: null,
        nextCursor: null,
        hasMore: null,
        failure: _mapDioError(e),
      );
    }
  }

  Future<({int? data, Failure? failure})> getUnreadCount() async {
    try {
      final count = await _remote.getUnreadCount();
      return (data: count, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> markAsRead({required String id}) async {
    try {
      await _remote.markAsRead(id: id);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<Failure?> markAllAsRead() async {
    try {
      await _remote.markAllAsRead();
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<Failure?> registerDevice({
    required String deviceToken,
    required int platform,
  }) async {
    try {
      await _remote.registerDevice(
          deviceToken: deviceToken, platform: platform);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<Failure?> unregisterDevice({required String deviceToken}) async {
    try {
      await _remote.unregisterDevice(deviceToken: deviceToken);
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
