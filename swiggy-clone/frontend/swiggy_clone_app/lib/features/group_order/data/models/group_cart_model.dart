import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../cart/data/models/cart_model.dart';

part 'group_cart_model.freezed.dart';
part 'group_cart_model.g.dart';

@freezed
class GroupCartModel with _$GroupCartModel {
  const factory GroupCartModel({
    required String groupOrderId,
    @Default([]) List<GroupParticipantCartModel> participantCarts,
    @Default(0) int grandTotal,
  }) = _GroupCartModel;

  factory GroupCartModel.fromJson(Map<String, dynamic> json) =>
      _$GroupCartModelFromJson(json);
}

@freezed
class GroupParticipantCartModel with _$GroupParticipantCartModel {
  const factory GroupParticipantCartModel({
    required String userId,
    required String userName,
    @Default([]) List<CartItemModel> items,
    @Default(0) int subtotal,
  }) = _GroupParticipantCartModel;

  factory GroupParticipantCartModel.fromJson(Map<String, dynamic> json) =>
      _$GroupParticipantCartModelFromJson(json);
}
