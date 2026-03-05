import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/group_cart_model.dart';

part 'group_cart_state.freezed.dart';

@freezed
sealed class GroupCartState with _$GroupCartState {
  const factory GroupCartState.initial() = GroupCartInitial;
  const factory GroupCartState.loading() = GroupCartLoading;
  const factory GroupCartState.loaded({
    required GroupCartModel cart,
  }) = GroupCartLoaded;
  const factory GroupCartState.empty() = GroupCartEmpty;
  const factory GroupCartState.error({required Failure failure}) =
      GroupCartError;
}
