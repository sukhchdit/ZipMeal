import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/extensions/l10n_extensions.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/group_order_model.dart';
import '../providers/active_group_order_notifier.dart';
import '../providers/group_order_notifier.dart';
import '../providers/group_order_state.dart';
import '../providers/group_order_websocket_notifier.dart';
import '../widgets/invite_code_share_widget.dart';

class GroupOrderLobbyScreen extends ConsumerStatefulWidget {
  const GroupOrderLobbyScreen({required this.groupOrderId, super.key});
  final String groupOrderId;

  @override
  ConsumerState<GroupOrderLobbyScreen> createState() =>
      _GroupOrderLobbyScreenState();
}

class _GroupOrderLobbyScreenState
    extends ConsumerState<GroupOrderLobbyScreen> {
  Timer? _countdownTimer;
  Duration _remaining = Duration.zero;

  @override
  void initState() {
    super.initState();
    ref.listenManual(
      groupOrderWebSocketNotifierProvider(widget.groupOrderId),
      (_, __) {},
    );
  }

  @override
  void dispose() {
    _countdownTimer?.cancel();
    super.dispose();
  }

  void _startCountdown(String expiresAtStr) {
    final expiresAt = DateTime.tryParse(expiresAtStr);
    if (expiresAt == null) return;

    _countdownTimer?.cancel();
    _countdownTimer = Timer.periodic(const Duration(seconds: 1), (_) {
      final remaining = expiresAt.difference(DateTime.now().toUtc());
      if (remaining.isNegative) {
        _countdownTimer?.cancel();
      }
      if (mounted) setState(() => _remaining = remaining);
    });
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(
      groupOrderNotifierProvider(widget.groupOrderId),
    );
    final l10n = context.l10n;

    return Scaffold(
      appBar: AppBar(title: Text(l10n.groupOrderLobby)),
      body: switch (state) {
        GroupOrderInitial() || GroupOrderLoading() =>
          const AppLoadingWidget(message: 'Loading group order...'),
        GroupOrderError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(
                    groupOrderNotifierProvider(widget.groupOrderId).notifier)
                .loadGroupOrder(),
          ),
        GroupOrderFinalized(:final orderId) => _FinalizedView(orderId: orderId),
        GroupOrderCancelled() => Center(child: Text(l10n.groupOrderCancelled)),
        GroupOrderExpired() => Center(child: Text(l10n.groupOrderExpired)),
        GroupOrderLoaded(:final groupOrder, :final isInitiator) => _LobbyBody(
            groupOrder: groupOrder,
            isInitiator: isInitiator,
            groupOrderId: widget.groupOrderId,
            remaining: _remaining,
            onStartCountdown: _startCountdown,
          ),
      },
    );
  }
}

class _LobbyBody extends ConsumerStatefulWidget {
  const _LobbyBody({
    required this.groupOrder,
    required this.isInitiator,
    required this.groupOrderId,
    required this.remaining,
    required this.onStartCountdown,
  });

  final GroupOrderModel groupOrder;
  final bool isInitiator;
  final String groupOrderId;
  final Duration remaining;
  final void Function(String) onStartCountdown;

  @override
  ConsumerState<_LobbyBody> createState() => _LobbyBodyState();
}

