import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/dine_in_session_model.dart';
import '../providers/dine_in_session_notifier.dart';
import '../providers/dine_in_session_state.dart';
import '../providers/dine_in_websocket_notifier.dart';
import '../widgets/dine_in_order_card.dart';
import '../widgets/member_list_widget.dart';
import '../widgets/session_code_share_widget.dart';

class DineInSessionScreen extends ConsumerStatefulWidget {
  const DineInSessionScreen({required this.sessionId, super.key});

  final String sessionId;

  @override
  ConsumerState<DineInSessionScreen> createState() =>
      _DineInSessionScreenState();
}

class _DineInSessionScreenState extends ConsumerState<DineInSessionScreen> {
  @override
  void initState() {
    super.initState();
    // Initialize WebSocket connection
    ref.listenManual(
      dineInWebSocketNotifierProvider(widget.sessionId),
      (_, __) {},
    );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(dineInSessionNotifierProvider(widget.sessionId));
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: const Text('Dine-In Session'),
        actions: [
          IconButton(
            onPressed: () => ref
                .read(dineInSessionNotifierProvider(widget.sessionId).notifier)
                .loadSession(),
            icon: const Icon(Icons.refresh),
          ),
        ],
      ),
      body: switch (state) {
        DineInSessionInitial() || DineInSessionLoading() =>
          const AppLoadingWidget(message: 'Loading session...'),
        DineInSessionError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(
                    dineInSessionNotifierProvider(widget.sessionId).notifier)
                .loadSession(),
          ),
        DineInSessionEnded() => Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                const Icon(Icons.check_circle,
                    size: 64, color: Colors.green),
                const SizedBox(height: 16),
                Text('Session Ended',
                    style: theme.textTheme.titleLarge),
                const SizedBox(height: 24),
                FilledButton(
                  onPressed: () => context.go(RouteNames.dineIn),
                  child: const Text('Back to Dine-In'),
                ),
              ],
            ),
          ),
        DineInSessionLoaded(:final session, :final isHost) =>
          _SessionBody(
            session: session,
            isHost: isHost,
            sessionId: widget.sessionId,
          ),
      },
    );
  }
}

class _SessionBody extends ConsumerWidget {
  const _SessionBody({
    required this.session,
    required this.isHost,
    required this.sessionId,
  });

  final DineInSessionModel session;
  final bool isHost;
  final String sessionId;

  static const _sessionStatusLabels = {
    0: 'Active',
    1: 'Bill Requested',
    2: 'Payment Pending',
    3: 'Completed',
    4: 'Cancelled',
  };

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    final statusLabel = _sessionStatusLabels[session.status] ?? 'Unknown';

