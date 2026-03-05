import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/extensions/l10n_extensions.dart';
import '../../../../routing/route_names.dart';
import '../providers/group_cart_notifier.dart';
import '../providers/group_cart_state.dart';

class GroupOrderMenuScreen extends ConsumerWidget {
  const GroupOrderMenuScreen({
    required this.groupOrderId,
    required this.restaurantId,
    super.key,
  });

  final String groupOrderId;
  final String restaurantId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final cartState = ref.watch(groupCartNotifierProvider(groupOrderId));
    final theme = Theme.of(context);
    final l10n = context.l10n;

    // Get item count and total from cart state
    final (itemCount, total) = switch (cartState) {
      GroupCartLoaded(:final cart) => (
          cart.participantCarts.fold<int>(
            0,
            (sum, pc) =>
                sum + pc.items.fold<int>(0, (s, i) => s + i.quantity),
          ),
          cart.grandTotal,
        ),
      _ => (0, 0),
    };

    return Scaffold(
      appBar: AppBar(title: Text(l10n.groupOrderMenu)),
      body: const Center(
        child: Text('Restaurant menu browser goes here.\n'
            'Reuses the existing restaurant menu components\n'
            'with group cart add-to-cart actions.'),
      ),
      bottomNavigationBar: itemCount > 0
          ? Container(
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
              child: Row(
                children: [
                  Expanded(
                    child: Text(
                      '${l10n.groupOrderYourItems}: $itemCount | '
                      '\u20B9${(total / 100).toStringAsFixed(0)}',
                      style: theme.textTheme.titleSmall,
                    ),
                  ),
                  FilledButton(
                    onPressed: () => context.go(
                      RouteNames.groupOrderLobbyPath(groupOrderId),
                    ),
                    child: Text(l10n.groupOrderLobby),
                  ),
                ],
              ),
            )
          : null,
    );
  }
}
