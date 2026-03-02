import 'package:freezed_annotation/freezed_annotation.dart';

part 'funnel_stage_model.freezed.dart';
part 'funnel_stage_model.g.dart';

@freezed
class FunnelStageModel with _$FunnelStageModel {
  const factory FunnelStageModel({
    required String stage,
    required int count,
    required double conversionRate,
  }) = _FunnelStageModel;

  factory FunnelStageModel.fromJson(Map<String, dynamic> json) =>
      _$FunnelStageModelFromJson(json);
}
