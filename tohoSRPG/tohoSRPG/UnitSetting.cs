using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace tohoSRPG
{
    static class UnitSetting
    {
        static ContentManager content;

        public static void Init(ContentManager c)
        {
            content = c;
        }

        public static Unit SetUnit(CharaID chara, int level)
        {
            switch (chara)
            {
                case CharaID.Reimu:
                    return SetReimu(level);
                case CharaID.Marisa:
                    return SetMarisa(level);
                case CharaID.Sanae:
                    return SetSanae(level);
                case CharaID.Chrino:
                    return SetChirno(level);
                case CharaID.Oku:
                    return SetOku(level);
                case CharaID.Zero:
                    return SetZero(level);
                case CharaID.Else:
                default:
                    return null;
            }
        }

        static Unit SetReimu(int level)
        {
            Unit unit = new Unit(CharaID.Reimu);
            unit.name = "博麗霊夢";
            unit.nickname = "霊夢";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\reimu"); 
            unit.t_battle_origin = new Vector2(64, 128);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\reimu");

            unit.type = Type.Fortune;
            unit.type2 = Type.Power;

            unit.level = level;

            unit.pHP = 230;

            unit.normalPar.speed = 23;
            unit.normalPar.avoid = 48;
            unit.normalPar.defence = 31;
            unit.normalPar.close = 13;
            unit.normalPar.far = 15;

            unit.drivePar.speed = 40;
            unit.drivePar.avoid = 80;
            unit.drivePar.defence = 68;
            unit.drivePar.close = 56;
            unit.drivePar.far = 61;

            unit.affinity[(int)Terrain.Plain] = Affinity.Good;
            unit.affinity[(int)Terrain.Forest] = Affinity.Good;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Good;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Good;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Good;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Normal;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Good;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Bad;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "博麗戦闘術";
            a.type = ActType.DualBoost;
            a.success = 47;
            a.power = 25;
            a.ap = 0;
            a.target = ActTarget.Equip;
            a.rangeMin = 0;
            a.rangeMax = 0;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "昇天脚";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Fast;
            a.proficiency = 1.5f;
            a.success = 47;
            a.power = 13;
            a.ap = 3;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "パスウェイジョンニードル";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Fast;
            a.proficiency = 1.5f;
            a.success = 21;
            a.power = 23;
            a.ap = 9;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 3;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "封魔陣";
            a.type = ActType.Grapple;
            a.proficiency = 1.5f;
            a.success = 75;
            a.power = 30;
            a.ap = 17;
            a.sp = 60;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "夢想封印";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Hit;
            a.ability2 = ActAbility.Fast;
            a.proficiency = 1.5f;
            a.success = 99;
            a.power = 66;
            a.ap = 24;
            a.sp = 60;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 3;
            a.rangeMax = 6;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "夢想天生";
            a.lastSpell = true;
            a.type = ActType.Musoutensei;
            a.success = 3;
            a.power = 99;
            a.ap = 24;
            a.sp = 100;
            a.target = ActTarget.Ally1;
            a.rangeMin = 0;
            a.rangeMax = 0;
            unit.acts[i++] = a;
            #endregion

            return unit;
        }

        static Unit SetMarisa(int level)
        {
            Unit unit = new Unit(CharaID.Marisa);
            unit.name = "霧雨魔理沙";
            unit.nickname = "魔理沙";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\marisa"); 
            unit.t_battle_origin = new Vector2(64, 110);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\marisa");

            unit.type = Type.Intelligence;
            unit.type2 = Type.Power;

            unit.level = level;

            unit.pHP = 175;

            unit.normalPar.speed = 72;
            unit.normalPar.avoid = 32;
            unit.normalPar.defence = 5;
            unit.normalPar.close = 9;
            unit.normalPar.far = 27;

            unit.drivePar.speed = 87;
            unit.drivePar.avoid = 63;
            unit.drivePar.defence = 46;
            unit.drivePar.close = 34;
            unit.drivePar.far = 78;

            unit.affinity[(int)Terrain.Plain] = Affinity.Good;
            unit.affinity[(int)Terrain.Forest] = Affinity.Best;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Good;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Bad;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Normal;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Good;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Good;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Bad;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "コンセントレート";
            a.type = ActType.SPUp;
            a.success = 11;
            a.power = 18;
            a.ap = 12;
            a.target = ActTarget.Ally1;
            a.rangeMin = 0;
            a.rangeMax = 5;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "メテオニックデブリ";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Diffuse;
            a.success = 67;
            a.power = 13;
            a.ap = 7;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 3;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "イリュージョンレーザー";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Laser;
            a.ability2 = ActAbility.Fast;
            a.success = 12;
            a.power = 70;
            a.ap = 21;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 5;
            a.rangeMax = 7;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "マスタースパーク";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Laser;
            a.ability2 = ActAbility.Penetrate;
            a.proficiency = 2;
            a.success = 28;
            a.power = 119;
            a.ap = 24;
            a.sp = 60;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 5;
            a.rangeMax = 7;
            unit.acts[i++] = a;
            
            a = new Act();
            a.name = "スターダストレヴァリエ";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Diffuse;
            a.success = 24;
            a.power = 33;
            a.ap = 24;
            a.sp = 80;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 5;
            a.rangeMax = 7;
            unit.acts[i++] = a;
            
            a = new Act();
            a.name = "ファイナルスパーク";
            a.lastSpell = true;
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Laser;
            a.ability2 = ActAbility.Penetrate;
            a.proficiency = 2;
            a.success = 37;
            a.power = 182;
            a.ap = 35;
            a.sp = 120;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 3;
            a.rangeMax = 7;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }

        static Unit SetSanae(int level)
        {
            Unit unit = new Unit(CharaID.Sanae);
            unit.name = "東風谷早苗";
            unit.nickname = "早苗";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\sanae");
            unit.t_battle_origin = new Vector2(64, 118);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\sanae");

            unit.type = Type.Guard;
            unit.type2 = Type.Fortune;

            unit.level = level;

            unit.pHP = 185;

            unit.normalPar.speed = 24;
            unit.normalPar.avoid = 22;
            unit.normalPar.defence = 46;
            unit.normalPar.close = 14;
            unit.normalPar.far = 13;

            unit.drivePar.speed = 30;
            unit.drivePar.avoid = 43;
            unit.drivePar.defence = 81;
            unit.drivePar.close = 47;
            unit.drivePar.far = 45;

            unit.affinity[(int)Terrain.Plain] = Affinity.Good;
            unit.affinity[(int)Terrain.Forest] = Affinity.Normal;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Good;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Normal;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Good;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Normal;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Good;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Bad;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "治療の奇跡";
            a.type = ActType.ClearMinusSympton;
            a.success = 17;
            a.power = 42;
            a.ap = 16;
            a.target = ActTarget.AllyEnemy1;
            a.rangeMin = 0;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "回復の奇跡";
            a.type = ActType.Heal;
            a.proficiency = 1.5f;
            a.success = 10;
            a.power = 48;
            a.ap = 15;
            a.target = ActTarget.Ally1;
            a.rangeMin = 0;
            a.rangeMax = 5;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "集中の奇跡";
            a.type = ActType.AddPlusSympton;
            a.sympton = (int)SymptonPlus.Concentrate;
            a.proficiency = 2;
            a.success = 6;
            a.power = 35;
            a.ap = 12;
            a.target = ActTarget.Ally1;
            a.rangeMin = 0;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "払いの儀式";
            a.type = ActType.SetCrystal;
            a.sympton = (int)CrystalEffect.Invalid;
            a.success = 38;
            a.power = 56;
            a.ap = 18;
            a.target = ActTarget.Field;
            a.rangeMin = 0;
            a.rangeMax = 2;
            a.fact = '光';
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ミラクルフルーツ";
            a.type = ActType.Heal;
            a.proficiency = 1.5f;
            a.success = 54;
            a.power = 97;
            a.ap = 20;
            a.sp = 100;
            a.target = ActTarget.AllyAll;
            a.rangeMin = 0;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "グレイソーマタージ";
            a.lastSpell = true;
            a.type = ActType.SetCrystal;
            a.sympton = (int)CrystalEffect.DamageDown;
            a.success = 52;
            a.power = 32;
            a.ap = 20;
            a.sp = 40;
            a.target = ActTarget.Field;
            a.rangeMin = 0;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }

        static Unit SetChirno(int level)
        {
            Unit unit = new Unit(CharaID.Chrino);
            unit.name = "チルノ";
            unit.nickname = "チルノ";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\chirno");
            unit.t_battle_origin = new Vector2(64, 118);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\chirno");

            unit.type = Type.Apparition;
            unit.type2 = Type.Power;

            unit.level = level;

            unit.pHP = 135;

            unit.normalPar.speed = 36;
            unit.normalPar.avoid = 17;
            unit.normalPar.defence = 9;
            unit.normalPar.close = 27;
            unit.normalPar.far = 18;

            unit.drivePar.speed = 42;
            unit.drivePar.avoid = 65;
            unit.drivePar.defence = 23;
            unit.drivePar.close = 51;
            unit.drivePar.far = 28;

            unit.affinity[(int)Terrain.Plain] = Affinity.Good;
            unit.affinity[(int)Terrain.Forest] = Affinity.Normal;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Good;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Good;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Normal;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Bad;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Normal;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Normal;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "アイスフィールド";
            a.type = ActType.SetCrystal;
            a.sympton = (int)CrystalEffect.CostUp;
            a.success = 11;
            a.power = 10;
            a.ap = 18;
            a.target = ActTarget.Field;
            a.rangeMin = 0;
            a.rangeMax = 3;
            a.fact = '氷';
            unit.acts[i++] = a;

            a = new Act();
            a.name = "フリーズタッチミー";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Cold;
            a.ability2 = ActAbility.Fast;
            a.sympton = (int)SymptonMinus.Restraint;
            a.success = 45;
            a.power = 9;
            a.ap = 11;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "リフリジレイション";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Cold;
            a.ability2 = ActAbility.Rush;
            a.sympton = (int)SymptonMinus.Stop;
            a.proficiency = 2;
            a.success = 4;
            a.power = 31;
            a.ap = 15;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "アイシクルフォール";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Diffuse;
            a.success = 56;
            a.power = 27;
            a.ap = 24;
            a.sp = 60;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 2;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ソードフリーザー";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Cold;
            a.ability2 = ActAbility.Fast;
            a.sympton = (int)SymptonMinus.Restraint;
            a.proficiency = 1.5f;
            a.success = 99;
            a.power = 60;
            a.ap = 18;
            a.sp = 50;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 0;
            a.rangeMax = 1;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "パーフェクトフリーズ";
            a.lastSpell = true;
            a.type = ActType.AddMinusSympton;
            a.sympton = (int)SymptonMinus.Stop;
            a.proficiency = 1.5f;
            a.success = 52;
            a.power = 37;
            a.ap = 24;
            a.sp = 80;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }

        static Unit SetOku(int level)
        {
            Unit unit = new Unit(CharaID.Oku);
            unit.name = "霊烏路空";
            unit.nickname = "お空";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\utsuho");
            unit.t_battle_origin = new Vector2(64, 110);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\utsuho");

            unit.type = Type.Power;
            unit.type2 = Type.Intelligence;

            unit.level = level;

            unit.pHP = 260;

            unit.normalPar.speed = 60;
            unit.normalPar.avoid = 21;
            unit.normalPar.defence = 44;
            unit.normalPar.close = 13;
            unit.normalPar.far = 28;

            unit.drivePar.speed = 66;
            unit.drivePar.avoid = 34;
            unit.drivePar.defence = 89;
            unit.drivePar.close = 33;
            unit.drivePar.far = 99;

            unit.affinity[(int)Terrain.Plain] = Affinity.Good;
            unit.affinity[(int)Terrain.Forest] = Affinity.Bad;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Normal;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Bad;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Normal;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Best;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Bad;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Good;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "チャージシュート";
            a.type = ActType.Scope;
            a.success = 52;
            a.power = 0;
            a.ap = 0;
            a.target = ActTarget.Equip;
            a.rangeMin = 0;
            a.rangeMax = 0;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "シューティングスター";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Diffuse;
            a.success = 18;
            a.power = 31;
            a.ap = 13;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 3;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "シューティングサン";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Heat;
            a.ability2 = ActAbility.Fast;
            a.proficiency = 1.5f;
            a.success = 0;
            a.power = 80;
            a.ap = 23;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 4;
            a.rangeMax = 6;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ニュークリアフュージョン";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Heat;
            a.proficiency = 1.5f;
            a.success = 41;
            a.power = 34;
            a.ap = 9;
            a.sp = 80;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 2;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ギガフレア";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Heat;
            a.ability2 = ActAbility.Fast;
            a.proficiency = 1.5f;
            a.success = 0;
            a.power = 123;
            a.ap = 11;
            a.sp = 70;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 5;
            a.rangeMax = 7;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "アビスノヴァ";
            a.lastSpell = true;
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Heat;
            a.ability2 = ActAbility.Vacuum;
            a.proficiency = 1.5f;
            a.success = 35;
            a.power = 86;
            a.ap = 13;
            a.sp = 130;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 2;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }

        static Unit SetZero(int level)
        {
            Unit unit = new Unit(CharaID.Zero);
            unit.name = "ゼロ・スーサイド";
            unit.nickname = "ゼロ";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\zero");
            unit.t_battle_origin = new Vector2(64, 110);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\zero");

            unit.type = Type.Power;
            unit.type2 = Type.Guard;

            unit.level = level;

            unit.pHP = 360;

            unit.normalPar.speed = 23;
            unit.normalPar.avoid = 44;
            unit.normalPar.defence = 44;
            unit.normalPar.close = 21;
            unit.normalPar.far = 7;

            unit.drivePar.speed = 46;
            unit.drivePar.avoid = 66;
            unit.drivePar.defence = 66;
            unit.drivePar.close = 54;
            unit.drivePar.far = 21;

            unit.affinity[(int)Terrain.Plain] = Affinity.Best;
            unit.affinity[(int)Terrain.Forest] = Affinity.Good;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Good;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Bad;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Good;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Normal;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Normal;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Normal;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "メイントライフ";
            a.type = ActType.LevelDrain;
            a.proficiency = 2;
            a.success = 1;
            a.power = 15;
            a.ap = 12;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ライフフォース";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Fast;
            a.proficiency = 1.5f;
            a.success = 5;
            a.power = 47;
            a.ap = 17;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ライフブラッド";
            a.type = ActType.Counter;
            a.proficiency = 0.5f;
            a.success = 1;
            a.power = 41;
            a.ap = 41;
            a.target = ActTarget.AllyAll;
            a.rangeMin = 1;
            a.rangeMax = 5;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "モノパンデミック";
            a.type = ActType.AddMinusSympton;
            a.sympton = (int)SymptonMinus.Distract;
            a.proficiency = 2;
            a.success = 13;
            a.power = 23;
            a.ap = 24;
            a.sp = 70;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "カルジェネレータ";
            a.type = ActType.SetCrystal;
            a.sympton = (int)CrystalEffect.APDown;
            a.proficiency = 0.5f;
            a.success = 0;
            a.power = 10;
            a.ap = 20;
            a.sp = 30;
            a.target = ActTarget.Field;
            a.rangeMin = 0;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ディフォース・ジュピターソード";
            a.lastSpell = true;
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Fast;
            a.proficiency = 1.5f;
            a.success = 99;
            a.power = 74;
            a.ap = 30;
            a.sp = 60;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }

        /*
        static Unit Set(int level)
        {
            Unit unit = new Unit(CharaID.);
            unit.name = "";
            unit.nickname = "";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\");
            unit.t_battle_origin = new Vector2(64, 110);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\");

            unit.type = Type.;
            unit.type2 = Type.;

            unit.level = level;

            unit.pHP = ;

            unit.normalPar.speed = ;
            unit.normalPar.avoid = ;
            unit.normalPar.defence = ;
            unit.normalPar.close = ;
            unit.normalPar.far = ;

            unit.drivePar.speed = ;
            unit.drivePar.avoid = ;
            unit.drivePar.defence = ;
            unit.drivePar.close = ;
            unit.drivePar.far = ;

            unit.affinity[(int)Terrain.Plain] = Affinity.;
            unit.affinity[(int)Terrain.Forest] = Affinity.;
            unit.affinity[(int)Terrain.Mountain] = Affinity.;
            unit.affinity[(int)Terrain.Waterside] = Affinity.;
            unit.affinity[(int)Terrain.Indoor] = Affinity.;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.;
            unit.affinity[(int)Terrain.Miasma] = Affinity.;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "";
            a.type = ActType.;
            a.success = ;
            a.power = ;
            a.ap = ;
            a.target = ActTarget.;
            a.rangeMin = ;
            a.rangeMax = ;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "";
            a.type = ActType.;
            a.success = ;
            a.power = ;
            a.ap = ;
            a.target = ActTarget.;
            a.rangeMin = ;
            a.rangeMax = ;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "";
            a.type = ActType.;
            a.success = ;
            a.power = ;
            a.ap = ;
            a.target = ActTarget.;
            a.rangeMin = ;
            a.rangeMax = ;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "";
            a.type = ActType.;
            a.success = ;
            a.power = ;
            a.ap = ;
            a.sp = ;
            a.target = ActTarget.;
            a.rangeMin = ;
            a.rangeMax = ;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "";
            a.type = ActType.;
            a.success = ;
            a.power = ;
            a.ap = ;
            a.sp = ;
            a.target = ActTarget.;
            a.rangeMin = ;
            a.rangeMax = ;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "";
            a.lastSpell = true;
            a.type = ActType.;
            a.success = ;
            a.power = ;
            a.ap = ;
            a.sp = ;
            a.target = ActTarget.;
            a.rangeMin = ;
            a.rangeMax = ;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }
        */
    }
}
