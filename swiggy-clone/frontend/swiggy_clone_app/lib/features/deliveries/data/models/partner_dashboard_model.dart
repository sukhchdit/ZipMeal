import 'package:freezed_annotation/freezed_annotation.dart';

part 'partner_dashboard_model.freezed.dart';
part 'partner_dashboard_model.g.dart';

@freezed
class PartnerDashboardModel with _$PartnerDashboardModel {
  const factory PartnerDashboardModel({
    required bool isOnline,
    required int totalDeliveries,
    required int todayDeliveries,
    required int todayEarnings,
    required int totalEarnings,
    @Default(0) int todayTips,
    @Default(0) int totalTips,
  }) = _PartnerDashboardModel;

  factory PartnerDashboardModel.fromJson(Map<String, dynamic> json) =>
      _$PartnerDashboardModelFromJson(json);
}
