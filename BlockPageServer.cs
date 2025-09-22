using System;
using System.Net;
using System.Text;

namespace SecuredWeb
{
    internal class BlockPageServer
    {
        public static void Start()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://*:80/"); // מאזין לכל הבקשות ב־פורט 80
            listener.Start();
            Console.WriteLine("HTTP Block Page Server started on port 80...");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerResponse response = context.Response;

                string html = @"<html>
                                <head><title>Blocked</title></head>
                                <body style='font-family: Arial; text-align: center; margin-top: 100px;'>
                                <h1 style='color: red;'>Access Denied</h1>
                                <p>This site has been blocked by your network administrator.</p>
                                </body></html>";

                byte[] buffer = Encoding.UTF8.GetBytes(html);
                response.ContentLength64 = buffer.Length;
                response.ContentType = "text/html";
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
        }
    }
}
