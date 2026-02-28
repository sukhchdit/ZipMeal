import 'package:dio/dio.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../../../core/constants/api_constants.dart';
import '../../../../core/errors/failures.dart';
import '../../../../core/network/api_client.dart';
import '../../../customer_discovery/data/models/public_restaurant_detail_model.dart';
import 'dine_in_menu_state.dart';

part 'dine_in_menu_notifier.g.dart';

@riverpod
class DineInMenuNotifier extends _$DineInMenuNotifier {
  @override
  DineInMenuState build(String sessionId) {
    loadMenu();
    return const DineInMenuState.initial();
  }

  Future<void> loadMenu() async {
    state = const DineInMenuState.loading();
    try {
      final dio = ref.read(apiClientProvider);
      final response = await dio.get<Map<String, dynamic>>(
        '${ApiConstants.dineInSessions}/$sessionId/menu',
      );
      final restaurant =
          PublicRestaurantDetailModel.fromJson(response.data!);
      state = DineInMenuState.loaded(restaurant: restaurant);
    } on DioException catch (e) {
      final message = e.response?.data is Map<String, dynamic>
          ? (e.response!.data as Map<String, dynamic>)['errorMessage']
                  as String? ??
              'Failed to load menu.'
          : 'Failed to load menu.';
      state = DineInMenuState.error(
        failure: ServerFailure(message: message),
      );
    }
  }
}
