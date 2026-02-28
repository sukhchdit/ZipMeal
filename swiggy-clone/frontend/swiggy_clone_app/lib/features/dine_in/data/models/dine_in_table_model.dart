import 'package:freezed_annotation/freezed_annotation.dart';

part 'dine_in_table_model.freezed.dart';
part 'dine_in_table_model.g.dart';

@freezed
class DineInTableModel with _$DineInTableModel {
  const factory DineInTableModel({
    required String id,
    required String tableNumber,
    required int capacity,
    String? floorSection,
  }) = _DineInTableModel;

  factory DineInTableModel.fromJson(Map<String, dynamic> json) =>
      _$DineInTableModelFromJson(json);
}
