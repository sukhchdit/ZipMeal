import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/extensions/l10n_extensions.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/dispute_model.dart';
import '../providers/dispute_detail_notifier.dart';
import '../providers/dispute_detail_state.dart';
import '../widgets/dispute_message_bubble.dart';

class DisputeDetailScreen extends ConsumerStatefulWidget {
  const DisputeDetailScreen({super.key, required this.disputeId});

  final String disputeId;

  @override
  ConsumerState<DisputeDetailScreen> createState() =>
      _DisputeDetailScreenState();
}

class _DisputeDetailScreenState extends ConsumerState<DisputeDetailScreen> {
  final _messageController = TextEditingController();

  @override
  void initState() {
    super.initState();
    Future.microtask(() => ref
        .read(disputeDetailNotifierProvider(widget.disputeId).notifier)
        .load());
  }

  @override
  void dispose() {
    _messageController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final state =
        ref.watch(disputeDetailNotifierProvider(widget.disputeId));
    final l10n = context.l10n;

    return Scaffold(
      appBar: AppBar(
        title: switch (state) {
          DisputeDetailLoaded(:final dispute) =>
            Text('${l10n.disputes} #${dispute.disputeNumber}'),
          _ => Text(l10n.disputes),
        },
      ),
      body: switch (state) {
        DisputeDetailInitial() ||
        DisputeDetailLoading() =>
          const AppLoadingWidget(),
        DisputeDetailError(:final message) => AppErrorWidget(
            message: message,
            onRetry: () => ref
                .read(
                    disputeDetailNotifierProvider(widget.disputeId).notifier)
                .load(),
          ),
        DisputeDetailLoaded(
          :final dispute,
          :final messages,
          :final isSending,
        ) =>
          Column(
            children: [
              // Status & Info header
              _DisputeInfoHeader(dispute: dispute),
              const Divider(height: 1),

              // Messages list
              Expanded(
                child: ListView.builder(
                  reverse: true,
                  padding: const EdgeInsets.symmetric(vertical: 8),
                  itemCount: messages.length,
                  itemBuilder: (context, index) {
                    final msg = messages[index];
                    return DisputeMessageBubble(
                      message: msg,
                      isCurrentUser:
                          msg.senderId == dispute.userId &&
                              !msg.isSystemMessage,
                    );
                  },
                ),
              ),

              // Input bar (disabled for terminal states)
              if (dispute.status != 3 &&
                  dispute.status != 4 &&
                  dispute.status != 6)
                _MessageInput(
                  controller: _messageController,
                  isSending: isSending,
                  onSend: () {
                    final text = _messageController.text.trim();
                    if (text.isEmpty) return;
                    ref
                        .read(disputeDetailNotifierProvider(widget.disputeId)
                            .notifier)
                        .sendMessage(text);
                    _messageController.clear();
                  },
                ),
            ],
          ),
      },
    );
  }
}

class _DisputeInfoHeader extends StatelessWidget {
  const _DisputeInfoHeader({required this.dispute});

  final DisputeModel dispute;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final l10n = context.l10n;

    final statusColor = switch (dispute.status) {
      0 => Colors.blue,
      1 => Colors.orange,
      2 => Colors.amber.shade700,
      3 => Colors.green,
      4 => Colors.grey,
      5 => Colors.red,
      6 => Colors.red.shade300,
      _ => Colors.grey,
    };

    final statusLabel = switch (dispute.status) {
      0 => l10n.disputeOpened,
      1 => l10n.disputeUnderReview,
      2 => l10n.disputeAwaitingResponse,
      3 => l10n.disputeStatusResolved,
      4 => l10n.disputeStatusClosed,
      5 => l10n.disputeStatusEscalated,
      6 => l10n.disputeStatusRejected,
      _ => 'Unknown',
    };

    final issueLabel = switch (dispute.issueType) {
      0 => l10n.issueWrongItems,
      1 => l10n.issueMissingItems,
      2 => l10n.issueQualityIssue,
      3 => l10n.issueLateDelivery,
      4 => l10n.issueNeverDelivered,
      5 => l10n.issueWrongOrder,
      6 => l10n.issueDamagedPackaging,
      _ => l10n.issueOther,
    };

