import 'package:flutter/widgets.dart';

import '../../l10n/app_localizations.dart';

/// Shorthand extension for accessing localised strings.
///
/// Usage: `context.l10n.appName` instead of `AppLocalizations.of(context)!.appName`.
extension L10nExtension on BuildContext {
  AppLocalizations get l10n => AppLocalizations.of(this)!;
}
