import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/widgets/error_widget.dart';
import '../../../../core/widgets/loading_widget.dart';
import '../providers/review_analytics_notifier.dart';
import '../providers/review_analytics_state.dart';
import '../widgets/review_analytics_card.dart';

class ReviewAnalyticsScreen extends ConsumerWidget {
  const ReviewAnalyticsScreen({required this.restaurantId, super.key});

  final String restaurantId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final state = ref.watch(reviewAnalyticsNotifierProvider(restaurantId));

    return Scaffold(
      appBar: AppBar(title: const Text('Review Analytics')),
      body: switch (state) {
        ReviewAnalyticsInitial() ||
        ReviewAnalyticsLoading() =>
          const AppLoadingWidget(),
        ReviewAnalyticsError(:final failure) => AppErrorWidget(
            message: failure.message,
            onRetry: () => ref
                .read(reviewAnalyticsNotifierProvider(restaurantId).notifier)
                .refresh(),
          ),
        ReviewAnalyticsLoaded(:final analytics) => RefreshIndicator(
            onRefresh: () => ref
                .read(reviewAnalyticsNotifierProvider(restaurantId).notifier)
                .refresh(),
            child: ListView(
              children: [
                ReviewAnalyticsCard(analytics: analytics),
                if (analytics.monthlyTrend.isNotEmpty) ...[
                  Padding(
                    padding: const EdgeInsets.all(16),
                    child: Text(
                      'Monthly Trend',
                      style: Theme.of(context).textTheme.titleMedium?.copyWith(
                            fontWeight: FontWeight.bold,
                          ),
                    ),
                  ),
                  ...analytics.monthlyTrend.map((item) => ListTile(
                        title: Text(item.month),
                        subtitle: Text('${item.count} reviews'),
                        trailing: Row(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            const Icon(Icons.star,
                                size: 16, color: Colors.amber),
                            const SizedBox(width: 4),
                            Text(item.avgRating.toStringAsFixed(1)),
                          ],
                        ),
                      )),
                ],
              ],
            ),
          ),
      },
    );
  }
}
