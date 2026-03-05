import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/review_remote_data_source.dart';
import '../models/review_analytics_model.dart';
import '../models/review_model.dart';

part 'review_repository.g.dart';

@riverpod
ReviewRepository reviewRepository(Ref ref) {
  final remoteDataSource = ref.watch(reviewRemoteDataSourceProvider);
  return ReviewRepository(remoteDataSource: remoteDataSource);
}

class ReviewRepository {
  ReviewRepository({required ReviewRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final ReviewRemoteDataSource _remote;

  Future<({ReviewModel? data, Failure? failure})> submitReview({
    required String orderId,
    required int rating,
    String? reviewText,
    int? deliveryRating,
    required bool isAnonymous,
    List<String> photoUrls = const [],
  }) async {
    try {
      final result = await _remote.submitReview(
        orderId: orderId,
        rating: rating,
        reviewText: reviewText,
        deliveryRating: deliveryRating,
        isAnonymous: isAnonymous,
        photoUrls: photoUrls,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<
      ({
        List<ReviewModel>? items,
        int? totalCount,
        Failure? failure,
      })> getRestaurantReviews({
    required String restaurantId,
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final data = await _remote.getRestaurantReviews(
        restaurantId: restaurantId,
        page: page,
        pageSize: pageSize,
      );
      final items = (data['items'] as List<dynamic>)
          .map((e) => ReviewModel.fromJson(e as Map<String, dynamic>))
          .toList();
      return (
        items: items,
        totalCount: data['totalCount'] as int?,
        failure: null,
      );
    } on DioException catch (e) {
      return (items: null, totalCount: null, failure: _mapDioError(e));
    }
  }

  Future<
      ({
        List<ReviewModel>? items,
        int? totalCount,
        Failure? failure,
      })> getMyReviews({int page = 1, int pageSize = 20}) async {
    try {
      final data = await _remote.getMyReviews(page: page, pageSize: pageSize);
      final items = (data['items'] as List<dynamic>)
          .map((e) => ReviewModel.fromJson(e as Map<String, dynamic>))
          .toList();
      return (
        items: items,
        totalCount: data['totalCount'] as int?,
        failure: null,
      );
    } on DioException catch (e) {
      return (items: null, totalCount: null, failure: _mapDioError(e));
    }
  }

  Future<({String? data, Failure? failure})> uploadReviewPhoto({
    required String filePath,
    required String fileName,
  }) async {
    try {
      final url = await _remote.uploadReviewPhoto(
        filePath: filePath,
        fileName: fileName,
      );
      return (data: url, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({bool success, Failure? failure})> voteReview({
    required String reviewId,
    required bool isHelpful,
  }) async {
    try {
      await _remote.voteReview(reviewId: reviewId, isHelpful: isHelpful);
      return (success: true, failure: null);
    } on DioException catch (e) {
      return (success: false, failure: _mapDioError(e));
    }
  }

  Future<({bool success, Failure? failure})> removeVote({
    required String reviewId,
  }) async {
    try {
      await _remote.removeVote(reviewId: reviewId);
      return (success: true, failure: null);
    } on DioException catch (e) {
      return (success: false, failure: _mapDioError(e));
    }
  }

  Future<({bool success, Failure? failure})> reportReview({
    required String reviewId,
    required String reason,
    String? description,
  }) async {
    try {
      await _remote.reportReview(
        reviewId: reviewId,
        reason: reason,
        description: description,
      );
      return (success: true, failure: null);
    } on DioException catch (e) {
      return (success: false, failure: _mapDioError(e));
    }
  }

  Future<({bool success, Failure? failure})> deleteReply({
    required String reviewId,
  }) async {
    try {
      await _remote.deleteReply(reviewId: reviewId);
      return (success: true, failure: null);
    } on DioException catch (e) {
      return (success: false, failure: _mapDioError(e));
    }
  }

  Future<({ReviewAnalyticsModel? data, Failure? failure})> getReviewAnalytics({
    required String restaurantId,
  }) async {
    try {
      final data =
          await _remote.getReviewAnalytics(restaurantId: restaurantId);
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
