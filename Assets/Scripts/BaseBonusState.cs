using UnityEngine;
using static IBonuseState;

public class BaseBonusState : IBonuseState
{
    private BonuseType _bonuseType;
    private MonoBehaviour _bonuseHandler = null;
    public BonuseType CurrentBonuseType { get => _bonuseType; }

    public MonoBehaviour BonuseHandler { get => _bonuseHandler; }

    public BaseBonusState(MonoBehaviour gameBonuse, BonuseType bonuseType)
    {
        _bonuseHandler = gameBonuse;
        _bonuseType = bonuseType;
    }
}
