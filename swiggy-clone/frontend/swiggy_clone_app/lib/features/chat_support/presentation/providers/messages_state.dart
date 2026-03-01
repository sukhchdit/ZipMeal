import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/support_message_model.dart';

part 'messages_state.freezed.dart';

@freezed
sealed class MessagesState with _$MessagesState {
  const factory MessagesState.initial() = MessagesInitial;
  const factory MessagesState.loading() = MessagesLoading;
  const factory MessagesState.loaded({
    required List<SupportMessageModel> messages,
    String? nextCursor,
    @Default(false) bool hasMore,
    @Default(false) bool isLoadingMore,
  }) = MessagesLoaded;
  const factory MessagesState.error({required Failure failure}) = MessagesError;
}
