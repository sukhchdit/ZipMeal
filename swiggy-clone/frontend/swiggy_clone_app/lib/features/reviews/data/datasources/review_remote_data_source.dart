import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/review_model.dart';

part 'review_remote_data_source.g.dart';

@riverpod
ReviewRemoteDataSource reviewRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return ReviewRemoteDataSource(dio: dio);
}

class ReviewRemoteDataSource {
  ReviewRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  Future<ReviewModel> submitReview({
    required String orderId,
    required int rating,
    String? reviewText,
    int? deliveryRating,
    required bool isAnonymous,
    List<String> photoUrls = const [],
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.reviews,
      data: {
        'orderId': orderId,
        'rating': rating,
        if (reviewText != null) 'reviewText': reviewText,
        if (deliveryRating != null) 'deliveryRating': deliveryRating,
        'isAnonymous': isAnonymous,
        'photoUrls': photoUrls,
      },
    );
    return ReviewModel.fromJson(response.data!);
  }

  Future<Map<String, dynamic>> getRestaurantReviews({
    required String restaurantId,
    int page = 1,
    int pageSize = 20,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.restaurantReviews(restaurantId),
      queryParameters: {'page': page, 'pageSize': pageSize},
    );
    return response.data!;
  }

  Future<Map<String, dynamic>> getMyReviews({
    int page = 1,
    int pageSize = 20,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.myReviews,
      queryParameters: {'page': page, 'pageSize': pageSize},
    );
    return response.data!;
  }

  Future<void> replyToReview({
    required String reviewId,
    required String replyText,
  }) async {
    await _dio.put<void>(
      '${ApiConstants.reviews}/$reviewId/reply',
      data: {'replyText': replyText},
    );
  }
}
