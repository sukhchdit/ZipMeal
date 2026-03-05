import 'package:freezed_annotation/freezed_annotation.dart';

part 'dispute_model.freezed.dart';
part 'dispute_model.g.dart';

@freezed
class DisputeModel with _$DisputeModel {
  const factory DisputeModel({
    required String id,
    required String disputeNumber,
    required String orderId,
    @Default('') String? orderNumber,
    required String userId,
    @Default('') String userName,
    String? assignedAgentId,
    String? assignedAgentName,
    required int issueType,
    required int status,
    required String description,
    int? resolutionType,
    int? resolutionAmountPaise,
    String? resolutionNotes,
    String? resolvedAt,
    String? rejectionReason,
    String? escalatedAt,
    required String createdAt,
    required String updatedAt,
  }) = _DisputeModel;

  factory DisputeModel.fromJson(Map<String, dynamic> json) =>
      _$DisputeModelFromJson(json);
}

@freezed
class DisputeSummaryModel with _$DisputeSummaryModel {
  const factory DisputeSummaryModel({
    required String id,
    required String disputeNumber,
    String? orderNumber,
    required int issueType,
    required int status,
    String? lastMessage,
    @Default(0) int unreadCount,
    required String createdAt,
    required String updatedAt,
  }) = _DisputeSummaryModel;

  factory DisputeSummaryModel.fromJson(Map<String, dynamic> json) =>
      _$DisputeSummaryModelFromJson(json);
}
