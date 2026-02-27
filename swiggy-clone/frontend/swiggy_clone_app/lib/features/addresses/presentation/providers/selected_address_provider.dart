import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/models/address_model.dart';

part 'selected_address_provider.g.dart';

@riverpod
class SelectedAddress extends _$SelectedAddress {
  @override
  AddressModel? build() => null;

  void select(AddressModel address) {
    state = address;
  }

  void clear() {
    state = null;
  }
}
