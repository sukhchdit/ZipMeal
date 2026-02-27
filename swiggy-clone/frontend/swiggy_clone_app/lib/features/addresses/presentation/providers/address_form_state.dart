import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/address_model.dart';

part 'address_form_state.freezed.dart';

@freezed
sealed class AddressFormState with _$AddressFormState {
  const factory AddressFormState.initial() = AddressFormInitial;
  const factory AddressFormState.saving() = AddressFormSaving;
  const factory AddressFormState.saved({required AddressModel address}) =
      AddressFormSaved;
  const factory AddressFormState.error({required Failure failure}) =
      AddressFormError;
}
