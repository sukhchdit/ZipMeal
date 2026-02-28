import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/platform_analytics_model.dart';
import '../../data/repositories/analytics_repository.dart';

part 'platform_analytics_notifier.freezed.dart';
part 'platform_analytics_notifier.g.dart';

@freezed
sealed class PlatformAnalyticsState with _$PlatformAnalyticsState {
  const factory PlatformAnalyticsState.initial() = PlatformAnalyticsInitial;
  const factory PlatformAnalyticsState.loading() = PlatformAnalyticsLoading;
  const factory PlatformAnalyticsState.loaded({
    required PlatformAnalyticsModel analytics,
    required String period,
    required int days,
  }) = PlatformAnalyticsLoaded;
  const factory PlatformAnalyticsState.error({required Failure failure}) =
      PlatformAnalyticsError;
}

@riverpod
class PlatformAnalyticsNotifier extends _$PlatformAnalyticsNotifier {
  late AnalyticsRepository _repository;

  @override
  PlatformAnalyticsState build() {
    _repository = ref.watch(analyticsRepositoryProvider);
    loadAnalytics();
    return const PlatformAnalyticsState.initial();
  }

  Future<void> loadAnalytics({
    String period = 'daily',
    int days = 30,
  }) async {
    state = const PlatformAnalyticsState.loading();
    final result = await _repository.getPlatformAnalytics(
      period: period,
      days: days,
    );
    if (result.failure != null) {
      state = PlatformAnalyticsState.error(failure: result.failure!);
    } else {
      state = PlatformAnalyticsState.loaded(
        analytics: result.data!,
        period: period,
        days: days,
      );
    }
  }
}
