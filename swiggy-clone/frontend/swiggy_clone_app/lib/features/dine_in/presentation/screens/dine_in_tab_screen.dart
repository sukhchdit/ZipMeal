import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../providers/active_session_notifier.dart';
import '../providers/active_session_state.dart';
import '../widgets/session_info_card.dart';

class DineInTabScreen extends ConsumerWidget {
  const DineInTabScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(activeSessionNotifierProvider);
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(title: const Text('Dine-In')),
      body: switch (state) {
        ActiveSessionInitial() || ActiveSessionLoading() =>
          const AppLoadingWidget(message: 'Checking active session...'),
        ActiveSessionError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(activeSessionNotifierProvider.notifier)
                .checkActiveSession(),
          ),
        ActiveSessionNone() => _NoSessionView(theme: theme),
        ActiveSessionActive(:final session) => Column(
            children: [
              const SizedBox(height: 16),
              SessionInfoCard(session: session),
              const SizedBox(height: 24),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 16),
                child: SizedBox(
                  width: double.infinity,
                  child: FilledButton.icon(
                    onPressed: () =>
                        context.push(RouteNames.dineInSessionPath(session.id)),
                    icon: const Icon(Icons.arrow_forward),
                    label: const Text('Go to Session'),
                    style: FilledButton.styleFrom(
                      backgroundColor: AppColors.primary,
                      padding: const EdgeInsets.symmetric(vertical: 14),
                    ),
                  ),
                ),
              ),
            ],
          ),
      },
    );
  }
}

class _NoSessionView extends StatelessWidget {
  const _NoSessionView({required this.theme});

  final ThemeData theme;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(32),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(Icons.qr_code_scanner,
                size: 80, color: AppColors.textTertiaryLight),
            const SizedBox(height: 16),
            Text(
              'Scan a table QR code',
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.w600,
              ),
            ),
            const SizedBox(height: 8),
            Text(
              'Scan the QR code on your restaurant table to start a dine-in session',
              textAlign: TextAlign.center,
              style: theme.textTheme.bodyMedium?.copyWith(
                color: AppColors.textSecondaryLight,
              ),
            ),
            const SizedBox(height: 24),
            SizedBox(
              width: double.infinity,
              child: FilledButton.icon(
                onPressed: () => context.push(RouteNames.qrScanner),
                icon: const Icon(Icons.qr_code_scanner),
                label: const Text('Scan QR Code'),
                style: FilledButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  padding: const EdgeInsets.symmetric(vertical: 14),
                ),
              ),
            ),
            const SizedBox(height: 12),
            SizedBox(
              width: double.infinity,
              child: OutlinedButton.icon(
                onPressed: () => _showJoinDialog(context),
                icon: const Icon(Icons.keyboard),
                label: const Text('Enter Session Code'),
              ),
            ),
          ],
        ),
      ),
    );
  }

  void _showJoinDialog(BuildContext context) {
    final controller = TextEditingController();
    showDialog(
      context: context,
      builder: (ctx) => Consumer(
        builder: (ctx, ref, _) => AlertDialog(
          title: const Text('Join Session'),
          content: TextField(
            controller: controller,
            textCapitalization: TextCapitalization.characters,
            maxLength: 10,
            decoration: const InputDecoration(
              hintText: 'Enter 6-digit code',
              border: OutlineInputBorder(),
            ),
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(ctx).pop(),
              child: const Text('Cancel'),
            ),
            FilledButton(
              onPressed: () async {
                final code = controller.text.trim();
                if (code.isEmpty) return;
                Navigator.of(ctx).pop();
                final result = await ref
                    .read(activeSessionNotifierProvider.notifier)
                    .joinSession(sessionCode: code);
                if (result.success && result.session != null && context.mounted) {
                  context.push(
                      RouteNames.dineInSessionPath(result.session!.id));
                }
              },
              child: const Text('Join'),
            ),
          ],
        ),
      ),
    );
  }
}
