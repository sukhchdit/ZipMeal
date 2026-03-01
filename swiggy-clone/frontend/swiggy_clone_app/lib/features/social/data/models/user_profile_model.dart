import 'package:freezed_annotation/freezed_annotation.dart';

import 'activity_feed_item_model.dart';

part 'user_profile_model.freezed.dart';
part 'user_profile_model.g.dart';

@freezed
class UserProfileModel with _$UserProfileModel {
  const factory UserProfileModel({
    required String userId,
    required String fullName,
    String? avatarUrl,
    required int followerCount,
    required int followingCount,
    required int reviewCount,
    required bool isFollowedByCurrentUser,
    @Default([]) List<ActivityFeedItemModel> recentActivity,
  }) = _UserProfileModel;

  factory UserProfileModel.fromJson(Map<String, dynamic> json) =>
      _$UserProfileModelFromJson(json);
}

@freezed
class FollowUserModel with _$FollowUserModel {
  const factory FollowUserModel({
    required String userId,
    required String fullName,
    String? avatarUrl,
    required String followedAt,
  }) = _FollowUserModel;

  factory FollowUserModel.fromJson(Map<String, dynamic> json) =>
      _$FollowUserModelFromJson(json);
}

@freezed
class FollowStatusModel with _$FollowStatusModel {
  const factory FollowStatusModel({
    required bool isFollowing,
  }) = _FollowStatusModel;

  factory FollowStatusModel.fromJson(Map<String, dynamic> json) =>
      _$FollowStatusModelFromJson(json);
}
