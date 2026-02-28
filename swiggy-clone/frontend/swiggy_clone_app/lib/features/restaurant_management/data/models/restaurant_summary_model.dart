import 'package:freezed_annotation/freezed_annotation.dart';

part 'restaurant_summary_model.freezed.dart';
part 'restaurant_summary_model.g.dart';

@freezed
class RestaurantSummaryModel with _$RestaurantSummaryModel {
  const factory RestaurantSummaryModel({
    required String id,
    required String name,
    required String slug,
    String? logoUrl,
    String? city,
    required double averageRating,
    required int totalRatings,
    required bool isAcceptingOrders,
    required String status,
  }) = _RestaurantSummaryModel;

  factory RestaurantSummaryModel.fromJson(Map<String, dynamic> json) =>
      _$RestaurantSummaryModelFromJson(json);
}
