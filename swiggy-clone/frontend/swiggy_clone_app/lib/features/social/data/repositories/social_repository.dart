import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/social_remote_data_source.dart';
import '../models/activity_feed_item_model.dart';
import '../models/user_profile_model.dart';

part 'social_repository.g.dart';

@riverpod
SocialRepository socialRepository(Ref ref) {
  final remoteDataSource = ref.watch(socialRemoteDataSourceProvider);
  return SocialRepository(remoteDataSource: remoteDataSource);
}

class SocialRepository {
  SocialRepository({
    required SocialRemoteDataSource remoteDataSource,
  }) : _remote = remoteDataSource;

  final SocialRemoteDataSource _remote;

  Future<Failure?> followUser({required String userId}) async {
    try {
      await _remote.followUser(userId: userId);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<Failure?> unfollowUser({required String userId}) async {
    try {
      await _remote.unfollowUser(userId: userId);
      return null;
    } on DioException catch (e) {
      return _mapDioError(e);
    }
  }

  Future<({ActivityFeedResponse? data, Failure? failure})>
      getActivityFeed({String? cursor, int pageSize = 20}) async {
    try {
      final result =
          await _remote.getActivityFeed(cursor: cursor, pageSize: pageSize);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({UserProfileModel? data, Failure? failure})> getUserProfile({
    required String userId,
  }) async {
    try {
      final result = await _remote.getUserProfile(userId: userId);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({List<FollowUserModel>? data, Failure? failure})> getFollowers({
    required String userId,
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final result = await _remote.getFollowers(
        userId: userId,
        page: page,
        pageSize: pageSize,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({List<FollowUserModel>? data, Failure? failure})> getFollowing({
    required String userId,
    int page = 1,
    int pageSize = 20,
  }) async {
    try {
      final result = await _remote.getFollowing(
        userId: userId,
        page: page,
        pageSize: pageSize,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({FollowStatusModel? data, Failure? failure})> checkFollowStatus({
    required String userId,
  }) async {
    try {
      final result = await _remote.checkFollowStatus(userId: userId);
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
    var message = 'An unexpected error occurred.';
    if (data is Map<String, dynamic>) {
      message = (data['errorMessage'] as String?) ?? message;
    }
    if (statusCode == 401) return AuthFailure(message: message);
    return ServerFailure(message: message, statusCode: statusCode);
  }
}
