import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/models/referral_stats_model.dart';
import '../../data/repositories/referral_repository.dart';

part 'referral_notifier.g.dart';

@riverpod
class ReferralNotifier extends _$ReferralNotifier {
  @override
  Future<ReferralStatsModel> build() async {
    final repository = ref.watch(referralRepositoryProvider);
    final result = await repository.getStats();
    if (result.failure != null) {
      throw Exception(result.failure!.message);
    }
    return result.data!;
  }

  Future<void> refresh() async {
    ref.invalidateSelf();
  }
}
