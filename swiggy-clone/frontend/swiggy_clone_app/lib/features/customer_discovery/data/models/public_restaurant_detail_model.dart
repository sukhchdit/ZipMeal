import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../restaurant_management/data/models/menu_item_model.dart';
import '../../../restaurant_management/data/models/operating_hours_model.dart';

part 'public_restaurant_detail_model.freezed.dart';
part 'public_restaurant_detail_model.g.dart';

@freezed
class PublicRestaurantDetailModel with _$PublicRestaurantDetailModel {
  const factory PublicRestaurantDetailModel({
    required String id,
    required String name,
    required String slug,
    String? description,
    String? logoUrl,
    String? bannerUrl,
    String? addressLine1,
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
    @Default([]) List<String> cuisines,
    @Default([]) List<OperatingHoursModel> operatingHours,
    @Default([]) List<MenuSectionModel> menuSections,
  }) = _PublicRestaurantDetailModel;

  factory PublicRestaurantDetailModel.fromJson(Map<String, dynamic> json) =>
      _$PublicRestaurantDetailModelFromJson(json);
}

@freezed
class MenuSectionModel with _$MenuSectionModel {
  const factory MenuSectionModel({
    required String categoryId,
    required String categoryName,
    required int sortOrder,
    @Default([]) List<MenuItemModel> items,
  }) = _MenuSectionModel;

  factory MenuSectionModel.fromJson(Map<String, dynamic> json) =>
      _$MenuSectionModelFromJson(json);
}
