import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/admin_banner_model.dart';

part 'admin_banners_state.freezed.dart';

@freezed
sealed class AdminBannersState with _$AdminBannersState {
  const factory AdminBannersState.initial() = AdminBannersInitial;
  const factory AdminBannersState.loading() = AdminBannersLoading;
  const factory AdminBannersState.loaded({
    required List<AdminBannerModel> banners,
    required int totalCount,
    required int page,
    required int totalPages,
    @Default(false) bool isLoadingMore,
  }) = AdminBannersLoaded;
  const factory AdminBannersState.error({required Failure failure}) =
      AdminBannersError;
}
