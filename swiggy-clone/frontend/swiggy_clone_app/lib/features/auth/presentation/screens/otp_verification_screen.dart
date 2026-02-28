import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../routing/route_names.dart';
import '../providers/auth_notifier.dart';
import '../providers/auth_state.dart';
import '../providers/otp_notifier.dart';

/// OTP verification screen used for both login and registration flows.
///
/// Displays 6 individual digit input boxes with auto-advance behaviour.
/// Automatically triggers verification once all 6 digits are entered.
class OtpVerificationScreen extends ConsumerStatefulWidget {
  const OtpVerificationScreen({
    required this.phoneNumber,
    required this.isLogin,
    this.fullName,
    super.key,
  });

  /// The phone number the OTP was sent to.
  final String phoneNumber;

  /// Whether this screen is being used for login (`true`) or registration.
  final bool isLogin;

  /// The user's full name -- only provided during registration.
  final String? fullName;

  @override
  ConsumerState<OtpVerificationScreen> createState() =>
      _OtpVerificationScreenState();
}

class _OtpVerificationScreenState
    extends ConsumerState<OtpVerificationScreen> {
  final List<TextEditingController> _otpControllers =
      List.generate(6, (_) => TextEditingController());
  final List<FocusNode> _focusNodes = List.generate(6, (_) => FocusNode());

  @override
  void initState() {
    super.initState();
    // Auto-send OTP when screen loads.
    WidgetsBinding.instance.addPostFrameCallback((_) {
      ref.read(otpNotifierProvider.notifier).sendOtp(widget.phoneNumber);
    });
  }

  @override
  void dispose() {
    for (final c in _otpControllers) {
      c.dispose();
    }
    for (final f in _focusNodes) {
      f.dispose();
    }
    super.dispose();
  }

  String get _otpCode => _otpControllers.map((c) => c.text).join();

  void _verifyOtp() {
    final otp = _otpCode;
    if (otp.length != 6) return;

    if (widget.isLogin) {
      ref.read(authNotifierProvider.notifier).loginByPhone(
            phoneNumber: widget.phoneNumber,
            otp: otp,
          );
    } else {
      ref.read(authNotifierProvider.notifier).registerByPhone(
            phoneNumber: widget.phoneNumber,
            otp: otp,
            fullName: widget.fullName ?? '',
          );
    }
  }

  @override
  Widget build(BuildContext context) {
    final authState = ref.watch(authNotifierProvider);
    final otpState = ref.watch(otpNotifierProvider);
    final theme = Theme.of(context);

    ref.listen<AuthState>(authNotifierProvider, (previous, next) {
      switch (next) {
        case AuthAuthenticated():
          context.go(RouteNames.home);
        case AuthError(:final failure):
          ScaffoldMessenger.of(context).showSnackBar(
            SnackBar(
              content: Text(failure.message),
              backgroundColor: Colors.red,
            ),
          );
        default:
          break;
      }
    });

    final isLoading = authState is AuthAuthenticating || otpState.isSending;

    return Scaffold(
      appBar: AppBar(title: const Text('Verify OTP')),
      body: SafeArea(
        child: Padding(
          padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 32),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              Text(
                'Enter verification code',
                style: theme.textTheme.headlineSmall?.copyWith(
                  fontWeight: FontWeight.bold,
                ),
              ),
              const SizedBox(height: 8),
              Text(
                'We sent a 6-digit code to ${widget.phoneNumber}',
                style: theme.textTheme.bodyMedium?.copyWith(
                  color: Colors.grey[600],
                ),
              ),
              const SizedBox(height: 8),
              Text(
                'Dev mode: use 123456',
                style: theme.textTheme.bodySmall?.copyWith(
                  color: AppColors.primary,
                  fontStyle: FontStyle.italic,
                ),
              ),
              const SizedBox(height: 32),

              // OTP input boxes
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                children: List.generate(6, (index) {
                  return SizedBox(
                    width: 48,
                    child: TextFormField(
                      controller: _otpControllers[index],
                      focusNode: _focusNodes[index],
                      keyboardType: TextInputType.number,
                      textAlign: TextAlign.center,
                      maxLength: 1,
                      style: theme.textTheme.headlineSmall,
                      decoration: const InputDecoration(
                        counterText: '',
                        border: OutlineInputBorder(),
                      ),
                      inputFormatters: [
                        FilteringTextInputFormatter.digitsOnly,
                      ],
                      onChanged: (value) {
                        if (value.isNotEmpty && index < 5) {
                          _focusNodes[index + 1].requestFocus();
                        }
                        if (value.isEmpty && index > 0) {
                          _focusNodes[index - 1].requestFocus();
                        }
                        if (_otpCode.length == 6) {
                          _verifyOtp();
                        }
                      },
                    ),
                  );
                }),
              ),
              const SizedBox(height: 32),

              FilledButton(
                onPressed:
                    isLoading || _otpCode.length != 6 ? null : _verifyOtp,
                style: FilledButton.styleFrom(
                  backgroundColor: AppColors.primary,
                  minimumSize: const Size.fromHeight(52),
                ),
                child: isLoading
                    ? const SizedBox(
                        height: 20,
                        width: 20,
                        child: CircularProgressIndicator(
                          strokeWidth: 2,
                          color: Colors.white,
                        ),
                      )
                    : const Text('Verify'),
              ),
              const SizedBox(height: 16),

              // Resend OTP
              Center(
                child: otpState.resendCountdown > 0
                    ? Text(
                        'Resend OTP in ${otpState.resendCountdown}s',
                        style: theme.textTheme.bodyMedium?.copyWith(
                          color: Colors.grey[600],
                        ),
                      )
                    : TextButton(
                        onPressed: otpState.isSending
                            ? null
                            : () => ref
                                .read(otpNotifierProvider.notifier)
                                .sendOtp(widget.phoneNumber),
                        child: const Text(
                          'Resend OTP',
                          style: TextStyle(color: AppColors.primary),
                        ),
                      ),
              ),

              if (otpState.errorMessage != null)
                Padding(
                  padding: const EdgeInsets.only(top: 16),
                  child: Text(
                    otpState.errorMessage!,
                    style: const TextStyle(color: Colors.red),
                    textAlign: TextAlign.center,
                  ),
                ),
            ],
          ),
        ),
      ),
    );
  }
}
