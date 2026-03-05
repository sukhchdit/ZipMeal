import 'package:freezed_annotation/freezed_annotation.dart';

part 'review_analytics_model.freezed.dart';
part 'review_analytics_model.g.dart';

@freezed
class ReviewAnalyticsModel with _$ReviewAnalyticsModel {
  const factory ReviewAnalyticsModel({
    required double averageRating,
    required int totalReviews,
    required int photoReviewsCount,
    double? averageDeliveryRating,
    required Map<String, int> ratingDistribution,
    @Default([]) List<MonthlyTrendItem> monthlyTrend,
  }) = _ReviewAnalyticsModel;

  factory ReviewAnalyticsModel.fromJson(Map<String, dynamic> json) =>
      _$ReviewAnalyticsModelFromJson(json);
}

@freezed
class MonthlyTrendItem with _$MonthlyTrendItem {
  const factory MonthlyTrendItem({
    required String month,
    required int count,
    required double avgRating,
  }) = _MonthlyTrendItem;

  factory MonthlyTrendItem.fromJson(Map<String, dynamic> json) =>
      _$MonthlyTrendItemFromJson(json);
}
