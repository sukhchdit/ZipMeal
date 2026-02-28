import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/delivery_repository.dart';
import 'partner_online_state.dart';

part 'partner_online_notifier.g.dart';

@riverpod
class PartnerOnlineNotifier extends _$PartnerOnlineNotifier {
  late DeliveryRepository _repository;

  @override
  PartnerOnlineState build() {
    _repository = ref.watch(deliveryRepositoryProvider);
    return const PartnerOnlineState.initial();
  }

  void setInitialStatus({required bool isOnline}) {
    state = PartnerOnlineState.loaded(isOnline: isOnline);
  }

  Future<void> toggleOnline({
    required bool isOnline,
    double? latitude,
    double? longitude,
  }) async {
    state = const PartnerOnlineState.loading();
    final failure = await _repository.toggleOnlineStatus(
      isOnline: isOnline,
      latitude: latitude,
      longitude: longitude,
    );
    if (failure != null) {
      state = PartnerOnlineState.error(failure: failure);
    } else {
      state = PartnerOnlineState.loaded(isOnline: isOnline);
    }
  }
}