class _LobbyBodyState extends ConsumerState<_LobbyBody> {
  @override
  void initState() {
    super.initState();
    widget.onStartCountdown(widget.groupOrder.expiresAt);
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final l10n = context.l10n;
    final go = widget.groupOrder;
    final activeParticipants =
        go.participants.where((p) => p.status != 2).toList();
    final allReady = activeParticipants
        .where((p) => !p.isInitiator)
        .every((p) => p.status == 1);

    final minutes = widget.remaining.inMinutes;
    final seconds = widget.remaining.inSeconds % 60;
    final timeStr = '${minutes.toString().padLeft(2, '0')}:'
        '${seconds.toString().padLeft(2, '0')}';

    return Column(
      children: [
        InviteCodeShareWidget(inviteCode: go.inviteCode),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(
                '${l10n.groupOrderExpiresIn} $timeStr',
                style: theme.textTheme.bodySmall?.copyWith(
                  color: widget.remaining.inMinutes < 5
                      ? theme.colorScheme.error
                      : theme.colorScheme.onSurfaceVariant,
                ),
              ),
              Text(
                allReady ? l10n.groupOrderAllReady : l10n.groupOrderWaitingForReady,
                style: theme.textTheme.bodySmall?.copyWith(
                  color: allReady
                      ? Colors.green
                      : theme.colorScheme.onSurfaceVariant,
                ),
              ),
            ],
          ),
        ),
        Padding(
          padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
          child: Text(
            '${l10n.groupOrderParticipants} (${activeParticipants.length})',
            style: theme.textTheme.titleMedium,
          ),
        ),
        Expanded(
          child: ListView.builder(
            itemCount: activeParticipants.length,
            padding: const EdgeInsets.symmetric(horizontal: 16),
            itemBuilder: (context, index) {
              final p = activeParticipants[index];
              return ListTile(
                leading: CircleAvatar(
                  backgroundImage:
                      p.avatarUrl != null ? NetworkImage(p.avatarUrl!) : null,
                  child: p.avatarUrl == null
                      ? Text(p.userName.isNotEmpty ? p.userName[0] : '?')
                      : null,
                ),
                title: Row(
                  children: [
                    Text(p.userName),
                    if (p.isInitiator) ...[
                      const SizedBox(width: 4),
                      Chip(
                        label: Text(l10n.groupOrderHostLabel),
                        materialTapTargetSize: MaterialTapTargetSize.shrinkWrap,
                        visualDensity: VisualDensity.compact,
                      ),
                    ],
                  ],
                ),
                subtitle: Text(
                  '${l10n.groupOrderItemsCount}: ${p.itemCount}',
                ),
                trailing: _StatusBadge(status: p.status),
              );
            },
          ),
        ),
        Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              OutlinedButton.icon(
                onPressed: () {
                  context.go(
                    RouteNames.groupOrderMenuPath(
                      widget.groupOrderId,
                      go.restaurantId,
                    ),
                  );
                },
                icon: const Icon(Icons.restaurant_menu),
                label: Text(l10n.groupOrderMenu),
              ),
              const SizedBox(height: 8),
              if (widget.isInitiator)
                Row(
                  children: [
                    Expanded(
                      child: OutlinedButton(
                        onPressed: () => _confirmCancel(context),
                        child: Text(l10n.groupOrderCancel),
                      ),
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      flex: 2,
                      child: FilledButton(
                        onPressed: allReady
                            ? () => context.go(
                                  RouteNames.groupOrderCheckoutPath(
                                      widget.groupOrderId),
                                )
                            : null,
                        child: Text(l10n.groupOrderFinalize),
                      ),
                    ),
                  ],
                )
              else
                Row(
                  children: [
                    Expanded(
                      child: OutlinedButton(
                        onPressed: () => _confirmLeave(context),
                        child: Text(l10n.groupOrderLeave),
                      ),
                    ),
                    const SizedBox(width: 8),
                    Expanded(
                      flex: 2,
                      child: FilledButton(
                        onPressed: () async {
                          await ref
                              .read(groupOrderNotifierProvider(
                                      widget.groupOrderId)
                                  .notifier)
                              .markReady();
                        },
                        child: Text(l10n.groupOrderMarkReady),
                      ),
                    ),
                  ],
                ),
            ],
          ),
        ),
      ],
    );
  }

  Future<void> _confirmCancel(BuildContext context) async {
    final l10n = context.l10n;
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text(l10n.groupOrderConfirmCancel),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('No'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Yes'),
          ),
        ],
      ),
    );
    if (confirmed == true && mounted) {
      await ref
          .read(groupOrderNotifierProvider(widget.groupOrderId).notifier)
          .cancelGroupOrder();
      ref
          .read(activeGroupOrderNotifierProvider.notifier)
          .checkActiveGroupOrder();
    }
  }

  Future<void> _confirmLeave(BuildContext context) async {
    final l10n = context.l10n;
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text(l10n.groupOrderConfirmLeave),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('No'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Yes'),
          ),
        ],
      ),
    );
    if (confirmed == true && mounted) {
      final success = await ref
          .read(groupOrderNotifierProvider(widget.groupOrderId).notifier)
          .leaveGroupOrder();
      if (success && mounted) {
        ref
            .read(activeGroupOrderNotifierProvider.notifier)
            .checkActiveGroupOrder();
        context.go('/');
      }
    }
  }
}

class _StatusBadge extends StatelessWidget {
  const _StatusBadge({required this.status});
  final int status;

  @override
  Widget build(BuildContext context) {
    final (label, color) = switch (status) {
      0 => ('Joined', Colors.blue),
      1 => ('Ready', Colors.green),
      2 => ('Left', Colors.grey),
      _ => ('Unknown', Colors.grey),
    };

    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.15),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Text(
        label,
        style: TextStyle(color: color, fontWeight: FontWeight.w600, fontSize: 12),
      ),
    );
  }
}

class _FinalizedView extends StatelessWidget {
  const _FinalizedView({required this.orderId});
  final String orderId;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final l10n = context.l10n;

    return Center(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          const Icon(Icons.check_circle, size: 64, color: Colors.green),
          const SizedBox(height: 16),
          Text(l10n.groupOrderFinalized, style: theme.textTheme.headlineSmall),
          const SizedBox(height: 24),
          FilledButton(
            onPressed: () => context.go('/orders/$orderId'),
            child: const Text('View Order'),
          ),
        ],
      ),
    );
  }
}
