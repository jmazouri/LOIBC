﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace LOIBC
{
    public class LOIBCConfig
    {
        public string BotKey { get; set; }
        public string ClientId { get; set; }
        public string LogPath { get; set; }
        public string DatabasePath { get; set; }

        public LOIBCConfig(IConfigurationRoot configRoot)
        {
            BotKey = configRoot["Discord:BotKey"];
            ClientId = configRoot["Discord:ClientId"];
            LogPath = configRoot["LOIBC:LogPath"];
            DatabasePath = configRoot["LOIBC:DatabasePath"];
        }
    }
}
