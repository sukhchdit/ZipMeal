import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/models/wallet_model.dart';
import '../../data/repositories/wallet_repository.dart';
import 'wallet_state.dart';

part 'wallet_notifier.g.dart';

@riverpod
class WalletNotifier extends _$WalletNotifier {
  late WalletRepository _repository;

  @override
  WalletState build() {
    _repository = ref.watch(walletRepositoryProvider);
    loadWallet();
    return const WalletState.initial();
  }

  Future<void> loadWallet() async {
    state = const WalletState.loading();

    final balanceResult = await _repository.getBalance();
    if (balanceResult.failure != null) {
      state = WalletState.error(failure: balanceResult.failure!);
      return;
    }

    final txnResult = await _repository.getTransactions();
    if (txnResult.failure != null) {
      state = WalletState.error(failure: txnResult.failure!);
      return;
    }

    state = WalletState.loaded(
      wallet: balanceResult.data!,
      transactions: txnResult.items!,
      totalCount: txnResult.totalCount ?? 0,
      currentPage: txnResult.page ?? 1,
      totalPages: txnResult.totalPages ?? 1,
    );
  }

  Future<void> loadMoreTransactions() async {
    final current = state;
    if (current is! WalletLoaded ||
        current.isLoadingMore ||
        current.currentPage >= current.totalPages) {
      return;
    }

    state = current.copyWith(isLoadingMore: true);
    final nextPage = current.currentPage + 1;
    final result = await _repository.getTransactions(page: nextPage);

    if (result.failure != null) {
      state = current.copyWith(isLoadingMore: false);
      return;
    }

    state = current.copyWith(
      transactions: [...current.transactions, ...result.items!],
      currentPage: result.page ?? nextPage,
      totalPages: result.totalPages ?? current.totalPages,
      totalCount: result.totalCount ?? current.totalCount,
      isLoadingMore: false,
    );
  }

  Future<bool> addMoney(int amountPaise) async {
    final current = state;
    if (current is! WalletLoaded) return false;

    state = current.copyWith(isAddingMoney: true);
    final result = await _repository.addMoney(amountPaise);

    if (result.failure != null) {
      state = current.copyWith(isAddingMoney: false);
      return false;
    }

    // Update balance and prepend the new transaction
    final updatedWallet = WalletModel(
      id: current.wallet.id,
      userId: current.wallet.userId,
      balancePaise: result.data!.balanceAfterPaise,
      updatedAt: DateTime.now(),
    );

    state = current.copyWith(
      wallet: updatedWallet,
      transactions: [result.data!, ...current.transactions],
      totalCount: current.totalCount + 1,
      isAddingMoney: false,
    );

    return true;
  }
}
