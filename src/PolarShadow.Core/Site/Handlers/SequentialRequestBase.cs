using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public abstract class SequentialRequestBase<TOutput> : ISequentialRequest<TOutput>
    {
        private List<ISite> _sites;
        private Queue<ISite> _siteFailedQueue;
        protected string RequestName;

        protected int Index = -1;

        public SequentialRequestBase(string requestName, IEnumerable<ISite> sites)
        {
            _sites = new List<ISite>(sites);
            RequestName = requestName;
            Reset();
        }

        /// <summary>
        /// false 请求失败时抛出异常
        /// true 忽略异常，并请求下个site
        /// </summary>
        public bool IgnoreError { get; set; }
        /// <summary>
        /// 自动排序(请求失败的站点下一轮请求时自动排到最后)
        /// </summary>
        public bool AutoSort { get; set; }

        public ISite Current => Index >= 0 && Index < _sites.Count ? _sites[Index] : null;

        public virtual void Reset()
        {
            Index = -1;
            if (AutoSort)
            {
                while (_siteFailedQueue != null && _siteFailedQueue.Count > 0)
                {
                    _sites.Add(_siteFailedQueue.Dequeue());
                }
            }
        }

        public async Task<TOutput> SearchNextAsync(CancellationToken cancellation = default)
        {
            using var stream = new MemoryStream();
            await SearchNextAsync(stream, cancellation);
            stream.Seek(0, SeekOrigin.Begin);
            if (stream.Length == 0)
            {
                return default;
            }
            return JsonSerializer.Deserialize<TOutput>(stream, JsonOption.DefaultSerializer);
        }

        public async Task SearchNextAsync(Stream stream, CancellationToken cancellation = default)
        {
            if (Index >= _sites.Count || cancellation.IsCancellationRequested)
            {
                return;
            }

            try
            {
                var request = BeforeRequest(stream, cancellation);

                if (request == null)
                {
                    return;
                }

                await ExecuteAsync(request, stream, cancellation).ConfigureAwait(false);

                await AfterRequest(stream, cancellation);
            }
            catch (Exception ex)
            {
                if (AutoSort)
                {
                    if (_siteFailedQueue == null)
                    {
                        _siteFailedQueue = new Queue<ISite>();
                    }
                    _siteFailedQueue.Enqueue(Current);
                    _sites.RemoveAt(Index);
                }

                if (!IgnoreError)
                {
                    throw ex;
                }
            }
            finally
            {
                stream.Seek(0, SeekOrigin.Begin);
            }

        }

        protected virtual Task ExecuteAsync(ISiteRequestHandler request, Stream stream, CancellationToken cancellation)
        {
            return request.ExecuteAsync(stream, cancellation);
        }

        protected virtual ISiteRequestHandler BeforeRequest(Stream stream, CancellationToken cancellation)
        {
            Index++;
            if (Current == null)
            {
                return null;
            }
            return Current.CreateRequestHandler(RequestName);
        }

        protected virtual async Task AfterRequest(Stream stream, CancellationToken cancellation)
        {
            stream.Seek(0, SeekOrigin.Begin);
            if (!HasResult(stream))
            {
                stream.SetLength(0);
                stream.Seek(0, SeekOrigin.Begin);
                await SearchNextAsync(stream, cancellation);
                return;
            }
            stream.Seek(0, SeekOrigin.Begin);
        }

        /// <summary>
        /// 判断本次请求结果是否有效
        /// false 则马上请求下一个站点
        /// true 不处理正常返回结果
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected abstract bool HasResult(Stream stream);
    }

    public abstract class SequentialRequestBase<TInput, TOutput> : SequentialRequestBase<TOutput> where TInput : class
    {
        private int _lastIndex = -2;

        protected TInput Input;
        public SequentialRequestBase(string requestName, TInput input, IEnumerable<ISite> sites) : base(requestName, sites)
        {
            Input = input;
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            _lastIndex = -2;
        }

        protected override ISiteRequestHandler BeforeRequest(Stream stream, CancellationToken cancellation)
        {
            if (_lastIndex < Index)
            {
                Index++;
                _lastIndex = Index;
                if (Current == null)
                {
                    return null;
                }
                var request = Current.CreateRequestHandler(RequestName);
                ResetRequest(Input, request);
                return request;
            }
            else
            {
                var request = Current.CreateRequestHandler(RequestName);
                if (request.TryGetParameter("canPage", out bool canPage) && canPage)
                {
                    NextRequest(Input, request);
                    return request;
                }
                else
                {
                    Index++;
                    _lastIndex = Index;
                    if (Current == null)
                    {
                        return default;
                    }
                    var nextRequest = Current.CreateRequestHandler(RequestName);
                    ResetRequest(Input, nextRequest);
                    return nextRequest;
                }
            }
        }

        protected override Task ExecuteAsync(ISiteRequestHandler request, Stream stream, CancellationToken cancellation)
        {
            return request.ExecuteAsync(JsonSerializer.Serialize(Input, JsonOption.DefaultSerializer), stream, cancellation);
        }

       
        /// <summary>
        /// 重置请求参数
        /// Reset或首次请求站点时调用
        /// </summary>
        /// <param name="input"></param>
        protected abstract void ResetRequest(TInput input, ISiteRequestHandler site);
        /// <summary>
        /// 同一个站点非第一次请求时调用
        /// </summary>
        /// <param name="input"></param>
        protected abstract void NextRequest(TInput input, ISiteRequestHandler site);
    }
}
