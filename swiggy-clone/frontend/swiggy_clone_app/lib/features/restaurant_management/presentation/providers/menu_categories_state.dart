import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/menu_category_model.dart';

part 'menu_categories_state.freezed.dart';

@freezed
sealed class MenuCategoriesState with _$MenuCategoriesState {
  const factory MenuCategoriesState.initial() = MenuCategoriesInitial;
  const factory MenuCategoriesState.loading() = MenuCategoriesLoading;
  const factory MenuCategoriesState.loaded({
    required List<MenuCategoryModel> categories,
  }) = MenuCategoriesLoaded;
  const factory MenuCategoriesState.error({required Failure failure}) =
      MenuCategoriesError;
}
