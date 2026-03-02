import 'package:freezed_annotation/freezed_annotation.dart';

import 'recommended_menu_item_model.dart';
import 'recommended_restaurant_model.dart';

part 'personalized_recommendations_model.freezed.dart';
part 'personalized_recommendations_model.g.dart';

@freezed
class PersonalizedRecommendationsModel
    with _$PersonalizedRecommendationsModel {
  const factory PersonalizedRecommendationsModel({
    @Default([]) List<RecommendedRestaurantModel> recommendedRestaurants,
    @Default([]) List<RecommendedMenuItemModel> recommendedDishes,
  }) = _PersonalizedRecommendationsModel;

  factory PersonalizedRecommendationsModel.fromJson(
          Map<String, dynamic> json) =>
      _$PersonalizedRecommendationsModelFromJson(json);
}
