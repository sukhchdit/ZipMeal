import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/loyalty_dashboard_model.dart';

part 'loyalty_state.freezed.dart';

@freezed
sealed class LoyaltyState with _$LoyaltyState {
  const factory LoyaltyState.initial() = LoyaltyInitial;
  const factory LoyaltyState.loading() = LoyaltyLoading;
  const factory LoyaltyState.loaded({
    required LoyaltyDashboardModel dashboard,
  }) = LoyaltyLoaded;
  const factory LoyaltyState.error({required Failure failure}) = LoyaltyError;
}
