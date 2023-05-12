using System;
using System.Collections.Generic;
using System.Text;

namespace PolarShadow.Core
{
    internal static class NameSlotConstants
    {
        public const byte a = (byte)'a';
        public const byte z = (byte)'z';
        public const byte A = (byte)'A';
        public const byte Z = (byte)'Z';
        public const byte _ = (byte)'_';
        public const byte Num0 = (byte)'0';
        public const byte Num9 = (byte)'9';
        public const byte Add = (byte)'+';
        public const byte Minus = (byte)'-';
        public const byte Multiply = (byte)'*';
        public const byte Divide = (byte)'/';
        public const byte Space = (byte)' ';
        public const byte SlotStart = (byte)'{';
        public const byte SlotEnd = (byte)'}';
        public const byte JsonPathRoot = (byte)'$';
        public const byte Colon = (byte)':';
        public const byte At = (byte)'@';
        public const byte Tilde = (byte)'~';

        public static ReadOnlySpan<byte> XPathEndChars => new byte[]
        { (byte)' ', (byte)'}', (byte)':'};

        public static ReadOnlySpan<byte> RegexModifyChars => new byte[] { (byte)'g', (byte)'i', (byte)'m', (byte)'s' };

    }
}
