import 'package:freezed_annotation/freezed_annotation.dart';

part 'admin_restaurant_model.freezed.dart';
part 'admin_restaurant_model.g.dart';

@freezed
class AdminRestaurantModel with _$AdminRestaurantModel {
  const factory AdminRestaurantModel({
    required String id,
    required String name,
    required String slug,
    String? description,
    String? city,
    String? state,
    required String ownerName,
    required String ownerPhone,
    String? logoUrl,
    required int status,
    String? statusReason,
    required double averageRating,
    required int totalRatings,
    required bool isAcceptingOrders,
    String? fssaiLicense,
    String? gstNumber,
    required DateTime createdAt,
  }) = _AdminRestaurantModel;

  factory AdminRestaurantModel.fromJson(Map<String, dynamic> json) =>
      _$AdminRestaurantModelFromJson(json);
}
