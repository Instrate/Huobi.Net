using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Huobi.Net.Objects
{
    internal class HuobiResponseUsdtSwap<T>
    {
        public string? Status { get; set; }
#pragma warning disable 8618
        public T Data { get; set; }
#pragma warning disable 8618
        [JsonProperty("ts")]
        public long TimeStamp { get; set; }
        [JsonProperty("err_code")]
        public int ErrorCode { get; set; }
        [JsonProperty("err_msg")]
        public string ErrorMessage { get; set; }
    }
}
