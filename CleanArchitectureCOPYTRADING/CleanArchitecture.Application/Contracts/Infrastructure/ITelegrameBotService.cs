using CleanArchitecture.Application.Models;

namespace CleanArchitecture.Application.Contracts.Infrastructure
{
    public interface ITelegrameBotService
    {
        Task<bool> InitBotClient();
        Task<bool> SendDataPositions(PositionData positionData);
        Task<List<PositionData>> LoadPosition(string pathDataJson);
        Task<bool> SendDataPositions(PositionData positionData, long Id);
        Task<bool> SendOtherPositionRetList(OtherPositionRetList otherPositionRetList, long Id
            , Dictionary<string, string> LongShort, string nickName);
    }
    
}
