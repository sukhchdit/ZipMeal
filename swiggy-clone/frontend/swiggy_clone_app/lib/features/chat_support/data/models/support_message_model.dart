import 'package:freezed_annotation/freezed_annotation.dart';

part 'support_message_model.freezed.dart';
part 'support_message_model.g.dart';

@freezed
class SupportMessageModel with _$SupportMessageModel {
  const factory SupportMessageModel({
    required String id,
    required String ticketId,
    required String senderId,
    required String senderName,
    required String content,
    required int messageType,
    @Default(false) bool isRead,
    String? readAt,
    required String createdAt,
  }) = _SupportMessageModel;

  factory SupportMessageModel.fromJson(Map<String, dynamic> json) =>
      _$SupportMessageModelFromJson(json);
}
