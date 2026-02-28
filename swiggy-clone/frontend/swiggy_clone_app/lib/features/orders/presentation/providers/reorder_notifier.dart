import 'dart:async';

import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../cart/presentation/providers/cart_notifier.dart';
import '../../data/repositories/order_repository.dart';
import 'reorder_state.dart';

part 'reorder_notifier.g.dart';

@riverpod
class ReorderNotifier extends _$ReorderNotifier {
  late OrderRepository _repository;

  @override
  ReorderState build() {
    _repository = ref.watch(orderRepositoryProvider);
    return const ReorderState.idle();
  }

  Future<void> reorder(String orderId) async {
    state = const ReorderState.loading();
    final result = await _repository.reorder(orderId: orderId);
    if (result.failure != null) {
      state = ReorderState.error(failure: result.failure!);
    } else {
      // Refresh cart state (fire-and-forget)
      unawaited(ref.read(cartNotifierProvider.notifier).loadCart());
      state = ReorderState.success(result: result.data!);
    }
  }
}
