import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../providers/auth_notifier.dart';
import '../providers/auth_state.dart';
import '../providers/profile_update_notifier.dart';
import '../providers/profile_update_state.dart';

class EditProfileScreen extends ConsumerStatefulWidget {
  const EditProfileScreen({super.key});

  @override
  ConsumerState<EditProfileScreen> createState() => _EditProfileScreenState();
}

class _EditProfileScreenState extends ConsumerState<EditProfileScreen> {
  final _formKey = GlobalKey<FormState>();
  final _fullNameController = TextEditingController();
  final _emailController = TextEditingController();
  final _avatarUrlController = TextEditingController();

  @override
  void initState() {
    super.initState();
    _loadCurrentProfile();
  }

  void _loadCurrentProfile() {
    final authState = ref.read(authNotifierProvider);
    if (authState is AuthAuthenticated) {
      _fullNameController.text = authState.user.fullName;
      _emailController.text = authState.user.email ?? '';
      _avatarUrlController.text = authState.user.avatarUrl ?? '';
    }
  }

  @override
  void dispose() {
    _fullNameController.dispose();
    _emailController.dispose();
    _avatarUrlController.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final updateState = ref.watch(profileUpdateNotifierProvider);

    ref.listen<ProfileUpdateState>(profileUpdateNotifierProvider, (prev, next) {
      if (next is ProfileUpdateSaved) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(content: Text('Profile updated successfully')),
        );
        Navigator.of(context).pop();
      } else if (next is ProfileUpdateError) {
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(content: Text(next.failure.message)),
        );
      }
    });

    return Scaffold(
      appBar: AppBar(title: const Text('Edit Profile')),
      body: Form(
        key: _formKey,
        child: ListView(
          padding: const EdgeInsets.all(16),
          children: [
            // Avatar preview
            Center(
              child: _buildAvatarPreview(),
            ),
            const SizedBox(height: 24),

            // Full Name
            TextFormField(
              controller: _fullNameController,
              decoration: const InputDecoration(
                labelText: 'Full Name',
                prefixIcon: Icon(Icons.person_outline),
                border: OutlineInputBorder(),
              ),
              validator: (v) => (v == null || v.trim().isEmpty)
                  ? 'Name is required'
                  : null,
            ),
            const SizedBox(height: 16),

            // Email
            TextFormField(
              controller: _emailController,
              decoration: const InputDecoration(
                labelText: 'Email',
                prefixIcon: Icon(Icons.email_outlined),
                border: OutlineInputBorder(),
              ),
              keyboardType: TextInputType.emailAddress,
            ),
            const SizedBox(height: 16),

            // Avatar URL
            TextFormField(
              controller: _avatarUrlController,
              decoration: const InputDecoration(
                labelText: 'Avatar URL',
                prefixIcon: Icon(Icons.image_outlined),
                border: OutlineInputBorder(),
                hintText: 'https://example.com/avatar.jpg',
              ),
              keyboardType: TextInputType.url,
            ),
            const SizedBox(height: 24),

            // Phone number (read-only)
            Builder(builder: (context) {
              final authState = ref.watch(authNotifierProvider);
              final phone = authState is AuthAuthenticated
                  ? authState.user.phoneNumber
                  : '';
              return TextFormField(
                initialValue: phone,
                decoration: const InputDecoration(
                  labelText: 'Phone Number',
                  prefixIcon: Icon(Icons.phone_outlined),
                  border: OutlineInputBorder(),
                  helperText: 'Phone number cannot be changed',
                ),
                readOnly: true,
                enabled: false,
              );
            }),
            const SizedBox(height: 32),

            // Save button
            FilledButton(
              onPressed: updateState is ProfileUpdateSaving ? null : _save,
              style: FilledButton.styleFrom(
                backgroundColor: AppColors.primary,
                minimumSize: const Size.fromHeight(52),
              ),
              child: updateState is ProfileUpdateSaving
                  ? const SizedBox(
                      width: 24,
                      height: 24,
                      child: CircularProgressIndicator(
                        strokeWidth: 2,
                        color: Colors.white,
                      ),
                    )
                  : const Text(
                      'Save Changes',
                      style: TextStyle(
                        fontWeight: FontWeight.bold,
                        fontSize: 16,
                      ),
                    ),
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildAvatarPreview() {
    final url = _avatarUrlController.text.trim();
    final authState = ref.watch(authNotifierProvider);
    final name =
        authState is AuthAuthenticated ? authState.user.fullName : '';

    return CircleAvatar(
      radius: 48,
      backgroundColor: AppColors.primary.withValues(alpha: 0.15),
      backgroundImage: url.isNotEmpty ? NetworkImage(url) : null,
      child: url.isEmpty
          ? Text(
              _initials(name),
              style: const TextStyle(
                fontSize: 32,
                color: AppColors.primary,
                fontWeight: FontWeight.bold,
              ),
            )
          : null,
    );
  }

  String _initials(String name) {
    final parts = name.trim().split(' ');
    if (parts.length >= 2) {
      return '${parts[0][0]}${parts[1][0]}'.toUpperCase();
    }
    return parts[0].isNotEmpty ? parts[0][0].toUpperCase() : '?';
  }

  void _save() {
    if (!_formKey.currentState!.validate()) return;

    final fullName = _fullNameController.text.trim();
    final email = _emailController.text.trim();
    final avatarUrl = _avatarUrlController.text.trim();

    ref.read(profileUpdateNotifierProvider.notifier).updateProfile(
          fullName: fullName.isNotEmpty ? fullName : null,
          email: email.isNotEmpty ? email : null,
          avatarUrl: avatarUrl.isNotEmpty ? avatarUrl : null,
        );
  }
}
