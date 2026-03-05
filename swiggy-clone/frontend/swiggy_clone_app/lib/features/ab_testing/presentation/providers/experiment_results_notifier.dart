import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/ab_testing_repository.dart';
import 'experiment_results_state.dart';

part 'experiment_results_notifier.g.dart';

@riverpod
class ExperimentResultsNotifier extends _$ExperimentResultsNotifier {
  late AbTestingRepository _repo;

  @override
  ExperimentResultsState build(String experimentId) {
    _repo = ref.watch(abTestingRepositoryProvider);
    return const ExperimentResultsState.initial();
  }

  Future<void> load() async {
    state = const ExperimentResultsState.loading();
    final result = await _repo.getExperimentResults(experimentId);
    if (result.failure != null) {
      state = ExperimentResultsState.error(message: result.failure!.message);
      return;
    }
    state = ExperimentResultsState.loaded(stats: result.data!);
  }

  Future<void> refresh() async => load();
}
