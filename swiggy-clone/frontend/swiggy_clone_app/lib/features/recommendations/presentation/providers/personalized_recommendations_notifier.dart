import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/personalized_recommendations_model.dart';
import '../../data/repositories/recommendations_repository.dart';

part 'personalized_recommendations_notifier.freezed.dart';
part 'personalized_recommendations_notifier.g.dart';

@freezed
sealed class PersonalizedRecommendationsState
    with _$PersonalizedRecommendationsState {
  const factory PersonalizedRecommendationsState.initial() =
      PersonalizedRecommendationsInitial;
  const factory PersonalizedRecommendationsState.loading() =
      PersonalizedRecommendationsLoading;
  const factory PersonalizedRecommendationsState.loaded({
    required PersonalizedRecommendationsModel recommendations,
  }) = PersonalizedRecommendationsLoaded;
  const factory PersonalizedRecommendationsState.error({
    required Failure failure,
  }) = PersonalizedRecommendationsError;
}

@riverpod
class PersonalizedRecommendationsNotifier
    extends _$PersonalizedRecommendationsNotifier {
  @override
  PersonalizedRecommendationsState build() {
    _load();
    return const PersonalizedRecommendationsState.loading();
  }

  Future<void> _load() async {
    final repository = ref.read(recommendationsRepositoryProvider);
    final result = await repository.getPersonalized();

    if (result.failure != null) {
      state = PersonalizedRecommendationsState.error(failure: result.failure!);
    } else {
      state = PersonalizedRecommendationsState.loaded(
        recommendations: result.data!,
      );
    }
  }

  Future<void> refresh() async {
    state = const PersonalizedRecommendationsState.loading();
    await _load();
  }
}
