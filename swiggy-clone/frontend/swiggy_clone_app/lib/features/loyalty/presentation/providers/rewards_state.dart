import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/loyalty_reward_model.dart';

part 'rewards_state.freezed.dart';

@freezed
sealed class RewardsState with _$RewardsState {
  const factory RewardsState.initial() = RewardsInitial;
  const factory RewardsState.loading() = RewardsLoading;
  const factory RewardsState.loaded({
    required List<LoyaltyRewardModel> rewards,
    @Default(false) bool isRedeeming,
  }) = RewardsLoaded;
  const factory RewardsState.error({required Failure failure}) = RewardsError;
}
