import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/models/cuisine_type_model.dart';
import '../../data/repositories/restaurant_repository.dart';

part 'cuisine_types_provider.g.dart';

@riverpod
Future<List<CuisineTypeModel>> cuisineTypes(CuisineTypesRef ref) async {
  final repository = ref.watch(restaurantRepositoryProvider);
  final result = await repository.getCuisineTypes();
  if (result.failure != null) return [];
  return result.data!;
}
