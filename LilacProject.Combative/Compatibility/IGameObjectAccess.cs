using System.Diagnostics.CodeAnalysis;

namespace LilacProject.Combative.Compatibility;

/// <summary>
/// Interface to access basic functionality to add [add component]-like functions, and position and forward direction of the object. 
/// <br/>
/// Though I called this component, I'm just calling it this because it is familiar with people who have used Unity.
/// <br/>
/// <br/>
/// The interface is only used in <see cref="Effect.IEffectHandle"/>, and <see cref="Weaponry.IWeaponPortion"/>.
/// </summary>
public interface IGameObjectAccess
{
    /// <summary>
    /// The active state of the entire object, not just the script.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets the game object itself.
    /// </summary>
    public object GameObject { get; }

    public bool TryGetComponent(Type type, [NotNullWhen(false)] out object? component);
    public bool HasComponent(Type type);
    public object AddComponent(Type type)
    {
        if (TryGetComponent(type, out object? res)) throw new InvalidOperationException("Does not contain such component.");
        return res;
    }
    /// <summary>
    /// This might be more annoying to work with because the array type is object. 
    /// <br/>
    /// If the result of the function you're hooking this up with returns an array, you can cast it down to the required type if you hook it up to a method that also returns an array. Uniquely, Arrays are the only objects that you can cast from BaseObj[] to ChildObj and vice versa provided that the original type is ChildObj[].
    /// </summary>
    public object[] GetAllComponents();

    public bool TryGetNode(Type type, [NotNullWhen(false)] out object? component) => TryGetComponent(type, out component);
    public bool HasNode(Type type) => HasComponent(type);
    public sealed object AddNode(Type type) => AddComponent(type);
    /// <summary>
    /// This might be more annoying to work with because the array type is object. 
    /// <br/>
    /// If the result of the function you're hooking this up with returns an array, you can cast it down to the required type if you hook it up to a method that also returns an array. Uniquely, Arrays are the only objects that you can cast from BaseObj[] to ChildObj and vice versa provided that the original type is ChildObj[].
    /// </summary>
    public sealed object[] GetAllNodes() => GetAllComponents();

    public Vector3 Position { get; set; }
    public Vector3 Forward { get; set; }
    public Vector3 Back { get => -Forward; set => Forward = -value; }
    public Vector3 Left { get; set; }
    public Vector3 Right { get => -Left; set => Left = -value; }
    public Vector3 Up { get; set; }
    public Vector3 Down { get => -Up; set => Up = -value; }
}
