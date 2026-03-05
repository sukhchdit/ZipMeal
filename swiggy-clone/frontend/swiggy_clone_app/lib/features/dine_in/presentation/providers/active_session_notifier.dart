import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../auth/presentation/providers/auth_notifier.dart';
import '../../../auth/presentation/providers/auth_state.dart';
import '../../data/models/dine_in_session_model.dart';
import '../../data/repositories/dine_in_repository.dart';
import 'active_session_state.dart';

part 'active_session_notifier.g.dart';

@Riverpod(keepAlive: true)
class ActiveSessionNotifier extends _$ActiveSessionNotifier {
  late DineInRepository _repository;

  @override
  ActiveSessionState build() {
    _repository = ref.watch(dineInRepositoryProvider);
    final authState = ref.watch(authNotifierProvider);
    if (authState is AuthAuthenticated) {
      checkActiveSession();
    } else {
      return const ActiveSessionState.noSession();
    }
    return const ActiveSessionState.initial();
  }

  Future<void> checkActiveSession() async {
    state = const ActiveSessionState.loading();
    final result = await _repository.getActiveSession();
    if (result.failure != null) {
      state = ActiveSessionState.error(failure: result.failure!);
    } else if (result.data == null) {
      state = const ActiveSessionState.noSession();
    } else {
      state = ActiveSessionState.active(session: result.data!);
    }
  }

  Future<({bool success, DineInSessionModel? session})> startSession({
    required String qrCodeData,
    int guestCount = 1,
  }) async {
    state = const ActiveSessionState.loading();
    final result = await _repository.startSession(
      qrCodeData: qrCodeData,
      guestCount: guestCount,
    );
    if (result.failure != null) {
      state = ActiveSessionState.error(failure: result.failure!);
      return (success: false, session: null);
    }
    // Convert full session to summary for the active state
    final session = result.data!;
    state = ActiveSessionState.active(
      session: DineInSessionSummaryModel(
        id: session.id,
        restaurantId: session.restaurantId,
        restaurantName: session.restaurantName,
        restaurantLogoUrl: session.restaurantLogoUrl,
        tableNumber: session.table.tableNumber,
        sessionCode: session.sessionCode,
        status: session.status,
        guestCount: session.guestCount,
        startedAt: session.startedAt,
      ),
    );
    return (success: true, session: session);
  }

  Future<({bool success, DineInSessionModel? session})> joinSession({
    required String sessionCode,
  }) async {
    state = const ActiveSessionState.loading();
    final result = await _repository.joinSession(sessionCode: sessionCode);
    if (result.failure != null) {
      state = ActiveSessionState.error(failure: result.failure!);
      return (success: false, session: null);
    }
    final session = result.data!;
    state = ActiveSessionState.active(
      session: DineInSessionSummaryModel(
        id: session.id,
        restaurantId: session.restaurantId,
        restaurantName: session.restaurantName,
        restaurantLogoUrl: session.restaurantLogoUrl,
        tableNumber: session.table.tableNumber,
        sessionCode: session.sessionCode,
        status: session.status,
        guestCount: session.guestCount,
        startedAt: session.startedAt,
      ),
    );
    return (success: true, session: session);
  }

  void clearSession() {
    state = const ActiveSessionState.noSession();
  }
}
