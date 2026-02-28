import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/admin_dashboard_model.dart';
import '../../data/repositories/admin_repository.dart';

part 'admin_dashboard_notifier.freezed.dart';
part 'admin_dashboard_notifier.g.dart';

@freezed
sealed class AdminDashboardState with _$AdminDashboardState {
  const factory AdminDashboardState.initial() = AdminDashboardInitial;
  const factory AdminDashboardState.loading() = AdminDashboardLoading;
  const factory AdminDashboardState.loaded({
    required AdminDashboardModel dashboard,
  }) = AdminDashboardLoaded;
  const factory AdminDashboardState.error({required Failure failure}) =
      AdminDashboardError;
}

@riverpod
class AdminDashboardNotifier extends _$AdminDashboardNotifier {
  late AdminRepository _repository;

  @override
  AdminDashboardState build() {
    _repository = ref.watch(adminRepositoryProvider);
    loadDashboard();
    return const AdminDashboardState.initial();
  }

  Future<void> loadDashboard() async {
    state = const AdminDashboardState.loading();
    final result = await _repository.getDashboard();
    if (result.failure != null) {
      state = AdminDashboardState.error(failure: result.failure!);
    } else {
      state = AdminDashboardState.loaded(dashboard: result.data!);
    }
  }
}
