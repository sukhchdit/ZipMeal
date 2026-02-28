import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/notification_model.dart';

part 'notification_remote_data_source.g.dart';

@riverpod
NotificationRemoteDataSource notificationRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return NotificationRemoteDataSource(dio: dio);
}

class NotificationRemoteDataSource {
  NotificationRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  Future<Map<String, dynamic>> getMyNotifications({
    String? cursor,
    int pageSize = 20,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.notifications,
      queryParameters: {
        if (cursor != null) 'cursor': cursor,
        'pageSize': pageSize,
      },
    );
    return response.data!;
  }

  Future<int> getUnreadCount() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.notificationUnreadCount,
    );
    return response.data!['count'] as int;
  }

  Future<void> markAsRead({required String id}) async {
    await _dio.put(ApiConstants.notificationMarkRead(id));
  }

  Future<void> markAllAsRead() async {
    await _dio.put(ApiConstants.notificationReadAll);
  }

  Future<void> registerDevice({
    required String deviceToken,
    required int platform,
  }) async {
    await _dio.post(
      ApiConstants.notificationDevices,
      data: {
        'deviceToken': deviceToken,
        'platform': platform,
      },
    );
  }

  Future<void> unregisterDevice({required String deviceToken}) async {
    await _dio.delete(
      ApiConstants.notificationDevices,
      data: {'deviceToken': deviceToken},
    );
  }
}
