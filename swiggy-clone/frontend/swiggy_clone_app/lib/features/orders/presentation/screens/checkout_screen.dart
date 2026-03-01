import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../l10n/app_localizations.dart';
import '../../../../routing/route_names.dart';
import '../../../cart/data/models/cart_model.dart';
import '../../../cart/presentation/providers/cart_notifier.dart';
import '../../../cart/presentation/providers/cart_state.dart';
import '../../../coupons/presentation/providers/coupon_validation_notifier.dart';
import '../../../coupons/presentation/providers/coupon_validation_state.dart';
import '../../../addresses/data/models/address_model.dart';
import '../../../addresses/presentation/providers/address_list_notifier.dart';
import '../../../addresses/presentation/providers/address_list_state.dart';
import '../../../addresses/presentation/widgets/address_selection_sheet.dart';
import '../../../coupons/presentation/widgets/available_coupons_sheet.dart';
import '../../../dietary/presentation/providers/dietary_profile_notifier.dart';
import '../../data/models/fee_config_model.dart';
import '../../../subscriptions/presentation/providers/subscription_notifier.dart';
import '../../../subscriptions/presentation/providers/subscription_state.dart';
import '../providers/fee_config_provider.dart';
import '../providers/place_order_notifier.dart';
import '../providers/place_order_state.dart';

class CheckoutScreen extends ConsumerStatefulWidget {
  const CheckoutScreen({super.key});

  @override
  ConsumerState<CheckoutScreen> createState() => _CheckoutScreenState();
}

class _CheckoutScreenState extends ConsumerState<CheckoutScreen> {
  int _selectedPaymentMethod = 5; // Default: Cash on Delivery
  final _couponController = TextEditingController();
  final _specialInstructionsController = TextEditingController();
  String? _appliedCouponCode;
  int _couponDiscount = 0;
  AddressModel? _selectedAddress;
  bool _isScheduled = false;
  DateTime? _scheduledDate;
  TimeOfDay? _scheduledTime;

  static const _paymentMethods = [
    (value: 1, label: 'UPI', icon: Icons.account_balance_wallet_outlined),
    (value: 2, label: 'Card', icon: Icons.credit_card_outlined),
    (value: 3, label: 'Net Banking', icon: Icons.account_balance_outlined),
    (value: 4, label: 'Wallet', icon: Icons.wallet_outlined),
    (value: 5, label: 'Cash on Delivery', icon: Icons.money_outlined),
  ];

