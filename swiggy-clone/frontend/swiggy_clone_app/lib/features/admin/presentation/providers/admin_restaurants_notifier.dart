import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/models/admin_restaurant_model.dart';
import '../../data/repositories/admin_repository.dart';
import 'admin_restaurants_state.dart';

part 'admin_restaurants_notifier.g.dart';

@riverpod
class AdminRestaurantsNotifier extends _$AdminRestaurantsNotifier {
  late AdminRepository _repository;

  @override
  AdminRestaurantsState build() {
    _repository = ref.watch(adminRepositoryProvider);
    loadRestaurants();
    return const AdminRestaurantsState.initial();
  }

  Future<void> loadRestaurants({
    int? statusFilter,
    String? search,
    int page = 1,
  }) async {
    state = const AdminRestaurantsState.loading();
    final result = await _repository.getRestaurants(
      status: statusFilter,
      search: search,
      page: page,
    );
    if (result.failure != null) {
      state = AdminRestaurantsState.error(failure: result.failure!);
    } else {
      state = AdminRestaurantsState.loaded(
        restaurants: result.items!,
        totalCount: result.totalCount ?? 0,
        page: result.page ?? 1,
        totalPages: result.totalPages ?? 1,
      );
    }
  }

  Future<bool> approveRestaurant(String id) async {
    final result = await _repository.approveRestaurant(id);
    if (result.failure != null) return false;
    _updateInList(result.data!);
    return true;
  }

  Future<bool> rejectRestaurant(String id, String reason) async {
    final result = await _repository.rejectRestaurant(id, reason);
    if (result.failure != null) return false;
    _updateInList(result.data!);
    return true;
  }

  Future<bool> suspendRestaurant(String id, String reason) async {
    final result = await _repository.suspendRestaurant(id, reason);
    if (result.failure != null) return false;
    _updateInList(result.data!);
    return true;
  }

  Future<bool> reactivateRestaurant(String id) async {
    final result = await _repository.reactivateRestaurant(id);
    if (result.failure != null) return false;
    _updateInList(result.data!);
    return true;
  }

  void _updateInList(AdminRestaurantModel updated) {
    final current = state;
    if (current is AdminRestaurantsLoaded) {
      final list = current.restaurants.map((r) {
        if (r.id == updated.id) return updated;
        return r;
      }).toList();
      state = current.copyWith(restaurants: list);
    }
  }
}
