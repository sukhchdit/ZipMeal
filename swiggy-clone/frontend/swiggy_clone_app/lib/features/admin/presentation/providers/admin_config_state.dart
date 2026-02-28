import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/admin_platform_config_model.dart';

part 'admin_config_state.freezed.dart';

@freezed
sealed class AdminConfigState with _$AdminConfigState {
  const factory AdminConfigState.initial() = AdminConfigInitial;
  const factory AdminConfigState.loading() = AdminConfigLoading;
  const factory AdminConfigState.loaded({
    required AdminPlatformConfigModel config,
  }) = AdminConfigLoaded;
  const factory AdminConfigState.saving() = AdminConfigSaving;
  const factory AdminConfigState.saved({
    required AdminPlatformConfigModel config,
  }) = AdminConfigSaved;
  const factory AdminConfigState.error({required Failure failure}) =
      AdminConfigError;
}
