import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/admin_repository.dart';
import 'admin_config_state.dart';

part 'admin_config_notifier.g.dart';

@riverpod
class AdminConfigNotifier extends _$AdminConfigNotifier {
  late AdminRepository _repository;

  @override
  AdminConfigState build() {
    _repository = ref.watch(adminRepositoryProvider);
    loadConfig();
    return const AdminConfigState.initial();
  }

  Future<void> loadConfig() async {
    state = const AdminConfigState.loading();
    final result = await _repository.getPlatformConfig();
    if (result.failure != null) {
      state = AdminConfigState.error(failure: result.failure!);
    } else {
      state = AdminConfigState.loaded(config: result.data!);
    }
  }

  Future<bool> updateConfig({
    required int deliveryFeePaise,
    required int packagingChargePaise,
    required double taxRatePercent,
    int? freeDeliveryThresholdPaise,
  }) async {
    state = const AdminConfigState.saving();
    final result = await _repository.updatePlatformConfig({
      'deliveryFeePaise': deliveryFeePaise,
      'packagingChargePaise': packagingChargePaise,
      'taxRatePercent': taxRatePercent,
      'freeDeliveryThresholdPaise': freeDeliveryThresholdPaise,
    });
    if (result.failure != null) {
      state = AdminConfigState.error(failure: result.failure!);
      return false;
    }
    state = AdminConfigState.saved(config: result.data!);
    return true;
  }
}
