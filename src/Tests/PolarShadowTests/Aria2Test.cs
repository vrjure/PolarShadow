using PolarShadow.Aria2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolarShadowTests
{
    public class Aria2Test
    {
        private Aria2Http _rpc;
        private Aria2WS _ws;

        private TaskCompletionSource _tcs;
        [SetUp]
        public void Initialize()
        {
            _rpc = new Aria2Http(new Uri("http://localhost:16800/jsonrpc"));
            _ws = new Aria2WS(new Uri("ws://localhost:16800/jsonrpc"));
            _ws.ResponseReceived += _ws_ResponseReceived;
        }

        [Test]
        public async Task HttpTest()
        {
            var request = _rpc.CreateGetVersion();
            var result = await _rpc.PostAsync(request);
            Console.WriteLine(JsonSerializer.Serialize(result, JsonOption.DefaultSerializer));
        }

        [Test]
        public async Task WsTest()
        {
            await _ws.ConnectAsync();
            var request = _ws.CreateGetVersion();            
            await _ws.SendAsync(request);
            _tcs = new TaskCompletionSource();
            await Task.WhenAny(_tcs.Task , Task.Delay(3000));
        }

        private void _ws_ResponseReceived(object? sender, Aria2Response e)
        {
            Console.WriteLine(JsonSerializer.Serialize<object>(e, JsonOption.DefaultSerializer));
            _tcs.SetResult();
        }
    }
}
