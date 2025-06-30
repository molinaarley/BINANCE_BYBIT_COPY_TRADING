
namespace CleanArchitecture.Application.Models
{
    // GetInstrumentsInfoRoot myDeserializedClass = JsonConvert.DeserializeObject<GetInstrumentsInfoRoot>(myJsonResponse);
    public class GetInstrumentsInfoLeverageFilter
    {
        public double? minLeverage { get; set; }
        public double? maxLeverage { get; set; }
        public double? leverageStep { get; set; }
    }

    public class GetInstrumentsInfoList
    {
        public string symbol { get; set; }
        public string contractType { get; set; }
        public string status { get; set; }
        public string baseCoin { get; set; }
        public string quoteCoin { get; set; }
        public string launchTime { get; set; }
        public string deliveryTime { get; set; }
        public string deliveryFeeRate { get; set; }
        public string priceScale { get; set; }
        public GetInstrumentsInfoLeverageFilter leverageFilter { get; set; }
        public PriceFilter priceFilter { get; set; }
        public LotSizeFilter lotSizeFilter { get; set; }
        public bool unifiedMarginTrade { get; set; }
        public int fundingInterval { get; set; }
        public string settleCoin { get; set; }
        public string copyTrading { get; set; }
    }

    public class LotSizeFilter
    {
        public double? maxOrderQty { get; set; }
        public double? minOrderQty { get; set; }
        public double? qtyStep { get; set; }
        public double? postOnlyMaxOrderQty { get; set; }
    }

    public class PriceFilter
    {
        public double? minPrice { get; set; }
        public double? maxPrice { get; set; }
        public double? tickSize { get; set; }
    }

    public class GetInstrumentsInfoResult
    {
        public string category { get; set; }
        public List<GetInstrumentsInfoList> list { get; set; }
        public string nextPageCursor { get; set; }
    }

    public class GetInstrumentsInfoRetExtInfo
    {
    }

    public class GetInstrumentsInfoRoot
    {
        public int retCode { get; set; }
        public string retMsg { get; set; }
        public GetInstrumentsInfoResult result { get; set; }
        public GetInstrumentsInfoRetExtInfo retExtInfo { get; set; }
        public long time { get; set; }
    }

}
