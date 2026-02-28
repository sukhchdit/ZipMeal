import 'dart:async';

import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:hive_flutter/hive_flutter.dart';
import 'package:logger/logger.dart';

import 'app/app.dart';
import 'core/services/fcm_token_handler.dart';
import 'core/storage/local_storage_service.dart';

final Logger _logger = Logger(
  printer: PrettyPrinter(
    methodCount: 2,
    errorMethodCount: 8,
    lineLength: 120,
    colors: true,
    printEmojis: false,
    dateTimeFormat: DateTimeFormat.onlyTimeAndSinceStart,
  ),
);

/// Top-level handler for Firebase background messages.
///
/// Must be a top-level function (not a class method) so the OS can invoke it
/// when the app is terminated or in the background.
@pragma('vm:entry-point')
Future<void> _firebaseMessagingBackgroundHandler(RemoteMessage message) async {
  _logger.i('Handling background message: ${message.messageId}');
}

Future<void> main() async {
  await runZonedGuarded(
    () async {
      WidgetsFlutterBinding.ensureInitialized();

      // ---- Platform error handling ----
      FlutterError.onError = (FlutterErrorDetails details) {
        FlutterError.presentError(details);
        _logger.e(
          'FlutterError caught',
          error: details.exception,
          stackTrace: details.stack,
        );
      };

      PlatformDispatcher.instance.onError = (Object error, StackTrace stack) {
        _logger.e(
          'Uncaught platform error',
          error: error,
          stackTrace: stack,
        );
        return true;
      };

      // ---- Hive (local storage) initialization ----
      await Hive.initFlutter();
      await LocalStorageService.instance.init();

      // ---- Firebase initialization ----
      // Graceful fallback when google-services.json / GoogleService-Info.plist
      // are missing — the app works without push notifications.
      bool firebaseInitialized = false;
      try {
        await Firebase.initializeApp();
        firebaseInitialized = true;
      } catch (e) {
        _logger.w('Firebase init failed (config files missing?)', error: e);
      }

      if (firebaseInitialized) {
        FirebaseMessaging.onBackgroundMessage(
          _firebaseMessagingBackgroundHandler,
        );
      }

      // ---- Run the app inside a ProviderScope ----
      final container = ProviderContainer();

      if (firebaseInitialized) {
        final fcmHandler = FcmTokenHandler(container);
        await fcmHandler.initialize();
      }

      runApp(
        UncontrolledProviderScope(
          container: container,
          child: const SwiggyCloneApp(),
        ),
      );
    },
    (Object error, StackTrace stack) {
      _logger.e(
        'Unhandled zone error',
        error: error,
        stackTrace: stack,
      );
    },
  );
}
