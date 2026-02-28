import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/delivery_repository.dart';
import 'my_deliveries_state.dart';

part 'my_deliveries_notifier.g.dart';

@riverpod
class MyDeliveriesNotifier extends _$MyDeliveriesNotifier {
  late DeliveryRepository _repository;

  @override
  MyDeliveriesState build() {
    _repository = ref.watch(deliveryRepositoryProvider);
    loadDeliveries();
    return const MyDeliveriesState.initial();
  }

  Future<void> loadDeliveries() async {
    state = const MyDeliveriesState.loading();
    final result = await _repository.getMyDeliveries();
    if (result.failure != null) {
      state = MyDeliveriesState.error(failure: result.failure!);
    } else if (result.items!.isEmpty) {
      state = const MyDeliveriesState.loaded(
        deliveries: [],
        hasMore: false,
        nextCursor: null,
      );
    } else {
      state = MyDeliveriesState.loaded(
        deliveries: result.items!,
        hasMore: result.hasMore ?? false,
        nextCursor: result.nextCursor,
      );
    }
  }

  Future<void> loadMore() async {
    final current = state;
    if (current is! MyDeliveriesLoaded ||
        !current.hasMore ||
        current.isLoadingMore) {
      return;
    }

    state = current.copyWith(isLoadingMore: true);
    final result =
        await _repository.getMyDeliveries(cursor: current.nextCursor);

    if (result.failure != null) {
      state = current.copyWith(isLoadingMore: false);
      return;
    }

    state = MyDeliveriesLoaded(
      deliveries: [...current.deliveries, ...result.items!],
      hasMore: result.hasMore ?? false,
      nextCursor: result.nextCursor,
    );
  }
}
