using SharpPcap;
using System;
using PacketDotNet;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using PacketDotNet.Ieee80211;
using System.Net;

namespace SecuredWeb
{
    internal class dnssniffer
    {
        public static void Start()
        {
            Console.WriteLine("Getting devices...");
            var devices = CaptureDeviceList.Instance;

            if (devices.Count < 1 )
            {
                Console.WriteLine("No network devices found! [make sure your admin]");
                return;
            }
            for (int i = 0; i < devices.Count; i++)
            {
                Console.WriteLine($"{i}: {devices[i].Description}");
            }

            Console.Write("Select a device (enter number): ");
            if (!int.TryParse(Console.ReadLine(), out int deviceIndex) || deviceIndex < 0 || deviceIndex >= devices.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }
            var device = devices[deviceIndex];
            Console.WriteLine($"The network used: {device.Description}");
            device.OnPacketArrival += new PacketArrivalEventHandler(Device_OnPacketArrival);
            device.Open(DeviceModes.Promiscuous, 1000);
            device.StartCapture();
        }

        private static void Device_OnPacketArrival(object sender, PacketCapture e)
        {
            var rawPack = e.GetPacket();
            var packet = PacketDotNet.Packet.ParsePacket(rawPack.LinkLayerType, rawPack.Data);
            var ipPacket = packet.Extract<PacketDotNet.IPPacket>();
            var udpPacket = packet.Extract<PacketDotNet.UdpPacket>();
            if (udpPacket != null)
            {
                if (udpPacket.SourcePort == 53 || udpPacket.DestinationPort == 53)
                {
                    Console.WriteLine("DNS packet detected");

                    int offset = 12;
                    var s = udpPacket.PayloadData;

                    string dns = "";
                    while (s[offset] != 0)
                    {
                        byte len = s[offset];
                        offset++;
                        dns += Encoding.ASCII.GetString(s, offset, len) + ".";
                        offset += len;  

                    }
                    dns = dns.TrimEnd('.');
                    if (BlockListManager.IsBlocked(dns))
                    {
                        Console.WriteLine($"Blocked: {dns}");
                        UdpClient b = new UdpClient();
                        byte[] bytes = { 0x00, 0x01, 0x02, 0x03 };
                        var remoteEP = new IPEndPoint(ipPacket.SourceAddress, udpPacket.SourcePort);
                        b.Send(bytes, bytes.Length, remoteEP);
                        b.Close();


                    }
                    else
                    {
                        Console.WriteLine($"Allowed: {dns}");
                    }

                }


            }
        }

    }
}
