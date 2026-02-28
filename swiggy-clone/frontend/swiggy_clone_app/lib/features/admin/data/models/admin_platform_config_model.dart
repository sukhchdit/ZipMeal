import 'package:freezed_annotation/freezed_annotation.dart';

part 'admin_platform_config_model.freezed.dart';
part 'admin_platform_config_model.g.dart';

@freezed
class AdminPlatformConfigModel with _$AdminPlatformConfigModel {
  const factory AdminPlatformConfigModel({
    required String id,
    required int deliveryFeePaise,
    required int packagingChargePaise,
    required double taxRatePercent,
    int? freeDeliveryThresholdPaise,
    required DateTime updatedAt,
  }) = _AdminPlatformConfigModel;

  factory AdminPlatformConfigModel.fromJson(Map<String, dynamic> json) =>
      _$AdminPlatformConfigModelFromJson(json);
}
