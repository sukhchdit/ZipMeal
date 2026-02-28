import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/admin_dashboard_model.dart';

part 'admin_orders_state.freezed.dart';

@freezed
sealed class AdminOrdersState with _$AdminOrdersState {
  const factory AdminOrdersState.initial() = AdminOrdersInitial;
  const factory AdminOrdersState.loading() = AdminOrdersLoading;
  const factory AdminOrdersState.loaded({
    required List<AdminOrderSummaryModel> orders,
    required int totalCount,
    required int page,
    required int totalPages,
    @Default(false) bool isLoadingMore,
  }) = AdminOrdersLoaded;
  const factory AdminOrdersState.error({required Failure failure}) =
      AdminOrdersError;
}
