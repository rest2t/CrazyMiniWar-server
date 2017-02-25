using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using CrazyMiniWar_server;

public class serv
{
    static void Main(string[] args)
    {
        Console.WriteLine("Server started");
        TcpServer server = new TcpServer(8001);
    }
}

class TcpServer
{
    private TcpListener _server;
    private Boolean _isRunning;
    private List<Client> _clients;

    public Map Map = Map.Load("maps/default.txt");
    public TcpServer(int port)
    {
        Console.WriteLine(Map.Serialize());
        _clients = new List<Client>();
        _server = new TcpListener(IPAddress.Any, port);
        _server.Start();

        _isRunning = true;

        LoopClients();
    }

    public void LoopClients()
    {
        while (_isRunning)
        {
            TcpClient newClient = _server.AcceptTcpClient();
            Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
            Client client = new Client();
            client.Tcp = newClient;

            t.Start(client);
        }
    }

    public void HandleClient(object obj)
    {
        Client client = (Client)obj;

        client.Writer = new StreamWriter(client.Tcp.GetStream(), Encoding.ASCII);
        client.Reader = new StreamReader(client.Tcp.GetStream(), Encoding.ASCII);
        _clients.Add(client);
        Boolean bClientConnected = true;
        String sData = null;

        while (bClientConnected)
        {
            try
            {
                sData = client.Reader.ReadLine();
                if (sData != null)
                {
                    String[] args = sData.Split('|');
                    if (sData.IndexOf("setName") == 0)
                    {
                        client.Name = args[1];
                        Console.WriteLine("new name: " + client.Name);
                        BroadCast(client, "newClient|" + client.Name);
                        foreach (var client1 in _clients)
                        {
                            if (client1 != client)
                            {
                                client.Writer.WriteLine("newClient|" + client1.Name);
                            }
                        }
                        client.Writer.WriteLine("map|" + Map.Serialize());
                        client.Writer.Flush();
                    }
                    if (sData.IndexOf("pos") == 0)
                    {
                       BroadCast(client, "pos|" + args[1] + "|" + args[2] + "|" + args[3] + "|" + client.Name);
                    }
                    if (args[0].Equals("shoot"))
                    {
                        BroadCast(client, "shoot|" + args[1] + "|" + args[2] + "|" + client.Name);
                    }
                    if (args[0].Equals("respawn"))
                    {
                        BroadCast(client, "respawn|" + client.Name);
                    }
                    if (args[0].Equals("chat"))
                    {
                        BroadCast(client, "chat|" + args[1] + "|" + client.Name);
                    }
                    if (args[0].Equals("switchMap"))
                    {
                        Map = Map.Load("maps/" + args[1] + ".txt");
                        BroadCast(null, "map|" + Map.Serialize());
                        Console.WriteLine("changing map");
                    }
                }
            }
            catch(IOException exception)
            {
                bClientConnected = false;
            }
            
        }
        Console.WriteLine("client disconnected: " + client.Name);
        _clients.Remove(client);
        foreach (var client1 in _clients)
        {
            client1.Writer.WriteLine("disconnect|" + client.Name);
            client1.Writer.Flush();
        }
    }

    private void BroadCast(Client curr, String msg)
    {
         foreach (var client1 in _clients)
         {
            if (client1 != curr)
            {
                client1.Writer.WriteLine(msg);
                client1.Writer.Flush();
            }
         }
    }
}
