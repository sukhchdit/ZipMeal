import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/errors/failures.dart';
import '../../../../core/extensions/l10n_extensions.dart';
import '../../data/models/loyalty_dashboard_model.dart';
import '../../data/repositories/loyalty_repository.dart';

part 'points_history_screen.g.dart';

@riverpod
class PointsHistoryNotifier extends _$PointsHistoryNotifier {
  late LoyaltyRepository _repository;
  int? _typeFilter;

  @override
  PointsHistoryState build() {
    _repository = ref.watch(loyaltyRepositoryProvider);
    _loadInitial();
    return const PointsHistoryState.loading();
  }

  Future<void> _loadInitial() async {
    state = const PointsHistoryState.loading();
    final result = await _repository.getTransactions(type: _typeFilter);
    if (result.failure != null) {
      state = PointsHistoryState.error(failure: result.failure!);
      return;
    }
    state = PointsHistoryState.loaded(
      transactions: result.items!,
      totalCount: result.totalCount ?? 0,
      currentPage: result.page ?? 1,
      totalPages: result.totalPages ?? 1,
      isLoadingMore: false,
      typeFilter: _typeFilter,
    );
  }

  Future<void> setTypeFilter(int? type) async {
    _typeFilter = type;
    await _loadInitial();
  }

  Future<void> loadMore() async {
    final current = state;
    if (current is! _Loaded ||
        current.isLoadingMore ||
        current.currentPage >= current.totalPages) {
      return;
    }
    state = current.copyWith(isLoadingMore: true);
    final nextPage = current.currentPage + 1;
    final result = await _repository.getTransactions(
      page: nextPage,
      type: _typeFilter,
    );
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
}

sealed class PointsHistoryState {
  const PointsHistoryState();
  const factory PointsHistoryState.loading() = _Loading;
  const factory PointsHistoryState.loaded({
    required List<LoyaltyTransactionModel> transactions,
    required int totalCount,
    required int currentPage,
    required int totalPages,
    required bool isLoadingMore,
    int? typeFilter,
  }) = _Loaded;
  const factory PointsHistoryState.error({required Failure failure}) = _Error;
}

class _Loading extends PointsHistoryState {
  const _Loading();
}

class _Loaded extends PointsHistoryState {
  const _Loaded({
    required this.transactions,
    required this.totalCount,
    required this.currentPage,
    required this.totalPages,
    required this.isLoadingMore,
    this.typeFilter,
  });

  final List<LoyaltyTransactionModel> transactions;
  final int totalCount;
  final int currentPage;
  final int totalPages;
  final bool isLoadingMore;
  final int? typeFilter;

  _Loaded copyWith({
    List<LoyaltyTransactionModel>? transactions,
    int? totalCount,
    int? currentPage,
    int? totalPages,
    bool? isLoadingMore,
    int? typeFilter,
  }) =>
      _Loaded(
        transactions: transactions ?? this.transactions,
        totalCount: totalCount ?? this.totalCount,
        currentPage: currentPage ?? this.currentPage,
        totalPages: totalPages ?? this.totalPages,
        isLoadingMore: isLoadingMore ?? this.isLoadingMore,
        typeFilter: typeFilter ?? this.typeFilter,
      );
}

class _Error extends PointsHistoryState {
  const _Error({required this.failure});
  final Failure failure;
}

class PointsHistoryScreen extends ConsumerWidget {
  const PointsHistoryScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final historyState = ref.watch(pointsHistoryNotifierProvider);

    return Scaffold(
      appBar: AppBar(
        title: Text(context.l10n.loyaltyPointsHistory),
      ),
      body: switch (historyState) {
        _Loading() => const Center(child: CircularProgressIndicator()),
        _Loaded(:final transactions, :final isLoadingMore, :final typeFilter) =>
          _LoadedBody(
            transactions: transactions,
            isLoadingMore: isLoadingMore,
            typeFilter: typeFilter,
          ),
        _Error(:final failure) => Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(failure.message),
                const SizedBox(height: 16),
                FilledButton(
                  onPressed: () => ref
                      .read(pointsHistoryNotifierProvider.notifier)
                      .setTypeFilter(null),
                  child: const Text('Retry'),
                ),
              ],
            ),
          ),
      },
    );
  }
}

