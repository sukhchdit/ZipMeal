import 'package:freezed_annotation/freezed_annotation.dart';

part 'dine_in_order_summary_model.freezed.dart';
part 'dine_in_order_summary_model.g.dart';

@freezed
class DineInOrderSummaryModel with _$DineInOrderSummaryModel {
  const factory DineInOrderSummaryModel({
    required String id,
    required String orderNumber,
    required String placedByUserId,
    required String placedByName,
    required int status,
    required int totalAmount,
    required int itemCount,
    required String createdAt,
  }) = _DineInOrderSummaryModel;

  factory DineInOrderSummaryModel.fromJson(Map<String, dynamic> json) =>
      _$DineInOrderSummaryModelFromJson(json);
}
