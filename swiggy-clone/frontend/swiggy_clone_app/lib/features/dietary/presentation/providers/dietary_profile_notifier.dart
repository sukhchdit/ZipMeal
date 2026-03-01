import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/datasources/dietary_remote_data_source.dart';
import '../../data/models/dietary_profile_model.dart';

part 'dietary_profile_notifier.g.dart';

@riverpod
class DietaryProfileNotifier extends _$DietaryProfileNotifier {
  late DietaryRemoteDataSource _dataSource;

  @override
  AsyncValue<DietaryProfileModel> build() {
    _dataSource = ref.watch(dietaryRemoteDataSourceProvider);
    _load();
    return const AsyncValue.loading();
  }

  Future<void> _load() async {
    try {
      final profile = await _dataSource.getDietaryProfile();
      state = AsyncValue.data(profile);
    } catch (e, st) {
      state = AsyncValue.error(e, st);
    }
  }

  Future<bool> save({
    required List<int> allergenAlerts,
    required List<int> dietaryPreferences,
    required int? maxSpiceLevel,
  }) async {
    try {
      final profile = await _dataSource.saveDietaryProfile(
        allergenAlerts: allergenAlerts,
        dietaryPreferences: dietaryPreferences,
        maxSpiceLevel: maxSpiceLevel,
      );
      state = AsyncValue.data(profile);
      return true;
    } catch (_) {
      return false;
    }
  }
}
