import 'package:freezed_annotation/freezed_annotation.dart';

part 'reorder_result_model.freezed.dart';
part 'reorder_result_model.g.dart';

@freezed
class ReorderResultModel with _$ReorderResultModel {
  const factory ReorderResultModel({
    required Map<String, dynamic> cart,
    @Default([]) List<UnavailableItemModel> unavailableItems,
  }) = _ReorderResultModel;

  factory ReorderResultModel.fromJson(Map<String, dynamic> json) =>
      _$ReorderResultModelFromJson(json);
}

@freezed
class UnavailableItemModel with _$UnavailableItemModel {
  const factory UnavailableItemModel({
    required String menuItemId,
    required String itemName,
    required String reason,
  }) = _UnavailableItemModel;

  factory UnavailableItemModel.fromJson(Map<String, dynamic> json) =>
      _$UnavailableItemModelFromJson(json);
}
