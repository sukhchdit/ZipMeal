import 'package:freezed_annotation/freezed_annotation.dart';

import 'review_photo_model.dart';

part 'review_model.freezed.dart';
part 'review_model.g.dart';

@freezed
class ReviewModel with _$ReviewModel {
  const factory ReviewModel({
    required String id,
    required String orderId,
    required String userId,
    String? reviewerName,
    String? reviewerAvatarUrl,
    required String restaurantId,
    required int rating,
    String? reviewText,
    int? deliveryRating,
    required bool isAnonymous,
    required bool isVisible,
    String? restaurantReply,
    String? repliedAt,
    required String createdAt,
    @Default([]) List<ReviewPhotoModel> photos,
  }) = _ReviewModel;

  factory ReviewModel.fromJson(Map<String, dynamic> json) =>
      _$ReviewModelFromJson(json);
}
