import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/partner_dashboard_model.dart';
import '../../data/repositories/delivery_repository.dart';

part 'partner_dashboard_notifier.freezed.dart';
part 'partner_dashboard_notifier.g.dart';

@freezed
sealed class PartnerDashboardState with _$PartnerDashboardState {
  const factory PartnerDashboardState.initial() = PartnerDashboardInitial;
  const factory PartnerDashboardState.loading() = PartnerDashboardLoading;
  const factory PartnerDashboardState.loaded({
    required PartnerDashboardModel dashboard,
  }) = PartnerDashboardLoaded;
  const factory PartnerDashboardState.error({required Failure failure}) =
      PartnerDashboardError;
}

@riverpod
class PartnerDashboardNotifier extends _$PartnerDashboardNotifier {
  late DeliveryRepository _repository;

  @override
  PartnerDashboardState build() {
    _repository = ref.watch(deliveryRepositoryProvider);
    loadDashboard();
    return const PartnerDashboardState.initial();
  }

  Future<void> loadDashboard() async {
    state = const PartnerDashboardState.loading();
    final result = await _repository.getDashboard();
    if (result.failure != null) {
      state = PartnerDashboardState.error(failure: result.failure!);
    } else {
      state = PartnerDashboardState.loaded(dashboard: result.data!);
    }
  }
}
