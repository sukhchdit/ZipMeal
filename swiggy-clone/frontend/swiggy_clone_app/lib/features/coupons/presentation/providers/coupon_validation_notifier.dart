import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/coupon_repository.dart';
import 'coupon_validation_state.dart';

part 'coupon_validation_notifier.g.dart';

@riverpod
class CouponValidationNotifier extends _$CouponValidationNotifier {
  late CouponRepository _repository;

  @override
  CouponValidationState build() {
    _repository = ref.watch(couponRepositoryProvider);
    return const CouponValidationState.initial();
  }

  Future<void> validateCoupon({
    required String code,
    required int subtotal,
    required int orderType,
    required String restaurantId,
  }) async {
    state = const CouponValidationState.validating();
    final result = await _repository.validateCoupon(
      code: code,
      subtotal: subtotal,
      orderType: orderType,
      restaurantId: restaurantId,
    );
    if (result.failure != null) {
      state = CouponValidationState.error(failure: result.failure!);
    } else {
      state = CouponValidationState.validated(result: result.data!);
    }
  }

  void clear() {
    state = const CouponValidationState.initial();
  }
}
