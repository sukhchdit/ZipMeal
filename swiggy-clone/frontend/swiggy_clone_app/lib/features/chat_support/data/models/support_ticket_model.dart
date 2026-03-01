import 'package:freezed_annotation/freezed_annotation.dart';

part 'support_ticket_model.freezed.dart';
part 'support_ticket_model.g.dart';

@freezed
class SupportTicketModel with _$SupportTicketModel {
  const factory SupportTicketModel({
    required String id,
    required String userId,
    String? userName,
    String? assignedAgentId,
    String? assignedAgentName,
    required String subject,
    required int category,
    required int status,
    String? orderId,
    String? lastMessage,
    String? closedAt,
    required String createdAt,
    required String updatedAt,
  }) = _SupportTicketModel;

  factory SupportTicketModel.fromJson(Map<String, dynamic> json) =>
      _$SupportTicketModelFromJson(json);
}

@freezed
class SupportTicketSummaryModel with _$SupportTicketSummaryModel {
  const factory SupportTicketSummaryModel({
    required String id,
    required String subject,
    required int category,
    required int status,
    String? lastMessage,
    @Default(0) int unreadCount,
    required String createdAt,
    required String updatedAt,
  }) = _SupportTicketSummaryModel;

  factory SupportTicketSummaryModel.fromJson(Map<String, dynamic> json) =>
      _$SupportTicketSummaryModelFromJson(json);
}