  @override
  void dispose() {
    _couponController.dispose();
    _specialInstructionsController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final cartState = ref.watch(cartNotifierProvider);
    final placeOrderState = ref.watch(placeOrderNotifierProvider);
    final couponState = ref.watch(couponValidationNotifierProvider);
    final theme = Theme.of(context);

    // Handle coupon validation result
    ref.listen<CouponValidationState>(couponValidationNotifierProvider,
        (prev, next) {
      if (next is CouponValidationValidated) {
        final result = next.result;
        if (result.isValid) {
          setState(() {
            _appliedCouponCode = result.code;
            _couponDiscount = result.discountAmount;
          });
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
                content: Text(
                    'Coupon applied! You save \u20B9${result.discountAmount ~/ 100}')),
          );
        } else {
          setState(() {
            _appliedCouponCode = null;
            _couponDiscount = 0;
          });
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
                content: Text(result.invalidReason ?? 'Invalid coupon code')),
          );
        }
      }
    });

    // Navigate on order placed
    ref.listen<PlaceOrderState>(placeOrderNotifierProvider, (prev, next) {
      if (next is PlaceOrderPlaced) {
        if (_selectedPaymentMethod <= 4) {
          context.go(RouteNames.payment, extra: {
            'orderId': next.order.id,
            'orderNumber': next.order.orderNumber,
            'paymentMethod': _selectedPaymentMethod,
          });
        } else {
          context.go(RouteNames.orderSuccess, extra: {
            'orderId': next.order.id,
            'orderNumber': next.order.orderNumber,
          });
        }
      }
    });

    final feeConfigAsync = ref.watch(feeConfigProvider);

    final cart = switch (cartState) {
      CartLoaded(:final cart) => cart,
      _ => null,
    };

    if (cart == null) {
      return Scaffold(
        appBar: AppBar(title: const Text('Checkout')),
        body: const Center(child: Text('Cart is empty')),
      );
    }

    // Use dynamic fees from server, fall back to defaults on error/loading
    final FeeConfigModel fees = feeConfigAsync.valueOrNull ??
        const FeeConfigModel(
          deliveryFeePaise: 4900,
          packagingChargePaise: 1500,
          taxRatePercent: 5.0,
        );

    final taxAmount = (cart.subtotal * (fees.taxRatePercent / 100)).round();
    var deliveryFee = fees.deliveryFeePaise;
    final packagingCharge = fees.packagingChargePaise;

    // Waive delivery fee if subtotal exceeds threshold
    if (fees.freeDeliveryThresholdPaise != null &&
        cart.subtotal >= fees.freeDeliveryThresholdPaise!) {
      deliveryFee = 0;
    }

    // Check subscription benefits for free delivery
    final subState = ref.watch(subscriptionNotifierProvider);
    final hasFreeDelivery = subState is SubscriptionLoaded &&
        subState.activeSubscription != null &&
        subState.activeSubscription!.freeDelivery;
    final originalDeliveryFee = deliveryFee;
    if (hasFreeDelivery) {
      deliveryFee = 0;
    }

    final totalAmount =
        cart.subtotal + taxAmount + deliveryFee + packagingCharge - _couponDiscount;

    return Scaffold(
      appBar: AppBar(title: const Text('Checkout')),
      body: switch (placeOrderState) {
        PlaceOrderPlacing() =>
          const AppLoadingWidget(message: 'Placing your order...'),
        _ => ListView(
            padding: const EdgeInsets.all(16),
            children: [
              // Restaurant
              Text(
                cart.restaurantName,
                style: theme.textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 16),

              // Items summary
              ...cart.items.map((item) => Padding(
                    padding: const EdgeInsets.only(bottom: 8),
                    child: Row(
                      children: [
                        Expanded(
                          child: Text(
                            '${item.quantity}x ${item.itemName}',
                            style: theme.textTheme.bodyMedium,
                          ),
                        ),
                        Text(
                          '\u20B9${item.totalPrice ~/ 100}',
                          style: theme.textTheme.bodyMedium?.copyWith(
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ],
                    ),
                  )),

              const Divider(height: 24),

              // Coupon section
              Text(
                'Apply Coupon',
                style: theme.textTheme.titleSmall?.copyWith(
                  fontWeight: FontWeight.w600,
                ),
              ),
              const SizedBox(height: 8),
              if (_appliedCouponCode != null) ...[
                Container(
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: AppColors.success.withValues(alpha: 0.08),
                    border:
                        Border.all(color: AppColors.success.withValues(alpha: 0.3)),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Row(
                    children: [
                      Icon(Icons.check_circle,
                          color: AppColors.success, size: 20),
                      const SizedBox(width: 8),
                      Expanded(
                        child: Column(
                          crossAxisAlignment: CrossAxisAlignment.start,
                          children: [
                            Text(
                              _appliedCouponCode!,
                              style: theme.textTheme.titleSmall?.copyWith(
                                fontWeight: FontWeight.bold,
                                color: AppColors.success,
                              ),
                            ),
                            Text(
                              'You save \u20B9${_couponDiscount ~/ 100}',
                              style: theme.textTheme.bodySmall?.copyWith(
                                color: AppColors.success,
                              ),
                            ),
                          ],
                        ),
                      ),
                      IconButton(
                        onPressed: () {
                          setState(() {
                            _appliedCouponCode = null;
                            _couponDiscount = 0;
                            _couponController.clear();
                          });
                          ref
                              .read(couponValidationNotifierProvider.notifier)
                              .clear();
                        },
                        icon: const Icon(Icons.close, size: 20),
                        visualDensity: VisualDensity.compact,
                      ),
                    ],
                  ),
                ),
              ] else ...[
                Row(
                  children: [
                    Expanded(
                      child: TextField(
                        controller: _couponController,
                        textCapitalization: TextCapitalization.characters,
                        decoration: InputDecoration(
                          hintText: 'Enter coupon code',
                          border: const OutlineInputBorder(),
                          isDense: true,
                          contentPadding: const EdgeInsets.symmetric(
                              horizontal: 12, vertical: 10),
                          suffixIcon: couponState is CouponValidationValidating
                              ? const Padding(
                                  padding: EdgeInsets.all(12),
                                  child: SizedBox(
                                    width: 16,
                                    height: 16,
                                    child: CircularProgressIndicator(
                                        strokeWidth: 2),
                                  ),
                                )
                              : null,
                        ),
                      ),
                    ),
                    const SizedBox(width: 8),
                    FilledButton(
                      onPressed: couponState is CouponValidationValidating
                          ? null
                          : () {
                              if (_couponController.text.trim().isNotEmpty) {
                                ref
                                    .read(couponValidationNotifierProvider
                                        .notifier)
                                    .validateCoupon(
                                      code: _couponController.text.trim(),
                                      subtotal: cart.subtotal,
                                      orderType: 1, // Delivery
                                      restaurantId: cart.restaurantId,
                                    );
                              }
                            },
                      style: FilledButton.styleFrom(
                        backgroundColor: AppColors.primary,
                        padding: const EdgeInsets.symmetric(
                            horizontal: 16, vertical: 10),
                      ),
                      child: const Text('APPLY'),
                    ),
                  ],
                ),
                const SizedBox(height: 8),
                GestureDetector(
                  onTap: () => _showAvailableCoupons(context),
                  child: Text(
                    'View available coupons',
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.primary,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
              ],

              const Divider(height: 24),

              // Price breakdown
              _PriceRow(label: 'Subtotal', amount: cart.subtotal),
              const SizedBox(height: 4),
              _PriceRow(
                label:
                    'Tax (${fees.taxRatePercent % 1 == 0 ? fees.taxRatePercent.toInt() : fees.taxRatePercent}%)',
                amount: taxAmount,
              ),
              const SizedBox(height: 4),
              if (hasFreeDelivery && originalDeliveryFee > 0)
                _SubscriptionDeliveryRow(
                    originalFee: originalDeliveryFee)
              else
                _PriceRow(label: 'Delivery Fee', amount: deliveryFee),
              const SizedBox(height: 4),
              _PriceRow(label: 'Packaging', amount: packagingCharge),
              if (_couponDiscount > 0) ...[
                const SizedBox(height: 4),
                _PriceRow(
                  label: 'Coupon Discount',
                  amount: -_couponDiscount,
                  isDiscount: true,
                ),
              ],
              const Divider(height: 24),
              _PriceRow(
                label: 'Total',
                amount: totalAmount,
                isBold: true,
              ),

              const SizedBox(height: 24),

              // Special instructions
              Text(
                'Special Instructions',
                style: theme.textTheme.titleSmall?.copyWith(
                  fontWeight: FontWeight.w600,
                ),
              ),
              const SizedBox(height: 8),
              TextFormField(
                controller: _specialInstructionsController,
                maxLength: 500,
                maxLines: 2,
                decoration: InputDecoration(
                  hintText: 'Any instructions for the restaurant...',
                  border: const OutlineInputBorder(),
                  isDense: true,
                  contentPadding: const EdgeInsets.all(12),
                  hintStyle: theme.textTheme.bodySmall?.copyWith(
                    color: AppColors.textTertiaryLight,
                  ),
                ),
              ),

              const SizedBox(height: 8),

              // Delivery address
              _buildAddressSection(theme),

              const SizedBox(height: 16),

              // Payment method selection
              Text(
                'Payment Method',
                style: theme.textTheme.titleSmall?.copyWith(
                  fontWeight: FontWeight.w600,
                ),
              ),
              const SizedBox(height: 8),
              ..._paymentMethods.map((method) => RadioListTile<int>(
                    value: method.value,
                    groupValue: _selectedPaymentMethod,
                    onChanged: (value) =>
                        setState(() => _selectedPaymentMethod = value!),
                    title: Text(method.label,
                        style: theme.textTheme.bodyMedium),
                    secondary: Icon(method.icon,
                        color: _selectedPaymentMethod == method.value
                            ? AppColors.primary
                            : AppColors.textTertiaryLight),
                    activeColor: AppColors.primary,
                    contentPadding: EdgeInsets.zero,
                    dense: true,
                    visualDensity: VisualDensity.compact,
                  )),

              const Divider(height: 24),

              // Schedule for later
              SwitchListTile(
                value: _isScheduled,
                onChanged: (val) => setState(() {
                  _isScheduled = val;
                  if (!val) {
                    _scheduledDate = null;
                    _scheduledTime = null;
                  }
                }),
                title: const Text('Schedule for Later'),
                secondary: const Icon(Icons.schedule_send, color: AppColors.primary),
                contentPadding: EdgeInsets.zero,
                dense: true,
              ),
              if (_isScheduled) ...[
                const SizedBox(height: 8),
                Row(
                  children: [
                    ActionChip(
                      avatar: const Icon(Icons.calendar_today, size: 16),
                      label: Text(_scheduledDate != null
                          ? _formatDate(_scheduledDate!)
                          : 'Select Date'),
                      onPressed: () async {
                        final now = DateTime.now();
                        final picked = await showDatePicker(
                          context: context,
                          initialDate: _scheduledDate ?? now,
                          firstDate: now,
                          lastDate: now.add(const Duration(days: 7)),
                        );
                        if (picked != null) {
                          setState(() => _scheduledDate = picked);
                        }
                      },
                    ),
                    const SizedBox(width: 8),
                    ActionChip(
                      avatar: const Icon(Icons.access_time, size: 16),
                      label: Text(_scheduledTime != null
                          ? _scheduledTime!.format(context)
                          : 'Select Time'),
                      onPressed: () async {
                        final picked = await showTimePicker(
                          context: context,
                          initialTime: _scheduledTime ?? TimeOfDay.now(),
                        );
                        if (picked != null) {
                          setState(() => _scheduledTime = picked);
                        }
                      },
                    ),
                  ],
                ),
                if (_scheduledDate != null && _scheduledTime != null) ...[
                  const SizedBox(height: 4),
                  if (_buildScheduledDateTime()!
                      .isBefore(DateTime.now().add(const Duration(minutes: 30))))
                    Text(
                      'Must be at least 30 minutes from now',
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppColors.error,
                      ),
                    ),
                ],
              ],

              if (placeOrderState is PlaceOrderError) ...[
                const SizedBox(height: 16),
                Container(
                  padding: const EdgeInsets.all(12),
                  decoration: BoxDecoration(
                    color: AppColors.error.withValues(alpha: 0.1),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Text(
                    (placeOrderState as PlaceOrderError).failure.message,
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.error,
                    ),
                  ),
                ),
              ],
            ],
          ),
      },
      bottomNavigationBar: SafeArea(
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: FilledButton(
            onPressed: placeOrderState is PlaceOrderPlacing ||
                    _selectedAddress == null ||
                    (_isScheduled && !_isScheduleValid())
                ? null
                : () => _onPlaceOrder(cart),
            style: FilledButton.styleFrom(
              backgroundColor: AppColors.primary,
              minimumSize: const Size.fromHeight(52),
            ),
            child: Text(
              '${_isScheduled ? 'Schedule' : 'Place'} Order  \u20B9${totalAmount ~/ 100}',
              style: const TextStyle(
                fontWeight: FontWeight.bold,
                fontSize: 16,
              ),
            ),
          ),
        ),
      ),
    );
  }

  static const _allergenNames = [
    'Gluten', 'Dairy', 'Nuts', 'Peanuts', 'Shellfish', 'Soy', 'Eggs',
    'Fish', 'Sesame', 'Mustard', 'Celery', 'Lupin', 'Molluscs', 'Sulfites',
  ];

  void _onPlaceOrder(CartModel cart) {
    // Check for allergen warnings
    final dietaryState = ref.read(dietaryProfileNotifierProvider);
    final profile = dietaryState.valueOrNull;
    if (profile != null && profile.allergenAlerts.isNotEmpty) {
      final flagged = <int>{};
      for (final item in cart.items) {
        for (final a in item.allergens) {
          if (profile.allergenAlerts.contains(a)) {
            flagged.add(a);
          }
        }
      }
      if (flagged.isNotEmpty) {
        _showAllergenWarning(flagged.toList()..sort());
        return;
      }
    }
    _confirmPlaceOrder();
  }

  void _confirmPlaceOrder() {
    final orderInstructions = _specialInstructionsController.text.trim();
    ref.read(placeOrderNotifierProvider.notifier).placeOrder(
          deliveryAddressId: _selectedAddress!.id,
          paymentMethod: _selectedPaymentMethod,
          specialInstructions:
              orderInstructions.isEmpty ? null : orderInstructions,
          couponCode: _appliedCouponCode,
          scheduledDeliveryTime: _isScheduled
              ? _buildScheduledDateTime()?.toUtc().toIso8601String()
              : null,
        );
  }

  void _showAllergenWarning(List<int> flaggedAllergens) {
    final l10n = AppLocalizations.of(context);
    final theme = Theme.of(context);
    showModalBottomSheet<bool>(
      context: context,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (ctx) => Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(Icons.warning_amber_rounded,
                color: Colors.amber.shade700, size: 48),
            const SizedBox(height: 12),
            Text(
              l10n?.allergenWarning ?? 'Allergen Warning',
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 8),
            Text(
              l10n?.allergenFound ??
                  'Allergen found in your cart',
              style: theme.textTheme.bodyMedium,
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 12),
            Wrap(
              spacing: 8,
              runSpacing: 4,
              children: flaggedAllergens.map((a) {
                final name = a >= 0 && a < _allergenNames.length
                    ? _allergenNames[a]
                    : 'Unknown';
                return Chip(
                  label: Text(name),
                  backgroundColor: Colors.amber.shade100,
                  labelStyle: TextStyle(color: Colors.amber.shade900),
                  visualDensity: VisualDensity.compact,
                );
              }).toList(),
            ),
            const SizedBox(height: 20),
            Row(
              children: [
                Expanded(
                  child: OutlinedButton(
                    onPressed: () => Navigator.pop(ctx, false),
                    child: Text(l10n?.cancel ?? 'Cancel'),
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: FilledButton(
                    onPressed: () => Navigator.pop(ctx, true),
                    style: FilledButton.styleFrom(
                      backgroundColor: Colors.amber.shade700,
                    ),
                    child: Text(
                        l10n?.proceedWithAllergens ?? 'Proceed Anyway'),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    ).then((proceed) {
      if (proceed == true) {
        _confirmPlaceOrder();
      }
    });
  }

  Widget _buildAddressSection(ThemeData theme) {
    // Auto-select default address from loaded list
    if (_selectedAddress == null) {
      final addrState = ref.read(addressListNotifierProvider);
      if (addrState is AddressListLoaded && addrState.addresses.isNotEmpty) {
        final defaultAddr = addrState.addresses.cast<AddressModel?>().firstWhere(
              (a) => a!.isDefault,
              orElse: () => addrState.addresses.first,
            );
        if (defaultAddr != null) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            if (mounted) setState(() => _selectedAddress = defaultAddr);
          });
        }
      }
    }

    return InkWell(
      onTap: _showAddressSelection,
      borderRadius: BorderRadius.circular(8),
      child: Container(
        padding: const EdgeInsets.all(12),
        decoration: BoxDecoration(
          border: Border.all(
            color: _selectedAddress == null
                ? AppColors.error.withValues(alpha: 0.5)
                : AppColors.borderLight,
          ),
          borderRadius: BorderRadius.circular(8),
        ),
        child: Row(
          children: [
            const Icon(Icons.location_on_outlined, color: AppColors.primary),
            const SizedBox(width: 8),
            Expanded(
              child: _selectedAddress != null
                  ? Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Text(
                          _selectedAddress!.label,
                          style: theme.textTheme.titleSmall?.copyWith(
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                        Text(
                          [
                            _selectedAddress!.addressLine1,
                            if (_selectedAddress!.city != null)
                              _selectedAddress!.city,
                            if (_selectedAddress!.postalCode != null)
                              _selectedAddress!.postalCode,
                          ].join(', '),
                          style: theme.textTheme.bodySmall?.copyWith(
                            color: AppColors.textSecondaryLight,
                          ),
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ],
                    )
                  : Text(
                      'Select a delivery address',
                      style: theme.textTheme.bodyMedium?.copyWith(
                        color: AppColors.error,
                      ),
                    ),
            ),
            const Icon(Icons.chevron_right, color: AppColors.textTertiaryLight),
          ],
        ),
      ),
    );
  }

  void _showAddressSelection() {
    showModalBottomSheet<AddressModel>(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (_) => AddressSelectionSheet(
        selectedId: _selectedAddress?.id,
      ),
    ).then((selected) {
      if (selected != null) {
        setState(() => _selectedAddress = selected);
      }
    });
  }

  DateTime? _buildScheduledDateTime() {
    if (_scheduledDate == null || _scheduledTime == null) return null;
    return DateTime(
      _scheduledDate!.year,
      _scheduledDate!.month,
      _scheduledDate!.day,
      _scheduledTime!.hour,
      _scheduledTime!.minute,
    );
  }

  bool _isScheduleValid() {
    final dt = _buildScheduledDateTime();
    if (dt == null) return false;
    return dt.isAfter(DateTime.now().add(const Duration(minutes: 30)));
  }

  String _formatDate(DateTime date) {
    final now = DateTime.now();
    final today = DateTime(now.year, now.month, now.day);
    final target = DateTime(date.year, date.month, date.day);
    if (target == today) return 'Today';
    if (target == today.add(const Duration(days: 1))) return 'Tomorrow';
    return '${date.day}/${date.month}';
  }

  void _showAvailableCoupons(BuildContext context) {
    showModalBottomSheet(
      context: context,
      isScrollControlled: true,
      shape: const RoundedRectangleBorder(
        borderRadius: BorderRadius.vertical(top: Radius.circular(16)),
      ),
      builder: (_) => AvailableCouponsSheet(
        onApply: (code) {
          _couponController.text = code;
          final cartState = ref.read(cartNotifierProvider);
          final cart = switch (cartState) {
            CartLoaded(:final cart) => cart,
            _ => null,
          };
          if (cart != null) {
            ref.read(couponValidationNotifierProvider.notifier).validateCoupon(
                  code: code,
                  subtotal: cart.subtotal,
                  orderType: 1,
                  restaurantId: cart.restaurantId,
                );
          }
        },
      ),
    );
  }
}

class _SubscriptionDeliveryRow extends StatelessWidget {
  const _SubscriptionDeliveryRow({required this.originalFee});

  final int originalFee;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text('Delivery Fee', style: theme.textTheme.bodyMedium),
        Row(
          children: [
            Text(
              '\u20B9${originalFee ~/ 100}',
              style: theme.textTheme.bodyMedium?.copyWith(
                decoration: TextDecoration.lineThrough,
                color: AppColors.textTertiaryLight,
              ),
            ),
            const SizedBox(width: 6),
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 6, vertical: 2),
              decoration: BoxDecoration(
                color: AppColors.success.withValues(alpha: 0.1),
                borderRadius: BorderRadius.circular(4),
              ),
              child: Text(
                'FREE',
                style: theme.textTheme.labelSmall?.copyWith(
                  color: AppColors.success,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
          ],
        ),
      ],
    );
  }
}

class _PriceRow extends StatelessWidget {
  const _PriceRow({
    required this.label,
    required this.amount,
    this.isBold = false,
    this.isDiscount = false,
  });

  final String label;
  final int amount;
  final bool isBold;
  final bool isDiscount;

  @override
  Widget build(BuildContext context) {
    final baseStyle = isBold
        ? Theme.of(context)
            .textTheme
            .titleMedium
            ?.copyWith(fontWeight: FontWeight.bold)
        : Theme.of(context).textTheme.bodyMedium;

    final style = isDiscount
        ? baseStyle?.copyWith(color: AppColors.success)
        : baseStyle;

    final prefix = amount < 0 ? '-' : '';
    final absAmount = amount.abs();

    return Row(
      mainAxisAlignment: MainAxisAlignment.spaceBetween,
      children: [
        Text(label, style: style),
        Text('$prefix\u20B9${absAmount ~/ 100}', style: style),
      ],
    );
  }
}
