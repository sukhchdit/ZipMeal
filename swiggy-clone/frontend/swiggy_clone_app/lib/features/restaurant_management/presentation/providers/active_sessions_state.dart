import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/owner_session_model.dart';

part 'active_sessions_state.freezed.dart';

@freezed
sealed class ActiveSessionsState with _$ActiveSessionsState {
  const factory ActiveSessionsState.initial() = ActiveSessionsInitial;
  const factory ActiveSessionsState.loading() = ActiveSessionsLoading;
  const factory ActiveSessionsState.loaded({
    required List<OwnerSessionModel> sessions,
  }) = ActiveSessionsLoaded;
  const factory ActiveSessionsState.error({required Failure failure}) =
      ActiveSessionsError;
}
