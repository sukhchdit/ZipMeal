import 'package:freezed_annotation/freezed_annotation.dart';

import 'data_point_model.dart';
import 'named_value_model.dart';

part 'restaurant_analytics_model.freezed.dart';
part 'restaurant_analytics_model.g.dart';

@freezed
class RestaurantAnalyticsModel with _$RestaurantAnalyticsModel {
  const factory RestaurantAnalyticsModel({
    @Default([]) List<DataPointModel> revenueTrend,
    @Default([]) List<DataPointModel> orderTrend,
    @Default([]) List<NamedValueModel> topMenuItems,
    @Default([]) List<DataPointModel> ratingTrend,
    @Default([]) List<NamedValueModel> orderTypeDistribution,
    @Default([]) List<NamedValueModel> orderStatusDistribution,
    @Default([]) List<DataPointModel> peakHoursDistribution,
    required double averageOrderValue,
  }) = _RestaurantAnalyticsModel;

  factory RestaurantAnalyticsModel.fromJson(Map<String, dynamic> json) =>
      _$RestaurantAnalyticsModelFromJson(json);
}
