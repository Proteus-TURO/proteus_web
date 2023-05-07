using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ProteusWeb.Helper;

public class HeloTuroReceiver
{
    private static readonly Thread Thread = new (BackgroundTask);
    private static IPEndPoint? _endPoint = null;
    private static DateTime? _lastReceived = null;
    public static void StartReceiving()
    {
        Thread.IsBackground = true;
        Thread.Start();
    }

    private static void BackgroundTask()
    {
        var udpClient = new UdpClient(6274);
        var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        try
        {
            while (true)
            {
                var receivedBytes = udpClient.Receive(ref remoteEndPoint);
                var receivedMessage = Encoding.UTF8.GetString(receivedBytes);

                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(receivedMessage, jsonOptions);
                    _lastReceived = DateTime.Now;
                    var port = dict?["bridge"] as JsonElement?;
                    _endPoint = new IPEndPoint(remoteEndPoint.Address, port?.GetInt32() ?? -1);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error parsing JSON: " + ex.Message);
                }
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Error receiving UDP message: " + ex.Message);
        }
        finally
        {
            udpClient.Close();
        }
    }

    public static IPEndPoint? GetEndpoint()
    {
        var timeSpan = DateTime.Now - _lastReceived;
        if (_endPoint == null || timeSpan == null || timeSpan?.TotalSeconds > 5 || _endPoint.Port == -1)
        {
            return null;
        }

        return _endPoint;
    }
}