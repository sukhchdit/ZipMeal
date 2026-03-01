import 'package:freezed_annotation/freezed_annotation.dart';

part 'promotion_model.freezed.dart';
part 'promotion_model.g.dart';

@freezed
class PromotionModel with _$PromotionModel {
  const factory PromotionModel({
    required String id,
    required String restaurantId,
    required String title,
    String? description,
    String? imageUrl,
    required int promotionType,
    required int discountType,
    required int discountValue,
    int? maxDiscount,
    int? minOrderAmount,
    required String validFrom,
    required String validUntil,
    required bool isActive,
    required int displayOrder,
    String? recurringStartTime,
    String? recurringEndTime,
    @Default([]) List<int> recurringDaysOfWeek,
    int? comboPrice,
    @Default([]) List<PromotionMenuItemModel> menuItems,
    String? createdAt,
    String? updatedAt,
  }) = _PromotionModel;

  factory PromotionModel.fromJson(Map<String, dynamic> json) =>
      _$PromotionModelFromJson(json);
}

@freezed
class PromotionMenuItemModel with _$PromotionMenuItemModel {
  const factory PromotionMenuItemModel({
    required String menuItemId,
    required String menuItemName,
    required int price,
    int? discountedPrice,
    @Default(1) int quantity,
  }) = _PromotionMenuItemModel;

  factory PromotionMenuItemModel.fromJson(Map<String, dynamic> json) =>
      _$PromotionMenuItemModelFromJson(json);
}

@freezed
class PromotionListResponse with _$PromotionListResponse {
  const factory PromotionListResponse({
    required List<PromotionModel> items,
    required int totalCount,
    required int page,
    required int pageSize,
    required int totalPages,
    required bool hasPreviousPage,
    required bool hasNextPage,
  }) = _PromotionListResponse;

  factory PromotionListResponse.fromJson(Map<String, dynamic> json) =>
      _$PromotionListResponseFromJson(json);
}
