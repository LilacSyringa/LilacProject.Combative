namespace LilacProject.Combative.Weaponry.Mechanism.Constructs;

public interface IWeaponMechanismConstruct
{
    protected internal abstract IWeaponMechanism ConstructWeaponMechanism(ActiveWeaponry manager);

    protected internal string NameID();
}
