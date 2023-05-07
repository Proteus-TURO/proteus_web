using Microsoft.AspNetCore.Mvc;
using ProteusWeb.Helper;
using RosSharp.RosBridgeClient;
using RosSharp.RosBridgeClient.Protocols;
using RosSharp.RosBridgeClient.MessageTypes.Geometry;
using RosSharp.RosBridgeClient.MessageTypes.Std;
using Timer = System.Timers.Timer;

namespace ProteusWeb.Controller;

[ApiController]
[Route("/api")]
public class CarController : ControllerBase
{
    private static bool led = false;
    private static int velocity = 0;
    
    public static void StartSubscriptions()
    {
        while (true)
        {
            var endpoint = HeloTuroReceiver.GetEndpoint();
            if (endpoint == null)
            {
                Thread.Sleep(1000);
                continue;
            }

            var uri = "ws://" + endpoint.Address + ":" + endpoint.Port;
            var rosSocket = new RosSocket(new WebSocketNetProtocol(uri));

            rosSocket.Subscribe<UInt8>("/serial/led", ReceiveLed);
            rosSocket.Subscribe<Twist>("/serial/drive", ReceiveDrive);
            break;
        }
    }
    
    private static void ReceiveLed(UInt8 message)
    {
        led = message.data > 0;
    }
    
    private static void ReceiveDrive(Twist message)
    {
        var sum = 0.0;
        sum += message.linear.x;
        sum += message.linear.y;
        sum += message.angular.z;

        velocity = (int)Math.Min(100, sum * 100);
    }

    [HttpGet("status")]
    public ActionResult Status()
    {
        var endpoint = HeloTuroReceiver.GetEndpoint();
        if (endpoint == null)
        {
            return Problem("Car is not reachable");   
        }

        var response = new Dictionary<string, object>
        {
            {"autonomousDrive", false},
            {"lights", led},
            {"velocity", velocity},
            {"colors", new Dictionary<string, object>
            {
                {"busStop", new Dictionary<string, object>
                {
                    {"R", 0},
                    {"G", 0},
                    {"B", 255}
                }},
                {"lane", new Dictionary<string, object>
                {
                    {"R", 0},
                    {"G", 0},
                    {"B", 0}
                }},
                {"stop", new Dictionary<string, object>
                {
                    {"R", 255},
                    {"G", 0},
                    {"B", 0}
                }}
            }}
        };
        
        return Ok(response);
    }

    [HttpPost("lights/{status}")]
    public ActionResult Lights(string status)
    {
        var endpoint = HeloTuroReceiver.GetEndpoint();
        if (endpoint == null)
        {
            return Problem("Car is not reachable");   
        }

        var uri = "ws://" + endpoint.Address + ":" + endpoint.Port;
        var rosSocket = new RosSocket(new WebSocketNetProtocol(uri));

        rosSocket.Connect();
        rosSocket.Advertise<UInt8>("/serial/led");
        rosSocket.Publish("/serial/led", status == "on" ? new UInt8(255) : new UInt8(0));
        rosSocket.Close();

        return Ok("OK");
    }
    
    [HttpPost("drive/{mode}")]
    public ActionResult Drive(string mode)
    {
        var endpoint = HeloTuroReceiver.GetEndpoint();
        if (endpoint == null)
        {
            return Problem("Car is not reachable");   
        }

        var uri = "ws://" + endpoint.Address + ":" + endpoint.Port;
        var rosSocket = new RosSocket(new WebSocketNetProtocol(uri));

        rosSocket.Connect();
        rosSocket.Advertise<Twist>("/serial/drive");
        if (mode == "forward")
        {
            rosSocket.Publish("/serial/drive", new Twist(new Vector3(1.0, 0.0, 0.0), new Vector3(0.0, 0.0, 0.0)));
        } else if (mode == "backward")
        {
            rosSocket.Publish("/serial/drive", new Twist(new Vector3(-1.0, 0.0, 0.0), new Vector3(0.0, 0.0, 0.0)));
        } else if (mode == "left")
        {
            rosSocket.Publish("/serial/drive", new Twist(new Vector3(0.0, 0.0, 0.0), new Vector3(0.0, 0.0, 1.0)));
        } else if (mode == "right")
        {
            rosSocket.Publish("/serial/drive", new Twist(new Vector3(1.0, 0.0, 0.0), new Vector3(0.0, 0.0, -1.0)));
        } else if (mode == "stop")
        {
            rosSocket.Publish("/serial/drive", new Twist(new Vector3(0.0, 0.0, 0.0), new Vector3(0.0, 0.0, 0.0)));
        }
        else
        {
            rosSocket.Close();
            return BadRequest("mode must be forward, backward, left, right or stop");
        }
        rosSocket.Close();

        return Ok("OK");
    }
    [HttpPost("autonomousDrive/{status}")]
    public ActionResult AutonomousDrive(string status)
    {
        if (status != "on" && status != "off")
        {
            return BadRequest("status must be on or off");
        }
        var endpoint = HeloTuroReceiver.GetEndpoint();
        if (endpoint == null)
        {
            return Problem("Car is not reachable");   
        }

        return Problem("Autonomous driving is deactivated, otherwise the system would crash");
    }
}