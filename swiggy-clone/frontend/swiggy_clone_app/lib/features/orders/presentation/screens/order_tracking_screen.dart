import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../deliveries/data/models/delivery_tracking_model.dart';
import '../../../deliveries/presentation/providers/delivery_tracking_notifier.dart';

class OrderTrackingScreen extends ConsumerWidget {
  const OrderTrackingScreen({required this.orderId, super.key});

  final String orderId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(deliveryTrackingNotifierProvider(orderId));

    return Scaffold(
      appBar: AppBar(title: const Text('Track Order')),
      body: switch (state) {
        DeliveryTrackingInitial() || DeliveryTrackingLoading() =>
          const AppLoadingWidget(message: 'Loading tracking info...'),
        DeliveryTrackingError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(deliveryTrackingNotifierProvider(orderId).notifier)
                .loadTracking(),
          ),
        DeliveryTrackingLoaded(:final tracking) =>
          _TrackingBody(tracking: tracking, orderId: orderId, ref: ref),
      },
    );
  }
}

class _TrackingBody extends StatelessWidget {
  const _TrackingBody({
    required this.tracking,
    required this.orderId,
    required this.ref,
  });

  final DeliveryTrackingModel tracking;
  final String orderId;
  final WidgetRef ref;

  static const _orderStatusSteps = [
    (0, 'Order Placed', Icons.receipt_long),
    (1, 'Confirmed', Icons.check_circle),
    (2, 'Preparing', Icons.restaurant),
    (3, 'Ready for Pickup', Icons.shopping_bag),
    (4, 'Out for Delivery', Icons.delivery_dining),
    (5, 'Delivered', Icons.done_all),
  ];

  static const _deliveryStatusLabels = {
    0: 'Partner Assigned',
    1: 'Partner Accepted',
    2: 'Picked Up',
    3: 'On the Way',
    4: 'Delivered',
  };

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return RefreshIndicator(
      onRefresh: () async {
        await ref
            .read(deliveryTrackingNotifierProvider(orderId).notifier)
            .refresh();
      },
      child: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          // Delivery partner info
          if (tracking.partnerName != null) ...[
            Card(
              shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12)),
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Row(
                  children: [
                    CircleAvatar(
                      backgroundColor:
                          AppColors.primary.withValues(alpha: 0.12),
                      child: const Icon(Icons.person,
                          color: AppColors.primary),
                    ),
                    const SizedBox(width: 12),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            tracking.partnerName!,
                            style: theme.textTheme.titleSmall?.copyWith(
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                          if (tracking.partnerPhone != null)
                            Text(
                              tracking.partnerPhone!,
                              style: theme.textTheme.bodySmall?.copyWith(
                                color: AppColors.textSecondaryLight,
                              ),
                            ),
                        ],
                      ),
                    ),
                    Text(
                      _deliveryStatusLabels[tracking.deliveryStatus] ??
                          'Unknown',
                      style: theme.textTheme.labelMedium?.copyWith(
                        color: AppColors.primary,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 16),
          ],

          // Map placeholder
          Container(
            height: 200,
            decoration: BoxDecoration(
              color: Colors.grey.shade200,
              borderRadius: BorderRadius.circular(12),
            ),
            child: Center(
              child: Column(
                mainAxisSize: MainAxisSize.min,
                children: [
                  Icon(Icons.map, size: 48, color: Colors.grey.shade400),
                  const SizedBox(height: 8),
                  Text(
                    tracking.currentLatitude != null
                        ? 'Partner Location: ${tracking.currentLatitude!.toStringAsFixed(4)}, ${tracking.currentLongitude!.toStringAsFixed(4)}'
                        : 'Map will appear when partner is en route',
                    style: theme.textTheme.bodySmall?.copyWith(
                      color: Colors.grey.shade600,
                    ),
                  ),
                ],
              ),
            ),
          ),

          const SizedBox(height: 16),

          // Estimated delivery time
          if (tracking.estimatedDeliveryTime != null) ...[
            Card(
              shape: RoundedRectangleBorder(
                  borderRadius: BorderRadius.circular(12)),
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Row(
                  children: [
                    const Icon(Icons.access_time, color: AppColors.info),
                    const SizedBox(width: 12),
                    Text(
                      'Estimated delivery time available',
                      style: theme.textTheme.bodyMedium,
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 16),
          ],

          // Status timeline
          Card(
            shape: RoundedRectangleBorder(
                borderRadius: BorderRadius.circular(12)),
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    'Order Status',
                    style: theme.textTheme.titleSmall?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  const SizedBox(height: 12),
                  // Map delivery status to approximate order status for stepper
                  ..._buildSteps(context),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }

  List<Widget> _buildSteps(BuildContext context) {
    // Map delivery status to the order progress level
    // deliveryStatus: 0=Assigned, 1=Accepted, 2=PickedUp, 3=EnRoute, 4=Delivered
    // We show order-level steps: the delivery status maps roughly to steps 3+
    final deliveryStatus = tracking.deliveryStatus;
    // Approximate: if we have a delivery assignment at all, at least Confirmed
    int orderProgress;
    if (deliveryStatus >= 4) {
      orderProgress = 5; // Delivered
    } else if (deliveryStatus >= 2) {
      orderProgress = 4; // Out for Delivery (picked up = on the way)
    } else if (deliveryStatus >= 0) {
      orderProgress = 3; // Ready for Pickup (partner assigned)
    } else {
      orderProgress = 1;
    }

    final widgets = <Widget>[];
    for (var i = 0; i < _orderStatusSteps.length; i++) {
      final step = _orderStatusSteps[i];
      final isCompleted = orderProgress > step.$1;
      final isCurrent = orderProgress == step.$1;

      widgets.add(_StepItem(
        icon: step.$3,
        label: step.$2,
        isCompleted: isCompleted,
        isCurrent: isCurrent,
      ));

      if (i < _orderStatusSteps.length - 1) {
        widgets.add(Padding(
          padding: const EdgeInsets.only(left: 15),
          child: Container(
            width: 2,
            height: 20,
            color: isCompleted
                ? AppColors.success
                : AppColors.textTertiaryLight.withValues(alpha: 0.3),
          ),
        ));
      }
    }
    return widgets;
  }
}

class _StepItem extends StatelessWidget {
  const _StepItem({
    required this.icon,
    required this.label,
    required this.isCompleted,
    required this.isCurrent,
  });

  final IconData icon;
  final String label;
  final bool isCompleted;
  final bool isCurrent;

  @override
  Widget build(BuildContext context) {
    final color = isCompleted
        ? AppColors.success
        : isCurrent
            ? AppColors.primary
            : AppColors.textTertiaryLight;

    return Row(
      children: [
        Icon(icon, color: color, size: 30),
        const SizedBox(width: 12),
        Text(
          label,
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                color: color,
                fontWeight: isCurrent ? FontWeight.bold : FontWeight.normal,
              ),
        ),
        if (isCurrent) ...[
          const SizedBox(width: 8),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
            decoration: BoxDecoration(
              color: AppColors.primary.withValues(alpha: 0.12),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Text(
              'Current',
              style: Theme.of(context).textTheme.labelSmall?.copyWith(
                    color: AppColors.primary,
                    fontWeight: FontWeight.bold,
                  ),
            ),
          ),
        ],
      ],
    );
  }
}
