import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/restaurant_dashboard_model.dart';

part 'dashboard_state.freezed.dart';

@freezed
sealed class DashboardState with _$DashboardState {
  const factory DashboardState.initial() = DashboardInitial;
  const factory DashboardState.loading() = DashboardLoading;
  const factory DashboardState.loaded({
    required RestaurantDashboardModel dashboard,
  }) = DashboardLoaded;
  const factory DashboardState.error({required Failure failure}) =
      DashboardError;
}
