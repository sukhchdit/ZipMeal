import 'package:freezed_annotation/freezed_annotation.dart';

part 'operating_hours_model.freezed.dart';
part 'operating_hours_model.g.dart';

@freezed
class OperatingHoursModel with _$OperatingHoursModel {
  const factory OperatingHoursModel({
    required String id,
    required int dayOfWeek,
    String? openTime,
    String? closeTime,
    required bool isClosed,
  }) = _OperatingHoursModel;

  factory OperatingHoursModel.fromJson(Map<String, dynamic> json) =>
      _$OperatingHoursModelFromJson(json);
}
