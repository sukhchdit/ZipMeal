import 'package:freezed_annotation/freezed_annotation.dart';

import 'customer_restaurant_model.dart';

part 'home_feed_model.freezed.dart';
part 'home_feed_model.g.dart';

@freezed
class HomeFeedModel with _$HomeFeedModel {
  const factory HomeFeedModel({
    @Default([]) List<BannerModel> banners,
    @Default([]) List<CuisineChipModel> cuisineChips,
    @Default([]) List<RestaurantSectionModel> sections,
  }) = _HomeFeedModel;

  factory HomeFeedModel.fromJson(Map<String, dynamic> json) =>
      _$HomeFeedModelFromJson(json);
}

@freezed
class BannerModel with _$BannerModel {
  const factory BannerModel({
    required String id,
    required String imageUrl,
    String? deepLink,
  }) = _BannerModel;

  factory BannerModel.fromJson(Map<String, dynamic> json) =>
      _$BannerModelFromJson(json);
}

@freezed
class CuisineChipModel with _$CuisineChipModel {
  const factory CuisineChipModel({
    required String id,
    required String name,
    String? iconUrl,
  }) = _CuisineChipModel;

  factory CuisineChipModel.fromJson(Map<String, dynamic> json) =>
      _$CuisineChipModelFromJson(json);
}

@freezed
class RestaurantSectionModel with _$RestaurantSectionModel {
  const factory RestaurantSectionModel({
    required String title,
    @Default([]) List<CustomerRestaurantModel> restaurants,
  }) = _RestaurantSectionModel;

  factory RestaurantSectionModel.fromJson(Map<String, dynamic> json) =>
      _$RestaurantSectionModelFromJson(json);
}
