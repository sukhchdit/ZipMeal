import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/revenue_forecast_model.dart';
import '../../data/repositories/analytics_repository.dart';

part 'revenue_forecast_notifier.freezed.dart';
part 'revenue_forecast_notifier.g.dart';

@freezed
sealed class RevenueForecastState with _$RevenueForecastState {
  const factory RevenueForecastState.initial() = RevenueForecastInitial;
  const factory RevenueForecastState.loading() = RevenueForecastLoading;
  const factory RevenueForecastState.loaded({
    required RevenueForecastModel forecast,
    required int days,
    required int forecastDays,
  }) = RevenueForecastLoaded;
  const factory RevenueForecastState.error({required Failure failure}) =
      RevenueForecastError;
}

@riverpod
class RevenueForecastNotifier extends _$RevenueForecastNotifier {
  late AnalyticsRepository _repository;

  @override
  RevenueForecastState build(String? restaurantId) {
    _repository = ref.watch(analyticsRepositoryProvider);
    loadForecast(restaurantId: restaurantId);
    return const RevenueForecastState.initial();
  }

  Future<void> loadForecast({
    String? restaurantId,
    int days = 30,
    int forecastDays = 14,
  }) async {
    state = const RevenueForecastState.loading();
    final result = restaurantId != null
        ? await _repository.getRestaurantForecast(
            restaurantId,
            days: days,
            forecastDays: forecastDays,
          )
        : await _repository.getRevenueForecast(
            days: days,
            forecastDays: forecastDays,
          );
    if (result.failure != null) {
      state = RevenueForecastState.error(failure: result.failure!);
    } else {
      state = RevenueForecastState.loaded(
        forecast: result.data!,
        days: days,
        forecastDays: forecastDays,
      );
    }
  }
}
