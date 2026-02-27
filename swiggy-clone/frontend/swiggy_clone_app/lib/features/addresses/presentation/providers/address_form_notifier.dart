import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/address_repository.dart';
import 'address_form_state.dart';
import 'address_list_notifier.dart';

part 'address_form_notifier.g.dart';

@riverpod
class AddressFormNotifier extends _$AddressFormNotifier {
  late AddressRepository _repository;

  @override
  AddressFormState build() {
    _repository = ref.watch(addressRepositoryProvider);
    return const AddressFormState.initial();
  }

  Future<void> saveAddress({
    String? addressId,
    required String label,
    required String addressLine1,
    String? addressLine2,
    required String city,
    required String stateProvince,
    required String postalCode,
    String? country,
    required double latitude,
    required double longitude,
    bool isDefault = false,
  }) async {
    state = const AddressFormState.saving();

    if (addressId != null) {
      // Update
      final result = await _repository.updateAddress(
        id: addressId,
        label: label,
        addressLine1: addressLine1,
        addressLine2: addressLine2,
        city: city,
        state: stateProvince,
        postalCode: postalCode,
        country: country,
        latitude: latitude,
        longitude: longitude,
      );
      if (result.failure != null) {
        state = AddressFormState.error(failure: result.failure!);
      } else {
        state = AddressFormState.saved(address: result.data!);
        ref.invalidate(addressListNotifierProvider);
      }
    } else {
      // Create
      final result = await _repository.createAddress(
        label: label,
        addressLine1: addressLine1,
        addressLine2: addressLine2,
        city: city,
        state: stateProvince,
        postalCode: postalCode,
        country: country,
        latitude: latitude,
        longitude: longitude,
        isDefault: isDefault,
      );
      if (result.failure != null) {
        state = AddressFormState.error(failure: result.failure!);
      } else {
        state = AddressFormState.saved(address: result.data!);
        ref.invalidate(addressListNotifierProvider);
      }
    }
  }
}
