import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/customer_funnel_model.dart';
import '../../data/repositories/analytics_repository.dart';

part 'customer_funnel_notifier.freezed.dart';
part 'customer_funnel_notifier.g.dart';

@freezed
sealed class CustomerFunnelState with _$CustomerFunnelState {
  const factory CustomerFunnelState.initial() = CustomerFunnelInitial;
  const factory CustomerFunnelState.loading() = CustomerFunnelLoading;
  const factory CustomerFunnelState.loaded({
    required CustomerFunnelModel funnel,
    required int days,
  }) = CustomerFunnelLoaded;
  const factory CustomerFunnelState.error({required Failure failure}) =
      CustomerFunnelError;
}

@riverpod
class CustomerFunnelNotifier extends _$CustomerFunnelNotifier {
  late AnalyticsRepository _repository;

  @override
  CustomerFunnelState build() {
    _repository = ref.watch(analyticsRepositoryProvider);
    loadFunnel();
    return const CustomerFunnelState.initial();
  }

  Future<void> loadFunnel({int days = 30}) async {
    state = const CustomerFunnelState.loading();
    final result = await _repository.getCustomerFunnel(days: days);
    if (result.failure != null) {
      state = CustomerFunnelState.error(failure: result.failure!);
    } else {
      state = CustomerFunnelState.loaded(
        funnel: result.data!,
        days: days,
      );
    }
  }
}
