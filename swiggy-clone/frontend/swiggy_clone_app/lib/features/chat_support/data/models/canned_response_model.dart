import 'package:freezed_annotation/freezed_annotation.dart';

part 'canned_response_model.freezed.dart';
part 'canned_response_model.g.dart';

@freezed
class CannedResponseModel with _$CannedResponseModel {
  const factory CannedResponseModel({
    required String id,
    required String title,
    required String content,
    required int category,
    required int sortOrder,
  }) = _CannedResponseModel;

  factory CannedResponseModel.fromJson(Map<String, dynamic> json) =>
      _$CannedResponseModelFromJson(json);
}
