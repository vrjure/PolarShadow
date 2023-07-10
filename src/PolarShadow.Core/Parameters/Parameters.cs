using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using System.Text.Json;

namespace PolarShadow.Core
{
    public class Parameters : IParameter
    {
        private IList<IParameter> _parameters;

        public Parameters() : this(Enumerable.Empty<IParameter>())
        {

        }

        public Parameters(params IParameter[] parameters) : this(new List<IParameter>(parameters))
        {

        }

        public Parameters(IEnumerable<IParameter> parameters)
        {
            _parameters = new List<IParameter>();

            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    Add(parameter);
                }
            }
        }

        public string Name => "parameters";

        public void Add(IParameter parameter)
        {
            if (parameter == null)
            {
                return;
            }
            _parameters.Add(parameter);
        }

        public bool TryGetValue(string key, out ParameterValue value)
        {
            value = default;
            var len = _parameters.Count;
            for (int i = len - 1; i >= 0; i--)
            {
                var parameter = _parameters[i];
                if (parameter.TryGetValue(key, out value))
                {
                    return true;
                }
            }

            return false;
        }

        public void WriteTo(Utf8JsonWriter writer)
        {
            foreach (var item in _parameters)
            {
                item.WriteTo(writer);
            }
        }
    }
}
