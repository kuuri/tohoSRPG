using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tohoSRPG
{
    public enum Scene
    {
        None, Title, World, Talk, Confront, Battle, Menu
    }

    /// <summary>
    /// 地形
    /// </summary>
    public enum Terrain
    {
        Plain, Sanctuary, Waterside, Mountain, Forest, Miasma, Red_hot, Indoor, Crystal, Banned
    }

    /// <summary>
    /// 地形相性
    /// </summary>
    public enum Affinity
    {
        Bad, Normal, Good, VeryGood
    }

    /// <summary>
    /// 属性
    /// </summary>
    public enum UnitType
    {
        /// <summary>
        /// 力
        /// </summary>
        Power,
        /// <summary>
        /// 護
        /// </summary>
        Guard,
        /// <summary>
        /// 知
        /// </summary>
        Intelligence,
        /// <summary>
        /// 幻
        /// </summary>
        Apparition,
        /// <summary>
        /// 技
        /// </summary>
        Technic,
        /// <summary>
        /// 運
        /// </summary>
        Fortune
    }

    /// <summary>
    /// 敵か味方か
    /// </summary>
    public enum FrendOfFoe
    {
        Ally, Enemy
    }

    /// <summary>
    /// 行動の効果
    /// 0～99:近接 100～199:遠隔 200～:装備
    /// %100 = 0:攻撃
    /// </summary>
    public enum ActType
    {
        Grapple = 0,
        Heal,
        Heal2,
        Revive,
        Revive2,
        AddPlusSympton,
        ClearMinusSympton,
        Guard,
        LessGuard,
        SPUp,
        SetField,
        SearchEnemy,
        Hide,
        UpSpeed,
        UpClose,
        UpFar,
        UpReact,
        UpAttack,
        UpDefense,
        LevelDrain,
        Musoutensei,

        Shot = 100,
        AddMinusSympton,
        AddDoubleSympton,
        ClearPlusSympton,
        Utsusemi,
        Counter,
        BarrierDefense,
        BarrierSpirit,
        SetTrap,
        ClearTrap,
        SPDrain,
        TransSpace,
        ClearParameter,
        TimeStop,
        MindCrash,

        Booster = 201,
        Scope,
        DualBoost,
        Charge,
        MoveAssist
    }

    /// <summary>
    /// 攻撃の特性
    /// </summary>
    public enum ActAbility
    {
        None,
        Fast,
        Rush,
        Hit,
        Penetrate,
        Diffuse,
        Shock,
        Vacuum,
        Geographic,
        Proficient,
        Drain,
        Spirit,
        AntiMinus,
        AntiPlus,
        AntiHuman,
        AntiMonster,
        Summon,
        Repeat,
        Time,
        Assassin,
        Whole,
        Revenge,
        Sacrifice,
        Destroy,
        Sanctio,
        Heat,
        Cold,
        Thunder,
        Laser
    }

    /// <summary>
    /// 行動の対象
    /// </summary>
    public enum ActTarget
    {
        Ally1, AllyAll, Enemy1, EnemyAll, AllyEnemy1, All, Field, Space, Equip
    }

    /// <summary>
    /// プラス症状
    /// </summary>
    public enum SymptonPlus
    {
        None, Heal, Charge, Concentrate, Swift, Musoutensei, ActAgain, Stigmata, Invalid
    }

    /// <summary>
    /// マイナス症状
    /// </summary>
    public enum SymptonMinus
    {
        None, Damage, Distract, Restraint, Stop, Confuse, Deguard, Dedodge, FixInside, FixOutside, CarvedSeal, Invalid, Stigmata
    }

    /// <summary>
    /// トラップ
    /// </summary>
    public enum Trap
    {
        None, GrappleTrap, ShotTrap, AttackTrap, OnceClear, SPPlant, HitPromise, MagicCharge, TrapClear
    }

    /// <summary>
    /// 効果
    /// </summary>
    public enum FieldEffect
    {
        None,
        APUp,
        APDown,
        HealBanned,
        DamageHalf,
        Invalid,

        HPHeal,
        HPDamage,
        HitUp,
        CostUp,
        DamageFix,
        SympInvalid,
        AffinityDown,
        AffinityReverse,
        TimeStop,
        Trinity,

        ChangeTerrain,
    }

    /// <summary>
    /// アーティファクト
    /// </summary>
    public enum Artifact
    {
        None
    }
}
