import 'package:freezed_annotation/freezed_annotation.dart';

import '../../data/models/dispute_model.dart';

part 'create_dispute_state.freezed.dart';

@freezed
sealed class CreateDisputeState with _$CreateDisputeState {
  const factory CreateDisputeState.initial() = CreateDisputeInitial;
  const factory CreateDisputeState.submitting() = CreateDisputeSubmitting;
  const factory CreateDisputeState.success({required DisputeModel dispute}) =
      CreateDisputeSuccess;
  const factory CreateDisputeState.error({required String message}) =
      CreateDisputeError;
}
