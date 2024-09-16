using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;
using System.Linq;

public class StunRequest : MonoBehaviour
{
    private const string StunServerIp = "stun.l.google.com";
    private const int StunServerPort = 19302;

    private void Start()
    {
        sendRequest();
    }

    int sendcount = 0;
    async void sendRequest()
    {
        // Resolve STUN server IP address
        IPAddress stunServerAddress = await ResolveHostname(StunServerIp);
        if (stunServerAddress == null)
        {
            Debug.LogError("Failed to resolve STUN server address.");
            return;
        }

        // Send STUN request
        try
        {
            var data = await SendStunRequest(stunServerAddress, StunServerPort);

            // Log the public IP and port
            Debug.Log($"Public IP: {data.Item1}");
            Debug.Log($"Public Port: {data.Item2}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"STUN request failed: {ex.Message}");
        }

        sendcount++;
        if (sendcount > 20) return;

        await Task.Delay(16);
        sendRequest();
    }

    private async Task<IPAddress> ResolveHostname(string hostname)
    {
        try
        {
            var addresses = await Dns.GetHostAddressesAsync(hostname);
            return addresses.Length > 0 ? addresses[0] : null;
        }
        catch
        {
            return null;
        }
    }

    private async Task<(string, int)> SendStunRequest(IPAddress stunServerAddress, int stunServerPort)
    {
        string publicIp = null;
        int publicPort = -1;

        using (var udpClient = new UdpClient())
        {
            // STUN Request packet (simple binding request)
            var request = CreateStunBindingRequest();
            await udpClient.SendAsync(request, request.Length, stunServerAddress.ToString(), stunServerPort);

            // Receive response
            var response = await udpClient.ReceiveAsync();
            var responseData = response.Buffer;

            // Parse STUN response
            ParseStunResponse(responseData, out publicIp, out publicPort);
        }

        return (publicIp, publicPort);
    }

    private byte[] CreateStunBindingRequest()
    {
        // STUN Binding Request (0x0001 is Binding Request message type)
        // This is a simplified example and might need to be adjusted for your use case
        byte[] request = new byte[20];
        request[0] = 0x00; // Message Type (Binding Request)
        request[1] = 0x01;
        return request;
    }

    private void ParseStunResponse(byte[] responseData, out string publicIp, out int publicPort)
    {
        publicIp = null;
        publicPort = -1;

        if (responseData.Length < 20) { print("too short"); return; }

        //skip header
        int index = 20; // Skip the header
        while (index < responseData.Length)
        {
            ushort attrType = (ushort)((responseData[index] << 8) | responseData[index + 1]);
            ushort attrLength = (ushort)((responseData[index + 2] << 8) | responseData[index + 3]);

            if (attrType == 0x0001) // Mapped Address
            {
                byte family = responseData[index + 4];

                ushort port = (ushort)((responseData[index + 6] << 8) | responseData[index + 7]);
                string ipAddress = string.Join(".", responseData.Skip(index + 8).Take(4));

                publicIp = ipAddress;
                publicPort = port;
                break;
            }
            index += 4 + attrLength; // Move to the next attribute
        }
    }
}