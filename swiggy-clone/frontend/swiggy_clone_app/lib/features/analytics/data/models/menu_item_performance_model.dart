import 'package:freezed_annotation/freezed_annotation.dart';

part 'menu_item_performance_model.freezed.dart';
part 'menu_item_performance_model.g.dart';

@freezed
class MenuItemPerformanceModel with _$MenuItemPerformanceModel {
  const factory MenuItemPerformanceModel({
    required String itemName,
    required int totalQuantitySold,
    required double totalRevenue,
    required int orderCount,
    required double avgRating,
  }) = _MenuItemPerformanceModel;

  factory MenuItemPerformanceModel.fromJson(Map<String, dynamic> json) =>
      _$MenuItemPerformanceModelFromJson(json);
}
