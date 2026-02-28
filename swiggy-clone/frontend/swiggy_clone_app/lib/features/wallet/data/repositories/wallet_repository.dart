import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/errors/failures.dart';
import '../datasources/wallet_remote_data_source.dart';
import '../models/wallet_model.dart';
import '../models/wallet_transaction_model.dart';

part 'wallet_repository.g.dart';

@riverpod
WalletRepository walletRepository(Ref ref) {
  final remoteDataSource = ref.watch(walletRemoteDataSourceProvider);
  return WalletRepository(remoteDataSource: remoteDataSource);
}

class WalletRepository {
  WalletRepository({required WalletRemoteDataSource remoteDataSource})
      : _remote = remoteDataSource;

  final WalletRemoteDataSource _remote;

  Future<({WalletModel? data, Failure? failure})> getBalance() async {
    try {
      final result = await _remote.getBalance();
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<({WalletTransactionModel? data, Failure? failure})> addMoney(
    int amountPaise,
  ) async {
    try {
      final result = await _remote.addMoney(amountPaise: amountPaise);
      return (data: result, failure: null);
    } on DioException catch (e) {
      return (data: null, failure: _mapDioError(e));
    }
  }

  Future<
      ({
        List<WalletTransactionModel>? items,
        int? totalCount,
        int? page,
        int? totalPages,
        Failure? failure,
      })> getTransactions({int page = 1, int pageSize = 20}) async {
    try {
      final data = await _remote.getTransactions(
        page: page,
        pageSize: pageSize,
      );
      final items = (data['items'] as List<dynamic>)
          .map(
            (e) =>
                WalletTransactionModel.fromJson(e as Map<String, dynamic>),
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
