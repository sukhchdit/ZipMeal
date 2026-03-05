import 'package:freezed_annotation/freezed_annotation.dart';

part 'experiment_model.freezed.dart';
part 'experiment_model.g.dart';

@freezed
class ExperimentModel with _$ExperimentModel {
  const factory ExperimentModel({
    required String id,
    required String key,
    required String name,
    String? description,
    required int status,
    String? targetAudience,
    String? startDate,
    String? endDate,
    String? goalDescription,
    required String createdByUserId,
    required List<ExperimentVariantModel> variants,
    required String createdAt,
    required String updatedAt,
  }) = _ExperimentModel;

  factory ExperimentModel.fromJson(Map<String, dynamic> json) =>
      _$ExperimentModelFromJson(json);
}

@freezed
class ExperimentVariantModel with _$ExperimentVariantModel {
  const factory ExperimentVariantModel({
    required String id,
    required String key,
    required String name,
    required int allocationPercent,
    String? configJson,
    required bool isControl,
  }) = _ExperimentVariantModel;

  factory ExperimentVariantModel.fromJson(Map<String, dynamic> json) =>
      _$ExperimentVariantModelFromJson(json);
}
