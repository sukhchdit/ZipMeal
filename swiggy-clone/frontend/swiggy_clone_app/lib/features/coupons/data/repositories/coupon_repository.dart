import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/coupon_remote_data_source.dart';
import '../models/coupon_validation_model.dart';

part 'coupon_repository.g.dart';

@riverpod
CouponRepository couponRepository(Ref ref) {
  final remoteDataSource = ref.watch(couponRemoteDataSourceProvider);
  return CouponRepository(remoteDataSource: remoteDataSource);
}

class CouponRepository {
  CouponRepository({required CouponRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final CouponRemoteDataSource _remote;

  Future<({CouponValidationModel? data, Failure? failure})> validateCoupon({
    required String code,
    required int subtotal,
    required int orderType,
    required String restaurantId,
  }) async {
    try {
      final result = await _remote.validateCoupon(
        code: code,
        subtotal: subtotal,
        orderType: orderType,
        restaurantId: restaurantId,
      );
      return (data: result, failure: null);
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