class _LoadedBody extends ConsumerWidget {
  const _LoadedBody({
    required this.transactions,
    required this.isLoadingMore,
    this.typeFilter,
  });

  final List<LoyaltyTransactionModel> transactions;
  final bool isLoadingMore;
  final int? typeFilter;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);

    return Column(
      children: [
        // Filter chips
        SingleChildScrollView(
          scrollDirection: Axis.horizontal,
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
          child: Row(
            children: [
              _FilterChip(
                label: context.l10n.loyaltyFilterAll,
                isSelected: typeFilter == null,
                onTap: () => ref
                    .read(pointsHistoryNotifierProvider.notifier)
                    .setTypeFilter(null),
              ),
              const SizedBox(width: 8),
              _FilterChip(
                label: context.l10n.loyaltyFilterEarn,
                isSelected: typeFilter == 0,
                onTap: () => ref
                    .read(pointsHistoryNotifierProvider.notifier)
                    .setTypeFilter(0),
              ),
              const SizedBox(width: 8),
              _FilterChip(
                label: context.l10n.loyaltyFilterRedeem,
                isSelected: typeFilter == 1,
                onTap: () => ref
                    .read(pointsHistoryNotifierProvider.notifier)
                    .setTypeFilter(1),
              ),
            ],
          ),
        ),
        const Divider(height: 1),

        // Transaction list
        Expanded(
          child: transactions.isEmpty
              ? Center(
                  child: Text(
                    context.l10n.loyaltyNoTransactions,
                    style: theme.textTheme.bodyLarge?.copyWith(
                      color: AppColors.textSecondaryLight,
                    ),
                  ),
                )
              : NotificationListener<ScrollNotification>(
                  onNotification: (notification) {
                    if (notification is ScrollEndNotification &&
                        notification.metrics.extentAfter < 100) {
                      ref
                          .read(pointsHistoryNotifierProvider.notifier)
                          .loadMore();
                    }
                    return false;
                  },
                  child: ListView.separated(
                    padding: const EdgeInsets.all(16),
                    itemCount: transactions.length + (isLoadingMore ? 1 : 0),
                    separatorBuilder: (_, __) => const Divider(height: 1),
                    itemBuilder: (context, index) {
                      if (index == transactions.length) {
                        return const Padding(
                          padding: EdgeInsets.all(16),
                          child:
                              Center(child: CircularProgressIndicator()),
                        );
                      }
                      final txn = transactions[index];
                      return _HistoryTile(transaction: txn);
                    },
                  ),
                ),
        ),
      ],
    );
  }
}

class _FilterChip extends StatelessWidget {
  const _FilterChip({
    required this.label,
    required this.isSelected,
    required this.onTap,
  });

  final String label;
  final bool isSelected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) => FilterChip(
        label: Text(label),
        selected: isSelected,
        onSelected: (_) => onTap(),
      );
}

class _HistoryTile extends StatelessWidget {
  const _HistoryTile({required this.transaction});

  final LoyaltyTransactionModel transaction;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final isEarn = transaction.type == 0;
    final isExpire = transaction.type == 2;

    final icon = isEarn
        ? Icons.arrow_upward_rounded
        : isExpire
            ? Icons.access_time_rounded
            : Icons.arrow_downward_rounded;

    final color = isEarn
        ? Colors.green
        : isExpire
            ? Colors.orange
            : Colors.red;

    final prefix = isEarn ? '+' : '-';

    return ListTile(
      contentPadding: const EdgeInsets.symmetric(vertical: 4),
      leading: CircleAvatar(
        backgroundColor: color.withValues(alpha: 0.12),
        child: Icon(icon, color: color, size: 20),
      ),
      title: Text(
        transaction.description,
        style: theme.textTheme.bodyMedium,
        maxLines: 2,
        overflow: TextOverflow.ellipsis,
      ),
      subtitle: Text(
        '${transaction.createdAt.day}/${transaction.createdAt.month}/${transaction.createdAt.year}',
        style: theme.textTheme.bodySmall?.copyWith(
          color: AppColors.textSecondaryLight,
        ),
      ),
      trailing: Text(
        '$prefix${transaction.points}',
        style: theme.textTheme.titleMedium?.copyWith(
          color: color,
          fontWeight: FontWeight.bold,
        ),
      ),
    );
  }
}
