using AntDesign.Core.Extensions;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PolarShadow
{
    internal class NavigationService : INavigationService
    {
        private readonly IStateContext _context;
        private readonly NavigationManager _nav;
        private Stack<NavigationState> _historyStack = new();
        private Stack<NavigationState> _stateStack = new();

        public NavigationService(IStateContext context, NavigationManager nav)
        {
            _context=  context;
            _nav = nav;
        }

        //TODO 保留状态的页面返回， 清除状态
        public void Back()
        {
            if (CanBack())
            {
                var pre = _historyStack.Pop();

                if (pre.Parameters != default && pre.Parameters.Count > 0)
                {
                    foreach (var item in pre.Parameters)
                    {
                        _context.Remove(item.Key);
                    }
                }

                if (_stateStack.TryPop(out NavigationState state) && state.Parameters != null && state.Parameters.Count > 0)
                {
                    foreach (var item in state.Parameters)
                    {
                        _context.Remove(item.Key);
                    }
                }

                NavigateTo(new NavigationState(pre.FromUrl)
                {
                    Parameters = pre.States
                }, false, true);
            }
        }

        public bool CanBack()
        {
            return _historyStack.Count > 0;
        }

        public void NavigateTo(NavigationState state, bool forceLoad = false)
        {
            NavigateTo(state, forceLoad, false);
        }

        private void NavigateTo(NavigationState state, bool forceLoad, bool isBack)
        {
            state.FromUrl = _nav.Uri;
            if (state.CanBack)
            {
                _historyStack.Push(state);
            }

            if (state.Parameters != default && state.Parameters.Count > 0)
            {
                foreach (var item in state.Parameters)
                {
                    _context.Set(item.Key, item.Value);
                }

                if (isBack)
                {
                    _stateStack.Push(state);
                }

            }
            _nav.NavigateTo(state.ToUrl, forceLoad, true);
        }
    }
}
