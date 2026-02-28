import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/delivery_tracking_model.dart';
import '../../data/repositories/delivery_repository.dart';

part 'delivery_tracking_notifier.freezed.dart';
part 'delivery_tracking_notifier.g.dart';

@freezed
sealed class DeliveryTrackingState with _$DeliveryTrackingState {
  const factory DeliveryTrackingState.initial() = DeliveryTrackingInitial;
  const factory DeliveryTrackingState.loading() = DeliveryTrackingLoading;
  const factory DeliveryTrackingState.loaded({
    required DeliveryTrackingModel tracking,
  }) = DeliveryTrackingLoaded;
  const factory DeliveryTrackingState.error({required Failure failure}) =
      DeliveryTrackingError;
}

@riverpod
class DeliveryTrackingNotifier extends _$DeliveryTrackingNotifier {
  late DeliveryRepository _repository;
  late String _orderId;

  @override
  DeliveryTrackingState build(String orderId) {
    _repository = ref.watch(deliveryRepositoryProvider);
    _orderId = orderId;
    loadTracking();
    return const DeliveryTrackingState.initial();
  }

  Future<void> loadTracking() async {
    state = const DeliveryTrackingState.loading();
    final result =
        await _repository.getDeliveryTracking(orderId: _orderId);
    if (result.failure != null) {
      state = DeliveryTrackingState.error(failure: result.failure!);
    } else {
      state = DeliveryTrackingState.loaded(tracking: result.data!);
    }
  }

  Future<void> refresh() async {
    final result =
        await _repository.getDeliveryTracking(orderId: _orderId);
    if (result.failure == null && result.data != null) {
      state = DeliveryTrackingState.loaded(tracking: result.data!);
    }
  }
}
