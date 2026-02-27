import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/address_repository.dart';
import 'address_list_state.dart';

part 'address_list_notifier.g.dart';

@riverpod
class AddressListNotifier extends _$AddressListNotifier {
  late AddressRepository _repository;

  @override
  AddressListState build() {
    _repository = ref.watch(addressRepositoryProvider);
    loadAddresses();
    return const AddressListState.initial();
  }

  Future<void> loadAddresses() async {
    state = const AddressListState.loading();
    final result = await _repository.getAddresses();
    if (result.failure != null) {
      state = AddressListState.error(failure: result.failure!);
    } else {
      state = AddressListState.loaded(addresses: result.data!);
    }
  }

  Future<void> deleteAddress(String id) async {
    final result = await _repository.deleteAddress(id);
    if (result.failure == null) {
      await loadAddresses();
    }
  }

  Future<void> setDefault(String id) async {
    final result = await _repository.setDefault(id);
    if (result.failure == null) {
      await loadAddresses();
    }
  }
}
