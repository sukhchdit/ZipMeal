import 'package:freezed_annotation/freezed_annotation.dart';

import 'data_point_model.dart';

part 'partner_analytics_model.freezed.dart';
part 'partner_analytics_model.g.dart';

@freezed
class PartnerAnalyticsModel with _$PartnerAnalyticsModel {
  const factory PartnerAnalyticsModel({
    @Default([]) List<DataPointModel> earningsTrend,
    @Default([]) List<DataPointModel> deliveryCountTrend,
    required double avgDeliveryTimeMinutes,
    required double completionRatePercent,
    required int totalDeliveries,
    required int totalEarnings,
    required double avgRating,
  }) = _PartnerAnalyticsModel;

  factory PartnerAnalyticsModel.fromJson(Map<String, dynamic> json) =>
      _$PartnerAnalyticsModelFromJson(json);
}
