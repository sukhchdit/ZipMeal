import 'package:riverpod_annotation/riverpod_annotation.dart';

import '../../data/repositories/restaurant_repository.dart';
import 'table_management_state.dart';

part 'table_management_notifier.g.dart';

@riverpod
class TableManagementNotifier extends _$TableManagementNotifier {
  late RestaurantRepository _repository;

  @override
  TableManagementState build(String restaurantId) {
    _repository = ref.watch(restaurantRepositoryProvider);
    loadTables();
    return const TableManagementState.initial();
  }

  Future<void> loadTables() async {
    state = const TableManagementState.loading();
    final result = await _repository.getTables(restaurantId: restaurantId);
    if (result.failure != null) {
      state = TableManagementState.error(failure: result.failure!);
    } else {
      state = TableManagementState.loaded(tables: result.data!);
    }
  }

  Future<bool> createTable(Map<String, dynamic> data) async {
    final result = await _repository.createTable(
      restaurantId: restaurantId,
      data: data,
    );
    if (result.failure != null) return false;
    await loadTables();
    return true;
  }

  Future<bool> updateTable(String tableId, Map<String, dynamic> data) async {
    final result = await _repository.updateTable(
      restaurantId: restaurantId,
      tableId: tableId,
      data: data,
    );
    if (result.failure != null) return false;
    await loadTables();
    return true;
  }

  Future<bool> deleteTable(String tableId) async {
    final failure = await _repository.deleteTable(
      restaurantId: restaurantId,
      tableId: tableId,
    );
    if (failure != null) return false;
    await loadTables();
    return true;
  }
}
