import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/admin_repository.dart';
import 'admin_orders_state.dart';

part 'admin_orders_notifier.g.dart';

@riverpod
class AdminOrdersNotifier extends _$AdminOrdersNotifier {
  late AdminRepository _repository;

  @override
  AdminOrdersState build() {
    _repository = ref.watch(adminRepositoryProvider);
    loadOrders();
    return const AdminOrdersState.initial();
  }

  Future<void> loadOrders({
    int? statusFilter,
    DateTime? fromDate,
    DateTime? toDate,
    int page = 1,
  }) async {
    state = const AdminOrdersState.loading();
    final result = await _repository.getOrders(
      status: statusFilter,
      fromDate: fromDate,
      toDate: toDate,
      page: page,
    );
    if (result.failure != null) {
      state = AdminOrdersState.error(failure: result.failure!);
    } else {
      state = AdminOrdersState.loaded(
        orders: result.items!,
        totalCount: result.totalCount ?? 0,
        page: result.page ?? 1,
        totalPages: result.totalPages ?? 1,
      );
    }
  }
}
