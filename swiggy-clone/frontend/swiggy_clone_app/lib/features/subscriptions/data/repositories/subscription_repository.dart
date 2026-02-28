import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/subscription_remote_data_source.dart';
import '../models/subscription_plan_model.dart';
import '../models/user_subscription_model.dart';

part 'subscription_repository.g.dart';

@riverpod
SubscriptionRepository subscriptionRepository(Ref ref) {
  final remoteDataSource = ref.watch(subscriptionRemoteDataSourceProvider);
  return SubscriptionRepository(remoteDataSource: remoteDataSource);
}

class SubscriptionRepository {
  SubscriptionRepository({
    required SubscriptionRemoteDataSource remoteDataSource,
  }) : _remote = remoteDataSource;

  final SubscriptionRemoteDataSource _remote;

  Future<({List<SubscriptionPlanModel>? data, Failure? failure})>
      getAvailablePlans() async {
    try {
      final result = await _remote.getAvailablePlans();
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({UserSubscriptionModel? data, Failure? failure})>
      getMySubscription() async {
    try {
      final result = await _remote.getMySubscription();
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({UserSubscriptionModel? data, Failure? failure})> subscribe(
      String planId) async {
    try {
      final result = await _remote.subscribe(planId);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({bool success, Failure? failure})> cancel() async {
    try {
      await _remote.cancel();
      return (success: true, failure: null);
    } on DioException catch (e) {
      return (success: false, failure: _mapDioError(e));
    }
  }

  Failure _mapDioError(DioException e) {
    if (e.type == DioExceptionType.connectionError ||
        e.type == DioExceptionType.connectionTimeout) {
      return const NetworkFailure();
    }
    final statusCode = e.response?.statusCode;
    final data = e.response?.data;
    var message = 'An unexpected error occurred.';
    if (data is Map<String, dynamic>) {
      message = (data['errorMessage'] as String?) ?? message;
    }
    if (statusCode == 401) return AuthFailure(message: message);
    return ServerFailure(message: message, statusCode: statusCode);
  }
}
