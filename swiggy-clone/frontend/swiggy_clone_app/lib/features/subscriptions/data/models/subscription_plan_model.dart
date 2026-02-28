import 'package:freezed_annotation/freezed_annotation.dart';

part 'subscription_plan_model.freezed.dart';
part 'subscription_plan_model.g.dart';

@freezed
class SubscriptionPlanModel with _$SubscriptionPlanModel {
  const factory SubscriptionPlanModel({
    required String id,
    required String name,
    String? description,
    required int pricePaise,
    required int durationDays,
    required bool freeDelivery,
    required int extraDiscountPercent,
    required bool noSurgeFee,
    required bool isActive,
  }) = _SubscriptionPlanModel;

  factory SubscriptionPlanModel.fromJson(Map<String, dynamic> json) =>
      _$SubscriptionPlanModelFromJson(json);
}
