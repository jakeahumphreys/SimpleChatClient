using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleClient
{
    static class Program
    {
        static void Main(string[] args)
        {
            SimpleClient sc = new SimpleClient();
            var connected = sc.Connect("127.0.0.1", 4444);

            if (connected)
                sc.Run();
            else
                MessageBox.Show("The Chatroom is currently closed");
        }
    }
}
