import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';

import '../../../../app/theme/app_colors.dart';
import '../../../../core/storage/secure_storage_service.dart';
import '../providers/chat_websocket_notifier.dart';
import '../providers/messages_notifier.dart';
import '../providers/messages_state.dart';
import '../providers/typing_indicator_notifier.dart';
import '../../data/repositories/chat_support_repository.dart';
import '../widgets/message_bubble_widget.dart';
import '../widgets/typing_indicator_widget.dart';
import '../widgets/canned_response_sheet.dart';

class ChatConversationScreen extends ConsumerStatefulWidget {
  const ChatConversationScreen({super.key, required this.ticketId});

  final String ticketId;

  @override
  ConsumerState<ChatConversationScreen> createState() =>
      _ChatConversationScreenState();
}

class _ChatConversationScreenState
    extends ConsumerState<ChatConversationScreen> {
  final _messageController = TextEditingController();
  final _scrollController = ScrollController();
  String? _currentUserId;
  Timer? _typingDebounce;

  @override
  void initState() {
    super.initState();
    _loadCurrentUserId();
    _scrollController.addListener(_onScroll);
  }

  Future<void> _loadCurrentUserId() async {
    final storage = ref.read(secureStorageServiceProvider);
    final userId = await storage.getUserId();
    if (mounted) {
      setState(() => _currentUserId = userId);
    }
  }

  void _onScroll() {
    if (_scrollController.position.pixels >=
        _scrollController.position.maxScrollExtent - 200) {
      ref
          .read(messagesNotifierProvider(widget.ticketId).notifier)
          .loadMore();
    }
  }

  @override
  void dispose() {
    _messageController.dispose();
    _scrollController.dispose();
    _typingDebounce?.cancel();
    super.dispose();
  }

  void _onTextChanged(String text) {
    _typingDebounce?.cancel();
    if (text.isNotEmpty) {
      ref
          .read(chatWebSocketNotifierProvider(widget.ticketId).notifier)
          .sendTypingIndicator(true);
      _typingDebounce = Timer(const Duration(seconds: 2), () {
        ref
            .read(chatWebSocketNotifierProvider(widget.ticketId).notifier)
            .sendTypingIndicator(false);
      });
    }
  }

  Future<void> _sendMessage() async {
    final content = _messageController.text.trim();
    if (content.isEmpty) return;

    _messageController.clear();
    ref
        .read(chatWebSocketNotifierProvider(widget.ticketId).notifier)
        .sendTypingIndicator(false);

    await ref
        .read(messagesNotifierProvider(widget.ticketId).notifier)
        .sendMessage(content);
  }

  void _showCannedResponses() async {
    final result = await showModalBottomSheet<String>(
      context: context,
      isScrollControlled: true,
      builder: (_) => const CannedResponseSheet(),
    );
    if (result != null && mounted) {
      _messageController.text = result;
    }
  }

  @override
  Widget build(BuildContext context) {
    // Connect WebSocket
    ref.watch(chatWebSocketNotifierProvider(widget.ticketId));

    final messagesState = ref.watch(messagesNotifierProvider(widget.ticketId));
    final isTyping =
        ref.watch(typingIndicatorNotifierProvider(widget.ticketId));

    // Mark messages as read
    if (messagesState is MessagesLoaded) {
      ref
          .read(chatSupportRepositoryProvider)
          .markMessagesRead(widget.ticketId);
    }

    return Scaffold(
      appBar: AppBar(
        title: const Text('Chat Support'),
      ),
      body: Column(
        children: [
          Expanded(
            child: switch (messagesState) {
              MessagesInitial() || MessagesLoading() => const Center(
                  child: CircularProgressIndicator(),
                ),
              MessagesError(:final failure) => Center(
                  child: Column(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(failure.message),
                      const SizedBox(height: 16),
                      ElevatedButton(
                        onPressed: () => ref
                            .read(messagesNotifierProvider(widget.ticketId)
                                .notifier)
                            .loadMessages(),
                        child: const Text('Retry'),
                      ),
                    ],
                  ),
                ),
              MessagesLoaded(
                :final messages,
                :final hasMore,
                :final isLoadingMore,
              ) =>
                messages.isEmpty
                    ? Center(
                        child: Text(
                          'No messages yet. Send the first one!',
                          style:
                              Theme.of(context).textTheme.bodyMedium?.copyWith(
                                    color: AppColors.textSecondaryLight,
                                  ),
                        ),
                      )
                    : ListView.builder(
                        controller: _scrollController,
                        reverse: true,
                        padding: const EdgeInsets.symmetric(
                            horizontal: 16, vertical: 8),
                        itemCount:
                            messages.length + (isTyping ? 1 : 0) + (hasMore && isLoadingMore ? 1 : 0),
                        itemBuilder: (context, index) {
                          if (isTyping && index == 0) {
                            return const TypingIndicatorWidget();
                          }
                          final msgIndex = isTyping ? index - 1 : index;
                          if (msgIndex >= messages.length) {
                            return const Center(
                              child: Padding(
                                padding: EdgeInsets.all(16),
                                child: CircularProgressIndicator(),
                              ),
                            );
                          }
                          final message = messages[msgIndex];
                          return MessageBubbleWidget(
                            message: message,
                            isCurrentUser:
                                message.senderId == _currentUserId,
                          );
                        },
                      ),
            },
          ),
          _MessageInputBar(
            controller: _messageController,
            onSend: _sendMessage,
            onChanged: _onTextChanged,
            onCannedTap: _showCannedResponses,
          ),
        ],
      ),
    );
  }
}

class _MessageInputBar extends StatelessWidget {
  const _MessageInputBar({
    required this.controller,
    required this.onSend,
    required this.onChanged,
    required this.onCannedTap,
  });

  final TextEditingController controller;
  final VoidCallback onSend;
  final ValueChanged<String> onChanged;
  final VoidCallback onCannedTap;

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: EdgeInsets.only(
        left: 16,
        right: 8,
        top: 8,
        bottom: MediaQuery.of(context).padding.bottom + 8,
      ),
      decoration: BoxDecoration(
        color: Theme.of(context).colorScheme.surface,
        border: Border(
          top: BorderSide(
            color: Theme.of(context).dividerColor,
          ),
        ),
      ),
      child: Row(
        children: [
          IconButton(
            icon: const Icon(Icons.flash_on_outlined),
            onPressed: onCannedTap,
            tooltip: 'Quick responses',
            color: AppColors.textSecondaryLight,
          ),
          Expanded(
            child: TextField(
              controller: controller,
              onChanged: onChanged,
              onSubmitted: (_) => onSend(),
              textInputAction: TextInputAction.send,
              maxLines: 4,
              minLines: 1,
              decoration: InputDecoration(
                hintText: 'Type a message...',
                border: OutlineInputBorder(
                  borderRadius: BorderRadius.circular(24),
                  borderSide: BorderSide.none,
                ),
                filled: true,
                fillColor:
                    Theme.of(context).colorScheme.surfaceContainerHighest,
                contentPadding: const EdgeInsets.symmetric(
                    horizontal: 16, vertical: 10),
              ),
            ),
          ),
          const SizedBox(width: 8),
          IconButton.filled(
            onPressed: onSend,
            icon: const Icon(Icons.send_rounded),
          ),
        ],
      ),
    );
  }
}
