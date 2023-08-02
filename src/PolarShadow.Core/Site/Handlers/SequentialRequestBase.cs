using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            Index++;
            return Current != null;
        }

        public void Dispose()
        {
            
        }

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

        public async Task<TOutput> ExecuteAsync(CancellationToken cancellation = default)
        {
            if (Current == null)
            {
                return default;
            }

            using var stream = new MemoryStream();
            await ExecuteAsync(stream, cancellation);
            stream.Seek(0, SeekOrigin.Begin);
            if (stream.Length == 0)
            {
                return default;
            }
            return JsonSerializer.Deserialize<TOutput>(stream, JsonOption.DefaultSerializer);
        }

        public async Task ExecuteAsync(Stream stream, CancellationToken cancellation = default)
        {
            if (Current == null || cancellation.IsCancellationRequested)
            {
                return;
            }

            var request = Current.CreateRequestHandler(RequestName);
            if (request == null) return;
            try
            {
                BeforeRequest(request);
                await ExecuteAsync(request, stream, cancellation).ConfigureAwait(false);
                AfterRequest(request);
            }
            catch (Exception ex)
            {
                if (AutoSort)
                {
                    if (Current != null)
                    {
                        if (_siteFailedQueue == null)
                        {
                            _siteFailedQueue = new Queue<ISite>();
                        }
                        _siteFailedQueue.Enqueue(Current);
                        _sites.RemoveAt(Index);
                    }          
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
        protected virtual void BeforeRequest(ISiteRequestHandler request)
        {

        }

        protected virtual void AfterRequest(ISiteRequestHandler request)
        {

        }
    }

    public abstract class SequentialRequestBase<TInput, TOutput> : SequentialRequestBase<TOutput> where TInput : class
    {
        protected TInput Input { get; }
        public SequentialRequestBase(string requestName, TInput input, IEnumerable<ISite> sites) : base(requestName, sites)
        {
            Input = input;
        }


        protected override Task ExecuteAsync(ISiteRequestHandler request, Stream stream, CancellationToken cancellation)
        {
            return request.ExecuteAsync(JsonSerializer.Serialize(Input, JsonOption.DefaultSerializer), stream, cancellation);
        }
    }
}
