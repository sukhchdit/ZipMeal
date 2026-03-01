import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/activity_feed_item_model.dart';
import '../models/user_profile_model.dart';

part 'social_remote_data_source.g.dart';

@riverpod
SocialRemoteDataSource socialRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return SocialRemoteDataSource(dio: dio);
}

class SocialRemoteDataSource {
  SocialRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  Future<void> followUser({required String userId}) async {
    await _dio.post<void>(ApiConstants.socialFollow(userId));
  }

  Future<void> unfollowUser({required String userId}) async {
    await _dio.delete<void>(ApiConstants.socialFollow(userId));
  }

  Future<ActivityFeedResponse> getActivityFeed({
    String? cursor,
    int pageSize = 20,
  }) async {
    final queryParams = <String, dynamic>{'pageSize': pageSize};
    if (cursor != null) {
      queryParams['cursor'] = cursor;
    }
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.socialFeed,
      queryParameters: queryParams,
    );
    return ActivityFeedResponse.fromJson(response.data!);
  }

  Future<UserProfileModel> getUserProfile({required String userId}) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.socialProfileById(userId),
    );
    return UserProfileModel.fromJson(response.data!);
  }

  Future<List<FollowUserModel>> getFollowers({
    required String userId,
    int page = 1,
    int pageSize = 20,
  }) async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.socialFollowers(userId),
      queryParameters: {'page': page, 'pageSize': pageSize},
    );
    return response.data!
        .map((e) => FollowUserModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<List<FollowUserModel>> getFollowing({
    required String userId,
    int page = 1,
    int pageSize = 20,
  }) async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.socialFollowing(userId),
      queryParameters: {'page': page, 'pageSize': pageSize},
    );
    return response.data!
        .map((e) => FollowUserModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<FollowStatusModel> checkFollowStatus({
    required String userId,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.socialFollowStatus(userId),
    );
    return FollowStatusModel.fromJson(response.data!);
  }

  Future<Map<String, dynamic>> getShareLink({
    required String type,
    required String entityId,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.socialShare(type, entityId),
    );
    return response.data!;
  }
}
