import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/notification_repository.dart';

part 'device_registration_notifier.g.dart';

@riverpod
class DeviceRegistrationNotifier extends _$DeviceRegistrationNotifier {
  late NotificationRepository _repository;

  @override
  bool build() {
    _repository = ref.watch(notificationRepositoryProvider);
    return false; // not registered
  }

  Future<void> registerDevice({
    required String deviceToken,
    required int platform,
  }) async {
    final failure = await _repository.registerDevice(
      deviceToken: deviceToken,
      platform: platform,
    );
    if (failure == null) state = true;
  }

  Future<void> unregisterDevice({required String deviceToken}) async {
    final failure = await _repository.unregisterDevice(
      deviceToken: deviceToken,
    );
    if (failure == null) state = false;
  }
}
