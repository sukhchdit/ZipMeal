import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/reorder_result_model.dart';

part 'reorder_state.freezed.dart';

@freezed
sealed class ReorderState with _$ReorderState {
  const factory ReorderState.idle() = ReorderIdle;
  const factory ReorderState.loading() = ReorderLoading;
  const factory ReorderState.success({
    required ReorderResultModel result,
  }) = ReorderSuccess;
  const factory ReorderState.error({required Failure failure}) = ReorderError;
}
