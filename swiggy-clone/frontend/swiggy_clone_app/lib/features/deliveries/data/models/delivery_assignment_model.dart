import 'package:freezed_annotation/freezed_annotation.dart';

part 'delivery_assignment_model.freezed.dart';
part 'delivery_assignment_model.g.dart';

@freezed
class DeliveryAssignmentModel with _$DeliveryAssignmentModel {
  const factory DeliveryAssignmentModel({
    required String id,
    required String orderId,
    required String orderNumber,
    required String restaurantName,
    String? restaurantAddress,
    String? customerAddress,
    required int status,
    required String assignedAt,
    String? acceptedAt,
    String? pickedUpAt,
    String? deliveredAt,
    double? distanceKm,
    required int earnings,
  }) = _DeliveryAssignmentModel;

  factory DeliveryAssignmentModel.fromJson(Map<String, dynamic> json) =>
      _$DeliveryAssignmentModelFromJson(json);
}
