import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/extensions/l10n_extensions.dart';
import '../../data/models/user_profile_model.dart';
import '../../data/repositories/social_repository.dart';
import '../widgets/user_list_tile.dart';

class FollowersScreen extends ConsumerStatefulWidget {
  const FollowersScreen({
    super.key,
    required this.userId,
    this.isFollowers = true,
  });

  final String userId;
  final bool isFollowers;

  @override
  ConsumerState<FollowersScreen> createState() => _FollowersScreenState();
}

class _FollowersScreenState extends ConsumerState<FollowersScreen> {
  final List<FollowUserModel> _users = [];
  bool _isLoading = true;
  bool _hasMore = true;
  int _page = 1;

  @override
  void initState() {
    super.initState();
    _loadUsers();
  }

  Future<void> _loadUsers() async {
    setState(() => _isLoading = true);
    final repo = ref.read(socialRepositoryProvider);

    final result = widget.isFollowers
        ? await repo.getFollowers(
            userId: widget.userId, page: _page)
        : await repo.getFollowing(
            userId: widget.userId, page: _page);

    if (mounted) {
      setState(() {
        _isLoading = false;
        if (result.data != null) {
          _users.addAll(result.data!);
          _hasMore = result.data!.length >= 20;
          _page++;
        }
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: Text(
          widget.isFollowers
              ? context.l10n.followers
              : context.l10n.following,
        ),
      ),
      body: _users.isEmpty && _isLoading
          ? const Center(child: CircularProgressIndicator())
          : _users.isEmpty
              ? Center(
                  child: Text(
                    widget.isFollowers
                        ? context.l10n.noActivityYet
                        : context.l10n.noActivityYet,
                    style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                          color: Colors.grey,
                        ),
                  ),
                )
              : NotificationListener<ScrollNotification>(
                  onNotification: (notification) {
                    if (notification is ScrollEndNotification &&
                        notification.metrics.pixels >=
                            notification.metrics.maxScrollExtent - 200 &&
                        _hasMore &&
                        !_isLoading) {
                      _loadUsers();
                    }
                    return false;
                  },
                  child: ListView.builder(
                    itemCount: _users.length + (_isLoading ? 1 : 0),
                    itemBuilder: (context, index) {
                      if (index == _users.length) {
                        return const Padding(
                          padding: EdgeInsets.all(16),
                          child: Center(child: CircularProgressIndicator()),
                        );
                      }
                      return UserListTile(user: _users[index]);
                    },
                  ),
                ),
    );
  }
}
