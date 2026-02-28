import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/network/api_client.dart';
import '../models/wallet_model.dart';
import '../models/wallet_transaction_model.dart';

part 'wallet_remote_data_source.g.dart';

@riverpod
WalletRemoteDataSource walletRemoteDataSource(Ref ref) {
  final dio = ref.watch(apiClientProvider);
  return WalletRemoteDataSource(dio: dio);
}

class WalletRemoteDataSource {
  WalletRemoteDataSource({required Dio dio}) : _dio = dio;

  final Dio _dio;

  /// GET /api/v1/wallet — Get wallet balance
  Future<WalletModel> getBalance() async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.wallet,
    );
    return WalletModel.fromJson(response.data!);
  }

  /// POST /api/v1/wallet/add-money — Add money to wallet
  Future<WalletTransactionModel> addMoney({
    required int amountPaise,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiConstants.walletAddMoney,
      data: {'amountPaise': amountPaise},
    );
    return WalletTransactionModel.fromJson(response.data!);
  }

  /// GET /api/v1/wallet/transactions — Get transaction history
  Future<Map<String, dynamic>> getTransactions({
    int page = 1,
    int pageSize = 20,
  }) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiConstants.walletTransactions,
      queryParameters: {
        'page': page,
        'pageSize': pageSize,
      },
    );
    return response.data!;
  }
}
