import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/dine_in_session_model.dart';

part 'active_session_state.freezed.dart';

@freezed
sealed class ActiveSessionState with _$ActiveSessionState {
  const factory ActiveSessionState.initial() = ActiveSessionInitial;
  const factory ActiveSessionState.loading() = ActiveSessionLoading;
  const factory ActiveSessionState.noSession() = ActiveSessionNone;
  const factory ActiveSessionState.active({
    required DineInSessionSummaryModel session,
  }) = ActiveSessionActive;
  const factory ActiveSessionState.error({required Failure failure}) =
      ActiveSessionError;
}
