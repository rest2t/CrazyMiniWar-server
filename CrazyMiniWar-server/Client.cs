using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CrazyMiniWar_server
{
    class Client
    {
        public TcpClient Tcp;
        public StreamWriter Writer;
        public StreamReader Reader;
        public String Name;
    }
}
