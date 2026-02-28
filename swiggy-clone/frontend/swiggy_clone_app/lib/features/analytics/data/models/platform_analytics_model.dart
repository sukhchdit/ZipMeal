import 'package:freezed_annotation/freezed_annotation.dart';

import 'data_point_model.dart';
import 'named_value_model.dart';

part 'platform_analytics_model.freezed.dart';
part 'platform_analytics_model.g.dart';

@freezed
class CouponAnalyticsModel with _$CouponAnalyticsModel {
  const factory CouponAnalyticsModel({
    required int totalCouponsUsed,
    required int totalDiscountGiven,
    required int uniqueCouponsUsed,
    @Default([]) List<NamedValueModel> topCouponsByUsage,
  }) = _CouponAnalyticsModel;

  factory CouponAnalyticsModel.fromJson(Map<String, dynamic> json) =>
      _$CouponAnalyticsModelFromJson(json);
}

@freezed
class DeliveryPerformanceModel with _$DeliveryPerformanceModel {
  const factory DeliveryPerformanceModel({
    required double avgDeliveryTimeMinutes,
    required double completionRatePercent,
    required int totalDeliveries,
    required int cancelledDeliveries,
  }) = _DeliveryPerformanceModel;

  factory DeliveryPerformanceModel.fromJson(Map<String, dynamic> json) =>
      _$DeliveryPerformanceModelFromJson(json);
}

@freezed
class PlatformAnalyticsModel with _$PlatformAnalyticsModel {
  const factory PlatformAnalyticsModel({
    @Default([]) List<DataPointModel> revenueTrend,
    @Default([]) List<DataPointModel> orderTrend,
    @Default([]) List<DataPointModel> userGrowthTrend,
    @Default([]) List<NamedValueModel> orderStatusDistribution,
    @Default([]) List<NamedValueModel> orderTypeDistribution,
    @Default([]) List<NamedValueModel> paymentMethodDistribution,
    @Default([]) List<NamedValueModel> topRestaurantsByRevenue,
    @Default([]) List<NamedValueModel> topRestaurantsByOrders,
    @Default([]) List<NamedValueModel> popularMenuItems,
    required CouponAnalyticsModel couponStats,
    required DeliveryPerformanceModel deliveryPerformance,
    @Default([]) List<DataPointModel> peakHoursDistribution,
  }) = _PlatformAnalyticsModel;

  factory PlatformAnalyticsModel.fromJson(Map<String, dynamic> json) =>
      _$PlatformAnalyticsModelFromJson(json);
}
