using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace tohoSRPG
{
    static class BattleScene
    {
        public const int MapSize = 9;

        static GraphicsDevice graphics;
        static SpriteBatch spriteBatch;
        static ContentManager content;
        static SpriteFont font;
        static Effect e_dot;
        static Effect e_desaturate;

        static TimeSpan currentTime;

        public static List<Unit> allyUnit;
        public static List<UnitGadget> allyUnitGadget;
        public static List<UnitGadget> enemyUnitGadget;
        public static List<UnitGadget> allUnitGadget;

        struct MapTip
        {
            public Terrain terrain;
            public Condition<CrystalEffect> effect;
        }
        static MapTip[,] map;
        struct A_Serch
        {
            public bool passable;
            public int cost;
            public int zocCost;
            public Positon parent;
        }
        static A_Serch[,] mapCost;

        public static Texture2D t_map;
        static RenderTarget2D r_map;
        static RenderTarget2D r_map_2;
        public static Texture2D t_bg;
        static Positon drawMapOrigin;
        static Texture2D t_shadow;
        static Texture2D t_cursor;

        /// <summary>
        /// -10:ターン開始 -9:APチャージ
        /// 0:行動選択 1:移動選択 2:対象選択 
        /// 3:ドライヴ 4:移動 5-6:対象表示 7:行動表示 8:行動計算 9:行動
        /// 18:SP減少 19:ドライヴ解除 20:SP溜め 21:未実装 22:行動失敗(確率) 23:行動失敗(対象不備)
        /// </summary>
        static int actStage;

        static bool battleStart;
        static int turn;
        static Queue<UnitGadget> unitOrder;
        static UnitGadget currentUnit;
        static int selectedAct;
        static int usingAP;
        static int aimedEnemy;
        static Positon unitCursor;
        static Positon targetCursor;

        static bool drive;
        static bool moved;
        static int target;
        static bool hit;
        static bool guard;
        static bool critical;
        static int damage;
        static float force;

        public static void Init(GraphicsDevice g, SpriteBatch s, ContentManager c)
        {
            graphics = g;
            spriteBatch = s;
            content = c;
            font = content.Load<SpriteFont>("font\\CommonFont");
            e_dot = content.Load<Effect>("effect\\dot");
            e_desaturate = content.Load<Effect>("effect\\desaturate");
            t_shadow = content.Load<Texture2D>("img\\battle\\shadow");
            t_cursor = content.Load<Texture2D>("img\\icon\\cursor");

            map = new MapTip[13, 13];
            mapCost = new A_Serch[13, 13];

            allyUnit = new List<Unit>();
            allyUnitGadget = new List<UnitGadget>();
            enemyUnitGadget = new List<UnitGadget>();

            r_map = new RenderTarget2D(graphics, 432, 432);
            r_map_2 = new RenderTarget2D(graphics, 432, 432);

            unitOrder = new Queue<UnitGadget>();

            ListsReset();
        }

        public static void ListsReset()
        {
            allyUnitGadget.Clear();
            enemyUnitGadget.Clear();
            drawMapOrigin = new Positon(156, 15);
            battleStart = false;
            actStage = -1;
            selectedAct = 0;
            turn = 0;
            unitOrder.Clear();
            currentTime = TimeSpan.Zero;
        }

        public static void Update(TimeSpan elps)
        {
            currentTime += elps;
            if (!IsMapMoving())
            {
                if (battleStart)
                {
                    #region 戦闘行為
                    if (currentUnit == null && unitOrder.Count == 0)
                    {
                        actStage = -10;
                        SetTurnOrder();
                        turn++;
                        for (int i = 0; i < allUnitGadget.Count; i++)
                            allUnitGadget[i].AP = allUnitGadget[i].GetAP();
                        currentTime = TimeSpan.Zero;
                    }
                    if (actStage == -10 && currentTime >= TimeSpan.FromMilliseconds(1000))
                    {
                        currentTime = TimeSpan.Zero;
                        actStage++;
                    }
                    if (actStage == -9 && currentTime >= TimeSpan.FromMilliseconds(1000))
                    {
                        currentTime = TimeSpan.Zero;
                        actStage = 0;
                    }
                    if (actStage == 0 && currentUnit == null)
                    {
                        currentUnit = unitOrder.Dequeue();
                        CalcMoveCost();
                        selectedAct = 0;
                        targetCursor = unitCursor = currentUnit.postion;
                        drive = currentUnit.drive;
                        moved = false;
                        target = 0;
                        currentUnit.dedodge = false;
                        currentUnit.deguard = false;
                        if (currentUnit.IsType(Type.Intelligence))
                            AddSP(currentUnit, 5);
                        if (currentUnit.trap.sympton == Trap.SPPlant)
                            AddSP(currentUnit, currentUnit.trap.power);
                    }
                    if (actStage >= 0 && actStage <= 2)
                    {
                        #region ユニットの行動選択
                        if (actStage == 0)// 行動選択
                        {

                            if (InputManager.GetButtonStateIsPush(InputManager.GameButton.ShoulderL))
                            {
                                currentTime = TimeSpan.Zero;
                                if (currentUnit.actPage == 0)
                                {
                                    currentUnit.actPage++;
                                    if (selectedAct != 0)
                                        selectedAct += 3;
                                }
                                else
                                {
                                    currentUnit.actPage--;
                                    if (selectedAct != 0)
                                        selectedAct -= 3;
                                }
                            }
                            if (currentUnit.unit.IsHaveAbility(Ability.Drive) && InputManager.GetButtonStateIsPush(InputManager.GameButton.ShoulderR))
                            {
                                if (drive)
                                    drive = false;
                                else if ((currentUnit.SP >= 10 && currentUnit.AP >= 6) || currentUnit.drive)
                                    drive = true;
                            }
                            if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Down))
                                selectedAct = 0;
                            else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Up))
                                selectedAct = currentUnit.actPage * 3 + 1;
                            else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Left))
                                selectedAct = currentUnit.actPage * 3 + 2;
                            else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Right))
                                selectedAct = currentUnit.actPage * 3 + 3;

                            if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide) && IsCanAct())
                            {
                                actStage++;
                            }
                            else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Cancel))
                            {
                                battleStart = false;
                                selectedAct = 0;
                                drive = currentUnit.drive;
                            }
                        }
                        else if (actStage == 1)// 移動先選択
                        {
                            int xd = 0;
                            int yd = 0;
                            if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Left))
                                xd = -1;
                            if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Right))
                                xd = 1;
                            if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Up))
                                yd = -1;
                            if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Down))
                                yd = 1;
                            if (xd != 0 || yd != 0)
                            {
                                int x = unitCursor.X;
                                int y = unitCursor.Y;
                                while (true)
                                {
                                    x += xd;
                                    y += yd;
                                    if (x < 0 || x >= MapSize || y < 0 || y >= MapSize)
                                    {
                                        if (x < 0 || x >= MapSize)
                                            x = unitCursor.X;
                                        if (y < 0 || y >= MapSize)
                                            y = unitCursor.Y;
                                        break;
                                    }
                                    if (mapCost[x, y].cost + GetUsingAP(false) <= currentUnit.AP)
                                        break;
                                }
                                unitCursor.X = x;
                                unitCursor.Y = y;
                            }
                            targetCursor = unitCursor;

                            if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Cancel))
                            {
                                actStage--;
                                selectedAct = 0;
                                unitCursor = currentUnit.postion;
                                usingAP = 0;
                            }
                            else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide))
                            {
                                if (selectedAct == 0)
                                {
                                    usingAP = GetUsingAP(true);
                                    currentTime = TimeSpan.Zero;
                                    if (unitCursor != currentUnit.postion)
                                        moved = true;
                                    if (currentUnit.drive != drive)
                                        actStage = 3;
                                    else if (!moved)
                                    {
                                        if (currentUnit.drive)
                                            actStage = 18;
                                        else
                                            actStage = 20;
                                    }
                                    else
                                        actStage = 4;
                                }
                                else if (selectedAct > 0 && GetActTarget().Count > 0)
                                {
                                    actStage++;
                                    aimedEnemy = -1;
                                }
                            }
                        }
                        else if (actStage == 2)// 対象選択
                        {
                            if (currentAct.target == ActTarget.Ally1 || currentAct.target == ActTarget.Enemy1 || currentAct.target == ActTarget.AllyEnemy1)
                            {
                                int xd = 0;
                                int yd = 0;
                                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Left))
                                    xd = -1;
                                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Right))
                                    xd = 1;
                                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Up))
                                    yd = -1;
                                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Down))
                                    yd = 1;
                                if (xd != 0 || yd != 0)
                                {
                                    int x = targetCursor.X;
                                    int y = targetCursor.Y;
                                    while (true)
                                    {
                                        x += xd;
                                        y += yd;
                                        if (x < 0 || x >= MapSize || y < 0 || y >= MapSize)
                                        {
                                            if (x < 0 || x >= MapSize)
                                                x = targetCursor.X;
                                            if (y < 0 || y >= MapSize)
                                                y = targetCursor.Y;
                                            break;
                                        }
                                        float d = Positon.Distance(unitCursor, new Positon(x, y));
                                        if (d >= currentAct.rangeMin && d <= currentAct.rangeMax)
                                            break;
                                    }
                                    targetCursor.X = x;
                                    targetCursor.Y = y;
                                }
                                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.ShoulderR))
                                {
                                    aimedEnemy++;
                                    if (aimedEnemy >= GetActTarget().Count)
                                        aimedEnemy = 0;
                                    targetCursor = GetActTarget()[aimedEnemy].postion;
                                }
                                else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.ShoulderL))
                                {
                                    aimedEnemy--;
                                    if (aimedEnemy < 0)
                                        aimedEnemy = GetActTarget().Count - 1;
                                    targetCursor = GetActTarget()[aimedEnemy].postion;
                                }
                            }

                            if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Cancel))
                            {
                                actStage--;
                                targetCursor = unitCursor;
                            }
                            else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide)
                                && (currentAct.IsTargetAll || Positon.Distance(unitCursor, targetCursor) >= currentAct.rangeMin))
                            {
                                bool targetExist = false;
                                List<UnitGadget> lug = GetActTarget();
                                if (currentAct.IsTargetAll)
                                {
                                    if (lug.Count > 0)
                                    {
                                        targetExist = true;
                                        target = 0;
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < lug.Count; i++ )
                                        if (lug[i] == currentUnit ? targetCursor == unitCursor : targetCursor == lug[i].postion)
                                        {
                                            targetExist = true;
                                            target = i;
                                        }
                                }

                                if (targetExist)
                                {
                                    usingAP = GetUsingAP(true);
                                    currentTime = TimeSpan.Zero;
                                    if (unitCursor != currentUnit.postion)
                                        moved = true;
                                    if (currentUnit.drive != drive)
                                        actStage++;
                                    else if (!moved)
                                        actStage = 5;
                                    else
                                        actStage = 4;
                                }
                            }
                        }
                        #endregion
                    }
                    else if (actStage >= 3)
                    {
                        #region ユニットの行動
                        if (actStage == 3) // ドライヴ
                        {
                            if (currentTime.TotalMilliseconds >= 1000)
                                currentUnit.drive = drive;
                            if (currentTime.TotalMilliseconds >= 2000)
                            {
                                currentTime = TimeSpan.Zero;
                                if (selectedAct == 0 && !moved)
                                {
                                    if (currentUnit.drive)
                                        actStage = 18;
                                    else
                                        TurnEnd();
                                }
                                else if (selectedAct > 0 && !moved)
                                    actStage = 5;
                                else
                                    actStage++;
                            }
                        }
                        else if (actStage == 4) // 移動
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(750))
                            {
                                currentTime = TimeSpan.Zero;
                                if (currentUnit.postion != unitCursor)
                                {
                                    Positon goal = GetMoveRoute(unitCursor);
                                    currentUnit.postion = goal;
                                }
                                else if (selectedAct == 0)
                                {
                                    if (currentUnit.drive)
                                        actStage = 18;
                                    else
                                        TurnEnd();
                                }
                                else
                                    actStage++;
                            }
                        }
                        else if (actStage == 5 || actStage == 6) // 対象表示
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(400))
                            {
                                currentTime = TimeSpan.Zero;
                                actStage++;
                            }
                        }
                        else if (actStage == 7) // 行動表示
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(1500))
                            {
                                currentTime = TimeSpan.Zero;
                                UnitGadget target = GetActTarget()[0];
                                if ((currentAct.type >= ActType.Booster)
                                    ||(currentAct.type == ActType.ClearMinusSympton && !IsTargetAlly(target) && target.symptonMinus.sympton != SymptonMinus.MinusInvalid)
                                    ||(currentAct.type == ActType.ClearPlusSympton && IsTargetAlly(target) && target.symptonPlus.sympton != SymptonPlus.PlusInvalid)
                                    ||(currentAct.type == ActType.ClearTrap && IsTargetAlly(target) && target.trap.sympton != Trap.TrapClear)
                                    ||(currentAct.type == ActType.SPDrain && target.SP == 0)
                                    ||(currentAct.type == ActType.LevelDrain && target.level == 0))
                                {
                                    actStage = 23;
                                }
                                else if (currentAct.TypeInt % 100 == 0)
                                {
                                    actStage++;
                                }
                                else
                                {
                                    actStage = 21;
                                }
                            }
                        }
                        else if (actStage == 8) // 行動計算
                        {
                            if (currentAct.TypeInt % 100 == 0)
                            {
                                CalcAttackResult(GetActTarget()[target++]);
                                currentTime = TimeSpan.Zero;
                                actStage++;
                            }
                        }
                        else if (actStage == 9) // 行動
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(2000))
                            {
                                currentTime = TimeSpan.Zero;
                                if (currentAct.TypeInt % 100 == 0 && hit)
                                {
                                    UnitGadget ug = GetActTarget()[target - 1];
                                    AddHP(ug, -damage);
                                    AddSP(ug, 10);
                                }
                                if (currentAct.IsTargetAll && target < GetActTarget().Count)
                                    actStage--;
                                else
                                {
                                    if (currentUnit.drive)
                                        actStage = 18;
                                    else
                                        TurnEnd();
                                }
                            }
                        }
                        else if (actStage == 18) // SP減少
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(2000))
                            {
                                currentTime = TimeSpan.Zero;
                                AddSP(currentUnit, currentUnit.AP - usingAP);
                                AddSP(currentUnit, -10);
                                if (currentUnit.SP <= 0)
                                {
                                    currentUnit.drive = false;
                                    actStage++;
                                }
                                else if (selectedAct == 0 && !moved && usingAP == 0)
                                    actStage = 20;
                                else
                                    TurnEnd();
                            }
                        }
                        else if (actStage == 19) // ドライヴ解除
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(1000))
                            {
                                currentTime = TimeSpan.Zero;
                                if (selectedAct == 0 && !moved)
                                    actStage++;
                                else
                                    TurnEnd();
                            }
                        }
                        else if (actStage == 20) // SP溜め
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(3000))
                            {
                                currentTime = TimeSpan.Zero;
                                TurnEnd();
                            }
                        }
                        else // 行動失敗
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(2000))
                            {
                                currentTime = TimeSpan.Zero;
                                if (currentUnit.drive)
                                    actStage = 18;
                                else
                                    TurnEnd();
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region 状態確認
                    if (actStage == -1)
                    {
                        if (currentTime >= TimeSpan.FromMilliseconds(2000))
                            actStage++;
                    }
                    else if (actStage == 0 || actStage == 1)
                    {
                        if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Up))
                        {
                            if (actStage == 0 && selectedAct > 0)
                                selectedAct--;
                            else if (actStage == 1 && selectedAct > 1)
                                selectedAct--;
                        }
                        else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Down))
                        {
                            if (actStage == 0 && selectedAct < allyUnitGadget.Count)
                                selectedAct++;
                            else if (actStage == 1 && selectedAct < enemyUnitGadget.Count)
                                selectedAct++;
                        }
                        else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Left))
                        {
                            if (actStage == 1)
                            {
                                actStage--;
                                if (selectedAct >= allyUnitGadget.Count)
                                    selectedAct = allyUnitGadget.Count;
                            }
                        }
                        else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Right))
                        {
                            if (actStage == 0)
                            {
                                actStage++;
                                if (selectedAct == 0)
                                    selectedAct++;
                                if (selectedAct >= enemyUnitGadget.Count)
                                    selectedAct = enemyUnitGadget.Count;
                            }
                        }
                        else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Pause)
                            || (selectedAct == 0 && InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide)))
                        {
                            battleStart = true;
                            selectedAct = 0;
                            if (turn == 0)
                                actStage = -1;
                            else
                                actStage = 0;
                        }
                        else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide))
                        {
                            actStage += 2;
                        }
                    }
                    else
                    {
                        if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Cancel))
                        {
                            actStage -= 2;
                        }
                    }
                    #endregion
                }
            }
            else
            {
                // マップの表示位置移動
                int goal = GetMapMoveGoal();
                int delta = 10;
                if (MathHelper.Distance(drawMapOrigin.X, goal) <= delta)
                    drawMapOrigin.X = goal;
                else if (drawMapOrigin.X - goal > 0)
                    drawMapOrigin.X -= delta;
                else
                    drawMapOrigin.X += delta;
            }
        }

        public static void Draw(GameTime gameTime)
        {
            Texture2D tw = new Texture2D(graphics, 1, 1);
            Color[] c = { Color.White };
            tw.SetData(c);
            string str;

            #region マップの色付け
            graphics.SetRenderTarget(r_map_2);

            spriteBatch.Begin(0, null, null, null, null, e_desaturate);
            int satu = 64;
            if (battleStart && actStage >= 0 && actStage <= 2 && !IsMapMoving())
                satu = 0;
            spriteBatch.Draw(r_map, Vector2.Zero, new Color(255, 255, 255, satu));
            spriteBatch.End();

            spriteBatch.Begin();

            if (satu == 0 && currentUnit != null && IsCanAct())
            {
                int actCost = GetUsingAP(false);
                for(int i = 0; i < MapSize; i++)
                    for (int j = 0; j < MapSize; j++)
                        if (mapCost[i, j].cost + actCost <= currentUnit.AP)
                            spriteBatch.Draw(tw, new Rectangle(i * 48, j * 48, 48, 48), new Color(0, 0, 128, 0));
                if (selectedAct > 0)
                {
                    for (int i = 0; i < MapSize; i++)
                        for (int j = 0; j < MapSize; j++)
                            if (Positon.Distance(new Positon(i, j), unitCursor) >= currentAct.rangeMin
                                && Positon.Distance(new Positon(i, j), unitCursor) <= currentAct.rangeMax)
                                spriteBatch.Draw(tw, new Rectangle(i * 48, j * 48, 48, 48), new Color(128, 0, 0, 0));
                }
            }

            spriteBatch.End();
            graphics.SetRenderTarget(null);
            #endregion

            spriteBatch.Begin(0, null, null, null, null, e_dot);

            
            // 背景描画
            spriteBatch.Draw(t_bg, new Vector2(0, 0), Color.White);
            spriteBatch.Draw(t_bg, new Vector2(384, 0), Color.White);


            if (battleStart && actStage >= 0 && !IsMapMoving() && currentUnit != null)
            {
                // ユニット描画
                DrawUnit(currentUnit, gameTime);
            }

            if (battleStart || actStage < 2)
            {
                // マップ描画
                spriteBatch.Draw(tw, new Rectangle(drawMapOrigin.X - 3, drawMapOrigin.Y - 9, 426, 432), Color.Black);
                spriteBatch.Draw(tw, new Rectangle(drawMapOrigin.X, drawMapOrigin.Y - 6, 3, 3), Color.White);
                spriteBatch.Draw(tw, new Rectangle(drawMapOrigin.X + 6, drawMapOrigin.Y - 6, 414, 3), Color.Red);
                spriteBatch.Draw(r_map_2, drawMapOrigin, new Rectangle(6, 6, 420, 420), Color.White);

                // メダフォース
                if (battleStart && (actStage == 18 || actStage == 20))
                {
                    Vector2 v = drawMapOrigin + new Vector2(currentUnit.postion.X * 48 + 18, currentUnit.postion.Y * 48 + 18);
                    float tt;
                    if (actStage == 18)
                        tt = 2000;
                    else
                        tt = 3000;
                    float t = (float)currentTime.TotalMilliseconds / tt;
                    float s = 1 + t;
                    float a = 1 - t;
                    spriteBatch.Draw(t_cursor, v, new Rectangle(200, 72, 48, 48), new Color(a, a, a, a), 0, new Vector2(24, 24), s, SpriteEffects.None, 0);
                }

                // ユニットアイコン描画
                foreach (UnitGadget ug in allUnitGadget)
                    spriteBatch.Draw(ug.unit.t_icon, new Rectangle(drawMapOrigin.X + ug.postion.X * 48 - 6, drawMapOrigin.Y + ug.postion.Y * 48 - 6, 48, 48), Color.White);
            }

            if (battleStart && !IsMapMoving())
            {
                #region 戦闘情報
                if (actStage == -10)
                {
                    Helper.DrawWindowBottom1("ターン" + turn);
                }
                else if (actStage == -9)
                {
                    foreach (UnitGadget ug in allUnitGadget)
                    {
                        Vector2 pos = drawMapOrigin + new Vector2(ug.postion.X * 48, ug.postion.Y * 48 + 18 - (int)GetIconFact());
                        spriteBatch.Draw(t_cursor, pos, new Rectangle(120, 120, 12, 24), Color.White);
                        if (ug.AP > 10)
                            spriteBatch.Draw(t_cursor, pos + new Vector2(12, 0), new Rectangle(12 * (ug.AP / 10), 120, 12, 24), Color.White);
                        spriteBatch.Draw(t_cursor, pos + new Vector2(24, 0), new Rectangle(12 * (ug.AP % 10), 120, 12, 24), Color.White);
                    }

                    Helper.DrawWindowBottom1("ユニットのAPチャージ!");
                }
                else if (currentUnit == null)
                {
                }
                else if (actStage >= 0 && actStage <= 2) // 行動選択
                {
                    if (true)//currentUnit.ff == FrendOfFoe.Ally)
                    {
                        // ユニット情報描画
                        spriteBatch.Draw(t_cursor, new Vector2(10, 10), new Rectangle((int)currentUnit.unit.type * 32, 72, 32, 32), Color.White);
                        if (currentUnit.drive)
                            spriteBatch.Draw(t_cursor, new Vector2(26, 26), new Rectangle((int)currentUnit.unit.type2 * 16, 104, 16, 16), Color.White);
                        Helper.DrawStringWithShadow(currentUnit.unit.name, new Vector2(52, 10));
                        Helper.DrawStringWithShadow("HP:", new Vector2(10, 39));
                        str = currentUnit.HP.ToString();
                        Helper.DrawStringWithShadow(str, new Vector2(95 - font.MeasureString(str).X, 39));
                        spriteBatch.Draw(tw, new Rectangle(106, 48, 160, 18), Color.White);
                        spriteBatch.Draw(tw, new Rectangle(108, 51, 156, 12), Color.Black);
                        spriteBatch.Draw(tw, new Rectangle(108, 51, 156 * currentUnit.HP / currentUnit.HPmax, 12), Color.LimeGreen);
                        Helper.DrawStringWithShadow("SP:", new Vector2(10, 63));
                        str = currentUnit.SP.ToString();
                        Helper.DrawStringWithShadow(str, new Vector2(95 - font.MeasureString(str).X, 63));
                        spriteBatch.Draw(tw, new Rectangle(106, 72, 160, 18), Color.White);
                        spriteBatch.Draw(tw, new Rectangle(108, 75, 156, 12), Color.Black);
                        spriteBatch.Draw(tw, new Rectangle(108, 75, 156 * currentUnit.SP / currentUnit.SPmax, 12), Color.SkyBlue);
                        Helper.DrawStringWithShadow("AP:  /", new Vector2(10, 87));
                        str = GetUsingAP(true).ToString();
                        Helper.DrawStringWithShadow(str, new Vector2(80 - font.MeasureString(str).X, 87));
                        str = currentUnit.AP.ToString();
                        Helper.DrawStringWithShadow(str, new Vector2(122 - font.MeasureString(str).X, 87));

                        #region 行動アイコン
                        float t = (float)currentTime.TotalMilliseconds * 0.2f;
                        if (t > 20)
                            t = 20;
                        spriteBatch.Draw(t_cursor, new Vector2(30 - t, 120 + t / 2), new Rectangle(248, (currentUnit.actPage + 1) % 2 * 24, 48, 24), Color.Gray);
                        spriteBatch.Draw(t_cursor, new Vector2(10 + t, 130 - t / 2), new Rectangle(248, currentUnit.actPage * 24, 48, 24), Color.White);

                        if (currentUnit.unit.IsHaveAbility(Ability.Drive))
                        {
                            if (drive)
                            {
                                spriteBatch.Draw(t_cursor, new Vector2(210, 120), new Rectangle(248, 72, 48, 24), Color.White);
                                spriteBatch.Draw(t_cursor, new Vector2(220, 140), new Rectangle(248, 120, 48, 24), Color.White);
                            }
                            else if ((currentUnit.SP >= 10 && currentUnit.AP >= 6) || currentUnit.drive)
                                spriteBatch.Draw(t_cursor, new Vector2(210, 120), new Rectangle(248, 48, 48, 24), Color.White);
                            else
                                spriteBatch.Draw(t_cursor, new Vector2(210, 120), new Rectangle(248, 96, 48, 24), Color.White);
                        }

                        Rectangle rect = new Rectangle(96, 0, 54, 45);
                        Color color;
                        if (selectedAct == 0)
                            color = Color.White;
                        else
                            color = Color.Gray;
                        spriteBatch.Draw(t_cursor, new Vector2(144, 300), rect, color, MathHelper.Pi, new Vector2(25, 23), 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(t_cursor, new Vector2(132, 286), new Rectangle(0, 48, 24, 24), color);

                        Act a = currentUnit.unit.acts[currentUnit.actPage * 3];
                        if (selectedAct == 1 || selectedAct == 4)
                            color = Color.White;
                        else
                            color = Color.Gray;
                        spriteBatch.Draw(t_cursor, new Vector2(144, 130), rect, color, 0, new Vector2(25, 23), 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(t_cursor, new Vector2(133, 118), new Rectangle(GetIconFact(a) * 24, 48, 24, 24), color);

                        a = currentUnit.unit.acts[currentUnit.actPage * 3 + 1];
                        if (selectedAct == 2 || selectedAct == 5)
                            color = Color.White;
                        else
                            color = Color.Gray;
                        spriteBatch.Draw(t_cursor, new Vector2(30, 215), rect, color, -MathHelper.PiOver2, new Vector2(25, 23), 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(t_cursor, new Vector2(20, 203), new Rectangle(GetIconFact(a) * 24, 48, 24, 24), color);

                        a = currentUnit.unit.acts[currentUnit.actPage * 3 + 2];
                        if (selectedAct == 3 || selectedAct == 6)
                            color = Color.White;
                        else
                            color = Color.Gray;
                        spriteBatch.Draw(t_cursor, new Vector2(258, 215), rect, color, MathHelper.PiOver2, new Vector2(25, 23), 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(t_cursor, new Vector2(246, 203), new Rectangle(GetIconFact(a) * 24, 48, 24, 24), color);
                        #endregion

                        // 行動詳細
                        if (selectedAct == 0)
                        {
                            Helper.DrawStringWithShadow("移動", new Vector2(40, 320));
                        }
                        else
                        {
                            a = currentAct;
                            if (a != null)
                            {
                                Helper.DrawStringWithShadow(a.name, new Vector2(40, 320));
                                Helper.DrawStringWithShadow(Helper.GetStringActType(a), new Vector2(60, 360));
                                Helper.DrawStringWithShadow("AP:", new Vector2(60, 400));
                                if (currentUnit.AP >= a.ap)
                                    Helper.DrawStringWithShadow(a.ap.ToString(), new Vector2(102, 400));
                                else
                                    Helper.DrawStringWithShadow(a.ap.ToString(), new Vector2(102, 400), Color.Red);
                                if (a.IsSpell)
                                {
                                    Helper.DrawStringWithShadow("SP:", new Vector2(180, 400));
                                    if (currentUnit.SP >= a.sp)
                                        Helper.DrawStringWithShadow(a.sp.ToString(), new Vector2(222, 400));
                                    else
                                        Helper.DrawStringWithShadow(a.sp.ToString(), new Vector2(222, 400), Color.Red);
                                }
                                Helper.DrawStringWithShadow(Helper.GetStringActTarget(a.target), new Vector2(60, 440));
                            }
                            else
                                Helper.DrawStringWithShadow("覚えていない", new Vector2(40, 320));
                        }

                        #region カーソル表示
                        if (actStage == 0)
                        {
                            Vector2 pos = drawMapOrigin + new Vector2(18 + currentUnit.postion.X * 48, 18 + currentUnit.postion.Y * 48);
                            float r = GetCursorFact(gameTime.TotalGameTime, 30, 8, 300);
                            rect = new Rectangle(0, 0, 24, 24);
                            spriteBatch.Draw(t_cursor, pos + new Vector2(-r, -r), rect, Color.White, 0, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_cursor, pos + new Vector2(r, -r), rect, Color.White, MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_cursor, pos + new Vector2(r, r), rect, Color.White, MathHelper.Pi, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_cursor, pos + new Vector2(-r, r), rect, Color.White, -MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                        }
                        else if (actStage == 1)
                        {
                            Vector2 pos = drawMapOrigin + new Vector2(18 + unitCursor.X * 48, 18 + unitCursor.Y * 48);
                            float r = GetCursorFact(gameTime.TotalGameTime, 45, 15, 600);
                            rect = new Rectangle(0, 24, 24, 24);
                            spriteBatch.Draw(t_cursor, pos + new Vector2(0, -r), rect, Color.White, 0, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_cursor, pos + new Vector2(r, 0), rect, Color.White, MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_cursor, pos + new Vector2(0, r), rect, Color.White, MathHelper.Pi, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_cursor, pos + new Vector2(-r, 0), rect, Color.White, -MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                        }
                        else if (actStage == 2)
                        {
                            Vector2 pos = unitCursor;
                            if (unitCursor != currentUnit.postion)
                                spriteBatch.Draw(currentUnit.unit.t_icon, drawMapOrigin + new Vector2(pos.X * 48 + 2, pos.Y * 48 + 2), new Color(128, 128, 128, 128));
                            float f;
                            bool aim = false;
                            rect = new Rectangle(24, 0, 24, 24);
                            List<UnitGadget> target = GetActTarget();
                            foreach (UnitGadget ug in target)
                            {
                                Positon p = ug.postion;
                                if (ug == currentUnit)
                                    p = unitCursor;
                                pos = drawMapOrigin + new Vector2(-6 + p.X * 48, -6 + p.Y * 48);
                                f = GetCursorFact(gameTime.TotalGameTime, 1, 0, 1200);
                                if (f >= 0.5 || targetCursor == p)
                                    spriteBatch.Draw(t_cursor, pos, new Rectangle(48, 0, 48, 48), Color.White);
                                if (p == targetCursor || a.IsTargetAll)
                                {
                                    pos += new Vector2(24, 24);
                                    f = GetCursorFact2(gameTime.TotalGameTime, 15, 8, 450);
                                    spriteBatch.Draw(t_cursor, pos + new Vector2(-f, -f), rect, Color.White, 0, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                                    spriteBatch.Draw(t_cursor, pos + new Vector2(f, -f), rect, Color.White, MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                                    spriteBatch.Draw(t_cursor, pos + new Vector2(f, f), rect, Color.White, MathHelper.Pi, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                                    spriteBatch.Draw(t_cursor, pos + new Vector2(-f, f), rect, Color.White, -MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                                    aim = true;
                                }
                            }
                            if (!aim)
                            {
                                pos = drawMapOrigin + new Vector2(18 + targetCursor.X * 48, 18 + targetCursor.Y * 48);
                                f = GetCursorFact(gameTime.TotalGameTime, 35, 8, 450);
                                spriteBatch.Draw(t_cursor, pos + new Vector2(-f, -f), rect, Color.White, 0, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                                spriteBatch.Draw(t_cursor, pos + new Vector2(f, -f), rect, Color.White, MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                                spriteBatch.Draw(t_cursor, pos + new Vector2(f, f), rect, Color.White, MathHelper.Pi, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                                spriteBatch.Draw(t_cursor, pos + new Vector2(-f, f), rect, Color.White, -MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                            }
                        }
                        #endregion
                    }
                }
                else if (actStage == 3) // ドライヴ
                {
                }
                else if (actStage == 4) // 移動
                {
                    Positon p = unitCursor;
                    while (p != currentUnit.postion)
                    {
                        spriteBatch.Draw(currentUnit.unit.t_icon,
                            new Rectangle(drawMapOrigin.X + p.X * 48 - 6, drawMapOrigin.Y + p.Y * 48 - 6, 48, 48), new Color(128, 128, 128, 128));
                        p = mapCost[p.X, p.Y].parent;
                    }
                    Vector2 pos = drawMapOrigin + new Vector2(18 + targetCursor.X * 48, 18 + targetCursor.Y * 48);
                    float r = GetCursorFact(gameTime.TotalGameTime, 35, 8, 300);
                    Rectangle rect = new Rectangle(0, 0, 24, 24);
                    spriteBatch.Draw(t_cursor, pos + new Vector2(-r, -r), rect, Color.White, 0, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(t_cursor, pos + new Vector2(r, -r), rect, Color.White, MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(t_cursor, pos + new Vector2(r, r), rect, Color.White, MathHelper.Pi, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(t_cursor, pos + new Vector2(-r, r), rect, Color.White, -MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                }
                else if (actStage == 5 || actStage == 6) // 対象表示
                {
                    float t = (float)currentTime.TotalMilliseconds / 400;
                    if (currentAct.IsTargetAll)
                    {
                        List<UnitGadget> lug = GetActTarget();
                        for (int i = 0; i < lug.Count; i++)
                        {
                            Positon tar = lug[i].postion;
                            Vector2 v = drawMapOrigin + new Vector2(48 * MathHelper.Lerp(currentUnit.postion.X, tar.X, t) - 6,
                                48 * MathHelper.Lerp(currentUnit.postion.Y, tar.Y, t) - 6);

                            spriteBatch.Draw(t_cursor, v, new Rectangle(48, 0, 48, 48), Color.White);
                        }
                    }
                    else if (targetCursor == unitCursor)
                    {
                        if (t > 0.2)
                        {
                            Vector2 v = drawMapOrigin + new Vector2(48 * currentUnit.postion.X - 6, 48 * currentUnit.postion.Y - 6);
                            spriteBatch.Draw(t_cursor, v, new Rectangle(48, 0, 48, 48), Color.White);
                        }
                    }
                    else
                    {
                        Vector2 v = drawMapOrigin + new Vector2(48 * MathHelper.Lerp(currentUnit.postion.X, targetCursor.X, t) - 6,
                            48 * MathHelper.Lerp(currentUnit.postion.Y, targetCursor.Y, t) - 6);

                        spriteBatch.Draw(t_cursor, v, new Rectangle(48, 0, 48, 48), Color.White);
                    }
                }
                else if (actStage == 7) // 行動表示
                {
                    Helper.DrawWindowBottom2(Helper.GetStringActType(currentAct), currentAct.name);
                }
                else if (actStage == 8) // 行動計算
                {
                }
                else if (actStage == 9) // 行動
                {
                    if (currentAct.TypeInt % 100 == 0)
                    {
                        UnitGadget target = GetActTarget()[BattleScene.target - 1];
                        if (!hit)
                        {
                            Vector2 pos = drawMapOrigin + new Vector2(target.postion.X * 48 - 6, target.postion.Y * 48 + 2 - (int)GetIconFact());
                            spriteBatch.Draw(t_cursor, pos, new Rectangle(152, 32, 48, 32), Color.White);
                            Helper.DrawWindowBottom1(target.unit.nickname + "は攻撃を回避した！");
                        }
                        else
                        {
                            Vector2 pos = drawMapOrigin + new Vector2(target.postion.X * 48 - 6, target.postion.Y * 48 + 2 - (int)GetIconFact());
                            if (critical)
                                spriteBatch.Draw(t_cursor, pos, new Rectangle(200, 0, 48, 32), Color.White);
                            else
                                spriteBatch.Draw(t_cursor, pos, new Rectangle(200, 32, 48, 32), Color.White);
                            Helper.DrawWindowBottom1(target.unit.nickname + "に" + damage + "のダメージ！");
                        }
                    }
                }
                else if (actStage == 18) // SP減少
                {
                    if (currentTime.TotalMilliseconds < 1500)
                    {
                        Vector2 v = drawMapOrigin + new Vector2(currentUnit.postion.X * 48 + 18, currentUnit.postion.Y * 48 + 18);
                        float t = (float)currentTime.TotalMilliseconds / 1500;
                        float p = 0;
                        float d = 0;
                        float s = 0;
                        float a = 4 * (1 - t);
                        for (int i = 0; i < 12; i++)
                        {
                            p += MathHelper.TwoPi / 12;
                            d += 13;
                            s += 0.367f;
                            float dd = 10 + (15 + d % 5) * t;
                            float ss = (0.3f + s % 0.6f) * t;
                            Vector2 vv = v + dd * new Vector2((float)Math.Cos(p), (float)Math.Sin(p));
                            spriteBatch.Draw(t_cursor, vv, new Rectangle(184, 104, 16, 16), new Color(a, a, a, a), 0, new Vector2(8, 8), ss, SpriteEffects.None, 0);
                        }
                    }
                }
                else if (actStage == 19) // ドライヴ解除
                {
                    Helper.DrawWindowBottom1("ドライヴが解除された！");
                }
                else if (actStage == 20) // SP溜め
                {
                    if (currentTime.TotalMilliseconds < 2000)
                    {
                        Vector2 v = drawMapOrigin + new Vector2(currentUnit.postion.X * 48 + 18, currentUnit.postion.Y * 48 + 18);
                        float t = (float)(currentTime.TotalMilliseconds / 1000) % 1;
                        float p = 0;
                        float d = 0;
                        float s = 0;
                        float a = t * 4;
                        for (int i = 0; i < 12; i++)
                        {
                            p += MathHelper.TwoPi / 12;
                            d += 13;
                            s += 0.367f;
                            float dd = 10 + (15 + d % 5) * (1 - t);
                            float ss = (0.3f + s % 0.6f) * (1 - t);
                            Vector2 vv = v + dd * new Vector2((float)Math.Cos(p), (float)Math.Sin(p));
                            spriteBatch.Draw(t_cursor, vv, new Rectangle(184, 104, 16, 16), new Color(a, a, a, a), 0, new Vector2(8, 8), ss, SpriteEffects.None, 0);
                        }
                    }

                    Helper.DrawWindowBottom1("SPが" + (currentUnit.AP - usingAP) + "ポイント増加した！");
                }
                else
                {
                    Vector2 pos = drawMapOrigin + new Vector2(currentUnit.postion.X * 48 - 6, currentUnit.postion.Y * 48 + 2 - (int)GetIconFact());
                    spriteBatch.Draw(t_cursor, pos, new Rectangle(152, 0, 48, 32), Color.White);
                    if (actStage == 21)
                        Helper.DrawWindowBottom1("この行動はまだ実装されていない！");
                    else if (actStage == 22)
                        Helper.DrawWindowBottom1(currentUnit.unit.nickname + "は行動に失敗した！");
                    else if (actStage == 23)
                    {
                        switch (currentAct.type)
                        {
                            case ActType.ClearMinusSympton:
                            case ActType.ClearPlusSympton:
                                Helper.DrawWindowBottom1("クリアする症状がなかった！");
                                break;
                            case ActType.ClearTrap:
                                Helper.DrawWindowBottom1("クリアするトラップがなかった！");
                                break;
                            case ActType.ClearCrystal:
                                Helper.DrawWindowBottom1("消去する結晶効果がなかった！");
                                break;
                            case ActType.SPDrain:
                                Helper.DrawWindowBottom1("奪い取るSPがなかった！");
                                break;
                            case ActType.LevelDrain:
                                Helper.DrawWindowBottom1("奪い取るレベルがなかった！");
                                break;
                            case ActType.Booster:
                            case ActType.Scope:
                            case ActType.DualBoost:
                            case ActType.Charge:
                                Helper.DrawWindowBottom1("このスキルは装備するだけで効果を発揮する！");
                                break;
                        }
                    }
                }

                if (currentUnit != null && actStage <= 5)
                {
                    // 地形情報
                    MapTip mt = map[targetCursor.X, targetCursor.Y];
                    Helper.DrawStringWithShadow(Helper.GetStringCrystalEffect(mt.effect.sympton), new Vector2(303, 440));
                    Helper.DrawStringWithShadow(Helper.GetStringTerrain(mt.terrain), new Vector2(570, 440));
                    Helper.DrawStringWithShadow(Helper.GetStringAffinity(currentUnit.unit.affinity[(int)mt.terrain]), new Vector2(680, 440));
                }
                #endregion
            }
            else if (!battleStart)
            {
                #region 状態確認
                if (actStage == 0 || actStage == 1)
                {
                    if (!IsMapMoving())
                    {
                        Positon p = new Positon();
                        if (actStage == 0)
                            p.X = 23;
                        else
                            p.X = 431;
                        if (selectedAct == 0)
                            p.Y = 10;
                        else
                            p.Y = 10 + selectedAct * 50;
                        spriteBatch.Draw(tw, new Rectangle(p.X, p.Y, 250, 40), new Color(0, 0, 0, 128));
                        Helper.DrawWindowBottom1("startボタンで戦闘開始");
                    }

                    Helper.DrawStringWithShadow("戦闘開始", drawMapOrigin + new Vector2(-205, -2));
                    for (int i = 0; i < allyUnitGadget.Count; i++)
                    {
                        Helper.DrawStringWithShadow(allyUnitGadget[i].unit.name, drawMapOrigin + new Vector2(-280, 50 + i * 50));
                    }
                    for (int i = 0; i < enemyUnitGadget.Count; i++)
                    {
                        Helper.DrawStringWithShadow(enemyUnitGadget[i].unit.name, drawMapOrigin + new Vector2(425, 50 + i * 50));
                    }
                    
                }
                if (actStage >= 2)
                {
                    UnitGadget ug;
                    int x1, x2;
                    SpriteEffects se;
                    if (actStage == 2)
                    {
                        ug = allyUnitGadget[selectedAct - 1];
                        x1 = 0;
                        x2 = 360;
                        se = SpriteEffects.None;
                    }
                    else
                    {
                        ug = enemyUnitGadget[selectedAct - 1];
                        x1 = 720 - 144 * 2;
                        x2 = 120;
                        se = SpriteEffects.FlipHorizontally;
                    }
                    DrawUnit(ug, gameTime);

                    spriteBatch.Draw(t_cursor, new Vector2(x1 + 10, 10), new Rectangle((int)ug.unit.type * 32, 72, 32, 32), Color.White);
                    if (ug.drive)
                        spriteBatch.Draw(t_cursor, new Vector2(x1 + 26, 26), new Rectangle((int)ug.unit.type2 * 16, 104, 16, 16), Color.White);
                    Helper.DrawStringWithShadow(ug.unit.name, new Vector2(x1 + 52, 10));
                    Helper.DrawStringWithShadow("HP:", new Vector2(x1 + 10, 39));
                    str = ug.HP.ToString();
                    Helper.DrawStringWithShadow(str, new Vector2(x1 + 95 - font.MeasureString(str).X, 39));
                    spriteBatch.Draw(tw, new Rectangle(x1 + 106, 48, 160, 18), Color.White);
                    spriteBatch.Draw(tw, new Rectangle(x1 + 108, 51, 156, 12), Color.Black);
                    spriteBatch.Draw(tw, new Rectangle(x1 + 108, 51, 156 * ug.HP / ug.HPmax, 12), Color.LimeGreen);
                    Helper.DrawStringWithShadow("SP:", new Vector2(x1 + 10, 63));
                    str = ug.SP.ToString();
                    Helper.DrawStringWithShadow(str, new Vector2(x1 + 95 - font.MeasureString(str).X, 63));
                    spriteBatch.Draw(tw, new Rectangle(x1 + 106, 72, 160, 18), Color.White);
                    spriteBatch.Draw(tw, new Rectangle(x1 + 108, 75, 156, 12), Color.Black);
                    spriteBatch.Draw(tw, new Rectangle(x1 + 108, 75, 156 * ug.SP / ug.SPmax, 12), Color.SkyBlue);
                    Helper.DrawStringWithShadow("AP: 0/", new Vector2(x1 + 10, 87));
                    str = ug.AP.ToString();
                    Helper.DrawStringWithShadow(str, new Vector2(x1 + 122 - font.MeasureString(str).X, 87));

                    for (int i = 0; i < 6; i++)
                    {
                        Act a = ug.unit.acts[i];
                        if (a != null)
                        {
                            spriteBatch.Draw(t_cursor, new Vector2(x2, 40 + i * 70), new Rectangle(GetIconFact(a) * 24, 48, 24, 24), Color.White);
                            Helper.DrawStringWithShadow(a.name, new Vector2(x2 + 40, 35 + i * 70));
                            Helper.DrawStringWithShadow(Helper.GetStringActType(a), new Vector2(x2 + 70, 65 + i * 70));
                        }
                        else
                            spriteBatch.Draw(t_cursor, new Vector2(x2, 40 + i * 70), new Rectangle(4 * 24, 48, 24, 24), Color.White);
                    }
                }
                #endregion
            }
            
            spriteBatch.End();
        }

        static void DrawUnit(UnitGadget ug, GameTime gameTime)
        {
            int x;
            SpriteEffects se;
            if (ug.ff == FrendOfFoe.Ally)
            {
                x = 0;
                se = SpriteEffects.None;
            }
            else
            {
                x = 720 - 144 * 2;
                se = SpriteEffects.FlipHorizontally;
            }
            spriteBatch.Draw(t_shadow, new Vector2(x + 96, 320), Color.White);
            if (ug.drive)
            {
                double d;
                float f;
                Color c;
                d = gameTime.TotalGameTime.TotalMilliseconds % 1000 * 0.001;
                f = (float)(1 - d);
                c = new Color(f, f, f, f);
                spriteBatch.Draw(ug.unit.t_battle, new Vector2(x + 144, 315 + GetUnitFact(gameTime.TotalGameTime, 4, 2000)),
                    null, c, 0, ug.unit.t_battle_origin, (float)(2 + d / 2), se, 0);

                d = (gameTime.TotalGameTime.TotalMilliseconds + 500) % 1000 * 0.001;
                f = (float)(1 - d);
                c = new Color(f, f, f, f);
                spriteBatch.Draw(ug.unit.t_battle, new Vector2(x + 144, 315 + GetUnitFact(gameTime.TotalGameTime, 4, 2000)),
                    null, c, 0, ug.unit.t_battle_origin, (float)(2 + d / 2), se, 0);
            }
            spriteBatch.Draw(ug.unit.t_battle, new Vector2(x + 144, 315 + GetUnitFact(gameTime.TotalGameTime, 4, 2000)),
                null, Color.White, 0, ug.unit.t_battle_origin, 2, se, 0);
        }

        static void SetTurnOrder()
        {
            SortedDictionary<int, UnitGadget> order = new SortedDictionary<int, UnitGadget>(new CaseInsensitiveReverseIntegerComparer());
            foreach (UnitGadget ug in allUnitGadget)
            {
                int d = Game.rand.Next(11) - 5;
                order.Add(ug.Parameter.speed + d + (ug.leader ? 60 : 0), ug);
            }
            foreach (UnitGadget ug in order.Values)
            {
                unitOrder.Enqueue(ug);
            }
        }

        public static void MapSet(string[] data, string[] crystal)
        {
            for (int i = 0; i < MapSize; i++)
                for (int j = 0; j < MapSize; j++)
                {
                    switch (data[i][j])
                    {
                        case 'P':
                            map[i, j].terrain = Terrain.Plain;
                            break;
                        case 'F':
                            map[i, j].terrain = Terrain.Forest;
                            break;
                        case 'M':
                            map[i, j].terrain = Terrain.Mountain;
                            break;
                        case 'W':
                            map[i, j].terrain = Terrain.Waterside;
                            break;
                        case 'I':
                            map[i, j].terrain = Terrain.Indoor;
                            break;
                        case 'R':
                            map[i, j].terrain = Terrain.Red_hot;
                            break;
                        case 'S':
                            map[i, j].terrain = Terrain.Sanctuary;
                            break;
                        case 'D':
                            map[i, j].terrain = Terrain.Miasma;
                            break;
                        case 'B':
                        default:
                            map[i, j].terrain = Terrain.Banned;
                            break;
                    }

                    switch (crystal[i][j])
                    {
                        case 'N':
                        default:
                            map[i, j].effect = new Condition<CrystalEffect>(CrystalEffect.None, 0, 0);
                            break;
                    }
                }

            graphics.SetRenderTarget(r_map);
            spriteBatch.Begin();
            for (int i = 0; i < MapSize; i++)
                for (int j = 0; j < MapSize; j++)
                    spriteBatch.Draw(t_map, new Vector2(j * 48, i * 48), new Rectangle((int)map[i, j].terrain * 48, 0, 48, 48), Color.White);
            spriteBatch.End();
            graphics.SetRenderTarget(null);
        }

        static int GetMapMoveGoal()
        {
            if (battleStart)
            {
                if (actStage < 0)
                {
                    return 156;
                }
                else if (currentUnit == null)
                {
                    return drawMapOrigin.X;
                }
                else if (currentUnit.ff == FrendOfFoe.Ally)
                {
                    return 303;
                }
                else
                {
                    return 6;
                }
            }
            else
            {
                if (actStage < 0)
                {
                    return 156;
                }
                else if (actStage == 0 || actStage == 2)
                {
                    return 303;
                }
                else
                {
                    return 6;
                }
            }
        }

        static bool IsMapMoving()
        {
            return drawMapOrigin.X != GetMapMoveGoal();
        }

        static void CalcMoveCost()
        {
            Positon p = currentUnit.postion;
            List<Positon> lp;
            // コストマップの初期設定
            for (int i = 0; i < MapSize; i++)
                for (int j = 0; j < MapSize; j++)
                {
                    if (map[i, j].terrain != Terrain.Banned)
                        mapCost[i, j].passable = true;
                    else
                        mapCost[i, j].passable = false;
                    mapCost[i, j].cost = 99;
                    mapCost[i, j].zocCost = 0;
                }
            foreach (UnitGadget ug in allyUnitGadget)
                if (ug != currentUnit)
                {
                    mapCost[ug.postion.X, ug.postion.Y].passable = false;
                    if (currentUnit.ff == FrendOfFoe.Enemy)
                    {
                        lp = GetNeighbor(ug.postion, true);
                        foreach (Positon pp in lp)
                            mapCost[pp.X, pp.Y].zocCost++;
                    }
                }
            foreach (UnitGadget ug in enemyUnitGadget)
                if (ug != currentUnit)
                {
                    mapCost[ug.postion.X, ug.postion.Y].passable = false;
                    if (currentUnit.ff == FrendOfFoe.Ally)
                    {
                        lp = GetNeighbor(ug.postion, true);
                        foreach (Positon pp in lp)
                            mapCost[pp.X, pp.Y].zocCost++;
                    }
                }

            mapCost[p.X, p.Y].cost = 0;
            mapCost[p.X, p.Y].parent = p;
            lp = new List<Positon>();
            lp.Add(p);
            while (lp.Count != 0)
            {
                lp.Sort(delegate(Positon p1, Positon p2) { return mapCost[p1.X, p1.Y].cost - mapCost[p2.X, p2.Y].cost; });
                p = lp[0];
                lp.RemoveAt(0);
                foreach (Positon pp in GetNeighbor(p))
                {
                    int cost = GetTerrainCost(pp);
                    if (currentUnit.symptonPlus.sympton == SymptonPlus.Swift)
                        cost = (int)Math.Round(cost * 0.5);
                    if (currentUnit.symptonMinus.sympton == SymptonMinus.Restraint)
                        cost += currentUnit.symptonMinus.power;
                    if (Positon.Distance(p, pp) > 1)
                        cost = (int)(cost * 1.5);
                    cost += mapCost[p.X, p.Y].cost + mapCost[pp.X, pp.Y].zocCost;

                    if (cost < mapCost[pp.X, pp.Y].cost)
                    {
                        mapCost[pp.X, pp.Y].cost = cost;
                        mapCost[pp.X, pp.Y].parent = p;
                        lp.Add(pp);
                    }
                }
            }
        }

        static Positon GetMoveRoute(Positon goal)
        {
            while (mapCost[goal.X, goal.Y].parent != currentUnit.postion)
                goal = mapCost[goal.X, goal.Y].parent;

            return goal;
        }

        static List<Positon> GetNeighbor(Positon p, bool only4 = false)
        {
            List<Positon> lp = new List<Positon>();

            for (int i = p.X - 1; i <= p.X + 1; i++)
            {
                if (i < 0 || i >= MapSize)
                    continue;
                for (int j = p.Y - 1; j <= p.Y + 1; j++)
                {
                    if (j < 0 || j >= MapSize)
                        continue;
                    if (mapCost[i, j].passable && (!only4 || i == p.X || j == p.Y))
                        lp.Add(new Positon(i, j));
                }
            }

            return lp;
        }

        static int GetTerrainCost(Positon pos)
        {
            Terrain tera = map[pos.X, pos.Y].terrain;
            if (tera == Terrain.Banned)
                return 99;
            switch (GetAffinity(currentUnit, pos))
            {
                case Affinity.Best:
                    return 4;
                case Affinity.Good:
                    switch (tera)
                    {
                        case Terrain.Plain:
                        case Terrain.Mountain:
                        case Terrain.Indoor:
                        case Terrain.Sanctuary:
                            return 6;
                        default:
                            return 4;
                    }
                case Affinity.Normal:
                    return 8;
                case Affinity.Bad:
                    switch (tera)
                    {
                        case Terrain.Plain:
                        case Terrain.Mountain:
                        case Terrain.Indoor:
                        case Terrain.Sanctuary:
                            return 10;
                        default:
                            return 12;
                    }
                default:
                    return 99;
            }
        }

        static List<UnitGadget> GetActTarget()
        {
            List<UnitGadget> lug = new List<UnitGadget>();
            Act a = currentAct;
            Positon p = currentUnit.postion;
            currentUnit.postion = unitCursor;

            if (a.target == ActTarget.Field || a.target == ActTarget.Space || a.target == ActTarget.Equip)
            {
                lug.Add(currentUnit);
                currentUnit.postion = p;
                return lug;
            }
            if (a.type == ActType.Guard || a.type == ActType.LessGuard || a.type == ActType.CoverCounter)
                lug.Add(currentUnit);

            if (((a.target == ActTarget.Enemy1 || a.target == ActTarget.EnemyAll) && currentUnit.ff == FrendOfFoe.Ally)
                || ((a.target == ActTarget.Ally1 || a.target == ActTarget.AllyAll) && currentUnit.ff == FrendOfFoe.Enemy)
                || a.target == ActTarget.AllyEnemy1)
            {
                for (int i = 0; i < enemyUnitGadget.Count; i++)
                {
                    float d = Positon.Distance(unitCursor, enemyUnitGadget[i].postion);
                    if (d >= a.rangeMin && d <= a.rangeMax)
                        lug.Add(enemyUnitGadget[i]);
                }
            }
            if (((a.target == ActTarget.Enemy1 || a.target == ActTarget.EnemyAll) && currentUnit.ff == FrendOfFoe.Enemy)
                || ((a.target == ActTarget.Ally1 || a.target == ActTarget.AllyAll) && currentUnit.ff == FrendOfFoe.Ally)
                || a.target == ActTarget.AllyEnemy1)
            {
                for (int i = 0; i < allyUnitGadget.Count; i++)
                {
                    float d = Positon.Distance(unitCursor, allyUnitGadget[i].postion);
                    if (d >= a.rangeMin && d <= a.rangeMax)
                        lug.Add(allyUnitGadget[i]);
                }
            }
            currentUnit.postion = p;
            

            return lug;
        }

        static bool IsCanAct()
        {
            return selectedAct == 0 || (currentAct != null && currentUnit.AP >= GetUsingAP(false) &&
                (!currentAct.IsSpell || currentUnit.SP >= currentAct.sp && (currentAct.lastSpell? drive: true)));
        }

        static int GetUsingAP(bool mapcost)
        {
            if (currentUnit == null)
                return 0;
            return ((!currentUnit.drive && drive) ? 6 : 0) + (selectedAct == 0 || currentAct == null ? 0 : currentAct.ap)
                + (mapcost ? mapCost[unitCursor.X, unitCursor.Y].cost : 0);
        }

        static void CalcAttackResult(UnitGadget target)
        {
            Act a = currentAct;
            if (a.TypeInt >= 200 || a.TypeInt % 100 != 0)
                return;

            MapTip tip = GetTip(currentUnit.postion);
            hit = false;
            critical = false;
            guard = false;
            damage = 0;
            force = 0;

            // 自分パラメータ補正
            // レベル
            int level = currentUnit.unit.level;
            level += (currentUnit.level - level) / 2;
            // 成功
            int success = a.success + (int)(level * 2 * a.proficiency);
            if (a.TypeInt < 100)
                success += currentUnit.Parameter.close;
            else
                success += currentUnit.Parameter.far;
            success += (3 - (int)GetAffinity(currentUnit, currentUnit.postion)) * 10;
            if (!moved)
            {
                foreach (Act act in currentUnit.unit.acts)
                    if (act != null && (act.type == ActType.Scope || act.type == ActType.DualBoost))
                        success += act.success;
            }
            if (a.TypeInt % 100 == 0 && currentUnit.IsType(Type.Technic))
                success += currentUnit.SP / 4;
            if (tip.effect.sympton == CrystalEffect.HitUp)
                success += tip.effect.power;
            if (currentUnit.symptonPlus.sympton == SymptonPlus.Concentrate)
                success += currentUnit.symptonPlus.power;
            if (currentUnit.symptonMinus.sympton == SymptonMinus.Distract)
                success -= currentUnit.symptonMinus.power;
            // 威力
            int power = a.power;
            if (a.TypeInt % 100 == 0 && a.IsHaveAbility(ActAbility.Rush))
                power += currentUnit.Parameter.speed / 2;
            if (moved)
            {
                foreach (Act act in currentUnit.unit.acts)
                    if (act != null && (act.type == ActType.Booster || act.type == ActType.DualBoost))
                        power += act.power;
            }
            if (a.TypeInt % 100 == 0 && currentUnit.IsType(Type.Power))
                power += currentUnit.SP / 5;

            // 敵パラメータ補正
            // レベル
            int elevel = target.unit.level;
            elevel += (target.level - elevel) / 2;
            // 回避
            int avoid = target.Parameter.avoid + elevel * 2;
            avoid += (3 - (int)GetAffinity(target, target.postion)) * 5;
            if (target.symptonPlus.sympton == SymptonPlus.Concentrate)
                avoid += target.symptonPlus.power;
            // 防御
            int defence = target.Parameter.defence + elevel / 5;

            // 命中判定
            if (target.dedodge || ((target.IsType(Type.Fortune) && Helper.GetProbability(success - avoid, 150))
                || (!target.IsType(Type.Fortune) && Helper.GetProbability(success - avoid))))
                hit = true;

            // 防御判定
            if (!target.deguard && Helper.GetProbability(defence * 2))
                guard = true;

            // クリティカル判定
            if (a.TypeInt % 100 == 0 && (target.symptonMinus.sympton == SymptonMinus.Stop
                || target.dedodge && target.deguard
                || (a.IsHaveAbility(ActAbility.Fast) && (a.TypeInt < 100 && Helper.GetProbability(1d * success / avoid - 5)
                    || (a.TypeInt >= 100 && Helper.GetProbability(1d * success / avoid - 2))))
                || Helper.GetProbability(1d * success / avoid - 10)))
            {
                hit = true;
                critical = true;
                guard = false;
            }

            if (hit)
            {
                // ダメージ計算
                tip = GetTip(target.postion);
                int tmp = (int)(success / 5 * GetRandomFact());
                if (!critical && target.dedodge)
                    tmp -= avoid / 4;
                if (guard)
                    tmp -= defence / 3;
                if (tmp < 0)
                    tmp = 0;
                damage = tmp + power;
                if (target.IsType(Type.Guard))
                    damage -= 10;
                if (target.leader)
                {
                    int n = target.ff == FrendOfFoe.Ally ? GetFightableNum(allyUnitGadget) : GetFightableNum(enemyUnitGadget);
                    n--;
                    damage -= n * 5;
                }
                if (tip.effect.sympton == CrystalEffect.DamageUp)
                    damage += tip.effect.power;
                else if (tip.effect.sympton == CrystalEffect.DamageDown)
                    damage -= tip.effect.power;
                if (a.IsHaveAbility(ActAbility.Diffuse))
                    damage = (int)(damage * GetRandomFact(0.5));
                if (damage < 1)
                    damage = 1;

                // ふっとび量計算
                if (a.IsHaveAbility(ActAbility.Shock) || a.IsHaveAbility(ActAbility.Vacuum))
                    force = 1f * damage / 20;
            }
        }

        static void TurnEnd()
        {
            if (!currentUnit.drive)
                AddSP(currentUnit, currentUnit.AP - usingAP);
            currentUnit.AP = 0;
            actStage = 0;
            currentUnit = null;
        }

        static float GetCursorFact(TimeSpan time, float rad, float min, float cycle)
        {
            double t = time.TotalMilliseconds % cycle;
            if (t <= cycle / 2)
            {
                return min + rad * (float)(t / (cycle / 2));
            }
            else
            {
                return min + rad * (float)((cycle - t) / (cycle / 2));
            }
            
        }

        static float GetCursorFact2(TimeSpan time, float rad, float min, float cycle)
        {
            double t = time.TotalMilliseconds % cycle;
            return min + rad * (float)(t / cycle);

        }

        static float GetUnitFact(TimeSpan time, float rad, float cycle)
        {
            return rad * (float)Math.Sin(MathHelper.TwoPi * time.TotalMilliseconds / cycle);
        }

        static double GetIconFact()
        {
            double d = currentTime.TotalMilliseconds * 0.4;
            if (d > 40)
                d = 0;
            else if (d > 20)
                d = 40 - d;
            return d;
        }

        static int GetIconFact(Act act)
        {
            if (act == null)
                return 5;

            if (act.IsPassive)
                return 2;

            if (act.IsSpell)
            {
                if (act.lastSpell)
                    return 4;
                return 3;
            }

            return 1;
        }

        static MapTip GetTip(Positon pos)
        {
            return map[pos.X, pos.Y];
        }

        static Affinity GetAffinity(UnitGadget ug, Positon pos)
        {
            MapTip tip = GetTip(pos);
            int affinity = (int)ug.unit.affinity[(int)tip.terrain];

            if (tip.effect.sympton == CrystalEffect.AffinityDown)
            {
                affinity += tip.effect.power;
                if (affinity > 3)
                    affinity = 3;
            }
            return (Affinity)affinity;
        }

        static int GetFightableNum(List<UnitGadget> lug)
        {
            int n = 0;
            foreach (UnitGadget ug in lug)
                if (ug.HP > 0)
                    n++;

            return n;
        }

        static double GetRandomFact(double width = 0.2)
        {
            double d = Game.rand.NextDouble() * (width * 2);
            return 1 + d - width;
        }

        static bool IsTargetAlly(UnitGadget target)
        {
            if (currentUnit.ff == FrendOfFoe.Ally)
                return target.ff == FrendOfFoe.Ally;
            else
                return target.ff == FrendOfFoe.Enemy;
        }

        static void AddHP(UnitGadget ug, int fact)
        {
            ug.HP += fact;
            if (ug.HP > ug.HPmax)
                ug.HP = ug.HPmax;
            else if (ug.HP < 0)
                ug.HP = 0;
        }

        static void AddSP(UnitGadget ug, int fact)
        {
            ug.SP += fact;
            if (ug.SP > ug.SPmax)
                ug.SP = ug.SPmax;
            else if (ug.SP < 0)
                ug.SP = 0;
        }

        static void AddLevel(UnitGadget ug, int fact)
        {
            ug.level += fact;
            if (ug.level > 100)
                ug.level = 100;
            else if (ug.SP < 0)
                ug.level = 0;
        }

        static Act currentAct
        {
            get { return selectedAct > 0? currentUnit.unit.acts[selectedAct - 1]: null; }
        }
    }

    class CaseInsensitiveReverseIntegerComparer : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return (y - x);
        }
    }
}
