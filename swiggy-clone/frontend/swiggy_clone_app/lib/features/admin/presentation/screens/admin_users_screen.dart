import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/admin_user_model.dart';
import '../providers/admin_users_notifier.dart';
import '../providers/admin_users_state.dart';

/// Paginated list of all platform users with search and role filter.
class AdminUsersScreen extends ConsumerStatefulWidget {
  const AdminUsersScreen({super.key});

  @override
  ConsumerState<AdminUsersScreen> createState() => _AdminUsersScreenState();
}

class _AdminUsersScreenState extends ConsumerState<AdminUsersScreen> {
  bool _isSearching = false;
  final _searchController = TextEditingController();
  int? _selectedRole;

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
  void dispose() {
    _searchController.dispose();
    super.dispose();
  }

  void _applyFilters() {
    ref.read(adminUsersNotifierProvider.notifier).loadUsers(
          search:
              _searchController.text.isEmpty ? null : _searchController.text,
          roleFilter: _selectedRole,
        );
  }

  @override
  Widget build(BuildContext context) {
    final state = ref.watch(adminUsersNotifierProvider);
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: _isSearching
            ? TextField(
                controller: _searchController,
                autofocus: true,
                decoration: const InputDecoration(
                  hintText: 'Search users...',
                  border: InputBorder.none,
                ),
                onSubmitted: (_) => _applyFilters(),
              )
            : const Text('Users'),
        actions: [
          IconButton(
            icon: Icon(_isSearching ? Icons.close : Icons.search),
            onPressed: () {
              setState(() {
                _isSearching = !_isSearching;
                if (!_isSearching) {
                  _searchController.clear();
                  _applyFilters();
                }
              });
            },
          ),
        ],
      ),
      body: Column(
        children: [
          // ──── Role Filter Chips ────
          SizedBox(
            height: 48,
            child: ListView(
              scrollDirection: Axis.horizontal,
              padding: const EdgeInsets.symmetric(horizontal: 12),
              children: [
                Padding(
                  padding: const EdgeInsets.only(right: 8),
                  child: FilterChip(
                    label: const Text('All'),
                    selected: _selectedRole == null,
                    onSelected: (_) {
                      setState(() => _selectedRole = null);
                      _applyFilters();
                    },
                  ),
                ),
                ..._roleLabels.entries.map(
                  (entry) => Padding(
                    padding: const EdgeInsets.only(right: 8),
                    child: FilterChip(
                      label: Text(entry.value),
                      selected: _selectedRole == entry.key,
                      onSelected: (_) {
                        setState(() => _selectedRole = entry.key);
                        _applyFilters();
                      },
                    ),
                  ),
                ),
              ],
            ),
          ),

          // ──── User List ────
          Expanded(
            child: switch (state) {
              AdminUsersInitial() || AdminUsersLoading() =>
                const AppLoadingWidget(message: 'Loading users...'),
              AdminUsersError(:final failure) => AppErrorWidget(
                  failure: failure,
                  onRetry: _applyFilters,
                ),
              AdminUsersLoaded(
                :final users,
                :final page,
                :final totalPages,
                :final isLoadingMore,
              ) =>
                users.isEmpty
                    ? Center(
                        child: Column(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            Icon(Icons.people_outline,
                                size: 80,
                                color: AppColors.textTertiaryLight),
                            const SizedBox(height: 16),
                            Text(
                              'No users found',
                              style: theme.textTheme.titleMedium?.copyWith(
                                color: AppColors.textSecondaryLight,
                              ),
                            ),
                          ],
                        ),
                      )
                    : RefreshIndicator(
                        color: AppColors.primary,
                        onRefresh: () async => _applyFilters(),
                        child: ListView.builder(
                          padding: const EdgeInsets.symmetric(vertical: 8),
                          itemCount:
                              users.length + (page < totalPages ? 1 : 0),
                          itemBuilder: (context, index) {
                            if (index >= users.length) {
                              if (!isLoadingMore) {
                                WidgetsBinding.instance
                                    .addPostFrameCallback((_) {
                                  ref
                                      .read(adminUsersNotifierProvider.notifier)
                                      .loadUsers(
                                        search: _searchController.text.isEmpty
                                            ? null
                                            : _searchController.text,
                                        roleFilter: _selectedRole,
                                        page: page + 1,
                                      );
                                });
                              }
                              return const Padding(
                                padding: EdgeInsets.all(16),
                                child: Center(
                                  child: CircularProgressIndicator(
                                      strokeWidth: 2),
                                ),
                              );
                            }
                            return _UserTile(
                              user: users[index],
                              roleLabel:
                                  _roleLabels[users[index].role] ?? 'Unknown',
                              roleColor: _roleColors[users[index].role] ??
                                  AppColors.textSecondaryLight,
                              onTap: () => context.push(
                                RouteNames.adminUserDetailPath(
                                    users[index].id),
                              ),
                            );
                          },
                        ),
                      ),
            },
          ),
        ],
      ),
    );
  }
}

class _UserTile extends StatelessWidget {
  const _UserTile({
    required this.user,
    required this.roleLabel,
    required this.roleColor,
    required this.onTap,
  });

  final AdminUserModel user;
  final String roleLabel;
  final Color roleColor;
  final VoidCallback onTap;

  String _initials(String name) {
    final parts = name.trim().split(' ');
    if (parts.length >= 2) {
      return '${parts.first[0]}${parts.last[0]}'.toUpperCase();
    }
    return parts.first.isNotEmpty ? parts.first[0].toUpperCase() : '?';
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 4),
      child: ListTile(
        onTap: onTap,
        leading: CircleAvatar(
          backgroundColor: roleColor.withValues(alpha: 0.15),
          child: Text(
            _initials(user.fullName),
            style: TextStyle(
              color: roleColor,
              fontWeight: FontWeight.bold,
            ),
          ),
        ),
        title: Row(
          children: [
            Expanded(
              child: Text(
                user.fullName,
                style: theme.textTheme.titleSmall?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
                maxLines: 1,
                overflow: TextOverflow.ellipsis,
              ),
            ),
            // Active indicator
            Container(
              width: 8,
              height: 8,
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                color: user.isActive ? AppColors.success : AppColors.error,
              ),
            ),
          ],
        ),
        subtitle: Row(
          children: [
            Expanded(
              child: Text(
                user.phoneNumber,
                style: theme.textTheme.bodySmall?.copyWith(
                  color: AppColors.textSecondaryLight,
                ),
              ),
            ),
            Container(
              padding:
                  const EdgeInsets.symmetric(horizontal: 8, vertical: 2),
              decoration: BoxDecoration(
                color: roleColor.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(4),
              ),
              child: Text(
                roleLabel,
                style: theme.textTheme.labelSmall?.copyWith(
                  color: roleColor,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ),
          ],
        ),
        trailing: const Icon(
          Icons.chevron_right,
          color: AppColors.textTertiaryLight,
        ),
      ),
    );
  }
}
