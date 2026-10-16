using System;
using System.Reflection;
using MonoMod.RuntimeDetour;

namespace EvenMoreBalancedMortician.Patches;

internal sealed class ScopedValue<T> where T : class
{
    private Hook _hook;

    public T Current { get; private set; }

    public void SetDuring(MethodInfo instanceMethod) => _hook = new Hook(instanceMethod, HoldDuringCall);

    private void HoldDuringCall(Action<T> orig, T self)
    {
        var outer = Current;
        Current = self;

        try
        {
            orig(self);
        }
        finally
        {
            Current = outer;
        }
    }
}
