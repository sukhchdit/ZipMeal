import 'package:freezed_annotation/freezed_annotation.dart';

part 'cuisine_type_model.freezed.dart';
part 'cuisine_type_model.g.dart';

@freezed
class CuisineTypeModel with _$CuisineTypeModel {
  const factory CuisineTypeModel({
    required String id,
    required String name,
    String? iconUrl,
  }) = _CuisineTypeModel;

  factory CuisineTypeModel.fromJson(Map<String, dynamic> json) =>
      _$CuisineTypeModelFromJson(json);
}
