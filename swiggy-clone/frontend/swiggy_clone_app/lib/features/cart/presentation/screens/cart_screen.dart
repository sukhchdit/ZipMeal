import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../providers/cart_notifier.dart';
import '../providers/cart_state.dart';
import '../widgets/cart_item_card.dart';

class CartScreen extends ConsumerWidget {
  const CartScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(cartNotifierProvider);
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Cart'),
        actions: [
          if (state is CartLoaded)
            TextButton(
              onPressed: () =>
                  ref.read(cartNotifierProvider.notifier).clearCart(),
              child: const Text('Clear'),
            ),
        ],
      ),
      body: switch (state) {
        CartInitial() || CartLoading() =>
          const AppLoadingWidget(message: 'Loading cart...'),
        CartError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () =>
                ref.read(cartNotifierProvider.notifier).loadCart(),
          ),
        CartEmpty() => Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Icon(Icons.shopping_cart_outlined,
                    size: 80, color: AppColors.textTertiaryLight),
                const SizedBox(height: 16),
                Text(
                  'Your cart is empty',
                  style: theme.textTheme.titleMedium?.copyWith(
                    color: AppColors.textSecondaryLight,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  'Add items from a restaurant to get started',
                  style: theme.textTheme.bodyMedium?.copyWith(
                    color: AppColors.textTertiaryLight,
                  ),
                ),
              ],
            ),
          ),
        CartLoaded(:final cart) => Column(
            children: [
              // Restaurant name header
              Container(
                width: double.infinity,
                padding:
                    const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
                color: AppColors.primary.withValues(alpha: 0.08),
                child: Text(
                  cart.restaurantName,
                  style: theme.textTheme.titleSmall?.copyWith(
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),

              // Cart items list
              Expanded(
                child: ListView.separated(
                  padding: const EdgeInsets.symmetric(vertical: 8),
                  itemCount: cart.items.length,
                  separatorBuilder: (_, __) => const Divider(height: 1),
                  itemBuilder: (context, index) {
                    final item = cart.items[index];
                    return CartItemCard(
                      item: item,
                      onQuantityChanged: (qty) => ref
                          .read(cartNotifierProvider.notifier)
                          .updateQuantity(
                            cartItemId: item.cartItemId,
                            quantity: qty,
                          ),
                      onRemove: () => ref
                          .read(cartNotifierProvider.notifier)
                          .removeItem(cartItemId: item.cartItemId),
                    );
                  },
                ),
              ),

              // Bottom bar with subtotal + checkout button
              SafeArea(
                child: Container(
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    color: theme.scaffoldBackgroundColor,
                    boxShadow: [
                      BoxShadow(
                        color: AppColors.shadow,
                        blurRadius: 8,
                        offset: const Offset(0, -2),
                      ),
                    ],
                  ),
                  child: Row(
                    children: [
                      Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        mainAxisSize: MainAxisSize.min,
                        children: [
                          Text(
                            'Subtotal',
                            style: theme.textTheme.bodySmall?.copyWith(
                              color: AppColors.textSecondaryLight,
                            ),
                          ),
                          Text(
                            '\u20B9${cart.subtotal ~/ 100}',
                            style: theme.textTheme.titleLarge?.copyWith(
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(width: 16),
                      Expanded(
                        child: FilledButton(
                          onPressed: () => context.push(RouteNames.checkout),
                          style: FilledButton.styleFrom(
                            backgroundColor: AppColors.primary,
                            minimumSize: const Size.fromHeight(48),
                          ),
                          child: const Text(
                            'Proceed to Checkout',
                            style: TextStyle(
                              fontWeight: FontWeight.bold,
                              fontSize: 16,
                            ),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ),
      },
    );
  }
}
