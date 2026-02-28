import 'package:freezed_annotation/freezed_annotation.dart';

import 'dine_in_member_model.dart';
import 'dine_in_order_summary_model.dart';
import 'dine_in_table_model.dart';

part 'dine_in_session_model.freezed.dart';
part 'dine_in_session_model.g.dart';

@freezed
class DineInSessionModel with _$DineInSessionModel {
  const factory DineInSessionModel({
    required String id,
    required String restaurantId,
    required String restaurantName,
    String? restaurantLogoUrl,
    required DineInTableModel table,
    required String sessionCode,
    required int status,
    required int guestCount,
    required String startedAt,
    String? endedAt,
    @Default([]) List<DineInMemberModel> members,
    @Default([]) List<DineInOrderSummaryModel> orders,
  }) = _DineInSessionModel;

  factory DineInSessionModel.fromJson(Map<String, dynamic> json) =>
      _$DineInSessionModelFromJson(json);
}

@freezed
class DineInSessionSummaryModel with _$DineInSessionSummaryModel {
  const factory DineInSessionSummaryModel({
    required String id,
    required String restaurantId,
    required String restaurantName,
    String? restaurantLogoUrl,
    required String tableNumber,
    required String sessionCode,
    required int status,
    required int guestCount,
    required String startedAt,
  }) = _DineInSessionSummaryModel;

  factory DineInSessionSummaryModel.fromJson(Map<String, dynamic> json) =>
      _$DineInSessionSummaryModelFromJson(json);
}
