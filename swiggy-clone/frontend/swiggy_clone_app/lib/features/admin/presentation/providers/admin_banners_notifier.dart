import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/models/admin_banner_model.dart';
import '../../data/repositories/admin_repository.dart';
import 'admin_banners_state.dart';

part 'admin_banners_notifier.g.dart';

@riverpod
class AdminBannersNotifier extends _$AdminBannersNotifier {
  late AdminRepository _repository;

  @override
  AdminBannersState build() {
    _repository = ref.watch(adminRepositoryProvider);
    loadBanners();
    return const AdminBannersState.initial();
  }

  Future<void> loadBanners({
    String? search,
    bool? isActive,
    int page = 1,
  }) async {
    state = const AdminBannersState.loading();
    final result = await _repository.getBanners(
      search: search,
      isActive: isActive,
      page: page,
    );
    if (result.failure != null) {
      state = AdminBannersState.error(failure: result.failure!);
    } else {
      state = AdminBannersState.loaded(
        banners: result.items!,
        totalCount: result.totalCount ?? 0,
        page: result.page ?? 1,
        totalPages: result.totalPages ?? 1,
      );
    }
  }

  Future<bool> createBanner(Map<String, dynamic> data) async {
    final result = await _repository.createBanner(data);
    if (result.failure != null) return false;
    final current = state;
    if (current is AdminBannersLoaded) {
      state = current.copyWith(
        banners: [result.data!, ...current.banners],
        totalCount: current.totalCount + 1,
      );
    }
    return true;
  }

  Future<bool> updateBanner(String id, Map<String, dynamic> data) async {
    final result = await _repository.updateBanner(id, data);
    if (result.failure != null) return false;
    _updateInList(result.data!);
    return true;
  }

  Future<bool> toggleBanner(String id, bool isActive) async {
    final failure = await _repository.toggleBanner(id, isActive);
    if (failure != null) return false;
    final current = state;
    if (current is AdminBannersLoaded) {
      final list = current.banners.map((b) {
        if (b.id == id) return b.copyWith(isActive: isActive);
        return b;
      }).toList();
      state = current.copyWith(banners: list);
    }
    return true;
  }

  void _updateInList(AdminBannerModel updated) {
    final current = state;
    if (current is AdminBannersLoaded) {
      final list = current.banners.map((b) {
        if (b.id == updated.id) return updated;
        return b;
      }).toList();
      state = current.copyWith(banners: list);
    }
  }
}
