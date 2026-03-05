import 'package:freezed_annotation/freezed_annotation.dart';

import '../../data/models/experiment_model.dart';

part 'create_experiment_state.freezed.dart';

@freezed
sealed class CreateExperimentState with _$CreateExperimentState {
  const factory CreateExperimentState.initial() = CreateExperimentInitial;
  const factory CreateExperimentState.submitting() = CreateExperimentSubmitting;
  const factory CreateExperimentState.success(
      {required ExperimentModel experiment}) = CreateExperimentSuccess;
  const factory CreateExperimentState.error({required String message}) =
      CreateExperimentError;
}
