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

        const int unit_sep = 10;

        public static void Init(GraphicsDevice g, SpriteBatch s, ContentManager c)
        {
            graphics = g;
            spriteBatch = s;
            content = c;
            font = content.Load<SpriteFont>("font\\CommonFont");
            e_dot = content.Load<Effect>("effect\\dot");
            t_icon = content.Load<Texture2D>("img\\system\\icon001");
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
                    if (selectedUnit % unit_sep > 0)
                        selectedUnit--;
                    else
                    {
                        selectedUnit += unit_sep - 1;
                        if (selectedUnit >= allyUnit.Count)
                            selectedUnit = allyUnit.Count - 1;
                    }
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Right))
                {
                    if (selectedUnit % unit_sep < unit_sep - 1)
                    {
                        selectedUnit++;
                        if (selectedUnit >= allyUnit.Count)
                            selectedUnit = allyUnit.Count / unit_sep * unit_sep;
                    }
                    else
                        selectedUnit -= unit_sep - 1;
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Up))
                {
                    if (selectedUnit < unit_sep)
                    {
                        selectedUnit += allyUnit.Count / unit_sep * unit_sep;
                        if (selectedUnit >= allyUnit.Count)
                            selectedUnit -= unit_sep;
                    }
                    else
                        selectedUnit -= unit_sep;
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Down))
                {
                    selectedUnit += unit_sep;
                    if (selectedUnit >= allyUnit.Count)
                        selectedUnit = selectedUnit % unit_sep;
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
                        select1 = 3;
                    select2 = 0;
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Right))
                {
                    select1++;
                    if (select1 > 3)
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
                    state = 3;
                }
            }
            else if (state == 3)
            {
                Unit unit = allyUnit[selectedUnit];
                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Left))
                {
                    select2--;
                    if (select2 < 0)
                    {
                        if (select1 == 0)
                            select2 = 1;
                        else if (select1 == 1)
                            select2 = 5;
                        else if (select1 == 2)
                            select2 = unit.ability.Count - 1;
                        else if (select1 == 3)
                            select2 = 1;
                    }
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Right))
                {
                    select2++;
                    if ((select1 == 0 && select2 > 1)
                        || (select1 == 1 && select2 > 5)
                        || (select1 == 2 && select2 >= unit.ability.Count)
                        || (select1 == 3 && select2 > 1))
                        select2 = 0;
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
                // コマンド
                Helper.DrawWindow(new Rectangle(20, 20, 240, 29 + 32 * 5));
                spriteBatch.Draw(tw, new Rectangle(40, 48 + 32 * select1, 10, 10), Color.Black);
                spriteBatch.DrawString(font, "ユニット", new Vector2(60, 35 + 32 * 0), Color.Black);
                spriteBatch.DrawString(font, "アーティファクト", new Vector2(60, 35 + 32 * 1), Color.Black);
                spriteBatch.DrawString(font, "システム", new Vector2(60, 35 + 32 * 2), Color.Black);
                spriteBatch.DrawString(font, "セーブ", new Vector2(60, 35 + 32 * 3), Color.Black);
                spriteBatch.DrawString(font, "ロード", new Vector2(60, 35 + 32 * 4), Color.Black);

                // ユニット
                Helper.DrawWindowDrak(new Rectangle(280, 20, 720, 460));
                for (int i = 0; i < allyUnit.Count; i++)
                {
                    Vector2 pos = new Vector2(300 + 68 * (i % unit_sep), 40 + 100 * (i / unit_sep));
                    int level = allyUnit[i].level;
                    spriteBatch.Draw(allyUnit[i].t_icon, pos, Color.White);
                    if (level > 100)
                        spriteBatch.Draw(t_icon, pos + new Vector2(0, 64), new Rectangle(16 * (level / 100), 192, 16, 32), Color.White);
                    if (level > 10)
                        spriteBatch.Draw(t_icon, pos + new Vector2(16, 64), new Rectangle(16 * (level % 100 / 10), 192, 16, 32), Color.White);
                    spriteBatch.Draw(t_icon, pos + new Vector2(32, 64), new Rectangle(16 * (level % 10), 192, 16, 32), Color.White);
                }
                if (state == 1)
                    Helper.DrawSquare(new Rectangle(300 + 68 * (selectedUnit % unit_sep), 40 + 100 * (selectedUnit / unit_sep), 64, 96), 3, Color.White);

                // 情報
                Helper.DrawWindow(new Rectangle(20, 435, 240, 29 + 32 * 3));
                spriteBatch.DrawString(font, "人数:" + allyUnit.Count, new Vector2(35, 450 + 32 * 0), Color.Black);
                float av = 0;
                foreach (Unit u in allyUnit)
                {
                    av += u.level;
                }
                av /= allyUnit.Count;
                spriteBatch.DrawString(font, "平均Lv:" + (float)Math.Round(av, 2), new Vector2(35, 450 + 32 * 1), Color.Black);
                spriteBatch.DrawString(font, "時間:" + TimeSpan.Zero.ToString(), new Vector2(35, 450 + 32 * 2), Color.Black);

                // 目的
                Helper.DrawWindow(new Rectangle(280, 500, 720, 60));
                spriteBatch.DrawString(font, "目的:" + objective, new Vector2(295, 514), Color.Black);

                // 説明
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

                unit.DrawBattle(spriteBatch, new Vector2(208, 420), Color.White, false);

                Helper.DrawStringWithOutLine(unit.name, new Vector2(22, 10));
                Helper.DrawStringWithOutLine("Lv." + unit.level, new Vector2(22, 50));

                spriteBatch.Draw(tw, new Rectangle(400, 0, 16, 640), Color.Black);
                spriteBatch.Draw(tw, new Rectangle(404, 0, 4, 640), new Color(184, 0, 0));
                spriteBatch.Draw(tw, new Rectangle(408, 0, 4, 640), Color.Red);

                #region 選択ボックス
                spriteBatch.Draw(tw, new Rectangle(416, 0, 608, 64), new Color(16, 40, 16));
                spriteBatch.Draw(tw, new Rectangle(416 + 4, 4, 4, 4), Color.White);
                spriteBatch.Draw(tw, new Rectangle(416 + 12, 4, 608 - 16, 4), new Color(184, 0, 0));
                if (select1 == 0)
                    DrawBox(new Rectangle(416, 8, 152, 56));
                else
                    DrawBoxDark(new Rectangle(416, 8, 152, 56));
                spriteBatch.DrawString(font, "能力", new Vector2(467, 20), Color.Black);
                if (select1 == 1)
                    DrawBox(new Rectangle(568, 8, 152, 56));
                else
                    DrawBoxDark(new Rectangle(568, 8, 152, 56));
                spriteBatch.DrawString(font, "行動", new Vector2(619, 20), Color.Black);
                if (select1 == 2)
                    DrawBox(new Rectangle(720, 8, 152, 56));
                else
                    DrawBoxDark(new Rectangle(720, 8, 152, 56));
                spriteBatch.DrawString(font, "技能", new Vector2(771, 20), Color.Black);
                if (select1 == 3)
                    DrawBox(new Rectangle(872, 8, 148, 56));
                else
                    DrawBoxDark(new Rectangle(872, 8, 148, 56));
                spriteBatch.DrawString(font, "装備", new Vector2(923, 20), Color.Black);
                #endregion

                Vector2 pos = new Vector2(472, 74);// 416, 64
                if (select1 == 0)// 能力値
                {
                    // 基本値
                    Helper.DrawStringWithOutline("能力値", pos, color);
                    Helper.DrawStringWithOutline("属性", pos + new Vector2(0, 40), color);
                    Helper.DrawStringWithOutline("体力", pos + new Vector2(0, 80), color);
                    Helper.DrawStringWithOutline("霊力", pos + new Vector2(276, 80), color);
                    Helper.DrawStringWithOutline("行動", pos + new Vector2(0, 120), color);
                    Helper.DrawStringWithOutline("速度", pos + new Vector2(276, 120), color);
                    Helper.DrawStringWithOutline("回避", pos + new Vector2(0, 160), color);
                    Helper.DrawStringWithOutline("防御", pos + new Vector2(276, 160), color);
                    Helper.DrawStringWithOutline("近接", pos + new Vector2(0, 200), color);
                    Helper.DrawStringWithOutline("遠隔", pos + new Vector2(276, 200), color);

                    if (select2 == 0)
                    {
                        Helper.DrawStringWithOutLine("基本", pos + new Vector2(276, 0));
                        Helper.DrawStringWithOutline("ドライヴ", pos + new Vector2(366, 0), Color.Gray);
                        if (state == 3)
                            Helper.DrawSquare(new Rectangle(740, 71, 70, 40), 3, Color.White);

                        str = Helper.GetStringType(unit.type);
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 40));
                        str = unit.pHP.ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 80));
                        str = "0";
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 80));
                        str = unit.GetAP(false).ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 120));
                        str = unit.normalPar.speed.ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 120));
                        str = unit.normalPar.avoid.ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 160));
                        str = unit.normalPar.defense.ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 160));
                        str = unit.normalPar.close.ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 200));
                        str = unit.normalPar.far.ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 200));

                        Helper.DrawWindowBottom2("ユニットの基本パラメータ");
                    }
                    else
                    {
                        Helper.DrawStringWithOutline("基本", pos + new Vector2(276, 0), Color.Gray);
                        Helper.DrawStringWithOutLine("ドライヴ", pos + new Vector2(366, 0));
                        if (state == 3)
                            Helper.DrawSquare(new Rectangle(830, 71, 90, 40), 3, Color.White);

                        str = Helper.GetStringType(unit.type) + ":" + Helper.GetStringType(unit.type2);
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 40));
                        str = unit.pHP.ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 80));
                        str = "0";
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 80));
                        str = unit.GetAP(true).ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 120));
                        str = unit.drivePar.speed.ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 120));
                        str = unit.drivePar.avoid.ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 160));
                        str = unit.drivePar.defense.ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 160));
                        str = unit.drivePar.close.ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 200));
                        str = unit.drivePar.far.ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 200));

                        Helper.DrawWindowBottom2("ユニットのドライヴ中のパラメータ");
                    }

                    // 地形相性
                    Helper.DrawStringWithOutline("地形相性", pos + new Vector2(0, 260), color);
                    Helper.DrawStringWithOutline("平原", pos + new Vector2(0, 300), color);
                    Helper.DrawStringWithOutline("森林", pos + new Vector2(276, 300), color);
                    Helper.DrawStringWithOutline("山地", pos + new Vector2(0, 340), color);
                    Helper.DrawStringWithOutline("水辺", pos + new Vector2(276, 340), color);
                    Helper.DrawStringWithOutline("屋内", pos + new Vector2(0, 380), color);
                    Helper.DrawStringWithOutline("灼熱", pos + new Vector2(276, 380), color);
                    Helper.DrawStringWithOutline("聖域", pos + new Vector2(0, 420), color);
                    Helper.DrawStringWithOutline("瘴気", pos + new Vector2(276, 420), color);

                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Plain]);
                    Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 300));
                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Forest]);
                    Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 300));
                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Mountain]);
                    Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 340));
                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Waterside]);
                    Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 340));
                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Indoor]);
                    Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 380));
                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Red_hot]);
                    Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 380));
                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Sanctuary]);
                    Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 420));
                    str = Helper.GetStringAffinity(unit.affinity[(int)Terrain.Miasma]);
                    Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 420));
                }
                else if (select1 == 1)// スキル・スペカ
                {
                    Act a;
                    for (int i = 0; i < 6; i++)
                    {
                        a = unit.acts[i];
                        if (i == select2)
                            spriteBatch.Draw(t_icon, pos + new Vector2(40 * i, 20), new Rectangle(BattleScene.GetIconFact(a) * 32, 64, 32, 32), Color.White);
                        else
                            spriteBatch.Draw(t_icon, pos + new Vector2(40 * i, 20), new Rectangle(BattleScene.GetIconFact(a) * 32, 64, 32, 32), Color.Gray);
                    }
                    if (state == 3)
                        Helper.DrawSquare(new Rectangle(468 + 40 * select2, 90, 40, 40), 3, Color.White);

                    a = unit.acts[select2];
                    if (a != null)
                    {
                        if (a.lastSpell)
                            Helper.DrawStringWithOutLine("ラストスペル", pos + new Vector2(276, 20));
                        else if (a.IsSpell)
                            Helper.DrawStringWithOutLine("スペルカード", pos + new Vector2(276, 20));
                        else if (a.IsPassive)
                            Helper.DrawStringWithOutLine("パッシブスキル", pos + new Vector2(276, 20));
                        else
                            Helper.DrawStringWithOutLine("スキル", pos + new Vector2(276, 20));
                        if (a.IsSpell)
                            Helper.DrawStringWithOutline("消費SP", pos + new Vector2(0, 230), color);
                        if (!a.IsLimited)
                            Helper.DrawStringWithOutline("消費AP", pos + new Vector2(276, 230), color);
                        else
                            Helper.DrawStringWithOutline("回数", pos + new Vector2(276, 230), color);
                        Helper.DrawStringWithOutline("成功", pos + new Vector2(0, 300), color);
                        Helper.DrawStringWithOutline("威力", pos + new Vector2(276, 300), color);
                        Helper.DrawStringWithOutline("対象", pos + new Vector2(0, 370), color);
                        Helper.DrawStringWithOutline("射程", pos + new Vector2(276, 370), color);

                        Helper.DrawStringWithOutline("名称", pos + new Vector2(0, 90), color);
                        Helper.DrawStringWithOutLine(a.name, pos + new Vector2(100, 90));
                        Helper.DrawStringWithOutline("特性", pos + new Vector2(0, 160), color);
                        Helper.DrawStringWithOutLine(Helper.GetStringActType(a), pos + new Vector2(100, 160));
                        if (a.IsSpell)
                        {
                            str = a.sp.ToString();
                            Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 230));
                        }
                        if (!a.IsLimited)
                        {
                            str = a.ap.ToString();
                            Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 230));
                        }
                        else
                        {
                            str = a.count.ToString();
                            Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 230));
                        }
                        str = a.success.ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 300));
                        str = a.power.ToString();
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 300));
                        str = Helper.GetStringActTarget(a.target);
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(220 - font.MeasureString(str).X, 370));
                        str = a.rangeMin + "～" + a.rangeMax;
                        Helper.DrawStringWithOutLine(str, pos + new Vector2(496 - font.MeasureString(str).X, 370));

                        Helper.DrawWindowBottom2("");
                    }
                    else
                        Helper.DrawWindowBottom2("覚えていない");
                }
                else if (select1 == 2)// アビリティ
                {
                    Helper.DrawStringWithOutline("アビリティ", pos, color);

                    Helper.DrawWindowBottom2("");
                }
                else if (select1 == 3)// アーティファクト
                {
                    Helper.DrawStringWithOutline("アーティファクト", pos, color);

                    Helper.DrawWindowBottom2("");
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
