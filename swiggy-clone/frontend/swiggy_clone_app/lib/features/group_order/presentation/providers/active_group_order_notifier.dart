import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/group_order_model.dart';
import '../../data/repositories/group_order_repository.dart';

part 'active_group_order_notifier.freezed.dart';
part 'active_group_order_notifier.g.dart';

@freezed
sealed class ActiveGroupOrderState with _$ActiveGroupOrderState {
  const factory ActiveGroupOrderState.initial() = ActiveGroupOrderInitial;
  const factory ActiveGroupOrderState.loading() = ActiveGroupOrderLoading;
  const factory ActiveGroupOrderState.active({
    required GroupOrderModel groupOrder,
  }) = ActiveGroupOrderActive;
  const factory ActiveGroupOrderState.none() = ActiveGroupOrderNone;
  const factory ActiveGroupOrderState.error({required Failure failure}) =
      ActiveGroupOrderError;
}

@Riverpod(keepAlive: true)
class ActiveGroupOrderNotifier extends _$ActiveGroupOrderNotifier {
  late GroupOrderRepository _repository;

  @override
  ActiveGroupOrderState build() {
    _repository = ref.watch(groupOrderRepositoryProvider);
    checkActiveGroupOrder();
    return const ActiveGroupOrderState.initial();
  }

  Future<void> checkActiveGroupOrder() async {
    state = const ActiveGroupOrderState.loading();
    final result = await _repository.getActiveGroupOrder();
    if (result.failure != null) {
      state = ActiveGroupOrderState.error(failure: result.failure!);
      return;
    }
    if (result.data == null) {
      state = const ActiveGroupOrderState.none();
      return;
    }
    state = ActiveGroupOrderState.active(groupOrder: result.data!);
  }
}
