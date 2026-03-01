import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../../data/models/user_profile_model.dart';
import '../../data/repositories/social_repository.dart';

part 'user_profile_notifier.g.dart';
part 'user_profile_notifier.freezed.dart';

@freezed
class UserProfileState with _$UserProfileState {
  const factory UserProfileState.initial() = _Initial;
  const factory UserProfileState.loading() = _Loading;
  const factory UserProfileState.loaded({
    required UserProfileModel profile,
  }) = UserProfileLoaded;
  const factory UserProfileState.error({required Failure failure}) = _Error;
}

@riverpod
class UserProfileNotifier extends _$UserProfileNotifier {
  late SocialRepository _repository;

  @override
  UserProfileState build(String userId) {
    _repository = ref.watch(socialRepositoryProvider);
    loadProfile();
    return const UserProfileState.initial();
  }

  Future<void> loadProfile() async {
    state = const UserProfileState.loading();
    final result = await _repository.getUserProfile(userId: userId);
    if (result.failure != null) {
      state = UserProfileState.error(failure: result.failure!);
    } else {
      state = UserProfileState.loaded(profile: result.data!);
    }
  }

  Future<bool> toggleFollow() async {
    final current = state;
    if (current is! UserProfileLoaded) return false;

    final profile = current.profile;
    if (profile.isFollowedByCurrentUser) {
      final failure = await _repository.unfollowUser(userId: userId);
      if (failure != null) return false;
      state = UserProfileState.loaded(
        profile: profile.copyWith(
          isFollowedByCurrentUser: false,
          followerCount: profile.followerCount - 1,
        ),
      );
    } else {
      final failure = await _repository.followUser(userId: userId);
      if (failure != null) return false;
      state = UserProfileState.loaded(
        profile: profile.copyWith(
          isFollowedByCurrentUser: true,
          followerCount: profile.followerCount + 1,
        ),
      );
    }
    return true;
  }
}
