import 'package:freezed_annotation/freezed_annotation.dart';

part 'forecast_point_model.freezed.dart';
part 'forecast_point_model.g.dart';

@freezed
class ForecastPointModel with _$ForecastPointModel {
  const factory ForecastPointModel({
    required String label,
    required double predictedValue,
    required double lowerBound,
    required double upperBound,
  }) = _ForecastPointModel;

  factory ForecastPointModel.fromJson(Map<String, dynamic> json) =>
      _$ForecastPointModelFromJson(json);
}
