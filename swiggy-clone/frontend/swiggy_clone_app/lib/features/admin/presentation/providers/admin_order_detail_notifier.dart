import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/admin_order_detail_model.dart';
import '../../data/repositories/admin_repository.dart';

part 'admin_order_detail_notifier.freezed.dart';
part 'admin_order_detail_notifier.g.dart';

@freezed
sealed class AdminOrderDetailState with _$AdminOrderDetailState {
  const factory AdminOrderDetailState.initial() = AdminOrderDetailInitial;
  const factory AdminOrderDetailState.loading() = AdminOrderDetailLoading;
  const factory AdminOrderDetailState.loaded({
    required AdminOrderDetailModel order,
  }) = AdminOrderDetailLoaded;
  const factory AdminOrderDetailState.error({required Failure failure}) =
      AdminOrderDetailError;
}

@riverpod
class AdminOrderDetailNotifier extends _$AdminOrderDetailNotifier {
  late AdminRepository _repository;

  @override
  AdminOrderDetailState build(String orderId) {
    _repository = ref.watch(adminRepositoryProvider);
    loadDetail();
    return const AdminOrderDetailState.initial();
  }

  Future<void> loadDetail() async {
    state = const AdminOrderDetailState.loading();
    final result = await _repository.getOrderDetail(orderId);
    if (result.failure != null) {
      state = AdminOrderDetailState.error(failure: result.failure!);
    } else {
      state = AdminOrderDetailState.loaded(order: result.data!);
    }
  }
}
