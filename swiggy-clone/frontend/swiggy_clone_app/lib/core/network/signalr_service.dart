import 'dart:async';

import 'package:logger/logger.dart';
import 'package:signalr_netcore/signalr_client.dart';

import '../constants/api_constants.dart';
import '../storage/secure_storage_service.dart';

/// Manages SignalR hub connections for real-time order tracking and dine-in events.
class SignalRService {
  final SecureStorageService _secureStorage;
  final _logger = Logger();

  HubConnection? _orderTrackingHub;
  HubConnection? _dineInHub;
  HubConnection? _chatSupportHub;
  HubConnection? _groupOrderHub;

  // Callbacks
  void Function(Map<String, dynamic>)? onOrderStatusChanged;
  void Function(Map<String, dynamic>)? onDeliveryLocationUpdated;
  void Function(Map<String, dynamic>)? onDineInEvent;
  void Function(Map<String, dynamic>)? onNewChatMessage;
  void Function(Map<String, dynamic>)? onTypingIndicator;
  void Function(Map<String, dynamic>)? onGroupOrderEvent;

  SignalRService({required SecureStorageService secureStorage})
      : _secureStorage = secureStorage;

  bool get isConnected =>
      _orderTrackingHub?.state == HubConnectionState.Connected;

  // ─────────────────── Connection Lifecycle ─────────────────────

  Future<void> connect() async {
    final token = await _secureStorage.getAccessToken();
    if (token == null) return;

    await _connectOrderTrackingHub(token);
    await _connectDineInHub(token);
    await _connectChatSupportHub(token);
    await _connectGroupOrderHub(token);
  }

  Future<void> disconnect() async {
    try {
      await _orderTrackingHub?.stop();
      await _dineInHub?.stop();
      await _chatSupportHub?.stop();
      await _groupOrderHub?.stop();
    } catch (e) {
      _logger.w('Error disconnecting SignalR hubs: $e');
    }
    _orderTrackingHub = null;
    _dineInHub = null;
    _chatSupportHub = null;
    _groupOrderHub = null;
  }

  // ─────────────── Order Tracking Hub ───────────────────────────

  Future<void> _connectOrderTrackingHub(String token) async {
    _orderTrackingHub = HubConnectionBuilder()
        .withUrl(
          ApiConstants.hubOrderTracking,
          options: HttpConnectionOptions(
            accessTokenFactory: () async => token,
          ),
        )
        .withAutomaticReconnect()
        .build();

    _orderTrackingHub!.on('OrderStatusChanged', (args) {
      if (args != null && args.isNotEmpty) {
        final data = args[0] as Map<String, dynamic>? ?? {};
        onOrderStatusChanged?.call(data);
      }
    });

    _orderTrackingHub!.on('DeliveryLocationUpdated', (args) {
      if (args != null && args.isNotEmpty) {
        final data = args[0] as Map<String, dynamic>? ?? {};
        onDeliveryLocationUpdated?.call(data);
      }
    });

    try {
      await _orderTrackingHub!.start();
      _logger.i('OrderTracking SignalR hub connected');
    } catch (e) {
      _logger.w('Failed to connect OrderTracking hub: $e');
    }
  }

  Future<void> subscribeToOrder(String orderId) async {
    try {
      await _orderTrackingHub?.invoke('SubscribeToOrder', args: [orderId]);
    } catch (e) {
      _logger.w('Failed to subscribe to order $orderId: $e');
    }
  }

  Future<void> unsubscribeFromOrder(String orderId) async {
    try {
      await _orderTrackingHub?.invoke('UnsubscribeFromOrder', args: [orderId]);
    } catch (e) {
      _logger.w('Failed to unsubscribe from order $orderId: $e');
    }
  }

  // ─────────────── Dine-In Hub ──────────────────────────────────

