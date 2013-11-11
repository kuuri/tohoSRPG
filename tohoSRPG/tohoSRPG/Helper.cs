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
            spriteBatch.Draw(tw, rect, Color.Black);
            spriteBatch.Draw(tw, new Rectangle(rect.X + 3, rect.Y + 3, 3, 3), Color.White);
            spriteBatch.Draw(tw, new Rectangle(rect.X + 9, rect.Y + 3, rect.Width - 12, 3), Color.Red);
            spriteBatch.Draw(tw, new Rectangle(rect.X + 3, rect.Y + 9, rect.Width - 6, rect.Height - 12), new Color(0, 180, 120));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 6, rect.Y + 12, rect.Width - 9, rect.Height - 15), new Color(0, 255, 160));
        }
        
        /// <summary>
        /// 1段のメッセージウィンドウを描画する
        /// </summary>
        /// <param name="str">描画するメッセージ</param>
        public static void DrawWindowBottom1(string str)
        {
            DrawWindow(new Rectangle(0, 430, 720, 50));
            spriteBatch.DrawString(font, str, new Vector2(20, 443), Color.Black);
        }

        /// <summary>
        /// 2段のメッセージウィンドウを描画する
        /// </summary>
        /// <param name="str1">1段目のメッセージ</param>
        /// <param name="str2">2段目のメッセージ</param>
        public static void DrawWindowBottom2(string str1, string str2)
        {
            DrawWindow(new Rectangle(0, 400, 720, 80));
            spriteBatch.DrawString(font, str1, new Vector2(20, 413), Color.Black);
            spriteBatch.DrawString(font, str2, new Vector2(20, 443), Color.Black);
        }

        /// <summary>
        /// 影のついた文字列を描画する
        /// </summary>
        /// <param name="text">描画する文字列</param>
        /// <param name="position">描画位置</param>
        /// <param name="color">描画色</param>
        public static void DrawStringWithShadow(string text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, text, position + new Vector2(-2, -2), Color.DarkBlue);
            spriteBatch.DrawString(font, text, position + new Vector2(2, -2), Color.DarkBlue);
            spriteBatch.DrawString(font, text, position + new Vector2(-2, 2), Color.DarkBlue);
            spriteBatch.DrawString(font, text, position + new Vector2(2, 2), Color.DarkBlue);
            spriteBatch.DrawString(font, text, position, color);
        }

        /// <summary>
        /// 影のついた文字列を白色で描画する
        /// </summary>
        /// <param name="text">描画する文字列</param>
        /// <param name="position">描画位置</param>
        public static void DrawStringWithShadow(string text, Vector2 position)
        {
            DrawStringWithShadow(text, position, Color.White);
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
                case Affinity.Best:
                    return "◎";
                case Affinity.Good:
                    return "○";
                case Affinity.Normal:
                    return "△";
                case Affinity.Bad:
                default:
                    return "×";
            }
        }

        /// <summary>
        /// 行動の効果を表す文字列を取得する
        /// </summary>
        public static string GetStringActType(Act a)
        {
            string str = "";
            if (a.target == ActTarget.AllyAll || a.target == ActTarget.EnemyAll)
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
                    if (a.IsHaveAbility(ActAbility.Shock))
                        str += "衝撃";
                    else if (a.IsHaveAbility(ActAbility.Vacuum))
                        str += "吸引";
                    if (a.IsHaveAbility(ActAbility.Absorb))
                        str += "吸収";
                    if (a.IsHaveAbility(ActAbility.Diffuse))
                        str += "拡散";
                    if (a.IsHaveAbility(ActAbility.Fast))
                        str += "高速";
                    if (a.IsHaveAbility(ActAbility.Hit))
                        str += "必中";
                    if (a.IsHaveAbility(ActAbility.Rush))
                        str += "特攻";
                    if (a.IsHaveAbility(ActAbility.Penetrate))
                        str += "貫通";
                    if (a.IsHaveAbility(ActAbility.Destroy))
                        str += "破壊";
                    if (a.type == ActType.Grapple)
                        str += "格闘";
                    else
                        str += "射撃";
                    if (a.ability1 == ActAbility.None && a.ability2 == ActAbility.None)
                        str += "攻撃";
                    switch ((SymptonMinus)a.sympton)
                    {
                        case SymptonMinus.Slip:
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
                            str += ":不防";
                            break;
                        case SymptonMinus.Dedodge:
                            str += ":不避";
                            break;
                        case SymptonMinus.FixInside:
                            str += ":内固定";
                            break;
                        case SymptonMinus.FixOutside:
                            str += ":外固定";
                            break;
                    }
                    return str;
                case ActType.Heal:
                    return str + "回復";
                case ActType.Revive:
                    return str + "蘇生";
                case ActType.AddPlusSympton:
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
                    return str + "マイナス症状クリア";
                case ActType.AddMinusSympton:
                    switch ((SymptonMinus)a.sympton)
                    {
                        case SymptonMinus.Slip:
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
                case ActType.ClearPlusSympton:
                    return str + "プラス症状クリア";
                case ActType.SetTrap:
                    switch ((Trap)a.sympton)
                    {
                        case Trap.GrappleTrap:
                            return "格闘トラップ";
                        case Trap.ShotTrap:
                            return "射撃トラップ";
                        case Trap.AttackTrap:
                            return "攻撃トラップ";
                        case Trap.OnceClear:
                            return "単発クリア";
                        case Trap.SPPlant:
                            return "SPプラント";
                    }
                    break;
                case ActType.ClearTrap:
                    return "トラップクリア";
                case ActType.SetCrystal:
                    str = "結晶解放：";
                    switch ((CrystalEffect)a.sympton)
                    {
                        case CrystalEffect.HPDamage:
                            return str + "HPダメージ";
                        case CrystalEffect.HPHeal:
                            return str + "HP回復";
                        case CrystalEffect.ForbidHeal:
                            return str + "回復不能";
                        case CrystalEffect.APUp:
                            return str + "APアップ";
                        case CrystalEffect.APDown:
                            return str + "APダウン";
                        case CrystalEffect.CostUp:
                            return str + "コスト増加";
                        case CrystalEffect.HitUp:
                            return str + "成功アップ";
                        case CrystalEffect.DamageUp:
                            return str + "ダメージ増加";
                        case CrystalEffect.DamageDown:
                            return str + "ダメージ減少";
                        case CrystalEffect.AffinityDown:
                            return str + "適正ダウン";
                    }
                    break;
                case ActType.ClearCrystal:
                    return "結晶消去";
                case ActType.Guard:
                    return "防御";
                case ActType.LessGuard:
                    return "未満防御";
                case ActType.CoverCounter:
                    return "援護反撃";
                case ActType.SPUp:
                    return "SPアップ";
                case ActType.SPDrain:
                    return "SPドレイン";
                case ActType.LevelDrain:
                    return "レベルドレイン";
                case ActType.Musoutensei:
                    return "夢想天生";
                case ActType.Booster:
                    return "ブースター";
                case ActType.Scope:
                    return "スコープ";
                case ActType.DualBoost:
                    return "デュアルブースト";
                case ActType.Charge:
                    return "チャージ";
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
                    return "味方単体";
                case ActTarget.Enemy1:
                    return "敵単体";
                case ActTarget.EnemyAll:
                    return "敵全体";
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
        /// 結晶効果を表す文字列を取得する
        /// </summary>
        public static string GetStringCrystalEffect(CrystalEffect ce)
        {
            switch (ce)
            {
                case CrystalEffect.None:
                default:
                    return "－－－－－－－－";
            }
        }

        /// <summary>
        /// 確率計算をする
        /// </summary>
        public static bool GetProbability(int fact, int pro = 100)
        {
            double d = Game.rand.NextDouble() * 0.998;
            double f = fact * (1d / pro);
            return d < f;
        }

        /// <summary>
        /// 確率計算をする
        /// </summary>
        public static bool GetProbability(double fact)
        {
            double d = Game.rand.NextDouble() * 0.998;
            return d < fact;
        }

    }
}
