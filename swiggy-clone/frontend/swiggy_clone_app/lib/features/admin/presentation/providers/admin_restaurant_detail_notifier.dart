import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/admin_restaurant_model.dart';
import '../../data/repositories/admin_repository.dart';

part 'admin_restaurant_detail_notifier.freezed.dart';
part 'admin_restaurant_detail_notifier.g.dart';

@freezed
sealed class AdminRestaurantDetailState with _$AdminRestaurantDetailState {
  const factory AdminRestaurantDetailState.initial() =
      AdminRestaurantDetailInitial;
  const factory AdminRestaurantDetailState.loading() =
      AdminRestaurantDetailLoading;
  const factory AdminRestaurantDetailState.loaded({
    required AdminRestaurantModel restaurant,
  }) = AdminRestaurantDetailLoaded;
  const factory AdminRestaurantDetailState.error({required Failure failure}) =
      AdminRestaurantDetailError;
}

@riverpod
class AdminRestaurantDetailNotifier extends _$AdminRestaurantDetailNotifier {
  late AdminRepository _repository;

  @override
  AdminRestaurantDetailState build(String restaurantId) {
    _repository = ref.watch(adminRepositoryProvider);
    loadDetail();
    return const AdminRestaurantDetailState.initial();
  }

  Future<void> loadDetail() async {
    state = const AdminRestaurantDetailState.loading();
    final result = await _repository.getRestaurantDetail(restaurantId);
    if (result.failure != null) {
      state = AdminRestaurantDetailState.error(failure: result.failure!);
    } else {
      state =
          AdminRestaurantDetailState.loaded(restaurant: result.data!);
    }
  }

  Future<bool> approve() async {
    final result = await _repository.approveRestaurant(restaurantId);
    if (result.failure != null) return false;
    state = AdminRestaurantDetailState.loaded(restaurant: result.data!);
    return true;
  }

  Future<bool> reject(String reason) async {
    final result = await _repository.rejectRestaurant(restaurantId, reason);
    if (result.failure != null) return false;
    state = AdminRestaurantDetailState.loaded(restaurant: result.data!);
    return true;
  }

  Future<bool> suspend(String reason) async {
    final result = await _repository.suspendRestaurant(restaurantId, reason);
    if (result.failure != null) return false;
    state = AdminRestaurantDetailState.loaded(restaurant: result.data!);
    return true;
  }

  Future<bool> reactivate() async {
    final result = await _repository.reactivateRestaurant(restaurantId);
    if (result.failure != null) return false;
    state = AdminRestaurantDetailState.loaded(restaurant: result.data!);
    return true;
  }
}
