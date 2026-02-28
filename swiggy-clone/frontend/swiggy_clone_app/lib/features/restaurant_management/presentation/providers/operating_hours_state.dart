import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/operating_hours_model.dart';

part 'operating_hours_state.freezed.dart';

@freezed
sealed class OperatingHoursState with _$OperatingHoursState {
  const factory OperatingHoursState.initial() = OperatingHoursInitial;
  const factory OperatingHoursState.loading() = OperatingHoursLoading;
  const factory OperatingHoursState.loaded({
    required List<OperatingHoursModel> hours,
  }) = OperatingHoursLoaded;
  const factory OperatingHoursState.error({required Failure failure}) =
      OperatingHoursError;
}