  Future<void> _connectDineInHub(String token) async {
    _dineInHub = HubConnectionBuilder()
        .withUrl(
          ApiConstants.hubDineIn,
          options: HttpConnectionOptions(
            accessTokenFactory: () async => token,
          ),
        )
        .withAutomaticReconnect()
        .build();

    _dineInHub!.on('DineInEvent', (args) {
      if (args != null && args.isNotEmpty) {
        final data = args[0] as Map<String, dynamic>? ?? {};
        onDineInEvent?.call(data);
      }
    });

    try {
      await _dineInHub!.start();
      _logger.i('DineIn SignalR hub connected');
    } catch (e) {
      _logger.w('Failed to connect DineIn hub: $e');
    }
  }

  Future<void> joinDineInSession(String sessionId) async {
    try {
      await _dineInHub?.invoke('JoinSession', args: [sessionId]);
    } catch (e) {
      _logger.w('Failed to join dine-in session $sessionId: $e');
    }
  }

  Future<void> leaveDineInSession(String sessionId) async {
    try {
      await _dineInHub?.invoke('LeaveSession', args: [sessionId]);
    } catch (e) {
      _logger.w('Failed to leave dine-in session $sessionId: $e');
    }
  }

  // ─────────────── Chat Support Hub ───────────────────────────

  Future<void> _connectChatSupportHub(String token) async {
    _chatSupportHub = HubConnectionBuilder()
        .withUrl(
          ApiConstants.hubChatSupport,
          options: HttpConnectionOptions(
            accessTokenFactory: () async => token,
          ),
        )
        .withAutomaticReconnect()
        .build();

    _chatSupportHub!.on('NewChatMessage', (args) {
      if (args != null && args.isNotEmpty) {
        final data = args[0] as Map<String, dynamic>? ?? {};
        onNewChatMessage?.call(data);
      }
    });

    _chatSupportHub!.on('TypingIndicator', (args) {
      if (args != null && args.isNotEmpty) {
        final data = args[0] as Map<String, dynamic>? ?? {};
        onTypingIndicator?.call(data);
      }
    });

    try {
      await _chatSupportHub!.start();
      _logger.i('ChatSupport SignalR hub connected');
    } catch (e) {
      _logger.w('Failed to connect ChatSupport hub: $e');
    }
  }

  Future<void> joinChatTicket(String ticketId) async {
    try {
      await _chatSupportHub?.invoke('JoinTicket', args: [ticketId]);
    } catch (e) {
      _logger.w('Failed to join chat ticket $ticketId: $e');
    }
  }

  Future<void> leaveChatTicket(String ticketId) async {
    try {
      await _chatSupportHub?.invoke('LeaveTicket', args: [ticketId]);
    } catch (e) {
      _logger.w('Failed to leave chat ticket $ticketId: $e');
    }
  }

  Future<void> sendTypingIndicator(String ticketId, bool isTyping) async {
    try {
      await _chatSupportHub?.invoke('SendTypingIndicator', args: [ticketId, isTyping]);
    } catch (e) {
      _logger.w('Failed to send typing indicator: $e');
    }
  }

  // ─────────────── Group Order Hub ───────────────────────────

  Future<void> _connectGroupOrderHub(String token) async {
    _groupOrderHub = HubConnectionBuilder()
        .withUrl(
          ApiConstants.hubGroupOrder,
          options: HttpConnectionOptions(
            accessTokenFactory: () async => token,
          ),
        )
        .withAutomaticReconnect()
        .build();

    _groupOrderHub!.on('GroupOrderEvent', (args) {
      if (args != null && args.isNotEmpty) {
        final data = args[0] as Map<String, dynamic>? ?? {};
        onGroupOrderEvent?.call(data);
      }
    });

    try {
      await _groupOrderHub!.start();
      _logger.i('GroupOrder SignalR hub connected');
    } catch (e) {
      _logger.w('Failed to connect GroupOrder hub: $e');
    }
  }

  Future<void> joinGroupOrder(String groupOrderId) async {
    try {
      await _groupOrderHub?.invoke('JoinGroupOrder', args: [groupOrderId]);
    } catch (e) {
      _logger.w('Failed to join group order $groupOrderId: $e');
    }
  }

  Future<void> leaveGroupOrder(String groupOrderId) async {
    try {
      await _groupOrderHub?.invoke('LeaveGroupOrder', args: [groupOrderId]);
    } catch (e) {
      _logger.w('Failed to leave group order $groupOrderId: $e');
    }
  }
}
