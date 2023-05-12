using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;

namespace PolarShadow.Core
{
    public struct HtmlElement
    {
        private readonly XPathNavigator _node;
        private readonly XPathNodeIterator _nodes;

        private HtmlValueKind _valueKind;

        public HtmlValueKind ValueKind => _valueKind;

        public HtmlElement(XPathDocument doc) : this(doc.CreateNavigator()) { }

        public HtmlElement(XPathNavigator nav)
        {
            _node = nav;
            _nodes = default;
            _valueKind = HtmlValueKind.Node;
        }

        public HtmlElement(XPathNodeIterator nodes)
        {
            _nodes = nodes;
            _node = default;
            _valueKind = HtmlValueKind.Nodes;
        }

        public HtmlElement Select(string xpath)
        {
            if (_valueKind == HtmlValueKind.Node)
            {
                var result = _node.Select(xpath);
                if (result.Count == 0)
                {
                    return default;
                }
                else if (result.Count == 1)
                {
                    result.MoveNext();
                    return new HtmlElement(result.Current);
                }
                return new HtmlElement(result);
            }
            else if (_valueKind == HtmlValueKind.Nodes)
            {
                XPathNodeCollection nodes = new XPathNodeCollection();
                while (_nodes.MoveNext())
                {
                    var childs = _nodes.Current.Select(xpath);
                    while (childs.MoveNext())
                    {
                        nodes.Add(childs.Current);
                    }
                }
                return new HtmlElement(nodes);
            }

            return default;
        }

        public string GetValue()
        {
            if (_valueKind == HtmlValueKind.Node)
            {
                return _node.Value;
            }
            throw new InvalidOperationException("The value must be a node");
        }

        public IEnumerable<HtmlElement> EnumerateElements()
        {
            if (_valueKind != HtmlValueKind.Nodes)
            {
                throw new InvalidOperationException("The value must be a collection of nodes");
            }
            while (_nodes.MoveNext())
            {
                yield return new HtmlElement(_nodes.Current);
            }
        }
        
    }
}
