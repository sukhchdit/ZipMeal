import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/dietary_profile_model.dart';

part 'dietary_remote_data_source.g.dart';

@riverpod
DietaryRemoteDataSource dietaryRemoteDataSource(DietaryRemoteDataSourceRef ref) {
  return DietaryRemoteDataSource(ref.watch(apiClientProvider));
}

class DietaryRemoteDataSource {
  final Dio _dio;

  DietaryRemoteDataSource(this._dio);

  Future<DietaryProfileModel> getDietaryProfile() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.dietaryProfile,
    );
    return DietaryProfileModel.fromJson(response.data!);
  }

  Future<DietaryProfileModel> saveDietaryProfile({
    required List<int>? allergenAlerts,
    required List<int>? dietaryPreferences,
    required int? maxSpiceLevel,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.dietaryProfile,
      data: {
        'allergenAlerts': allergenAlerts,
        'dietaryPreferences': dietaryPreferences,
        'maxSpiceLevel': maxSpiceLevel,
      },
    );
    return DietaryProfileModel.fromJson(response.data!);
  }
}
