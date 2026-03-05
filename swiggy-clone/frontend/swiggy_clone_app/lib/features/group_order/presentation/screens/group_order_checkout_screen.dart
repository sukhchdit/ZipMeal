import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/extensions/l10n_extensions.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../providers/active_group_order_notifier.dart';
import '../providers/group_cart_notifier.dart';
import '../providers/group_cart_state.dart';
import '../providers/group_order_notifier.dart';

class GroupOrderCheckoutScreen extends ConsumerStatefulWidget {
  const GroupOrderCheckoutScreen({required this.groupOrderId, super.key});
  final String groupOrderId;

  @override
  ConsumerState<GroupOrderCheckoutScreen> createState() =>
      _GroupOrderCheckoutScreenState();
}

class _GroupOrderCheckoutScreenState
    extends ConsumerState<GroupOrderCheckoutScreen> {
  bool _isFinalizing = false;

  @override
  Widget build(BuildContext context) {
    final cartState = ref.watch(groupCartNotifierProvider(widget.groupOrderId));
    final l10n = context.l10n;
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(title: Text(l10n.groupOrderCheckout)),
      body: switch (cartState) {
        GroupCartInitial() || GroupCartLoading() =>
          const AppLoadingWidget(message: 'Loading cart...'),
        GroupCartError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(groupCartNotifierProvider(widget.groupOrderId).notifier)
                .loadCart(),
          ),
        GroupCartEmpty() => Center(child: Text(l10n.groupOrderNoItems)),
        GroupCartLoaded(:final cart) => Column(
            children: [
              Expanded(
                child: ListView.builder(
                  padding: const EdgeInsets.all(16),
                  itemCount: cart.participantCarts.length,
                  itemBuilder: (context, index) {
                    final pc = cart.participantCarts[index];
                    return Card(
                      margin: const EdgeInsets.only(bottom: 12),
                      child: ExpansionTile(
                        title: Text(pc.userName),
                        subtitle: Text(
                          '${pc.items.length} items | '
                          '\u20B9${(pc.subtotal / 100).toStringAsFixed(0)}',
                        ),
                        children: pc.items
                            .map(
                              (item) => ListTile(
                                title: Text(item.itemName),
                                trailing: Text(
                                  '${item.quantity} x '
                                  '\u20B9${(item.unitPrice / 100).toStringAsFixed(0)}',
                                ),
                              ),
                            )
                            .toList(),
                      ),
                    );
                  },
                ),
              ),
              Container(
                padding: const EdgeInsets.all(16),
                decoration: BoxDecoration(
                  color: theme.colorScheme.surface,
                  boxShadow: [
                    BoxShadow(
                      color: Colors.black.withValues(alpha: 0.1),
                      blurRadius: 8,
                      offset: const Offset(0, -2),
                    ),
                  ],
                ),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.stretch,
                  children: [
                    Row(
                      mainAxisAlignment: MainAxisAlignment.spaceBetween,
                      children: [
                        Text(
                          l10n.groupOrderGrandTotal,
                          style: theme.textTheme.titleMedium,
                        ),
                        Text(
                          '\u20B9${(cart.grandTotal / 100).toStringAsFixed(0)}',
                          style: theme.textTheme.titleLarge?.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 12),
                    FilledButton.icon(
                      onPressed: _isFinalizing ? null : _finalize,
                      icon: _isFinalizing
                          ? const SizedBox(
                              width: 20,
                              height: 20,
                              child:
                                  CircularProgressIndicator(strokeWidth: 2),
                            )
                          : const Icon(Icons.check_circle),
                      label: Text(l10n.groupOrderFinalize),
                    ),
                  ],
                ),
              ),
            ],
          ),
      },
    );
  }

  Future<void> _finalize() async {
    setState(() => _isFinalizing = true);

    // For v1, use a simplified finalize with placeholder address and payment
    final result = await ref
        .read(groupOrderNotifierProvider(widget.groupOrderId).notifier)
        .finalizeGroupOrder(
          deliveryAddressId: '', // Would come from address picker
          paymentMethod: 1, // Would come from payment selector
        );

    setState(() => _isFinalizing = false);

    if (!mounted) return;

    if (result == null) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(content: Text('Failed to finalize group order')),
      );
      return;
    }

    ref
        .read(activeGroupOrderNotifierProvider.notifier)
        .checkActiveGroupOrder();

    final orderId = result['id'] as String? ?? '';
    if (orderId.isNotEmpty) {
      context.go('/orders/$orderId');
    }
  }
}