    return Column(
      children: [
        Expanded(
          child: RefreshIndicator(
            onRefresh: () => ref
                .read(dineInSessionNotifierProvider(sessionId).notifier)
                .loadSession(),
            child: ListView(
              children: [
                // Restaurant & Table info
                Card(
                  margin:
                      const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
                  shape: RoundedRectangleBorder(
                      borderRadius: BorderRadius.circular(12)),
                  child: Padding(
                    padding: const EdgeInsets.all(16),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          children: [
                            CircleAvatar(
                              radius: 20,
                              backgroundImage:
                                  session.restaurantLogoUrl != null
                                      ? NetworkImage(
                                          session.restaurantLogoUrl!)
                                      : null,
                              child: session.restaurantLogoUrl == null
                                  ? const Icon(Icons.restaurant, size: 20)
                                  : null,
                            ),
                            const SizedBox(width: 12),
                            Expanded(
                              child: Column(
                                crossAxisAlignment:
                                    CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    session.restaurantName,
                                    style: theme.textTheme.titleMedium
                                        ?.copyWith(
                                            fontWeight: FontWeight.w600),
                                  ),
                                  Text(
                                    'Table ${session.table.tableNumber}'
                                    '${session.table.floorSection != null ? ' \u2022 ${session.table.floorSection}' : ''}',
                                    style: theme.textTheme.bodySmall?.copyWith(
                                      color: AppColors.textSecondaryLight,
                                    ),
                                  ),
                                ],
                              ),
                            ),
                            Container(
                              padding: const EdgeInsets.symmetric(
                                  horizontal: 8, vertical: 4),
                              decoration: BoxDecoration(
                                color: session.status == 0
                                    ? Colors.green.withOpacity(0.1)
                                    : Colors.orange.withOpacity(0.1),
                                borderRadius: BorderRadius.circular(4),
                              ),
                              child: Text(
                                statusLabel,
                                style: TextStyle(
                                  color: session.status == 0
                                      ? Colors.green
                                      : Colors.orange,
                                  fontSize: 12,
                                  fontWeight: FontWeight.w600,
                                ),
                              ),
                            ),
                          ],
                        ),
                      ],
                    ),
                  ),
                ),

                // Session code
                if (session.status == 0)
                  SessionCodeShareWidget(sessionCode: session.sessionCode),

                // Members
                const SizedBox(height: 8),
                MemberListWidget(members: session.members),

                // Browse Menu button
                if (session.status == 0)
                  Padding(
                    padding: const EdgeInsets.symmetric(
                        horizontal: 16, vertical: 12),
                    child: SizedBox(
                      width: double.infinity,
                      child: FilledButton.icon(
                        onPressed: () => context.push(
                          RouteNames.dineInMenuPath(sessionId),
                          extra: {
                            'restaurantId': session.restaurantId,
                          },
                        ),
                        icon: const Icon(Icons.restaurant_menu),
                        label: const Text('Browse Menu & Order'),
                        style: FilledButton.styleFrom(
                          backgroundColor: AppColors.primary,
                          padding: const EdgeInsets.symmetric(vertical: 14),
                        ),
                      ),
                    ),
                  ),

                // Orders section
                Padding(
                  padding: const EdgeInsets.fromLTRB(16, 8, 16, 8),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.spaceBetween,
                    children: [
                      Text(
                        'Orders (${session.orders.length})',
                        style: theme.textTheme.titleSmall
                            ?.copyWith(fontWeight: FontWeight.w600),
                      ),
                      if (session.orders.length > 3)
                        TextButton(
                          onPressed: () => context.push(
                            RouteNames.dineInSessionOrdersPath(sessionId),
                          ),
                          child: const Text('View All'),
                        ),
                    ],
                  ),
                ),
                if (session.orders.isEmpty)
                  Padding(
                    padding: const EdgeInsets.symmetric(
                        horizontal: 16, vertical: 8),
                    child: Text(
                      'No orders yet. Browse the menu to place an order.',
                      style: theme.textTheme.bodyMedium?.copyWith(
                        color: AppColors.textTertiaryLight,
                      ),
                    ),
                  )
                else
                  ...session.orders.take(3).map(
                        (order) => DineInOrderCard(order: order),
                      ),
                const SizedBox(height: 80),
              ],
            ),
          ),
        ),

        // Bottom action bar
        SafeArea(
          child: Container(
            padding: const EdgeInsets.all(16),
            decoration: BoxDecoration(
              color: Colors.white,
              boxShadow: [
                BoxShadow(
                  color: Colors.black.withOpacity(0.05),
                  blurRadius: 8,
                  offset: const Offset(0, -2),
                ),
              ],
            ),
            child: session.status == 0
                ? isHost
                    ? SizedBox(
                        width: double.infinity,
                        child: FilledButton(
                          onPressed: () => _confirmRequestBill(context, ref),
                          style: FilledButton.styleFrom(
                            backgroundColor: AppColors.primary,
                            padding:
                                const EdgeInsets.symmetric(vertical: 14),
                          ),
                          child: const Text('Request Bill'),
                        ),
                      )
                    : SizedBox(
                        width: double.infinity,
                        child: OutlinedButton(
                          onPressed: () =>
                              _confirmLeaveSession(context, ref),
                          child: const Text('Leave Session'),
                        ),
                      )
                : session.status == 1
                    ? isHost
                        ? SizedBox(
                            width: double.infinity,
                            child: FilledButton(
                              onPressed: () =>
                                  _confirmEndSession(context, ref),
                              style: FilledButton.styleFrom(
                                backgroundColor: Colors.green,
                                padding: const EdgeInsets.symmetric(
                                    vertical: 14),
                              ),
                              child: const Text('End Session'),
                            ),
                          )
                        : const SizedBox.shrink()
                    : const SizedBox.shrink(),
          ),
        ),
      ],
    );
  }

  void _confirmRequestBill(BuildContext context, WidgetRef ref) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Request Bill'),
        content: const Text(
            'Are you sure you want to request the bill? No more orders can be placed after this.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () {
              Navigator.of(ctx).pop();
              ref
                  .read(dineInSessionNotifierProvider(sessionId).notifier)
                  .requestBill();
            },
            child: const Text('Request Bill'),
          ),
        ],
      ),
    );
  }

  void _confirmLeaveSession(BuildContext context, WidgetRef ref) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Leave Session'),
        content:
            const Text('Are you sure you want to leave this dine-in session?'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () {
              Navigator.of(ctx).pop();
              ref
                  .read(dineInSessionNotifierProvider(sessionId).notifier)
                  .leaveSession();
            },
            child: const Text('Leave'),
          ),
        ],
      ),
    );
  }

  void _confirmEndSession(BuildContext context, WidgetRef ref) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('End Session'),
        content:
            const Text('This will mark the session as complete and free the table.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () {
              Navigator.of(ctx).pop();
              ref
                  .read(dineInSessionNotifierProvider(sessionId).notifier)
                  .endSession();
            },
            child: const Text('End Session'),
          ),
        ],
      ),
    );
  }
}
