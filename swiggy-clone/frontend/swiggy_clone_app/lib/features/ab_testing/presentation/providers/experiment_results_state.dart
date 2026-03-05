import 'package:freezed_annotation/freezed_annotation.dart';

import '../../data/models/experiment_stats_model.dart';

part 'experiment_results_state.freezed.dart';

@freezed
sealed class ExperimentResultsState with _$ExperimentResultsState {
  const factory ExperimentResultsState.initial() = ExperimentResultsInitial;
  const factory ExperimentResultsState.loading() = ExperimentResultsLoading;
  const factory ExperimentResultsState.loaded(
      {required ExperimentStatsModel stats}) = ExperimentResultsLoaded;
  const factory ExperimentResultsState.error({required String message}) =
      ExperimentResultsError;
}
