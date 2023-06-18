using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PolarShadow.Core
{
    public abstract class SequentialRequestBase<TInput, TOutput> : ISequentialRequest<TOutput> where TInput : class
    {
        private List<IPolarShadowSite> _sites;
        private Queue<IPolarShadowSite> _siteFailedQueue;
        private int _index = 0;
        private int _lastIndex = -1;
        private string _abilityName;

        protected TInput Input;
        protected NameSlotValueCollection Parameters;
        public SequentialRequestBase(string abilityName, TInput input, IEnumerable<IPolarShadowSite> sites, NameSlotValueCollection parameters)
        {
            _sites = new List<IPolarShadowSite>(sites);
            _abilityName = abilityName;
            Input = input;
            Parameters = parameters;
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

        public IPolarShadowSite Current => _index >= 0 && _index < _sites.Count ? _sites[_index] : null;

        public void Reset()
        {
            _index = 0;
            _lastIndex = -1;
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
            return JsonSerializer.Deserialize<TOutput>(stream, JsonOption.DefaultSerializer);
        }

        public async Task SearchNextAsync(Stream stream, CancellationToken cancellation = default)
        {
            if (_index >= _sites.Count)
            {
                return;
            }

            var site = _sites[_index];

            try
            {
                if (_lastIndex < _index)
                {
                    _lastIndex = _index;
                    var request = site.CreateRequestHandler(_abilityName);
                    ResetRequest(Input, request);
                    
                    await request.ExecuteAsync(JsonSerializer.Serialize(Input, JsonOption.DefaultSerializer), stream, cancellation).ConfigureAwait(false);
                }
                else
                {
                    var request = site.CreateRequestHandler(_abilityName);
                    NextRequest(Input, request);
                    await request.ExecuteAsync(JsonSerializer.Serialize(Input, JsonOption.DefaultSerializer), stream, cancellation).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                if (AutoSort)
                {
                    if (_siteFailedQueue == null)
                    {
                        _siteFailedQueue = new Queue<IPolarShadowSite>();
                    }
                    _siteFailedQueue.Enqueue(site);
                    _sites.RemoveAt(_index);
                }

                if (!IgnoreError)
                {
                    throw ex;
                }
            }

            if (cancellation.IsCancellationRequested)
            {
                return;
            }

            if (!HasResult(stream))
            {
                _index++;
                await SearchNextAsync(stream, cancellation);
            }

            stream.Seek(0, SeekOrigin.Begin);
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
        /// <summary>
        /// 判断本次请求结果是否有效
        /// false 则马上请求下一个站点
        /// true 不处理正常返回结果
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected abstract bool HasResult(Stream stream);
    }
}
