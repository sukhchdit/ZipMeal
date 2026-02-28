import 'package:freezed_annotation/freezed_annotation.dart';

part 'admin_user_model.freezed.dart';
part 'admin_user_model.g.dart';

@freezed
class AdminUserModel with _$AdminUserModel {
  const factory AdminUserModel({
    required String id,
    required String fullName,
    required String phoneNumber,
    String? email,
    String? avatarUrl,
    required int role,
    required bool isVerified,
    required bool isActive,
    DateTime? lastLoginAt,
    required DateTime createdAt,
  }) = _AdminUserModel;

  factory AdminUserModel.fromJson(Map<String, dynamic> json) =>
      _$AdminUserModelFromJson(json);
}
