import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../routing/route_names.dart';
import '../providers/payment_notifier.dart';
import '../providers/payment_state.dart';

class PaymentProcessingScreen extends ConsumerStatefulWidget {
  const PaymentProcessingScreen({
    this.orderId,
    this.orderNumber,
    this.sessionId,
    required this.paymentMethod,
    super.key,
  });

  final String? orderId;
  final String? orderNumber;
  final String? sessionId;
  final int paymentMethod;

  @override
  ConsumerState<PaymentProcessingScreen> createState() =>
      _PaymentProcessingScreenState();
}

class _PaymentProcessingScreenState
    extends ConsumerState<PaymentProcessingScreen> {
  bool _initiated = false;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addPostFrameCallback((_) => _initiatePayment());
  }

  void _initiatePayment() {
    if (_initiated) return;
    _initiated = true;

    final notifier = ref.read(paymentNotifierProvider.notifier);
    if (widget.sessionId != null) {
      notifier.initiateDineInPayment(
        sessionId: widget.sessionId!,
        paymentMethod: widget.paymentMethod,
      );
    } else if (widget.orderId != null) {
      notifier.initiatePayment(
        orderId: widget.orderId!,
        paymentMethod: widget.paymentMethod,
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(paymentNotifierProvider);
    final theme = Theme.of(context);

    // Navigate on success
    ref.listen<PaymentState>(paymentNotifierProvider, (prev, next) {
      if (next is PaymentSuccess) {
        if (widget.sessionId != null) {
          // Dine-in: go back to session
          context.go(RouteNames.dineInSessionPath(widget.sessionId!));
        } else {
          // Delivery: go to order success
          context.go(RouteNames.orderSuccess, extra: {
            'orderId': widget.orderId,
            'orderNumber': widget.orderNumber ?? '',
          });
        }
      }
    });

    return Scaffold(
      appBar: AppBar(
        title: const Text('Payment'),
        automaticallyImplyLeading: false,
      ),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: switch (state) {
          PaymentFailed(:final failure) => Center(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Icon(Icons.error_outline, size: 64, color: AppColors.error),
                  const SizedBox(height: 16),
                  Text(
                    'Payment Failed',
                    style: theme.textTheme.titleLarge?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 8),
                  Text(
                    failure.message,
                    style: theme.textTheme.bodyMedium?.copyWith(
                      color: AppColors.textSecondaryLight,
                    ),
                    textAlign: TextAlign.center,
                  ),
                  const SizedBox(height: 24),
                  FilledButton(
                    onPressed: () {
                      _initiated = false;
                      _initiatePayment();
                    },
                    style: FilledButton.styleFrom(
                      backgroundColor: AppColors.primary,
                    ),
                    child: const Text('Retry'),
                  ),
                  const SizedBox(height: 12),
                  TextButton(
                    onPressed: () => context.pop(),
                    child: const Text('Go Back'),
                  ),
                ],
              ),
            ),
          PaymentSuccess() => Center(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Icon(Icons.check_circle,
                      size: 64, color: AppColors.success),
                  const SizedBox(height: 16),
                  Text(
                    'Payment Successful!',
                    style: theme.textTheme.titleLarge?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ],
              ),
            ),
          _ => Center(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  const SizedBox(
                    width: 64,
                    height: 64,
                    child: CircularProgressIndicator(
                      strokeWidth: 3,
                      color: AppColors.primary,
                    ),
                  ),
                  const SizedBox(height: 24),
                  Text(
                    _statusMessage(state),
                    style: theme.textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  const SizedBox(height: 8),
                  if (state is PaymentOrderCreated) ...[
                    Text(
                      state.paymentOrder.gatewayOrderId,
                      style: theme.textTheme.bodySmall?.copyWith(
                        color: AppColors.textTertiaryLight,
                        fontFamily: 'monospace',
                      ),
                    ),
                    const SizedBox(height: 8),
                  ],
                  Text(
                    'Please wait, do not close this screen.',
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: AppColors.textSecondaryLight,
                    ),
                  ),
                ],
              ),
            ),
        },
      ),
    );
  }

  String _statusMessage(PaymentState state) => switch (state) {
        PaymentCreatingOrder() => 'Creating payment order...',
        PaymentOrderCreated() => 'Redirecting to gateway...',
        PaymentProcessing() => 'Processing payment...',
        PaymentVerifying() => 'Verifying payment...',
        _ => 'Initializing...',
      };
}
