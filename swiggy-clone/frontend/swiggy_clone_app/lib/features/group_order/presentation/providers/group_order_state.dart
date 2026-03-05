import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/group_order_model.dart';

part 'group_order_state.freezed.dart';

@freezed
sealed class GroupOrderState with _$GroupOrderState {
  const factory GroupOrderState.initial() = GroupOrderInitial;
  const factory GroupOrderState.loading() = GroupOrderLoading;
  const factory GroupOrderState.loaded({
    required GroupOrderModel groupOrder,
    required bool isInitiator,
  }) = GroupOrderLoaded;
  const factory GroupOrderState.finalized({required String orderId}) =
      GroupOrderFinalized;
  const factory GroupOrderState.cancelled() = GroupOrderCancelled;
  const factory GroupOrderState.expired() = GroupOrderExpired;
  const factory GroupOrderState.error({required Failure failure}) =
      GroupOrderError;
}
