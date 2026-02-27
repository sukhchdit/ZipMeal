import 'package:freezed_annotation/freezed_annotation.dart';

part 'fee_config_model.freezed.dart';
part 'fee_config_model.g.dart';

@freezed
class FeeConfigModel with _$FeeConfigModel {
  const factory FeeConfigModel({
    required int deliveryFeePaise,
    required int packagingChargePaise,
    required double taxRatePercent,
    int? freeDeliveryThresholdPaise,
  }) = _FeeConfigModel;

  factory FeeConfigModel.fromJson(Map<String, dynamic> json) =>
      _$FeeConfigModelFromJson(json);
}
