import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/group_order_repository.dart';
import 'group_order_state.dart';

part 'group_order_notifier.g.dart';

@riverpod
class GroupOrderNotifier extends _$GroupOrderNotifier {
  late GroupOrderRepository _repository;

  @override
  GroupOrderState build(String groupOrderId) {
    _repository = ref.watch(groupOrderRepositoryProvider);
    loadGroupOrder();
    return const GroupOrderState.initial();
  }

  Future<void> loadGroupOrder() async {
    state = const GroupOrderState.loading();
    final result = await _repository.getGroupOrderDetail(
      groupOrderId: groupOrderId,
    );
    if (result.failure != null) {
      state = GroupOrderState.error(failure: result.failure!);
      return;
    }
    final groupOrder = result.data!;

    // Check status
    if (groupOrder.status == 2) {
      state = GroupOrderState.finalized(orderId: groupOrder.orderId ?? '');
      return;
    }
    if (groupOrder.status == 3) {
      state = const GroupOrderState.expired();
      return;
    }
    if (groupOrder.status == 4) {
      state = const GroupOrderState.cancelled();
      return;
    }

    // Determine if current user is the initiator
    final isInitiator = groupOrder.participants.any(
      (p) => p.isInitiator && p.userId == groupOrder.initiatorUserId,
    );

    state = GroupOrderState.loaded(
      groupOrder: groupOrder,
      isInitiator: isInitiator,
    );
  }

  Future<bool> markReady() async {
    final failure = await _repository.markReady(groupOrderId: groupOrderId);
    if (failure != null) return false;
    await loadGroupOrder();
    return true;
  }

  Future<bool> leaveGroupOrder() async {
    final failure = await _repository.leaveGroupOrder(
      groupOrderId: groupOrderId,
    );
    if (failure != null) return false;
    return true;
  }

  Future<bool> cancelGroupOrder() async {
    final failure = await _repository.cancelGroupOrder(
      groupOrderId: groupOrderId,
    );
    if (failure != null) return false;
    state = const GroupOrderState.cancelled();
    return true;
  }

  Future<Map<String, dynamic>?> finalizeGroupOrder({
    required String deliveryAddressId,
    required int paymentMethod,
    String? couponCode,
    String? specialInstructions,
  }) async {
    final result = await _repository.finalizeGroupOrder(
      groupOrderId: groupOrderId,
      deliveryAddressId: deliveryAddressId,
      paymentMethod: paymentMethod,
      couponCode: couponCode,
      specialInstructions: specialInstructions,
    );
    if (result.failure != null) return null;
    final orderId = result.data?['id'] as String? ?? '';
    state = GroupOrderState.finalized(orderId: orderId);
    return result.data;
  }
}
