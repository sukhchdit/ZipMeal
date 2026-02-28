import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/restaurant_table_model.dart';

part 'table_management_state.freezed.dart';

@freezed
sealed class TableManagementState with _$TableManagementState {
  const factory TableManagementState.initial() = TableManagementInitial;
  const factory TableManagementState.loading() = TableManagementLoading;
  const factory TableManagementState.loaded({
    required List<RestaurantTableModel> tables,
  }) = TableManagementLoaded;
  const factory TableManagementState.error({required Failure failure}) =
      TableManagementError;
}
