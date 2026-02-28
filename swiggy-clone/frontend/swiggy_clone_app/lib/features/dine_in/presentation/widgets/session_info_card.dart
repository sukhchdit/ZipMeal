import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/dine_in_session_model.dart';

class SessionInfoCard extends StatelessWidget {
  const SessionInfoCard({required this.session, super.key});

  final DineInSessionSummaryModel session;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 8),
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Row(
          children: [
            CircleAvatar(
              radius: 24,
              backgroundImage: session.restaurantLogoUrl != null
                  ? NetworkImage(session.restaurantLogoUrl!)
                  : null,
              child: session.restaurantLogoUrl == null
                  ? const Icon(Icons.restaurant)
                  : null,
            ),
            const SizedBox(width: 12),
            Expanded(
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text(
                    session.restaurantName,
                    style: theme.textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  const SizedBox(height: 4),
                  Row(
                    children: [
                      Icon(Icons.table_restaurant,
                          size: 16, color: AppColors.textSecondaryLight),
                      const SizedBox(width: 4),
                      Text(
                        'Table ${session.tableNumber}',
                        style: theme.textTheme.bodyMedium?.copyWith(
                          color: AppColors.textSecondaryLight,
                        ),
                      ),
                      const SizedBox(width: 16),
                      Icon(Icons.people_outline,
                          size: 16, color: AppColors.textSecondaryLight),
                      const SizedBox(width: 4),
                      Text(
                        '${session.guestCount} guests',
                        style: theme.textTheme.bodyMedium?.copyWith(
                          color: AppColors.textSecondaryLight,
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
