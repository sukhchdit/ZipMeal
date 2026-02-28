import 'package:freezed_annotation/freezed_annotation.dart';

part 'data_point_model.freezed.dart';
part 'data_point_model.g.dart';

@freezed
class DataPointModel with _$DataPointModel {
  const factory DataPointModel({
    required String label,
    required double value,
  }) = _DataPointModel;

  factory DataPointModel.fromJson(Map<String, dynamic> json) =>
      _$DataPointModelFromJson(json);
}
