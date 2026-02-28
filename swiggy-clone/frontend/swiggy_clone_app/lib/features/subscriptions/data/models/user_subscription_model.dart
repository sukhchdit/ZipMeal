import 'package:freezed_annotation/freezed_annotation.dart';

part 'user_subscription_model.freezed.dart';
part 'user_subscription_model.g.dart';

@freezed
class UserSubscriptionModel with _$UserSubscriptionModel {
  const factory UserSubscriptionModel({
    required String id,
    required String planId,
    required String planName,
    required int paidAmountPaise,
    required DateTime startDate,
    required DateTime endDate,
    required int status,
    required bool freeDelivery,
    required int extraDiscountPercent,
    required bool noSurgeFee,
  }) = _UserSubscriptionModel;

  factory UserSubscriptionModel.fromJson(Map<String, dynamic> json) =>
      _$UserSubscriptionModelFromJson(json);
}
