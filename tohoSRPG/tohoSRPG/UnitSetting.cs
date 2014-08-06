using System;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace tohoSRPG
{
    /// <summary>
    /// ユニットの種類
    /// </summary>
    public enum CharaID
    {
        Reimu, Marisa,
        Rumia, Cirno, Meirin, Patchouli, Sakuya, Remilia, Flandre,
        Letty, Chen, Alice, Lunasa, Marlin, Lyrica, Youmu, Yuyuko, Ran, Yukari,
        Wriggle, Mystia, Keine, Tewi, Udonge, Eirin, Kaguya, Mokou,
        Suika, Aya, Medicine, Yuka, Komachi, Eiki,
        Shizuham, Minoriko, hina, Momizi, Sanae, Kanako, Suwako,
        Iku, Tenshi, Hatate, Kokoro,
        Yamame, Parsee, Yugi, Satori, Rin, Oku, Koishi,
        Nazrin, Kogasa, Ichirin, Murasa, Syou, Hijiri, Nue,
        Kyouko, Yoshika, Seiga, Toziko, Futo, Miko, Mamizou,
        Wakasagi, Sekibanki, Kagerou, Benben, Yatsuhashi, Seija, Shinmyoumaru, Raiko,
        Riml, Zero, Lana,
        Parawee, Balserga, Farraha, Kecak, Karahaiya, Alfes,
        Else
    }

    static class UnitSetting
    {
        static ContentManager content;

        public static void Init(ContentManager c)
        {
            content = c;
        }

        public static Unit SetUnit(CharaID chara, int level)
        {
            string name = "Set" + chara.ToString();
            MethodInfo mi = typeof(UnitSetting).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static);
            return (Unit)mi.Invoke(null, new object[] {level});
        }

        static Unit SetRiml(int level)
        {
            Unit unit = new Unit(CharaID.Riml);
            unit.name = "リムル・ストレイジ";
            unit.nickname = "リムル";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\riml");
            unit.t_battle_origin = new Vector2(128, 250);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\riml");

            unit.type = UnitType.Technic;
            unit.type2 = UnitType.Apparition;

            unit.level = level;

            unit.pHP = 180;

            unit.normalPar.speed = 26;
            unit.normalPar.avoid = 35;
            unit.normalPar.defense = 35;
            unit.normalPar.close = 18;
            unit.normalPar.far = 18;

            unit.drivePar.speed = 55;
            unit.drivePar.avoid = 52;
            unit.drivePar.defense = 52;
            unit.drivePar.close = 44;
            unit.drivePar.far = 44;

            unit.affinity[(int)Terrain.Plain] = Affinity.Normal;
            unit.affinity[(int)Terrain.Forest] = Affinity.Normal;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Normal;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Normal;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Normal;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Normal;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.VeryGood;
            unit.affinity[(int)Terrain.Miasma] = Affinity.VeryGood;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "フォークロア";
            a.type = ActType.MoveAssist;
            a.success = 0;
            a.power = 4;
            a.ap = 0;
            a.target = ActTarget.Equip;
            a.rangeMin = 0;
            a.rangeMax = 0;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "トラディション";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.AntiMinus;
            a.success = 42;
            a.power = 8;
            a.ap = 6;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "レジェンダリー";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.AntiPlus;
            a.success = 42;
            a.power = 8;
            a.ap = 6;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 3;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ハーモナイズ";
            a.type = ActType.SetField;
            a.sympton = (int)FieldEffect.APUp;
            a.success = 46;
            a.power = 12;
            a.count = 1;
            a.target = ActTarget.Field;
            a.rangeMin = 0;
            a.rangeMax = 0;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "キルクイトス";
            a.type = ActType.AddPlusSympton;
            a.sympton = (int)SymptonPlus.ActAgain;
            a.success = 0;
            a.power = 2;
            a.ap = 25;
            a.target = ActTarget.Ally1;
            a.rangeMin = 2;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ルナプレーナ";
            a.lastSpell = true;
            a.type = ActType.AddDoubleSympton;
            a.success = 66;
            a.power = 0;
            a.ap = 24;
            a.sp = 100;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 5;
            unit.acts[i++] = a;

            #endregion

            unit.ability.Add(Ability.Drive);

            return unit;
        }

        static Unit SetReimu(int level)
        {
            Unit unit = new Unit(CharaID.Reimu);
            unit.name = "博麗霊夢";
            unit.nickname = "霊夢";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\reimu"); 
            unit.t_battle_origin = new Vector2(128, 246);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\reimu");

            unit.type = UnitType.Fortune;
            unit.type2 = UnitType.Power;

            unit.level = level;

            unit.pHP = 230;

            unit.normalPar.speed = 23;
            unit.normalPar.avoid = 48;
            unit.normalPar.defense = 31;
            unit.normalPar.close = 13;
            unit.normalPar.far = 15;

            unit.drivePar.speed = 40;
            unit.drivePar.avoid = 80;
            unit.drivePar.defense = 68;
            unit.drivePar.close = 56;
            unit.drivePar.far = 61;

            unit.affinity[(int)Terrain.Plain] = Affinity.VeryGood;
            unit.affinity[(int)Terrain.Forest] = Affinity.Good;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Normal;
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
            a.ability2 = ActAbility.AntiHuman;
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
            a.ability2 = ActAbility.AntiMonster;
            a.success = 36;
            a.power = 23;
            a.ap = 9;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 3;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "封魔陣";
            a.type = ActType.Grapple;
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
            a.success = 1;
            a.power = 199;
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
            unit.t_battle_origin = new Vector2(128, 256);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\marisa");

            unit.type = UnitType.Intelligence;
            unit.type2 = UnitType.Power;

            unit.level = level;

            unit.pHP = 175;

            unit.normalPar.speed = 72;
            unit.normalPar.avoid = 32;
            unit.normalPar.defense = 5;
            unit.normalPar.close = 9;
            unit.normalPar.far = 27;

            unit.drivePar.speed = 87;
            unit.drivePar.avoid = 63;
            unit.drivePar.defense = 46;
            unit.drivePar.close = 34;
            unit.drivePar.far = 78;

            unit.affinity[(int)Terrain.Plain] = Affinity.Good;
            unit.affinity[(int)Terrain.Forest] = Affinity.VeryGood;
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
            a.name = "イリュージョンレーザー";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Laser;
            a.success = 36;
            a.power = 30;
            a.ap = 18;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 4;
            a.rangeMax = 6;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ルミナスレーザー";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Laser;
            a.ability2 = ActAbility.Fast;
            a.success = 30;
            a.power = 42;
            a.ap = 24;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 5;
            a.rangeMax = 7;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "マスタースパーク";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Laser;
            a.ability2 = ActAbility.Penetrate;
            a.success = 43;
            a.power = 75;
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
            a.success = 68;
            a.power = 108;
            a.ap = 30;
            a.sp = 100;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 3;
            a.rangeMax = 7;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }

        static Unit SetCirno(int level)
        {
            Unit unit = new Unit(CharaID.Cirno);
            unit.name = "チルノ";
            unit.nickname = "チルノ";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\cirno");
            unit.t_battle_origin = new Vector2(144, 228);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\cirno");

            unit.type = UnitType.Apparition;
            unit.type2 = UnitType.Power;

            unit.level = level;

            unit.pHP = 135;

            unit.normalPar.speed = 36;
            unit.normalPar.avoid = 17;
            unit.normalPar.defense = 9;
            unit.normalPar.close = 27;
            unit.normalPar.far = 18;

            unit.drivePar.speed = 42;
            unit.drivePar.avoid = 65;
            unit.drivePar.defense = 23;
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
            a.type = ActType.SetField;
            a.sympton = (int)FieldEffect.CostUp;
            a.success = 11;
            a.power = 7;
            a.count = 2;
            a.target = ActTarget.Field;
            a.rangeMin = 0;
            a.rangeMax = 0;
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
            a.success = 35;
            a.power = 28;
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

        static Unit SetMeirin(int level)
        {
            Unit unit = new Unit(CharaID.Meirin);
            unit.name = "紅美鈴";
            unit.nickname = "美鈴";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\meirin");
            unit.t_battle_origin = new Vector2(116, 252);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\meirin");

            unit.type = UnitType.Guard;
            unit.type2 = UnitType.Technic;

            unit.level = level;

            unit.pHP = 300;

            unit.normalPar.speed = 22;
            unit.normalPar.avoid = 13;
            unit.normalPar.defense = 58;
            unit.normalPar.close = 18;
            unit.normalPar.far = 7;

            unit.drivePar.speed = 36;
            unit.drivePar.avoid = 23;
            unit.drivePar.defense = 99;
            unit.drivePar.close = 66;
            unit.drivePar.far = 23;

            unit.affinity[(int)Terrain.Plain] = Affinity.Good;
            unit.affinity[(int)Terrain.Forest] = Affinity.Normal;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Normal;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Bad;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Good;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Normal;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Bad;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Good;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "天龍脚";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Fast;
            a.success = 30;
            a.power = 14;
            a.ap = 9;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "鉄壁の陣";
            a.type = ActType.Guard;
            a.success = 7;
            a.power = 29;
            a.ap = 12;
            a.target = ActTarget.AllyAll;
            a.rangeMin = 2;
            a.rangeMax = 6;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "練虹気";
            a.type = ActType.Heal;
            a.success = 15;
            a.power = 41;
            a.ap = 13;
            a.target = ActTarget.Ally1;
            a.rangeMin = 0;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "見切りの構え";
            a.type = ActType.Counter;
            a.success = 7;
            a.power = 72;
            a.ap = 23;
            a.target = ActTarget.Ally1;
            a.rangeMin = 0;
            a.rangeMax = 0;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "彩光蓮華掌";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Fast;
            a.ability2 = ActAbility.Proficient;
            a.success = 78;
            a.power = 45;
            a.ap = 19;
            a.sp = 50;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 1;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "極光天山";
            a.lastSpell = true;
            a.type = ActType.LessGuard;
            a.success = 21;
            a.power = 198;
            a.ap = 24;
            a.sp = 60;
            a.target = ActTarget.AllyAll;
            a.rangeMin = 1;
            a.rangeMax = 5;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }

        static Unit SetSakuya(int level)
        {
            Unit unit = new Unit(CharaID.Sakuya);
            unit.name = "十六夜咲夜";
            unit.nickname = "咲夜";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\sakuya");
            unit.t_battle_origin = new Vector2(118, 250);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\sakuya");

            unit.type = UnitType.Technic;
            unit.type2 = UnitType.Apparition;

            unit.level = level;

            unit.pHP = 165;

            unit.normalPar.speed = 36;
            unit.normalPar.avoid = 52;
            unit.normalPar.defense = 33;
            unit.normalPar.close = 10;
            unit.normalPar.far = 15;

            unit.drivePar.speed = 53;
            unit.drivePar.avoid = 74;
            unit.drivePar.defense = 38;
            unit.drivePar.close = 36;
            unit.drivePar.far = 47;

            unit.affinity[(int)Terrain.Plain] = Affinity.Good;
            unit.affinity[(int)Terrain.Forest] = Affinity.Good;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Good;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Normal;
            unit.affinity[(int)Terrain.Indoor] = Affinity.VeryGood;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Normal;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Good;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Bad;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "メイドマジック";
            a.type = ActType.SetTrap;
            a.sympton = (int)Trap.GrappleTrap;
            a.success = 45;
            a.power = 40;
            a.count = 3;
            a.target = ActTarget.Ally1;
            a.rangeMin = 0;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ミスディレクション";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Diffuse;
            a.success = 68;
            a.power = 5;
            a.ap = 2;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 2;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "マジックスターソード";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Fast;
            a.success = 38;
            a.power = 21;
            a.ap = 5;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 3;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "殺人ドール";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Fast;
            a.success = 75;
            a.power = 56;
            a.ap = 20;
            a.sp = 50;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 2;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ソウルスカルプチュア";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Assassin;
            a.sympton = (int)SymptonMinus.Damage;
            a.success = 99;
            a.power = 78;
            a.ap = 20;
            a.sp = 80;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "咲夜の世界";
            a.lastSpell = true;
            a.type = ActType.TimeStop;
            a.success = 2;
            a.power = 12;
            a.ap = 0;
            a.sp = 100;
            a.target = ActTarget.Field;
            a.rangeMin = 0;
            a.rangeMax = 0;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }

        static Unit SetRemilia(int level)
        {
            Unit unit = new Unit(CharaID.Remilia);
            unit.name = "レミリア・スカーレット";
            unit.nickname = "レミリア";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\remilia");
            unit.t_battle_origin = new Vector2(110, 240);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\remilia");

            unit.type = UnitType.Power;
            unit.type2 = UnitType.Technic;

            unit.level = level;

            unit.pHP = 275;

            unit.normalPar.speed = 66;
            unit.normalPar.avoid = 45;
            unit.normalPar.defense = 36;
            unit.normalPar.close = 25;
            unit.normalPar.far = 17;

            unit.drivePar.speed = 83;
            unit.drivePar.avoid = 78;
            unit.drivePar.defense = 72;
            unit.drivePar.close = 44;
            unit.drivePar.far = 41;

            unit.affinity[(int)Terrain.Plain] = Affinity.Normal;
            unit.affinity[(int)Terrain.Forest] = Affinity.Normal;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Normal;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Bad;
            unit.affinity[(int)Terrain.Indoor] = Affinity.VeryGood;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Good;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Bad;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Good;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "デーモンロードウォーク";
            a.type = ActType.Booster;
            a.success = 0;
            a.power = 27;
            a.ap = 0;
            a.target = ActTarget.Equip;
            a.rangeMin = 0;
            a.rangeMax = 0;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ブラッディナイフ";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Fast;
            a.ability2 = ActAbility.Drain;
            a.success = 46;
            a.power = 13;
            a.ap = 10;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "デーモンヘッドハント";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Rush;
            a.ability2 = ActAbility.Drain;
            a.success = 4;
            a.power = 36;
            a.ap = 15;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "不夜城レッド";
            a.type = ActType.AddMinusSympton;
            a.sympton = (int)SymptonMinus.Damage;
            a.success = 31;
            a.power = 44;
            a.ap = 20;
            a.sp = 70;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "スピア・ザ・グングニル";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Fast;
            a.ability2 = ActAbility.Penetrate;
            a.sympton = (int)SymptonMinus.Stop;
            a.success = 99;
            a.power = 13;
            a.ap = 24;
            a.sp = 60;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 6;
            a.rangeMax = 7;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "レッドマジック";
            a.lastSpell = true;
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Drain;
            a.success = 83;
            a.power = 37;
            a.ap = 24;
            a.sp = 80;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }
        
        static Unit SetFlandre(int level)
        {
            Unit unit = new Unit(CharaID.Flandre);
            unit.name = "フランドール・S";
            unit.nickname = "フランドール";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\flandre");
            unit.t_battle_origin = new Vector2(128, 256);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\flandre");

            unit.type = UnitType.Power;
            unit.type2 = UnitType.Fortune;

            unit.level = level;

            unit.pHP = 250;

            unit.normalPar.speed = 62;
            unit.normalPar.avoid = 74;
            unit.normalPar.defense = 0;
            unit.normalPar.close = 21;
            unit.normalPar.far = 29;

            unit.drivePar.speed = 80;
            unit.drivePar.avoid = 97;
            unit.drivePar.defense = 2;
            unit.drivePar.close = 43;
            unit.drivePar.far = 51;

            unit.affinity[(int)Terrain.Plain] = Affinity.Normal;
            unit.affinity[(int)Terrain.Forest] = Affinity.Bad;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Normal;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Bad;
            unit.affinity[(int)Terrain.Indoor] = Affinity.VeryGood;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.VeryGood;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Bad;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Good;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "ディストラクション";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Destroy;
            a.ability2 = ActAbility.Rush;
            a.success = 0;
            a.power = 0;
            a.count = 2;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 1;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ガードインターセプト";
            a.type = ActType.Shot;
            a.sympton = (int)SymptonMinus.Deguard;
            a.success = 9;
            a.power = 65;
            a.ap = 22;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 3;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ドッヂインターセプト";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Fast;
            a.sympton = (int)SymptonMinus.Dedodge;
            a.success = 1;
            a.power = 80;
            a.ap = 24;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 4;
            a.rangeMax = 5;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "レーヴァテイン";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Heat;
            a.ability2 = ActAbility.Rush;
            a.success = 9;
            a.power = 98;
            a.ap = 20;
            a.sp = 50;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 1;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "スターボウブレイク";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Laser;
            a.ability2 = ActAbility.Diffuse;
            a.success = 18;
            a.power = 64;
            a.ap = 24;
            a.sp = 80;
            a.target = ActTarget.All;
            a.rangeMin = 3;
            a.rangeMax = 5;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "そして誰もいなくなるか？";
            a.lastSpell = true;
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Destroy;
            a.success = 0;
            a.power = 0;
            a.ap = 30;
            a.sp = 120;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 9;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }

        static Unit SetYoumu(int level)
        {
            Unit unit = new Unit(CharaID.Youmu);
            unit.name = "魂魄妖夢";
            unit.nickname = "妖夢";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\youmu");
            unit.t_battle_origin = new Vector2(120, 238);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\youmu");

            unit.type = UnitType.Technic;
            unit.type2 = UnitType.Power;

            unit.level = level;

            unit.pHP = 205;

            unit.normalPar.speed = 34;
            unit.normalPar.avoid = 43;
            unit.normalPar.defense = 21;
            unit.normalPar.close = 38;
            unit.normalPar.far = 0;

            unit.drivePar.speed = 55;
            unit.drivePar.avoid = 57;
            unit.drivePar.defense = 25;
            unit.drivePar.close = 94;
            unit.drivePar.far = 0;

            unit.affinity[(int)Terrain.Plain] = Affinity.VeryGood;
            unit.affinity[(int)Terrain.Forest] = Affinity.Good;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Good;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Bad;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Good;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Bad;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Normal;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Normal;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "魂魄歩行術";
            a.type = ActType.Booster;
            a.success = 0;
            a.power = 26;
            a.ap = 0;
            a.target = ActTarget.Equip;
            a.rangeMin = 0;
            a.rangeMax = 0;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "弦月斬";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Fast;
            a.success = 57;
            a.power = 15;
            a.ap = 3;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 1;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "生死流転斬";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Rush;
            a.success = 22;
            a.power = 33;
            a.ap = 5;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 1;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "現世斬";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Fast;
            a.success = 75;
            a.power = 60;
            a.ap = 18;
            a.sp = 50;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "幽明求聞持聡明の法";
            a.type = ActType.AddPlusSympton;
            a.sympton = (int)SymptonPlus.ActAgain;
            a.success = 0;
            a.power = 2;
            a.ap = 0;
            a.sp = 30;
            a.target = ActTarget.Ally1;
            a.rangeMin = 0;
            a.rangeMax = 0;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "未来永劫斬";
            a.lastSpell = true;
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Fast;
            a.ability2 = ActAbility.Rush;
            a.success = 99;
            a.power = 99;
            a.ap = 24;
            a.sp = 70;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 1;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }

        static Unit SetUdonge(int level)
        {
            Unit unit = new Unit(CharaID.Udonge);
            unit.name = "鈴仙・U・イナバ";
            unit.nickname = "鈴仙";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\udonge");
            unit.t_battle_origin = new Vector2(116, 252);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\udonge");

            unit.type = UnitType.Apparition;
            unit.type2 = UnitType.Technic;

            unit.level = level;

            unit.pHP = 180;

            unit.normalPar.speed = 24;
            unit.normalPar.avoid = 25;
            unit.normalPar.defense = 31;
            unit.normalPar.close = 3;
            unit.normalPar.far = 17;

            unit.drivePar.speed = 33;
            unit.drivePar.avoid = 49;
            unit.drivePar.defense = 55;
            unit.drivePar.close = 12;
            unit.drivePar.far = 73;

            unit.affinity[(int)Terrain.Plain] = Affinity.Normal;
            unit.affinity[(int)Terrain.Forest] = Affinity.VeryGood;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Good;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Bad;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Good;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Normal;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Bad;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Good;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "ビジョナリチューニング";
            a.type = ActType.TransSpace;
            a.success = 11;
            a.power = 0;
            a.ap = 6;
            a.target = ActTarget.Space;
            a.rangeMin = 1;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "マインドシェイカー";
            a.type = ActType.AddMinusSympton;
            a.sympton = (int)SymptonMinus.Confuse;
            a.success = 11;
            a.power = 0;
            a.ap = 16;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 2;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "マインドエクスプロージョン";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.AntiMinus;
            a.ability2 = ActAbility.Fast;
            a.success = 63;
            a.power = 17;
            a.ap = 16;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 3;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "近眼花火(マインドスターマイン)";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.AntiMinus;
            a.success = 95;
            a.power = 23;
            a.ap = 19;
            a.sp = 60;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 2;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "月兎遠隔催眠術(テレメスメリズム)";
            a.type = ActType.AddMinusSympton;
            a.sympton = (int)SymptonMinus.Confuse;
            a.success = 40;
            a.power = 0;
            a.ap = 24;
            a.sp = 80;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "幻朧月睨(ルナティックレッドアイズ)";
            a.lastSpell = true;
            a.type = ActType.Shot;
            a.ability1 = ActAbility.AntiMinus;
            a.ability2 = ActAbility.Fast;
            a.success = 82;
            a.power = 57;
            a.ap = 24;
            a.sp = 70;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }

        static Unit SetAya(int level)
        {
            Unit unit = new Unit(CharaID.Aya);
            unit.name = "射命丸文";
            unit.nickname = "文";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\aya");
            unit.t_battle_origin = new Vector2(72, 248);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\aya");

            unit.type = UnitType.Technic;
            unit.type2 = UnitType.Fortune;

            unit.level = level;

            unit.pHP = 195;

            unit.normalPar.speed = 86;
            unit.normalPar.avoid = 63;
            unit.normalPar.defense = 13;
            unit.normalPar.close = 12;
            unit.normalPar.far = 13;

            unit.drivePar.speed = 99;
            unit.drivePar.avoid = 99;
            unit.drivePar.defense = 27;
            unit.drivePar.close = 35;
            unit.drivePar.far = 41;

            unit.affinity[(int)Terrain.Plain] = Affinity.Good;
            unit.affinity[(int)Terrain.Forest] = Affinity.Bad;
            unit.affinity[(int)Terrain.Mountain] = Affinity.VeryGood;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Normal;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Bad;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Normal;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Bad;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Good;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "猿田彦の先導";
            a.type = ActType.AddPlusSympton;
            a.sympton = (int)SymptonPlus.Swift;
            a.success = 6;
            a.power = 12;
            a.ap = 15;
            a.target = ActTarget.Ally1;
            a.rangeMin = 0;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "疾風扇";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Shock;
            a.success = 48;
            a.power = 12;
            a.ap = 5;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 2;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "烈風扇";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Shock;
            a.ability2 = ActAbility.Fast;
            a.success = 21;
            a.power = 24;
            a.ap = 12;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 3;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "文々。速報";
            a.type = ActType.SetTrap;
            a.sympton = (int)Trap.HitPromise;
            a.success = 11;
            a.power = 0;
            a.ap = 13;
            a.target = ActTarget.Ally1;
            a.rangeMin = 0;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "紅葉扇風";
            a.type = ActType.Shot;
            a.success = 99;
            a.power = 36;
            a.ap = 24;
            a.sp = 60;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "天孫降臨の道しるべ";
            a.lastSpell = true;
            a.type = ActType.Shot;
            a.ability2 = ActAbility.Fast;
            a.success = 87;
            a.power = 53;
            a.ap = 24;
            a.sp = 80;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 3;
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
            unit.t_battle_origin = new Vector2(128, 242);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\sanae");

            unit.type = UnitType.Guard;
            unit.type2 = UnitType.Fortune;

            unit.level = level;

            unit.pHP = 185;

            unit.normalPar.speed = 24;
            unit.normalPar.avoid = 22;
            unit.normalPar.defense = 46;
            unit.normalPar.close = 14;
            unit.normalPar.far = 13;

            unit.drivePar.speed = 30;
            unit.drivePar.avoid = 43;
            unit.drivePar.defense = 81;
            unit.drivePar.close = 47;
            unit.drivePar.far = 45;

            unit.affinity[(int)Terrain.Plain] = Affinity.Good;
            unit.affinity[(int)Terrain.Forest] = Affinity.Normal;
            unit.affinity[(int)Terrain.Mountain] = Affinity.VeryGood;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Normal;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Good;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Bad;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Good;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Bad;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "集中の奇跡";
            a.type = ActType.SearchEnemy;
            a.success = 38;
            a.power = 37;
            a.count = 4;
            a.target = ActTarget.AllyAll;
            a.rangeMin = 0;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "回復の奇跡";
            a.type = ActType.Heal;
            a.success = 38;
            a.power = 36;
            a.ap = 15;
            a.target = ActTarget.Ally1;
            a.rangeMin = 0;
            a.rangeMax = 5;
            unit.acts[i++] = a;

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
            a.name = "払いの儀式";
            a.type = ActType.SetField;
            a.sympton = (int)FieldEffect.SympInvalid;
            a.success = 12;
            a.power = 0;
            a.ap = 18;
            a.target = ActTarget.Field;
            a.rangeMin = 0;
            a.rangeMax = 0;
            a.fact = '光';
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ミラクルフルーツ";
            a.type = ActType.Heal2;
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
            a.type = ActType.SetField;
            a.sympton = (int)FieldEffect.DamageFix;
            a.success = 52;
            a.power = 60;
            a.ap = 20;
            a.sp = 40;
            a.target = ActTarget.Field;
            a.rangeMin = 0;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }

        static Unit SetRin(int level)
        {
            Unit unit = new Unit(CharaID.Rin);
            unit.name = "火焔猫燐";
            unit.nickname = "お燐";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\rin");
            unit.t_battle_origin = new Vector2(134, 252);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\rin");

            unit.type = UnitType.Apparition;
            unit.type2 = UnitType.Guard;

            unit.level = level;

            unit.pHP = 190;

            unit.normalPar.speed = 36;
            unit.normalPar.avoid = 22;
            unit.normalPar.defense = 36;
            unit.normalPar.close = 13;
            unit.normalPar.far = 12;

            unit.drivePar.speed = 63;
            unit.drivePar.avoid = 46;
            unit.drivePar.defense = 78;
            unit.drivePar.close = 36;
            unit.drivePar.far = 33;

            unit.affinity[(int)Terrain.Plain] = Affinity.Good;
            unit.affinity[(int)Terrain.Forest] = Affinity.Bad;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Normal;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Bad;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Good;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.VeryGood;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Bad;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Good;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "リビングデッド";
            a.type = ActType.Revive;
            a.success = 11;
            a.power = 39;
            a.count = 4;
            a.target = ActTarget.Ally1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ゾンビバーニング";
            a.type = ActType.AddPlusSympton;
            a.sympton = (int)SymptonPlus.Charge;
            a.success = 5;
            a.power = 4;
            a.ap = 9;
            a.target = ActTarget.Ally1;
            a.rangeMin = 0;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ゾンビスピリット";
            a.type = ActType.SetTrap;
            a.sympton = (int)Trap.SPPlant;
            a.success = 12;
            a.power = 25;
            a.ap = 14;
            a.target = ActTarget.Ally1;
            a.rangeMin = 0;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ゾンビフェアリー";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Summon;
            a.success = 73;
            a.power = 35;
            a.ap = 16;
            a.sp = 40;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 2;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "スプリーンイーター";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Drain;
            a.success = 20;
            a.power = 55;
            a.ap = 20;
            a.sp = 40;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "死灰復燃";
            a.lastSpell = true;
            a.type = ActType.Revive2;
            a.success = 99;
            a.power = 67;
            a.ap = 30;
            a.sp = 120;
            a.target = ActTarget.AllyAll;
            a.rangeMin = 0;
            a.rangeMax = 4;
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
            unit.t_battle_origin = new Vector2(148, 244);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\utsuho");

            unit.type = UnitType.Power;
            unit.type2 = UnitType.Intelligence;

            unit.level = level;

            unit.pHP = 260;

            unit.normalPar.speed = 60;
            unit.normalPar.avoid = 21;
            unit.normalPar.defense = 44;
            unit.normalPar.close = 13;
            unit.normalPar.far = 28;

            unit.drivePar.speed = 66;
            unit.drivePar.avoid = 34;
            unit.drivePar.defense = 89;
            unit.drivePar.close = 33;
            unit.drivePar.far = 99;

            unit.affinity[(int)Terrain.Plain] = Affinity.Good;
            unit.affinity[(int)Terrain.Forest] = Affinity.Bad;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Normal;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Bad;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Normal;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.VeryGood;
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
            a.success = 31;
            a.power = 18;
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
            a.success = 0;
            a.power = 57;
            a.ap = 23;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 4;
            a.rangeMax = 6;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ニュークリアフュージョン";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Heat;
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
            a.success = 0;
            a.power = 86;
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
            a.ability2 = ActAbility.Whole;
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

        static Unit SetIchirin(int level)
        {
            Unit unit = new Unit(CharaID.Ichirin);
            unit.name = "雲居一輪";
            unit.nickname = "一輪";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\ichirin");
            unit.t_battle_origin = new Vector2(108, 250);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\ichirin");

            unit.type = UnitType.Power;
            unit.type2 = UnitType.Guard;

            unit.level = level;

            unit.pHP = 260;

            unit.normalPar.speed = 13;
            unit.normalPar.avoid = 18;
            unit.normalPar.defense = 58;
            unit.normalPar.close = 24;
            unit.normalPar.far = 7;

            unit.drivePar.speed = 31;
            unit.drivePar.avoid = 34;
            unit.drivePar.defense = 89;
            unit.drivePar.close = 83;
            unit.drivePar.far = 22;

            unit.affinity[(int)Terrain.Plain] = Affinity.VeryGood;
            unit.affinity[(int)Terrain.Forest] = Affinity.Good;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Good;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Bad;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Good;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Bad;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Normal;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Good;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "流流する大雲";
            a.type = ActType.Booster;
            a.success = 0;
            a.power = 32;
            a.ap = 0;
            a.target = ActTarget.Equip;
            a.rangeMin = 0;
            a.rangeMax = 0;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "怒りの鉄拳";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Fast;
            a.ability2 = ActAbility.Shock;
            a.success = 48;
            a.power = 12;
            a.ap = 4;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "制裁の鉄拳";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Rush;
            a.ability2 = ActAbility.Shock;
            a.success = 13;
            a.power = 34;
            a.ap = 8;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "げんこつスマッシュ";
            a.type = ActType.Grapple;
            a.success = 99;
            a.power = 44;
            a.ap = 20;
            a.sp = 40;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "仏罰の野分雲";
            a.type = ActType.Grapple;
            a.success = 64;
            a.power = 38;
            a.ap = 26;
            a.sp = 60;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "憤激の天津風ラッシュ";
            a.lastSpell = true;
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Revenge;
            a.success = 31;
            a.power = 30;
            a.ap = 24;
            a.sp = 80;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            #endregion

            return unit;
        }

        static Unit SetFuto(int level)
        {
            Unit unit = new Unit(CharaID.Futo);
            unit.name = "物部布都";
            unit.nickname = "布都";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\futo");
            unit.t_battle_origin = new Vector2(142, 252);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\futo");

            unit.type = UnitType.Fortune;
            unit.type2 = UnitType.Intelligence;

            unit.level = level;

            unit.pHP = 170;

            unit.normalPar.speed = 24;
            unit.normalPar.avoid = 27;
            unit.normalPar.defense = 18;
            unit.normalPar.close = 17;
            unit.normalPar.far = 9;

            unit.drivePar.speed = 28;
            unit.drivePar.avoid = 56;
            unit.drivePar.defense = 48;
            unit.drivePar.close = 74;
            unit.drivePar.far = 29;

            unit.affinity[(int)Terrain.Plain] = Affinity.Good;
            unit.affinity[(int)Terrain.Forest] = Affinity.Normal;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Normal;
            unit.affinity[(int)Terrain.Waterside] = Affinity.VeryGood;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Good;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Bad;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Good;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Bad;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "地脈抑制";
            a.type = ActType.SetField;
            a.sympton = (int)FieldEffect.DamageHalf;
            a.success = 43;
            a.power = 0;
            a.count = 6;
            a.target = ActTarget.Field;
            a.rangeMin = 0;
            a.rangeMax = 0;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "立向坐山";
            a.type = ActType.BarrierDefense;
            a.success = 42;
            a.power = 18;
            a.ap = 16;
            a.target = ActTarget.AllyAll;
            a.rangeMin = 1;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "六壬神課";
            a.type = ActType.BarrierSpirit;
            a.success = 12;
            a.power = 36;
            a.ap = 16;
            a.target = ActTarget.AllyAll;
            a.rangeMin = 1;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "太乙真火";
            a.type = ActType.SetField;
            a.sympton = (int)FieldEffect.HPDamage;
            a.success = 58;
            a.power = 22;
            a.ap = 18;
            a.sp = 30;
            a.target = ActTarget.Field;
            a.rangeMin = 0;
            a.rangeMax = 0;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "三合火局";
            a.type = ActType.SetField;
            a.sympton = (int)FieldEffect.HitUp;
            a.success = 34;
            a.power = 39;
            a.ap = 24;
            a.sp = 40;
            a.target = ActTarget.Field;
            a.rangeMin = 0;
            a.rangeMax = 0;
            unit.acts[i++] = a;

            //a = new Act();
            //a.name = "";
            //a.lastSpell = true;
            //a.type = ActType.;
            //a.success = ;
            //a.power = ;
            //a.ap = ;
            //a.sp = ;
            //a.target = ActTarget.;
            //a.rangeMin = ;
            //a.rangeMax = ;
            //unit.acts[i++] = a;

            #endregion

            return unit;
        }

        static Unit SetKokoro(int level)
        {
            Unit unit = new Unit(CharaID.Kokoro);
            unit.name = "秦こころ";
            unit.nickname = "こころ";
            unit.t_battle = content.Load<Texture2D>("img\\battle\\kokoro");
            unit.t_battle_origin = new Vector2(100, 252);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\kokoro");

            unit.type = UnitType.Apparition;
            unit.type2 = UnitType.Fortune;

            unit.level = level;

            unit.pHP = 190;

            unit.normalPar.speed = 24;
            unit.normalPar.avoid = 20;
            unit.normalPar.defense = 20;
            unit.normalPar.close = 12;
            unit.normalPar.far = 12;

            unit.drivePar.speed = 66;
            unit.drivePar.avoid = 66;
            unit.drivePar.defense = 66;
            unit.drivePar.close = 66;
            unit.drivePar.far = 66;

            unit.affinity[(int)Terrain.Plain] = Affinity.Normal;
            unit.affinity[(int)Terrain.Forest] = Affinity.Normal;
            unit.affinity[(int)Terrain.Mountain] = Affinity.Normal;
            unit.affinity[(int)Terrain.Waterside] = Affinity.Normal;
            unit.affinity[(int)Terrain.Indoor] = Affinity.Normal;
            unit.affinity[(int)Terrain.Red_hot] = Affinity.Normal;
            unit.affinity[(int)Terrain.Sanctuary] = Affinity.Bad;
            unit.affinity[(int)Terrain.Miasma] = Affinity.Good;

            #region 行動
            Act a;
            int i = 0;

            a = new Act();
            a.name = "火男奇体舞";
            a.type = ActType.Utsusemi;
            a.success = 37;
            a.power = 36;
            a.ap = 18;
            a.target = ActTarget.AllyAll;
            a.rangeMin = 2;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "妖狐猛襲舞";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Rush;
            a.success = 19;
            a.power = 46;
            a.ap = 14;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 2;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "鬼婆憂虞舞";
            a.type = ActType.Shot;
            a.ability1 = ActAbility.Spirit;
            a.success = 61;
            a.power = 13;
            a.ap = 9;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 3;
            a.rangeMax = 4;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "怒れる忌狼の面";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Rush;
            a.success = 24;
            a.power = 74;
            a.ap = 20;
            a.sp = 50;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "喜怒哀楽ポゼッション";
            a.type = ActType.AddMinusSympton;
            a.sympton = (int)SymptonMinus.Deguard;
            a.success = 42;
            a.power = 24;
            a.ap = 24;
            a.sp = 90;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 2;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "仮面喪心舞 暗黒能楽";
            a.lastSpell = true;
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Hit;
            a.ability2 = ActAbility.Penetrate;
            a.success = 73;
            a.power = 122;
            a.ap = 24;
            a.sp = 90;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 2;
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
            unit.t_battle_origin = new Vector2(104, 228);
            unit.t_icon = content.Load<Texture2D>("img\\icon\\zero");

            unit.type = UnitType.Power;
            unit.type2 = UnitType.Guard;

            unit.level = level;

            unit.pHP = 360;

            unit.normalPar.speed = 23;
            unit.normalPar.avoid = 44;
            unit.normalPar.defense = 44;
            unit.normalPar.close = 21;
            unit.normalPar.far = 7;

            unit.drivePar.speed = 46;
            unit.drivePar.avoid = 66;
            unit.drivePar.defense = 66;
            unit.drivePar.close = 54;
            unit.drivePar.far = 21;

            unit.affinity[(int)Terrain.Plain] = Affinity.VeryGood;
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
            a.success = 1;
            a.power = 15;
            a.count = 5;
            a.target = ActTarget.Enemy1;
            a.rangeMin = 1;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ライフフォース";
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Fast;
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
            a.success = 1;
            a.power = 41;
            a.ap = 41;
            a.target = ActTarget.AllyAll;
            a.rangeMin = 1;
            a.rangeMax = 5;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ライフレス";
            a.type = ActType.AddMinusSympton;
            a.sympton = (int)SymptonMinus.Distract;
            a.success = 13;
            a.power = 23;
            a.ap = 24;
            a.sp = 70;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 3;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ラージアズライフ";
            a.type = ActType.LevelDrain;
            a.success = 1;
            a.power = 15;
            a.ap = 24;
            a.sp = 60;
            a.target = ActTarget.EnemyAll;
            a.rangeMin = 1;
            a.rangeMax = 6;
            unit.acts[i++] = a;

            a = new Act();
            a.name = "ブレスオブライフ";
            a.lastSpell = true;
            a.type = ActType.Grapple;
            a.ability1 = ActAbility.Fast;
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

            unit.type = UnitType.;
            unit.type2 = UnitType.;

            unit.level = level;

            unit.pHP = ;

            unit.normalPar.speed = ;
            unit.normalPar.avoid = ;
            unit.normalPar.defense = ;
            unit.normalPar.close = ;
            unit.normalPar.far = ;

            unit.drivePar.speed = ;
            unit.drivePar.avoid = ;
            unit.drivePar.defense = ;
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
