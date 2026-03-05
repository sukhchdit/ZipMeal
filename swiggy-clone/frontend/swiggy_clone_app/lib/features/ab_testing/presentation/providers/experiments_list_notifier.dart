import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/ab_testing_repository.dart';
import 'experiments_list_state.dart';

part 'experiments_list_notifier.g.dart';

@riverpod
class ExperimentsListNotifier extends _$ExperimentsListNotifier {
  late AbTestingRepository _repo;

  @override
  ExperimentsListState build() {
    _repo = ref.watch(abTestingRepositoryProvider);
    return const ExperimentsListState.initial();
  }

  Future<void> loadExperiments({int? statusFilter}) async {
    state = const ExperimentsListState.loading();
    final result = await _repo.getExperiments(status: statusFilter);
    if (result.failure != null) {
      state = ExperimentsListState.error(message: result.failure!.message);
      return;
    }
    state = ExperimentsListState.loaded(
      experiments: result.data ?? [],
      totalCount: result.totalCount ?? 0,
      currentPage: result.page ?? 1,
      pageSize: result.pageSize ?? 20,
      statusFilter: statusFilter,
    );
  }

  Future<void> loadMore() async {
    final current = state;
    if (current is! ExperimentsListLoaded || current.isLoadingMore) return;
    if (current.experiments.length >= current.totalCount) return;

    state = current.copyWith(isLoadingMore: true);
    final result = await _repo.getExperiments(
      status: current.statusFilter,
      page: current.currentPage + 1,
      pageSize: current.pageSize,
    );
    if (result.failure != null) {
      state = current.copyWith(isLoadingMore: false);
      return;
    }
    state = current.copyWith(
      experiments: [...current.experiments, ...result.data ?? []],
      totalCount: result.totalCount ?? current.totalCount,
      currentPage: result.page ?? current.currentPage + 1,
      isLoadingMore: false,
    );
  }
}
