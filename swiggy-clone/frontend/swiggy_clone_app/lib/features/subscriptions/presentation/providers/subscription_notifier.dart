import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/subscription_repository.dart';
import 'subscription_state.dart';

part 'subscription_notifier.g.dart';

@riverpod
class SubscriptionNotifier extends _$SubscriptionNotifier {
  late SubscriptionRepository _repository;

  @override
  SubscriptionState build() {
    _repository = ref.watch(subscriptionRepositoryProvider);
    loadData();
    return const SubscriptionState.initial();
  }

  Future<void> loadData() async {
    state = const SubscriptionState.loading();

    final plansResult = await _repository.getAvailablePlans();
    if (plansResult.failure != null) {
      state = SubscriptionState.error(failure: plansResult.failure!);
      return;
    }

    final subResult = await _repository.getMySubscription();
    if (subResult.failure != null) {
      state = SubscriptionState.error(failure: subResult.failure!);
      return;
    }

    state = SubscriptionState.loaded(
      plans: plansResult.data!,
      activeSubscription: subResult.data,
    );
  }

  Future<bool> subscribe(String planId) async {
    final current = state;
    if (current is! SubscriptionLoaded) return false;

    state = current.copyWith(isSubscribing: true);
    final result = await _repository.subscribe(planId);

    if (result.failure != null) {
      state = current.copyWith(isSubscribing: false);
      return false;
    }

    state = current.copyWith(
      activeSubscription: result.data,
      isSubscribing: false,
    );
    return true;
  }

  Future<bool> cancelSubscription() async {
    final current = state;
    if (current is! SubscriptionLoaded) return false;

    state = current.copyWith(isCancelling: true);
    final result = await _repository.cancel();

    if (result.failure != null) {
      state = current.copyWith(isCancelling: false);
      return false;
    }

    state = current.copyWith(
      activeSubscription: null,
      isCancelling: false,
    );
    return true;
  }
}
