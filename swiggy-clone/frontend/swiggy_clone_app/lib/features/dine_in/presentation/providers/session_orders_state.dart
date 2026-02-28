import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../../orders/data/models/order_model.dart';

part 'session_orders_state.freezed.dart';

@freezed
sealed class SessionOrdersState with _$SessionOrdersState {
  const factory SessionOrdersState.initial() = SessionOrdersInitial;
  const factory SessionOrdersState.loading() = SessionOrdersLoading;
  const factory SessionOrdersState.loaded({
    required List<OrderModel> orders,
  }) = SessionOrdersLoaded;
  const factory SessionOrdersState.error({required Failure failure}) =
      SessionOrdersError;
}
