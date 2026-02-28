import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/delivery_assignment_model.dart';

part 'active_delivery_state.freezed.dart';

@freezed
sealed class ActiveDeliveryState with _$ActiveDeliveryState {
  const factory ActiveDeliveryState.initial() = ActiveDeliveryInitial;
  const factory ActiveDeliveryState.loading() = ActiveDeliveryLoading;
  const factory ActiveDeliveryState.loaded({
    DeliveryAssignmentModel? assignment,
  }) = ActiveDeliveryLoaded;
  const factory ActiveDeliveryState.error({required Failure failure}) =
      ActiveDeliveryError;
}
