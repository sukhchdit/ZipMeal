import 'package:freezed_annotation/freezed_annotation.dart';

import '../../data/models/dispute_message_model.dart';
import '../../data/models/dispute_model.dart';

part 'dispute_detail_state.freezed.dart';

@freezed
sealed class DisputeDetailState with _$DisputeDetailState {
  const factory DisputeDetailState.initial() = DisputeDetailInitial;
  const factory DisputeDetailState.loading() = DisputeDetailLoading;
  const factory DisputeDetailState.loaded({
    required DisputeModel dispute,
    required List<DisputeMessageModel> messages,
    String? nextCursor,
    @Default(false) bool hasMore,
    @Default(false) bool isLoadingMore,
    @Default(false) bool isSending,
  }) = DisputeDetailLoaded;
  const factory DisputeDetailState.error({required String message}) =
      DisputeDetailError;
}
