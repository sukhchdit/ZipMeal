import 'package:freezed_annotation/freezed_annotation.dart';

part 'group_order_participant_model.freezed.dart';
part 'group_order_participant_model.g.dart';

@freezed
class GroupOrderParticipantModel with _$GroupOrderParticipantModel {
  const factory GroupOrderParticipantModel({
    required String id,
    required String userId,
    required String userName,
    String? avatarUrl,
    required bool isInitiator,
    required int status,
    required String joinedAt,
    String? leftAt,
    @Default(0) int itemCount,
    @Default(0) int itemsTotal,
  }) = _GroupOrderParticipantModel;

  factory GroupOrderParticipantModel.fromJson(Map<String, dynamic> json) =>
      _$GroupOrderParticipantModelFromJson(json);
}
