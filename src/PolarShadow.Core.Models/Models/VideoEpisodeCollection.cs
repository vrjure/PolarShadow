using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    public class VideoEpisodeCollection : ICollection<VideoEpisode>
    {
        private List<VideoEpisode> _list = new List<VideoEpisode>();

        private readonly VideoSummary _summary;

        public VideoEpisodeCollection(VideoSummary summary)
        {
            _summary = summary;
        }

        public int Count => _list.Count;

        public bool IsReadOnly => false;

        public void Add(VideoEpisode item)
        {
            item.Summary = _summary;
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(VideoEpisode item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(VideoEpisode[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<VideoEpisode> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public bool Remove(VideoEpisode item)
        {
            return _list.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }
    }
}
