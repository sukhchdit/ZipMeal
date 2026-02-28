import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/order_model.dart';

part 'order_detail_state.freezed.dart';

@freezed
sealed class OrderDetailState with _$OrderDetailState {
  const factory OrderDetailState.initial() = OrderDetailInitial;
  const factory OrderDetailState.loading() = OrderDetailLoading;
  const factory OrderDetailState.loaded({required OrderModel order}) =
      OrderDetailLoaded;
  const factory OrderDetailState.error({required Failure failure}) =
      OrderDetailError;
}
