using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core.Models
{
    public interface IKeyNameCollection<T> : ICollection<T>, IReadOnlyDictionary<string, T> where T : IKeyName
    {
    }
}
