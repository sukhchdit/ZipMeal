import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/owner_dine_in_order_model.dart';

part 'dine_in_orders_state.freezed.dart';

@freezed
sealed class DineInOrdersState with _$DineInOrdersState {
  const factory DineInOrdersState.initial() = DineInOrdersInitial;
  const factory DineInOrdersState.loading() = DineInOrdersLoading;
  const factory DineInOrdersState.loaded({
    required List<OwnerDineInOrderModel> orders,
  }) = DineInOrdersLoaded;
  const factory DineInOrdersState.error({required Failure failure}) =
      DineInOrdersError;
}
