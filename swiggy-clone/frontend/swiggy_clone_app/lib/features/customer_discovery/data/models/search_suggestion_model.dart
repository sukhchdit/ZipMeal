import 'package:freezed_annotation/freezed_annotation.dart';

part 'search_suggestion_model.freezed.dart';
part 'search_suggestion_model.g.dart';

@freezed
class SearchSuggestionModel with _$SearchSuggestionModel {
  const factory SearchSuggestionModel({
    required String text,
    required String type,
    required String id,
    String? restaurantId,
    String? restaurantName,
    String? imageUrl,
  }) = _SearchSuggestionModel;

  factory SearchSuggestionModel.fromJson(Map<String, dynamic> json) =>
      _$SearchSuggestionModelFromJson(json);
}
