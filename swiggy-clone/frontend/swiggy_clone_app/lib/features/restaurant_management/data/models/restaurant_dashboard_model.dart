import 'package:freezed_annotation/freezed_annotation.dart';

part 'restaurant_dashboard_model.freezed.dart';
part 'restaurant_dashboard_model.g.dart';

@freezed
class RestaurantDashboardModel with _$RestaurantDashboardModel {
  const factory RestaurantDashboardModel({
    required int totalOrders,
    required int pendingOrders,
    required int totalMenuItems,
    required int activeMenuItems,
    required double averageRating,
    required int totalRatings,
    required bool isAcceptingOrders,
    required String status,
    @Default(0) int totalTables,
    @Default(0) int activeTables,
    @Default(0) int activeSessions,
    @Default(0) int pendingDineInOrders,
  }) = _RestaurantDashboardModel;

  factory RestaurantDashboardModel.fromJson(Map<String, dynamic> json) =>
      _$RestaurantDashboardModelFromJson(json);
}
