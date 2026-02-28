import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/delivery_assignment_model.dart';
import '../providers/active_delivery_notifier.dart';
import '../providers/active_delivery_state.dart';

class ActiveDeliveryScreen extends ConsumerWidget {
  const ActiveDeliveryScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(activeDeliveryNotifierProvider);

    return Scaffold(
      appBar: AppBar(title: const Text('Active Delivery')),
      body: switch (state) {
        ActiveDeliveryInitial() || ActiveDeliveryLoading() =>
          const AppLoadingWidget(message: 'Loading delivery...'),
        ActiveDeliveryError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(activeDeliveryNotifierProvider.notifier)
                .loadActiveDelivery(),
          ),
        ActiveDeliveryLoaded(:final assignment) => assignment == null
            ? const _NoActiveDelivery()
            : _ActiveDeliveryBody(assignment: assignment, ref: ref),
      },
    );
  }
}

class _NoActiveDelivery extends StatelessWidget {
  const _NoActiveDelivery();

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          Icon(Icons.delivery_dining,
              size: 64, color: AppColors.textTertiaryLight),
          const SizedBox(height: 16),
          Text(
            'No active delivery',
            style: Theme.of(context).textTheme.titleMedium?.copyWith(
                  color: AppColors.textSecondaryLight,
                ),
          ),
        ],
      ),
    );
  }
}

class _ActiveDeliveryBody extends StatelessWidget {
  const _ActiveDeliveryBody({required this.assignment, required this.ref});

  final DeliveryAssignmentModel assignment;
  final WidgetRef ref;

  static const _statusLabels = {
    0: 'Assigned',
    1: 'Accepted',
    2: 'Picked Up',
    3: 'En Route',
    4: 'Delivered',
    5: 'Cancelled',
  };

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        // Order info
        Card(
          shape:
              RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Row(
                  children: [
                    const Icon(Icons.restaurant, color: AppColors.primary),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        assignment.restaurantName,
                        style: theme.textTheme.titleMedium?.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 4),
                Text(
                  'Order #${assignment.orderNumber}',
                  style: theme.textTheme.bodySmall?.copyWith(
                    color: AppColors.textTertiaryLight,
                  ),
                ),
                if (assignment.restaurantAddress != null) ...[
                  const SizedBox(height: 8),
                  Row(
                    children: [
                      const Icon(Icons.store, size: 16, color: Colors.orange),
                      const SizedBox(width: 6),
                      Expanded(
                        child: Text(
                          'Pickup: ${assignment.restaurantAddress}',
                          style: theme.textTheme.bodySmall,
                        ),
                      ),
                    ],
                  ),
                ],
                if (assignment.customerAddress != null) ...[
                  const SizedBox(height: 4),
                  Row(
                    children: [
                      const Icon(Icons.location_on,
                          size: 16, color: AppColors.error),
                      const SizedBox(width: 6),
                      Expanded(
                        child: Text(
                          'Drop: ${assignment.customerAddress}',
                          style: theme.textTheme.bodySmall,
                        ),
                      ),
                    ],
                  ),
                ],
              ],
            ),
          ),
        ),

        const SizedBox(height: 16),

        // Status stepper
        _DeliveryStatusStepper(currentStatus: assignment.status),

        const SizedBox(height: 16),

        // Earnings
        Card(
          shape:
              RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Text('Earnings', style: theme.textTheme.titleSmall),
                Text(
                  '\u20B9${assignment.earnings ~/ 100}',
                  style: theme.textTheme.titleMedium?.copyWith(
                    fontWeight: FontWeight.bold,
                    color: AppColors.success,
                  ),
                ),
              ],
            ),
          ),
        ),

        const SizedBox(height: 24),

        // Action button
        _ActionButton(assignment: assignment, ref: ref),
      ],
    );
  }
}

class _DeliveryStatusStepper extends StatelessWidget {
  const _DeliveryStatusStepper({required this.currentStatus});

  final int currentStatus;

  static const _steps = [
    (0, 'Assigned', Icons.assignment),
    (1, 'Accepted', Icons.check_circle),
    (2, 'Picked Up', Icons.shopping_bag),
    (3, 'En Route', Icons.delivery_dining),
    (4, 'Delivered', Icons.done_all),
  ];

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Delivery Progress',
              style: theme.textTheme.titleSmall?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 12),
            for (var i = 0; i < _steps.length; i++) ...[
              _StepItem(
                icon: _steps[i].$3,
                label: _steps[i].$2,
                isCompleted: currentStatus > _steps[i].$1,
                isCurrent: currentStatus == _steps[i].$1,
              ),
              if (i < _steps.length - 1)
                Padding(
                  padding: const EdgeInsets.only(left: 15),
                  child: Container(
                    width: 2,
                    height: 24,
                    color: currentStatus > _steps[i].$1
                        ? AppColors.success
                        : AppColors.textTertiaryLight.withValues(alpha: 0.3),
                  ),
                ),
            ],
          ],
        ),
      ),
    );
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

class _ActionButton extends StatefulWidget {
  const _ActionButton({required this.assignment, required this.ref});

  final DeliveryAssignmentModel assignment;
  final WidgetRef ref;

  @override
  State<_ActionButton> createState() => _ActionButtonState();
}

class _ActionButtonState extends State<_ActionButton> {
  bool _isLoading = false;

  // Status: 0=Assigned, 1=Accepted, 2=PickedUp, 3=EnRoute, 4=Delivered
  static const _actionLabels = {
    0: 'Accept Delivery',
    1: 'Mark as Picked Up',
    2: 'Start Delivery',
    3: 'Mark as Delivered',
  };

  static const _nextStatus = {
    0: -1, // Accept uses a separate API
    1: 2,  // Accepted -> PickedUp
    2: 3,  // PickedUp -> EnRoute
    3: 4,  // EnRoute -> Delivered
  };

  @override
  Widget build(BuildContext context) {
    final status = widget.assignment.status;
    if (status >= 4) return const SizedBox.shrink();

    final label = _actionLabels[status] ?? 'Update';

    return FilledButton.icon(
      onPressed: _isLoading ? null : () => _handleAction(status),
      icon: _isLoading
          ? const SizedBox(
              width: 20,
              height: 20,
              child: CircularProgressIndicator(
                strokeWidth: 2,
                color: Colors.white,
              ),
            )
          : const Icon(Icons.arrow_forward),
      label: Text(label),
      style: FilledButton.styleFrom(
        minimumSize: const Size.fromHeight(52),
        backgroundColor:
            status == 3 ? AppColors.success : AppColors.primary,
      ),
    );
  }

  Future<void> _handleAction(int currentStatus) async {
    setState(() => _isLoading = true);
    final notifier =
        widget.ref.read(activeDeliveryNotifierProvider.notifier);

    bool success;
    if (currentStatus == 0) {
      success = await notifier.acceptDelivery(widget.assignment.id);
    } else {
      final nextStatus = _nextStatus[currentStatus];
      if (nextStatus == null) return;
      success = await notifier.updateStatus(
        widget.assignment.id,
        nextStatus,
      );
    }

    if (mounted) {
      setState(() => _isLoading = false);
      if (success && currentStatus == 3) {
        if (context.mounted) {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(
              content: Text('Delivery completed!'),
              backgroundColor: AppColors.success,
            ),
          );
          Navigator.of(context).pop();
        }
      }
    }
  }
}
