import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/admin_user_model.dart';
import '../../data/repositories/admin_repository.dart';

part 'admin_user_detail_notifier.freezed.dart';
part 'admin_user_detail_notifier.g.dart';

@freezed
sealed class AdminUserDetailState with _$AdminUserDetailState {
  const factory AdminUserDetailState.initial() = AdminUserDetailInitial;
  const factory AdminUserDetailState.loading() = AdminUserDetailLoading;
  const factory AdminUserDetailState.loaded({
    required AdminUserModel user,
  }) = AdminUserDetailLoaded;
  const factory AdminUserDetailState.error({required Failure failure}) =
      AdminUserDetailError;
}

@riverpod
class AdminUserDetailNotifier extends _$AdminUserDetailNotifier {
  late AdminRepository _repository;

  @override
  AdminUserDetailState build(String userId) {
    _repository = ref.watch(adminRepositoryProvider);
    loadDetail();
    return const AdminUserDetailState.initial();
  }

  Future<void> loadDetail() async {
    state = const AdminUserDetailState.loading();
    final result = await _repository.getUserDetail(userId);
    if (result.failure != null) {
      state = AdminUserDetailState.error(failure: result.failure!);
    } else {
      state = AdminUserDetailState.loaded(user: result.data!);
    }
  }

  Future<bool> toggleActive({required bool isActive}) async {
    final result = await _repository.toggleUserActive(userId, isActive);
    if (result.failure != null) return false;
    state = AdminUserDetailState.loaded(user: result.data!);
    return true;
  }

  Future<bool> changeRole({required int newRole}) async {
    final result = await _repository.changeUserRole(userId, newRole);
    if (result.failure != null) return false;
    state = AdminUserDetailState.loaded(user: result.data!);
    return true;
  }
}
