import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/coupon_validation_model.dart';

part 'coupon_remote_data_source.g.dart';

@riverpod
CouponRemoteDataSource couponRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return CouponRemoteDataSource(dio: dio);
}

class CouponRemoteDataSource {
  CouponRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  Future<CouponValidationModel> validateCoupon({
    required String code,
    required int subtotal,
    required int orderType,
    required String restaurantId,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.couponValidate,
      queryParameters: {
        'code': code,
        'subtotal': subtotal,
        'orderType': orderType,
        'restaurantId': restaurantId,
      },
    );
    return CouponValidationModel.fromJson(response.data!);
  }
}
