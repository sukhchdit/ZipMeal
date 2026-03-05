import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/ab_testing_repository.dart';
import 'experiment_detail_state.dart';

part 'experiment_detail_notifier.g.dart';

@riverpod
class ExperimentDetailNotifier extends _$ExperimentDetailNotifier {
  late AbTestingRepository _repo;

  @override
  ExperimentDetailState build(String experimentId) {
    _repo = ref.watch(abTestingRepositoryProvider);
    return const ExperimentDetailState.initial();
  }

  Future<void> load() async {
    state = const ExperimentDetailState.loading();
    final result = await _repo.getExperimentById(experimentId);
    if (result.failure != null) {
      state = ExperimentDetailState.error(message: result.failure!.message);
      return;
    }
    state = ExperimentDetailState.loaded(experiment: result.data!);
  }

  Future<void> activate() async {
    final current = state;
    if (current is! ExperimentDetailLoaded) return;
    state = current.copyWith(isPerformingAction: true);
    final result = await _repo.activateExperiment(experimentId);
    if (result.failure != null) {
      state = current.copyWith(isPerformingAction: false);
      return;
    }
    await load();
  }

  Future<void> pause() async {
    final current = state;
    if (current is! ExperimentDetailLoaded) return;
    state = current.copyWith(isPerformingAction: true);
    final result = await _repo.pauseExperiment(experimentId);
    if (result.failure != null) {
      state = current.copyWith(isPerformingAction: false);
      return;
    }
    await load();
  }

  Future<void> complete() async {
    final current = state;
    if (current is! ExperimentDetailLoaded) return;
    state = current.copyWith(isPerformingAction: true);
    final result = await _repo.completeExperiment(experimentId);
    if (result.failure != null) {
      state = current.copyWith(isPerformingAction: false);
      return;
    }
    await load();
  }
}
