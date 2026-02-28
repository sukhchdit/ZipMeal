import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/order_summary_model.dart';

part 'my_orders_state.freezed.dart';

@freezed
sealed class MyOrdersState with _$MyOrdersState {
  const factory MyOrdersState.initial() = MyOrdersInitial;
  const factory MyOrdersState.loading() = MyOrdersLoading;
  const factory MyOrdersState.loaded({
    required List<OrderSummaryModel> orders,
    required bool hasMore,
    required String? nextCursor,
    @Default(false) bool isLoadingMore,
  }) = MyOrdersLoaded;
  const factory MyOrdersState.error({required Failure failure}) = MyOrdersError;
}
