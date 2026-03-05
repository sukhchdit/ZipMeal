import 'package:freezed_annotation/freezed_annotation.dart';

part 'loyalty_dashboard_model.freezed.dart';
part 'loyalty_dashboard_model.g.dart';

@freezed
class LoyaltyDashboardModel with _$LoyaltyDashboardModel {
  const factory LoyaltyDashboardModel({
    required int pointsBalance,
    required int lifetimePointsEarned,
    required LoyaltyTierModel currentTier,
    required int pointsToNextTier,
    required List<LoyaltyTransactionModel> recentTransactions,
    LoyaltyTierModel? nextTier,
  }) = _LoyaltyDashboardModel;

  factory LoyaltyDashboardModel.fromJson(Map<String, dynamic> json) =>
      _$LoyaltyDashboardModelFromJson(json);
}

@freezed
class LoyaltyTierModel with _$LoyaltyTierModel {
  const factory LoyaltyTierModel({
    required int level,
    required String name,
    required int minLifetimePoints,
    required double multiplier,
  }) = _LoyaltyTierModel;

  factory LoyaltyTierModel.fromJson(Map<String, dynamic> json) =>
      _$LoyaltyTierModelFromJson(json);
}

@freezed
class LoyaltyTransactionModel with _$LoyaltyTransactionModel {
  const factory LoyaltyTransactionModel({
    required String id,
    required int points,
    required int type,
    required int source,
    required String description,
    required int balanceAfter,
    required DateTime createdAt,
    String? referenceId,
  }) = _LoyaltyTransactionModel;

  factory LoyaltyTransactionModel.fromJson(Map<String, dynamic> json) =>
      _$LoyaltyTransactionModelFromJson(json);
}
