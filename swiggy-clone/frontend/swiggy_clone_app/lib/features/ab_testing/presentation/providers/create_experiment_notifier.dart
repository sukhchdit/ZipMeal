import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/ab_testing_repository.dart';
import 'create_experiment_state.dart';

part 'create_experiment_notifier.g.dart';

@riverpod
class CreateExperimentNotifier extends _$CreateExperimentNotifier {
  late AbTestingRepository _repo;

  @override
  CreateExperimentState build() {
    _repo = ref.watch(abTestingRepositoryProvider);
    return const CreateExperimentState.initial();
  }

  Future<void> submit({
    required String key,
    required String name,
    String? description,
    String? targetAudience,
    String? startDate,
    String? endDate,
    String? goalDescription,
    required List<Map<String, dynamic>> variants,
  }) async {
    state = const CreateExperimentState.submitting();
    final result = await _repo.createExperiment(
      key: key,
      name: name,
      description: description,
      targetAudience: targetAudience,
      startDate: startDate,
      endDate: endDate,
      goalDescription: goalDescription,
      variants: variants,
    );
    if (result.failure != null) {
      state = CreateExperimentState.error(message: result.failure!.message);
      return;
    }
    state = CreateExperimentState.success(experiment: result.data!);
  }
}
