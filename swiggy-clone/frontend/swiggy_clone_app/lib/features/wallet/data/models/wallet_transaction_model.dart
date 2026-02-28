import 'package:freezed_annotation/freezed_annotation.dart';

part 'wallet_transaction_model.freezed.dart';
part 'wallet_transaction_model.g.dart';

@freezed
class WalletTransactionModel with _$WalletTransactionModel {
  const factory WalletTransactionModel({
    required String id,
    required int amountPaise,
    required int type, // 0=Credit, 1=Debit
    required int source, // 0=AddMoney, 1=OrderPayment, 2=Refund, 3=Cashback, 4=Promotional
    required String description,
    required int balanceAfterPaise,
    required DateTime createdAt,
    String? referenceId,
  }) = _WalletTransactionModel;

  factory WalletTransactionModel.fromJson(Map<String, dynamic> json) =>
      _$WalletTransactionModelFromJson(json);
}
