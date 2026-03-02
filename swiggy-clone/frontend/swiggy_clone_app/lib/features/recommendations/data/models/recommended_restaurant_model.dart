import 'package:freezed_annotation/freezed_annotation.dart';

part 'recommended_restaurant_model.freezed.dart';
part 'recommended_restaurant_model.g.dart';

@freezed
class RecommendedRestaurantModel with _$RecommendedRestaurantModel {
  const factory RecommendedRestaurantModel({
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
    String? recommendationReason,
    required double score,
  }) = _RecommendedRestaurantModel;

  factory RecommendedRestaurantModel.fromJson(Map<String, dynamic> json) =>
      _$RecommendedRestaurantModelFromJson(json);
}
