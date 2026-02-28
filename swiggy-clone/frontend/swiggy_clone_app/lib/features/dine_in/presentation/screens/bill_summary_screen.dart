import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../../orders/data/models/order_model.dart';
import '../providers/session_orders_notifier.dart';
import '../providers/session_orders_state.dart';

class BillSummaryScreen extends ConsumerWidget {
  const BillSummaryScreen({required this.sessionId, super.key});

  final String sessionId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(sessionOrdersNotifierProvider(sessionId));
    final theme = Theme.of(context);

    final totalAmount = switch (state) {
      SessionOrdersLoaded(:final orders) =>
        orders.fold<int>(0, (sum, o) => sum + o.totalAmount),
      _ => 0,
    };

    return Scaffold(
      appBar: AppBar(title: const Text('Bill Summary')),
      body: switch (state) {
        SessionOrdersInitial() || SessionOrdersLoading() =>
          const AppLoadingWidget(message: 'Loading bill...'),
        SessionOrdersError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(sessionOrdersNotifierProvider(sessionId).notifier)
                .loadOrders(),
          ),
        SessionOrdersLoaded(:final orders) =>
          _BillBody(orders: orders, theme: theme),
      },
      bottomNavigationBar: state is SessionOrdersLoaded && totalAmount > 0
          ? SafeArea(
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: FilledButton(
                  onPressed: () => _showPaymentMethodSheet(
                      context, totalAmount),
                  style: FilledButton.styleFrom(
                    backgroundColor: AppColors.primary,
                    minimumSize: const Size.fromHeight(52),
                  ),
                  child: Text(
                    'Pay Now  \u20B9${totalAmount ~/ 100}',
                    style: const TextStyle(
                      fontWeight: FontWeight.bold,
                      fontSize: 16,
                    ),
                  ),
                ),
              ),
            )
          : null,
    );
  }

  void _showPaymentMethodSheet(BuildContext context, int totalAmount) {
    const methods = [
      (value: 1, label: 'UPI', icon: Icons.account_balance_wallet_outlined),
      (value: 2, label: 'Card', icon: Icons.credit_card_outlined),
      (value: 3, label: 'Net Banking', icon: Icons.account_balance_outlined),
      (value: 4, label: 'Wallet', icon: Icons.wallet_outlined),
    ];

    showModalBottomSheet(
      context: context,
      builder: (ctx) => Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Center(
              child: Container(
                width: 40,
                height: 4,
                decoration: BoxDecoration(
                  color: Colors.grey[300],
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
            ),
            const SizedBox(height: 16),
            Text(
              'Select Payment Method',
              style: Theme.of(context)
                  .textTheme
                  .titleMedium
                  ?.copyWith(fontWeight: FontWeight.bold),
            ),
            const SizedBox(height: 12),
            ...methods.map((method) => ListTile(
                  leading: Icon(method.icon, color: AppColors.primary),
                  title: Text(method.label),
                  trailing: const Icon(Icons.chevron_right),
                  onTap: () {
                    Navigator.of(ctx).pop();
                    context.push(RouteNames.payment, extra: {
                      'sessionId': sessionId,
                      'paymentMethod': method.value,
                    });
                  },
                )),
            const SizedBox(height: 8),
          ],
        ),
      ),
    );
  }
}

class _BillBody extends StatelessWidget {
  const _BillBody({required this.orders, required this.theme});

  final List<OrderModel> orders;
  final ThemeData theme;

  @override
  Widget build(BuildContext context) {
    // Aggregate totals
    int subtotal = 0;
    int taxAmount = 0;
    int totalAmount = 0;
    for (final order in orders) {
      subtotal += order.subtotal;
      taxAmount += order.taxAmount;
      totalAmount += order.totalAmount;
    }

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        // Order breakdown
        ...orders.map((order) => Card(
              margin: const EdgeInsets.only(bottom: 12),
              shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12)),
              child: Padding(
                padding: const EdgeInsets.all(12),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      order.orderNumber,
                      style: theme.textTheme.titleSmall
                          ?.copyWith(fontWeight: FontWeight.w600),
                    ),
                    const SizedBox(height: 8),
                    ...order.items.map((item) => Padding(
                          padding: const EdgeInsets.symmetric(vertical: 2),
                          child: Row(
                            mainAxisAlignment:
                                MainAxisAlignment.spaceBetween,
                            children: [
                              Expanded(
                                child: Text(
                                  '${item.quantity}x ${item.itemName}',
                                  style: theme.textTheme.bodyMedium,
                                ),
                              ),
                              Text(
                                '\u20B9${(item.totalPrice / 100).toStringAsFixed(0)}',
                                style: theme.textTheme.bodyMedium,
                              ),
                            ],
                          ),
                        )),
                  ],
                ),
              ),
            )),

        const Divider(height: 32),

        // Totals
        _PriceRow(
          label: 'Subtotal',
          amount: subtotal,
          theme: theme,
        ),
        const SizedBox(height: 4),
        _PriceRow(
          label: 'Tax (5% GST)',
          amount: taxAmount,
          theme: theme,
        ),
        const Divider(height: 24),
        _PriceRow(
          label: 'Total',
          amount: totalAmount,
          theme: theme,
          isBold: true,
        ),
      ],
    );
  }
}

class _PriceRow extends StatelessWidget {
  const _PriceRow({
    required this.label,
    required this.amount,
    required this.theme,
    this.isBold = false,
  });

  final String label;
  final int amount;
  final ThemeData theme;
  final bool isBold;

  @override
  Widget build(BuildContext context) {
    final style = isBold
        ? theme.textTheme.titleMedium?.copyWith(fontWeight: FontWeight.bold)
        : theme.textTheme.bodyMedium?.copyWith(
            color: AppColors.textSecondaryLight,
          );

    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(label, style: style),
        Text('\u20B9${(amount / 100).toStringAsFixed(0)}', style: style),
      ],
    );
  }
}
