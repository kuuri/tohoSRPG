using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tohoSRPG
{
    /// <summary>
    /// 地形
    /// </summary>
    public enum Terrain
    {
        Plain, Forest, Mountain, Waterside, Indoor, Red_hot, Sanctuary, Miasma, Banned
    }

    /// <summary>
    /// 地形相性
    /// </summary>
    public enum Affinity
    {
        Best, Good, Normal, Bad
    }

    /// <summary>
    /// 属性
    /// </summary>
    public enum Type
    {
        Power, Guard, Intelligence, Apparition, Technic, Fortune
    }

    /// <summary>
    /// 敵か味方か
    /// </summary>
    public enum FrendOfFoe
    {
        Ally, Enemy
    }

    /// <summary>
    /// ユニットの種類
    /// </summary>
    public enum CharaID
    {
        Reimu, Marisa, Sanae, Chrino, Oku, Zero, Else
    }

    /// <summary>
    /// 行動の効果
    /// 0～99:近接 100～199:遠隔 200～:装備
    /// %100 = 0:攻撃
    /// %2 = 0:相手に回避される 1:相手に回避されない
    /// </summary>
    public enum ActType
    {
        Grapple = 0,
        Heal = 1,
        Revive = 3,
        AddPlusSympton = 5,
        ClearMinusSympton = 7,
        Guard = 9,
        LessGuard = 11,
        Counter = 13,
        SPUp = 15,
        SetCrystal = 17,
        ClearCrystal = 19,
        LevelDrain = 21,
        Musoutensei = 23,

        Shot = 100,
        AddMinusSympton = 102,
        ClearPlusSympton = 103,
        SetTrap = 105,
        ClearTrap = 107,
        SPDrain = 109,

        Booster = 201,
        Scope,
        DualBoost,
        Charge
    }

    /// <summary>
    /// 攻撃の特性
    /// </summary>
    public enum ActAbility
    {
        None, Fast, Rush, Hit, Penetrate, Diffuse, Shock, Vacuum, Drain, AntiMinus, AntiPlus, Destroy, Sanctio, Heat, Cold, Thunder, Laser
    }

    /// <summary>
    /// 行動の対象
    /// </summary>
    public enum ActTarget
    {
        Ally1, AllyAll, Enemy1, EnemyAll, AllyEnemy1, Field, Space, Equip
    }

    /// <summary>
    /// プラス症状
    /// </summary>
    public enum SymptonPlus
    {
        None, Heal, Charge, Concentrate, Swift, AbsoluteDodge, ActAgain, Stigmata, PlusInvalid
    }

    /// <summary>
    /// マイナス症状
    /// </summary>
    public enum SymptonMinus
    {
        None, Slip, Distract, Restraint, Stop, Confuse, Deguard, Dedodge, FixInside, FixOutside, CarvedSeal, MinusInvalid
    }

    /// <summary>
    /// トラップ
    /// </summary>
    public enum Trap
    {
        None, GrappleTrap, ShotTrap, AttackTrap, OnceClear, SPPlant, HitPromise, MagicCharge, TrapClear
    }

    /// <summary>
    /// 結晶効果
    /// </summary>
    public enum CrystalEffect
    {
        None, HPDamage, HPHeal, ForbidHeal, APUp, APDown, CostUp, HitUp, DamageUp, DamageDown, TimeStop, AffinityDown, ChangeTerrain
    }

    /// <summary>
    /// アビリティ
    /// </summary>
    public enum Ability
    {
        None,
        ActAgain,
        Drive
    }

    /// <summary>
    /// アーティファクト
    /// </summary>
    public enum Artifact
    {
        None
    }
}
