import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/delivery_repository.dart';
import 'active_delivery_state.dart';

part 'active_delivery_notifier.g.dart';

@riverpod
class ActiveDeliveryNotifier extends _$ActiveDeliveryNotifier {
  late DeliveryRepository _repository;

  @override
  ActiveDeliveryState build() {
    _repository = ref.watch(deliveryRepositoryProvider);
    loadActiveDelivery();
    return const ActiveDeliveryState.initial();
  }

  Future<void> loadActiveDelivery() async {
    state = const ActiveDeliveryState.loading();
    final result = await _repository.getActiveDelivery();
    if (result.failure != null) {
      state = ActiveDeliveryState.error(failure: result.failure!);
    } else {
      state = ActiveDeliveryState.loaded(assignment: result.data);
    }
  }

  Future<bool> acceptDelivery(String assignmentId) async {
    final failure =
        await _repository.acceptDelivery(assignmentId: assignmentId);
    if (failure != null) return false;
    await loadActiveDelivery();
    return true;
  }

  Future<bool> updateStatus(String assignmentId, int newStatus) async {
    final failure = await _repository.updateDeliveryStatus(
      assignmentId: assignmentId,
      newStatus: newStatus,
    );
    if (failure != null) return false;
    await loadActiveDelivery();
    return true;
  }

  Future<void> updateLocation(double latitude, double longitude) async {
    await _repository.updateLocation(
      latitude: latitude,
      longitude: longitude,
    );
  }
}
