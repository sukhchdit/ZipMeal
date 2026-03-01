import 'package:freezed_annotation/freezed_annotation.dart';

part 'dietary_profile_model.freezed.dart';
part 'dietary_profile_model.g.dart';

@freezed
class DietaryProfileModel with _$DietaryProfileModel {
  const factory DietaryProfileModel({
    String? id,
    @Default([]) List<int> allergenAlerts,
    @Default([]) List<int> dietaryPreferences,
    int? maxSpiceLevel,
  }) = _DietaryProfileModel;

  factory DietaryProfileModel.fromJson(Map<String, dynamic> json) =>
      _$DietaryProfileModelFromJson(json);
}
