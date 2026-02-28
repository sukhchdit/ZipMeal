import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/subscription_plan_model.dart';

class PlanCard extends StatelessWidget {
  const PlanCard({
    super.key,
    required this.plan,
    required this.onSubscribe,
    this.hasActiveSub = false,
    this.isSubscribing = false,
  });

  final SubscriptionPlanModel plan;
  final VoidCallback onSubscribe;
  final bool hasActiveSub;
  final bool isSubscribing;

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    final priceRupees = plan.pricePaise / 100;

    return Card(
      margin: const EdgeInsets.only(bottom: 12),
      elevation: 1,
      shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(12)),
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    plan.name,
                    style: theme.textTheme.titleMedium?.copyWith(
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                ),
                Text(
                  '\u20B9${priceRupees % 1 == 0 ? priceRupees.toInt() : priceRupees}',
                  style: theme.textTheme.titleLarge?.copyWith(
                    fontWeight: FontWeight.bold,
                    color: AppColors.primary,
                  ),
                ),
              ],
            ),
            const SizedBox(height: 4),
            Text(
              '${plan.durationDays} days',
              style: theme.textTheme.bodySmall?.copyWith(
                color: AppColors.textSecondaryLight,
              ),
            ),
            if (plan.description != null) ...[
              const SizedBox(height: 8),
              Text(
                plan.description!,
                style: theme.textTheme.bodyMedium?.copyWith(
                  color: AppColors.textSecondaryLight,
                ),
                maxLines: 2,
                overflow: TextOverflow.ellipsis,
              ),
            ],
            const SizedBox(height: 12),

            // Benefit chips
            Wrap(
              spacing: 8,
              runSpacing: 6,
              children: [
                if (plan.freeDelivery)
                  _BenefitChip(
                    icon: Icons.local_shipping_outlined,
                    label: 'Free Delivery',
                  ),
                if (plan.extraDiscountPercent > 0)
                  _BenefitChip(
                    icon: Icons.percent_rounded,
                    label: '${plan.extraDiscountPercent}% Extra Off',
                  ),
                if (plan.noSurgeFee)
                  _BenefitChip(
                    icon: Icons.trending_down_rounded,
                    label: 'No Surge Fee',
                  ),
              ],
            ),

            const SizedBox(height: 16),
            SizedBox(
              width: double.infinity,
              child: FilledButton(
                onPressed: hasActiveSub || isSubscribing
                    ? null
                    : () => _showConfirmDialog(context),
                style: FilledButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  padding: const EdgeInsets.symmetric(vertical: 12),
                ),
                child: isSubscribing
                    ? const SizedBox(
                        height: 18,
                        width: 18,
                        child: CircularProgressIndicator(
                            strokeWidth: 2, color: Colors.white),
                      )
                    : Text(hasActiveSub ? 'Already Subscribed' : 'Subscribe'),
              ),
            ),
          ],
        ),
      ),
    );
  }

  void _showConfirmDialog(BuildContext context) {
    final priceRupees = plan.pricePaise / 100;
    showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text('Subscribe to ${plan.name}'),
        content: Text(
          'You will be charged \u20B9${priceRupees % 1 == 0 ? priceRupees.toInt() : priceRupees} for ${plan.durationDays} days. Continue?',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx, false),
            child: const Text('Cancel'),
          ),
          FilledButton(
            onPressed: () => Navigator.pop(ctx, true),
            child: const Text('Subscribe'),
          ),
        ],
      ),
    ).then((confirmed) {
      if (confirmed == true) onSubscribe();
    });
  }
}

class _BenefitChip extends StatelessWidget {
  const _BenefitChip({required this.icon, required this.label});

  final IconData icon;
  final String label;

  @override
  Widget build(BuildContext context) => Container(
        padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
        decoration: BoxDecoration(
          color: AppColors.primary.withValues(alpha: 0.08),
          borderRadius: BorderRadius.circular(20),
          border: Border.all(
            color: AppColors.primary.withValues(alpha: 0.2),
          ),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(icon, size: 14, color: AppColors.primary),
            const SizedBox(width: 4),
            Text(
              label,
              style: TextStyle(
                color: AppColors.primary,
                fontSize: 12,
                fontWeight: FontWeight.w600,
              ),
            ),
          ],
        ),
      );
}
