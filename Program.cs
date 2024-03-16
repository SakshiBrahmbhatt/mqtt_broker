using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using MQTTnet;
using MQTTnet.Server;

class Program
{
    private static int MessageCounter = 0;

    static void Main(string[] args)
    {
        var options = new MqttServerOptionsBuilder()
            .WithDefaultEndpoint()
            .WithDefaultEndpointPort(1883)
            .WithConnectionValidator(OnNewConnection)
            .WithApplicationMessageInterceptor(OnNewMessage)
            .WithSubscriptionInterceptor(context =>
            {
                Console.WriteLine("Client " + context.ClientId + " subscribed to topic " + context.TopicFilter);
            });

        var mqttServer = new MqttFactory().CreateMqttServer();

        var port = options.Build().DefaultEndpointOptions.Port;
        var localIp = GetLocalIPAddress();
        Console.WriteLine("MQTT server is running on "+ localIp+":"+ port);

        mqttServer.StartAsync(options.Build()).GetAwaiter().GetResult();

        Console.ReadLine();
    }

    public static void OnNewConnection(MqttConnectionValidatorContext context)
    {
        Console.WriteLine("New connection: ClientId = "+ context.ClientId+", Endpoint = "+ context.Endpoint + ", CleanSession = "+context.CleanSession);
    }

    public static void OnNewMessage(MqttApplicationMessageInterceptorContext context)
    {
        var payload = context.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(context.ApplicationMessage?.Payload);

        MessageCounter++;

        Console.WriteLine("MessageId: " + MessageCounter + " -- Message: ClientId = "+ context.ClientId + ", Topic = "+ context.ApplicationMessage?.Topic+", Message = "+payload);
    }

    // Method to get the local IP address of the machine
    public static string GetLocalIPAddress()
    {
        string localIP = "";
        foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 &&
                netInterface.OperationalStatus == OperationalStatus.Up)
            {
                foreach (var addrInfo in netInterface.GetIPProperties().UnicastAddresses)
                {
                    if (addrInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = addrInfo.Address.ToString();
                    }
                }
            }
        }
        return localIP;
    }
}
