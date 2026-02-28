import 'package:freezed_annotation/freezed_annotation.dart';

part 'review_photo_model.freezed.dart';
part 'review_photo_model.g.dart';

@freezed
class ReviewPhotoModel with _$ReviewPhotoModel {
  const factory ReviewPhotoModel({
    required String id,
    required String photoUrl,
    required int sortOrder,
  }) = _ReviewPhotoModel;

  factory ReviewPhotoModel.fromJson(Map<String, dynamic> json) =>
      _$ReviewPhotoModelFromJson(json);
}
