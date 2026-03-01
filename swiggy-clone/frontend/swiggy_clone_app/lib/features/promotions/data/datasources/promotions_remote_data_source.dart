import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/promotion_model.dart';

part 'promotions_remote_data_source.g.dart';

@riverpod
PromotionsRemoteDataSource promotionsRemoteDataSource(PromotionsRemoteDataSourceRef ref) {
  return PromotionsRemoteDataSource(ref.watch(apiClientProvider));
}

class PromotionsRemoteDataSource {
  final Dio _dio;

  PromotionsRemoteDataSource(this._dio);

  // ── Owner endpoints ──

  Future<Map<String, dynamic>> getPromotions({
    int? promotionType,
    bool? isActive,
    String? search,
    int page = 1,
    int pageSize = 20,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.promotions,
      queryParameters: {
        'page': page,
        'pageSize': pageSize,
        if (promotionType != null) 'promotionType': promotionType,
        if (isActive != null) 'isActive': isActive,
        if (search != null && search.isNotEmpty) 'search': search,
      },
    );
    return response.data!;
  }

  Future<PromotionModel> getPromotionById(String id) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.promotionById(id),
    );
    return PromotionModel.fromJson(response.data!);
  }

  Future<String> createPromotion(Map<String, dynamic> data) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.promotions,
      data: data,
    );
    return response.data!['id'] as String;
  }

  Future<void> updatePromotion(String id, Map<String, dynamic> data) async {
    await _dio.put<void>(
      ApiConstants.promotionById(id),
      data: data,
    );
  }

  Future<void> togglePromotion(String id, {required bool isActive}) async {
    await _dio.put<void>(
      ApiConstants.promotionToggle(id),
      data: {'isActive': isActive},
    );
  }

  Future<void> deletePromotion(String id) async {
    await _dio.delete<void>(ApiConstants.promotionById(id));
  }

  // ── Customer endpoint ──

  Future<List<PromotionModel>> getActivePromotions(String restaurantId) async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.restaurantPromotions(restaurantId),
    );
    return response.data!
        .map((e) => PromotionModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }
}
