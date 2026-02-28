import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/referral_remote_data_source.dart';
import '../models/referral_stats_model.dart';

part 'referral_repository.g.dart';

@riverpod
ReferralRepository referralRepository(Ref ref) {
  final remoteDataSource = ref.watch(referralRemoteDataSourceProvider);
  return ReferralRepository(remoteDataSource: remoteDataSource);
}

class ReferralRepository {
  ReferralRepository({required ReferralRemoteDataSource remoteDataSource})
      : _remoteDataSource = remoteDataSource;

  final ReferralRemoteDataSource _remoteDataSource;

  Future<({ReferralStatsModel? data, Failure? failure})> getStats() async {
    try {
      final stats = await _remoteDataSource.getStats();
      return (data: stats, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Failure _mapDioError(DioException e) {
    if (e.type == DioExceptionType.connectionError ||
        e.type == DioExceptionType.connectionTimeout) {
      return const NetworkFailure();
    }

    final statusCode = e.response?.statusCode;
    final data = e.response?.data;
    String message = 'An unexpected error occurred.';

    if (data is Map<String, dynamic>) {
      message = (data['errorMessage'] as String?) ?? message;
    }

    if (statusCode == 401) {
      return AuthFailure(message: message);
    }

    return ServerFailure(message: message, statusCode: statusCode);
  }
}
