import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/restaurant_analytics_model.dart';
import '../../data/repositories/analytics_repository.dart';

part 'restaurant_analytics_notifier.freezed.dart';
part 'restaurant_analytics_notifier.g.dart';

@freezed
sealed class RestaurantAnalyticsState with _$RestaurantAnalyticsState {
  const factory RestaurantAnalyticsState.initial() =
      RestaurantAnalyticsInitial;
  const factory RestaurantAnalyticsState.loading() =
      RestaurantAnalyticsLoading;
  const factory RestaurantAnalyticsState.loaded({
    required RestaurantAnalyticsModel analytics,
    required String period,
    required int days,
  }) = RestaurantAnalyticsLoaded;
  const factory RestaurantAnalyticsState.error({required Failure failure}) =
      RestaurantAnalyticsError;
}

@riverpod
class RestaurantAnalyticsNotifier extends _$RestaurantAnalyticsNotifier {
  late AnalyticsRepository _repository;

  @override
  RestaurantAnalyticsState build(String restaurantId) {
    _repository = ref.watch(analyticsRepositoryProvider);
    loadAnalytics(restaurantId: restaurantId);
    return const RestaurantAnalyticsState.initial();
  }

  Future<void> loadAnalytics({
    required String restaurantId,
    String period = 'daily',
    int days = 30,
  }) async {
    state = const RestaurantAnalyticsState.loading();
    final result = await _repository.getRestaurantAnalytics(
      restaurantId,
      period: period,
      days: days,
    );
    if (result.failure != null) {
      state = RestaurantAnalyticsState.error(failure: result.failure!);
    } else {
      state = RestaurantAnalyticsState.loaded(
        analytics: result.data!,
        period: period,
        days: days,
      );
    }
  }
}
