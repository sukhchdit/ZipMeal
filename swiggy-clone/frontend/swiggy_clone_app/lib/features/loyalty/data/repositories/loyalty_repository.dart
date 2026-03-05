import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/loyalty_remote_data_source.dart';
import '../models/loyalty_dashboard_model.dart';
import '../models/loyalty_reward_model.dart';

part 'loyalty_repository.g.dart';

@riverpod
LoyaltyRepository loyaltyRepository(Ref ref) {
  final remoteDataSource = ref.watch(loyaltyRemoteDataSourceProvider);
  return LoyaltyRepository(remoteDataSource: remoteDataSource);
}

class LoyaltyRepository {
  LoyaltyRepository({required LoyaltyRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final LoyaltyRemoteDataSource _remote;

  Future<({LoyaltyDashboardModel? data, Failure? failure})>
      getDashboard() async {
    try {
      final result = await _remote.getDashboard();
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<
      ({
        List<LoyaltyTransactionModel>? items,
        int? totalCount,
        int? page,
        int? totalPages,
        Failure? failure,
      })> getTransactions({int page = 1, int pageSize = 20, int? type}) async {
    try {
      final data = await _remote.getTransactions(
        page: page,
        pageSize: pageSize,
        type: type,
      );
      final items = (data['items'] as List<dynamic>)
          .map(
            (e) =>
                LoyaltyTransactionModel.fromJson(e as Map<String, dynamic>),
          )
          .toList();
      return (
        items: items,
        totalCount: data['totalCount'] as int?,
        page: data['page'] as int?,
        totalPages: data['totalPages'] as int?,
        failure: null,
      );
    } on DioException catch (e) {
      return (
        items: null,
        totalCount: null,
        page: null,
        totalPages: null,
        failure: _mapDioError(e),
      );
    }
  }

  Future<({List<LoyaltyRewardModel>? data, Failure? failure})>
      getRewards() async {
    try {
      final result = await _remote.getRewards();
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({bool success, Failure? failure})> redeemReward(
    String rewardId,
  ) async {
    try {
      await _remote.redeemReward(rewardId);
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
