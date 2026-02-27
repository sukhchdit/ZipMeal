import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/address_model.dart';

part 'address_list_state.freezed.dart';

@freezed
sealed class AddressListState with _$AddressListState {
  const factory AddressListState.initial() = AddressListInitial;
  const factory AddressListState.loading() = AddressListLoading;
  const factory AddressListState.loaded({
    required List<AddressModel> addresses,
  }) = AddressListLoaded;
  const factory AddressListState.error({required Failure failure}) =
      AddressListError;
}
