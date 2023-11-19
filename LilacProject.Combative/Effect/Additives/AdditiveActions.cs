using static LilacProject.Combative.Effect.EffectOperation;
using LilacProject.Combative.Effect.Instance.Active;
using LilacProject.Miscellaneous;
using System.Runtime.CompilerServices;

namespace LilacProject.Combative.Effect.Additives;

public sealed class AdditiveActions
{
    private readonly static int operations = Enum.GetNames<Flags>().Length;

    private readonly EffectAct?[] Integrated_actions;
    private readonly EffectAct?[] Temprary_actions;

    public void Start(EffectInstance instance) => Invoke(instance, Value.Start);
    public void Update(EffectInstance instance) => Invoke(instance, Value.Update);
    public void Hit(EffectInstance instance) => Invoke(instance, Value.Hit);
    public void End(EffectInstance instance) => Invoke(instance, Value.End);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Invoke(EffectInstance instance, Value index)
    {
        int i = unchecked((int)index);
        Invoke(instance, i);
    }

    public void Invoke(EffectInstance instance, int opeartion_id)
    {
        Integrated_actions[opeartion_id]?.Invoke(instance);
        Temprary_actions[opeartion_id]?.Invoke(instance);
    }

    internal AdditiveActions(IEnumerable<(Flags, EffectAct)> weap_methods)
    {
        Integrated_actions = new EffectAct[operations];
        Temprary_actions = new EffectAct[operations];

        AddMethods(weap_methods, Integrated_actions);
    }

    internal void AddTemporaryActions(IEnumerable<(Flags, EffectAct)> weap_methods) => AddMethods(weap_methods, Temprary_actions);

    internal void RemoveTemporaryActions(IEnumerable<(Flags, EffectAct)> weap_methods) => RemoveMethds(weap_methods, Temprary_actions);

    private static void AddMethods(IEnumerable<(Flags, EffectAct)> weap_methods, EffectAct?[] methods)
    {
        foreach ((Flags operation, EffectAct action) in weap_methods)
        {
            foreach (int indexes in BitFlags32.FlagIndexes(unchecked((uint)operation)))
            {
                methods[indexes] += action;
            }
        }
    }

    private static void RemoveMethds(IEnumerable<(Flags, EffectAct)> weap_methods, EffectAct?[] methods)
    {
        foreach ((Flags operation, EffectAct action) in weap_methods)
        {
            foreach (int indexes in BitFlags32.FlagIndexes(unchecked((uint)operation)))
            {
                methods[indexes] -= action;
            }
        }
    }
}
