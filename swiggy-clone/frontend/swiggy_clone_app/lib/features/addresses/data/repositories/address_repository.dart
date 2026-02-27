import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/address_remote_data_source.dart';
import '../models/address_model.dart';

part 'address_repository.g.dart';

@riverpod
AddressRepository addressRepository(Ref ref) {
  final remoteDataSource = ref.watch(addressRemoteDataSourceProvider);
  return AddressRepository(remoteDataSource: remoteDataSource);
}

class AddressRepository {
  AddressRepository({required AddressRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final AddressRemoteDataSource _remote;

  Future<({List<AddressModel>? data, Failure? failure})> getAddresses() async {
    try {
      final result = await _remote.getAddresses();
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({AddressModel? data, Failure? failure})> getAddress(
      String id) async {
    try {
      final result = await _remote.getAddress(id);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({AddressModel? data, Failure? failure})> createAddress({
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
    try {
      final result = await _remote.createAddress(
        label: label,
        addressLine1: addressLine1,
        addressLine2: addressLine2,
        city: city,
        state: state,
        postalCode: postalCode,
        country: country,
        latitude: latitude,
        longitude: longitude,
        isDefault: isDefault,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({AddressModel? data, Failure? failure})> updateAddress({
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
    try {
      final result = await _remote.updateAddress(
        id: id,
        label: label,
        addressLine1: addressLine1,
        addressLine2: addressLine2,
        city: city,
        state: state,
        postalCode: postalCode,
        country: country,
        latitude: latitude,
        longitude: longitude,
      );
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({bool success, Failure? failure})> deleteAddress(String id) async {
    try {
      await _remote.deleteAddress(id);
      return (success: true, failure: null);
    } on DioException catch (e) {
      return (success: false, failure: _mapDioError(e));
    }
  }

  Future<({bool success, Failure? failure})> setDefault(String id) async {
    try {
      await _remote.setDefault(id);
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
