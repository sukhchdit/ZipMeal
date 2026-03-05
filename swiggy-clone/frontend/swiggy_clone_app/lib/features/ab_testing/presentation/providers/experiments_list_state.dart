import 'package:freezed_annotation/freezed_annotation.dart';

import '../../data/models/experiment_model.dart';

part 'experiments_list_state.freezed.dart';

@freezed
sealed class ExperimentsListState with _$ExperimentsListState {
  const factory ExperimentsListState.initial() = ExperimentsListInitial;
  const factory ExperimentsListState.loading() = ExperimentsListLoading;
  const factory ExperimentsListState.loaded({
    required List<ExperimentModel> experiments,
    required int totalCount,
    required int currentPage,
    required int pageSize,
    int? statusFilter,
    @Default(false) bool isLoadingMore,
  }) = ExperimentsListLoaded;
  const factory ExperimentsListState.error({required String message}) =
      ExperimentsListError;
}
