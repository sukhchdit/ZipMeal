import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/models/user_assignment_model.dart';
import '../../data/repositories/ab_testing_repository.dart';

part 'user_ab_testing_notifier.g.dart';

@riverpod
class UserAbTestingNotifier extends _$UserAbTestingNotifier {
  late AbTestingRepository _repo;
  List<UserAssignmentModel> _assignments = [];

  @override
  List<UserAssignmentModel> build() {
    _repo = ref.watch(abTestingRepositoryProvider);
    return [];
  }

  /// Fetches all assignments and caches them locally.
  Future<void> fetchAssignments() async {
    final result = await _repo.getAssignments();
    if (result.data != null) {
      _assignments = result.data!;
      state = _assignments;
    }
  }

  /// Returns the variant key for a given experiment key, or null if unassigned.
  String? getVariantKey(String experimentKey) {
    final match = _assignments.where((a) => a.experimentKey == experimentKey);
    return match.isEmpty ? null : match.first.variantKey;
  }

  /// Returns the config JSON for a given experiment key, or null.
  String? getConfig(String experimentKey) {
    final match = _assignments.where((a) => a.experimentKey == experimentKey);
    return match.isEmpty ? null : match.first.configJson;
  }

  /// Records an exposure event (fire-and-forget).
  Future<void> recordExposure({
    required String experimentKey,
    String? context,
  }) async {
    await _repo.recordExposure(
      experimentKey: experimentKey,
      context: context,
    );
  }

  /// Records a conversion event (fire-and-forget).
  Future<void> recordConversion({
    required String experimentKey,
    required String goalKey,
    double? value,
  }) async {
    await _repo.recordConversion(
      experimentKey: experimentKey,
      goalKey: goalKey,
      value: value,
    );
  }
}
