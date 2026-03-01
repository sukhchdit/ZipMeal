import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../routing/route_names.dart';

class HelpSupportScreen extends StatelessWidget {
  const HelpSupportScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(title: const Text('Help & Support')),
      body: ListView(
        padding: const EdgeInsets.all(16),
        children: [
          // FAQ section
          Text(
            'Frequently Asked Questions',
            style: theme.textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),
          const _FaqTile(
            question: 'How do I track my order?',
            answer:
                'Go to My Orders, tap on the active order, and press "Track Order" to see live delivery tracking.',
          ),
          const _FaqTile(
            question: 'How do I cancel an order?',
            answer:
                'You can cancel an order within 2 minutes of placing it. Go to My Orders, tap the order, and select "Cancel Order".',
          ),
          const _FaqTile(
            question: 'How do I change my delivery address?',
            answer:
                'Go to Account > Addresses to manage your saved addresses. You can also select a different address during checkout.',
          ),
          const _FaqTile(
            question: 'What payment methods are accepted?',
            answer:
                'We accept UPI, credit/debit cards, net banking, and cash on delivery.',
          ),
          const _FaqTile(
            question: 'How do I apply a coupon?',
            answer:
                'Enter your coupon code at checkout in the "Apply Coupon" field. Valid coupons will automatically adjust your total.',
          ),
          const SizedBox(height: 24),

          // Chat with us
          Card(
            color: AppColors.primary.withAlpha(15),
            child: InkWell(
              onTap: () => context.go(RouteNames.chatTickets),
              borderRadius: BorderRadius.circular(12),
              child: Padding(
                padding: const EdgeInsets.all(16),
                child: Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.all(12),
                      decoration: BoxDecoration(
                        color: AppColors.primary.withAlpha(25),
                        shape: BoxShape.circle,
                      ),
                      child: const Icon(
                        Icons.chat_outlined,
                        color: AppColors.primary,
                      ),
                    ),
                    const SizedBox(width: 16),
                    Expanded(
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text(
                            'Chat with us',
                            style: theme.textTheme.titleSmall?.copyWith(
                              fontWeight: FontWeight.bold,
                            ),
                          ),
                          const SizedBox(height: 4),
                          Text(
                            'Get real-time help from our support team',
                            style: theme.textTheme.bodySmall?.copyWith(
                              color: AppColors.textSecondaryLight,
                            ),
                          ),
                        ],
                      ),
                    ),
                    const Icon(
                      Icons.chevron_right,
                      color: AppColors.primary,
                    ),
                  ],
                ),
              ),
            ),
          ),
          const SizedBox(height: 24),

          // Contact section
          Text(
            'Contact Us',
            style: theme.textTheme.titleMedium?.copyWith(
              fontWeight: FontWeight.bold,
            ),
          ),
          const SizedBox(height: 12),
          Card(
            child: Padding(
              padding: const EdgeInsets.all(16),
              child: Column(
                children: [
                  ListTile(
                    leading: const Icon(Icons.email_outlined,
                        color: AppColors.primary),
                    title: const Text('Email'),
                    subtitle: const Text('support@zipmeal.com'),
                    contentPadding: EdgeInsets.zero,
                  ),
                  const Divider(height: 1),
                  ListTile(
                    leading: const Icon(Icons.phone_outlined,
                        color: AppColors.primary),
                    title: const Text('Phone'),
                    subtitle: const Text('+91 1800-123-4567'),
                    contentPadding: EdgeInsets.zero,
                  ),
                  const Divider(height: 1),
                  ListTile(
                    leading: const Icon(Icons.access_time_outlined,
                        color: AppColors.primary),
                    title: const Text('Support Hours'),
                    subtitle: const Text('24/7'),
                    contentPadding: EdgeInsets.zero,
                  ),
                ],
              ),
            ),
          ),
          const SizedBox(height: 24),

          // App info
          Center(
            child: Text(
              'ZipMeal v1.0.0',
              style: theme.textTheme.bodySmall?.copyWith(
                color: AppColors.textTertiaryLight,
              ),
            ),
          ),
          const SizedBox(height: 8),
        ],
      ),
    );
  }
}

class _FaqTile extends StatelessWidget {
  const _FaqTile({required this.question, required this.answer});

  final String question;
  final String answer;

  @override
  Widget build(BuildContext context) => ExpansionTile(
        title: Text(
          question,
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                fontWeight: FontWeight.w600,
              ),
        ),
        childrenPadding: const EdgeInsets.fromLTRB(16, 0, 16, 16),
        children: [
          Text(
            answer,
            style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                  color: AppColors.textSecondaryLight,
                ),
          ),
        ],
      );
}
