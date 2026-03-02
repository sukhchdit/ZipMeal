import 'package:freezed_annotation/freezed_annotation.dart';

import 'data_point_model.dart';
import 'funnel_stage_model.dart';

part 'customer_funnel_model.freezed.dart';
part 'customer_funnel_model.g.dart';

@freezed
class CustomerFunnelModel with _$CustomerFunnelModel {
  const factory CustomerFunnelModel({
    @Default([]) List<FunnelStageModel> stages,
    @Default([]) List<DataPointModel> activeUserTrend,
  }) = _CustomerFunnelModel;

  factory CustomerFunnelModel.fromJson(Map<String, dynamic> json) =>
      _$CustomerFunnelModelFromJson(json);
}
