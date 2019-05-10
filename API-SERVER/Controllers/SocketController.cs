using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;
using API_SERVER.Common;
using API_SERVER.Buss;

namespace API_SERVER.Controllers
{
    public class SocketController
    {
        public const int BufferSize = 4096;
        public string basestringjson = string.Empty;
        WebSocket socket;
        SocketController(WebSocket socket)
        {
            this.socket = socket;
        }

        /// <summary>
        /// branches the request pipeline for this SocketHandler usage
        /// </summary>
        /// <param name="app"></param>
        public static void Map(IApplicationBuilder app)
        {
            app.UseWebSockets();
            app.Use(SocketController.Acceptor);
        }

        static async Task Acceptor(HttpContext hc, Func<Task> n)
        {
            if (!hc.WebSockets.IsWebSocketRequest)
                return;
            try
            {
                var socket = await hc.WebSockets.AcceptWebSocketAsync();
                var h = new SocketController(socket);
                await h.EchoLoop();
                Console.Out.WriteLine(socket.State); 
            }
            catch
            { }
        }

        async Task EchoLoop()
        {
            var buffer = new byte[BufferSize];
            var seg = new ArraySegment<byte>(buffer);

            while (this.socket.State == WebSocketState.Open)
            {
                var incoming = await this.socket.ReceiveAsync(seg, CancellationToken.None);
                string receivemsg = Encoding.UTF8.GetString(buffer, 0, incoming.Count);

                if (receivemsg.StartsWith("getPayState"))
                {
                    Task task = Task.Run(() => GetPayState(receivemsg.Split(':')[1]));
                }
            }

        }

        async Task GetPayState(string scanCode)
        {
            DateTime dateTime = DateTime.Now;
            while (this.socket.State == WebSocketState.Open)
            {
                WxpayStatus state = Common.Utils.GetCache<WxpayStatus>(scanCode);
                if (state == null)
                {
                    Thread.Sleep(500);                   
                    if (DateTime.Now < dateTime.AddSeconds(15))
                    {                      
                        continue;
                    }
                }
                string userMsg = "";
                string reMsg = "";
                if (state != null)
                {
                    userMsg = state.msg == null ? "" : state.msg;
                    reMsg = "{\"type\":\"" + state.type.ToString() + "\",\"msg\":\"" + userMsg+"\"}";
                }
                else
                {
                    reMsg = "{\"type\":\"0\",\"msg\":\"请刷新页面查看支付情况\"}";
                }
                Common.Utils.DeleteCache(scanCode);
                byte[] x = Encoding.UTF8.GetBytes(reMsg);
                var outgoing = new ArraySegment<byte>(x);
                await this.socket.SendAsync(outgoing, WebSocketMessageType.Text, true, CancellationToken.None);
               
                break;
            }
        }


    }
}
