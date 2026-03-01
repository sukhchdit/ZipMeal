import 'package:freezed_annotation/freezed_annotation.dart';

part 'activity_feed_item_model.freezed.dart';
part 'activity_feed_item_model.g.dart';

@freezed
class ActivityFeedItemModel with _$ActivityFeedItemModel {
  const factory ActivityFeedItemModel({
    required String id,
    required String userId,
    required String userName,
    String? userAvatarUrl,
    required String activityType,
    String? targetEntityId,
    String? metadata,
    required String createdAt,
  }) = _ActivityFeedItemModel;

  factory ActivityFeedItemModel.fromJson(Map<String, dynamic> json) =>
      _$ActivityFeedItemModelFromJson(json);
}

@freezed
class ActivityFeedResponse with _$ActivityFeedResponse {
  const factory ActivityFeedResponse({
    required List<ActivityFeedItemModel> items,
    String? nextCursor,
    required bool hasMore,
  }) = _ActivityFeedResponse;

  factory ActivityFeedResponse.fromJson(Map<String, dynamic> json) =>
      _$ActivityFeedResponseFromJson(json);
}
