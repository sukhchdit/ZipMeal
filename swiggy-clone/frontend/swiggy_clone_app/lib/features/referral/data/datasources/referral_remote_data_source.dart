import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/referral_stats_model.dart';

part 'referral_remote_data_source.g.dart';

@riverpod
ReferralRemoteDataSource referralRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return ReferralRemoteDataSource(dio: dio);
}

class ReferralRemoteDataSource {
  ReferralRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  Future<ReferralStatsModel> getStats() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.referralStats,
    );
    return ReferralStatsModel.fromJson(response.data!);
  }
}
