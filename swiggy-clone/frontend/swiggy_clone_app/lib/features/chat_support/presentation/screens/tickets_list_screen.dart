import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/support_ticket_model.dart';
import '../providers/tickets_notifier.dart';
import '../providers/tickets_state.dart';

class TicketsListScreen extends ConsumerWidget {
  const TicketsListScreen({super.key});

  static const _statusLabels = ['Open', 'In Progress', 'Waiting', 'Resolved', 'Closed'];
  static const _categoryLabels = [
    'General',
    'Order Issue',
    'Payment Issue',
    'Delivery Issue',
    'Account Issue',
    'Other',
  ];

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(ticketsNotifierProvider);

    return Scaffold(
      appBar: AppBar(title: const Text('Support Tickets')),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => context.go(RouteNames.newChatTicket),
        icon: const Icon(Icons.add),
        label: const Text('New Ticket'),
      ),
      body: switch (state) {
        TicketsInitial() || TicketsLoading() => const Center(
            child: CircularProgressIndicator(),
          ),
        TicketsError(:final failure) => Center(
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Text(failure.message),
                const SizedBox(height: 16),
                ElevatedButton(
                  onPressed: () =>
                      ref.read(ticketsNotifierProvider.notifier).loadTickets(),
                  child: const Text('Retry'),
                ),
              ],
            ),
          ),
        TicketsLoaded(:final tickets, :final hasMore, :final isLoadingMore) =>
          tickets.isEmpty
              ? _EmptyState(
                  onCreateTap: () => context.go(RouteNames.newChatTicket),
                )
              : RefreshIndicator(
                  onRefresh: () =>
                      ref.read(ticketsNotifierProvider.notifier).loadTickets(),
                  child: ListView.builder(
                    padding: const EdgeInsets.symmetric(vertical: 8),
                    itemCount: tickets.length + (hasMore ? 1 : 0),
                    itemBuilder: (context, index) {
                      if (index == tickets.length) {
                        if (!isLoadingMore) {
                          ref
                              .read(ticketsNotifierProvider.notifier)
                              .loadMore();
                        }
                        return const Center(
                          child: Padding(
                            padding: EdgeInsets.all(16),
                            child: CircularProgressIndicator(),
                          ),
                        );
                      }
                      return _TicketCard(
                        ticket: tickets[index],
                        statusLabel: _statusLabels[tickets[index].status.clamp(0, 4)],
                        categoryLabel: _categoryLabels[tickets[index].category.clamp(0, 5)],
                      );
                    },
                  ),
                ),
      },
    );
  }
}

class _TicketCard extends StatelessWidget {
  const _TicketCard({
    required this.ticket,
    required this.statusLabel,
    required this.categoryLabel,
  });

  final SupportTicketSummaryModel ticket;
  final String statusLabel;
  final String categoryLabel;

  Color _statusColor(int status) {
    return switch (status) {
      0 => Colors.blue,
      1 => Colors.orange,
      2 => Colors.amber,
      3 => Colors.green,
      4 => AppColors.textTertiaryLight,
      _ => AppColors.textSecondaryLight,
    };
  }

  @override
  Widget build(BuildContext context) {
    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      child: InkWell(
        onTap: () => context.go(RouteNames.chatConversationPath(ticket.id)),
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
                      ticket.subject,
                      style: Theme.of(context).textTheme.titleSmall?.copyWith(
                            fontWeight: FontWeight.w600,
                          ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                  if (ticket.unreadCount > 0)
                    Container(
                      padding: const EdgeInsets.symmetric(
                          horizontal: 8, vertical: 2),
                      decoration: BoxDecoration(
                        color: AppColors.primary,
                        borderRadius: BorderRadius.circular(12),
                      ),
                      child: Text(
                        '${ticket.unreadCount}',
                        style: const TextStyle(
                          color: Colors.white,
                          fontSize: 12,
                          fontWeight: FontWeight.bold,
                        ),
                      ),
                    ),
                ],
              ),
              const SizedBox(height: 8),
              Row(
                children: [
                  Container(
                    padding:
                        const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
                    decoration: BoxDecoration(
                      color: _statusColor(ticket.status).withAlpha(25),
                      borderRadius: BorderRadius.circular(8),
                    ),
                    child: Text(
                      statusLabel,
                      style: TextStyle(
                        color: _statusColor(ticket.status),
                        fontSize: 12,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Text(
                    categoryLabel,
                    style: Theme.of(context).textTheme.bodySmall?.copyWith(
                          color: AppColors.textTertiaryLight,
                        ),
                  ),
                ],
              ),
              if (ticket.lastMessage != null) ...[
                const SizedBox(height: 8),
                Text(
                  ticket.lastMessage!,
                  maxLines: 1,
                  overflow: TextOverflow.ellipsis,
                  style: Theme.of(context).textTheme.bodySmall?.copyWith(
                        color: AppColors.textSecondaryLight,
                      ),
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }
}

class _EmptyState extends StatelessWidget {
  const _EmptyState({required this.onCreateTap});

  final VoidCallback onCreateTap;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(
              Icons.support_agent_outlined,
              size: 64,
              color: AppColors.textTertiaryLight,
            ),
            const SizedBox(height: 16),
            Text(
              'No support tickets yet',
              style: Theme.of(context).textTheme.titleMedium,
            ),
            const SizedBox(height: 8),
            Text(
              'Need help? Create a ticket and our team will assist you.',
              textAlign: TextAlign.center,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    color: AppColors.textSecondaryLight,
                  ),
            ),
            const SizedBox(height: 24),
            ElevatedButton.icon(
              onPressed: onCreateTap,
              icon: const Icon(Icons.add),
              label: const Text('Create Ticket'),
            ),
          ],
        ),
      ),
    );
  }
}
