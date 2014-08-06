using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using BT = tohoSRPG.BattleScene;
using FF = tohoSRPG.FrendOfFoe;

namespace tohoSRPG
{
    static class ConfrontScene
    {
        static GraphicsDevice graphics;
        static SpriteBatch spriteBatch;
        static ContentManager content;
        static SpriteFont font;
        static Effect e_dot;
        static Texture2D t_icon;

        /// <summary>
        /// 0:戦闘前 1:出撃選択 2:セーブ 3:勝利 4:敗北
        /// </summary>
        static int state;
        static int select1, select2;

        static Location location;
        static string teamName;
        static string beforeBattleWords;
        static string victoryWords;
        static string defeatWords;
        static List<Unit> allyList, enemyList;

        public static void Init(GraphicsDevice g, SpriteBatch s, ContentManager c)
        {
            graphics = g;
            spriteBatch = s;
            content = c;
            font = content.Load<SpriteFont>("font\\CommonFont");
            e_dot = content.Load<Effect>("effect\\dot");
            t_icon = content.Load<Texture2D>("img\\system\\icon001");
            state = 0;
            select1 = select2 = 0;
        }

        public static void Update(GameTime gameTime)
        {
            if (state == 0)
            {
                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide))
                    state = 1;
            }
            else if (state == 1)
            {
                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Up))
                {
                    if (select1 % 5 == 0)
                        select2 = 1;
                    else
                        select1--;
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Down))
                {
                    if (select2 > 0)
                        select2 = 0;
                    else if (select1 % 5 < 4 && select1 < BT.allyUnit.Count - 1)
                        select1++;
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Left))
                {
                    if (select2 > 0)
                        select2 = 1;
                    else if (select1 / 5 > 0)
                        select1 -= 5;
                    else
                    {
                        select1 += BT.allyUnit.Count / 5 * 5;
                        if (select1 >= BT.allyUnit.Count)
                            select1 = BT.allyUnit.Count - 1;
                    }
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Right))
                {
                    if (select2 > 0)
                        select2 = 2;
                    else if (select1 / 5 < (BT.allyUnit.Count - 1) / 5)
                    {
                        select1 += 5;
                        if (select1 >= BT.allyUnit.Count)
                            select1 = BT.allyUnit.Count - 1;
                    }
                    else
                        select1 = select1 % 5;
                }

                if (allyList.Count > 0 && InputManager.GetButtonStateIsPush(InputManager.GameButton.Pause))
                {
                    if (allyList.Count % 2 == 1)
                    {
                        BT.allyUnitGadget.Add(new UnitGadget(FF.Ally, allyList[0], new Positon(2, 4), true));
                        if (allyList.Count > 1)
                        {
                            BT.allyUnitGadget.Add(new UnitGadget(FF.Ally, allyList[1], new Positon(2, 2)));
                            BT.allyUnitGadget.Add(new UnitGadget(FF.Ally, allyList[2], new Positon(2, 6)));
                            if (allyList.Count > 3)
                            {
                                BT.allyUnitGadget.Add(new UnitGadget(FF.Ally, allyList[3], new Positon(1, 3)));
                                BT.allyUnitGadget.Add(new UnitGadget(FF.Ally, allyList[4], new Positon(1, 5)));
                            }
                        }
                    }
                    else
                    {
                        BT.allyUnitGadget.Add(new UnitGadget(FF.Ally, allyList[0], new Positon(2, 3), true));
                        BT.allyUnitGadget.Add(new UnitGadget(FF.Ally, allyList[1], new Positon(2, 5)));
                        if (allyList.Count > 2)
                        {
                            BT.allyUnitGadget.Add(new UnitGadget(FF.Ally, allyList[2], new Positon(1, 2)));
                            BT.allyUnitGadget.Add(new UnitGadget(FF.Ally, allyList[3], new Positon(1, 6)));
                        }
                    }

                    if (enemyList.Count % 2 == 1)
                    {
                        BT.enemyUnitGadget.Add(new UnitGadget(FF.Enemy, enemyList[0], new Positon(6, 4), true));
                        if (enemyList.Count > 1)
                        {
                            BT.enemyUnitGadget.Add(new UnitGadget(FF.Enemy, enemyList[1], new Positon(6, 6)));
                            BT.enemyUnitGadget.Add(new UnitGadget(FF.Enemy, enemyList[2], new Positon(6, 2)));
                            if (enemyList.Count > 3)
                            {
                                BT.enemyUnitGadget.Add(new UnitGadget(FF.Enemy, enemyList[3], new Positon(7, 5)));
                                BT.enemyUnitGadget.Add(new UnitGadget(FF.Enemy, enemyList[4], new Positon(7, 3)));
                            }
                        }
                    }
                    else
                    {
                        BT.enemyUnitGadget.Add(new UnitGadget(FF.Enemy, enemyList[0], new Positon(6, 5), true));
                        BT.enemyUnitGadget.Add(new UnitGadget(FF.Enemy, enemyList[1], new Positon(6, 3)));
                        if (enemyList.Count > 2)
                        {
                            BT.enemyUnitGadget.Add(new UnitGadget(FF.Enemy, enemyList[2], new Positon(7, 6)));
                            BT.enemyUnitGadget.Add(new UnitGadget(FF.Enemy, enemyList[3], new Positon(7, 2)));
                        }
                    }

                    BT.allUnitGadget = new List<UnitGadget>(BT.allyUnitGadget);
                    BT.allUnitGadget.AddRange(BT.enemyUnitGadget);

                    GameBody.ChangeScene(Scene.Battle);
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide))
                {
                    if (select2 == 0)
                    {
                        if (allyList.Count < 5 && !allyList.Contains(BT.allyUnit[select1]))
                            allyList.Add(BT.allyUnit[select1]);
                    }
                }
                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Cancel))
                {
                    if (select2 == 0)
                    {
                        if (allyList.Count > 0)
                            allyList.RemoveAt(allyList.Count - 1);
                    }
                }
            }
            else if (state == 2)
            {
            }
            else if (state == 3)
            {
                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide))
                {
                    GameBody.ChangeScene(Scene.World);
                }
            }
        }

        public static void Draw(GameTime gameTime)
        {
            graphics.Clear(Color.Black);

            Texture2D tw = new Texture2D(graphics, 1, 1);
            Color[] c = { Color.White };
            tw.SetData(c);
            string str;

            spriteBatch.Begin(0, null, null, null, null, e_dot);

            if (state == 0 || state >= 3)
            {
                enemyList[0].DrawBattle(spriteBatch, new Vector2(144, 335), Color.White, false);

                Helper.DrawWindow(new Rectangle(48, 24, 336, 96));
                Helper.DrawStringWithShadow("TEAM", new Vector2(72, 64));
                spriteBatch.DrawString(font, teamName, new Vector2(144, 64), Color.Black);

                spriteBatch.Draw(tw, new Rectangle(0, 0, 336, 48), new Color(16, 40, 16));
                spriteBatch.Draw(tw, new Rectangle(3, 3, 330, 42), new Color(216, 40, 40));
                spriteBatch.Draw(tw, new Rectangle(6, 6, 327, 39), new Color(248, 128, 128));
                spriteBatch.Draw(tw, new Rectangle(3, 3, 12, 12), new Color(216, 40, 40));
                spriteBatch.Draw(tw, new Rectangle(3, 3, 9, 9), new Color(16, 40, 16));
                spriteBatch.Draw(tw, new Rectangle(3, 3, 6, 6), new Color(192, 192, 192));
                spriteBatch.Draw(tw, new Rectangle(6, 6, 3, 3), Color.White);
                Helper.DrawStringWithShadow("SPELL CARD BATTLE!", new Vector2(39, 6));

                Helper.DrawWindow(new Rectangle(384, 48, 336, 96));
                str = Helper.GetStringLocation(location);
                spriteBatch.DrawString(font, str, new Vector2(552 - font.MeasureString(str).X / 2, 84), Color.Black);

                Helper.DrawWindow(new Rectangle(48, 312, 360, 96));
                Helper.DrawStringWithShadow("LEADER", new Vector2(72, 336));
                spriteBatch.DrawString(font, enemyList[0].name, new Vector2(162, 336), Color.Black);

                Helper.DrawWindow(new Rectangle(432, 144, 288, 240));
                for (int i = 1; i < enemyList.Count; i++)
                {
                    spriteBatch.Draw(enemyList[i].t_icon, new Vector2(444, 108 + 54 * i), Color.White);
                    spriteBatch.DrawString(font, enemyList[i].name, new Vector2(500, 114 + 54 * i), Color.Black);
                }

                if (state == 0)
                    Helper.DrawWindowBottom2(beforeBattleWords);
                else if (state == 3)
                    Helper.DrawWindowBottom2(victoryWords);
                else if (state == 4)
                    Helper.DrawWindowBottom2(defeatWords);
            }
            else if (state == 1)
            {
                BT.allyUnit[select1].DrawBattle(spriteBatch, new Vector2(370, 370), Color.White, true);

                Helper.DrawWindow(new Rectangle(432, 0, 288, 64));
                spriteBatch.DrawString(font, "セーブ", new Vector2(462, 18), Color.Black);
                spriteBatch.DrawString(font, "撤退", new Vector2(608, 18), Color.Black);

                Helper.DrawWindow(new Rectangle(0, 64, 288, 320));
                if (allyList.Count % 2 == 1)
                {
                    spriteBatch.Draw(allyList[0].t_icon, new Vector2(168, 200), Color.White);
                    spriteBatch.Draw(t_icon, new Vector2(168, 224), new Rectangle(192, 24, 48, 24), Color.White);
                    if (allyList.Count > 1)
                    {
                        spriteBatch.Draw(allyList[1].t_icon, new Vector2(168, 104), Color.White);
                        spriteBatch.Draw(allyList[2].t_icon, new Vector2(168, 296), Color.White);
                        if (allyList.Count > 3)
                        {
                            spriteBatch.Draw(allyList[3].t_icon, new Vector2(72, 152), Color.White);
                            spriteBatch.Draw(allyList[4].t_icon, new Vector2(72, 248), Color.White);
                        }
                    }
                }
                else if(allyList.Count > 0)
                {
                    spriteBatch.Draw(allyList[0].t_icon, new Vector2(168, 152), Color.White);
                    spriteBatch.Draw(t_icon, new Vector2(168, 176), new Rectangle(192, 24, 48, 24), Color.White);
                    spriteBatch.Draw(allyList[1].t_icon, new Vector2(168, 248), Color.White);
                    if (allyList.Count > 2)
                    {
                        spriteBatch.Draw(allyList[2].t_icon, new Vector2(72, 104), Color.White);
                        spriteBatch.Draw(allyList[3].t_icon, new Vector2(72, 296), Color.White);
                    }
                }

                Helper.DrawWindow(new Rectangle(432, 52, 288, 332));
                spriteBatch.DrawString(font, "page:" + (select1 / 5 + 1) + "/" + (BT.allyUnit.Count / 5 + 1), new Vector2(444, 63), Color.Black);
                for (int i = 0; i < 5 && select1 / 5 * 5 + i < BT.allyUnit.Count; i++)
                {
                    Unit u = BT.allyUnit[select1 / 5 * 5 + i];
                    if (!allyList.Contains(u))
                    {
                        spriteBatch.Draw(u.t_icon, new Vector2(444, 96 + 54 * i), Color.White);
                        spriteBatch.DrawString(font, u.name, new Vector2(500, 102 + 54 * i), Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(u.t_icon, new Vector2(444, 96 + 54 * i), Color.Gray);
                        spriteBatch.DrawString(font, u.name, new Vector2(500, 102 + 54 * i), Color.Gray);
                    }
                }
                if (select2 == 0)
                    Helper.DrawSquare(new Rectangle(444, 96 + 54 * (select1 % 5), 256, 48), 3, Color.Black);
                else if (select2 == 1)
                    Helper.DrawSquare(new Rectangle(448, 18, 100, 32), 3, Color.Black);
                else
                    Helper.DrawSquare(new Rectangle(587, 18, 100, 32), 3, Color.Black);

                Helper.DrawWindowBottom2("チームのメンバーを決めてください\nSTARTで決定");
            }


            spriteBatch.End();
        }

        public static void SetScene(string team, List<Unit> enelist, string before, string victory, string defeat)
        {
            teamName = team;
            enemyList = enelist;
            beforeBattleWords = before;
            victoryWords = victory;
            defeatWords = defeat;
            state = 0;
            select1 = select2 = 0;
            allyList = new List<Unit>();
        }

        public static void SetMapData(Location type)
        {
            string[] map = new string[BattleScene.MapSize];

            location = type;
            switch (type)
            {
                case Location.HakureiJinja:
                    map[0] = "FFFPPPFFF";
                    map[1] = "FCPPSPPCF";
                    map[2] = "FPPSSSPPF";
                    map[3] = "PPSSSSSPP";
                    map[4] = "PSSSCSSSP";
                    map[5] = "PPSSSSSPP";
                    map[6] = "FPPSSSPPF";
                    map[7] = "FCPPSPPCF";
                    map[8] = "FFFPPPFFF";
                    break;
                case Location.MahounoMori:
                    map[0] = "DDDFFFDDD";
                    map[1] = "DCDDFFFCD";
                    map[2] = "DFFDFFFDD";
                    map[3] = "FFFDDDDDF";
                    map[4] = "FFFDCDFFF";
                    map[5] = "FDDDDDFFF";
                    map[6] = "DDFFFDFFD";
                    map[7] = "DCFFFDDCD";
                    map[8] = "DDDFFFDDD";
                    break;
                case Location.KirinoMizuumi:
                    map[0] = "WPWWPPWPW";
                    map[1] = "PCPWWWPCP";
                    map[2] = "WPWPWWWPW";
                    map[3] = "PWPWPWWPW";
                    map[4] = "PWPPCPPWP";
                    map[5] = "WPWWPWPWP";
                    map[6] = "WPWWWPWPW";
                    map[7] = "PCPWWWPCP";
                    map[8] = "WPWPPWWPW";
                    break;
                case Location.Koumakan:
                    map[0] = "DIIIDIIID";
                    map[1] = "ICIIIIICI";
                    map[2] = "IIIIDIIII";
                    map[3] = "IIIDRDIII";
                    map[4] = "DIDRCRDID";
                    map[5] = "IIIDRDIII";
                    map[6] = "IIIIDIIII";
                    map[7] = "ICIIIIICI";
                    map[8] = "DIIIDIIID";
                    break;
                case Location.Meikai:
                    map[0] = "DDPPPPDDD";
                    map[1] = "DCPPPDDCD";
                    map[2] = "DDPPPDPPP";
                    map[3] = "PDDDDDPPP";
                    map[4] = "PPPDCDPPP";
                    map[5] = "PPPDDDDDP";
                    map[6] = "PPPDPPPDD";
                    map[7] = "DCDDPPPCD";
                    map[8] = "DDDPPPPDD";
                    break;
                case Location.MayoinoTikurin:
                    map[0] = "FFFFFFFFF";
                    map[1] = "FCBFFFBCF";
                    map[2] = "FFFFFFFFF";
                    map[3] = "FFFFBFFFF";
                    map[4] = "FBFFCFFBF";
                    map[5] = "FFFFBFFFF";
                    map[6] = "FFFFFFFFF";
                    map[7] = "FCBFFFBCF";
                    map[8] = "FFFFFFFFF";
                    break;
                case Location.YoukainoYama:
                    map[0] = "MMMPMMMMW";
                    map[1] = "MCPMMMMCM";
                    map[2] = "MPPMPMWMM";
                    map[3] = "PMMPWWMMM";
                    map[4] = "MMPWCWPMM";
                    map[5] = "MMMWWPMMP";
                    map[6] = "MMWMPMPPM";
                    map[7] = "MCMMMMPCM";
                    map[8] = "WMMMMPMMM";
                    break;
                case Location.Titei:
                    map[0] = "MMMMMMMMM";
                    map[1] = "MCIMMMICM";
                    map[2] = "MMMIIIMMM";
                    map[3] = "MIIMMMIIM";
                    map[4] = "MMMICIMMM";
                    map[5] = "MIIMMMIIM";
                    map[6] = "MMMIIIMMM";
                    map[7] = "MCIMMMICM";
                    map[8] = "MMMMMMMMM";
                    break;
                case Location.ShakunetuGigoku:
                    map[0] = "RMRMRRRMR";
                    map[1] = "MCRMMMMCM";
                    map[2] = "RMRMRMRRR";
                    map[3] = "RMMRMRMMM";
                    map[4] = "RMRMCMRMR";
                    map[5] = "MMMRMRMMR";
                    map[6] = "RRRMRMRMR";
                    map[7] = "MCMMMMRCM";
                    map[8] = "RMRRRMRMR";
                    break;
                case Location.Tenkai:
                    map[0] = "SSSSSSSSS";
                    map[1] = "SCPPSPPCS";
                    map[2] = "SPIPPPIPS";
                    map[3] = "SPPISIPPS";
                    map[4] = "SSPSCSPSS";
                    map[5] = "SPPISIPPS";
                    map[6] = "SPIPPPIPS";
                    map[7] = "SCPPSPPCS";
                    map[8] = "SSSSSSSSS";
                    break;
                case Location.Kaidou:
                    map[0] = "PPMMMMMMM";
                    map[1] = "PCPPPMMCP";
                    map[2] = "PSSPPPPPP";
                    map[3] = "SSSSSSSSS";
                    map[4] = "SSSSCSSSS";
                    map[5] = "SSSSSSSSS";
                    map[6] = "PPPPPPSSP";
                    map[7] = "PCMMPPPCP";
                    map[8] = "MMMMMMMPP";
                    break;
                case Location.MorinoNaka:
                    map[0] = "WWWFFFFFF";
                    map[1] = "WCWWFFFCF";
                    map[2] = "FFWWWFFFF";
                    map[3] = "FFFWWFFFF";
                    map[4] = "FFFFCFFFF";
                    map[5] = "FFFFWWFFF";
                    map[6] = "FFFFWWWFF";
                    map[7] = "FCFFFWWCW";
                    map[8] = "FFFFFFWWW";
                    break;
                case Location.Kyuuryo:
                    map[0] = "PPPMMMPPP";
                    map[1] = "PCPPMMPCP";
                    map[2] = "PPMPMMMPP";
                    map[3] = "MMMMMMPPM";
                    map[4] = "MMMMCMMMM";
                    map[5] = "MPPMMMMMM";
                    map[6] = "PPMMMPMPP";
                    map[7] = "PCPMMPPCP";
                    map[8] = "PPPMMMPPP";
                    break;
                case Location.MuranoNaka:
                    map[0] = "PIPPIPPPP";
                    map[1] = "PCIPPIPCI";
                    map[2] = "PPIIPPIIP";
                    map[3] = "PIPIPIIPP";
                    map[4] = "IPPPCPPPI";
                    map[5] = "PPIIPIPIP";
                    map[6] = "PIIPPIIPP";
                    map[7] = "PCPIPPICP";
                    map[8] = "PIPPIPPIP";
                    break;
                case Location.Kekkai:
                    map[0] = "BBBBBBBBB";
                    map[1] = "BCSSBSSCB";
                    map[2] = "BSSSSSSSB";
                    map[3] = "BSSSSSSSB";
                    map[4] = "BBSSCSSBB";
                    map[5] = "BSSSSSSSB";
                    map[6] = "BSSSSSSSB";
                    map[7] = "BCSSBSSCB";
                    map[8] = "BBBBBBBBB";
                    break;
                case Location.Makai:
                    map[0] = "DDDBDBDDD";
                    map[1] = "DCDDDDDCD";
                    map[2] = "DDDDDDDDD";
                    map[3] = "BDDDBDDDB";
                    map[4] = "DDDBCBDDD";
                    map[5] = "BDDDBDDDB";
                    map[6] = "DDDDDDDDD";
                    map[7] = "DCDDDDDCD";
                    map[8] = "DDDBDBDDD";
                    break;
                case Location.Special1:
                    map[0] = "DDDDDDDDD";
                    map[1] = "DCDSSSDCD";
                    map[2] = "DDSSSSSDD";
                    map[3] = "DSSSSSSSD";
                    map[4] = "DSSSCSSSD";
                    map[5] = "DSSSSSSSD";
                    map[6] = "DDSSSSSDD";
                    map[7] = "DCDSSSDCD";
                    map[8] = "DDDDDDDDD";
                    break;
                case Location.Special2:
                    map[0] = "DSDSDSDSD";
                    map[1] = "SCSDSDSCS";
                    map[2] = "DSDSDSDSD";
                    map[3] = "SDSDSDSDS";
                    map[4] = "DSDSCSDSD";
                    map[5] = "SDSDSDSDS";
                    map[6] = "DSDSDSDSD";
                    map[7] = "SCSDSDSCS";
                    map[8] = "DSDSDSDSD";
                    break;
                case Location.Special3:
                    map[0] = "SSDDDDDDD";
                    map[1] = "SCSSSSSCD";
                    map[2] = "SSSSSSSSD";
                    map[3] = "SSDDDDSSS";
                    map[4] = "SSDDCDDSS";
                    map[5] = "SSSDDDDSS";
                    map[6] = "DSSSSSSSS";
                    map[7] = "DCSSSSSCS";
                    map[8] = "DDDDDDDSS";
                    break;
                case Location.Special4:
                    map[0] = "DSDDDDDSD";
                    map[1] = "SCSDDDSCS";
                    map[2] = "SSSDDDSSS";
                    map[3] = "SSSDDDSSS";
                    map[4] = "SSSSCSSSS";
                    map[5] = "SSSDDDSSS";
                    map[6] = "SSSDDDSSS";
                    map[7] = "SCSDDDSCS";
                    map[8] = "DSDDDDDSD";
                    break;
                case Location.Shinrabansho:
                    map[0] = "IRRRRDDDD";
                    map[1] = "ICRRRDDCF";
                    map[2] = "IIIRRDDFF";
                    map[3] = "IIIIRDFFF";
                    map[4] = "PPPPCFFFF";
                    map[5] = "PPPSWMMMM";
                    map[6] = "PPSSWWMMM";
                    map[7] = "PCSSWWWCM";
                    map[8] = "SSSSWWWWM";
                    break;
            }

            BT.MapSet(map);
        }

        public static int State
        {
            set { state = value; }
        }
    }

    public enum Location
    {
        HakureiJinja, MahounoMori, KirinoMizuumi, Koumakan, Meikai, MayoinoTikurin, YoukainoYama,
        Titei, ShakunetuGigoku, Tenkai, Kaidou, MorinoNaka, Kyuuryo, MuranoNaka,
        Kekkai, Makai, Special1, Special2, Special3, Special4, Shinrabansho
    }
}
