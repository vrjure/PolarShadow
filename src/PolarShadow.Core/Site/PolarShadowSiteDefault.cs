﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal class PolarShadowSiteDefault : IPolarShadowSite
    {
        private readonly Dictionary<string, object> _abilities;
        internal PolarShadowSiteDefault(string name, string domain, Dictionary<string, object> abilities)
        {
            this.Name = name;
            this.Domain = domain;
            _abilities = abilities;
        }

        public string Name { get; }

        public string Domain { get; }

        public object GetAbility(string abilityName)
        {
            if (_abilities == null)
            {
                return null;
            }

            if (_abilities.TryGetValue(abilityName, out object val))
            {
                return val;
            }

            return null;
        }

        public bool HasAbility(string abilityName)
        {
            if (_abilities == null)
            {
                return false;
            }

            return _abilities.ContainsKey(abilityName);
        }
    }
}
