import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/auth_response_model.dart';

part 'profile_update_state.freezed.dart';

@freezed
sealed class ProfileUpdateState with _$ProfileUpdateState {
  const factory ProfileUpdateState.initial() = ProfileUpdateInitial;
  const factory ProfileUpdateState.saving() = ProfileUpdateSaving;
  const factory ProfileUpdateState.saved({required UserModel user}) =
      ProfileUpdateSaved;
  const factory ProfileUpdateState.error({required Failure failure}) =
      ProfileUpdateError;
}
