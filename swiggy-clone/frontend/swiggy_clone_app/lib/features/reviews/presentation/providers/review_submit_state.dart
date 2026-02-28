import 'package:freezed_annotation/freezed_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/review_model.dart';

part 'review_submit_state.freezed.dart';

@freezed
sealed class ReviewSubmitState with _$ReviewSubmitState {
  const factory ReviewSubmitState.initial() = ReviewSubmitInitial;
  const factory ReviewSubmitState.submitting() = ReviewSubmitSubmitting;
  const factory ReviewSubmitState.submitted({required ReviewModel review}) =
      ReviewSubmitSubmitted;
  const factory ReviewSubmitState.error({required Failure failure}) =
      ReviewSubmitError;
}
