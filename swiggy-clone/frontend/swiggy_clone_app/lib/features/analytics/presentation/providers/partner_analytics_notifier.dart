import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/partner_analytics_model.dart';
import '../../data/repositories/analytics_repository.dart';

part 'partner_analytics_notifier.freezed.dart';
part 'partner_analytics_notifier.g.dart';

@freezed
sealed class PartnerAnalyticsState with _$PartnerAnalyticsState {
  const factory PartnerAnalyticsState.initial() = PartnerAnalyticsInitial;
  const factory PartnerAnalyticsState.loading() = PartnerAnalyticsLoading;
  const factory PartnerAnalyticsState.loaded({
    required PartnerAnalyticsModel analytics,
    required String period,
    required int days,
  }) = PartnerAnalyticsLoaded;
  const factory PartnerAnalyticsState.error({required Failure failure}) =
      PartnerAnalyticsError;
}

@riverpod
class PartnerAnalyticsNotifier extends _$PartnerAnalyticsNotifier {
  late AnalyticsRepository _repository;

  @override
  PartnerAnalyticsState build() {
    _repository = ref.watch(analyticsRepositoryProvider);
    loadAnalytics();
    return const PartnerAnalyticsState.initial();
  }

  Future<void> loadAnalytics({
    String period = 'daily',
    int days = 30,
  }) async {
    state = const PartnerAnalyticsState.loading();
    final result = await _repository.getPartnerAnalytics(
      period: period,
      days: days,
    );
    if (result.failure != null) {
      state = PartnerAnalyticsState.error(failure: result.failure!);
    } else {
      state = PartnerAnalyticsState.loaded(
        analytics: result.data!,
        period: period,
        days: days,
      );
    }
  }
}
