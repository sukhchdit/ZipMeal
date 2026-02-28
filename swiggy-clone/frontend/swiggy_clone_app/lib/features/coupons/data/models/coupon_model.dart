import 'package:freezed_annotation/freezed_annotation.dart';

part 'coupon_model.freezed.dart';
part 'coupon_model.g.dart';

@freezed
class CouponModel with _$CouponModel {
  const factory CouponModel({
    required String id,
    required String code,
    required String title,
    String? description,
    required int discountType,
    required int discountValue,
    int? maxDiscount,
    required int minOrderAmount,
    required String validFrom,
    required String validUntil,
    required bool isActive,
  }) = _CouponModel;

  factory CouponModel.fromJson(Map<String, dynamic> json) =>
      _$CouponModelFromJson(json);
}
