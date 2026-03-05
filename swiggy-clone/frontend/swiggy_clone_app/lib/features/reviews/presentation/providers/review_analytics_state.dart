import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/review_analytics_model.dart';

part 'review_analytics_state.freezed.dart';

@freezed
sealed class ReviewAnalyticsState with _$ReviewAnalyticsState {
  const factory ReviewAnalyticsState.initial() = ReviewAnalyticsInitial;
  const factory ReviewAnalyticsState.loading() = ReviewAnalyticsLoading;
  const factory ReviewAnalyticsState.loaded({
    required ReviewAnalyticsModel analytics,
  }) = ReviewAnalyticsLoaded;
  const factory ReviewAnalyticsState.error({required Failure failure}) =
      ReviewAnalyticsError;
}
