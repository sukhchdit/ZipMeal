import 'package:freezed_annotation/freezed_annotation.dart';

import 'group_order_participant_model.dart';

part 'group_order_model.freezed.dart';
part 'group_order_model.g.dart';

@freezed
class GroupOrderModel with _$GroupOrderModel {
  const factory GroupOrderModel({
    required String id,
    required String restaurantId,
    required String restaurantName,
    String? restaurantLogoUrl,
    required String initiatorUserId,
    required String initiatorName,
    required String inviteCode,
    required int status,
    required int paymentSplitType,
    String? deliveryAddressId,
    String? specialInstructions,
    required String expiresAt,
    String? finalizedAt,
    String? orderId,
    required String createdAt,
    @Default([]) List<GroupOrderParticipantModel> participants,
  }) = _GroupOrderModel;

  factory GroupOrderModel.fromJson(Map<String, dynamic> json) =>
      _$GroupOrderModelFromJson(json);
}
