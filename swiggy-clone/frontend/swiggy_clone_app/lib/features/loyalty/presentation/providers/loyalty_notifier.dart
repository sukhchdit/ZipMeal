import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/loyalty_repository.dart';
import 'loyalty_state.dart';

part 'loyalty_notifier.g.dart';

@riverpod
class LoyaltyNotifier extends _$LoyaltyNotifier {
  late LoyaltyRepository _repository;

  @override
  LoyaltyState build() {
    _repository = ref.watch(loyaltyRepositoryProvider);
    loadDashboard();
    return const LoyaltyState.initial();
  }

  Future<void> loadDashboard() async {
    state = const LoyaltyState.loading();

    final result = await _repository.getDashboard();
    if (result.failure != null) {
      state = LoyaltyState.error(failure: result.failure!);
      return;
    }

    state = LoyaltyState.loaded(dashboard: result.data!);
  }
}
