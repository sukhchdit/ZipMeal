import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../routing/app_router.dart';
import 'theme/app_theme.dart';

/// Root widget of the Swiggy Clone application.
///
/// Uses [MaterialApp.router] wired to [GoRouter] for declarative navigation
/// and a Material 3 theme inspired by Swiggy's brand palette.
class SwiggyCloneApp extends ConsumerWidget {
  const SwiggyCloneApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final router = ref.watch(appRouterProvider);

    return MaterialApp.router(
      title: 'Swiggy Clone',
      debugShowCheckedModeBanner: false,

      // Theme
      theme: AppTheme.lightTheme,
      darkTheme: AppTheme.darkTheme,
      themeMode: ThemeMode.system,

      // Router
      routerConfig: router,
    );
  }
}
