import 'package:freezed_annotation/freezed_annotation.dart';

import '../../data/models/experiment_model.dart';

part 'experiment_detail_state.freezed.dart';

@freezed
sealed class ExperimentDetailState with _$ExperimentDetailState {
  const factory ExperimentDetailState.initial() = ExperimentDetailInitial;
  const factory ExperimentDetailState.loading() = ExperimentDetailLoading;
  const factory ExperimentDetailState.loaded({
    required ExperimentModel experiment,
    @Default(false) bool isPerformingAction,
  }) = ExperimentDetailLoaded;
  const factory ExperimentDetailState.error({required String message}) =
      ExperimentDetailError;
}
