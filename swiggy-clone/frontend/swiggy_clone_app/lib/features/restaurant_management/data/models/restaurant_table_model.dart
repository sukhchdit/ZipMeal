import 'package:freezed_annotation/freezed_annotation.dart';

part 'restaurant_table_model.freezed.dart';
part 'restaurant_table_model.g.dart';

@freezed
class RestaurantTableModel with _$RestaurantTableModel {
  const factory RestaurantTableModel({
    required String id,
    required String tableNumber,
    required int capacity,
    String? floorSection,
    required String qrCodeData,
    required int status,
    required bool isActive,
    @Default(0) int activeSessionCount,
    required String createdAt,
    required String updatedAt,
  }) = _RestaurantTableModel;

  factory RestaurantTableModel.fromJson(Map<String, dynamic> json) =>
      _$RestaurantTableModelFromJson(json);
}
