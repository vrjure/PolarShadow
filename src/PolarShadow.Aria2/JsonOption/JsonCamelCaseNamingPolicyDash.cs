using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PolarShadow.Aria2
{
    internal class JsonCamelCaseNamingPolicyDash : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return CamelCase.ConvertName(name).Replace('_', '-');
        }
    }
}
