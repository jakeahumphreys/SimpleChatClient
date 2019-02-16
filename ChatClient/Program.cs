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
            sc.Connect("127.0.0.1",4444);
            sc.Run();
        }
    }
}
