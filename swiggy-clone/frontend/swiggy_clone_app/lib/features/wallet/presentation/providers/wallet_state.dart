import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/wallet_model.dart';
import '../../data/models/wallet_transaction_model.dart';

part 'wallet_state.freezed.dart';

@freezed
sealed class WalletState with _$WalletState {
  const factory WalletState.initial() = WalletInitial;
  const factory WalletState.loading() = WalletLoading;
  const factory WalletState.loaded({
    required WalletModel wallet,
    required List<WalletTransactionModel> transactions,
    required int totalCount,
    required int currentPage,
    required int totalPages,
    @Default(false) bool isLoadingMore,
    @Default(false) bool isAddingMoney,
  }) = WalletLoaded;
  const factory WalletState.error({required Failure failure}) = WalletError;
}
