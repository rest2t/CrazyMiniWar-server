using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

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

    public TcpServer(int port)
    {
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
            t.Start(newClient);
        }
    }

    public void HandleClient(object obj)
    {
        // retrieve client from parameter passed to thread
        TcpClient client = (TcpClient)obj;
        Console.WriteLine("New client: " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());
        // sets two streams
        StreamWriter sWriter = new StreamWriter(client.GetStream(), Encoding.ASCII);
        StreamReader sReader = new StreamReader(client.GetStream(), Encoding.ASCII);
        // you could use the NetworkStream to read and write, 
        // but there is no forcing flush, even when requested

        Boolean bClientConnected = true;
        String sData = null;

        while (bClientConnected)
        {
            // reads from stream
            sData = sReader.ReadLine();

            // shows content on the console.
            Console.WriteLine("Client &gt; " + sData);

            // to write something back.
            // sWriter.WriteLine("Meaningfull things here");
            // sWriter.Flush();
        }
    }
}
