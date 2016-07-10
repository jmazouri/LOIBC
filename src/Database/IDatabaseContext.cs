using System.Collections.Generic;
using System.Threading.Tasks;

namespace LOIBC
{
    public interface IDatabaseContext
    {
        bool IsInitialized { get; }

        Task<IEnumerable<MessageLog>> GetLogMessages();
        Task<IEnumerable<MonitoredChannel>> GetMonitoredChannels();
        Task Initialize();
        Task InsertLogMessage(MessageLog item);
        Task UpdateChannelMonitoring(MonitoredChannel channel);
    }
}