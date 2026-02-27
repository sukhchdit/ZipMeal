import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/address_model.dart';

part 'address_remote_data_source.g.dart';

@riverpod
AddressRemoteDataSource addressRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return AddressRemoteDataSource(dio: dio);
}

class AddressRemoteDataSource {
  AddressRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  Future<List<AddressModel>> getAddresses() async {
    final response = await _dio.get<List<dynamic>>(
      ApiConstants.userAddresses,
    );
    return response.data!
        .map((e) => AddressModel.fromJson(e as Map<String, dynamic>))
        .toList();
  }

  Future<AddressModel> getAddress(String id) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.userAddressById(id),
    );
    return AddressModel.fromJson(response.data!);
  }

  Future<AddressModel> createAddress({
    required String label,
    required String addressLine1,
    String? addressLine2,
    required String city,
    required String state,
    required String postalCode,
    String? country,
    required double latitude,
    required double longitude,
    required bool isDefault,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.userAddresses,
      data: {
        'label': label,
        'addressLine1': addressLine1,
        if (addressLine2 != null) 'addressLine2': addressLine2,
        'city': city,
        'state': state,
        'postalCode': postalCode,
        if (country != null) 'country': country,
        'latitude': latitude,
        'longitude': longitude,
        'isDefault': isDefault,
      },
    );
    return AddressModel.fromJson(response.data!);
  }

  Future<AddressModel> updateAddress({
    required String id,
    required String label,
    required String addressLine1,
    String? addressLine2,
    required String city,
    required String state,
    required String postalCode,
    String? country,
    required double latitude,
    required double longitude,
  }) async {
    final response = await _dio.put<Map<String, dynamic>>(
      ApiConstants.userAddressById(id),
      data: {
        'label': label,
        'addressLine1': addressLine1,
        if (addressLine2 != null) 'addressLine2': addressLine2,
        'city': city,
        'state': state,
        'postalCode': postalCode,
        if (country != null) 'country': country,
        'latitude': latitude,
        'longitude': longitude,
      },
    );
    return AddressModel.fromJson(response.data!);
  }

  Future<void> deleteAddress(String id) async {
    await _dio.delete<void>(ApiConstants.userAddressById(id));
  }

  Future<void> setDefault(String id) async {
    await _dio.put<void>(ApiConstants.userAddressSetDefault(id));
  }
}
