import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:intl/intl.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../data/models/admin_user_model.dart';
import '../providers/admin_user_detail_notifier.dart';

/// Detail screen for a single user with admin actions (toggle active, change role).
class AdminUserDetailScreen extends ConsumerWidget {
  const AdminUserDetailScreen({required this.userId, super.key});

  final String userId;

  static const _roleLabels = {
    1: 'Customer',
    2: 'Owner',
    3: 'Partner',
    4: 'Admin',
  };

  static const _roleColors = {
    1: AppColors.info,
    2: AppColors.primary,
    3: Colors.deepPurple,
    4: AppColors.error,
  };

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(adminUserDetailNotifierProvider(userId));

    return Scaffold(
      appBar: AppBar(title: const Text('User Detail')),
      body: switch (state) {
        AdminUserDetailInitial() || AdminUserDetailLoading() =>
          const AppLoadingWidget(message: 'Loading user...'),
        AdminUserDetailError(:final failure) => AppErrorWidget(
            failure: failure,
            onRetry: () => ref
                .read(adminUserDetailNotifierProvider(userId).notifier)
                .loadDetail(),
          ),
        AdminUserDetailLoaded(:final user) => _UserDetailBody(
            user: user,
            userId: userId,
          ),
      },
    );
  }
}

class _UserDetailBody extends ConsumerWidget {
  const _UserDetailBody({required this.user, required this.userId});

  final AdminUserModel user;
  final String userId;

  static const _roleLabels = {
    1: 'Customer',
    2: 'Owner',
    3: 'Partner',
    4: 'Admin',
  };

  static const _roleColors = {
    1: AppColors.info,
    2: AppColors.primary,
    3: Colors.deepPurple,
    4: AppColors.error,
  };

