import 'package:freezed_annotation/freezed_annotation.dart';

part 'delivery_tracking_model.freezed.dart';
part 'delivery_tracking_model.g.dart';

@freezed
class DeliveryTrackingModel with _$DeliveryTrackingModel {
  const factory DeliveryTrackingModel({
    required String orderId,
    required int deliveryStatus,
    String? partnerName,
    String? partnerPhone,
    double? currentLatitude,
    double? currentLongitude,
    String? assignedAt,
    String? acceptedAt,
    String? pickedUpAt,
    String? estimatedDeliveryTime,
  }) = _DeliveryTrackingModel;

  factory DeliveryTrackingModel.fromJson(Map<String, dynamic> json) =>
      _$DeliveryTrackingModelFromJson(json);
}
