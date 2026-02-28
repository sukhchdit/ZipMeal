import 'package:freezed_annotation/freezed_annotation.dart';

part 'admin_dashboard_model.freezed.dart';
part 'admin_dashboard_model.g.dart';

@freezed
class UserCountsModel with _$UserCountsModel {
  const factory UserCountsModel({
    required int total,
    required int customers,
    required int restaurantOwners,
    required int deliveryPartners,
    required int admins,
  }) = _UserCountsModel;

  factory UserCountsModel.fromJson(Map<String, dynamic> json) =>
      _$UserCountsModelFromJson(json);
}

@freezed
class RestaurantCountsModel with _$RestaurantCountsModel {
  const factory RestaurantCountsModel({
    required int total,
    required int pending,
    required int approved,
    required int suspended,
    required int rejected,
  }) = _RestaurantCountsModel;

  factory RestaurantCountsModel.fromJson(Map<String, dynamic> json) =>
      _$RestaurantCountsModelFromJson(json);
}

@freezed
class OrderCountsModel with _$OrderCountsModel {
  const factory OrderCountsModel({
    required int today,
    required int thisWeek,
    required int thisMonth,
    required int allTime,
  }) = _OrderCountsModel;

  factory OrderCountsModel.fromJson(Map<String, dynamic> json) =>
      _$OrderCountsModelFromJson(json);
}

@freezed
class RevenueModel with _$RevenueModel {
  const factory RevenueModel({
    required int today,
    required int thisWeek,
    required int thisMonth,
    required int allTime,
  }) = _RevenueModel;

  factory RevenueModel.fromJson(Map<String, dynamic> json) =>
      _$RevenueModelFromJson(json);
}

@freezed
class AdminOrderSummaryModel with _$AdminOrderSummaryModel {
  const factory AdminOrderSummaryModel({
    required String id,
    required String orderNumber,
    required String customerName,
    required String restaurantName,
    required int status,
    required int paymentStatus,
    required int totalAmount,
    required DateTime createdAt,
  }) = _AdminOrderSummaryModel;

  factory AdminOrderSummaryModel.fromJson(Map<String, dynamic> json) =>
      _$AdminOrderSummaryModelFromJson(json);
}

@freezed
class AdminDashboardModel with _$AdminDashboardModel {
  const factory AdminDashboardModel({
    required UserCountsModel userCounts,
    required RestaurantCountsModel restaurantCounts,
    required OrderCountsModel orderCounts,
    required RevenueModel revenue,
    @Default([]) List<AdminOrderSummaryModel> recentOrders,
  }) = _AdminDashboardModel;

  factory AdminDashboardModel.fromJson(Map<String, dynamic> json) =>
      _$AdminDashboardModelFromJson(json);
}
