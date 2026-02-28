import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/coupon_validation_model.dart';

part 'coupon_validation_state.freezed.dart';

@freezed
sealed class CouponValidationState with _$CouponValidationState {
  const factory CouponValidationState.initial() = CouponValidationInitial;
  const factory CouponValidationState.validating() = CouponValidationValidating;
  const factory CouponValidationState.validated({
    required CouponValidationModel result,
  }) = CouponValidationValidated;
  const factory CouponValidationState.error({required Failure failure}) =
      CouponValidationError;
}
