import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/errors/failures.dart';
import '../../../../core/network/api_client.dart';
import 'order_detail_notifier.dart';

part 'tip_notifier.g.dart';

@riverpod
class TipNotifier extends _$TipNotifier {
  @override
  AsyncValue<void> build() => const AsyncData(null);

  Future<bool> submitTip({
    required String orderId,
    required int amountPaise,
  }) async {
    state = const AsyncLoading();
    try {
      final dio = ref.read(apiClientProvider);
      await dio.post<void>(
        '${ApiConstants.orders}/$orderId/tip',
        data: {'amountPaise': amountPaise},
      );
      state = const AsyncData(null);
      ref.invalidate(orderDetailNotifierProvider(orderId));
      return true;
    } on DioException catch (e) {
      final message = _extractErrorMessage(e);
      state = AsyncError(ServerFailure(message: message), StackTrace.current);
      return false;
    }
  }

  String _extractErrorMessage(DioException e) {
    final data = e.response?.data;
    if (data is Map<String, dynamic>) {
      return (data['errorMessage'] as String?) ?? 'Failed to submit tip.';
    }
    return 'Failed to submit tip.';
  }
}
