import 'package:freezed_annotation/freezed_annotation.dart';

part 'file_upload_result_model.freezed.dart';
part 'file_upload_result_model.g.dart';

@freezed
class FileUploadResultModel with _$FileUploadResultModel {
  const factory FileUploadResultModel({
    required String url,
  }) = _FileUploadResultModel;

  factory FileUploadResultModel.fromJson(Map<String, dynamic> json) =>
      _$FileUploadResultModelFromJson(json);
}
