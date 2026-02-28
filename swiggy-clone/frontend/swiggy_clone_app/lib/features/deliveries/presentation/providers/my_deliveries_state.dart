import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/delivery_assignment_model.dart';

part 'my_deliveries_state.freezed.dart';

@freezed
sealed class MyDeliveriesState with _$MyDeliveriesState {
  const factory MyDeliveriesState.initial() = MyDeliveriesInitial;
  const factory MyDeliveriesState.loading() = MyDeliveriesLoading;
  const factory MyDeliveriesState.loaded({
    required List<DeliveryAssignmentModel> deliveries,
    required bool hasMore,
    required String? nextCursor,
    @Default(false) bool isLoadingMore,
  }) = MyDeliveriesLoaded;
  const factory MyDeliveriesState.error({required Failure failure}) =
      MyDeliveriesError;
}
