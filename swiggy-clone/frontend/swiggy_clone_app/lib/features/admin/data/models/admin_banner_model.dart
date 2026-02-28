import 'package:freezed_annotation/freezed_annotation.dart';

part 'admin_banner_model.freezed.dart';
part 'admin_banner_model.g.dart';

@freezed
class AdminBannerModel with _$AdminBannerModel {
  const factory AdminBannerModel({
    required String id,
    required String title,
    required String imageUrl,
    String? deepLink,
    required int displayOrder,
    required DateTime validFrom,
    required DateTime validUntil,
    required bool isActive,
    required DateTime createdAt,
    required DateTime updatedAt,
  }) = _AdminBannerModel;

  factory AdminBannerModel.fromJson(Map<String, dynamic> json) =>
      _$AdminBannerModelFromJson(json);
}
