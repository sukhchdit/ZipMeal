import 'package:freezed_annotation/freezed_annotation.dart';

part 'customer_restaurant_model.freezed.dart';
part 'customer_restaurant_model.g.dart';

@freezed
class CustomerRestaurantModel with _$CustomerRestaurantModel {
  const factory CustomerRestaurantModel({
    required String id,
    required String name,
    required String slug,
    String? logoUrl,
    String? bannerUrl,
    String? city,
    required double averageRating,
    required int totalRatings,
    int? avgDeliveryTimeMin,
    int? avgCostForTwo,
    required bool isVegOnly,
    required bool isAcceptingOrders,
    required bool isDineInEnabled,
    @Default([]) List<String> cuisines,
  }) = _CustomerRestaurantModel;

  factory CustomerRestaurantModel.fromJson(Map<String, dynamic> json) =>
      _$CustomerRestaurantModelFromJson(json);
}
