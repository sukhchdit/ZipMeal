import 'package:freezed_annotation/freezed_annotation.dart';

part 'experiment_stats_model.freezed.dart';
part 'experiment_stats_model.g.dart';

@freezed
class ExperimentStatsModel with _$ExperimentStatsModel {
  const factory ExperimentStatsModel({
    required String experimentId,
    required List<VariantStatsModel> variants,
    required String computedAt,
  }) = _ExperimentStatsModel;

  factory ExperimentStatsModel.fromJson(Map<String, dynamic> json) =>
      _$ExperimentStatsModelFromJson(json);
}

@freezed
class VariantStatsModel with _$VariantStatsModel {
  const factory VariantStatsModel({
    required String variantId,
    required String variantKey,
    required String variantName,
    required bool isControl,
    required int exposures,
    required int conversions,
    required double conversionRate,
    double? relativeLift,
    double? zScore,
    double? pValue,
    bool? isSignificant,
  }) = _VariantStatsModel;

  factory VariantStatsModel.fromJson(Map<String, dynamic> json) =>
      _$VariantStatsModelFromJson(json);
}
