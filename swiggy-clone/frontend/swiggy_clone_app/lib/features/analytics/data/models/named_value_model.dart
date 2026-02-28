import 'package:freezed_annotation/freezed_annotation.dart';

part 'named_value_model.freezed.dart';
part 'named_value_model.g.dart';

@freezed
class NamedValueModel with _$NamedValueModel {
  const factory NamedValueModel({
    required String name,
    required double value,
  }) = _NamedValueModel;

  factory NamedValueModel.fromJson(Map<String, dynamic> json) =>
      _$NamedValueModelFromJson(json);
}
