import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/loyalty_repository.dart';
import 'rewards_state.dart';

part 'rewards_notifier.g.dart';

@riverpod
class RewardsNotifier extends _$RewardsNotifier {
  late LoyaltyRepository _repository;

  @override
  RewardsState build() {
    _repository = ref.watch(loyaltyRepositoryProvider);
    loadRewards();
    return const RewardsState.initial();
  }

  Future<void> loadRewards() async {
    state = const RewardsState.loading();

    final result = await _repository.getRewards();
    if (result.failure != null) {
      state = RewardsState.error(failure: result.failure!);
      return;
    }

    state = RewardsState.loaded(rewards: result.data!);
  }

  Future<bool> redeemReward(String rewardId) async {
    final current = state;
    if (current is! RewardsLoaded) return false;

    state = current.copyWith(isRedeeming: true);
    final result = await _repository.redeemReward(rewardId);

    if (result.failure != null) {
      state = current.copyWith(isRedeeming: false);
      return false;
    }

    // Reload rewards to get updated stock
    await loadRewards();
    return true;
  }
}
