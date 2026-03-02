import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/recommendations_repository.dart';

part 'interaction_tracker.g.dart';

@riverpod
InteractionTracker interactionTracker(Ref ref) {
  final repository = ref.watch(recommendationsRepositoryProvider);
  return InteractionTracker(repository);
}

class InteractionTracker {
  InteractionTracker(this._repository);

  final RecommendationsRepository _repository;

  /// Track a view interaction for a restaurant (entityType: 0).
  void trackRestaurantView(String restaurantId) {
    _repository.trackInteraction(
      entityType: 0,
      entityId: restaurantId,
      interactionType: 0,
    );
  }

  /// Track a click interaction for a restaurant (entityType: 0).
  void trackRestaurantClick(String restaurantId) {
    _repository.trackInteraction(
      entityType: 0,
      entityId: restaurantId,
      interactionType: 1,
    );
  }

  /// Track a view interaction for a menu item (entityType: 1).
  void trackMenuItemView(String menuItemId) {
    _repository.trackInteraction(
      entityType: 1,
      entityId: menuItemId,
      interactionType: 0,
    );
  }

  /// Track a click interaction for a menu item (entityType: 1).
  void trackMenuItemClick(String menuItemId) {
    _repository.trackInteraction(
      entityType: 1,
      entityId: menuItemId,
      interactionType: 1,
    );
  }
}
