import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';

part 'partner_online_state.freezed.dart';

@freezed
sealed class PartnerOnlineState with _$PartnerOnlineState {
  const factory PartnerOnlineState.initial() = PartnerOnlineInitial;
  const factory PartnerOnlineState.loading() = PartnerOnlineLoading;
  const factory PartnerOnlineState.loaded({required bool isOnline}) =
      PartnerOnlineLoaded;
  const factory PartnerOnlineState.error({required Failure failure}) =
      PartnerOnlineError;
}
