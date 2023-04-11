using System.Collections.Generic;
using CryptoExchange39.Net.Converters;
using Huobi.Net.Objects;

namespace Huobi.Net.Converters
{
    internal class TransactionTypeConverter : BaseConverter<HuobiTransactionType>
    {
        public TransactionTypeConverter() : this(true) { }
        public TransactionTypeConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<HuobiTransactionType, string>> Mapping => new List<KeyValuePair<HuobiTransactionType, string>>
        {
            new KeyValuePair<HuobiTransactionType, string>(HuobiTransactionType.Trade, "trade"),
            new KeyValuePair<HuobiTransactionType, string>(HuobiTransactionType.Etf, "etf"),
            new KeyValuePair<HuobiTransactionType, string>(HuobiTransactionType.TransactionFee, "transact-fee"),
            new KeyValuePair<HuobiTransactionType, string>(HuobiTransactionType.Deduction, "fee-deduction"),
            new KeyValuePair<HuobiTransactionType, string>(HuobiTransactionType.Transfer, "transfer"),
            new KeyValuePair<HuobiTransactionType, string>(HuobiTransactionType.Credit, "credit"),
            new KeyValuePair<HuobiTransactionType, string>(HuobiTransactionType.Liquidation, "liquidation"),
            new KeyValuePair<HuobiTransactionType, string>(HuobiTransactionType.Interest, "interest"),
            new KeyValuePair<HuobiTransactionType, string>(HuobiTransactionType.Deposit, "deposit"),
            new KeyValuePair<HuobiTransactionType, string>(HuobiTransactionType.Withdraw, "withdraw"),
            new KeyValuePair<HuobiTransactionType, string>(HuobiTransactionType.WithdrawFee, "withdraw-fee"),
            new KeyValuePair<HuobiTransactionType, string>(HuobiTransactionType.Exchange, "exchange"),
            new KeyValuePair<HuobiTransactionType, string>(HuobiTransactionType.Other, "other-types"),
            new KeyValuePair<HuobiTransactionType, string>(HuobiTransactionType.Rebate, "rebate")
        };
    }
}