using MediatR;

namespace CleanArchitecture.Application.Features.Binance.Commands.CreatePosition
{
    public class CreatePositionCommand : IRequest<bool>
    {
        public string Symbol { get; set; } = string.Empty;

        public float? Amount { get; set; }
        public float? EntryPrice { get; set; }
        public float? Leverage { get; set; }
        public float? MarkPrice { get; set; }
        public float? Pnl { get; set; }
        public bool TradeBefore { get; set; }
        public bool Yellow { get; set; }
        public bool EncryptedUid { get; set; }
        /*
         
         ,[UpdateTime]
      ,[CreatedOn]
      ,[UpdateTimeStamp]*/


    }    
}
