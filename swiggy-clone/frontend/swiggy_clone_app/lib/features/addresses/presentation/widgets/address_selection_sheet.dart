import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../../../../routing/route_names.dart';
import '../../data/models/address_model.dart';
import '../providers/address_list_notifier.dart';
import '../providers/address_list_state.dart';

class AddressSelectionSheet extends ConsumerWidget {
  const AddressSelectionSheet({super.key, this.selectedId});

  final String? selectedId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(addressListNotifierProvider);
    final theme = Theme.of(context);

    return DraggableScrollableSheet(
      initialChildSize: 0.5,
      maxChildSize: 0.85,
      minChildSize: 0.3,
      expand: false,
      builder: (context, scrollController) {
        return Column(
          children: [
            // Handle
            Padding(
              padding: const EdgeInsets.symmetric(vertical: 12),
              child: Container(
                width: 40,
                height: 4,
                decoration: BoxDecoration(
                  color: AppColors.borderLight,
                  borderRadius: BorderRadius.circular(2),
                ),
              ),
            ),
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16),
              child: Text(
                'Select Delivery Address',
                style: theme.textTheme.titleMedium?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
            ),
            const SizedBox(height: 12),
            Expanded(
              child: switch (state) {
                AddressListInitial() || AddressListLoading() =>
                  const AppLoadingWidget(message: 'Loading...'),
                AddressListError(:final failure) => Center(
                    child: Text(failure.message),
                  ),
                AddressListLoaded(:final addresses) =>
                  ListView.builder(
                    controller: scrollController,
                    padding: const EdgeInsets.symmetric(horizontal: 16),
                    itemCount: addresses.length + 1, // +1 for "Add New"
                    itemBuilder: (context, index) {
                      if (index == addresses.length) {
                        return ListTile(
                          leading: const Icon(Icons.add_circle_outline,
                              color: AppColors.primary),
                          title: Text(
                            'Add New Address',
                            style: theme.textTheme.bodyMedium?.copyWith(
                              color: AppColors.primary,
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                          onTap: () {
                            Navigator.of(context).pop();
                            context.go(RouteNames.addAddress);
                          },
                        );
                      }

                      final address = addresses[index];
                      final isSelected = address.id == selectedId ||
                          (selectedId == null && address.isDefault);

                      return RadioListTile<String>(
                        value: address.id,
                        groupValue:
                            isSelected ? address.id : '__none__',
                        onChanged: (_) =>
                            Navigator.of(context).pop(address),
                        activeColor: AppColors.primary,
                        title: Row(
                          children: [
                            Text(
                              address.label,
                              style: theme.textTheme.titleSmall?.copyWith(
                                fontWeight: FontWeight.bold,
                              ),
                            ),
                            if (address.isDefault) ...[
                              const SizedBox(width: 8),
                              Container(
                                padding: const EdgeInsets.symmetric(
                                    horizontal: 6, vertical: 1),
                                decoration: BoxDecoration(
                                  color: AppColors.primary
                                      .withValues(alpha: 0.1),
                                  borderRadius: BorderRadius.circular(4),
                                ),
                                child: Text(
                                  'Default',
                                  style:
                                      theme.textTheme.labelSmall?.copyWith(
                                    color: AppColors.primary,
                                    fontWeight: FontWeight.w600,
                                  ),
                                ),
                              ),
                            ],
                          ],
                        ),
                        subtitle: Text(
                          [
                            address.addressLine1,
                            if (address.city != null) address.city,
                            if (address.postalCode != null)
                              address.postalCode,
                          ].join(', '),
                          maxLines: 2,
                          overflow: TextOverflow.ellipsis,
                        ),
                      );
                    },
                  ),
              },
            ),
          ],
        );
      },
    );
  }
}
