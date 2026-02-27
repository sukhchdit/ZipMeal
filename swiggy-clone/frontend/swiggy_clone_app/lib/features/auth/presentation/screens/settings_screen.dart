import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';

class SettingsScreen extends StatelessWidget {
  const SettingsScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(title: const Text('Settings')),
      body: ListView(
        children: [
          // Notifications section
          _SectionHeader(title: 'Notifications', theme: theme),
          SwitchListTile(
            title: const Text('Push Notifications'),
            subtitle: const Text('Order updates, promotions, etc.'),
            value: true,
            onChanged: (_) {
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('Notification preferences coming soon'),
                ),
              );
            },
            activeColor: AppColors.primary,
          ),
          SwitchListTile(
            title: const Text('Email Notifications'),
            subtitle: const Text('Receipts and account updates'),
            value: true,
            onChanged: (_) {
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('Notification preferences coming soon'),
                ),
              );
            },
            activeColor: AppColors.primary,
          ),
          const Divider(height: 1),

          // Appearance section
          _SectionHeader(title: 'Appearance', theme: theme),
          ListTile(
            title: const Text('Theme'),
            subtitle: const Text('System default'),
            trailing: const Icon(Icons.chevron_right, size: 20),
            onTap: () {
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('Theme settings coming soon'),
                ),
              );
            },
          ),
          const Divider(height: 1),

          // About section
          _SectionHeader(title: 'About', theme: theme),
          const ListTile(
            title: Text('App Version'),
            subtitle: Text('1.0.0'),
          ),
          ListTile(
            title: const Text('Terms of Service'),
            trailing: const Icon(Icons.chevron_right, size: 20),
            onTap: () {},
          ),
          ListTile(
            title: const Text('Privacy Policy'),
            trailing: const Icon(Icons.chevron_right, size: 20),
            onTap: () {},
          ),
        ],
      ),
    );
  }
}

class _SectionHeader extends StatelessWidget {
  const _SectionHeader({required this.title, required this.theme});

  final String title;
  final ThemeData theme;

  @override
  Widget build(BuildContext context) => Padding(
        padding: const EdgeInsets.fromLTRB(16, 20, 16, 8),
        child: Text(
          title,
          style: theme.textTheme.titleSmall?.copyWith(
            color: AppColors.textSecondaryLight,
            fontWeight: FontWeight.w600,
          ),
        ),
      );
}
