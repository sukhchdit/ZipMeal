import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/dispute_repository.dart';
import 'create_dispute_state.dart';

part 'create_dispute_notifier.g.dart';

@riverpod
class CreateDisputeNotifier extends _$CreateDisputeNotifier {
  late DisputeRepository _repo;

  @override
  CreateDisputeState build() {
    _repo = ref.watch(disputeRepositoryProvider);
    return const CreateDisputeState.initial();
  }

  Future<void> submit({
    required String orderId,
    required int issueType,
    required String description,
  }) async {
    state = const CreateDisputeState.submitting();
    final result = await _repo.createDispute(
      orderId: orderId,
      issueType: issueType,
      description: description,
    );
    if (result.failure != null) {
      state = CreateDisputeState.error(message: result.failure!.message);
      return;
    }
    state = CreateDisputeState.success(dispute: result.data!);
  }
}
