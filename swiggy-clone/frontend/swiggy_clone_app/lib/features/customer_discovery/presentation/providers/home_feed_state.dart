import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/home_feed_model.dart';

part 'home_feed_state.freezed.dart';

@freezed
sealed class HomeFeedState with _$HomeFeedState {
  const factory HomeFeedState.initial() = HomeFeedInitial;
  const factory HomeFeedState.loading() = HomeFeedLoading;
  const factory HomeFeedState.loaded({required HomeFeedModel feed}) =
      HomeFeedLoaded;
  const factory HomeFeedState.error({required Failure failure}) =
      HomeFeedError;
}
