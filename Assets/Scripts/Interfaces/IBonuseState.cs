using static IBonuseState;

public interface IBonuseState
{
    public enum BonuseType
    {
        SHIELD, SMARTBULLETS
    }

    BonuseType CurrentBonuseType { get; }
}
public interface IBonuseOwner
{
    void ActivateBonuseByType(BonuseType bonuseType);
}
