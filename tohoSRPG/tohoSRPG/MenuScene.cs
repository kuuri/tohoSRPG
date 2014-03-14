using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace tohoSRPG
{
    static class MenuScene
    {
        static GraphicsDevice graphics;
        static SpriteBatch spriteBatch;
        static ContentManager content;
        static SpriteFont font;
        static Effect e_dot;
        static Texture2D t_icon;

        public static string objective = "なし";

        /// <summary>
        /// 0:基本 1:ユニット選択 2:ユニット確認1 3:ユニット確認2
        /// </summary>
        static int state;
        static int select1, select2;
        static int selectedUnit;


        public static void Init(GraphicsDevice g, SpriteBatch s, ContentManager c)
        {
            graphics = g;
            spriteBatch = s;
            content = c;
            font = content.Load<SpriteFont>("font\\CommonFont");
            e_dot = content.Load<Effect>("effect\\dot");
            t_icon = content.Load<Texture2D>("img\\icon\\system\\icon001");
            state = 0;
            select1 = 0;
            selectedUnit = 0;
        }

        public static void Update(GameTime gameTime)
        {
            List<Unit> allyUnit = BattleScene.allyUnit;
            if (state == 0)
            {
                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Up))
                {
                    select1--;
                    if (select1 < 0)
                        select1 = 4;
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Down))
                {
                    select1++;
                    if (select1 > 4)
                        select1 = 0;
                }

                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide))
                {
                    switch (select1)
                    {
                        case 0:
                            state = 1;
                            selectedUnit = 0;
                            break;
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                    }
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Cancel))
                {
                    GameBody.ChangeScene(Scene.World);
                }
            }
            else if (state == 1)
            {
                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Left))
                {
                    if (selectedUnit % 8 > 0)
                        selectedUnit--;
                    else
                    {
                        selectedUnit += 7;
                        if (selectedUnit >= allyUnit.Count)
                            selectedUnit = allyUnit.Count - 1;
                    }
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Right))
                {
                    if (selectedUnit % 8 < 7)
                    {
                        selectedUnit++;
                        if (selectedUnit >= allyUnit.Count)
                            selectedUnit = allyUnit.Count / 8 * 8;
                    }
                    else
                        selectedUnit -= 7;
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Up))
                {
                    if (selectedUnit < 8)
                    {
                        selectedUnit += allyUnit.Count / 8 * 8;
                        if (selectedUnit >= allyUnit.Count)
                            selectedUnit -= 8;
                    }
                    else
                        selectedUnit -= 8;
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Down))
                {
                    selectedUnit += 8;
                    if (selectedUnit >= allyUnit.Count)
                        selectedUnit = selectedUnit % 8;
                }

                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Cancel))
                {
                    state = 0;
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide))
                {
                    state = 2;
                    select1 = 0;
                    select2 = 0;
                }
            }
            else if (state == 2)
            {
                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Left))
                {
                    select1--;
                    if (select1 < 0)
                        select1 = 4;
                    select2 = 0;
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Right))
                {
                    select1++;
                    if (select1 > 4)
                        select1 = 0;
                    select2 = 0;
                }

                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.ShoulderL))
                {
                    selectedUnit--;
                    if (selectedUnit < 0)
                        selectedUnit = allyUnit.Count - 1;
                    select2 = 0;
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.ShoulderR))
                {
                    selectedUnit++;
                    if (selectedUnit >= allyUnit.Count)
                        selectedUnit = 0;
                    select2 = 0;
                }

                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Cancel))
                {
                    state = 1;
                    select1 = 0;
                    select2 = 0;
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide))
                {
                    if (select1 != 1)
                        state = 3;
                }
            }
            else if (state == 3)
            {
                Unit unit = allyUnit[selectedUnit];
                if (select1 >= 0 || select1 <= 4)
                {
                    if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Left))
                    {
                        select2--;
                        if (select2 < 0)
                        {
                            if (select1 == 0)
                                select2 = 1;
                            else if (select1 == 2)
                                select2 = 5;
                            else if (select1 == 3)
                                select2 = unit.ability.Count - 1;
                            else if (select1 == 4)
                                select2 = 1;
                        }
                    }
                    else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Right))
                    {
                        select2++;
                        if ((select1 == 0 && select2 > 1)
                            || (select1 == 2 && select2 > 5)
                            || (select1 == 3 && select2 >= unit.ability.Count)
                            || (select1 == 4 && select2 > 1))
                            select2 = 0;
                    }
                }

                //if (InputManager.GetButtonStateIsPush(InputManager.GameButton.ShoulderL))
                //{
                //    selectedUnit--;
                //    if (selectedUnit < 0)
                //        selectedUnit = allyUnit.Count - 1;
                //}
                //else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.ShoulderR))
                //{
                //    selectedUnit++;
                //    if (selectedUnit >= allyUnit.Count)
                //        selectedUnit = 0;
                //}

                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Cancel))
                {
                    state = 2;
                }
            }
        }

        public static void Draw(GameTime gameTime)
        {
            graphics.Clear(Color.Gray);

            Texture2D tw = new Texture2D(graphics, 1, 1);
            Color[] c = { Color.White };
            tw.SetData(c);
            string str;
            List<Unit> allyUnit = BattleScene.allyUnit;

            spriteBatch.Begin(0, null, null, null, null, e_dot);

            if (state == 0 || state == 1)
            {
                #region 基本
                Helper.DrawWindow(new Rectangle(20, 20, 200, 29 + 32 * 5));
                spriteBatch.Draw(tw, new Rectangle(40, 48 + 32 * select1, 10, 10), Color.Black);
                spriteBatch.DrawString(font, "ユニット", new Vector2(60, 35 + 32 * 0), Color.Black);
                spriteBatch.DrawString(font, "アーティファクト", new Vector2(60, 35 + 32 * 1), Color.Black);
                spriteBatch.DrawString(font, "システム", new Vector2(60, 35 + 32 * 2), Color.Black);
                spriteBatch.DrawString(font, "セーブ", new Vector2(60, 35 + 32 * 3), Color.Black);
                spriteBatch.DrawString(font, "ロード", new Vector2(60, 35 + 32 * 4), Color.Black);

                Helper.DrawWindowDrak(new Rectangle(240, 20, 460, 324));
                for (int i = 0; i < allyUnit.Count; i++)
                {
                    Vector2 pos = new Vector2(260 + 52 * (i % 8), 40 + 76 * (i / 8));
                    int level = allyUnit[i].level;
                    spriteBatch.Draw(allyUnit[i].t_icon, pos, Color.White);
                    if (level > 100)
                        spriteBatch.Draw(t_icon, pos + new Vector2(0, 48), new Rectangle(12 * (level / 100), 120, 12, 24), Color.White);
                    if (level > 10)
                        spriteBatch.Draw(t_icon, pos + new Vector2(12, 48), new Rectangle(12 * (level % 100 / 10), 120, 12, 24), Color.White);
                    spriteBatch.Draw(t_icon, pos + new Vector2(24, 48), new Rectangle(12 * (level % 10), 120, 12, 24), Color.White);
                }
                if (state == 1)
                    Helper.DrawSquare(new Rectangle(260 + 52 * (selectedUnit % 8), 40 + 76 * (selectedUnit / 8), 48, 72), 3, Color.White);

                Helper.DrawWindow(new Rectangle(20, 300, 200, 29 + 32 * 3));
                spriteBatch.DrawString(font, "人数:" + allyUnit.Count, new Vector2(35, 315 + 32 * 0), Color.Black);
                float av = 0;
                foreach (Unit u in allyUnit)
                {
                    av += u.level;
                }
                av /= allyUnit.Count;
                spriteBatch.DrawString(font, "平均レベル:" + (float)Math.Round(av, 2), new Vector2(35, 315 + 32 * 1), Color.Black);
                spriteBatch.DrawString(font, "時間:" + TimeSpan.Zero.ToString(), new Vector2(35, 315 + 32 * 2), Color.Black);

                Helper.DrawWindow(new Rectangle(240, 364, 460, 61));
                spriteBatch.DrawString(font, "目的:" + objective, new Vector2(255, 379), Color.Black);

                switch (select1)
                {
                    case 0:
                        Helper.DrawWindowBottom1("ユニットのパラメータを確認します");
                        break;
                    case 1:
                        Helper.DrawWindowBottom1("アーティファクトの装備を設定します");
                        break;
                    case 2:
                        Helper.DrawWindowBottom1("システム設定を行います");
                        break;
                    case 3:
                        Helper.DrawWindowBottom1("現在のデータをセーブします");
                        break;
                    case 4:
                        Helper.DrawWindowBottom1("データをロードします");
                        break;
                }
                #endregion
            }
            else if (state == 2 || state == 3)
            {
                #region ユニット情報
                Unit unit = allyUnit[selectedUnit];
                Color color = new Color(128, 128, 248);

                unit.DrawBattle(spriteBatch, new Vector2(128, 315), Color.White, false);

                Helper.DrawStringWithOutLine(unit.name, new Vector2(22, 10));

                spriteBatch.Draw(tw, new Rectangle(256, 0, 12, 480), Color.Black);
                spriteBatch.Draw(tw, new Rectangle(259, 0, 3, 480), new Color(184, 0, 0));
                spriteBatch.Draw(tw, new Rectangle(262, 0, 3, 480), Color.Red);

                #region 選択ボックス
                spriteBatch.Draw(tw, new Rectangle(265, 0, 455, 55), new Color(16, 40, 16));
                spriteBatch.Draw(tw, new Rectangle(265 + 3, 3, 3, 3), Color.White);
                spriteBatch.Draw(tw, new Rectangle(265 + 9, 3, 455 - 12, 3), new Color(184, 0, 0));
                if (select1 == 0)
                    DrawBox(new Rectangle(265, 6, 90, 49));
                else
                    DrawBoxDark(new Rectangle(265, 6, 90, 49));
                spriteBatch.DrawString(font, "能力", new Vector2(283, 15), Color.Black);
                if (select1 == 1)
                    DrawBox(new Rectangle(355, 6, 90, 49));
                else
                    DrawBoxDark(new Rectangle(355, 6, 90, 49));
                spriteBatch.DrawString(font, "相性", new Vector2(373, 15), Color.Black);
                if (select1 == 2)
                    DrawBox(new Rectangle(445, 6, 90, 49));
                else
                    DrawBoxDark(new Rectangle(445, 6, 90, 49));
                spriteBatch.DrawString(font, "行動", new Vector2(463, 15), Color.Black);
                if (select1 == 3)
                    DrawBox(new Rectangle(535, 6, 90, 49));
                else
                    DrawBoxDark(new Rectangle(535, 6, 90, 49));
                spriteBatch.DrawString(font, "技能", new Vector2(553, 15), Color.Black);
                if (select1 == 4)
                    DrawBox(new Rectangle(625, 6, 92, 49));
                else
                    DrawBoxDark(new Rectangle(625, 6, 92, 49));
                spriteBatch.DrawString(font, "装備", new Vector2(643, 15), Color.Black);
                #endregion

                if (select1 == 0)// 能力値
                {
                    Helper.DrawStringWithOutline("能力値", new Vector2(300, 75), color);
                    Helper.DrawStringWithOutline("属性", new Vector2(300, 125), color);
                    Helper.DrawStringWithOutline("体力", new Vector2(300, 175), color);
                    Helper.DrawStringWithOutline("霊力", new Vector2(510, 175), color);
                    Helper.DrawStringWithOutline("行動", new Vector2(300, 225), color);
                    Helper.DrawStringWithOutline("速度", new Vector2(510, 225), color);
                    Helper.DrawStringWithOutline("回避", new Vector2(300, 275), color);
                    Helper.DrawStringWithOutline("防御", new Vector2(510, 275), color);
                    Helper.DrawStringWithOutline("近接", new Vector2(300, 325), color);
                    Helper.DrawStringWithOutline("遠隔", new Vector2(510, 325), color);

                    if (select2 == 0)
                    {
                        Helper.DrawStringWithOutLine("基本", new Vector2(510, 75));
                        Helper.DrawStringWithOutline("ドライヴ", new Vector2(600, 75), Color.Gray);

                        str = Helper.GetStringType(unit.type);
                        Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 125));
                        str = unit.pHP.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 175));
                        str = "0";
                        Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 175));
                        str = unit.GetAP(false).ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 225));
                        str = unit.normalPar.speed.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 225));
                        str = unit.normalPar.avoid.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 275));
                        str = unit.normalPar.defense.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 275));
                        str = unit.normalPar.close.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 325));
                        str = unit.normalPar.far.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 325));

                        Helper.DrawWindowBottom2("ユニットの基本パラメータ", "");
                    }
                    else
                    {
                        Helper.DrawStringWithOutline("基本", new Vector2(510, 75), Color.Gray);
                        Helper.DrawStringWithOutLine("ドライヴ", new Vector2(600, 75));

                        str = Helper.GetStringType(unit.type);
                        Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 125));
                        Helper.DrawStringWithOutLine(":" + Helper.GetStringType(unit.type2), new Vector2(475, 125));
                        str = unit.pHP.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 175));
                        str = "0";
                        Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 175));
                        str = unit.GetAP(true).ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 225));
                        str = unit.drivePar.speed.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 225));
                        str = unit.drivePar.avoid.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 275));
                        str = unit.drivePar.defense.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 275));
                        str = unit.drivePar.close.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 325));
                        str = unit.drivePar.far.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 325));

                        Helper.DrawWindowBottom2("ユニットのドライヴ中のパラメータ", "");
                    }
                }
                else if (select1 == 1)// 地形相性
                {
                    Helper.DrawStringWithOutline("地形相性", new Vector2(300, 75), color);
                    Helper.DrawStringWithOutline("平原", new Vector2(300, 175), color);
                    Helper.DrawStringWithOutline("森林", new Vector2(510, 175), color);
                    Helper.DrawStringWithOutline("山地", new Vector2(300, 225), color);
                    Helper.DrawStringWithOutline("水辺", new Vector2(510, 225), color);
                    Helper.DrawStringWithOutline("屋内", new Vector2(300, 275), color);
                    Helper.DrawStringWithOutline("灼熱", new Vector2(510, 275), color);
                    Helper.DrawStringWithOutline("聖域", new Vector2(300, 325), color);
                    Helper.DrawStringWithOutline("瘴気", new Vector2(510, 325), color);

                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Plain]);
                    Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 175));
                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Forest]);
                    Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 175));
                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Mountain]);
                    Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 225));
                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Waterside]);
                    Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 225));
                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Indoor]);
                    Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 275));
                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Red_hot]);
                    Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 275));
                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Sanctuary]);
                    Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 325));
                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Miasma]);
                    Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 325));

                    Helper.DrawWindowBottom2("◎、○、△、✕の順に相性が良い", "結晶地形は全てのユニットと相性が悪い");
                }
                else if (select1 == 2)// スキル・スペカ
                {
                    Act a;
                    for (int i = 0; i < 6; i++)
                    {
                        a = unit.acts[i];
                        if (i == select2)
                            spriteBatch.Draw(t_icon, new Vector2(300 + 30 * i, 75), new Rectangle(BattleScene.GetIconFact(a) * 24, 48, 24, 24), Color.White);
                        else
                            spriteBatch.Draw(t_icon, new Vector2(300 + 30 * i, 75), new Rectangle(BattleScene.GetIconFact(a) * 24, 48, 24, 24), Color.Gray);
                    }
                    a = unit.acts[select2];
                    if (a != null)
                    {
                        if (a.lastSpell)
                            Helper.DrawStringWithOutLine("ラストスペル", new Vector2(510, 75));
                        else if (a.IsSpell)
                            Helper.DrawStringWithOutLine("スペルカード", new Vector2(510, 75));
                        else if (a.IsPassive)
                            Helper.DrawStringWithOutLine("パッシブスキル", new Vector2(510, 75));
                        else
                            Helper.DrawStringWithOutLine("スキル", new Vector2(510, 75));
                        if (a.IsSpell)
                            Helper.DrawStringWithOutline("消費SP", new Vector2(300, 225), color);
                        if (!a.IsLimited)
                            Helper.DrawStringWithOutline("消費AP", new Vector2(510, 225), color);
                        else
                            Helper.DrawStringWithOutline("回数", new Vector2(510, 225), color);
                        Helper.DrawStringWithOutline("成功", new Vector2(300, 275), color);
                        Helper.DrawStringWithOutline("威力", new Vector2(510, 275), color);
                        Helper.DrawStringWithOutline("対象", new Vector2(300, 325), color);
                        Helper.DrawStringWithOutline("射程", new Vector2(510, 325), color);

                        Helper.DrawStringWithOutLine(a.name, new Vector2(300, 125));
                        Helper.DrawStringWithOutLine(Helper.GetStringActType(a), new Vector2(300, 175));
                        if (a.IsSpell)
                        {
                            str = a.sp.ToString();
                            Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 225));
                        }
                        if (!a.IsLimited)
                        {
                            str = a.ap.ToString();
                            Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 225));
                        }
                        else
                        {
                            str = a.count.ToString();
                            Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 225));
                        }
                        str = a.success.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 275));
                        str = a.power.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 275));
                        str = Helper.GetStringActTarget(a.target);
                        Helper.DrawStringWithOutLine(str, new Vector2(475 - font.MeasureString(str).X, 325));
                        str = a.rangeMin + "～" + a.rangeMax;
                        Helper.DrawStringWithOutLine(str, new Vector2(685 - font.MeasureString(str).X, 325));

                        Helper.DrawWindowBottom2("", "");
                    }
                    else
                        Helper.DrawWindowBottom2("覚えていない", "");
                }
                else if (select1 == 3)// アビリティ
                {
                    Helper.DrawStringWithOutline("アビリティ", new Vector2(300, 75), color);

                    Helper.DrawWindowBottom2("", "");
                }
                else if (select1 == 4)// アーティファクト
                {
                    Helper.DrawStringWithOutline("アーティファクト", new Vector2(300, 75), color);

                    Helper.DrawWindowBottom2("", "");
                }
                #endregion
            }

            spriteBatch.End();
        }

        static void DrawBox(Rectangle rect)
        {
            Texture2D tw = new Texture2D(graphics, 1, 1);
            Color[] c = { Color.White };
            tw.SetData(c);

            spriteBatch.Draw(tw, rect, new Color(16, 40, 16));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 3, rect.Y + 3, rect.Width - 3, rect.Height - 6), new Color(0, 180, 120));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 6, rect.Y + 6, rect.Width - 6, rect.Height - 9), new Color(0, 248, 160));
        }

        static void DrawBoxDark(Rectangle rect)
        {
            Texture2D tw = new Texture2D(graphics, 1, 1);
            Color[] c = { Color.White };
            tw.SetData(c);

            spriteBatch.Draw(tw, rect, new Color(16, 40, 16));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 3, rect.Y + 3, rect.Width - 3, rect.Height - 6), new Color(0, 120, 64));
            spriteBatch.Draw(tw, new Rectangle(rect.X + 6, rect.Y + 6, rect.Width - 6, rect.Height - 9), new Color(0, 180, 120));
        }
    }
}
