import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../core/constants/locale_constants.dart';
import '../../../../core/extensions/l10n_extensions.dart';
import '../../../../core/providers/locale_notifier.dart';
import '../widgets/language_tile_widget.dart';

/// Language picker screen with radio tiles for each supported locale.
class LanguageScreen extends ConsumerWidget {
  const LanguageScreen({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final currentLocale = ref.watch(localeNotifierProvider);

    return Scaffold(
      appBar: AppBar(
        title: Text(context.l10n.language),
      ),
      body: ListView.separated(
        padding: const EdgeInsets.symmetric(vertical: 8),
        itemCount: LocaleConstants.supportedLocales.length,
        separatorBuilder: (_, __) => const Divider(height: 1),
        itemBuilder: (context, index) {
          final locale = LocaleConstants.supportedLocales[index];
          final code = locale.languageCode;

          return LanguageTileWidget(
            languageName:
                LocaleConstants.localeDisplayNames[code] ?? code,
            nativeName:
                LocaleConstants.localeNativeNames[code] ?? code,
            isSelected: currentLocale.languageCode == code,
            onTap: () {
              ref
                  .read(localeNotifierProvider.notifier)
                  .setLocale(locale);
            },
          );
        },
      ),
    );
  }
}
