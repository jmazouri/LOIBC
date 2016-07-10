using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LOIBC.src.Database;
using Serilog;
using SQLite;

namespace LOIBC.Database
{
    public class DatabaseContext
    {
        public bool IsInitialized { get; private set; }

        private SQLiteAsyncConnection _connection;

        public DatabaseContext(FileInfo databasePath)
        {
            _connection = new SQLiteAsyncConnection(databasePath.ToString());
        }

        public async Task Initialize()
        {
            Log.Information("Initializing Database");

            await CreateTables(typeof(MessageLog), typeof(MonitoredChannel));

            IsInitialized = true;
        }

        private async Task CreateTables(params Type[] types)
        {
            foreach (Type type in types)
            {
                if (!(await _connection.TableExists(type.Name)))
                {
                    await _connection.CreateTablesAsync(CreateFlags.AllImplicit, type).ContinueWith((results) =>
                    {
                        Log.Information("Created {tablename} Table", type.Name);
                    });
                }
                else
                {
                    Log.Information("{tablename} table already exists, skipping.", type.Name);
                }
            }
        }

        public async Task InsertLogMessage(MessageLog item)
        {
            await _connection.InsertAsync(item);
        }

        public async Task UpdateChannelMonitoring(MonitoredChannel channel)
        {
            var foundItem = await _connection.Table<MonitoredChannel>()
                .Where(d=>d.ChannelId == channel.ChannelId).FirstOrDefaultAsync();

            if (foundItem != null)
            {
                foundItem.IsMonitored = channel.IsMonitored;
                await _connection.UpdateAsync(foundItem);
            }
            else
            {
                await _connection.InsertAsync(channel);
            }
        }

        public async Task<IEnumerable<MessageLog>> GetLogMessages()
        {
            return await _connection.Table<MessageLog>().ToListAsync();
        }

        public async Task<IEnumerable<MonitoredChannel>> GetMonitoredChannels()
        {
            return await _connection.Table<MonitoredChannel>().ToListAsync();
        }
    }
}
