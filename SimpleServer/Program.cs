﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Packets;
namespace SimpleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ChatService server = new ChatService("127.0.0.1",4444);
            server.Start();
        }
    }
}
