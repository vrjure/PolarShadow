using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.Controls;

public class NativePointerEventArgs : RoutedEventArgs
{
    private readonly int _id;
    private readonly RawPointerEventType _eventType;
    private readonly PointerType _pointerType;

    public NativePointerEventArgs(int id, PointerType pointerType, RawPointerEventType eventType) : this(default, id, pointerType, eventType)
    {

    }

    public NativePointerEventArgs(RoutedEvent routedEvent, int id, PointerType pointerType, RawPointerEventType eventType) : base(routedEvent)
    {
        _id = id;
        _pointerType = pointerType;
        _eventType = eventType;
    }
    public int Id => _id;
    public PointerType Type => _pointerType;
    public RawPointerEventType Event => _eventType;
}

public class NativePointerPointEventArgs : NativePointerEventArgs
{
    private readonly Point _point;
    public NativePointerPointEventArgs(int id, PointerType pointerType, RawPointerEventType eventType, Point point) : base(id, pointerType, eventType)
    {
        _point = point;
    }

    public Point Point => _point;
}
