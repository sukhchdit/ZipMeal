import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/subscription_plan_model.dart';
import '../models/user_subscription_model.dart';

part 'subscription_remote_data_source.g.dart';

@riverpod
SubscriptionRemoteDataSource subscriptionRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return SubscriptionRemoteDataSource(dio: dio);
}

class SubscriptionRemoteDataSource {
  SubscriptionRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  /// GET /api/v1/subscriptions/plans — Get available plans
  Future<List<SubscriptionPlanModel>> getAvailablePlans() async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.subscriptionPlans,
    );
    return response.data!
        .map((e) => SubscriptionPlanModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  /// GET /api/v1/subscriptions/my — Get current subscription
  Future<UserSubscriptionModel?> getMySubscription() async {
    final response = await _dio.get<dynamic>(
      ApiConstants.subscriptionMy,
    );
    if (response.data == null) return null;
    return UserSubscriptionModel.fromJson(
        response.data as Map<String, dynamic>);
  }

  /// POST /api/v1/subscriptions/subscribe — Subscribe to a plan
  Future<UserSubscriptionModel> subscribe(String planId) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.subscriptionSubscribe,
      data: {'planId': planId},
    );
    return UserSubscriptionModel.fromJson(response.data!);
  }

  /// POST /api/v1/subscriptions/cancel — Cancel subscription
  Future<void> cancel() async {
    await _dio.post<dynamic>(ApiConstants.subscriptionCancel);
  }
}
