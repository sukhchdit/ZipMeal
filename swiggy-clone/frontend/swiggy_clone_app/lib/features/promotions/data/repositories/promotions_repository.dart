import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/promotions_remote_data_source.dart';
import '../models/promotion_model.dart';

part 'promotions_repository.g.dart';

@riverpod
PromotionsRepository promotionsRepository(PromotionsRepositoryRef ref) {
  return PromotionsRepository(ref.watch(promotionsRemoteDataSourceProvider));
}

class PromotionsRepository {
  final PromotionsRemoteDataSource _dataSource;

  PromotionsRepository(this._dataSource);

  Future<({PromotionListResponse? data, Failure? failure})> getPromotions({
    int? promotionType,
    bool? isActive,
    String? search,
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final json = await _dataSource.getPromotions(
        promotionType: promotionType,
        isActive: isActive,
        search: search,
        page: page,
        pageSize: pageSize,
      );
      final data = PromotionListResponse.fromJson(json);
      return (data: data, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({PromotionModel? data, Failure? failure})> getPromotionById(
      String id) async {
    try {
      final data = await _dataSource.getPromotionById(id);
      return (data: data, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({String? data, Failure? failure})> createPromotion(
      Map<String, dynamic> data) async {
    try {
      final id = await _dataSource.createPromotion(data);
      return (data: id, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<Failure?> updatePromotion(
      String id, Map<String, dynamic> data) async {
    try {
      await _dataSource.updatePromotion(id, data);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<Failure?> togglePromotion(String id, {required bool isActive}) async {
    try {
      await _dataSource.togglePromotion(id, isActive: isActive);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<Failure?> deletePromotion(String id) async {
    try {
      await _dataSource.deletePromotion(id);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<({List<PromotionModel>? data, Failure? failure})> getActivePromotions(
      String restaurantId) async {
    try {
      final data = await _dataSource.getActivePromotions(restaurantId);
      return (data: data, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Failure _mapDioError(DioException e) {
    if (e.type == DioExceptionType.connectionError ||
        e.type == DioExceptionType.connectionTimeout) {
      return const NetworkFailure();
    }
    if (e.response?.statusCode == 401) {
      return const AuthFailure();
    }
    final message =
        (e.response?.data as Map<String, dynamic>?)?['errorMessage']
                as String? ??
            'Something went wrong';
    return ServerFailure(message: message);
  }
}
