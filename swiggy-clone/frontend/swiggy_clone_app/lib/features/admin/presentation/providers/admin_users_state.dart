import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/admin_user_model.dart';

part 'admin_users_state.freezed.dart';

@freezed
sealed class AdminUsersState with _$AdminUsersState {
  const factory AdminUsersState.initial() = AdminUsersInitial;
  const factory AdminUsersState.loading() = AdminUsersLoading;
  const factory AdminUsersState.loaded({
    required List<AdminUserModel> users,
    required int totalCount,
    required int page,
    required int totalPages,
    @Default(false) bool isLoadingMore,
  }) = AdminUsersLoaded;
  const factory AdminUsersState.error({required Failure failure}) =
      AdminUsersError;
}
