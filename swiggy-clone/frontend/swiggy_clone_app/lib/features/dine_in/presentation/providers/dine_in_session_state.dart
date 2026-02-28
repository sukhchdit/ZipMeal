import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/dine_in_session_model.dart';

part 'dine_in_session_state.freezed.dart';

@freezed
sealed class DineInSessionState with _$DineInSessionState {
  const factory DineInSessionState.initial() = DineInSessionInitial;
  const factory DineInSessionState.loading() = DineInSessionLoading;
  const factory DineInSessionState.loaded({
    required DineInSessionModel session,
    required bool isHost,
  }) = DineInSessionLoaded;
  const factory DineInSessionState.ended() = DineInSessionEnded;
  const factory DineInSessionState.error({required Failure failure}) =
      DineInSessionError;
}
