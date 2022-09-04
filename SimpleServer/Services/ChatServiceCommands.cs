using System;
using System.Collections.Generic;

namespace SimpleServer.Services
{
    public static class ChatServiceCommands
    {
        public static void ListConnectedClients(List<Client> connectedClients)
        {
            foreach (var client in connectedClients)
            {
                Console.WriteLine($"{client.Id} ({client.Username ?? "No Username"})");
            }
        }
    }
}