import 'package:freezed_annotation/freezed_annotation.dart';

import 'cuisine_type_model.dart';

part 'restaurant_model.freezed.dart';
part 'restaurant_model.g.dart';

@freezed
class RestaurantModel with _$RestaurantModel {
  const factory RestaurantModel({
    required String id,
    required String name,
    required String slug,
    String? description,
    String? phoneNumber,
    String? email,
    String? logoUrl,
    String? bannerUrl,
    String? addressLine1,
    String? addressLine2,
    String? city,
    String? state,
    String? postalCode,
    double? latitude,
    double? longitude,
    required double averageRating,
    required int totalRatings,
    int? avgDeliveryTimeMin,
    int? avgCostForTwo,
    required bool isVegOnly,
    required bool isAcceptingOrders,
    required bool isDineInEnabled,
    String? fssaiLicense,
    String? gstNumber,
    required String status,
    required DateTime createdAt,
    @Default([]) List<CuisineTypeModel> cuisines,
  }) = _RestaurantModel;

  factory RestaurantModel.fromJson(Map<String, dynamic> json) =>
      _$RestaurantModelFromJson(json);
}
