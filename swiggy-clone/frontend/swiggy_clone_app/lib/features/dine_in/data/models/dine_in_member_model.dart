import 'package:freezed_annotation/freezed_annotation.dart';

part 'dine_in_member_model.freezed.dart';
part 'dine_in_member_model.g.dart';

@freezed
class DineInMemberModel with _$DineInMemberModel {
  const factory DineInMemberModel({
    required String userId,
    required String fullName,
    String? avatarUrl,
    required int role,
    required String joinedAt,
  }) = _DineInMemberModel;

  factory DineInMemberModel.fromJson(Map<String, dynamic> json) =>
      _$DineInMemberModelFromJson(json);
}
