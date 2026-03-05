import 'package:flutter/material.dart';

import '../../../../core/extensions/l10n_extensions.dart';
import '../../data/models/dispute_model.dart';

class DisputeCard extends StatelessWidget {
  const DisputeCard({
    super.key,
    required this.dispute,
    required this.onTap,
  });

  final DisputeSummaryModel dispute;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(12),
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      dispute.disputeNumber,
                      style: theme.textTheme.titleSmall?.copyWith(
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ),
                  _StatusChip(status: dispute.status),
                ],
              ),
              const SizedBox(height: 8),
              Row(
                children: [
                  Icon(
                    _issueTypeIcon(dispute.issueType),
                    size: 16,
                    color: theme.colorScheme.onSurfaceVariant,
                  ),
                  const SizedBox(width: 6),
                  Text(
                    _issueTypeLabel(context, dispute.issueType),
                    style: theme.textTheme.bodyMedium?.copyWith(
                      color: theme.colorScheme.onSurfaceVariant,
                    ),
                  ),
                ],
              ),
              if (dispute.orderNumber != null) ...[
                const SizedBox(height: 4),
                Text(
                  'Order: ${dispute.orderNumber}',
                  style: theme.textTheme.bodySmall?.copyWith(
                    color: theme.colorScheme.onSurfaceVariant,
                  ),
                ),
              ],
              if (dispute.lastMessage != null) ...[
                const SizedBox(height: 8),
                Text(
                  dispute.lastMessage!,
                  maxLines: 2,
                  overflow: TextOverflow.ellipsis,
                  style: theme.textTheme.bodySmall,
                ),
              ],
              const SizedBox(height: 8),
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text(
                    _formatDate(dispute.createdAt),
                    style: theme.textTheme.labelSmall?.copyWith(
                      color: theme.colorScheme.onSurfaceVariant,
                    ),
                  ),
                  if (dispute.unreadCount > 0)
                    Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 8, vertical: 2),
                      decoration: BoxDecoration(
                        color: theme.colorScheme.primary,
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Text(
                        '${dispute.unreadCount}',
                        style: theme.textTheme.labelSmall?.copyWith(
                          color: theme.colorScheme.onPrimary,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                ],
              ),
            ],
          ),
        ),
      ),
    );
  }

  String _formatDate(String iso) {
    try {
      final dt = DateTime.parse(iso);
      return '${dt.day}/${dt.month}/${dt.year}';
    } catch (_) {
      return iso;
    }
  }

  static IconData _issueTypeIcon(int type) => switch (type) {
        0 => Icons.swap_horiz,
        1 => Icons.remove_shopping_cart,
        2 => Icons.thumb_down_outlined,
        3 => Icons.access_time,
        4 => Icons.cancel_outlined,
        5 => Icons.shuffle,
        6 => Icons.broken_image_outlined,
        _ => Icons.help_outline,
      };

  static String _issueTypeLabel(BuildContext context, int type) {
    final l10n = context.l10n;
    return switch (type) {
      0 => l10n.issueWrongItems,
      1 => l10n.issueMissingItems,
      2 => l10n.issueQualityIssue,
      3 => l10n.issueLateDelivery,
      4 => l10n.issueNeverDelivered,
      5 => l10n.issueWrongOrder,
      6 => l10n.issueDamagedPackaging,
      _ => l10n.issueOther,
    };
  }
}

class _StatusChip extends StatelessWidget {
  const _StatusChip({required this.status});

  final int status;

  @override
  Widget build(BuildContext context) {
    final (label, color) = _statusInfo(context, status);
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(
        color: color.withValues(alpha: 0.15),
        borderRadius: BorderRadius.circular(12),
      ),
      child: Text(
        label,
        style: Theme.of(context).textTheme.labelSmall?.copyWith(
              color: color,
              fontWeight: FontWeight.bold,
            ),
      ),
    );
  }

  static (String, Color) _statusInfo(BuildContext context, int status) {
    final l10n = context.l10n;
    return switch (status) {
      0 => (l10n.disputeOpened, Colors.blue),
      1 => (l10n.disputeUnderReview, Colors.orange),
      2 => (l10n.disputeAwaitingResponse, Colors.amber.shade700),
      3 => (l10n.disputeStatusResolved, Colors.green),
      4 => (l10n.disputeStatusClosed, Colors.grey),
      5 => (l10n.disputeStatusEscalated, Colors.red),
      6 => (l10n.disputeStatusRejected, Colors.red.shade300),
      _ => ('Unknown', Colors.grey),
    };
  }
}
