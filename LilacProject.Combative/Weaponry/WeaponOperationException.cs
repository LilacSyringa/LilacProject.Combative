using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace LilacProject.Combative.Weaponry
{
    internal class WeaponOperationException : NotImplementedException
    {
        public WeaponOperationException()
        {
        }

        public WeaponOperationException(string? message) : base(message)
        {
        }

        [DoesNotReturn, StackTraceHidden]
        public static void Throw(string name) => throw new WeaponOperationException(name);
    }
}
