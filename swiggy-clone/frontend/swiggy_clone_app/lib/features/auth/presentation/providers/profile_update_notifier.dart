import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/auth_repository.dart';
import 'auth_notifier.dart';
import 'auth_state.dart';
import 'profile_update_state.dart';

part 'profile_update_notifier.g.dart';

@riverpod
class ProfileUpdateNotifier extends _$ProfileUpdateNotifier {
  late AuthRepository _repository;

  @override
  ProfileUpdateState build() {
    _repository = ref.watch(authRepositoryProvider);
    return const ProfileUpdateState.initial();
  }

  Future<void> updateProfile({
    String? fullName,
    String? email,
    String? avatarUrl,
  }) async {
    state = const ProfileUpdateState.saving();

    final result = await _repository.updateProfile(
      fullName: fullName,
      email: email,
      avatarUrl: avatarUrl,
    );

    if (result.failure != null) {
      state = ProfileUpdateState.error(failure: result.failure!);
    } else {
      state = ProfileUpdateState.saved(user: result.data!);
      // Sync updated user into the global auth state
      ref.read(authNotifierProvider.notifier).updateUser(result.data!);
    }
  }
}
