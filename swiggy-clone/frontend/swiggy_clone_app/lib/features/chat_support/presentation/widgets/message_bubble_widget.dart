import 'package:flutter/material.dart';

import '../../../../app/theme/app_colors.dart';
import '../../data/models/support_message_model.dart';

class MessageBubbleWidget extends StatelessWidget {
  const MessageBubbleWidget({
    super.key,
    required this.message,
    required this.isCurrentUser,
  });

  final SupportMessageModel message;
  final bool isCurrentUser;

  @override
  Widget build(BuildContext context) {
    // System messages are centered
    if (message.messageType == 1) {
      return _SystemMessage(content: message.content);
    }

    return Align(
      alignment: isCurrentUser ? Alignment.centerRight : Alignment.centerLeft,
      child: Container(
        constraints: BoxConstraints(
          maxWidth: MediaQuery.of(context).size.width * 0.75,
        ),
        margin: const EdgeInsets.symmetric(vertical: 4),
        padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 10),
        decoration: BoxDecoration(
          color: isCurrentUser
              ? AppColors.primary
              : Theme.of(context).colorScheme.surfaceContainerHighest,
          borderRadius: BorderRadius.only(
            topLeft: const Radius.circular(16),
            topRight: const Radius.circular(16),
            bottomLeft: Radius.circular(isCurrentUser ? 16 : 4),
            bottomRight: Radius.circular(isCurrentUser ? 4 : 16),
          ),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            if (!isCurrentUser)
              Padding(
                padding: const EdgeInsets.only(bottom: 4),
                child: Text(
                  message.senderName,
                  style: Theme.of(context).textTheme.labelSmall?.copyWith(
                        color: isCurrentUser
                            ? Colors.white70
                            : AppColors.textSecondaryLight,
                        fontWeight: FontWeight.w600,
                      ),
                ),
              ),
            Text(
              message.content,
              style: Theme.of(context).textTheme.bodyMedium?.copyWith(
                    color: isCurrentUser ? Colors.white : null,
                  ),
            ),
            const SizedBox(height: 4),
            Text(
              _formatTime(message.createdAt),
              style: Theme.of(context).textTheme.labelSmall?.copyWith(
                    color: isCurrentUser
                        ? Colors.white60
                        : AppColors.textTertiaryLight,
                    fontSize: 10,
                  ),
            ),
          ],
        ),
      ),
    );
  }

  String _formatTime(String dateStr) {
    try {
      final dt = DateTime.parse(dateStr).toLocal();
      final hour = dt.hour.toString().padLeft(2, '0');
      final minute = dt.minute.toString().padLeft(2, '0');
      return '$hour:$minute';
    } catch (_) {
      return '';
    }
  }
}

class _SystemMessage extends StatelessWidget {
  const _SystemMessage({required this.content});

  final String content;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Container(
        margin: const EdgeInsets.symmetric(vertical: 8),
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
        decoration: BoxDecoration(
          color: Theme.of(context)
              .colorScheme
              .surfaceContainerHighest
              .withAlpha(128),
          borderRadius: BorderRadius.circular(12),
        ),
        child: Text(
          content,
          style: Theme.of(context).textTheme.bodySmall?.copyWith(
                color: AppColors.textTertiaryLight,
                fontStyle: FontStyle.italic,
              ),
          textAlign: TextAlign.center,
        ),
      ),
    );
  }
}
