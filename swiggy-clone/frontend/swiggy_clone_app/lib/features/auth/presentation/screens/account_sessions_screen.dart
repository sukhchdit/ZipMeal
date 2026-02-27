import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/session_model.dart';
import '../../data/repositories/auth_repository.dart';
import '../providers/auth_notifier.dart';

class AccountSessionsScreen extends ConsumerStatefulWidget {
  const AccountSessionsScreen({super.key});

  @override
  ConsumerState<AccountSessionsScreen> createState() =>
      _AccountSessionsScreenState();
}

class _AccountSessionsScreenState
    extends ConsumerState<AccountSessionsScreen> {
  List<SessionModel>? _sessions;
  bool _isLoading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadSessions();
  }

  Future<void> _loadSessions() async {
    setState(() {
      _isLoading = true;
      _error = null;
    });

    final result = await ref.read(authRepositoryProvider).getSessions();

    if (!mounted) return;
    setState(() {
      _isLoading = false;
      if (result.failure != null) {
        _error = result.failure!.message;
      } else {
        _sessions = result.data;
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final dateFormat = DateFormat('MMM d, yyyy HH:mm');

    return Scaffold(
      appBar: AppBar(
        title: const Text('Active Sessions'),
        actions: [
          if (_sessions != null && _sessions!.length > 1)
            TextButton(
              onPressed: _revokeAll,
              child: const Text(
                'Revoke All',
                style: TextStyle(color: AppColors.error),
              ),
            ),
        ],
      ),
      body: _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _error != null
              ? Center(
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      const Icon(Icons.error_outline,
                          size: 48, color: AppColors.error),
                      const SizedBox(height: 12),
                      Text(_error!, style: theme.textTheme.bodyLarge),
                      const SizedBox(height: 16),
                      FilledButton(
                        onPressed: _loadSessions,
                        child: const Text('Retry'),
                      ),
                    ],
                  ),
                )
              : _sessions == null || _sessions!.isEmpty
                  ? const Center(
                      child: Text('No active sessions'),
                    )
                  : RefreshIndicator(
                      onRefresh: _loadSessions,
                      child: ListView.separated(
                        padding: const EdgeInsets.all(16),
                        itemCount: _sessions!.length,
                        separatorBuilder: (_, __) => const SizedBox(height: 12),
                        itemBuilder: (context, index) {
                          final session = _sessions![index];
                          return Card(
                            child: Padding(
                              padding: const EdgeInsets.all(16),
                              child: Row(
                                children: [
                                  Icon(
                                    _deviceIcon(session.deviceInfo),
                                    size: 32,
                                    color: session.isCurrent
                                        ? AppColors.primary
                                        : AppColors.textSecondaryLight,
                                  ),
                                  const SizedBox(width: 16),
                                  Expanded(
                                    child: Column(
                                      crossAxisAlignment:
                                          CrossAxisAlignment.start,
                                      children: [
                                        Row(
                                          children: [
                                            Expanded(
                                              child: Text(
                                                session.deviceInfo ??
                                                    'Unknown Device',
                                                style: theme
                                                    .textTheme.bodyMedium
                                                    ?.copyWith(
                                                  fontWeight: FontWeight.w600,
                                                ),
                                                maxLines: 1,
                                                overflow: TextOverflow.ellipsis,
                                              ),
                                            ),
                                            if (session.isCurrent)
                                              Container(
                                                padding:
                                                    const EdgeInsets.symmetric(
                                                  horizontal: 8,
                                                  vertical: 2,
                                                ),
                                                decoration: BoxDecoration(
                                                  color: AppColors.primary
                                                      .withValues(alpha: 0.15),
                                                  borderRadius:
                                                      BorderRadius.circular(12),
                                                ),
                                                child: const Text(
                                                  'Current',
                                                  style: TextStyle(
                                                    fontSize: 11,
                                                    color: AppColors.primary,
                                                    fontWeight: FontWeight.w600,
                                                  ),
                                                ),
                                              ),
                                          ],
                                        ),
                                        const SizedBox(height: 4),
                                        Text(
                                          'Created: ${dateFormat.format(session.createdAt)}',
                                          style: theme.textTheme.bodySmall
                                              ?.copyWith(
                                            color:
                                                AppColors.textSecondaryLight,
                                          ),
                                        ),
                                        Text(
                                          'Expires: ${dateFormat.format(session.expiresAt)}',
                                          style: theme.textTheme.bodySmall
                                              ?.copyWith(
                                            color:
                                                AppColors.textSecondaryLight,
                                          ),
                                        ),
                                      ],
                                    ),
                                  ),
                                  if (!session.isCurrent)
                                    IconButton(
                                      icon: const Icon(
                                        Icons.logout,
                                        color: AppColors.error,
                                      ),
                                      onPressed: () =>
                                          _revokeSession(session),
                                      tooltip: 'Revoke',
                                    ),
                                ],
                              ),
                            ),
                          );
                        },
                      ),
                    ),
    );
  }

  IconData _deviceIcon(String? deviceInfo) {
    if (deviceInfo == null) return Icons.device_unknown;
    final lower = deviceInfo.toLowerCase();
    if (lower.contains('mobile') || lower.contains('android') || lower.contains('ios')) {
      return Icons.phone_android;
    }
    if (lower.contains('tablet') || lower.contains('ipad')) {
      return Icons.tablet;
    }
    return Icons.computer;
  }

  Future<void> _revokeSession(SessionModel session) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Revoke Session'),
        content: const Text(
          'This will log out the device associated with this session.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(ctx, true),
            style: FilledButton.styleFrom(backgroundColor: AppColors.error),
            child: const Text('Revoke'),
          ),
        ],
      ),
    );

    if (confirmed == true && mounted) {
      final failure = await ref
          .read(authRepositoryProvider)
          .revokeSession(sessionId: session.id);
      if (mounted) {
        if (failure != null) {
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(content: Text(failure.message)),
          );
        } else {
          ScaffoldMessenger.of(context).showSnackBar(
            const SnackBar(content: Text('Session revoked')),
          );
          _loadSessions();
        }
      }
    }
  }

  Future<void> _revokeAll() async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Revoke All Sessions'),
        content: const Text(
          'This will log you out from all devices, including this one. You will need to log in again.',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(ctx, true),
            style: FilledButton.styleFrom(backgroundColor: AppColors.error),
            child: const Text('Revoke All'),
          ),
        ],
      ),
    );

    if (confirmed == true && mounted) {
      await ref.read(authNotifierProvider.notifier).logoutAll();
      if (mounted) context.go(RouteNames.login);
    }
  }
}
