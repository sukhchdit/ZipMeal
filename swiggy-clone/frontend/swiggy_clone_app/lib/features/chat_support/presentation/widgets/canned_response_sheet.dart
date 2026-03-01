import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/canned_response_model.dart';
import '../../data/repositories/chat_support_repository.dart';

class CannedResponseSheet extends ConsumerStatefulWidget {
  const CannedResponseSheet({super.key, this.category});

  final int? category;

  @override
  ConsumerState<CannedResponseSheet> createState() =>
      _CannedResponseSheetState();
}

class _CannedResponseSheetState extends ConsumerState<CannedResponseSheet> {
  List<CannedResponseModel>? _responses;
  bool _isLoading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadResponses();
  }

  Future<void> _loadResponses() async {
    final repository = ref.read(chatSupportRepositoryProvider);
    final result =
        await repository.getCannedResponses(category: widget.category);

    if (mounted) {
      setState(() {
        _isLoading = false;
        if (result.failure != null) {
          _error = result.failure!.message;
        } else {
          _responses = result.data;
        }
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return DraggableScrollableSheet(
      initialChildSize: 0.5,
      minChildSize: 0.3,
      maxChildSize: 0.8,
      expand: false,
      builder: (context, scrollController) {
        return Column(
          children: [
            Padding(
              padding: const EdgeInsets.all(16),
              child: Row(
                children: [
                  Text(
                    'Quick Responses',
                    style: Theme.of(context).textTheme.titleMedium?.copyWith(
                          fontWeight: FontWeight.bold,
                        ),
                  ),
                  const Spacer(),
                  IconButton(
                    icon: const Icon(Icons.close),
                    onPressed: () => Navigator.of(context).pop(),
                  ),
                ],
              ),
            ),
            const Divider(height: 1),
            Expanded(
              child: _buildBody(scrollController),
            ),
          ],
        );
      },
    );
  }

  Widget _buildBody(ScrollController scrollController) {
    if (_isLoading) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_error != null) {
      return Center(child: Text(_error!));
    }
    if (_responses == null || _responses!.isEmpty) {
      return const Center(child: Text('No canned responses available.'));
    }

    return ListView.separated(
      controller: scrollController,
      padding: const EdgeInsets.symmetric(vertical: 8),
      itemCount: _responses!.length,
      separatorBuilder: (_, __) => const Divider(height: 1),
      itemBuilder: (context, index) {
        final response = _responses![index];
        return ListTile(
          title: Text(
            response.title,
            style: const TextStyle(fontWeight: FontWeight.w600),
          ),
          subtitle: Text(
            response.content,
            maxLines: 2,
            overflow: TextOverflow.ellipsis,
            style: TextStyle(color: AppColors.textSecondaryLight),
          ),
          onTap: () => Navigator.of(context).pop(response.content),
        );
      },
    );
  }
}