    return Container(
      color: statusColor.withValues(alpha: 0.08),
      padding: const EdgeInsets.all(16),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            children: [
              Container(
                padding:
                    const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                decoration: BoxDecoration(
                  color: statusColor.withValues(alpha: 0.2),
                  borderRadius: BorderRadius.circular(12),
                ),
                child: Text(
                  statusLabel,
                  style: theme.textTheme.labelMedium?.copyWith(
                    color: statusColor,
                    fontWeight: FontWeight.bold,
                  ),
                ),
              ),
              const SizedBox(width: 8),
              Text(issueLabel, style: theme.textTheme.bodyMedium),
            ],
          ),
          if (dispute.orderNumber != null && dispute.orderNumber!.isNotEmpty) ...[
            const SizedBox(height: 6),
            Text('Order: ${dispute.orderNumber}',
                style: theme.textTheme.bodySmall),
          ],
          // Resolution card
          if (dispute.status == 3 && dispute.resolutionType != null) ...[
            const SizedBox(height: 12),
            Card(
              child: Padding(
                padding: const EdgeInsets.all(12),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(l10n.resolutionDetails,
                        style: theme.textTheme.titleSmall
                            ?.copyWith(fontWeight: FontWeight.bold)),
                    const SizedBox(height: 4),
                    if (dispute.resolutionAmountPaise != null)
                      Text(
                          'Amount: \u20b9${(dispute.resolutionAmountPaise! / 100).toStringAsFixed(2)}'),
                    if (dispute.resolutionNotes != null)
                      Text(dispute.resolutionNotes!),
                  ],
                ),
              ),
            ),
          ],
          // Rejection card
          if (dispute.status == 6 && dispute.rejectionReason != null) ...[
            const SizedBox(height: 12),
            Card(
              color: Colors.red.shade50,
              child: Padding(
                padding: const EdgeInsets.all(12),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(l10n.rejectionReason,
                        style: theme.textTheme.titleSmall?.copyWith(
                            fontWeight: FontWeight.bold,
                            color: Colors.red.shade700)),
                    const SizedBox(height: 4),
                    Text(dispute.rejectionReason!),
                  ],
                ),
              ),
            ),
          ],
        ],
      ),
    );
  }
}

class _MessageInput extends StatelessWidget {
  const _MessageInput({
    required this.controller,
    required this.isSending,
    required this.onSend,
  });

  final TextEditingController controller;
  final bool isSending;
  final VoidCallback onSend;

  @override
  Widget build(BuildContext context) => Container(
        padding: EdgeInsets.only(
          left: 16,
          right: 8,
          top: 8,
          bottom: MediaQuery.paddingOf(context).bottom + 8,
        ),
        decoration: BoxDecoration(
          color: Theme.of(context).colorScheme.surface,
          boxShadow: [
            BoxShadow(
              color: Colors.black.withValues(alpha: 0.05),
              blurRadius: 4,
              offset: const Offset(0, -2),
            ),
          ],
        ),
        child: Row(
          children: [
            Expanded(
              child: TextField(
                controller: controller,
                maxLines: 3,
                minLines: 1,
                textInputAction: TextInputAction.send,
                onSubmitted: (_) => onSend(),
                decoration: InputDecoration(
                  hintText: context.l10n.typeMessage,
                  border: OutlineInputBorder(
                    borderRadius: BorderRadius.circular(24),
                  ),
                  contentPadding: const EdgeInsets.symmetric(
                      horizontal: 16, vertical: 10),
                  isDense: true,
                ),
              ),
            ),
            const SizedBox(width: 8),
            IconButton.filled(
              onPressed: isSending ? null : onSend,
              icon: isSending
                  ? const SizedBox(
                      height: 18,
                      width: 18,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Icon(Icons.send),
            ),
          ],
        ),
      );
}
