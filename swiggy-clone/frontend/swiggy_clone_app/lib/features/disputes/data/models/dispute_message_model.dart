import 'package:freezed_annotation/freezed_annotation.dart';

part 'dispute_message_model.freezed.dart';
part 'dispute_message_model.g.dart';

@freezed
class DisputeMessageModel with _$DisputeMessageModel {
  const factory DisputeMessageModel({
    required String id,
    required String disputeId,
    required String senderId,
    @Default('') String senderName,
    required String content,
    @Default(false) bool isSystemMessage,
    @Default(false) bool isRead,
    String? readAt,
    required String createdAt,
  }) = _DisputeMessageModel;

  factory DisputeMessageModel.fromJson(Map<String, dynamic> json) =>
      _$DisputeMessageModelFromJson(json);
}
