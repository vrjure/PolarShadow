using Android.Views;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls
{
    /// <summary>
    /// <see cref="https://github.com/AvaloniaUI/Avalonia/blob/master/src/Android/Avalonia.Android/Platform/Specific/Helpers/AndroidMotionEventsHelper.cs"/>
    /// </summary>
    internal class PlatformEventHandler : IDisposable
    {
        private readonly View _view;
        private readonly double _renderScaling;

        private INativeInteraction _interaction;
        private Point _moveStartPoint;
        public PlatformEventHandler(View view, INativeInteraction interaction)
        {
            _view = view;
            _interaction = interaction;
            var topLevel = TopLevel.GetTopLevel(_interaction as VirtualView);
            _renderScaling = topLevel == null ? 1 : topLevel.RenderScaling;
            _view.GenericMotion += _view_GenericMotion;
            _view.Touch += _view_Touch;
        }

        public void Dispose()
        {
            //_view.GenericMotion -= _view_GenericMotion;
            //_view.Touch -= _view_Touch;
        }

        private void DispatchMotionEvent(MotionEvent e)
        {
            var actionMasked = e.ActionMasked;
            var index = e.ActionIndex;
            var toolType = e.GetToolType(index);
            var pointer = GetPointerType(toolType);
            var actionType = GetActionType(toolType, actionMasked, e.ActionButton);
            var point = CreatePoint(e, index);

            if (actionMasked == MotionEventActions.Move)
            {
                //var delta = _moveStartPoint - point;
                //_moveStartPoint = point;

                if (toolType == MotionEventToolType.Finger)
                {
                    _interaction.OnNativePointerPoint(CreatePointEventArg(index, pointer, actionType, point));
                    //_interaction.OnNativePointerDelta(CreateDeltaEventArg(index, pointer, actionType, delta));
                }
            }
            else
            {
                if (actionMasked == MotionEventActions.Scroll && toolType == MotionEventToolType.Mouse)
                {
                    //var delta = new Vector(e.GetAxisValue(Axis.Hscroll), e.GetAxisValue(Axis.Vscroll));
                    //_interaction.OnNativePointerDelta(CreateDeltaEventArg(index, pointer, actionType, delta));
                }
                else
                {
                    _interaction.OnNativePointerPoint(CreatePointEventArg(index, pointer, actionType, point));
                }
            }
        }

        private void _view_GenericMotion(object sender, View.GenericMotionEventArgs e)
        {
            DispatchMotionEvent(e.Event);
        }

        private void _view_Touch(object sender, View.TouchEventArgs e)
        {
            DispatchMotionEvent(e.Event);
        }

        private static NativePointerPointEventArgs CreatePointEventArg(int id, PointerType type, RawPointerEventType eventType, Point point)
        {
            return new NativePointerPointEventArgs(id, type, eventType, point);
        }

        private Point CreatePoint(MotionEvent e, int index)
        {
            return new Point(e.GetX(index), e.GetY(index)) / _renderScaling;
        }

        private static PointerType GetPointerType(MotionEventToolType tooType)
        {
            return tooType switch
            {
                MotionEventToolType.Mouse => PointerType.Mouse,
                MotionEventToolType.Finger => PointerType.Touch,
                MotionEventToolType.Stylus => PointerType.Pen,
                MotionEventToolType.Eraser => PointerType.Pen,
                _ => PointerType.Mouse
            };
        }

        private static RawInputModifiers GetModifiers(MetaKeyStates metaState, MotionEventButtonState buttonState)
        {
            var modifiers = RawInputModifiers.None;
            if ((metaState & MetaKeyStates.ShiftOn) == MetaKeyStates.ShiftOn)
            {
                modifiers |= RawInputModifiers.Shift;
            }
            if ((metaState & MetaKeyStates.CtrlOn) == MetaKeyStates.CtrlOn)
            {
                modifiers |= RawInputModifiers.Control;
            }
            if ((metaState & MetaKeyStates.AltOn) == MetaKeyStates.AltOn)
            {
                modifiers |= RawInputModifiers.Alt;
            }
            if ((metaState & MetaKeyStates.MetaOn) == MetaKeyStates.MetaOn)
            {
                modifiers |= RawInputModifiers.Meta;
            }
            if ((buttonState & MotionEventButtonState.Primary) == MotionEventButtonState.Primary)
            {
                modifiers |= RawInputModifiers.LeftMouseButton;
            }
            if ((buttonState & MotionEventButtonState.Secondary) == MotionEventButtonState.Secondary)
            {
                modifiers |= RawInputModifiers.RightMouseButton;
            }
            if ((buttonState & MotionEventButtonState.Tertiary) == MotionEventButtonState.Tertiary)
            {
                modifiers |= RawInputModifiers.MiddleMouseButton;
            }
            if ((buttonState & MotionEventButtonState.Back) == MotionEventButtonState.Back)
            {
                modifiers |= RawInputModifiers.XButton1MouseButton;
            }
            if ((buttonState & MotionEventButtonState.Forward) == MotionEventButtonState.Forward)
            {
                modifiers |= RawInputModifiers.XButton2MouseButton;
            }
            if ((buttonState & MotionEventButtonState.StylusSecondary) == MotionEventButtonState.StylusPrimary)
            {
                modifiers |= RawInputModifiers.PenBarrelButton;
            }
            return modifiers;
        }


        private static RawPointerEventType GetActionType(MotionEventToolType toolType, MotionEventActions actionMasked, MotionEventButtonState actionButton)
        {
            var isTouch = toolType == MotionEventToolType.Finger;
            var isMouse = toolType == MotionEventToolType.Mouse;
            switch (actionMasked)
            {
                // DOWN
                case MotionEventActions.Down when !isMouse:
                case MotionEventActions.PointerDown when !isMouse:
                    return isTouch ? RawPointerEventType.TouchBegin : RawPointerEventType.LeftButtonDown;
                case MotionEventActions.ButtonPress:
                    return actionButton switch
                    {
                        MotionEventButtonState.Back => RawPointerEventType.XButton1Down,
                        MotionEventButtonState.Forward => RawPointerEventType.XButton2Down,
                        MotionEventButtonState.Primary => RawPointerEventType.LeftButtonDown,
                        MotionEventButtonState.Secondary => RawPointerEventType.RightButtonDown,
                        MotionEventButtonState.StylusPrimary => RawPointerEventType.LeftButtonDown,
                        MotionEventButtonState.StylusSecondary => RawPointerEventType.RightButtonDown,
                        MotionEventButtonState.Tertiary => RawPointerEventType.MiddleButtonDown,
                        _ => RawPointerEventType.LeftButtonDown
                    };
                // UP
                case MotionEventActions.Up when !isMouse:
                case MotionEventActions.PointerUp when !isMouse:
                    return isTouch ? RawPointerEventType.TouchEnd : RawPointerEventType.LeftButtonUp;
                case MotionEventActions.ButtonRelease:
                    return actionButton switch
                    {
                        MotionEventButtonState.Back => RawPointerEventType.XButton1Up,
                        MotionEventButtonState.Forward => RawPointerEventType.XButton2Up,
                        MotionEventButtonState.Primary => RawPointerEventType.LeftButtonUp,
                        MotionEventButtonState.Secondary => RawPointerEventType.RightButtonUp,
                        MotionEventButtonState.StylusPrimary => RawPointerEventType.LeftButtonUp,
                        MotionEventButtonState.StylusSecondary => RawPointerEventType.RightButtonUp,
                        MotionEventButtonState.Tertiary => RawPointerEventType.MiddleButtonUp,
                        _ => RawPointerEventType.LeftButtonUp
                    };
                // MOVE
                case MotionEventActions.Outside:
                case MotionEventActions.HoverMove:
                case MotionEventActions.Move:
                    return isTouch ? RawPointerEventType.TouchUpdate : RawPointerEventType.Move;
                // CANCEL
                case MotionEventActions.Cancel:
                    return isTouch ? RawPointerEventType.TouchCancel : RawPointerEventType.LeaveWindow;
                default:
                    return (RawPointerEventType)(-1);
            }
        }

        private static PointerUpdateKind GetPointerUpdateKind(RawPointerEventType type)
        {
            return type switch
            {
                RawPointerEventType.XButton2Up => PointerUpdateKind.XButton2Released,
                RawPointerEventType.XButton1Up => PointerUpdateKind.XButton1Released,
                RawPointerEventType.XButton2Down => PointerUpdateKind.XButton2Pressed,
                RawPointerEventType.XButton1Down => PointerUpdateKind.XButton1Pressed,
                RawPointerEventType.LeftButtonDown => PointerUpdateKind.LeftButtonPressed,
                RawPointerEventType.LeftButtonUp => PointerUpdateKind.LeftButtonReleased,
                RawPointerEventType.MiddleButtonDown => PointerUpdateKind.MiddleButtonPressed,
                RawPointerEventType.MiddleButtonUp => PointerUpdateKind.MiddleButtonReleased,
                RawPointerEventType.RightButtonDown => PointerUpdateKind.RightButtonPressed,
                RawPointerEventType.RightButtonUp => PointerUpdateKind.RightButtonReleased,
                RawPointerEventType.TouchBegin => PointerUpdateKind.LeftButtonPressed,
                RawPointerEventType.TouchEnd => PointerUpdateKind.LeftButtonReleased,
                RawPointerEventType.TouchCancel => PointerUpdateKind.LeftButtonReleased,
                _ => PointerUpdateKind.Other
            };
        }
    }
}
