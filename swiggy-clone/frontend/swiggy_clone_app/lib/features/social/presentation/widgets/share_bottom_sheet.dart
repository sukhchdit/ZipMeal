import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:share_plus/share_plus.dart';

import '../../../../core/extensions/l10n_extensions.dart';

class ShareBottomSheet extends StatelessWidget {
  const ShareBottomSheet({
    super.key,
    required this.shareUrl,
    required this.shareText,
  });

  final String shareUrl;
  final String shareText;

  static Future<void> show(
    BuildContext context, {
    required String shareUrl,
    required String shareText,
  }) {
    return showModalBottomSheet(
      context: context,
      builder: (_) => ShareBottomSheet(
        shareUrl: shareUrl,
        shareText: shareText,
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return SafeArea(
      child: Padding(
        padding: const EdgeInsets.all(16),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(
              context.l10n.shareVia,
              style: theme.textTheme.titleMedium?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            ListTile(
              leading: const Icon(Icons.share_outlined),
              title: Text(context.l10n.shareVia),
              onTap: () {
                Navigator.pop(context);
                Share.share(shareText);
              },
            ),
            ListTile(
              leading: const Icon(Icons.copy_outlined),
              title: Text(context.l10n.copiedToClipboard),
              onTap: () {
                Clipboard.setData(ClipboardData(text: shareUrl));
                Navigator.pop(context);
                ScaffoldMessenger.of(context).showSnackBar(
                  SnackBar(content: Text(context.l10n.copiedToClipboard)),
                );
              },
            ),
          ],
        ),
      ),
    );
  }
}
