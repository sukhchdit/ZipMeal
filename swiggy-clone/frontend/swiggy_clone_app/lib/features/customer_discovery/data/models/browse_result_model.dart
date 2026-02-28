import 'package:freezed_annotation/freezed_annotation.dart';

import 'customer_restaurant_model.dart';

part 'browse_result_model.freezed.dart';
part 'browse_result_model.g.dart';

@freezed
class BrowseResultModel with _$BrowseResultModel {
  const factory BrowseResultModel({
    @Default([]) List<CustomerRestaurantModel> items,
    String? nextCursor,
    String? previousCursor,
    required bool hasMore,
    required int pageSize,
  }) = _BrowseResultModel;

  factory BrowseResultModel.fromJson(Map<String, dynamic> json) =>
      _$BrowseResultModelFromJson(json);
}
