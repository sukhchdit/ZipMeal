import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/datasources/order_remote_data_source.dart';
import '../../data/models/fee_config_model.dart';

part 'fee_config_provider.g.dart';

@riverpod
Future<FeeConfigModel> feeConfig(Ref ref) async {
  final dataSource = ref.watch(orderRemoteDataSourceProvider);
  return dataSource.getFeeConfig();
}
