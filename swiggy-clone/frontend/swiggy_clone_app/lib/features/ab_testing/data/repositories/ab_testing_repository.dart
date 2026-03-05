import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/ab_testing_remote_data_source.dart';
import '../models/experiment_model.dart';
import '../models/experiment_stats_model.dart';
import '../models/user_assignment_model.dart';

part 'ab_testing_repository.g.dart';

@riverpod
AbTestingRepository abTestingRepository(Ref ref) {
  final remote = ref.watch(abTestingRemoteDataSourceProvider);
  return AbTestingRepository(remote: remote);
}

class AbTestingRepository {
  AbTestingRepository({required AbTestingRemoteDataSource remote})
      : _remote = remote;
  final AbTestingRemoteDataSource _remote;

  Future<({ExperimentModel? data, Failure? failure})> createExperiment({
    required String key,
    required String name,
    String? description,
    String? targetAudience,
    String? startDate,
    String? endDate,
    String? goalDescription,
    required List<Map<String, dynamic>> variants,
  }) async {
    try {
      final result = await _remote.createExperiment(
        key: key,
        name: name,
        description: description,
        targetAudience: targetAudience,
        startDate: startDate,
        endDate: endDate,
        goalDescription: goalDescription,
        variants: variants,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<
      ({
        List<ExperimentModel>? data,
        int? totalCount,
        int? page,
        int? pageSize,
        Failure? failure,
      })> getExperiments({int? status, int page = 1, int pageSize = 20}) async {
    try {
      final result = await _remote.getExperiments(
        status: status,
        page: page,
        pageSize: pageSize,
      );
      return (
        data: result.items,
        totalCount: result.totalCount,
        page: result.page,
        pageSize: result.pageSize,
        failure: null,
      );
    } on DioException catch (e) {
      return (
        data: null,
        totalCount: null,
        page: null,
        pageSize: null,
        failure: _mapDioError(e),
      );
    }
  }

  Future<({ExperimentModel? data, Failure? failure})> getExperimentById(
      String id) async {
    try {
      final result = await _remote.getExperimentById(id);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({ExperimentModel? data, Failure? failure})> updateExperiment({
    required String id,
    required String name,
    String? description,
    String? targetAudience,
    String? startDate,
    String? endDate,
    String? goalDescription,
  }) async {
    try {
      final result = await _remote.updateExperiment(
        id: id,
        name: name,
        description: description,
        targetAudience: targetAudience,
        startDate: startDate,
        endDate: endDate,
        goalDescription: goalDescription,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({bool success, Failure? failure})> activateExperiment(
      String id) async {
    try {
      await _remote.activateExperiment(id);
      return (success: true, failure: null);
    } on DioException catch (e) {
      return (success: false, failure: _mapDioError(e));
    }
  }

  Future<({bool success, Failure? failure})> pauseExperiment(
      String id) async {
    try {
      await _remote.pauseExperiment(id);
      return (success: true, failure: null);
    } on DioException catch (e) {
      return (success: false, failure: _mapDioError(e));
    }
  }

  Future<({bool success, Failure? failure})> completeExperiment(
      String id) async {
    try {
      await _remote.completeExperiment(id);
      return (success: true, failure: null);
    } on DioException catch (e) {
      return (success: false, failure: _mapDioError(e));
    }
  }

  Future<({ExperimentStatsModel? data, Failure? failure})>
      getExperimentResults(String id) async {
    try {
      final result = await _remote.getExperimentResults(id);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({List<UserAssignmentModel>? data, Failure? failure})>
      getAssignments() async {
    try {
      final result = await _remote.getAssignments();
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({bool success, Failure? failure})> recordExposure({
    required String experimentKey,
    String? context,
  }) async {
    try {
      await _remote.recordExposure(
          experimentKey: experimentKey, context: context);
      return (success: true, failure: null);
    } on DioException catch (e) {
      return (success: false, failure: _mapDioError(e));
    }
  }

  Future<({bool success, Failure? failure})> recordConversion({
    required String experimentKey,
    required String goalKey,
    double? value,
  }) async {
    try {
      await _remote.recordConversion(
        experimentKey: experimentKey,
        goalKey: goalKey,
        value: value,
      );
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
    String message = 'An unexpected error occurred.';
    if (data is Map<String, dynamic>) {
      message = (data['errorMessage'] as String?) ?? message;
    }
    if (statusCode == 401) return AuthFailure(message: message);
    return ServerFailure(message: message, statusCode: statusCode);
  }
}
