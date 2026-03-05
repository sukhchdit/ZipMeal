import 'package:freezed_annotation/freezed_annotation.dart';

part 'user_assignment_model.freezed.dart';
part 'user_assignment_model.g.dart';

@freezed
class UserAssignmentModel with _$UserAssignmentModel {
  const factory UserAssignmentModel({
    required String experimentKey,
    required String variantKey,
    String? configJson,
    required String assignedAt,
  }) = _UserAssignmentModel;

  factory UserAssignmentModel.fromJson(Map<String, dynamic> json) =>
      _$UserAssignmentModelFromJson(json);
}
