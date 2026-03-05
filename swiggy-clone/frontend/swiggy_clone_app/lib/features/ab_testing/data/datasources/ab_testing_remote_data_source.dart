import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/experiment_model.dart';
import '../models/experiment_stats_model.dart';
import '../models/user_assignment_model.dart';

part 'ab_testing_remote_data_source.g.dart';

@riverpod
AbTestingRemoteDataSource abTestingRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return AbTestingRemoteDataSource(dio: dio);
}

class AbTestingRemoteDataSource {
  AbTestingRemoteDataSource({required Dio dio}) : _dio = dio;
  final Dio _dio;

  /// Create a new experiment with variants.
  Future<ExperimentModel> createExperiment({
    required String key,
    required String name,
    String? description,
    String? targetAudience,
    String? startDate,
    String? endDate,
    String? goalDescription,
    required List<Map<String, dynamic>> variants,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.abTestingExperiments,
      data: {
        'key': key,
        'name': name,
        if (description != null) 'description': description,
        if (targetAudience != null) 'targetAudience': targetAudience,
        if (startDate != null) 'startDate': startDate,
        if (endDate != null) 'endDate': endDate,
        if (goalDescription != null) 'goalDescription': goalDescription,
        'variants': variants,
      },
    );
    return ExperimentModel.fromJson(response.data!);
  }

  /// List experiments with optional status filter and pagination.
  Future<({List<ExperimentModel> items, int totalCount, int page, int pageSize})>
      getExperiments({int? status, int page = 1, int pageSize = 20}) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.abTestingExperiments,
      queryParameters: {
        if (status != null) 'status': status,
        'page': page,
        'pageSize': pageSize,
      },
    );
    final data = response.data!;
    final items = (data['items'] as List<dynamic>)
        .map((e) => ExperimentModel.fromJson(e as Map<String, dynamic>))
        .toList();
    return (
      items: items,
      totalCount: data['totalCount'] as int,
      page: data['page'] as int,
      pageSize: data['pageSize'] as int,
    );
  }

  /// Get a single experiment by ID.
  Future<ExperimentModel> getExperimentById(String id) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.abTestingExperimentById(id),
    );
    return ExperimentModel.fromJson(response.data!);
  }

  /// Update a draft experiment.
  Future<ExperimentModel> updateExperiment({
    required String id,
    required String name,
    String? description,
    String? targetAudience,
    String? startDate,
    String? endDate,
    String? goalDescription,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.abTestingExperimentById(id),
      data: {
        'name': name,
        if (description != null) 'description': description,
        if (targetAudience != null) 'targetAudience': targetAudience,
        if (startDate != null) 'startDate': startDate,
        if (endDate != null) 'endDate': endDate,
        if (goalDescription != null) 'goalDescription': goalDescription,
      },
    );
    return ExperimentModel.fromJson(response.data!);
  }

  /// Activate a draft or paused experiment.
  Future<void> activateExperiment(String id) async {
    await _dio.post<dynamic>(ApiConstants.abTestingActivate(id));
  }

  /// Pause an active experiment.
  Future<void> pauseExperiment(String id) async {
    await _dio.post<dynamic>(ApiConstants.abTestingPause(id));
  }

  /// Complete an experiment.
  Future<void> completeExperiment(String id) async {
    await _dio.post<dynamic>(ApiConstants.abTestingComplete(id));
  }

  /// Get statistical results for an experiment.
  Future<ExperimentStatsModel> getExperimentResults(String id) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.abTestingResults(id),
    );
    return ExperimentStatsModel.fromJson(response.data!);
  }

  /// Get all active experiment assignments for the current user.
  Future<List<UserAssignmentModel>> getAssignments() async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.abTestingAssignments,
    );
    return response.data!
        .map((e) => UserAssignmentModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  /// Record an exposure event.
  Future<void> recordExposure({
    required String experimentKey,
    String? context,
  }) async {
    await _dio.post<dynamic>(
      ApiConstants.abTestingExposure,
      data: {
        'experimentKey': experimentKey,
        if (context != null) 'context': context,
      },
    );
  }

  /// Record a conversion event.
  Future<void> recordConversion({
    required String experimentKey,
    required String goalKey,
    double? value,
  }) async {
    await _dio.post<dynamic>(
      ApiConstants.abTestingConversion,
      data: {
        'experimentKey': experimentKey,
        'goalKey': goalKey,
        if (value != null) 'value': value,
      },
    );
  }
}
