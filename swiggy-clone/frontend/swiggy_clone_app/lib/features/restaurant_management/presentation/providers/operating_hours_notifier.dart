import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/restaurant_repository.dart';
import 'operating_hours_state.dart';

part 'operating_hours_notifier.g.dart';

@riverpod
class OperatingHoursNotifier extends _$OperatingHoursNotifier {
  late RestaurantRepository _repository;

  @override
  OperatingHoursState build(String restaurantId) {
    _repository = ref.watch(restaurantRepositoryProvider);
    loadHours();
    return const OperatingHoursState.initial();
  }

  Future<void> loadHours() async {
    state = const OperatingHoursState.loading();
    final result =
        await _repository.getOperatingHours(restaurantId: restaurantId);
    if (result.failure != null) {
      state = OperatingHoursState.error(failure: result.failure!);
    } else {
      state = OperatingHoursState.loaded(hours: result.data!);
    }
  }

  Future<bool> upsertHours(Map<String, dynamic> data) async {
    final result = await _repository.upsertOperatingHours(
      restaurantId: restaurantId,
      data: data,
    );
    if (result.failure != null) return false;
    state = OperatingHoursState.loaded(hours: result.data!);
    return true;
  }
}
