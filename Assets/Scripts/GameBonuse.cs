using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBonuse : FlyableObject
{
    [Serializable]
    public struct SpriteConfig
    {
        public IBonuseState.BonuseType bonuseType;
        public Sprite sprite;
    }

    private IBonuseState _bonuseState;

    public List<SpriteConfig> spriteConfigs;
    public string[] colisionTags;


    #region Unity Methods
    private void Update()
    {
        UpdateMovingLogic();
    }
    #endregion

    #region Public Methods
    public void Activate(IBonuseState.BonuseType bonuseType)
    {
        _bonuseState = new BaseBonusState(this, bonuseType);
        gameObject.SetActive(true);
    }

    public override void Initialize()
    {
        base.Initialize();
        var spriteConfig = spriteConfigs.SingleOrDefault(item => item.bonuseType == _bonuseState.CurrentBonuseType);
        GetComponent<SpriteRenderer>().sprite = spriteConfig.sprite;
        var colisionAction = new Action<GameObject>(obj =>
        {
            if (obj.TryGetComponent<IBonuseOwner>(out var component))
            {
                component.ActivateBonuseByType(_bonuseState.CurrentBonuseType);
            }
        });
        colisingDetecter.Activate(_bonuseState.GetType(), colisionAction, gameObject, colisionTags, IColisingDetecter.RayDirection.DOWN);
        colisingDetecter.OnDetectedEvent.AddListener(() => Destroy(gameObject));
    }
    #endregion

    #region Private Methods
    private void UpdateMovingLogic()
    {
        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) > 25
            && PlayerController.Instance.IsControlLocked)
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
