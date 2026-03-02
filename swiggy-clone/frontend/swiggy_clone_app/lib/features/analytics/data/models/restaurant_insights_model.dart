import 'package:freezed_annotation/freezed_annotation.dart';

import 'data_point_model.dart';
import 'menu_item_performance_model.dart';
import 'named_value_model.dart';

part 'restaurant_insights_model.freezed.dart';
part 'restaurant_insights_model.g.dart';

@freezed
class RestaurantInsightsModel with _$RestaurantInsightsModel {
  const factory RestaurantInsightsModel({
    required int newCustomers,
    required int repeatCustomers,
    required double repeatRate,
    @Default([]) List<DataPointModel> customerRetentionTrend,
    @Default([]) List<MenuItemPerformanceModel> menuPerformance,
    required double completionRate,
    required double cancellationRate,
    @Default([]) List<DataPointModel> orderCompletionTrend,
    @Default([]) List<NamedValueModel> revenueByOrderType,
    @Default([]) List<NamedValueModel> revenueByDayOfWeek,
    required double avgRevenuePerCustomer,
  }) = _RestaurantInsightsModel;

  factory RestaurantInsightsModel.fromJson(Map<String, dynamic> json) =>
      _$RestaurantInsightsModelFromJson(json);
}
