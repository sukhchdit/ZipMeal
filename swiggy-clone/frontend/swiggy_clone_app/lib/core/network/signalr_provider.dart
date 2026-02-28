import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../features/auth/presentation/providers/auth_notifier.dart';
import '../../features/auth/presentation/providers/auth_state.dart';
import '../storage/secure_storage_service.dart';
import 'signalr_service.dart';

part 'signalr_provider.g.dart';

/// Global singleton SignalR service that auto-connects on auth changes.
@Riverpod(keepAlive: true)
SignalRService signalRService(SignalRServiceRef ref) {
  final secureStorage = ref.watch(secureStorageServiceProvider);
  final service = SignalRService(secureStorage: secureStorage);

  ref.listen(authNotifierProvider, (prev, next) {
    if (next is AuthAuthenticated) {
      service.connect();
    } else if (next is AuthUnauthenticated) {
      service.disconnect();
    }
  });

  ref.onDispose(() {
    service.disconnect();
  });

  return service;
}
