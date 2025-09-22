using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecuredWeb
{
    internal class DnsRelayServer
    {
        public static void Start()
        {
            UdpClient client = new UdpClient(53);
            Console.WriteLine("started,awaiting for port 53 incomings ");
            while (true)
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] received = client.Receive(ref remoteEP);

                Console.WriteLine($"Received DNS query from {remoteEP.Address}");
                int offset = 12;
                string dns = "";
                while (received[offset] != 0)
                {
                    byte len = received[offset];
                    offset++;
                    dns += Encoding.ASCII.GetString(received, offset, len) + ".";
                    offset += len;
                }
                dns = dns.TrimEnd('.');
                Console.WriteLine(dns);
                if (BlockListManager.IsBlocked(dns))
                {
                    Console.WriteLine("Blocked");

                    int questionEnd = 12;
                    while (received[questionEnd] != 0)
                    {
                        questionEnd += received[questionEnd] + 1;
                    }
                    questionEnd += 5;
                    byte[] response = new byte[questionEnd + 16];
                    Array.Copy(received, 12, response, 12, questionEnd - 12);
                    response[0] = received[0];
                    response[1] = received[1];
                    response[2] = 0x81;
                    response[3] = 0x80;
                    // שאלה אחת
                    response[4] = 0x00;
                    response[5] = 0x01;

                    // תשובה אחת
                    response[6] = 0x00;
                    response[7] = 0x01;

                    // אין NS
                    response[8] = 0x00;
                    response[9] = 0x00;

                    // אין AR
                    response[10] = 0x00;
                    response[11] = 0x00;



                    int answerStart = questionEnd;
                    response[answerStart] = 0xC0;// תמיד מתחיל ב192)11)
                    response[answerStart++] = 0x0C;
                    response[answerStart++] = 0x00;
                    response[answerStart++] = 0x01;
                    response[answerStart++] = 0x00;
                    response[answerStart++] = 0x01;
                    response[answerStart++] = 0x00;
                    response[answerStart++] = 0x00;
                    response[answerStart++] = 0x00;
                    response[answerStart++] = 0x00;
                    response[answerStart++] = 0x00;
                    response[answerStart++] = 0x04;
                    response[answerStart++] = 0xC0;
                    response[answerStart++] = 0xA8;
                    response[answerStart++] = 0x07;
                    response[answerStart++] = 0x18;

                    client.Send(response, response.Length, remoteEP);
                    Console.WriteLine("Sent spoofed DNS response.");
                }
                else
                {
                    Console.WriteLine("Allowed - Forwarding to upstream DNS");
                    try
                    {
                        using (UdpClient upstreamClient = new UdpClient())
                        {
                            upstreamClient.Connect("8.8.8.8", 53);
                            upstreamClient.Send(received, received.Length);
                            IPEndPoint upstreamEP = new IPEndPoint(IPAddress.Any, 0);
                            byte[] upstreamResponse = upstreamClient.Receive(ref upstreamEP);
                            client.Send(upstreamResponse, upstreamResponse.Length, remoteEP);
                            Console.WriteLine($"Relayed response from 8.8.8.8 to {remoteEP.Address}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error forwarding DNS query: {ex.Message}");
                    }
                }



            }

        }
    }
}
