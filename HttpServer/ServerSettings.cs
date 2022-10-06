﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    public class ServerSettings
    {
        public int Port { get; set; } = 7700;
        public string Path { get; set; } = @"./site";
        public ServerSettings(int port, string path)
        {
            Port = port;
            Path = path;
        }
    }
}
