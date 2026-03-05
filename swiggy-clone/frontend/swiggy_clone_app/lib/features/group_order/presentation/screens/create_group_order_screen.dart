import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/extensions/l10n_extensions.dart';
import '../../../../routing/route_names.dart';
import '../../data/repositories/group_order_repository.dart';
import '../providers/active_group_order_notifier.dart';

class CreateGroupOrderScreen extends ConsumerStatefulWidget {
  const CreateGroupOrderScreen({
    required this.restaurantId,
    required this.restaurantName,
    super.key,
  });

  final String restaurantId;
  final String restaurantName;

  @override
  ConsumerState<CreateGroupOrderScreen> createState() =>
      _CreateGroupOrderScreenState();
}

class _CreateGroupOrderScreenState
    extends ConsumerState<CreateGroupOrderScreen> {
  int _selectedSplitType = 0;
  bool _isCreating = false;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final l10n = context.l10n;

    return Scaffold(
      appBar: AppBar(title: Text(l10n.groupOrderCreate)),
      body: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    Text(
                      widget.restaurantName,
                      style: theme.textTheme.titleLarge,
                    ),
                    const SizedBox(height: 4),
                    Text(
                      l10n.groupOrderStartFromRestaurant,
                      style: theme.textTheme.bodyMedium?.copyWith(
                        color: theme.colorScheme.onSurfaceVariant,
                      ),
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 24),
            Text(
              l10n.groupOrderPaymentSplit,
              style: theme.textTheme.titleMedium,
            ),
            const SizedBox(height: 12),
            _SplitOptionTile(
              title: l10n.groupOrderIllPay,
              value: 0,
              groupValue: _selectedSplitType,
              onChanged: (v) => setState(() => _selectedSplitType = v!),
            ),
            _SplitOptionTile(
              title: l10n.groupOrderSplitEqual,
              value: 1,
              groupValue: _selectedSplitType,
              onChanged: (v) => setState(() => _selectedSplitType = v!),
            ),
            _SplitOptionTile(
              title: l10n.groupOrderPayYourShare,
              value: 2,
              groupValue: _selectedSplitType,
              onChanged: (v) => setState(() => _selectedSplitType = v!),
            ),
            const Spacer(),
            FilledButton.icon(
              onPressed: _isCreating ? null : _createGroupOrder,
              icon: _isCreating
                  ? const SizedBox(
                      width: 20,
                      height: 20,
                      child: CircularProgressIndicator(strokeWidth: 2),
                    )
                  : const Icon(Icons.group_add),
              label: Text(l10n.groupOrderCreate),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _createGroupOrder() async {
    setState(() => _isCreating = true);
    final repository = ref.read(groupOrderRepositoryProvider);
    final result = await repository.createGroupOrder(
      restaurantId: widget.restaurantId,
      paymentSplitType: _selectedSplitType,
    );
    setState(() => _isCreating = false);

    if (!mounted) return;

    if (result.failure != null) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(result.failure!.message)),
      );
      return;
    }

    ref.read(activeGroupOrderNotifierProvider.notifier).checkActiveGroupOrder();
    context.go(RouteNames.groupOrderLobbyPath(result.data!.id));
  }
}

class _SplitOptionTile extends StatelessWidget {
  const _SplitOptionTile({
    required this.title,
    required this.value,
    required this.groupValue,
    required this.onChanged,
  });

  final String title;
  final int value;
  final int groupValue;
  final ValueChanged<int?> onChanged;

  @override
  Widget build(BuildContext context) {
    return RadioListTile<int>(
      title: Text(title),
      value: value,
      groupValue: groupValue,
      onChanged: onChanged,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(8)),
    );
  }
}
