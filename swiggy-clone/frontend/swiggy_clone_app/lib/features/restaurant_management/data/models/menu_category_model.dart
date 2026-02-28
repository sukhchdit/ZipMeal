import 'package:freezed_annotation/freezed_annotation.dart';

part 'menu_category_model.freezed.dart';
part 'menu_category_model.g.dart';

@freezed
class MenuCategoryModel with _$MenuCategoryModel {
  const factory MenuCategoryModel({
    required String id,
    required String name,
    String? description,
    required int sortOrder,
    required bool isActive,
    required int itemCount,
  }) = _MenuCategoryModel;

  factory MenuCategoryModel.fromJson(Map<String, dynamic> json) =>
      _$MenuCategoryModelFromJson(json);
}
