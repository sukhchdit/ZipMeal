import 'package:freezed_annotation/freezed_annotation.dart';

part 'referral_stats_model.freezed.dart';
part 'referral_stats_model.g.dart';

@freezed
class ReferralStatsModel with _$ReferralStatsModel {
  const factory ReferralStatsModel({
    required String referralCode,
    required int totalReferrals,
    required int totalRewardsPaise,
  }) = _ReferralStatsModel;

  factory ReferralStatsModel.fromJson(Map<String, dynamic> json) =>
      _$ReferralStatsModelFromJson(json);
}
