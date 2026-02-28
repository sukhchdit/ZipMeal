import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/subscription_plan_model.dart';
import '../../data/models/user_subscription_model.dart';

part 'subscription_state.freezed.dart';

@freezed
sealed class SubscriptionState with _$SubscriptionState {
  const factory SubscriptionState.initial() = SubscriptionInitial;
  const factory SubscriptionState.loading() = SubscriptionLoading;
  const factory SubscriptionState.loaded({
    required List<SubscriptionPlanModel> plans,
    UserSubscriptionModel? activeSubscription,
    @Default(false) bool isSubscribing,
    @Default(false) bool isCancelling,
  }) = SubscriptionLoaded;
  const factory SubscriptionState.error({required Failure failure}) =
      SubscriptionError;
}
