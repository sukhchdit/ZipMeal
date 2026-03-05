import 'package:freezed_annotation/freezed_annotation.dart';

part 'review_report_model.freezed.dart';
part 'review_report_model.g.dart';

@freezed
class ReviewReportModel with _$ReviewReportModel {
  const factory ReviewReportModel({
    required String id,
    required String reviewId,
    String? reviewText,
    required int reviewRating,
    required String reviewerName,
    required String reporterName,
    required String reason,
    String? description,
    required String status,
    String? adminNotes,
    required String createdAt,
    String? resolvedAt,
  }) = _ReviewReportModel;

  factory ReviewReportModel.fromJson(Map<String, dynamic> json) =>
      _$ReviewReportModelFromJson(json);
}
