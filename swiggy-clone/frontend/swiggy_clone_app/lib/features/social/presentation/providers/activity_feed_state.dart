import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/activity_feed_item_model.dart';

part 'activity_feed_state.freezed.dart';

@freezed
class ActivityFeedState with _$ActivityFeedState {
  const factory ActivityFeedState.initial() = _Initial;
  const factory ActivityFeedState.loading() = _Loading;
  const factory ActivityFeedState.loaded({
    required List<ActivityFeedItemModel> items,
    String? nextCursor,
    required bool hasMore,
    @Default(false) bool isLoadingMore,
  }) = ActivityFeedLoaded;
  const factory ActivityFeedState.error({required Failure failure}) = _Error;
}
