import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/order_model.dart';

part 'place_order_state.freezed.dart';

@freezed
sealed class PlaceOrderState with _$PlaceOrderState {
  const factory PlaceOrderState.initial() = PlaceOrderInitial;
  const factory PlaceOrderState.placing() = PlaceOrderPlacing;
  const factory PlaceOrderState.placed({required OrderModel order}) =
      PlaceOrderPlaced;
  const factory PlaceOrderState.error({required Failure failure}) =
      PlaceOrderError;
}
