import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/cart_model.dart';

part 'cart_state.freezed.dart';

@freezed
sealed class CartState with _$CartState {
  const factory CartState.initial() = CartInitial;
  const factory CartState.loading() = CartLoading;
  const factory CartState.loaded({required CartModel cart}) = CartLoaded;
  const factory CartState.empty() = CartEmpty;
  const factory CartState.error({required Failure failure}) = CartError;
}
