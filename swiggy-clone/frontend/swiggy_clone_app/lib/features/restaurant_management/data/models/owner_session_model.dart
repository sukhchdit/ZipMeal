import 'package:freezed_annotation/freezed_annotation.dart';

part 'owner_session_model.freezed.dart';
part 'owner_session_model.g.dart';

@freezed
class OwnerSessionModel with _$OwnerSessionModel {
  const factory OwnerSessionModel({
    required String id,
    required String tableNumber,
    String? floorSection,
    required String sessionCode,
    required int status,
    required int guestCount,
    required int memberCount,
    required int orderCount,
    required int totalAmount,
    required String startedAt,
    String? endedAt,
  }) = _OwnerSessionModel;

  factory OwnerSessionModel.fromJson(Map<String, dynamic> json) =>
      _$OwnerSessionModelFromJson(json);
}
