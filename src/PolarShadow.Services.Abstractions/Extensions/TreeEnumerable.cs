using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolarShadow.Services
{
    public sealed class TreeEnumerable
    {
        public static IEnumerable<T> EnumerateDeepFirst<T>(T tree, Func<T, IEnumerable<T>> getChildren)
        {
            var stack = new Stack<T>();
            stack.Push(tree);
            while (stack.Count > 0)
            {
                var node = stack.Pop();
                var children = getChildren(node);
                if (children != null)
                {
                    foreach (var item in children.Reverse())
                    {
                        stack.Push(item);
                    }
                }
                yield return node;
            }
        }

        public static IEnumerable<T> EnumerateBreadthFirst<T>(T tree, Func<T, IEnumerable<T>> getChildren)
        {
            var queue = new Queue<T>();
            queue.Enqueue(tree);
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                var children = getChildren(node);
                if (children != null)
                {
                    foreach (var item in children)
                    {
                        queue.Enqueue(item);
                    }
                }

                yield return node;
            }
        }

        public static IEnumerable<T> EnumerateBreadthFirstWithLayerFlag<T>(T tree, Func<T, IEnumerable<T>> getChildren)
        {
            var queue = new Queue<T>();
            queue.Enqueue(tree);
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (node != null)
                {
                    var children = getChildren(node);
                    if (children != null)
                    {
                        queue.Enqueue(default);
                        foreach (var item in children)
                        {
                            queue.Enqueue(item);
                        }
                    }
                }
                
                yield return node;
            }
        }

        public static IEnumerable<KeyValuePair<T,T>>EnumerateBreadthFirstWithParentChild<T>(T tree, Func<T, IEnumerable<T>> getChildren)
        {
            var queue = new Queue<T>();
            T parent = default;
            foreach (var item in EnumerateBreadthFirstWithLayerFlag(tree, getChildren))
            {
                if (item != null)
                {
                    queue.Enqueue(item);
                }
                else
                {
                    parent = queue.Dequeue();
                }

                yield return new KeyValuePair<T, T>(parent, item);
            }
        }
    }
}
