import 'package:freezed_annotation/freezed_annotation.dart';

part 'trending_item_model.freezed.dart';
part 'trending_item_model.g.dart';

@freezed
class TrendingItemModel with _$TrendingItemModel {
  const factory TrendingItemModel({
    required String id,
    required String name,
    String? imageUrl,
    required int price,
    required bool isVeg,
    required bool isBestseller,
    required String restaurantId,
    required String restaurantName,
    required int orderCount,
    required int trendRank,
  }) = _TrendingItemModel;

  factory TrendingItemModel.fromJson(Map<String, dynamic> json) =>
      _$TrendingItemModelFromJson(json);
}
