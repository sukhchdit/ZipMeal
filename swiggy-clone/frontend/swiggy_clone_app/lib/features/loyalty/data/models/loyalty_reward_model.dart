import 'package:freezed_annotation/freezed_annotation.dart';

part 'loyalty_reward_model.freezed.dart';
part 'loyalty_reward_model.g.dart';

@freezed
class LoyaltyRewardModel with _$LoyaltyRewardModel {
  const factory LoyaltyRewardModel({
    required String id,
    required String name,
    required int pointsCost,
    required int rewardType,
    required int rewardValue,
    String? description,
    int? stock,
    DateTime? expiresAt,
  }) = _LoyaltyRewardModel;

  factory LoyaltyRewardModel.fromJson(Map<String, dynamic> json) =>
      _$LoyaltyRewardModelFromJson(json);
}
