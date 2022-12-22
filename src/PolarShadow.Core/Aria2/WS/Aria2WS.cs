using System;
using System.Collections.Generic;
using System.Text;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Pipes;
using System.IO.Pipelines;
using System.Buffers;
using System.Text.Json;
using System.IO;

namespace PolarShadow.Core.Aria2
{
    public class Aria2WS : IAria2Client, IDisposable
    {
        private readonly ClientWebSocket _ws;
        private readonly Uri _uri;
        private readonly Pipe _pipe;
        private Dictionary<string, Type> _responseMapper = new Dictionary<string, Type>();

        public Aria2WS(Uri uri):this(uri, null)
        {

        }

        public Aria2WS(Uri uri, string secret)
        {
            _ws = new ClientWebSocket();
            _uri = uri;
            _pipe = new Pipe();

            Secret = secret;
        }

        public int ReceiveBufferSize = 1024;
        public int SendBufferSize = 1024 * 4;

        public string Secret { get; }

        public Uri Uri => _uri;

        public string Id { get; }

        public event EventHandler<Aria2Response> ResponseReceived;

        public async Task ConnectAsync(CancellationToken cancellation = default)
        {
            if (_ws.State == WebSocketState.Open)
            {
                return;
            }
            await _ws.ConnectAsync(_uri, cancellation);

            ProcessLinesAsync(cancellation);
        }

        public Task CloseAsync(CancellationToken cancellation = default)
        {
            return _ws.CloseAsync(WebSocketCloseStatus.Empty, "user", cancellation);
        }

        public Aria2Request CreateRequest()
        {
            return new Aria2Request(Secret);
        }

        public Task SendAsync(Aria2Request request, CancellationToken cancellation = default)
        {
            request.Id = request.Method;
            var type = request.GetType();
            if (!_responseMapper.ContainsKey(request.Id) && type.IsGenericType)
            {
                var genericsArgumentTypes = type.GetGenericArguments();
                if (genericsArgumentTypes.Length > 0)
                {
                    _responseMapper[request.Id] = genericsArgumentTypes[0];
                }
            }
            var json = JsonSerializer.Serialize(request, JsonOption.ForDashSerializer);
            return _ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(json)), WebSocketMessageType.Text, true, cancellation);
        }

        private async void ProcessLinesAsync(CancellationToken cancellation)
        {
            var writing = FillPipeAsync(_pipe.Writer, cancellation);
            var reading = ReadPipeAsync(_pipe.Reader, cancellation);
            await Task.WhenAll(writing, reading);
        }

        private async Task FillPipeAsync(PipeWriter writer, CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                var memory = writer.GetMemory(ReceiveBufferSize);
                try
                {
                    var result = await _ws.ReceiveAsync(memory, cancellation).ConfigureAwait(false);
                    if (result.Count == 0)
                    {
                        break;
                    }
                    writer.Advance(result.Count);
                }
                catch (Exception ex)
                {
                    try
                    {
                        await _ws.CloseAsync(WebSocketCloseStatus.ProtocolError, ex.Message, CancellationToken.None);
                    }
                    catch { }
                    break;
                }


                var flushResult = await writer.FlushAsync().ConfigureAwait(false);
                if (flushResult.IsCompleted)
                {
                    break;
                }
            }

            await writer.CompleteAsync();
        }

        private async Task ReadPipeAsync(PipeReader reader, CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                var result = await reader.ReadAsync().ConfigureAwait(false);
                var buffer = result.Buffer;

                var data = buffer.Slice(buffer.Start);
                reader.AdvanceTo(buffer.Start, buffer.End);

                try
                {
                    ProcessData(ref data);
                }
                catch (Exception ex)
                {
                    try
                    {
                        await _ws.CloseAsync(WebSocketCloseStatus.ProtocolError, ex.Message, CancellationToken.None);
                    }
                    catch { }
                    break;
                }

                if (result.IsCompleted)
                {
                    break;
                }
            }

            await reader.CompleteAsync();
        }

        private void ProcessData(ref ReadOnlySequence<byte> buffer)
        {
            var doc = JsonDocument.Parse(buffer);

            var method = string.Empty;
            if (doc.RootElement.TryGetProperty("method", out JsonElement element))
            {
                method = element.GetString();
            }

            if (string.IsNullOrEmpty(method))
            {
                if (doc.RootElement.TryGetProperty("id", out element))
                {
                    method = element.GetString();
                }
            }

            if (string.IsNullOrEmpty(method))
            {
                return;
            }

            if (_responseMapper.TryGetValue(method, out Type type))
            {   
                var responseType = typeof(Aria2Response<>).MakeGenericType(type);
                var response = JsonSerializer.Deserialize(doc, responseType, JsonOption.ForDashSerializer);
                ProcessResponse(method, response as Aria2Response);
                ResponseReceived?.Invoke(this, response as Aria2Response);

            }

        }

        protected virtual void ProcessResponse(string method, Aria2Response response)
        {

        }

        public void Dispose()
        {
            _ws.Dispose();
        }
    }
}
