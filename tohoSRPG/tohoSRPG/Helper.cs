using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace tohoSRPG
{
    static class Helper
    {
        static SpriteBatch spriteBatch;
        static SpriteFont font;
        static Texture2D tw;

        public static void Init(SpriteBatch sb, SpriteFont f)
        {
            spriteBatch = sb;
            font = f;
            tw = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            Color[] c = { Color.White };
            tw.SetData(c);
        }

        /// <summary>
        /// ウィンドウを描画する
        /// </summary>
        /// <param name="rect">描画範囲の矩形</param>
        public static void DrawWindow(Rectangle rect)
        {
            spriteBatch.Draw(tw, rect, new Color(16, 40, 16));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 3, rect.Y + 3, 6, 6), new Color(192, 192, 192));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 6, rect.Y + 6, 3, 3), Color.White);
            spriteBatch.Draw(tw, new Rectangle(rect.X + 12, rect.Y + 3, rect.Width - 15, 6), new Color(216, 40, 40));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 15, rect.Y + 6, rect.Width - 18, 3), new Color(248, 128, 128));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 3, rect.Y + 12, rect.Width - 6, rect.Height - 24), new Color(80, 184, 80));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 6, rect.Y + 15, rect.Width - 9, rect.Height - 27), new Color(144, 248, 144));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 3, rect.Y + rect.Height - 9, rect.Width - 6, 6), new Color(216, 40, 40));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 6, rect.Y + rect.Height - 6, rect.Width - 9, 3), new Color(248, 128, 128));
        }

        /// <summary>
        /// 暗いウィンドウを描画する
        /// </summary>
        /// <param name="rect">描画範囲の矩形</param>
        public static void DrawWindowDrak(Rectangle rect)
        {
            spriteBatch.Draw(tw, rect, new Color(16, 40, 16));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 3, rect.Y + 3, 6, 6), new Color(192, 192, 192));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 6, rect.Y + 6, 3, 3), Color.White);
            spriteBatch.Draw(tw, new Rectangle(rect.X + 12, rect.Y + 3, rect.Width - 15, 6), new Color(216, 40, 40));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 15, rect.Y + 6, rect.Width - 18, 3), new Color(248, 128, 128));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 3, rect.Y + 12, rect.Width - 6, rect.Height - 24), new Color(64, 128, 64));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 6, rect.Y + 15, rect.Width - 9, rect.Height - 27), new Color(80, 184, 80));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 3, rect.Y + rect.Height - 9, rect.Width - 6, 6), new Color(216, 40, 40));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 6, rect.Y + rect.Height - 6, rect.Width - 9, 3), new Color(248, 128, 128));
        }
        
        /// <summary>
        /// 1段のメッセージウィンドウを描画する
        /// </summary>
        /// <param name="str">描画するメッセージ</param>
        public static void DrawWindowBottom1(string str)
        {
            Rectangle rect = new Rectangle(0, 576, 1024, 64);
            spriteBatch.Draw(tw, rect, new Color(8, 8, 8));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 4, rect.Y + 4, 4, 4), Color.White);
            spriteBatch.Draw(tw, new Rectangle(rect.X + 12, rect.Y + 4, rect.Width - 16, 4), new Color(184, 0, 0));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 4, rect.Y + 12, rect.Width - 8, rect.Height - 16), new Color(0, 184, 120));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 4, rect.Y + 16, rect.Width - 8, rect.Height - 20), new Color(0, 248, 160));
            spriteBatch.DrawString(font, str, new Vector2(20, 596), Color.Black);
        }

        /// <summary>
        /// 2段のメッセージウィンドウを描画する
        /// </summary>
        public static void DrawWindowBottom2(string str)
        {
            DrawWindow(new Rectangle(0, 535, 1024, 105));
            spriteBatch.DrawString(font, str, new Vector2(20, 555), Color.Black);
        }

        /// <summary>
        /// 輪郭線のついた文字列を描画する
        /// </summary>
        /// <param name="text">描画する文字列</param>
        /// <param name="position">描画位置</param>
        /// <param name="color">描画色</param>
        public static void DrawStringWithOutline(string text, Vector2 position, Color color)
        {
            Color back = new Color(40, 40, 184);
            spriteBatch.DrawString(font, text, position + new Vector2(-2, 0), back);
            spriteBatch.DrawString(font, text, position + new Vector2(2, 0), back);
            spriteBatch.DrawString(font, text, position + new Vector2(0, -2), back);
            spriteBatch.DrawString(font, text, position + new Vector2(0, 2), back);
            spriteBatch.DrawString(font, text, position + new Vector2(-2, -2), back);
            spriteBatch.DrawString(font, text, position + new Vector2(2, -2), back);
            spriteBatch.DrawString(font, text, position + new Vector2(-2, 2), back);
            spriteBatch.DrawString(font, text, position + new Vector2(2, 2), back);
            spriteBatch.DrawString(font, text, position, color);
        }

        /// <summary>
        /// 輪郭線のついた文字列を白色で描画する
        /// </summary>
        /// <param name="text">描画する文字列</param>
        /// <param name="position">描画位置</param>
        public static void DrawStringWithOutLine(string text, Vector2 position)
        {
            DrawStringWithOutline(text, position, Color.White);
        }

        /// <summary>
        /// 影の付いた文字列を描画する
        /// </summary>
        public static void DrawStringWithShadow(string text, Vector2 position)
        {
            spriteBatch.DrawString(font, text, position + new Vector2(3, 3), new Color(0, 0, 0, 96));
            spriteBatch.DrawString(font, text, position, Color.Black);
        }

        /// <summary>
        /// 塗り潰しのない四角を描画する
        /// </summary>
        /// <param name="rect">描画矩形</param>
        public static void DrawSquare(Rectangle rect, int width, Color color)
        {
            spriteBatch.Draw(tw, new Rectangle(rect.X, rect.Y, rect.Width, width), color);
            spriteBatch.Draw(tw, new Rectangle(rect.X, rect.Y, width, rect.Height), color);
            spriteBatch.Draw(tw, new Rectangle(rect.X, rect.Y + rect.Height - width, rect.Width, width), color);
            spriteBatch.Draw(tw, new Rectangle(rect.X + rect.Width - width, rect.Y, width, rect.Height), color);
        }

        /// <summary>
        /// 地名を表す文字列を取得する
        /// </summary>
        public static string GetStringLocation(Location loc)
        {
            switch (loc)
            {
                case Location.HakureiJinja:
                    return "博麗神社";
                case Location.MahounoMori:
                    return "魔法の森";
                case Location.KirinoMizuumi:
                    return "霧の湖";
                case Location.Koumakan:
                    return "紅魔館";
                case Location.Meikai:
                    return "冥界";
                case Location.MayoinoTikurin:
                    return "迷いの竹林";
                case Location.YoukainoYama:
                    return "妖怪の山";
                case Location.Titei:
                    return "地底";
                case Location.ShakunetuGigoku:
                    return "灼熱地獄";
                case Location.Tenkai:
                    return "天界";
                case Location.Kaidou:
                    return "街道";
                case Location.MorinoNaka:
                    return "森の中";
                case Location.Kyuuryo:
                    return "丘陵";
                case Location.MuranoNaka:
                    return "村の中";
                case Location.Kekkai:
                    return "結界領域";
                case Location.Makai:
                    return "魔界";
                case Location.Special1:
                case Location.Special2:
                case Location.Special3:
                case Location.Special4:
                    return "超絶空間";
                case Location.Shinrabansho:
                    return "神羅万象";
            }
            return "";
        }

        /// <summary>
        /// 属性を表す文字列を取得する
        /// </summary>
        public static string GetStringType(UnitType type)
        {
            switch (type)
            {
                case UnitType.Power:
                    return "力";
                case UnitType.Guard:
                    return "護";
                case UnitType.Intelligence:
                    return "知";
                case UnitType.Apparition:
                    return "幻";
                case UnitType.Technic:
                    return "技";
                case UnitType.Fortune:
                    return "運";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 地形を表す文字列を取得する
        /// </summary>
        public static string GetStringTerrain(Terrain tera)
        {
            switch (tera)
            {
                case Terrain.Plain:
                    return "平地";
                case Terrain.Forest:
                    return "森林";
                case Terrain.Mountain:
                    return "山地";
                case Terrain.Waterside:
                    return "水辺";
                case Terrain.Indoor:
                    return "屋内";
                case Terrain.Red_hot:
                    return "灼熱";
                case Terrain.Sanctuary:
                    return "聖域";
                case Terrain.Miasma:
                    return "瘴気";
                case Terrain.Crystal:
                    return "結晶";
                case Terrain.Banned:
                default:
                    return "不可";
            }
        }

        /// <summary>
        /// 地形相性を表す記号文字を取得する
        /// </summary>
        public static string GetStringAffinity(Affinity aff)
        {
            switch (aff)
            {
                case Affinity.VeryGood:
                    return "◎";
                case Affinity.Good:
                    return "○";
                case Affinity.Normal:
                    return "△";
                case Affinity.Bad:
                default:
                    return "✕";
            }
        }

        /// <summary>
        /// 行動の効果を表す文字列を取得する
        /// </summary>
        public static string GetStringActType(Act a)
        {
            string str = "";
            if (a.IsTargetAll)
                str += "全体";
            switch (a.type)
            {
                case ActType.Grapple:
                case ActType.Shot:
                    if (a.IsHaveAbility(ActAbility.Heat))
                        str += "高熱";
                    else if (a.IsHaveAbility(ActAbility.Cold))
                        str += "低温";
                    else if (a.IsHaveAbility(ActAbility.Thunder))
                        str += "電撃";
                    else if (a.IsHaveAbility(ActAbility.Laser))
                        str += "光学";
                    if (a.IsHaveAbility(ActAbility.AntiMinus))
                        str += "対負";
                    else if (a.IsHaveAbility(ActAbility.AntiPlus))
                        str += "対正";
                    if (a.IsHaveAbility(ActAbility.AntiHuman))
                        str += "対人";
                    else if (a.IsHaveAbility(ActAbility.AntiMonster))
                        str += "対妖";
                    if (a.IsHaveAbility(ActAbility.Revenge))
                        str += "報復";
                    else if (a.IsHaveAbility(ActAbility.Sacrifice))
                        str += "犠牲";
                    else if (a.IsHaveAbility(ActAbility.Destroy))
                        str += "破壊";
                    else if (a.IsHaveAbility(ActAbility.Sanctio))
                        str += "制裁";
                    if (a.IsHaveAbility(ActAbility.Repeat))
                        str += "反復";
                    else if (a.IsHaveAbility(ActAbility.Time))
                        str += "時間";
                    if (a.IsHaveAbility(ActAbility.Shock))
                        str += "衝撃";
                    else if (a.IsHaveAbility(ActAbility.Vacuum))
                        str += "吸引";
                    if (a.IsHaveAbility(ActAbility.Diffuse))
                        str += "拡散";
                    if (a.IsHaveAbility(ActAbility.Fast))
                        str += "高速";
                    if (a.IsHaveAbility(ActAbility.Hit))
                        str += "必中";
                    if (a.IsHaveAbility(ActAbility.Rush))
                        str += "特攻";
                    if (a.IsHaveAbility(ActAbility.Assassin))
                        str += "隠密";
                    if (a.IsHaveAbility(ActAbility.Geographic))
                        str += "地生";
                    else if (a.IsHaveAbility(ActAbility.Proficient))
                        str += "練達";
                    if (a.IsHaveAbility(ActAbility.Drain))
                        str += "吸収";
                    else if (a.IsHaveAbility(ActAbility.Spirit))
                        str += "精神";
                    if (a.IsHaveAbility(ActAbility.Penetrate))
                        str += "貫通";
                    else if (a.IsHaveAbility(ActAbility.Summon))
                        str += "召喚";
                    if (a.type == ActType.Grapple)
                        str += "格闘";
                    else
                        str += "射撃";
                    if (a.ability1 == ActAbility.None && a.ability2 == ActAbility.None)
                        str += "攻撃";
                    switch ((SymptonMinus)a.sympton)
                    {
                        case SymptonMinus.Damage:
                            str += ":継続";
                            break;
                        case SymptonMinus.Distract:
                            str += ":散漫";
                            break;
                        case SymptonMinus.Restraint:
                            str += ":束縛";
                            break;
                        case SymptonMinus.Stop:
                            str += ":停止";
                            break;
                        case SymptonMinus.Confuse:
                            str += ":混乱";
                            break;
                        case SymptonMinus.Deguard:
                            str += ":防不能";
                            break;
                        case SymptonMinus.Dedodge:
                            str += ":避不能";
                            break;
                        case SymptonMinus.FixInside:
                            str += ":内固定";
                            break;
                        case SymptonMinus.FixOutside:
                            str += ":外固定";
                            break;
                        case SymptonMinus.CarvedSeal:
                            str += ":刻印";
                            break;
                        case SymptonMinus.Stigmata:
                            str += ":聖痕";
                            break;
                    }
                    return str;
                case ActType.Heal:
                    return str + "回復";
                case ActType.Heal2:
                    return str + "回復2";
                case ActType.Revive:
                    return str + "蘇生";
                case ActType.Revive2:
                    return str + "蘇生2";
                case ActType.AddPlusSympton:
                    str = "[P]";
                    if (a.IsTargetAll)
                        str += "全体";
                    switch ((SymptonPlus)a.sympton)
                    {
                        case SymptonPlus.Heal:
                            return str + "再生";
                        case SymptonPlus.Charge:
                            return str + "活気";
                        case SymptonPlus.Concentrate:
                            return str + "集中";
                        case SymptonPlus.Swift:
                            return str + "俊足";
                        case SymptonPlus.ActAgain:
                            return str + "連続行動";
                    }
                    break;
                case ActType.ClearMinusSympton:
                    return "マイナス症状クリア";
                case ActType.AddMinusSympton:
                    str = "[M]";
                    if (a.IsTargetAll)
                        str += "全体";
                    switch ((SymptonMinus)a.sympton)
                    {
                        case SymptonMinus.Damage:
                            return str + "継続";
                        case SymptonMinus.Distract:
                            return str + "散漫";
                        case SymptonMinus.Restraint:
                            return str + "束縛";
                        case SymptonMinus.Stop:
                            return str + "停止";
                        case SymptonMinus.Confuse:
                            return str + "混乱";
                        case SymptonMinus.Deguard:
                            return str + "防御不能";
                        case SymptonMinus.Dedodge:
                            return str + "回避不能";
                        case SymptonMinus.FixInside:
                            return str + "範囲内固定";
                        case SymptonMinus.FixOutside:
                            return str + "範囲外固定";
                    }
                    break;
                case ActType.AddDoubleSympton:
                    return "聖痕 & 刻印";
                case ActType.ClearPlusSympton:
                    return "プラス症状クリア";
                case ActType.SetTrap:
                    str = "[T]";
                    switch ((Trap)a.sympton)
                    {
                        case Trap.GrappleTrap:
                            return str + "格闘トラップ";
                        case Trap.ShotTrap:
                            return str + "射撃トラップ";
                        case Trap.AttackTrap:
                            return str + "攻撃トラップ";
                        case Trap.OnceClear:
                            return str + "単発クリア";
                        case Trap.SPPlant:
                            return str + "SPプラント";
                        case Trap.HitPromise:
                            return str + "絶対命中";
                        case Trap.MagicCharge:
                            return str + "魔力充填";
                    }
                    break;
                case ActType.ClearTrap:
                    return "トラップクリア";
                case ActType.SetField:
                    str = "[F]";
                    return str + GetStringFieldEffect((FieldEffect)a.sympton, a.power);
                case ActType.Guard:
                    return "防御";
                case ActType.LessGuard:
                    return "未満防御";
                case ActType.Utsusemi:
                    return "空蝉";
                case ActType.Counter:
                    return "反撃";
                case ActType.BarrierDefense:
                    return "守護結界";
                case ActType.BarrierSpirit:
                    return "神羅結界";
                case ActType.TransSpace:
                    return "空間移動";
                case ActType.SearchEnemy:
                    return "索敵";
                case ActType.Hide:
                    return "隠蔽";
                case ActType.UpSpeed:
                    return "速度アップ";
                case ActType.UpClose:
                    return "近接アップ";
                case ActType.UpFar:
                    return "遠隔アップ";
                case ActType.UpReact:
                    return "反応アップ";
                case ActType.ClearParameter:
                    return "能力クリア";
                case ActType.SPUp:
                    return "SPアップ";
                case ActType.SPDrain:
                    return "SPドレイン";
                case ActType.LevelDrain:
                    return "レベルドレイン";
                case ActType.Musoutensei:
                    return "博麗奥義";
                case ActType.TimeStop:
                    return "時間停止";
                case ActType.MindCrash:
                    return "精神破壊";

                case ActType.Booster:
                    return "ブースター";
                case ActType.Scope:
                    return "スコープ";
                case ActType.DualBoost:
                    return "デュアルブースト";
                case ActType.Charge:
                    return "チャージ";
                case ActType.MoveAssist:
                    return "適地移動";
            }
            return "仕様外";
        }

        /// <summary>
        /// 行動の対象を表す文字列を取得する
        /// </summary>
        public static string GetStringActTarget(ActTarget at)
        {
            switch (at)
            {
                case ActTarget.Ally1:
                    return "味方単体";
                case ActTarget.AllyAll:
                    return "味方全体";
                case ActTarget.Enemy1:
                    return "敵単体";
                case ActTarget.EnemyAll:
                    return "敵全体";
                case ActTarget.AllyEnemy1:
                    return "敵味方単";
                case ActTarget.All:
                    return "敵味方全";
                case ActTarget.Field:
                    return "フィールド";
                case ActTarget.Space:
                    return "スペース";
                case ActTarget.Equip:
                    return "装備";
                default:
                    return "仕様外";
            }
        }

        /// <summary>
        /// フィールド効果を表す文字列を取得する
        /// </summary>
        public static string GetStringFieldEffect(FieldEffect ce, int fact = 0)
        {
            switch (ce)
            {
                case FieldEffect.APUp:
                    return "APアップ";
                case FieldEffect.APDown:
                    return "APダウン";
                case FieldEffect.HealBanned:
                    return "回復無効";
                case FieldEffect.DamageHalf:
                    return "ダメージ半減";
                case FieldEffect.Invalid:
                    return "効果無効";

                case FieldEffect.HPDamage:
                    return "HPダメージ";
                case FieldEffect.HPHeal:
                    return "HP回復";
                case FieldEffect.HitUp:
                    return "成功アップ";
                case FieldEffect.CostUp:
                    return "コスト増加";
                case FieldEffect.DamageFix:
                    return "ダメージ" + fact;
                case FieldEffect.SympInvalid:
                    return "症状クリア";
                case FieldEffect.AffinityDown:
                    return "適正ダウン";
                case FieldEffect.AffinityReverse:
                    return "相性反転";
                case FieldEffect.TimeStop:
                    return "時間停止";
                case FieldEffect.ChangeTerrain:
                    return "地形変化";
                case FieldEffect.None:
                default:
                    return "－－－－－－－－";
            }
        }

        /// <summary>
        /// 確率計算をする
        /// </summary>
        public static bool GetProbability(int fact, int pro = 100)
        {
            double d = GameBody.rand.NextDouble() * 0.998;
            double f = fact * (1d / pro);
            return d < f;
        }

        /// <summary>
        /// 確率計算をする
        /// </summary>
        public static bool GetProbability(double fact)
        {
            double d = GameBody.rand.NextDouble() * 0.998;
            return d < fact;
        }

        /// <summary>
        /// ラジアン角度の極座標を取得する
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Vector2 GetPolarCoord(float r, float a)
        {
            return r * new Vector2((float)Math.Cos(a), (float)Math.Sin(a));
        }

        /// <summary>
        /// Rectangleと円の衝突を判定する
        /// </summary>
        public static bool CheckIntersectRectCircle(Rectangle rect, Vector2 center, float rad)
        {
            if (Vector2.Distance(new Vector2(rect.X, rect.Y), center) <= rad)
                return true;
            if (Vector2.Distance(new Vector2(rect.X + rect.Width, rect.Y), center) <= rad)
                return true;
            if (Vector2.Distance(new Vector2(rect.X, rect.Y + rect.Height), center) <= rad)
                return true;
            if (Vector2.Distance(new Vector2(rect.X + rect.Width, rect.Y + rect.Height), center) <= rad)
                return true;

            if (center.X < rect.X)
            {
                if (center.X >= rect.X - rad && center.Y >= rect.Y && center.Y <= rect.Y + rect.Height)
                    return true;
            }
            if (center.X > rect.X + rect.Width)
            {
                if (center.X <= rect.X + rect.Width + rad && center.Y >= rect.Y && center.Y <= rect.Y + rect.Height)
                    return true;
            }
            else if (center.Y >= rect.Y - rad && center.Y <= rect.Y + rect.Height + rad)
                return true;

            return false;
        }
    }
}
