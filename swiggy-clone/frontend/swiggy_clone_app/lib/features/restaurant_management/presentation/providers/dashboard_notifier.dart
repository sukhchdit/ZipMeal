import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/restaurant_repository.dart';
import 'dashboard_state.dart';

part 'dashboard_notifier.g.dart';

@riverpod
class DashboardNotifier extends _$DashboardNotifier {
  late RestaurantRepository _repository;

  @override
  DashboardState build(String restaurantId) {
    _repository = ref.watch(restaurantRepositoryProvider);
    loadDashboard();
    return const DashboardState.initial();
  }

  Future<void> loadDashboard() async {
    state = const DashboardState.loading();
    final result = await _repository.getDashboard(id: restaurantId);
    if (result.failure != null) {
      state = DashboardState.error(failure: result.failure!);
    } else {
      state = DashboardState.loaded(dashboard: result.data!);
    }
  }
}
