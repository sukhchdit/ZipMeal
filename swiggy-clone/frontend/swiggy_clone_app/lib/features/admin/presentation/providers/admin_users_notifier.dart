import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/admin_repository.dart';
import 'admin_users_state.dart';

part 'admin_users_notifier.g.dart';

@riverpod
class AdminUsersNotifier extends _$AdminUsersNotifier {
  late AdminRepository _repository;

  @override
  AdminUsersState build() {
    _repository = ref.watch(adminRepositoryProvider);
    loadUsers();
    return const AdminUsersState.initial();
  }

  Future<void> loadUsers({String? search, int? roleFilter, int page = 1}) async {
    state = const AdminUsersState.loading();
    final result = await _repository.getUsers(
      search: search,
      role: roleFilter,
      page: page,
    );
    if (result.failure != null) {
      state = AdminUsersState.error(failure: result.failure!);
    } else {
      state = AdminUsersState.loaded(
        users: result.items!,
        totalCount: result.totalCount ?? 0,
        page: result.page ?? 1,
        totalPages: result.totalPages ?? 1,
      );
    }
  }

  Future<bool> toggleUserActive(String userId, {required bool isActive}) async {
    final result = await _repository.toggleUserActive(userId, isActive);
    if (result.failure != null) return false;
    // Refresh the list
    final current = state;
    if (current is AdminUsersLoaded) {
      final updated = current.users.map((u) {
        if (u.id == userId) return result.data!;
        return u;
      }).toList();
      state = current.copyWith(users: updated);
    }
    return true;
  }

  Future<bool> changeUserRole(String userId, {required int newRole}) async {
    final result = await _repository.changeUserRole(userId, newRole);
    if (result.failure != null) return false;
    final current = state;
    if (current is AdminUsersLoaded) {
      final updated = current.users.map((u) {
        if (u.id == userId) return result.data!;
        return u;
      }).toList();
      state = current.copyWith(users: updated);
    }
    return true;
  }
}
