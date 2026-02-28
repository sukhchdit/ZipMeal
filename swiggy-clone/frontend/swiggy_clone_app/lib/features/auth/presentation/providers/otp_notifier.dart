import 'dart:async';

import 'package:equatable/equatable.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/auth_repository.dart';

part 'otp_notifier.g.dart';

/// Immutable state for the OTP send / resend lifecycle.
class OtpState extends Equatable {
  const OtpState({
    this.isSending = false,
    this.isSent = false,
    this.resendCountdown = 0,
    this.errorMessage,
  });

  /// Whether an OTP send request is currently in flight.
  final bool isSending;

  /// Whether the OTP was successfully sent at least once.
  final bool isSent;

  /// Seconds remaining before the user can request a resend (0 = can resend).
  final int resendCountdown;

  /// Error message from the last failed send attempt, if any.
  final String? errorMessage;

  OtpState copyWith({
    bool? isSending,
    bool? isSent,
    int? resendCountdown,
    String? errorMessage,
  }) =>
      OtpState(
        isSending: isSending ?? this.isSending,
        isSent: isSent ?? this.isSent,
        resendCountdown: resendCountdown ?? this.resendCountdown,
        errorMessage: errorMessage,
      );

  @override
  List<Object?> get props => [isSending, isSent, resendCountdown, errorMessage];
}

/// Manages the OTP send / resend flow including a 30-second cooldown timer.
@riverpod
class OtpNotifier extends _$OtpNotifier {
  Timer? _timer;

  @override
  OtpState build() {
    ref.onDispose(() => _timer?.cancel());
    return const OtpState();
  }

  /// Sends an OTP to [phoneNumber] and starts the resend cooldown timer.
  Future<void> sendOtp(String phoneNumber) async {
    state = state.copyWith(isSending: true, errorMessage: null);

    final repository = ref.read(authRepositoryProvider);
    final failure = await repository.sendOtp(phoneNumber: phoneNumber);

    if (failure != null) {
      state = state.copyWith(
        isSending: false,
        errorMessage: failure.message,
      );
      return;
    }

    state = state.copyWith(
      isSending: false,
      isSent: true,
      resendCountdown: 30,
    );
    _startResendTimer();
  }

  /// Starts a 1-second periodic timer that decrements the countdown.
  void _startResendTimer() {
    _timer?.cancel();
    _timer = Timer.periodic(const Duration(seconds: 1), (timer) {
      final remaining = state.resendCountdown - 1;
      if (remaining <= 0) {
        timer.cancel();
        state = state.copyWith(resendCountdown: 0);
      } else {
        state = state.copyWith(resendCountdown: remaining);
      }
    });
  }
}
