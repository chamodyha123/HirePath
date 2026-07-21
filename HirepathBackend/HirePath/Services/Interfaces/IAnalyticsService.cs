using HirePathAI.API.DTOs.PlatformAdmin.Analytics;

namespace HirePathAI.API.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<AnalyticsResponseDto> GetAnalyticsDataAsync();
    }
}
