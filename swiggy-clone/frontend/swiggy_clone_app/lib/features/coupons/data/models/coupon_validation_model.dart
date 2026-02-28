import 'package:freezed_annotation/freezed_annotation.dart';

part 'coupon_validation_model.freezed.dart';
part 'coupon_validation_model.g.dart';

@freezed
class CouponValidationModel with _$CouponValidationModel {
  const factory CouponValidationModel({
    required String id,
    required String code,
    required String title,
    String? description,
    required int discountAmount,
    required bool isValid,
    String? invalidReason,
  }) = _CouponValidationModel;

  factory CouponValidationModel.fromJson(Map<String, dynamic> json) =>
      _$CouponValidationModelFromJson(json);
}
