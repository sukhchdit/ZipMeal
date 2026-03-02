import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/restaurant_insights_model.dart';
import '../../data/repositories/analytics_repository.dart';

part 'restaurant_insights_notifier.freezed.dart';
part 'restaurant_insights_notifier.g.dart';

@freezed
sealed class RestaurantInsightsState with _$RestaurantInsightsState {
  const factory RestaurantInsightsState.initial() = RestaurantInsightsInitial;
  const factory RestaurantInsightsState.loading() = RestaurantInsightsLoading;
  const factory RestaurantInsightsState.loaded({
    required RestaurantInsightsModel insights,
    required String period,
    required int days,
  }) = RestaurantInsightsLoaded;
  const factory RestaurantInsightsState.error({required Failure failure}) =
      RestaurantInsightsError;
}

@riverpod
class RestaurantInsightsNotifier extends _$RestaurantInsightsNotifier {
  late AnalyticsRepository _repository;

  @override
  RestaurantInsightsState build(String restaurantId) {
    _repository = ref.watch(analyticsRepositoryProvider);
    loadInsights(restaurantId: restaurantId);
    return const RestaurantInsightsState.initial();
  }

  Future<void> loadInsights({
    required String restaurantId,
    String period = 'daily',
    int days = 30,
  }) async {
    state = const RestaurantInsightsState.loading();
    final result = await _repository.getRestaurantInsights(
      restaurantId,
      period: period,
      days: days,
    );
    if (result.failure != null) {
      state = RestaurantInsightsState.error(failure: result.failure!);
    } else {
      state = RestaurantInsightsState.loaded(
        insights: result.data!,
        period: period,
        days: days,
      );
    }
  }
}
