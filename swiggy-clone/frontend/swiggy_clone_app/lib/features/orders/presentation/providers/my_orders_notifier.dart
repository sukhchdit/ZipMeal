import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/order_repository.dart';
import 'my_orders_state.dart';

part 'my_orders_notifier.g.dart';

@riverpod
class MyOrdersNotifier extends _$MyOrdersNotifier {
  late OrderRepository _repository;

  @override
  MyOrdersState build() {
    _repository = ref.watch(orderRepositoryProvider);
    loadOrders();
    return const MyOrdersState.initial();
  }

  Future<void> loadOrders() async {
    state = const MyOrdersState.loading();
    final result = await _repository.getMyOrders();
    if (result.failure != null) {
      state = MyOrdersState.error(failure: result.failure!);
    } else if (result.items!.isEmpty) {
      state = const MyOrdersState.loaded(
        orders: [],
        hasMore: false,
        nextCursor: null,
      );
    } else {
      state = MyOrdersState.loaded(
        orders: result.items!,
        hasMore: result.hasMore ?? false,
        nextCursor: result.nextCursor,
      );
    }
  }

  Future<void> loadMore() async {
    final current = state;
    if (current is! MyOrdersLoaded || !current.hasMore || current.isLoadingMore) {
      return;
    }

    state = current.copyWith(isLoadingMore: true);
    final result = await _repository.getMyOrders(cursor: current.nextCursor);

    if (result.failure != null) {
      state = current.copyWith(isLoadingMore: false);
      return;
    }

    state = MyOrdersLoaded(
      orders: [...current.orders, ...result.items!],
      hasMore: result.hasMore ?? false,
      nextCursor: result.nextCursor,
    );
  }
}
