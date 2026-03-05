import 'package:freezed_annotation/freezed_annotation.dart';

import '../../data/models/dispute_model.dart';

part 'disputes_state.freezed.dart';

@freezed
sealed class DisputesState with _$DisputesState {
  const factory DisputesState.initial() = DisputesInitial;
  const factory DisputesState.loading() = DisputesLoading;
  const factory DisputesState.loaded({
    required List<DisputeSummaryModel> disputes,
    String? nextCursor,
    @Default(false) bool hasMore,
    @Default(false) bool isLoadingMore,
  }) = DisputesLoaded;
  const factory DisputesState.error({required String message}) = DisputesError;
}
