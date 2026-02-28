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

  // Callbacks
  void Function(Map<String, dynamic>)? onOrderStatusChanged;
  void Function(Map<String, dynamic>)? onDeliveryLocationUpdated;
  void Function(Map<String, dynamic>)? onDineInEvent;

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
  }

  Future<void> disconnect() async {
    try {
      await _orderTrackingHub?.stop();
      await _dineInHub?.stop();
    } catch (e) {
      _logger.w('Error disconnecting SignalR hubs: $e');
    }
    _orderTrackingHub = null;
    _dineInHub = null;
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
}
