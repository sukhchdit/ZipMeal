import 'dart:io';

import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:logger/logger.dart';

import '../../features/notifications/presentation/providers/device_registration_notifier.dart';

final _logger = Logger();

/// Manages FCM token lifecycle: permission request, token acquisition,
/// token rotation, and foreground message logging.
///
/// Initialized once from `main.dart` after Firebase init.
class FcmTokenHandler {
  FcmTokenHandler(this._container);

  final ProviderContainer _container;
  String? _currentToken;

  /// Request permission, acquire FCM token, and listen for token refresh.
  Future<void> initialize() async {
    final messaging = FirebaseMessaging.instance;

    // Request notification permission
    final settings = await messaging.requestPermission();
    if (settings.authorizationStatus == AuthorizationStatus.denied) {
      _logger.w('Push notification permission denied');
      return;
    }

    // Get initial token
    final token = await messaging.getToken();
    if (token != null) {
      _currentToken = token;
      await _registerToken(token);
    }

    // Listen for token rotation
    messaging.onTokenRefresh.listen((newToken) async {
      if (_currentToken != null && _currentToken != newToken) {
        await _unregisterToken();
      }
      _currentToken = newToken;
      await _registerToken(newToken);
    });

    // Log foreground messages (system tray handles background/killed automatically)
    FirebaseMessaging.onMessage.listen((message) {
      _logger.i(
        'Foreground FCM message: ${message.notification?.title} - '
        '${message.notification?.body}',
      );
    });
  }

  Future<void> _registerToken(String token) async {
    try {
      final platform = Platform.isAndroid ? 1 : 2;
      await _container
          .read(deviceRegistrationNotifierProvider.notifier)
          .registerDevice(deviceToken: token, platform: platform);
      _logger.i('FCM token registered');
    } catch (e) {
      _logger.e('Failed to register FCM token', error: e);
    }
  }

  Future<void> _unregisterToken() async {
    if (_currentToken == null) return;
    try {
      await _container
          .read(deviceRegistrationNotifierProvider.notifier)
          .unregisterDevice(deviceToken: _currentToken!);
      _logger.i('FCM token unregistered');
    } catch (e) {
      _logger.e('Failed to unregister FCM token', error: e);
    }
  }
}
