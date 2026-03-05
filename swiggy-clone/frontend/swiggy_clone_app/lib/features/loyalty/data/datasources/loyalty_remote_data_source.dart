import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/loyalty_dashboard_model.dart';
import '../models/loyalty_reward_model.dart';

part 'loyalty_remote_data_source.g.dart';

@riverpod
LoyaltyRemoteDataSource loyaltyRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return LoyaltyRemoteDataSource(dio: dio);
}

class LoyaltyRemoteDataSource {
  LoyaltyRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  /// GET /api/v1/loyalty/dashboard
  Future<LoyaltyDashboardModel> getDashboard() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.loyaltyDashboard,
    );
    return LoyaltyDashboardModel.fromJson(response.data!);
  }

  /// GET /api/v1/loyalty/transactions
  Future<Map<String, dynamic>> getTransactions({
    int page = 1,
    int pageSize = 20,
    int? type,
  }) async {
    final queryParams = <String, dynamic>{
      'page': page,
      'pageSize': pageSize,
    };
    if (type != null) {
      queryParams['type'] = type;
    }
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.loyaltyTransactions,
      queryParameters: queryParams,
    );
    return response.data!;
  }

  /// GET /api/v1/loyalty/rewards
  Future<List<LoyaltyRewardModel>> getRewards() async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.loyaltyRewards,
    );
    return response.data!
        .map((e) => LoyaltyRewardModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  /// POST /api/v1/loyalty/redeem/{rewardId}
  Future<Map<String, dynamic>> redeemReward(String rewardId) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.loyaltyRedeem(rewardId),
    );
    return response.data!;
  }
}
