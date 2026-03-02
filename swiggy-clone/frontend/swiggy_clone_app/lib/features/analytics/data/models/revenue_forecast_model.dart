import 'package:freezed_annotation/freezed_annotation.dart';

import 'data_point_model.dart';
import 'forecast_point_model.dart';

part 'revenue_forecast_model.freezed.dart';
part 'revenue_forecast_model.g.dart';

@freezed
class RevenueForecastModel with _$RevenueForecastModel {
  const factory RevenueForecastModel({
    @Default([]) List<DataPointModel> historicalData,
    @Default([]) List<ForecastPointModel> forecastData,
    required double projectedTotalRevenue,
    required double avgDailyProjected,
    required double growthRate,
  }) = _RevenueForecastModel;

  factory RevenueForecastModel.fromJson(Map<String, dynamic> json) =>
      _$RevenueForecastModelFromJson(json);
}
