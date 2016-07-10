using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLite;

namespace LOIBC.src.Database
{
    public static class SQLiteAsyncExtensions
    {
        public static async Task<bool> TableExists(this SQLiteAsyncConnection connection, string tableName)
        {
            return await connection.ExecuteScalarAsync<bool>
                ("SELECT EXISTS(SELECT name FROM sqlite_master WHERE type = 'table' AND name = ?)", tableName);
        }
    }
}
