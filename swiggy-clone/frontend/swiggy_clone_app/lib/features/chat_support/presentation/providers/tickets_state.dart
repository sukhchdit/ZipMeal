import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/support_ticket_model.dart';

part 'tickets_state.freezed.dart';

@freezed
sealed class TicketsState with _$TicketsState {
  const factory TicketsState.initial() = TicketsInitial;
  const factory TicketsState.loading() = TicketsLoading;
  const factory TicketsState.loaded({
    required List<SupportTicketSummaryModel> tickets,
    String? nextCursor,
    @Default(false) bool hasMore,
    @Default(false) bool isLoadingMore,
  }) = TicketsLoaded;
  const factory TicketsState.error({required Failure failure}) = TicketsError;
}