  String _initials(String name) {
    final parts = name.trim().split(' ');
    if (parts.length >= 2) {
      return '${parts.first[0]}${parts.last[0]}'.toUpperCase();
    }
    return parts.first.isNotEmpty ? parts.first[0].toUpperCase() : '?';
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final theme = Theme.of(context);
    final dateFormat = DateFormat('dd MMM yyyy, hh:mm a');
    final roleLabel = _roleLabels[user.role] ?? 'Unknown';
    final roleColor = _roleColors[user.role] ?? AppColors.textSecondaryLight;

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        // ──── Avatar & Name ────
        Center(
          child: CircleAvatar(
            radius: 40,
            backgroundColor: roleColor.withValues(alpha: 0.15),
            child: Text(
              _initials(user.fullName),
              style: TextStyle(
                fontSize: 28,
                color: roleColor,
                fontWeight: FontWeight.bold,
              ),
            ),
          ),
        ),
        const SizedBox(height: 12),
        Center(
          child: Text(
            user.fullName,
            style: theme.textTheme.headlineSmall?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        const SizedBox(height: 8),

        // ──── Badges Row ────
        Row(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            // Role badge
            Container(
              padding:
                  const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
              decoration: BoxDecoration(
                color: roleColor.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(6),
              ),
              child: Text(
                roleLabel,
                style: theme.textTheme.labelMedium?.copyWith(
                  color: roleColor,
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            const SizedBox(width: 8),
            // Verified badge
            if (user.isVerified)
              Container(
                padding:
                    const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
                decoration: BoxDecoration(
                  color: AppColors.success.withValues(alpha: 0.12),
                  borderRadius: BorderRadius.circular(6),
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    const Icon(Icons.verified, size: 14,
                        color: AppColors.success),
                    const SizedBox(width: 4),
                    Text(
                      'Verified',
                      style: theme.textTheme.labelMedium?.copyWith(
                        color: AppColors.success,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ],
                ),
              ),
            const SizedBox(width: 8),
            // Active badge
            Container(
              padding:
                  const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
              decoration: BoxDecoration(
                color: (user.isActive ? AppColors.success : AppColors.error)
                    .withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(6),
              ),
              child: Text(
                user.isActive ? 'Active' : 'Inactive',
                style: theme.textTheme.labelMedium?.copyWith(
                  color: user.isActive ? AppColors.success : AppColors.error,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ),
          ],
        ),

        const Divider(height: 32),

        // ──── Info Section ────
        _InfoRow(label: 'Phone', value: user.phoneNumber),
        if (user.email != null) _InfoRow(label: 'Email', value: user.email!),
        _InfoRow(label: 'Created', value: dateFormat.format(user.createdAt)),
        if (user.lastLoginAt != null)
          _InfoRow(
            label: 'Last Login',
            value: dateFormat.format(user.lastLoginAt!),
          ),

        const Divider(height: 32),

        // ──── Actions ────
        Text(
          'Actions',
          style: theme.textTheme.titleMedium?.copyWith(
            fontWeight: FontWeight.bold,
          ),
        ),
        const SizedBox(height: 12),

        // Toggle Active
        user.isActive
            ? OutlinedButton.icon(
                onPressed: () async {
                  final confirmed = await _confirmAction(
                    context,
                    title: 'Deactivate User',
                    message:
                        'Are you sure you want to deactivate ${user.fullName}?',
                  );
                  if (confirmed) {
                    await ref
                        .read(adminUserDetailNotifierProvider(userId).notifier)
                        .toggleActive(isActive: false);
                  }
                },
                icon: const Icon(Icons.block),
                label: const Text('Deactivate User'),
                style: OutlinedButton.styleFrom(
                  foregroundColor: AppColors.error,
                  side: const BorderSide(color: AppColors.error),
                  minimumSize: const Size.fromHeight(48),
                ),
              )
            : FilledButton.icon(
                onPressed: () async {
                  final confirmed = await _confirmAction(
                    context,
                    title: 'Activate User',
                    message:
                        'Are you sure you want to activate ${user.fullName}?',
                  );
                  if (confirmed) {
                    await ref
                        .read(adminUserDetailNotifierProvider(userId).notifier)
                        .toggleActive(isActive: true);
                  }
                },
                icon: const Icon(Icons.check_circle_outline),
                label: const Text('Activate User'),
                style: FilledButton.styleFrom(
                  minimumSize: const Size.fromHeight(48),
                ),
              ),

        const SizedBox(height: 12),

        // Change Role
        OutlinedButton.icon(
          onPressed: () => _showChangeRoleDialog(context, ref),
          icon: const Icon(Icons.swap_horiz),
          label: const Text('Change Role'),
          style: OutlinedButton.styleFrom(
            minimumSize: const Size.fromHeight(48),
          ),
        ),
      ],
    );
  }

  Future<bool> _confirmAction(
    BuildContext context, {
    required String title,
    required String message,
  }) async {
    final result = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text(title),
        content: Text(message),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(false),
            child: const Text('Cancel'),
          ),
          TextButton(
            onPressed: () => Navigator.of(ctx).pop(true),
            child: const Text('Confirm'),
          ),
        ],
      ),
    );
    return result ?? false;
  }

  void _showChangeRoleDialog(BuildContext context, WidgetRef ref) {
    int selectedRole = user.role;

    showDialog(
      context: context,
      builder: (ctx) => StatefulBuilder(
        builder: (ctx, setDialogState) => AlertDialog(
          title: const Text('Change Role'),
          content: DropdownButtonFormField<int>(
            value: selectedRole,
            decoration: const InputDecoration(
              labelText: 'Role',
              border: OutlineInputBorder(),
            ),
            items: const [
              DropdownMenuItem(value: 1, child: Text('Customer')),
              DropdownMenuItem(value: 2, child: Text('Owner')),
              DropdownMenuItem(value: 3, child: Text('Partner')),
              DropdownMenuItem(value: 4, child: Text('Admin')),
            ],
            onChanged: (value) {
              if (value != null) {
                setDialogState(() => selectedRole = value);
              }
            },
          ),
          actions: [
            TextButton(
              onPressed: () => Navigator.of(ctx).pop(),
              child: const Text('Cancel'),
            ),
            FilledButton(
              onPressed: () async {
                Navigator.of(ctx).pop();
                if (selectedRole != user.role) {
                  await ref
                      .read(adminUserDetailNotifierProvider(userId).notifier)
                      .changeRole(newRole: selectedRole);
                }
              },
              child: const Text('Save'),
            ),
          ],
        ),
      ),
    );
  }
}

class _InfoRow extends StatelessWidget {
  const _InfoRow({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Padding(
      padding: const EdgeInsets.only(bottom: 8),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 100,
            child: Text(
              label,
              style: theme.textTheme.bodyMedium?.copyWith(
                color: AppColors.textSecondaryLight,
              ),
            ),
          ),
          Expanded(
            child: Text(value, style: theme.textTheme.bodyMedium),
          ),
        ],
      ),
    );
  }
}
