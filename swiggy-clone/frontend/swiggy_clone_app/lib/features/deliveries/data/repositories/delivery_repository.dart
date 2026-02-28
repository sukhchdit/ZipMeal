import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/delivery_remote_data_source.dart';
import '../models/delivery_assignment_model.dart';
import '../models/delivery_tracking_model.dart';
import '../models/partner_dashboard_model.dart';

part 'delivery_repository.g.dart';

@riverpod
DeliveryRepository deliveryRepository(Ref ref) {
  final remoteDataSource = ref.watch(deliveryRemoteDataSourceProvider);
  return DeliveryRepository(remoteDataSource: remoteDataSource);
}

class DeliveryRepository {
  DeliveryRepository(
      {required DeliveryRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final DeliveryRemoteDataSource _remote;

  Future<Failure?> toggleOnlineStatus({
    required bool isOnline,
    double? latitude,
    double? longitude,
  }) async {
    try {
      await _remote.toggleOnlineStatus(
        isOnline: isOnline,
        latitude: latitude,
        longitude: longitude,
      );
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<
      ({
        List<DeliveryAssignmentModel>? items,
        String? nextCursor,
        bool? hasMore,
        Failure? failure,
      })> getMyDeliveries({String? cursor, int pageSize = 20}) async {
    try {
      final data =
          await _remote.getMyDeliveries(cursor: cursor, pageSize: pageSize);
      final items = (data['items'] as List<dynamic>)
          .map((e) =>
              DeliveryAssignmentModel.fromJson(e as Map<String, dynamic>))
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

  Future<({DeliveryAssignmentModel? data, Failure? failure})>
      getActiveDelivery() async {
    try {
      final raw = await _remote.getActiveDelivery();
      if (raw == null) return (data: null, failure: null);
      return (
        data: DeliveryAssignmentModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({PartnerDashboardModel? data, Failure? failure})>
      getDashboard() async {
    try {
      final raw = await _remote.getDashboard();
      return (
        data: PartnerDashboardModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> acceptDelivery({required String assignmentId}) async {
    try {
      await _remote.acceptDelivery(assignmentId: assignmentId);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<Failure?> updateDeliveryStatus({
    required String assignmentId,
    required int newStatus,
  }) async {
    try {
      await _remote.updateDeliveryStatus(
        assignmentId: assignmentId,
        newStatus: newStatus,
      );
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<Failure?> updateLocation({
    required double latitude,
    required double longitude,
    double? heading,
    double? speed,
  }) async {
    try {
      await _remote.updateLocation(
        latitude: latitude,
        longitude: longitude,
        heading: heading,
        speed: speed,
      );
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<({DeliveryTrackingModel? data, Failure? failure})>
      getDeliveryTracking({required String orderId}) async {
    try {
      final raw = await _remote.getDeliveryTracking(orderId: orderId);
      return (
        data: DeliveryTrackingModel.fromJson(raw),
        failure: null,
      );
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
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
