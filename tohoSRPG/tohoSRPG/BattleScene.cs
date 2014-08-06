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

        static Terrain[,] map;
        static Condition<FieldEffect> crystal_base;
        static Condition<FieldEffect> crystal_face;
        struct A_Serch
        {
            public bool movable;
            public int cost;
            public int zocCost;
            public Positon parent;
        }
        static A_Serch[,] mapCost;

        public static Texture2D t_map;
        static RenderTarget2D r_map;
        static RenderTarget2D r_map_2;
        static float mapWhite = 0;
        static Texture2D[] t_bg;
        static int bgNum;
        static float bgBlack = 0;
        static RenderTarget2D r_bgMove;
        static float bgMoveOffset;
        static Positon mapOrigin;
        static Texture2D t_shadow;
        static Texture2D t_icon1;
        static Texture2D t_icon2;
        static Texture2D t_icon3;
        static Texture2D t_icon4;

        enum ActState
        {
            /// <summary>
            /// 対峙
            /// </summary>
            Confront,
            /// <summary>
            /// 味方リスト
            /// </summary>
            ListAlly,
            /// <summary>
            /// 敵リスト
            /// </summary>
            ListEnemy,
            /// <summary>
            /// 味方ユニット詳細
            /// </summary>
            ShowAlly,
            /// <summary>
            /// 敵ユニット詳細
            /// </summary>
            ShowEnemy,
            /// <summary>
            /// 背景黒塗り
            /// </summary>
            BackBlackout,
            /// <summary>
            /// ターン開始
            /// </summary>
            TurnStart,
            /// <summary>
            /// APチャージ
            /// </summary>
            APCharge,
            /// <summary>
            /// フィールド症状発動(消滅)
            /// </summary>
            CrystalEffect,
            /// <summary>
            /// プラス症状消滅
            /// </summary>
            DisPlus,
            /// <summary>
            /// マイナス症状消滅
            /// </summary>
            DisMinus,
            /// <summary>
            /// 活気発動
            /// </summary>
            SymCharge,
            /// <summary>
            /// 再生発動
            /// </summary>
            SymHeal,
            /// <summary>
            /// 継続発動
            /// </summary>
            SymDamage,
            /// <summary>
            /// 症状消滅2
            /// </summary>
            Dis2,
            /// <summary>
            /// 行動選択
            /// </summary>
            SelectAct,
            /// <summary>
            /// 移動選択
            /// </summary>
            SelectMove,
            /// <summary>
            /// 対象選択
            /// </summary>
            SelectTarget,
            /// <summary>
            /// ドライヴ
            /// </summary>
            Drive,
            /// <summary>
            /// 移動
            /// </summary>
            Move,
            MoveEnd,
            /// <summary>
            /// 対象表示
            /// </summary>
            TargetCursor,
            /// <summary>
            /// 行動表示
            /// </summary>
            ShowActExplain,
            /// <summary>
            /// 行動計算
            /// </summary>
            CalcAct,
            /// <summary>
            /// 攻撃回復
            /// </summary>
            Attack_Heal,
            Heal2,
            /// <summary>
            /// 付加飛ばし
            /// </summary>
            AddSympSub,
            /// <summary>
            /// 付加
            /// </summary>
            AddSymp,
            AddSympFromAttack,
            /// <summary>
            /// プラスマイナストラップクリア
            /// </summary>
            ClearSymp,
            ClearResult,
            /// <summary>
            /// SP、レベルドレイン
            /// </summary>
            Drain,
            /// <summary>
            /// トラップ設置
            /// </summary>
            SetTrap,
            /// <summary>
            /// フィールド効果解放
            /// </summary>
            SetCrystal,
            /// <summary>
            /// 索敵隠蔽能力アップ
            /// </summary>
            Search_Hide,
            /// <summary>
            /// 援護、構え
            /// </summary>
            Cover_Stance,
            /// <summary>
            /// 援護発生
            /// </summary>
            CoverRun,
            /// <summary>
            /// 空間移動
            /// </summary>
            TransSpace1,
            TransSpace2,
            /// <summary>
            /// 敵撃破
            /// </summary>
            DefeatEnemy,
            /// <summary>
            /// 自滅
            /// </summary>
            Defeated,
            /// <summary>
            /// 連続行動発動
            /// </summary>
            SymActAgain,
            /// <summary>
            /// ドライヴ中のDP減少
            /// </summary>
            LeakDP,
            /// <summary>
            /// DP切れによるドライヴ解除
            /// </summary>
            DriveOff,
            /// <summary>
            /// SP溜め
            /// </summary>
            SPCharge,
            /// <summary>
            /// 行動失敗(確率)
            /// </summary>
            Failure1,
            /// <summary>
            /// 行動失敗(条件不満)
            /// </summary>
            Failure2,
            /// <summary>
            /// トラップ発動
            /// </summary>
            TakeTrap,
            /// <summary>
            /// トラップ解除
            /// </summary>
            DisarmTrap,
            /// <summary>
            /// 強制移動
            /// </summary>
            Force,
            /// <summary>
            /// 吸収精神犠牲
            /// </summary>
            HPSPgain,
            /// <summary>
            /// 勝利
            /// </summary>
            Win,
            /// <summary>
            /// 敗北
            /// </summary>
            Lose
        }
        static ActState actState;

        static int turn;
        static List<UnitGadget> unitOrder;
        static UnitGadget currentUnit;
        static int selectedAct;
        static int usingAP;
        static int aimedEnemy;
        static Positon unitCursor;
        static Positon targetCursor;
        static bool covered;
        static bool countered;

        static int timeSpan;

        static bool drive;
        static bool moved;

        static List<UnitGadget> targetList;
        static int targetNo;
        static UnitGadget target;
        static UnitGadget target_old;
        static int attackStage = 0;

        struct CalcResult
        {
            public bool hit;
            public bool guard;
            public bool critical;
            public int damage;
            public int sympTurn;
            public int sympPower;
            public Positon forceDir;
            public Positon forcedPos;
        }
        static CalcResult[] calcResult;

        public static void Init(GraphicsDevice g, SpriteBatch s, ContentManager c)
        {
            graphics = g;
            spriteBatch = s;
            content = c;
            font = content.Load<SpriteFont>("font\\CommonFont");
            e_dot = content.Load<Effect>("effect\\dot");
            e_desaturate = content.Load<Effect>("effect\\desaturate");
            t_shadow = content.Load<Texture2D>("img\\battle\\shadow");
            t_icon1 = content.Load<Texture2D>("img\\system\\icon001");
            t_icon2 = content.Load<Texture2D>("img\\system\\icon002");
            t_icon3 = content.Load<Texture2D>("img\\system\\icon003");
            t_icon4 = content.Load<Texture2D>("img\\system\\icon004");

            t_map = content.Load<Texture2D>("img\\maptip\\map");
            t_bg = new Texture2D[10];
            t_bg[0] = content.Load<Texture2D>("img\\bg\\bgPlain");
            t_bg[1] = content.Load<Texture2D>("img\\bg\\bgSanctuary");
            t_bg[2] = content.Load<Texture2D>("img\\bg\\bgWaterside");
            t_bg[3] = content.Load<Texture2D>("img\\bg\\bgMountain");
            t_bg[4] = content.Load<Texture2D>("img\\bg\\bgForest");
            t_bg[5] = content.Load<Texture2D>("img\\bg\\bgMiasma");
            t_bg[6] = content.Load<Texture2D>("img\\bg\\bgRed_hot");
            t_bg[7] = content.Load<Texture2D>("img\\bg\\bgIndoor");
            t_bg[8] = content.Load<Texture2D>("img\\bg\\bgCrystal");
            t_bg[9] = content.Load<Texture2D>("img\\bg\\bg000");

            map = new Terrain[MapSize, MapSize];
            mapCost = new A_Serch[MapSize, MapSize];

            allyUnit = new List<Unit>();
            allyUnitGadget = new List<UnitGadget>();
            enemyUnitGadget = new List<UnitGadget>();

            r_map = new RenderTarget2D(graphics, 576, 576);
            r_map_2 = new RenderTarget2D(graphics, 576, 576);

            unitOrder = new List<UnitGadget>();

            calcResult = new CalcResult[10];

            ListsReset();
        }

        public static void ListsReset()
        {
            allyUnitGadget.Clear();
            enemyUnitGadget.Clear();
            mapOrigin = new Positon(218, 10);
            actState = ActState.Confront;
            selectedAct = 0;
            turn = 0;
            bgNum = 9;
            bgBlack = 0;
            unitOrder.Clear();
            currentTime = TimeSpan.Zero;
        }

        public static void Update(GameTime gameTime)
        {
            currentTime += gameTime.ElapsedGameTime;
            if (!IsMapMoving())
            {
                if (battleStart)
                {
                    #region 戦闘行為

                    #region ターン開始
                    if (actState >= ActState.Win)
                    {
                        if (currentTime >= TimeSpan.FromMilliseconds(5000))
                        {
                            if (actState == ActState.Win)
                                ConfrontScene.State = 3;
                            else
                                ConfrontScene.State = 4;
                            GameBody.ChangeScene(Scene.Confront);
                        }
                        return;
                    }
                    if (currentUnit == null && unitOrder.Count == 0)
                    {
                        actState = ActState.TurnStart;
                        SetTurnOrder();
                        turn++;
                        foreach (UnitGadget ug in allUnitGadget)
                        {
                            if (ug.dead)
                                continue;
                            int ap = ug.GetAP();
                            if(ug.IsType(UnitType.Apparition)) // 幻属性補正
                                ap += ug.SP / 20;
                            if (GetTip(ug.postion) == Terrain.Crystal) // 結晶地形ボーナス
                            {
                                ap += 6;
                                AddSP(ug, 10);
                            }
                            if (IsTakeFieldEffect(FieldEffect.APUp, ug)) // フィールド効果：APアップ
                                ap += Crystal.power;
                            else if (IsTakeFieldEffect(FieldEffect.APDown, ug)) // フィールド効果：APダウン
                                ap -= Crystal.power;
                            if (ap > 99)
                                ap = 99;
                            else if (ap < 0)
                                ap = 0;
                            ug.AP = ap;
                        }
                        currentTime = TimeSpan.Zero;
                    }
                    if (actState == ActState.TurnStart && currentTime >= TimeSpan.FromMilliseconds(1000))
                    {
                        currentTime = TimeSpan.Zero;
                        actState++;
                    }
                    if (actState == ActState.APCharge && currentTime >= TimeSpan.FromMilliseconds(1000))
                    {// APチャージ
                        currentTime = TimeSpan.Zero;
                        foreach (UnitGadget ug in allUnitGadget)
                        {
                            if (ug.dead)
                                continue;

                            // 症状値減少
                            if (ug.symptonMinus.sympton > 0)
                                ug.symptonMinus.turn -= 16;
                            if (ug.symptonPlus.sympton > 0)
                                ug.symptonPlus.turn -= 16;
                            if (ug.trap.sympton > 0)
                                ug.trap.turn -= 16;
                            if (ug.trap.sympton > 0 && ug.trap.turn <= 0)
                                ug.trap.sympton = Trap.None;

                            // 索敵隠蔽減少
                            ug.upParameter.AddSearch(-16);
                            ug.upParameter.AddHide(-16);
                        }
                        // 結晶値減少
                        if (crystal_face.sympton > 0)
                            crystal_face.turn -= 16;
                        CheckSymptonState();
                    }
                    if (actState == ActState.CrystalEffect && currentTime >= TimeSpan.FromMilliseconds(timeSpan))
                    {
                        #region 結晶効果
                        currentTime = TimeSpan.Zero;
                        if (crystal_face.sympton > 0 && crystal_face.turn <= 0)
                            crystal_face.sympton = FieldEffect.None;
                        else if (Crystal.sympton == FieldEffect.ChangeTerrain)
                        {
                            graphics.SetRenderTarget(r_map);
                            spriteBatch.Begin();
                            for (int i = 0; i < MapSize; i++)
                                for (int j = 0; j < MapSize; j++)
                                {
                                    if ((int)map[j, i] < 8)
                                    {
                                        map[j, i]++;
                                        if ((int)map[j, i] >= 8)
                                            map[j, i] = 0;
                                    }
                                    spriteBatch.Draw(t_map, new Vector2(j * 64, i * 64), new Rectangle((int)map[j, i] * 64, 0, 64, 64), Color.White);
                                }
                            spriteBatch.End();
                            graphics.SetRenderTarget(null);
                        }
                        CheckSymptonState();
                        #endregion
                    }

                    #region 症状関係
                    if (actState == ActState.DisPlus && currentTime >= TimeSpan.FromMilliseconds(1000))
                    {// プラス症状消滅
                        currentTime = TimeSpan.Zero;
                        foreach (UnitGadget ug in allUnitGadget)
                        {
                            if (ug.symptonPlus.sympton > 0 && ug.symptonPlus.turn <= 0)
                                ug.symptonPlus.sympton = SymptonPlus.None;
                        }
                        CheckSymptonState();
                    }
                    if (actState == ActState.DisMinus && currentTime >= TimeSpan.FromMilliseconds(1000))
                    {// マイナス症状消滅
                        currentTime = TimeSpan.Zero;
                        foreach (UnitGadget ug in allUnitGadget)
                        {
                            if (ug.symptonMinus.sympton > 0 && ug.symptonMinus.turn <= 0)
                                ug.symptonMinus.sympton = SymptonMinus.None;
                        }
                        CheckSymptonState();
                    }
                    if (actState == ActState.SymCharge && currentTime >= TimeSpan.FromMilliseconds(1000))
                    {// 活気発動
                        currentTime = TimeSpan.Zero;
                        foreach (UnitGadget ug in allUnitGadget)
                        {
                            if (ug.symptonPlus.sympton == SymptonPlus.Charge)
                                ug.AP += ug.symptonPlus.power;
                        }
                        CheckSymptonState();
                    }
                    if (actState == ActState.SymHeal && currentTime >= TimeSpan.FromMilliseconds(2000))
                    {// 再生発動
                        currentTime = TimeSpan.Zero;
                        CheckSymptonState();
                    }
                    if (actState == ActState.SymDamage && currentTime >= TimeSpan.FromMilliseconds(2000))
                    {// 継続発動
                        currentTime = TimeSpan.Zero;
                        actState = ActState.SelectAct;
                    }
                    if (actState == ActState.Dis2 && currentTime >= TimeSpan.FromMilliseconds(1000))
                    {// 症状消滅2
                        currentTime = TimeSpan.Zero;
                        currentUnit.symptonMinus.sympton = SymptonMinus.None;
                        actState = ActState.SelectAct;
                    }
                    #endregion

                    if (actState == ActState.SelectAct && currentUnit == null)
                    {// ユニット行動開始
                        currentTime = TimeSpan.Zero;
                        currentUnit = unitOrder[0];
                        unitOrder.RemoveAt(0);
                        CalcMoveCost();
                        selectedAct = 0;
                        targetCursor = unitCursor = currentUnit.postion;
                        drive = currentUnit.drive;
                        moved = false;
                        targetNo = 0;
                        currentUnit.dedodge = false;
                        currentUnit.deguard = false;
                        currentUnit.stance = -1;
                        if (currentUnit.IsType(UnitType.Intelligence))
                            AddSP(currentUnit, 10);
                        if (currentUnit.trap.sympton == Trap.SPPlant)
                            AddSP(currentUnit, currentUnit.trap.power);

                        if (currentUnit.symptonMinus.sympton == SymptonMinus.Stop)
                        {
                            int ap = currentUnit.AP;
                            currentUnit.AP -= currentUnit.symptonMinus.power;
                            if (currentUnit.AP < 0)
                                currentUnit.AP = 0;
                            currentUnit.symptonMinus.power -= ap;

                            if (currentUnit.symptonMinus.power <= 0)
                                currentUnit.symptonMinus.turn = 0;
                        }

                        if (currentUnit.symptonMinus.sympton > 0 && currentUnit.symptonMinus.turn <= 0)
                            actState = ActState.Dis2;
                    }
                    #endregion
                    else if (actState >= ActState.SelectAct && actState <= ActState.SelectTarget)
                    {
                        #region ユニットの行動選択
                        bgNum = (int)GetTip(currentUnit.postion);
                        if (actState == ActState.SelectAct)// 行動選択
                        {
                            if (bgBlack > 0)
                            {
                                bgBlack -= (float)gameTime.ElapsedGameTime.TotalMilliseconds / 250;
                                if (bgBlack < 0)
                                    bgBlack = 0;
                                return;
                            }

                            if (currentUnit.unit.IsHaveAbility(Ability.Drive) && InputManager.GetButtonStateIsPush(InputManager.GameButton.ShoulderR))
                            {
                                if (drive)
                                    drive = false;
                                else if ((currentUnit.DP >= 100 && currentUnit.AP >= 6) || currentUnit.drive)
                                    drive = true;
                            }

                            if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Down))
                            {
                                if (InputManager.GetButtonStateAsBool(InputManager.GameButton.Left))
                                    selectedAct = 4;
                                else if (InputManager.GetButtonStateAsBool(InputManager.GameButton.Right))
                                    selectedAct = 5;
                                else
                                    selectedAct = 0;
                            }
                            else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Left))
                            {
                                if (InputManager.GetButtonStateAsBool(InputManager.GameButton.Down))
                                    selectedAct = 4;
                                else
                                    selectedAct = 2;
                            }
                            else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Right))
                            {
                                if (InputManager.GetButtonStateAsBool(InputManager.GameButton.Down))
                                    selectedAct = 5;
                                else
                                    selectedAct = 3;
                            }
                            else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Up))
                                selectedAct = 1;

                            if (drive && InputManager.GetButtonStateIsPush(InputManager.GameButton.ShoulderL))
                            {
                                selectedAct = 6;
                            }

                            if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide) && IsCanAct())
                            {
                                actState++;
                            }
                            else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Cancel))
                            {
                                actState = ActState.ListAlly;
                                bgNum = 9;
                                selectedAct = 0;
                                drive = currentUnit.drive;
                            }
                        }
                        else if (actState == ActState.SelectMove)// 移動先選択
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
                                actState--;
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
                                        actState = ActState.Drive;
                                    else if (!moved)
                                        TurnEnd();
                                    else
                                    {
                                        DrawMoveBackGround();
                                        actState = ActState.Move;
                                    }
                                }
                                else if (selectedAct > 0 && GetActTarget().Count > 0)
                                {
                                    actState++;
                                    aimedEnemy = -1;
                                }
                            }
                        }
                        else if (actState == ActState.SelectTarget)// 対象選択
                        {
                            if (currentAct.target == ActTarget.Ally1 || currentAct.target == ActTarget.Enemy1
                                || currentAct.target == ActTarget.AllyEnemy1 || currentAct.target == ActTarget.Space)
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
                                if (InputManager.GetButtonStateIsPush(InputManager.GameButton.ShoulderR)
                                    || InputManager.GetButtonStateIsPush(InputManager.GameButton.ShoulderL))
                                {
                                    int aimed = -1;
                                    Positon p = currentUnit.postion;
                                    currentUnit.postion = unitCursor;
                                    List<UnitGadget> lug = GetActTarget();
                                    for (int i = 0; i < lug.Count; i++)
                                        if (lug[i].postion == targetCursor)
                                            aimed = i;

                                    if (InputManager.GetButtonStateIsPush(InputManager.GameButton.ShoulderR))
                                    {
                                        aimed++;
                                        if (aimed >= lug.Count)
                                            aimed = 0;
                                    }
                                    else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.ShoulderL))
                                    {
                                        aimed--;
                                        if (aimed < 0)
                                            aimed = lug.Count - 1;
                                    }
                                    targetCursor = lug[aimed].postion;
                                    currentUnit.postion = p;
                                }
                            }

                            if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Cancel))
                            {
                                actState--;
                                targetCursor = unitCursor;
                            }
                            else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide)
                                && (currentAct.IsTargetAll || Positon.Distance(unitCursor, targetCursor) >= currentAct.rangeMin))
                            {
                                targetList = GetActTarget();
                                bool targetExist = false;
                                if (currentAct.type == ActType.TransSpace)
                                    targetExist = true;
                                else
                                {
                                    if (currentAct.IsTargetAll)
                                    {
                                        if (targetList.Count > 0)
                                        {
                                            targetExist = true;
                                            targetNo = 0;
                                        }
                                    }
                                    else
                                    {
                                        for (int i = 0; i < targetList.Count; i++)
                                            if (targetList[i] == currentUnit ? targetCursor == unitCursor : targetCursor == targetList[i].postion)
                                            {
                                                targetExist = true;
                                                targetNo = i;
                                            }
                                    }
                                }

                                if (targetExist)
                                {
                                    usingAP = GetUsingAP(true);
                                    currentTime = TimeSpan.Zero;
                                    if (unitCursor != currentUnit.postion)
                                        moved = true;
                                    if (currentUnit.drive != drive)
                                        actState = ActState.Drive;
                                    else if (!moved)
                                        actState = ActState.TargetCursor;
                                    else
                                    {
                                        DrawMoveBackGround();
                                        actState = ActState.Move;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else if (actState > ActState.SelectTarget)
                    {
                        #region ユニットの行動
                        if (actState == ActState.Drive) // ドライヴ
                        {
                            if (currentTime.TotalMilliseconds >= 1000)
                                currentUnit.drive = drive;
                            if (currentTime.TotalMilliseconds >= 2000)
                            {
                                currentTime = TimeSpan.Zero;
                                if (selectedAct == 0 && !moved)
                                    TurnEnd();
                                else if (selectedAct > 0 && !moved)
                                    actState = ActState.TargetCursor;
                                else
                                    actState = ActState.Move;
                            }
                        }
                        else if (actState == ActState.Move) // 移動
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(550))
                            {
                                currentTime = TimeSpan.Zero;
                                if (currentUnit.postion != unitCursor)
                                    currentUnit.postion = GetMoveRoute(unitCursor);
                                else
                                {
                                    bgNum = (int)GetTip(unitCursor);
                                    if (selectedAct == 0)
                                        actState = ActState.MoveEnd;
                                    else
                                        actState = ActState.TargetCursor;
                                }
                            }
                        }
                        else if (actState == ActState.MoveEnd) // 移動終わり
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(250))
                            {
                                currentTime = TimeSpan.Zero;
                                TurnEnd();
                            }
                        }
                        else if (actState == ActState.TargetCursor) // 対象表示
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(800))
                            {
                                currentTime = TimeSpan.Zero;
                                target = targetList[targetNo];
                                covered = false;
                                countered = false;
                                actState = ActState.ShowActExplain;
                                #region 援護、トラップ
                                if (currentAct.TypeInt % 100 == 0 && !currentAct.IsTargetAll && !currentAct.IsPenetrate)
                                {
                                    if (!currentAct.IsHaveAbility(ActAbility.Summon) && target.trap.sympton == Trap.OnceClear)
                                    {
                                        timeSpan = 1000 + (int)(125 * Vector2.Distance(unitCursor, target.postion));
                                        actState = ActState.TakeTrap;
                                    }
                                    else if (!currentAct.IsHaveAbility(ActAbility.Summon)
                                        && (target.trap.sympton == Trap.AttackTrap
                                            || (currentAct.type == ActType.Grapple && target.trap.sympton == Trap.GrappleTrap)
                                            || (currentAct.type == ActType.Shot && target.trap.sympton == Trap.ShotTrap)))
                                    {
                                        target.trap.turn -= currentUnit.Parameter.speed;
                                        if (target.trap.turn > 0)
                                        {
                                            timeSpan = 3000 + (int)(125 * Vector2.Distance(unitCursor, target.postion));
                                            AddHP(currentUnit, -target.trap.power);
                                            actState = ActState.TakeTrap;
                                        }
                                        else
                                            actState = ActState.DisarmTrap;
                                    }
                                    else
                                    {
                                        foreach (UnitGadget ug in allUnitGadget)
                                        {
                                            if (ug.stance < 0 || !ug.StanceAct.IsCover || target.ff != ug.ff || ug == target)
                                                continue;

                                            float dis = Positon.Distance(target.postion, ug.postion);
                                            if (dis >= ug.StanceAct.rangeMin && dis <= ug.StanceAct.rangeMax)
                                            {
                                                target_old = target;
                                                target = ug;
                                                covered = true;
                                                actState = ActState.CoverRun;
                                                break;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        else if (actState == ActState.ShowActExplain) // 行動表示
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(1500))
                            {
                                currentTime = TimeSpan.Zero;
                                if (currentAct.IsLimited) // 回数消費
                                    currentUnit.actCount[selectedAct - 1]--;
                                if (currentAct.IsPassive
                                    || (currentAct.type == ActType.Heal && !currentAct.IsTargetAll && target.HP == target.HPmax)
                                    || (Crystal.sympton == FieldEffect.SympInvalid
                                        && (currentAct.type == ActType.AddMinusSympton || currentAct.type == ActType.AddPlusSympton
                                            || currentAct.type == ActType.AddDoubleSympton))
                                    || (currentAct.type == ActType.AddMinusSympton
                                        && !currentAct.IsTargetAll && target.IsMinusSymptonInvalid((SymptonMinus)currentAct.sympton))
                                    || (currentAct.type == ActType.AddPlusSympton
                                        && target.IsPlusSymptonInvalid((SymptonPlus)currentAct.sympton))
                                    || (currentAct.type == ActType.SetTrap && !currentAct.IsTargetAll && target.trap.sympton == Trap.TrapClear)
                                    || (currentAct.type == ActType.ClearMinusSympton && !IsAlly(target) && target.symptonMinus.sympton != SymptonMinus.Invalid)
                                    || (currentAct.type == ActType.ClearPlusSympton && IsAlly(target) && target.symptonPlus.sympton != SymptonPlus.Invalid)
                                    || (currentAct.type == ActType.ClearTrap && IsAlly(target) && target.trap.sympton != Trap.TrapClear)
                                    || (currentAct.type == ActType.SPDrain && target.SP == 0)
                                    || (currentAct.type == ActType.LevelDrain && !currentAct.IsTargetAll && target.level == 0)
                                    || (currentAct.IsActiveDefense && currentUnit.symptonMinus.sympton == SymptonMinus.Deguard)
                                    || (currentAct.IsActiveDodge && currentUnit.symptonMinus.sympton == SymptonMinus.Dedodge))
                                {
                                    actState = ActState.Failure2;
                                }
                                else
                                    actState = ActState.CalcAct;
                            }
                        }
                        else if (actState == ActState.CalcAct)
                        {
                            #region 行動計算
                            if (currentAct.IsSpell) // SP消費
                                AddSP(currentUnit, -currentAct.sp);
                            if (currentAct.TypeInt % 100 == 0 || currentAct.type == ActType.AddMinusSympton) // 攻撃、妨害
                            {
                                currentTime = TimeSpan.Zero;
                                CalcAttackResult();
                                if (currentUnit.trap.sympton == Trap.HitPromise)
                                {
                                    currentUnit.trap.power--;
                                    if (currentUnit.trap.power <= 0)
                                        currentUnit.trap.sympton = Trap.None;
                                }
                                timeSpan = (int)(200 * Vector2.Distance(unitCursor, target.postion));
                                if (currentAct.type == ActType.AddMinusSympton
                                    && (currentAct.sympton == (int)SymptonMinus.FixInside || currentAct.sympton == (int)SymptonMinus.FixOutside))
                                    timeSpan += 200;
                                if (currentAct.IsTargetAll)
                                    actState = ActState.AddSymp;
                                else if (calcResult[0].hit)
                                    actState = ActState.AddSympSub;
                                else
                                    actState = ActState.Failure1;
                                if (currentAct.TypeInt % 100 == 0)
                                {
                                    if (currentAct.IsTargetAll)
                                        for (int i = 0; i < targetList.Count; i++)
                                        {
                                            if (calcResult[i].hit && calcResult[i].damage > 0)
                                            {
                                                AddHP(targetList[i], -calcResult[i].damage);
                                                AddSP(targetList[i], 10);
                                            }
                                        }
                                    else if (calcResult[0].hit && calcResult[0].damage > 0)
                                    {
                                        if (!currentAct.IsPenetrate && target.stance >= 0
                                            && target.StanceAct.type == ActType.Counter && calcResult[0].damage >= target.StanceAct.power)
                                            countered = true;
                                        if (!countered)
                                        {
                                            AddHP(target, -calcResult[0].damage);
                                            AddSP(target, 10);
                                        }
                                        else
                                        {
                                            AddHP(currentUnit, -calcResult[0].damage);
                                            AddSP(currentUnit, 10);
                                        }
                                    }
                                    actState = ActState.Attack_Heal;
                                }
                            }
                            else // 補助
                            {
                                currentTime = TimeSpan.Zero;
                                CalcSupportResult();
                                if (calcResult[0].hit)
                                {
                                    timeSpan = (int)(200 * Vector2.Distance(unitCursor, target.postion));
                                    if (currentAct.type == ActType.ClearTrap)
                                        timeSpan /= 2;
                                    switch (currentAct.type)
                                    {
                                        case ActType.Heal:
                                        case ActType.Heal2:
                                            for (int i = 0; i < targetList.Count; i++)
                                            {
                                                UnitGadget tar = currentAct.IsTargetAll ? targetList[i] : target;
                                                AddHP(tar, calcResult[i].damage);
                                                if (currentAct.type == ActType.Heal2 && tar.symptonMinus.sympton > 0 && tar.symptonMinus.sympton != SymptonMinus.Invalid)
                                                    tar.symptonMinus.turn -= calcResult[9].damage;
                                                if (!currentAct.IsTargetAll)
                                                    break;
                                            }
                                            actState = ActState.Attack_Heal;
                                            break;
                                        case ActType.Revive:
                                        case ActType.Revive2:
                                            if (currentAct.IsTargetAll)
                                            {
                                                timeSpan = 4000;
                                                actState = ActState.AddSymp;
                                            }
                                            else
                                                actState = ActState.AddSympSub;
                                            break;
                                        case ActType.AddPlusSympton:
                                        case ActType.ClearMinusSympton:
                                        case ActType.ClearPlusSympton:
                                        case ActType.ClearTrap:
                                        case ActType.SPUp:
                                            if (currentAct.type == ActType.ClearMinusSympton)
                                                target.symptonMinus.turn -= calcResult[0].damage;
                                            else if (currentAct.type == ActType.ClearPlusSympton)
                                                target.symptonPlus.turn -= calcResult[0].damage;
                                            else if (currentAct.type == ActType.ClearTrap)
                                                target.trap.turn -= calcResult[0].damage;
                                            if (currentAct.IsTargetAll || target == currentUnit)
                                                SetSympFact();
                                            else
                                                actState = ActState.AddSympSub;
                                            break;
                                        case ActType.AddDoubleSympton:
                                            timeSpan = 4000;
                                            actState = ActState.AddSymp;
                                            break;
                                        case ActType.SetTrap:
                                            actState = ActState.SetTrap;
                                            break;
                                        case ActType.SetField:
                                            actState = ActState.SetCrystal;
                                            break;
                                        case ActType.Guard:
                                        case ActType.LessGuard:
                                        case ActType.Utsusemi:
                                        case ActType.Counter:
                                        case ActType.BarrierDefense:
                                        case ActType.BarrierSpirit:
                                            actState = ActState.Cover_Stance;
                                            break;
                                        case ActType.SearchEnemy:
                                        case ActType.Hide:
                                        case ActType.UpSpeed:
                                        case ActType.UpClose:
                                        case ActType.UpFar:
                                        case ActType.ClearParameter:
                                            actState = ActState.Search_Hide;
                                            break;
                                        case ActType.LevelDrain:
                                        case ActType.SPDrain:
                                            if(currentAct.IsTargetAll)
                                                timeSpan = 1800 + 200 * targetList.Count;
                                            else
                                                timeSpan = 2000;
                                            actState = ActState.Drain;
                                            break;
                                        case ActType.TransSpace:
                                            actState = ActState.TransSpace1;
                                            break;
                                        default:
                                            actState = ActState.Failure1;
                                            break;
                                    }
                                }
                                else
                                    actState = ActState.Failure1;
                            }
                            #endregion
                        }
                        else if (actState == ActState.Attack_Heal)
                        {
                            #region 攻撃回復
                            if (currentTime >= TimeSpan.FromMilliseconds(2000))
                            {
                                currentTime = TimeSpan.Zero;
                                foreach (UnitGadget ug in allUnitGadget)
                                    ug.HPold = ug.HP;
                                if (currentAct.TypeInt % 100 == 0)
                                {
                                    for (int i = 0; i < targetList.Count; i++)
                                    {
                                        if (currentAct.IsTargetAll)
                                            target = targetList[i];
                                        if (calcResult[i].hit && calcResult[i].damage > 0)
                                        {
                                            UnitGadget tar = countered ? currentUnit : target;
                                            if (currentAct.sympton > 0 && Crystal.sympton != FieldEffect.SympInvalid
                                                && !tar.IsMinusSymptonInvalid((SymptonMinus)currentAct.sympton))
                                            {
                                                if (currentAct.sympton == (int)SymptonMinus.Stigmata)
                                                {
                                                    tar.symptonPlus.sympton = SymptonPlus.Stigmata;
                                                    tar.symptonPlus.power = calcResult[i].sympPower;
                                                    tar.symptonPlus.turn = calcResult[i].sympTurn;
                                                }
                                                else
                                                {
                                                    tar.symptonMinus.sympton = (SymptonMinus)currentAct.sympton;
                                                    tar.symptonMinus.power = calcResult[i].sympPower;
                                                    tar.symptonMinus.turn = calcResult[i].sympTurn;
                                                    tar.symptonMinus.doer = currentUnit;
                                                }
                                            }
                                        }
                                        if (target.HP <= 0)
                                        {
                                            target.drive = false;
                                            target.symptonMinus.sympton = SymptonMinus.None;
                                            target.symptonPlus.sympton = SymptonPlus.None;
                                            target.trap.sympton = Trap.None;
                                            target.stance = -1;
                                            target.dedodge = false;
                                            target.deguard = false;
                                            unitOrder.Remove(target);
                                        }
                                        if (!currentAct.IsTargetAll)
                                            break;
                                    }
                                    AttackEnd();
                                }
                                else if (currentAct.type == ActType.Heal2)
                                    actState = ActState.Heal2;
                                else
                                    TurnEnd();
                            }
                            #endregion
                        }
                        else if (actState == ActState.Heal2)
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(1000))
                            {
                                currentTime = TimeSpan.Zero;
                                foreach (UnitGadget ug in allUnitGadget)
                                {
                                    if (ug.symptonMinus.sympton > 0 && ug.symptonMinus.turn <= 0)
                                        ug.symptonMinus.sympton = SymptonMinus.None;
                                }
                                TurnEnd();
                            }
                        }
                        else if (actState == ActState.AddSympSub)
                        {
                            #region 付加
                            // 付加飛ばし
                            if (currentTime >= TimeSpan.FromMilliseconds(timeSpan))
                            {
                                currentTime = TimeSpan.Zero;
                                SetSympFact();
                            }
                        }
                        else if (actState == ActState.AddSymp)
                        {
                            // 付加
                            if (currentTime >= TimeSpan.FromMilliseconds(timeSpan))
                            {
                                currentTime = TimeSpan.Zero;
                                List<UnitGadget> lug = GetActTarget();
                                switch (currentAct.type)
                                {
                                    case ActType.AddPlusSympton:
                                        target.symptonPlus = new Condition<SymptonPlus>((SymptonPlus)currentAct.sympton, calcResult[0].sympTurn, calcResult[0].sympPower);
                                        break;
                                    case ActType.AddMinusSympton:
                                        for (int i = 0; i < lug.Count; i++)
                                        {
                                            if (calcResult[i].hit)
                                            {
                                                UnitGadget tar = currentAct.IsTargetAll ? lug[i] : target;
                                                tar.symptonMinus = new Condition<SymptonMinus>(
                                                    (SymptonMinus)currentAct.sympton, calcResult[i].sympTurn, calcResult[i].sympPower, currentUnit);
                                            }
                                            if (!currentAct.IsTargetAll)
                                                break;
                                        }
                                        break;
                                    case ActType.AddDoubleSympton:
                                        for (int i = 0; i < lug.Count; i++)
                                        {
                                            if (calcResult[0].hit)
                                            {
                                                if (!lug[i].IsPlusSymptonInvalid(SymptonPlus.Stigmata))
                                                    lug[i].symptonPlus = new Condition<SymptonPlus>(
                                                        SymptonPlus.Stigmata, calcResult[0].sympTurn, calcResult[0].sympPower);
                                                if (!lug[i].IsMinusSymptonInvalid(SymptonMinus.CarvedSeal))
                                                    lug[i].symptonMinus = new Condition<SymptonMinus>(
                                                        SymptonMinus.CarvedSeal, calcResult[0].sympTurn, calcResult[0].sympPower, currentUnit);
                                            }
                                        }
                                        break;
                                    case ActType.ClearMinusSympton:
                                        target.symptonMinus = new Condition<SymptonMinus>(SymptonMinus.Invalid, calcResult[0].sympTurn, calcResult[0].sympPower);
                                        break;
                                    case ActType.ClearPlusSympton:
                                        target.symptonPlus = new Condition<SymptonPlus>(SymptonPlus.Invalid, calcResult[0].sympTurn, calcResult[0].sympPower);
                                        break;
                                    case ActType.ClearTrap:
                                        target.trap = new Condition<Trap>(Trap.TrapClear, calcResult[0].sympTurn, calcResult[0].sympPower);
                                        break;
                                    case ActType.SPUp:
                                        AddSP(target, calcResult[0].damage);
                                        break;
                                    case ActType.Revive:
                                    case ActType.Revive2:
                                        for (int i = 0; i < lug.Count; i++)
                                        {
                                            UnitGadget tar = currentAct.IsTargetAll ? lug[i] : target;
                                            if (tar.dead || currentAct.type == ActType.Revive2)
                                            {
                                                tar.dead = false;
                                                AddHP(tar, calcResult[i].damage);
                                            }
                                            if (!currentAct.IsTargetAll)
                                                break;
                                        }
                                        break;
                                }
                                if (currentAct.type == ActType.ClearMinusSympton || currentAct.type == ActType.ClearPlusSympton
                                     || currentAct.type == ActType.ClearTrap)
                                {
                                    actState = ActState.ClearResult;
                                }
                                else if (currentAct.type == ActType.Revive || currentAct.type == ActType.Revive2)
                                {
                                    actState = ActState.Attack_Heal;
                                }
                                else
                                    TurnEnd();
                            }
                        }
                        else if (actState == ActState.AddSympFromAttack)
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(1000))
                            {
                                currentTime = TimeSpan.Zero;
                                AttackEnd();
                            }
                        }
                        else if (actState == ActState.ClearSymp) // プラスマイナストラップクリア
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(timeSpan))
                            {
                                currentTime = TimeSpan.Zero;
                                timeSpan = 1500;
                                actState = ActState.ClearResult;
                                switch (currentAct.type)
                                {
                                    case ActType.ClearMinusSympton:
                                        if (target.symptonMinus.turn <= 0)
                                        {
                                            target.symptonMinus.sympton = SymptonMinus.None;
                                            if (IsAlly(target))
                                                actState = ActState.AddSymp;
                                        }
                                        break;
                                    case ActType.ClearPlusSympton:
                                        if (target.symptonPlus.turn <= 0)
                                        {
                                            target.symptonPlus.sympton = SymptonPlus.None;
                                            if (!IsAlly(target))
                                                actState = ActState.AddSymp;
                                        }
                                        break;
                                    case ActType.ClearTrap:
                                        if (target.trap.turn <= 0)
                                        {
                                            target.trap.sympton = Trap.None;
                                            if (!IsAlly(target))
                                            {
                                                timeSpan = (int)(100 * Vector2.Distance(unitCursor, target.postion));
                                                actState = ActState.AddSympSub;
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                        else if (actState == ActState.ClearResult)
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(1000))
                            {
                                currentTime = TimeSpan.Zero;
                                TurnEnd();
                            }
                            #endregion
                        }
                        else if (actState == ActState.Drain) // SP、レベルドレイン
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(timeSpan))
                            {
                                currentTime = TimeSpan.Zero;
                                if (currentAct.type == ActType.LevelDrain)
                                {
                                    if (currentAct.IsTargetAll)
                                    {
                                        for (int i = 0; i < targetList.Count; i++)
                                        {
                                            AddLevel(targetList[i], -calcResult[i].damage);
                                            AddLevel(currentUnit, calcResult[i].damage);
                                        }
                                    }
                                    else
                                    {
                                        AddLevel(target, -calcResult[0].damage);
                                        AddLevel(currentUnit, calcResult[0].damage);
                                    }
                                }
                                else
                                {
                                    AddSP(target, -calcResult[0].damage);
                                    AddSP(currentUnit, calcResult[0].damage);
                                }
                                TurnEnd();
                            }
                        }
                        else if (actState == ActState.SetTrap) // トラップ設置
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(3500))
                            {
                                currentTime = TimeSpan.Zero;
                                if (!currentAct.IsTargetAll)
                                    target.trap = new Condition<Trap>((Trap)currentAct.sympton, calcResult[0].sympTurn, calcResult[0].sympPower);
                                else
                                    foreach (UnitGadget ug in targetList)
                                        ug.trap = new Condition<Trap>((Trap)currentAct.sympton, calcResult[0].sympTurn, calcResult[0].sympPower);
                                TurnEnd();
                            }
                        }
                        else if (actState == ActState.SetCrystal) // 結晶解放
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(6000))
                            {
                                currentTime = TimeSpan.Zero;
                                crystal_face = new Condition<FieldEffect>((FieldEffect)currentAct.sympton, calcResult[0].sympTurn, calcResult[0].sympPower);
                                if (crystal_face.sympton == FieldEffect.SympInvalid)
                                {
                                    foreach (UnitGadget ug in allUnitGadget)
                                    {
                                        ug.symptonMinus.sympton = SymptonMinus.None;
                                        ug.symptonPlus.sympton = SymptonPlus.None;
                                    }
                                }
                                TurnEnd();
                            }
                        }
                        else if (actState == ActState.Search_Hide) // 索敵隠蔽能力アップ
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(4000))
                            {
                                currentTime = TimeSpan.Zero;
                                if (currentAct.type == ActType.Hide)
                                    currentUnit.upParameter.AddHide(calcResult[0].damage);
                                else
                                {
                                    foreach (UnitGadget ug in targetList)
                                    {
                                        switch (currentAct.type)
                                        {
                                            case ActType.SearchEnemy:
                                                ug.upParameter.AddSearch(calcResult[0].damage);
                                                break;
                                            case ActType.UpSpeed:
                                                ug.upParameter.AddSpeed(calcResult[0].damage);
                                                break;
                                            case ActType.UpClose:
                                                ug.upParameter.AddClose(calcResult[0].damage);
                                                break;
                                            case ActType.UpFar:
                                                ug.upParameter.AddFar(calcResult[0].damage);
                                                break;
                                            case ActType.UpReact:
                                                currentUnit.upParameter.AddSearch(calcResult[0].damage);
                                                currentUnit.upParameter.AddHide(calcResult[0].damage);
                                                break;
                                            case ActType.ClearParameter:
                                                ug.upParameter = new UnitGadget.UpParameter();
                                                break;
                                        }
                                    }
                                }
                                TurnEnd();
                            }
                        }
                        else if (actState == ActState.Cover_Stance) // 援護構え
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(3000))
                            {
                                currentTime = TimeSpan.Zero;
                                currentUnit.stance = selectedAct - 1;
                                TurnEnd();
                            }
                        }
                        else if (actState == ActState.CoverRun) // 援護発生
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(2000))
                            {
                                currentTime = TimeSpan.Zero;
                                actState = ActState.ShowActExplain;
                            }
                        }
                        else if (actState == ActState.TransSpace1) // 空間移動
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(1000))
                            {
                                currentTime = TimeSpan.Zero;
                                bgBlack = 1;
                                foreach(UnitGadget ug in allUnitGadget)
                                    if (ug.postion == targetCursor)
                                    {
                                        ug.postion = unitCursor;
                                        break;
                                    }
                                currentUnit.postion = targetCursor;
                                bgNum = (int)GetTip(currentUnit.postion);
                                actState = ActState.TransSpace2;
                            }
                        }
                        else if (actState == ActState.TransSpace2)
                        {
                            if (bgBlack > 0)
                            {
                                bgBlack -= (float)gameTime.ElapsedGameTime.TotalMilliseconds / 500;
                                if (bgBlack < 0)
                                    bgBlack = 0;
                            }
                            if (currentTime >= TimeSpan.FromMilliseconds(1000))
                            {
                                currentTime = TimeSpan.Zero;
                                TurnEnd();
                            }
                        }
                        else if (actState == ActState.DefeatEnemy) // 敵撃破
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(1000))
                            {
                                currentTime = TimeSpan.Zero;
                                for (int i = 0; i < targetList.Count; i++)
                                    if (targetList[i].HP <= 0)
                                    {
                                        targetList[i].dead = true;
                                        foreach (UnitGadget ug in allUnitGadget)
                                            if ((ug.symptonMinus.sympton == SymptonMinus.FixInside || ug.symptonMinus.sympton == SymptonMinus.FixOutside)
                                                && ug.symptonMinus.doer == targetList[i])
                                                ug.symptonMinus.sympton = SymptonMinus.None;
                                    }
                                AttackEnd();
                            }
                        }
                        else if (actState == ActState.Defeated) // 自滅
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(1000))
                            {
                                currentTime = TimeSpan.Zero;
                                currentUnit.dead = true;
                                currentUnit.AP = 0;
                                foreach (UnitGadget ug in allUnitGadget)
                                    if ((ug.symptonMinus.sympton == SymptonMinus.FixInside || ug.symptonMinus.sympton == SymptonMinus.FixOutside)
                                        && ug.symptonMinus.doer == currentUnit)
                                        ug.symptonMinus.sympton = SymptonMinus.None;
                                actState = ActState.SelectAct;
                                currentUnit = null;
                            }
                        }
                        else if (actState == ActState.SymActAgain) // 連続行動
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(2000))
                            {
                                currentTime = TimeSpan.Zero;
                                if (currentUnit.symptonPlus.power <= 0)
                                    currentUnit.symptonPlus.sympton = SymptonPlus.None;
                                CalcMoveCost();
                                selectedAct = 0;
                                targetCursor = unitCursor = currentUnit.postion;
                                drive = currentUnit.drive;
                                moved = false;
                                targetNo = 0;
                                currentUnit.dedodge = false;
                                currentUnit.deguard = false;
                                currentUnit.stance = -1;
                                actState = ActState.SelectAct;
                            }
                        }
                        else if (actState == ActState.LeakDP) // DP減少
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(2000))
                            {
                                currentTime = TimeSpan.Zero;
                                AddDP(currentUnit, -10);
                                if (currentUnit.DP <= 0)
                                {
                                    currentUnit.drive = false;
                                    actState = ActState.DriveOff;
                                }
                                else
                                    TurnEnd();
                            }
                        }
                        else if (actState == ActState.DriveOff) // ドライヴ解除
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(1000))
                            {
                                currentTime = TimeSpan.Zero;
                                TurnEnd();
                            }
                        }
                        else if (actState == ActState.SPCharge) // SP溜め
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(3000))
                            {
                                currentTime = TimeSpan.Zero;
                                TurnEnd();
                            }
                        }
                        else if (actState == ActState.Failure1 || actState == ActState.Failure2) // 行動失敗
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(2000))
                            {
                                currentTime = TimeSpan.Zero;
                                TurnEnd();
                            }
                        }
                        else if (actState == ActState.TakeTrap) // トラップ発動
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(timeSpan))
                            {
                                currentTime = TimeSpan.Zero;
                                if (target.trap.sympton == Trap.OnceClear)
                                {
                                    currentUnit.trap.power--;
                                    if (currentUnit.trap.power <= 0)
                                        currentUnit.trap.sympton = Trap.None;
                                    TurnEnd();
                                }
                                else
                                {
                                    if (currentUnit.HP > 0)
                                        actState = ActState.ShowActExplain;
                                    else
                                        actState = ActState.Defeated;
                                }
                            }
                        }
                        else if (actState == ActState.DisarmTrap) // トラップ解除
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(2000))
                            {
                                currentTime = TimeSpan.Zero;
                                target.trap.sympton = Trap.None;
                                actState = ActState.ShowActExplain;
                            }
                        }
                        else if (actState == ActState.Force) // 強制移動
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(2000))
                            {
                                currentTime = TimeSpan.Zero;
                                if (countered)
                                    currentUnit.postion = calcResult[1].forcedPos;
                                else
                                {
                                    for (int i = 0; i < targetList.Count; i++)
                                    {
                                        UnitGadget tar = currentAct.IsTargetAll ? targetList[i] : target;
                                        if (!tar.dead)
                                            tar.postion = calcResult[i].forcedPos;
                                        if (!currentAct.IsTargetAll)
                                            break;
                                    }
                                }
                                AttackEnd();
                            }
                        }
                        else if (actState == ActState.HPSPgain) // 吸収精神侵蝕犠牲
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(1000))
                            {
                                currentTime = TimeSpan.Zero;
                                AttackEnd();
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                else
                {
                    #region 状態確認
                    if (actState == ActState.Confront)
                    {
                        if (currentTime >= TimeSpan.FromMilliseconds(2000))
                            actState = ActState.ListAlly;
                    }
                    else if (actState == ActState.BackBlackout)
                    {
                        bgBlack += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 250;
                        if (bgBlack >= 1)
                        {
                            bgBlack = 1;
                            selectedAct = 0;
                            if (turn == 0)
                                actState = ActState.TurnStart;
                            else
                            {
                                bgNum = (int)GetTip(currentUnit.postion);
                                actState = ActState.SelectAct;
                            }
                        }
                    }
                    else if (actState == ActState.ListAlly || actState == ActState.ListEnemy)
                    {
                        if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Up))
                        {
                            if (selectedAct == 1)
                            {
                                selectedAct--;
                                if (actState == ActState.ListEnemy)
                                    actState--;
                            }
                            else if (selectedAct > 1)
                            {
                                int sa = selectedAct;
                                List<UnitGadget> list;
                                if (actState == ActState.ListAlly)
                                    list = allyUnitGadget;
                                else
                                    list = enemyUnitGadget;

                                do
                                {
                                    selectedAct--;
                                    if (selectedAct > list.Count)
                                    {
                                        selectedAct = sa;
                                        break;
                                    }

                                } while (list[selectedAct - 1].dead);
                            }
                        }
                        else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Down))
                        {
                            int sa = selectedAct;
                            List<UnitGadget> list;
                            if (actState == ActState.ListAlly)
                                list = allyUnitGadget;
                            else
                                list = enemyUnitGadget;

                            do
                            {
                                selectedAct++;
                                if (selectedAct > list.Count)
                                {
                                    selectedAct = sa;
                                    break;
                                }

                            } while (list[selectedAct - 1].dead);
                        }
                        else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Left))
                        {
                            if (actState == ActState.ListEnemy)
                            {
                                actState--;
                                if (selectedAct >= allyUnitGadget.Count)
                                    selectedAct = allyUnitGadget.Count;
                            }
                        }
                        else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Right))
                        {
                            if (actState == ActState.ListAlly)
                            {
                                actState++;
                                if (selectedAct == 0)
                                    selectedAct++;
                                if (selectedAct >= enemyUnitGadget.Count)
                                    selectedAct = enemyUnitGadget.Count;
                            }
                        }
                        else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Pause)
                            || (selectedAct == 0 && InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide)))
                        {
                            actState = ActState.BackBlackout;
                        }
                        else if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Decide))
                        {
                            UnitGadget ug = (actState == ActState.ListAlly) ? allyUnitGadget[selectedAct - 1] : enemyUnitGadget[selectedAct - 1];
                            bgNum = (int)GetTip(ug.postion);
                            actState += 2;
                        }
                    }
                    else
                    {
                        if (InputManager.GetButtonStateIsPush(InputManager.GameButton.Cancel))
                        {
                            actState -= 2;
                            bgNum = 9;
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
                if (MathHelper.Distance(mapOrigin.X, goal) <= delta)
                    mapOrigin.X = goal;
                else if (mapOrigin.X - goal > 0)
                    mapOrigin.X -= delta;
                else
                    mapOrigin.X += delta;
            }
        }

        public static void Draw(GameTime gameTime)
        {
            Texture2D tw = new Texture2D(graphics, 1, 1);
            Color[] c = { Color.White };
            tw.SetData(c);
            string str = "";

            float time = (float)currentTime.TotalMilliseconds;
            float elps = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            #region マップの色付け
            graphics.SetRenderTarget(r_map_2);

            spriteBatch.Begin(0, null, null, null, null, e_desaturate);
            int satu = 64;
            if (battleStart && actState >= ActState.SelectAct && actState <= ActState.SelectTarget && !IsMapMoving())
                satu = 0;
            spriteBatch.Draw(r_map, Vector2.Zero, new Color(255, 255, 255, satu));
            spriteBatch.End();

            spriteBatch.Begin();

            if (satu == 0 && currentUnit != null && IsCanAct())
            {
                int actCost = GetUsingAP(false);
                for (int i = 0; i < MapSize; i++)
                    for (int j = 0; j < MapSize; j++)
                        if (mapCost[i, j].cost + actCost <= currentUnit.AP)
                            spriteBatch.Draw(tw, new Rectangle(i * 64, j * 64, 64, 64), new Color(0, 0, 128, 0));
                if (selectedAct > 0)
                {
                    for (int i = 0; i < MapSize; i++)
                        for (int j = 0; j < MapSize; j++)
                            if (Positon.Distance(new Positon(i, j), unitCursor) >= currentAct.rangeMin
                                && Positon.Distance(new Positon(i, j), unitCursor) <= currentAct.rangeMax)
                                spriteBatch.Draw(tw, new Rectangle(i * 64, j * 64, 64, 64), new Color(128, 0, 0, 0));
                }
            }

            spriteBatch.End();
            graphics.SetRenderTarget(null);
            #endregion

            graphics.Clear(Color.Black);
            spriteBatch.Begin(0, null, null, null, null, e_dot);

            #region 全体
            // 背景描画
            if (actState == ActState.Move)
            {
                bgMoveOffset += 512 * elps / 550;
                if (currentUnit.ff == FrendOfFoe.Ally)
                    spriteBatch.Draw(r_bgMove, new Vector2(-bgMoveOffset, 0), Color.White);
                else
                    spriteBatch.Draw(r_bgMove, new Vector2(-r_bgMove.Width + 1024 + bgMoveOffset, 0), Color.White);
            }
            else if (actState == ActState.TransSpace1)
            {
                float scaleX, scaleY;
                if (time < 500)
                {
                    scaleX = 1;
                    scaleY = MathHelper.Clamp(1 - time / 300, 0.05f, 1);
                }
                else
                {
                    scaleX = MathHelper.Clamp(1 - (time - 500) / 300, 0, 1);
                    scaleY = 0.05f;
                }
                spriteBatch.Draw(t_bg[bgNum], new Rectangle((int)(512 * (1 - scaleX)), (int)(320 * (1 - scaleY)), (int)(512 * scaleX), (int)(640 * scaleY)),
                    new Rectangle(0, 0, 512, 640), Color.White);
                spriteBatch.Draw(t_bg[bgNum], new Rectangle(512, (int)(320 * (1 - scaleY)), (int)(512 * scaleX), (int)(640 * scaleY)),
                    new Rectangle(0, 0, 512, 640), Color.White);
            }
            else
            {
                spriteBatch.Draw(t_bg[bgNum], new Vector2(0, 0), Color.White);
                spriteBatch.Draw(t_bg[bgNum], new Vector2(512, 0), Color.White);
            }
            if (bgBlack > 0)
                spriteBatch.Draw(tw, new Rectangle(0, 0, 1024, 640), new Color(0, 0, 0, bgBlack));

            if (battleStart && actState >= ActState.SelectAct && actState < ActState.Win && !IsMapMoving() && currentUnit != null)
            {
                // ユニット描画
                DrawUnit(currentUnit, gameTime);
            }

            if (battleStart || (actState != ActState.ShowAlly && actState != ActState.ShowEnemy))
            {
                // マップ描画
                spriteBatch.Draw(tw, new Rectangle(mapOrigin.X + 6, mapOrigin.Y - 2, 564, 572), new Color(40, 56, 32));
                spriteBatch.Draw(tw, new Rectangle(mapOrigin.X + 10, mapOrigin.Y + 2, 4, 4), Color.White);
                spriteBatch.Draw(tw, new Rectangle(mapOrigin.X + 18, mapOrigin.Y + 2, 548, 4), Color.Red);
                spriteBatch.Draw(r_map_2, mapOrigin + new Vector2(10, 10), new Rectangle(10, 10, 556, 556), Color.White);
                if (mapWhite > 0)
                    spriteBatch.Draw(tw, new Rectangle(mapOrigin.X + 6, mapOrigin.Y - 2, 564, 572), new Color(mapWhite, mapWhite, mapWhite, mapWhite));
                

                // SPチャージ
                foreach (UnitGadget ug in allUnitGadget)
                {
                    if (battleStart && ug.spChargeFact > 0)
                    {
                        Vector2 v = mapOrigin + new Vector2(ug.postion.X * 64 + 32, ug.postion.Y * 64 + 32);
                        float s = 1;
                        if (ug.spChargeFact < 0.8)
                            s = 0.5f + 0.625f * ug.spChargeFact;
                        float a = 0.5f;
                        if (ug.spChargeFact > 0.8)
                            a = (1 - ug.spChargeFact) * 2.5f;
                        if (gameTime.TotalGameTime.TotalMilliseconds % 100 >= 50)
                            spriteBatch.Draw(t_icon3, v, new Rectangle(0, 0, 128, 128), new Color(a, a, a, a), 0, new Vector2(64), s, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(t_icon3, v, new Rectangle(0, 128, 128, 128), new Color(a, a, a, a), 0, new Vector2(64), s, SpriteEffects.None, 0);
                        ug.spChargeFact = 0;
                    }
                }

                // ユニットアイコン描画
                foreach (UnitGadget ug in allUnitGadget)
                {
                    if (ug.HP <= 0 && ug.HPold <= 0)
                        continue;

                    // ユニット描画
                    if (actState == ActState.Force && (countered ? ug == currentUnit : (currentAct.IsTargetAll ? targetList.Contains(ug) : ug == target)))
                    {
                        Vector2 s = mapOrigin + 64 * (Vector2)ug.postion;
                        int n = countered ? 1 : (currentAct.IsTargetAll ? targetList.IndexOf(ug) : 0);
                        if (ug.postion != calcResult[n].forcedPos)
                        {
                            Vector2 g = mapOrigin + 64 * (Vector2)calcResult[n].forcedPos;
                            float d = Vector2.Distance(s, g);
                            float t = (float)currentTime.TotalMilliseconds / (d * 5);
                            t = MathHelper.Clamp(t, 0, 1);
                            spriteBatch.Draw(ug.unit.t_icon, Vector2.Lerp(s, g, t), Color.White);
                        }
                        else
                        {
                            Vector2 g = s + 16 * (Vector2)calcResult[n].forceDir;
                            if (currentTime.TotalMilliseconds < 75)
                            {
                                float t = (float)currentTime.TotalMilliseconds / 75;
                                spriteBatch.Draw(ug.unit.t_icon, Vector2.Lerp(s, g, t), Color.White);
                            }
                            else if (currentTime.TotalMilliseconds < 150)
                            {
                                float t = (float)(currentTime.TotalMilliseconds - 75) / 75;
                                spriteBatch.Draw(ug.unit.t_icon, Vector2.Lerp(g, s, t), Color.White);
                            }
                            else
                                spriteBatch.Draw(ug.unit.t_icon, s, Color.White);
                        }
                    }
                    else
                        spriteBatch.Draw(ug.unit.t_icon, mapOrigin + 64 * (Vector2)ug.postion, Color.White);

                    // 情報表示
                    if ((!battleStart || (actState >= ActState.SelectAct && actState <= ActState.ShowActExplain))
                        && gameTime.TotalGameTime.TotalMilliseconds % 1500 >= 750)
                    {
                        if (ug.symptonMinus.sympton > 0 && ug.symptonMinus.turn > 0)
                        {
                            // プラス症状アイコン
                            Vector2 p;
                            if (ug.ff == FrendOfFoe.Ally)
                                p = mapOrigin + new Vector2(ug.postion.X * 64, ug.postion.Y * 64);
                            else
                                p = mapOrigin + new Vector2(ug.postion.X * 64 + 32, ug.postion.Y * 64);
                            spriteBatch.Draw(t_icon1, p, new Rectangle((int)(ug.symptonMinus.sympton - 1) * 32, 256, 32, 32), Color.White);
                        }
                        if (ug.stance >= 0 && actState != ActState.DisPlus)
                        {
                            // 援護・構えアイコン
                            Vector2 p;
                            if (ug.ff == FrendOfFoe.Ally)
                                p = mapOrigin + new Vector2(ug.postion.X * 64 + 32, ug.postion.Y * 64);
                            else
                                p = mapOrigin + new Vector2(ug.postion.X * 64, ug.postion.Y * 64);
                            switch (ug.Acts[ug.stance].type)
                            {
                                case ActType.Guard:
                                case ActType.LessGuard:
                                case ActType.Utsusemi:
                                    spriteBatch.Draw(t_icon1, p, new Rectangle(256, 256, 32, 32), Color.White);
                                    break;
                                case ActType.Counter:
                                    spriteBatch.Draw(t_icon1, p, new Rectangle(288, 256, 32, 32), Color.White);
                                    break;
                                case ActType.BarrierDefense:
                                case ActType.BarrierSpirit:
                                    spriteBatch.Draw(t_icon1, p, new Rectangle(320, 256, 32, 32), Color.White);
                                    break;
                            }
                        }
                        else if (ug.symptonPlus.sympton > 0 && ug.symptonPlus.turn > 0)
                        {
                            // マイナス症状アイコン
                            Vector2 p;
                            if (ug.ff == FrendOfFoe.Ally)
                                p = mapOrigin + new Vector2(ug.postion.X * 64 + 32, ug.postion.Y * 64);
                            else
                                p = mapOrigin + new Vector2(ug.postion.X * 64, ug.postion.Y * 64);
                            spriteBatch.Draw(t_icon1, p, new Rectangle((int)(ug.symptonPlus.sympton - 1) * 32, 224, 32, 32), Color.White);
                        }
                        if (ug.leader)
                        {
                            // リーダーアイコン
                            Vector2 p = mapOrigin + new Vector2(ug.postion.X * 64, ug.postion.Y * 64 + 32);
                            if (ug.ff == FrendOfFoe.Ally)
                                spriteBatch.Draw(t_icon1, p, new Rectangle(256, 32, 64, 32), Color.White);
                            else
                                spriteBatch.Draw(t_icon1, p, new Rectangle(256, 0, 64, 32), Color.White);
                        }
                    }
                }
            }
            #endregion

            if (battleStart && !IsMapMoving())
            {
                #region 戦闘

                if (currentUnit != null)
                {
                    // 地形情報
                    Terrain tera = GetTip(targetCursor);
                    Helper.DrawStringWithOutLine(Helper.GetStringFieldEffect(Crystal.sympton, Crystal.power), new Vector2(607, 600));
                    Helper.DrawStringWithOutLine(Helper.GetStringTerrain(tera), new Vector2(874, 600));
                    Helper.DrawStringWithOutLine(Helper.GetStringAffinity(GetAffinity(currentUnit, tera)), new Vector2(984, 600));
                }

                #region ターン開始
                if (actState == ActState.Win)
                {
                    if (time < 1000)
                        Helper.DrawWindowBottom1("敵のリーダーが戦闘不能になった！");
                    else
                        Helper.DrawWindowBottom1("そこまで！勝者　" + allUnitGadget[0].unit.name + "！");
                }
                else if (actState == ActState.Lose)
                {
                    if (time < 1000)
                        Helper.DrawWindowBottom1("味方のリーダーが戦闘不能になった！");
                    else
                        Helper.DrawWindowBottom1("そこまで！勝者　" + enemyUnitGadget[0].unit.name + "！");
                }
                else if (actState == ActState.TurnStart)
                {
                    Helper.DrawWindowBottom1("ターン" + turn);
                }
                else if (actState == ActState.APCharge)
                {
                    foreach (UnitGadget ug in allUnitGadget)
                    {
                        if (ug.dead)
                            continue;
                        Vector2 pos = mapOrigin + new Vector2(ug.postion.X * 64 + 6, ug.postion.Y * 64 + 32 - (int)GetIconFact());
                        DrawNumber(ug.postion, (int)GetIconFact(), true, ug.AP, Color.White);
                    }

                    Helper.DrawWindowBottom1("ユニットのAPチャージ!");
                }
                else if (actState == ActState.CrystalEffect)
                {
                    if (crystal_face.sympton > 0 && crystal_face.turn <= 0)
                    {
                        Helper.DrawWindowBottom1("結晶の力の流れが元に戻った！");
                    }
                    else
                    {
                        switch (Crystal.sympton)
                        {
                            case FieldEffect.HPHeal:
                                Helper.DrawWindowBottom1("結晶の力が傷を癒やす！");
                                break;
                            case FieldEffect.HPDamage:
                                Helper.DrawWindowBottom1("結晶の力が体を蝕む！");
                                break;
                            case FieldEffect.ChangeTerrain:
                                Helper.DrawWindowBottom1("結晶の力が地形を変化させる！");
                                break;
                        }
                    }
                }
                else if (actState == ActState.DisPlus)
                {
                    if (time < 750)
                    {
                        foreach (UnitGadget ug in allUnitGadget)
                        {
                            if (!ug.dead && ug.symptonPlus.sympton > 0 && ug.symptonPlus.turn <= 0)
                            {
                                Vector2 p;
                                if (ug.ff == FrendOfFoe.Ally)
                                    p = mapOrigin + new Vector2(ug.postion.X * 64 + 32, ug.postion.Y * 64);
                                else
                                    p = mapOrigin + new Vector2(ug.postion.X * 64, ug.postion.Y * 64);
                                spriteBatch.Draw(t_icon1, p, new Rectangle(352, 32 * (int)(time / 250), 32, 32), Color.White);
                            }
                        }
                    }

                    Helper.DrawWindowBottom1("プラス症状の効果が切れた！");
                }
                else if (actState == ActState.DisMinus || actState == ActState.Dis2)
                {
                    if (time < 750)
                    {
                        foreach (UnitGadget ug in allUnitGadget)
                        {
                            if (!ug.dead && ug.symptonMinus.sympton > 0 && ug.symptonMinus.turn <= 0)
                            {
                                Vector2 p;
                                if (ug.ff == FrendOfFoe.Ally)
                                    p = mapOrigin + new Vector2(ug.postion.X * 64, ug.postion.Y * 64);
                                else
                                    p = mapOrigin + new Vector2(ug.postion.X * 64 + 32, ug.postion.Y * 64);
                                spriteBatch.Draw(t_icon1, p, new Rectangle(320, 32 * (int)(time / 250), 32, 32), Color.White);
                            }
                        }
                    }

                    Helper.DrawWindowBottom1("マイナス症状の効果が切れた！");
                }
                else if (actState == ActState.SymCharge)
                {
                    foreach (UnitGadget ug in allUnitGadget)
                    {
                        if (ug.dead || ug.symptonPlus.sympton != SymptonPlus.Charge)
                            continue;

                        Vector2 pos = mapOrigin + new Vector2(ug.postion.X * 64 + 6, ug.postion.Y * 64 + 32 - (int)GetIconFact());
                        int ap = ug.symptonPlus.power;
                        spriteBatch.Draw(t_icon1, pos, new Rectangle(160, 160, 16, 32), Color.White);
                        if (ap > 10)
                            spriteBatch.Draw(t_icon1, pos + new Vector2(16, 0), new Rectangle(16 * (ap / 10), 160, 16, 32), Color.White);
                        spriteBatch.Draw(t_icon1, pos + new Vector2(32, 0), new Rectangle(16 * (ap % 10), 160, 16, 32), Color.White);
                    }

                    Helper.DrawWindowBottom1("活気のプラス症状の効果発動！");
                }
                else if (actState == ActState.SymHeal)
                {
                    float tt = time / 1000;
                    foreach (UnitGadget ug in allUnitGadget)
                    {
                        if (ug.dead || ug.symptonPlus.sympton != SymptonPlus.Heal || ug.HP >= ug.HPmax)
                            continue;

                        if (time < 1000)
                        {
                            Vector2 v = mapOrigin + 64 * (Vector2)ug.postion + new Vector2(0, - 16 * tt);
                            spriteBatch.Draw(t_icon1, v, new Rectangle(288, 64 * (int)(time % 500 / 125), 64, 64), Color.White);
                            ug.spChargeFact = tt;
                        }
                        else
                        {
                            Vector2 pos = mapOrigin + 64 * (Vector2)ug.postion + new Vector2(18, 32 - (int)GetIconFact(1000));
                            int damage = Math.Abs(ug.HP - ug.HPold);
                            if (damage >= 100)
                                pos.X -= 6;
                            else if (damage < 10)
                                pos.X += 6;
                            spriteBatch.Draw(t_icon1, pos - new Vector2(-16, 0), new Rectangle(160, 160, 16, 32), Color.White);
                            if (damage >= 100)
                            {
                                spriteBatch.Draw(t_icon1, pos, new Rectangle(16 * (damage / 100), 160, 16, 32), Color.White);
                                pos.X += 16;
                            }
                            if (damage >= 10)
                            {
                                spriteBatch.Draw(t_icon1, pos, new Rectangle(16 * ((damage % 100) / 10), 160, 16, 32), Color.White);
                                pos.X += 16;
                            }
                            spriteBatch.Draw(t_icon1, pos, new Rectangle(16 * (damage % 10), 160, 16, 32), Color.White);

                            pos = mapOrigin + (Vector2)ug.postion * 64;
                            spriteBatch.Draw(tw, new Rectangle((int)pos.X, (int)pos.Y + 64, 64, 6), Color.Black);
                            float hp = 46 * MathHelper.Lerp((float)ug.HPold / ug.HPmax, (float)ug.HP / ug.HPmax, MathHelper.Clamp(time / 1000, 0, 1));
                            spriteBatch.Draw(tw, new Rectangle((int)pos.X + 1, (int)pos.Y + 49, (int)hp, 4), Color.LimeGreen);
                        }
                    }

                    Helper.DrawWindowBottom1("再生のプラス症状の効果発動！");
                }
                else if (actState == ActState.SymDamage)
                {
                    float tt = time / 1000;
                    foreach (UnitGadget ug in allUnitGadget)
                    {
                        if (ug.dead || ug.symptonMinus.sympton != SymptonMinus.Damage || ug.HP == 1)
                            continue;

                        if (time < 1000)
                        {
                            Vector2 v = mapOrigin + 64 * (Vector2)ug.postion;
                            spriteBatch.Draw(t_icon1, v, new Rectangle(384, 384 + 64 * (int)(time % 200 / 100), 64, 64), Color.White);
                        }
                        else
                        {
                            Vector2 pos = mapOrigin + 64 * (Vector2)ug.postion + new Vector2(16, 32 - (int)GetIconFact(1000));
                            int damage = Math.Abs(ug.HP - ug.HPold);
                            if (damage >= 100)
                                pos.X -= 6;
                            else if (damage < 10)
                                pos.X += 6;
                            if (damage >= 100)
                            {
                                spriteBatch.Draw(t_icon1, pos, new Rectangle(16 * (damage / 100), 160, 16, 32), Color.White);
                                pos.X += 16;
                            }
                            if (damage >= 10)
                            {
                                spriteBatch.Draw(t_icon1, pos, new Rectangle(16 * ((damage % 100) / 10), 160, 16, 32), Color.White);
                                pos.X += 16;
                            }
                            spriteBatch.Draw(t_icon1, pos, new Rectangle(16 * (damage % 10), 160, 16, 32), Color.White);

                            pos = mapOrigin + (Vector2)ug.postion * 64;
                            spriteBatch.Draw(tw, new Rectangle((int)pos.X, (int)pos.Y + 64, 64, 6), Color.Black);
                            float hp = 46 * MathHelper.Lerp((float)ug.HPold / ug.HPmax, (float)ug.HP / ug.HPmax, MathHelper.Clamp(time / 1000, 0, 1));
                            spriteBatch.Draw(tw, new Rectangle((int)pos.X + 1, (int)pos.Y + 49, (int)hp, 4), Color.LimeGreen);
                        }
                    }

                    Helper.DrawWindowBottom1("継続ダメージのマイナス症状の効果発動！");
                }
                #endregion
                else if (currentUnit == null)
                {
                }
                else if (actState >= ActState.SelectAct && actState <= ActState.SelectTarget)
                {
                    #region // 行動選択
                    if (true)//currentUnit.ff == FrendOfFoe.Ally)
                    {
                        // ユニット情報描画
                        spriteBatch.Draw(t_icon1, new Vector2(10, 10), new Rectangle((int)currentUnit.unit.type * 48, 96, 48, 48), Color.White);
                        if (currentUnit.drive)
                            spriteBatch.Draw(t_icon1, new Vector2(34, 34), new Rectangle((int)currentUnit.unit.type2 * 24, 144, 24, 24), Color.White);
                        Helper.DrawStringWithOutLine(currentUnit.unit.name, new Vector2(64, 16));
                        Helper.DrawStringWithOutLine("HP:", new Vector2(10, 56));
                        str = currentUnit.HP.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(95 - font.MeasureString(str).X, 56));
                        spriteBatch.Draw(tw, new Rectangle(106, 64, 160, 18), Color.White);
                        spriteBatch.Draw(tw, new Rectangle(109, 67, 154, 12), Color.Black);
                        spriteBatch.Draw(tw, new Rectangle(109, 67, 154 * currentUnit.HP / currentUnit.HPmax, 12), Color.LimeGreen);
                        Helper.DrawStringWithOutLine("SP:", new Vector2(10, 80));
                        str = currentUnit.SP.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(95 - font.MeasureString(str).X, 80));
                        spriteBatch.Draw(tw, new Rectangle(106, 88, 160, 18), Color.White);
                        spriteBatch.Draw(tw, new Rectangle(109, 91, 154, 12), Color.Black);
                        spriteBatch.Draw(tw, new Rectangle(109, 91, 154 * currentUnit.SP / currentUnit.SPmax, 12), Color.SkyBlue);
                        Helper.DrawStringWithOutLine("AP:  /", new Vector2(10, 104));
                        str = GetUsingAP(true).ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(80 - font.MeasureString(str).X, 104));
                        str = currentUnit.AP.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(122 - font.MeasureString(str).X, 104));

                        Rectangle rect;
                        #region 行動アイコン
                        if (actState == ActState.SelectAct)
                        {
                            if (currentUnit.unit.IsHaveAbility(Ability.Drive))
                            {
                                if (drive)
                                {
                                    spriteBatch.Draw(t_icon1, new Vector2(210, 160), new Rectangle(320, 96, 64, 32), Color.White);
                                    spriteBatch.Draw(t_icon1, new Vector2(220, 140), new Rectangle(320, 160, 64, 32), Color.White);
                                }
                                else if ((currentUnit.SP >= 10 && currentUnit.AP >= 6) || currentUnit.drive)
                                    spriteBatch.Draw(t_icon1, new Vector2(210, 160), new Rectangle(320, 64, 64, 32), Color.White);
                                else
                                    spriteBatch.Draw(t_icon1, new Vector2(210, 160), new Rectangle(320, 96, 64, 32), Color.White);
                            }

                            rect = new Rectangle(128, 0, 64, 64);
                            Color color;
                            if (selectedAct == 0)
                                color = Color.White;
                            else
                                color = Color.Gray;
                            Vector2 p = new Vector2(192, 420);
                            spriteBatch.Draw(t_icon1, p, rect, color);
                            spriteBatch.Draw(t_icon1, p + new Vector2(16), new Rectangle(0, 64, 32, 32), color);

                            Vector2[] pos = { new Vector2(192, 116), new Vector2(16, 192), new Vector2(368, 192),
                                                new Vector2(16, 344), new Vector2(368, 344) };

                            for (int i = 0; i < 5; i++)
                            {
                                Act a = currentUnit.Acts[i];
                                if (selectedAct == i + 1)
                                    color = Color.White;
                                else
                                    color = Color.Gray;
                                spriteBatch.Draw(t_icon1, pos[i], rect, color);
                                spriteBatch.Draw(t_icon1, pos[i] + new Vector2(16), new Rectangle(GetIconFact(a) * 32, 64, 32, 32), color);
                            }
                            //if (drive)
                            //{new Vector2(160, 191)
                            //    if (selectedAct == 6)
                            //        color = Color.White;
                            //    else
                            //        color = Color.Gray;
                            //    spriteBatch.Draw(t_icon, pos[i], rect, color);
                            //    spriteBatch.Draw(t_icon, pos[i] + new Vector2(16), new Rectangle(GetIconFact(currentUnit.unit.acts[7]) * 32, 64, 32, 32), color);
                            //}
                        }
                        #endregion

                        // 行動詳細
                        if (selectedAct == 0)
                        {
                            Helper.DrawStringWithOutLine("移動", new Vector2(48, 484));
                        }
                        else
                        {
                            if (currentAct != null)
                            {
                                spriteBatch.Draw(t_icon1, new Vector2(8, 485), new Rectangle(GetIconFact(currentAct) * 32, 64, 32, 32), Color.White);
                                Helper.DrawStringWithOutLine(currentAct.name, new Vector2(48, 484));
                                Helper.DrawStringWithOutLine(Helper.GetStringActType(currentAct), new Vector2(68, 520));
                                if (!currentAct.IsLimited)
                                {
                                    Helper.DrawStringWithOutLine("AP:", new Vector2(68, 560));
                                    str = currentAct.ap.ToString();
                                    if (currentUnit.AP >= currentAct.ap)
                                        Helper.DrawStringWithOutLine(str, new Vector2(138 - font.MeasureString(str).X, 560));
                                    else
                                        Helper.DrawStringWithOutline(str, new Vector2(1380 - font.MeasureString(str).X, 560), Color.Red);
                                }
                                else
                                {
                                    Helper.DrawStringWithOutLine("回数:  /", new Vector2(68, 560));
                                    str = currentActCount.ToString();
                                    if (currentActCount > 0)
                                        Helper.DrawStringWithOutLine(str, new Vector2(158 - font.MeasureString(str).X, 560));
                                    else
                                        Helper.DrawStringWithOutline(str, new Vector2(158 - font.MeasureString(str).X, 560), Color.Red);
                                    str = currentAct.count.ToString();
                                    Helper.DrawStringWithOutLine(str, new Vector2(264 - font.MeasureString(str).X, 560));
                                }
                                if (currentAct.IsSpell)
                                {
                                    Helper.DrawStringWithOutLine("SP:", new Vector2(188, 560));
                                    if (currentUnit.SP >= currentAct.sp)
                                        Helper.DrawStringWithOutLine(currentAct.sp.ToString(), new Vector2(230, 560));
                                    else
                                        Helper.DrawStringWithOutline(currentAct.sp.ToString(), new Vector2(230, 560), Color.Red);
                                }
                                Helper.DrawStringWithOutLine(Helper.GetStringActTarget(currentAct.target), new Vector2(68, 600));
                            }
                            else
                                Helper.DrawStringWithOutLine("覚えていない", new Vector2(48, 484));
                        }

                        #region カーソル表示
                        if (actState == ActState.SelectAct)
                        {
                            Vector2 pos = mapOrigin + new Vector2(32 + currentUnit.postion.X * 64, 32 + currentUnit.postion.Y * 64);
                            float r = GetCursorFact(gameTime.TotalGameTime, 30, 8, 300);
                            rect = new Rectangle(0, 0, 32, 32);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(-r, -r), rect, Color.White, 0, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(r, -r), rect, Color.White, MathHelper.PiOver2, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(r, r), rect, Color.White, MathHelper.Pi, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(-r, r), rect, Color.White, -MathHelper.PiOver2, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                        }
                        else if (actState == ActState.SelectMove)
                        {
                            Vector2 pos = mapOrigin + new Vector2(32 + unitCursor.X * 64, 32 + unitCursor.Y * 64);
                            float r = GetCursorFact(gameTime.TotalGameTime, 45, 15, 600);
                            rect = new Rectangle(0, 32, 32, 32);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(0, -r), rect, Color.White, 0, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(r, 0), rect, Color.White, MathHelper.PiOver2, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(0, r), rect, Color.White, MathHelper.Pi, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(-r, 0), rect, Color.White, -MathHelper.PiOver2, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                        }
                        else if (actState == ActState.SelectTarget)
                        {
                            Vector2 pos = unitCursor;
                            if (unitCursor != currentUnit.postion)
                                spriteBatch.Draw(currentUnit.unit.t_icon, mapOrigin + pos * 64, new Color(128, 128, 128, 128));
                            float f;
                            rect = new Rectangle(32, 0, 32, 32);
                            if (currentAct.type != ActType.TransSpace)
                            {
                                bool aim = false;
                                foreach (UnitGadget ug in GetActTarget())
                                {
                                    Positon p = ug.postion;
                                    if (ug == currentUnit)
                                        p = unitCursor;
                                    pos = mapOrigin + 64 * (Vector2)p;
                                    f = GetCursorFact(gameTime.TotalGameTime, 1, 0, 1200);
                                    if (f >= 0.5 || p == targetCursor)
                                    {
                                        Color color;
                                        if (currentAct.IsPassive)
                                            color = Color.White;
                                        else
                                        {
                                            int success, avoid;
                                            bool hit;
                                            GetActSuccessRate(ug, out success, out hit, out avoid);
                                            if (currentAct.TypeInt % 100 == 0)
                                            {
                                                success -= avoid;
                                                if (ug.IsType(UnitType.Fortune))
                                                    success = (int)(success * 0.8);
                                            }

                                            if (success >= 100)
                                                color = Color.Orange;
                                            else if (success >= 80)
                                                color = Color.Lime;
                                            else if (success >= 50)
                                                color = Color.Yellow;
                                            else if (success > 0)
                                                color = Color.Red;
                                            else
                                                color = Color.Gray;
                                        }
                                        spriteBatch.Draw(t_icon1, pos, new Rectangle(64, 0, 64, 64), color);
                                    }
                                    if (p == targetCursor || currentAct.IsTargetAll)
                                    {
                                        pos += new Vector2(32);
                                        f = GetCursorFact2(gameTime.TotalGameTime, 15, 8, 450);
                                        spriteBatch.Draw(t_icon1, pos + new Vector2(-f, -f), rect, Color.White, 0, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                                        spriteBatch.Draw(t_icon1, pos + new Vector2(f, -f), rect, Color.White, MathHelper.PiOver2, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                                        spriteBatch.Draw(t_icon1, pos + new Vector2(f, f), rect, Color.White, MathHelper.Pi, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                                        spriteBatch.Draw(t_icon1, pos + new Vector2(-f, f), rect, Color.White, -MathHelper.PiOver2, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                                        aim = true;
                                    }
                                }
                                if (!aim)
                                {
                                    pos = mapOrigin + 64 * (Vector2)targetCursor + new Vector2(32);
                                    f = GetCursorFact(gameTime.TotalGameTime, 35, 8, 450);
                                    spriteBatch.Draw(t_icon1, pos + new Vector2(-f, -f), rect, Color.White, 0, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                                    spriteBatch.Draw(t_icon1, pos + new Vector2(f, -f), rect, Color.White, MathHelper.PiOver2, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                                    spriteBatch.Draw(t_icon1, pos + new Vector2(f, f), rect, Color.White, MathHelper.Pi, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                                    spriteBatch.Draw(t_icon1, pos + new Vector2(-f, f), rect, Color.White, -MathHelper.PiOver2, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                                }
                            }
                            else
                            {
                                pos = mapOrigin + 64 * (Vector2)targetCursor;
                                spriteBatch.Draw(t_icon1, pos, new Rectangle(64, 0, 64, 64), Color.White);
                                pos += new Vector2(32);
                                f = GetCursorFact2(gameTime.TotalGameTime, 15, 8, 450);
                                spriteBatch.Draw(t_icon1, pos + new Vector2(-f, -f), rect, Color.White, 0, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                                spriteBatch.Draw(t_icon1, pos + new Vector2(f, -f), rect, Color.White, MathHelper.PiOver2, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                                spriteBatch.Draw(t_icon1, pos + new Vector2(f, f), rect, Color.White, MathHelper.Pi, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                                spriteBatch.Draw(t_icon1, pos + new Vector2(-f, f), rect, Color.White, -MathHelper.PiOver2, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                #region // 行動
                else if (actState == ActState.Drive) // ドライヴ
                {
                }
                else if (actState == ActState.Move) // 移動
                {
                    Positon p = unitCursor;
                    while (p != currentUnit.postion)
                    {
                        spriteBatch.Draw(currentUnit.unit.t_icon, mapOrigin + new Vector2(p.X * 64, p.Y * 64), new Color(128, 128, 128, 128));
                        spriteBatch.Draw(t_icon1, mapOrigin + new Vector2(p.X * 64 + 16, p.Y * 64 + 16), new Rectangle(192, 192, 32, 32), Color.White);
                        p = mapCost[p.X, p.Y].parent;
                    }
                    Vector2 pos = mapOrigin + new Vector2(32 + targetCursor.X * 64, 32 + targetCursor.Y * 64);
                    float r = GetCursorFact(gameTime.TotalGameTime, 35, 8, 300);
                    Rectangle rect = new Rectangle(0, 0, 32, 32);
                    spriteBatch.Draw(t_icon1, pos + new Vector2(-r, -r), rect, Color.White, 0, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(t_icon1, pos + new Vector2(r, -r), rect, Color.White, MathHelper.PiOver2, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(t_icon1, pos + new Vector2(r, r), rect, Color.White, MathHelper.Pi, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(t_icon1, pos + new Vector2(-r, r), rect, Color.White, -MathHelper.PiOver2, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                }
                else if (actState == ActState.TargetCursor) // 対象表示
                {
                    float tt = (time % 400) / 400;
                    if (tt > 0.1)
                    {
                        tt = (tt - 0.1f) * 1.11111f;
                        if (currentAct.IsTargetAll)
                        {
                            foreach (UnitGadget ug in targetList)
                            {
                                if (currentAct.IsCover && currentUnit.ff == FrendOfFoe.Enemy && ug != currentUnit)
                                    continue;

                                Vector2 v = mapOrigin + 64 * Vector2.Lerp(unitCursor, ug.postion, tt);
                                spriteBatch.Draw(t_icon1, v, new Rectangle(64, 0, 64, 64), Color.White);
                            }
                        }
                        else
                        {
                            Vector2 v = mapOrigin + 64 * Vector2.Lerp(unitCursor, targetCursor, tt);
                            spriteBatch.Draw(t_icon1, v, new Rectangle(64, 0, 64, 64), Color.White);
                        }
                    }
                }
                else if (actState == ActState.ShowActExplain) // 行動表示
                {
                    Helper.DrawWindowBottom2(Helper.GetStringActType(currentAct) + "\n" + currentAct.name);
                }
                else if (actState == ActState.CalcAct) // 行動計算
                {
                }
                else if (actState == ActState.Attack_Heal) // 攻撃回復
                {
                    #region
                    for (int i = 0; i < targetList.Count; i++)
                    {
                        if (currentAct.IsTargetAll)
                            target = targetList[i];
                        if (currentAct.TypeInt % 100 == 0 && !calcResult[i].hit)
                        {
                            Vector2 pos = mapOrigin + new Vector2(target.postion.X * 64, target.postion.Y * 64 + 8 - (int)GetIconFact());
                            spriteBatch.Draw(t_icon1, pos, new Rectangle(192, 48, 64, 48), Color.White);
                            if (!currentAct.IsTargetAll)
                                Helper.DrawWindowBottom1(target.unit.nickname + "は攻撃を回避した！");
                        }
                        else if (currentAct.TypeInt % 100 == 0 && calcResult[i].damage == 0)
                        {
                            Vector2 pos = mapOrigin + new Vector2(target.postion.X * 64, target.postion.Y * 64 + 8 - (int)GetIconFact());
                            spriteBatch.Draw(t_icon1, pos, new Rectangle(192, 0, 64, 48), Color.White);
                            if (!currentAct.IsTargetAll)
                                Helper.DrawWindowBottom1(target.unit.nickname + "は攻撃を無効化した！");
                        }
                        else
                        {
                            UnitGadget tar = countered ? currentUnit : target;
                            int damage = Math.Abs(tar.HP - tar.HPold);
                            if (currentAct.TypeInt % 100 != 0)
                                DrawNumber(tar.postion, (int)GetIconFact(), true, damage, Color.White);
                            else
                            {
                                if (calcResult[i].critical)
                                    DrawNumber(tar.postion, (int)GetIconFact(), false, damage, new Color(1, 0.64f, 0.3f));
                                else
                                    DrawNumber(tar.postion, (int)GetIconFact(), false, damage, Color.White);
                            }

                            Vector2 pos = mapOrigin + (Vector2)tar.postion * 64;
                            spriteBatch.Draw(tw, new Rectangle((int)pos.X, (int)pos.Y + 64, 64, 8), Color.Black);
                            float hp = 60 * MathHelper.Lerp((float)tar.HPold / tar.HPmax, (float)tar.HP / tar.HPmax, MathHelper.Clamp(time / 1000, 0, 1));
                            spriteBatch.Draw(tw, new Rectangle((int)pos.X + 2, (int)pos.Y + 66, (int)hp, 4), Color.LimeGreen);
                            if (countered)
                                Helper.DrawWindowBottom1(target.unit.nickname + "は攻撃を跳ね返した！");
                        }
                        if (!currentAct.IsTargetAll)
                            break;
                    }
                    if (currentAct.type == ActType.Revive && !currentAct.IsTargetAll)
                        Helper.DrawWindowBottom1(target.unit.nickname + "は戦闘に復帰した！");
                }
                else if (actState == ActState.Heal2)
                {
                    if (time < 750)
                    {
                        foreach (UnitGadget ug in allUnitGadget)
                        {
                            if (!ug.dead && ug.symptonMinus.sympton > 0 && ug.symptonMinus.turn <= 0)
                            {
                                Vector2 p;
                                if (ug.ff == FrendOfFoe.Ally)
                                    p = mapOrigin + new Vector2(ug.postion.X * 64, ug.postion.Y * 64);
                                else
                                    p = mapOrigin + new Vector2(ug.postion.X * 64 + 32, ug.postion.Y * 64);
                                spriteBatch.Draw(t_icon1, p, new Rectangle(256, 64 + 32 * (int)(time / 250), 32, 32), Color.White);
                            }
                        }
                    }

                    Helper.DrawWindowBottom1("マイナス症状にダメージを与えた！");
                    #endregion
                }
                else if (actState >= ActState.AddSympSub && actState <= ActState.AddSympFromAttack)
                {
                    // 付加
                    #region アニメーション
                    float t = time / timeSpan;
                    if (actState == ActState.AddSympSub) // 付加飛ばし
                    {
                        Vector2 v = mapOrigin + new Vector2(32) + 64 * Vector2.Lerp(unitCursor, target.postion, t);
                        if (currentAct.type == ActType.AddMinusSympton)
                        {
                            float r = (float)gameTime.TotalGameTime.TotalMilliseconds / 32;
                            switch ((SymptonMinus)currentAct.sympton)
                            {
                                case SymptonMinus.Confuse:
                                    spriteBatch.Draw(t_icon3, v, new Rectangle(384, 288, 64, 64), Color.White, r, new Vector2(32), 1, SpriteEffects.None, 0);
                                    break;
                                case SymptonMinus.Deguard:
                                case SymptonMinus.Dedodge:
                                    spriteBatch.Draw(t_icon3, v, new Rectangle(288, 288, 64, 64), Color.White, r, new Vector2(32), 1 + t, SpriteEffects.None, 0);
                                    break;
                                case SymptonMinus.FixInside:
                                    if (time < 200)
                                        spriteBatch.Draw(t_icon3, mapOrigin + new Vector2(32) + 64 * (Vector2)unitCursor, new Rectangle(288, 528, 64, 64),
                                            new Color(196, 196, 196, 196), r, new Vector2(32), 1 + time / 200, SpriteEffects.None, 0);
                                    else
                                    {
                                        v = mapOrigin + new Vector2(32) + 64 * Vector2.Lerp(unitCursor, target.postion, (time - 200) / (timeSpan - 200));
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(288, 528, 64, 64), new Color(196, 196, 196, 196),
                                            r, new Vector2(32), 2, SpriteEffects.None, 0);
                                    }
                                    break;
                                case SymptonMinus.FixOutside:
                                    float rr = (float)Math.Atan2(target.postion.Y - unitCursor.Y, target.postion.X - unitCursor.X);
                                    if (time < (timeSpan - 200) / 2)
                                    {
                                        v = mapOrigin + new Vector2(32) + 64 * Vector2.Lerp(unitCursor, target.postion, time / (timeSpan - 200));
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(288, 384, 64, 64), Color.White, rr, new Vector2(64, 32), 1, SpriteEffects.None, 0);
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(288, 384, 64, 64), Color.White, rr + MathHelper.Pi, new Vector2(64, 32), 1, SpriteEffects.FlipVertically, 0);
                                    }
                                    else if (time < timeSpan - (timeSpan - 200) / 2)
                                    {
                                        v = mapOrigin + new Vector2(32) + 64 * Vector2.Lerp(unitCursor, target.postion, 0.5f);
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(288, 384, 64, 64), Color.White, rr, new Vector2(64, 32), 1, SpriteEffects.None, 0);
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(288, 384, 64, 64), Color.White, rr + MathHelper.Pi, new Vector2(64, 32), 1, SpriteEffects.FlipVertically, 0);
                                    }
                                    else
                                    {
                                        v = mapOrigin + new Vector2(32) + 64 * Vector2.Lerp(unitCursor, target.postion, (time - 200) / (timeSpan - 200));
                                        Vector2 v2 = mapOrigin + new Vector2(32) + 64 * Vector2.Lerp(target.postion, unitCursor, (time - 200) / (timeSpan - 200));
                                        spriteBatch.Draw(t_icon3, v2, new Rectangle(288, 384, 64, 64), Color.White, rr, new Vector2(64, 32), 1, SpriteEffects.None, 0);
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(288, 384, 64, 64), Color.White, rr + MathHelper.Pi, new Vector2(64, 32), 1, SpriteEffects.FlipVertically, 0);
                                    }
                                    break;
                            }
                        }
                        else if (currentAct.type == ActType.AddPlusSympton)
                        {
                            switch ((SymptonPlus)currentAct.sympton)
                            {
                                case SymptonPlus.Heal:
                                    spriteBatch.Draw(t_icon3, v - new Vector2(32), new Rectangle(288, 64 * (int)(time % 500 / 125), 64, 64), Color.White);
                                    break;
                                case SymptonPlus.Charge:
                                    if (time % 500 <= 250)
                                        spriteBatch.Draw(t_icon3, v - new Vector2(32), new Rectangle(384, 64 * (int)(time % 500 / 62.5), 64, 64), Color.White);
                                    else
                                        spriteBatch.Draw(t_icon3, v - new Vector2(32), new Rectangle(384, 64 * (int)((500 - time % 500) / 62.5), 64, 64), Color.White);
                                    break;
                                case SymptonPlus.Concentrate:
                                    spriteBatch.Draw(t_icon3, v, new Rectangle(336, 320, 64, 64), Color.White, -time / 500 * MathHelper.TwoPi, new Vector2(32), 1, SpriteEffects.None, 0);
                                    spriteBatch.Draw(t_icon3, v, new Rectangle(288, 320, 64, 64), Color.White, time / 500 * MathHelper.TwoPi, new Vector2(32), 1, SpriteEffects.None, 0);
                                    break;
                                case SymptonPlus.Swift:
                                    spriteBatch.Draw(t_icon3, v - new Vector2(32), new Rectangle(432, 64 * (int)(time % 500 / 125), 64, 64), Color.White);
                                    break;
                                case SymptonPlus.ActAgain:
                                    spriteBatch.Draw(t_icon3, v, new Rectangle(384, 64, 64, 64), Color.White, time / 500 * MathHelper.TwoPi, new Vector2(32), 1, SpriteEffects.None, 0);
                                    break;
                            }
                        }
                        else if (currentAct.type == ActType.ClearMinusSympton)
                        {
                            spriteBatch.Draw(t_icon3, v, new Rectangle(96, 0, 96, 96), Color.White, time / 500 * MathHelper.TwoPi, new Vector2(64), 0.5f, SpriteEffects.None, 0);
                        }
                        else if (currentAct.type == ActType.ClearPlusSympton)
                        {
                            spriteBatch.Draw(t_icon3, v, new Rectangle(256, 0, 96, 96), Color.White, time / 500 * MathHelper.TwoPi, new Vector2(64), 0.5f, SpriteEffects.None, 0);
                        }
                        else if (currentAct.type == ActType.ClearTrap)
                        {
                            spriteBatch.Draw(t_icon4, v, new Rectangle(0, 672, 96, 96), Color.White, 0, new Vector2(64), 1, SpriteEffects.None, 0);
                        }
                        else if (currentAct.type == ActType.SPUp)
                        {
                            spriteBatch.Draw(t_icon3, v, new Rectangle(336, 256, 64, 64), Color.White, 0, new Vector2(32), 1 - ((time % 500) / 500), SpriteEffects.None, 0);
                            spriteBatch.Draw(t_icon3, v, new Rectangle(288, 256, 64, 64), Color.White, -time / 500 * MathHelper.TwoPi, new Vector2(32), 1, SpriteEffects.None, 0);
                        }
                        else if (currentAct.type == ActType.Revive || currentAct.type == ActType.Revive2)
                        {
                            spriteBatch.Draw(t_icon3, v - new Vector2(32), new Rectangle(336, 64 * (int)(time % 500 / 125), 64, 64), Color.White);
                        }
                    }
                    else if (actState == ActState.AddSymp) // 付加
                    {
                        Vector2 v = mapOrigin + new Vector2(32) + 64 * (Vector2)target.postion;
                        if (currentAct.type == ActType.AddMinusSympton)
                        {
                            float r = (float)gameTime.TotalGameTime.TotalMilliseconds / 32;
                            float rr = (float)Math.Atan2(target.postion.Y - unitCursor.Y, target.postion.X - unitCursor.X);
                            switch ((SymptonMinus)currentAct.sympton)
                            {
                                case SymptonMinus.Confuse:
                                    if (!currentAct.IsTargetAll)
                                    {
                                        if (time < 500)
                                            spriteBatch.Draw(t_icon3, v, new Rectangle(384, 288, 64, 64), Color.White, r, new Vector2(32), 1 + time / 500, SpriteEffects.None, 0);
                                        else
                                            spriteBatch.Draw(t_icon3, v + new Vector2(-32, -32 - 16 * (time - 500) / 500), new Rectangle(384, 336, 64, 64), Color.White);
                                    }
                                    break;
                                case SymptonMinus.Deguard:
                                case SymptonMinus.Dedodge:
                                    if (!currentAct.IsTargetAll)
                                    {
                                        if (time < 500)
                                        {
                                            float a = 1 - time / 500;
                                            spriteBatch.Draw(t_icon3, v, new Rectangle(288, 288, 64, 64), new Color(a, a, a, a),
                                                r, new Vector2(32), 2 - time / 500, SpriteEffects.None, 0);
                                        }
                                        else if (time < 1000)
                                        {
                                            int x = currentAct.sympton == (int)SymptonMinus.Deguard ? 288 : 336;
                                            spriteBatch.Draw(t_icon3, v, new Rectangle(x, 336, 64, 64), Color.White,
                                                0, new Vector2(32), 2 - MathHelper.Clamp((time - 500) / 400, 0, 1), SpriteEffects.None, 0);
                                        }
                                        else
                                            spriteBatch.Draw(t_icon3, v, new Rectangle(336, 288, 64, 64), Color.White,
                                                0, new Vector2(32), 2 - MathHelper.Clamp((time - 1000) / 400, 0, 1), SpriteEffects.None, 0);
                                    }
                                    break;
                                case SymptonMinus.FixInside:
                                    if (!currentAct.IsTargetAll)
                                    {
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(288, 528, 64, 64), new Color(196, 196, 196, 196),
                                            r, new Vector2(32), 2 - MathHelper.Clamp(time / 200, 0, 1), SpriteEffects.None, 0);
                                        if (time >= 200)
                                        {
                                            float d = ((time - 200) / 300) % 2 < 1 ? (time - 200) % 300 / 300 : 1 - (time - 200) % 300 / 300;
                                            Vector2 p = mapOrigin + new Vector2(32) + 64 * Vector2.Lerp(unitCursor, target.postion, d);
                                            spriteBatch.Draw(t_icon3, p, new Rectangle(288, 432 + 64 * (((int)(time - 200) / 100) % 2), 96, 64), Color.White,
                                                rr, new Vector2(64, 32), 1, SpriteEffects.None, 0);
                                        }
                                    }
                                    break;
                                case SymptonMinus.FixOutside:
                                    if (!currentAct.IsTargetAll)
                                    {
                                        v = mapOrigin + new Vector2(32) + 64 * (Vector2)unitCursor;
                                        Vector2 v2 = mapOrigin + new Vector2(32) + 64 * (Vector2)target.postion;
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(288, 384, 64, 64), Color.White, rr, new Vector2(64, 32), 1, SpriteEffects.None, 0);
                                        spriteBatch.Draw(t_icon3, v2, new Rectangle(288, 384, 64, 64), Color.White,
                                            rr + MathHelper.Pi, new Vector2(64, 32), 1, SpriteEffects.FlipVertically, 0);
                                        float tt = (t % 0.25f) * 4;
                                        Matrix m = Matrix.CreateRotationZ(rr);
                                        spriteBatch.Draw(t_icon3, v + Vector2.Transform(new Vector2(0, -16), m), new Rectangle(336, 384, 32, 64), Color.White,
                                            rr - MathHelper.PiOver4 / 2, new Vector2(0, 32), 0.5f + 0.5f * tt, SpriteEffects.None, 0);
                                        spriteBatch.Draw(t_icon3, v + Vector2.Transform(new Vector2(0, 16), m), new Rectangle(360, 384, 32, 64), Color.White,
                                            rr + MathHelper.PiOver4 / 2, new Vector2(0, 32), 0.5f + 0.5f * tt, SpriteEffects.None, 0);
                                        spriteBatch.Draw(t_icon3, v2 + Vector2.Transform(new Vector2(0, -16), m), new Rectangle(336, 384, 32, 64), Color.White,
                                            rr + MathHelper.Pi + MathHelper.PiOver4 / 2, new Vector2(0, 32), 0.5f + 0.5f * tt, SpriteEffects.None, 0);
                                        spriteBatch.Draw(t_icon3, v2 + Vector2.Transform(new Vector2(0, 16), m), new Rectangle(360, 384, 32, 64), Color.White,
                                            rr + MathHelper.Pi - MathHelper.PiOver4 / 2, new Vector2(0, 32), 0.5f + 0.5f * tt, SpriteEffects.None, 0);
                                    }
                                    break;
                            }
                        }
                        else if (currentAct.type == ActType.AddPlusSympton)
                        {
                            switch ((SymptonPlus)currentAct.sympton)
                            {
                                case SymptonPlus.Heal:
                                    spriteBatch.Draw(t_icon3, v + new Vector2(-32, -32 - 16 * t), new Rectangle(288, 64 * (int)(time % 500 / 125), 64, 64), Color.White);
                                    target.spChargeFact = time / 1000;
                                    break;
                                case SymptonPlus.Charge:
                                    if (time <= 500)
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(0, 256, 96, 96), new Color(128, 128, 128, 128), 0, new Vector2(64), time / 500, SpriteEffects.None, 0);
                                    else if (time <= 1500)
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(0, 288 + 96 * (int)((time - 500) / 200), 96, 96), new Color(128, 128, 128, 128), 0, new Vector2(64), 1, SpriteEffects.None, 0);
                                    else if (time <= 2500)
                                        target.spChargeFact = (time - 1500) / 1000;
                                    break;
                                case SymptonPlus.Concentrate:
                                    spriteBatch.Draw(t_icon3, v, new Rectangle(336, 320, 64, 64), Color.White, -time / 500 * MathHelper.TwoPi, new Vector2(32), 1.5f - t, SpriteEffects.None, 0);
                                    spriteBatch.Draw(t_icon3, v, new Rectangle(288, 320, 64, 64), Color.White, time / 500 * MathHelper.TwoPi, new Vector2(32), 1, SpriteEffects.None, 0);
                                    target.spChargeFact = time / 2000;
                                    break;
                                case SymptonPlus.Swift:
                                    spriteBatch.Draw(t_icon3, v + new Vector2(-32, -32 - 16 * t), new Rectangle(432, 64 * (int)(time % 250 / 62.5), 64, 64), Color.White);
                                    target.spChargeFact = time / 2000;
                                    break;
                                case SymptonPlus.ActAgain:
                                    if (time <= 500)
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(0, 256, 96, 96), new Color(128, 128, 128, 128), 0, new Vector2(64), time / 500, SpriteEffects.None, 0);
                                    else if (time <= 1500)
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(0, 288 + 96 * (int)((time - 500) / 200), 96, 96), new Color(128, 128, 128, 128), 0, new Vector2(64), 1, SpriteEffects.None, 0);
                                    else if (time <= 2500)
                                    {
                                        Vector2 pos = mapOrigin + new Vector2(targetCursor.X * 64 + 16, targetCursor.Y * 64 + 32 - (int)GetIconFact(1500));
                                        spriteBatch.Draw(t_icon1, pos, new Rectangle(160, 160, 16, 32), Color.White);
                                        spriteBatch.Draw(t_icon1, pos + new Vector2(16, 0), new Rectangle(16 * currentAct.power, 160, 16, 32), Color.White);
                                    }
                                    break;
                            }
                        }
                        else if (currentAct.type == ActType.ClearMinusSympton || currentAct.type == ActType.ClearPlusSympton)
                        {
                            int x = currentAct.type == ActType.ClearMinusSympton ? 96 : 256;
                            if (time <= 500)
                                spriteBatch.Draw(t_icon3, v, new Rectangle(x, 0, 96, 96), Color.White, time / 500 * MathHelper.TwoPi, new Vector2(64), 0.25f + time / 1500, SpriteEffects.None, 0);
                            else if (time <= 900)
                                spriteBatch.Draw(t_icon3, v, new Rectangle(x, 96 + 96 * (int)((time - 500) / 134), 96, 96), Color.White, 0, new Vector2(64), 1, SpriteEffects.None, 0);
                            else if (time <= 1000)
                            {// なし
                            }
                            else
                                spriteBatch.Draw(t_icon3, v, new Rectangle(x, 384 + 96 * (int)((time - 1000) / 125), 96, 96), Color.White, 0, new Vector2(64), 1, SpriteEffects.None, 0);
                        }
                        else if (currentAct.type == ActType.ClearTrap)
                        {
                            if (time < 500)
                                spriteBatch.Draw(t_icon4, v, new Rectangle(96 * (int)((time % 250) / 125), 672, 96, 96), Color.White, 0, new Vector2(64), 1, SpriteEffects.None, 0);
                            else
                            {
                                float a = 1 - (time - 500) / 500;
                                spriteBatch.Draw(t_icon4, v, new Rectangle(0, 672, 96, 96), new Color(a, a, a, a), 0, new Vector2(64), 1, SpriteEffects.None, 0);
                            }
                        }
                        else if (currentAct.type == ActType.SPUp)
                        {
                            if (currentTime.TotalMilliseconds < 2000)
                                DrawSPCharge(targetCursor, (time / 1000) % 1);
                            target.spChargeFact = time / 3000;
                        }
                        else if (currentAct.type == ActType.Revive || currentAct.type == ActType.Revive2)
                        {
                            if (!currentAct.IsTargetAll)
                            {
                                spriteBatch.Draw(t_icon3, v + new Vector2(-32, -32 - 16 * t), new Rectangle(336, 64 * (int)(time % 500 / 125), 64, 64), Color.White);
                                target.spChargeFact = time / 1000;
                            }
                            else
                            {
                                float d;
                                if (time < 1200)
                                    d = 160 * (float)Math.Sin(time / 1200 * MathHelper.PiOver2);
                                else if (time < 2400)
                                    d = 100 + 20 * (float)Math.Cos((time - 1200) / 1200 * MathHelper.Pi);
                                else
                                    d = 140 + 60 * -(float)Math.Cos((time - 2400) / 1600 * MathHelper.Pi);
                                for (int i = 0; i < 6; i++)
                                {
                                    spriteBatch.Draw(t_icon3, mapOrigin + new Vector2(288) + Helper.GetPolarCoord(d, (time / 2000 + 0.166f * i) * MathHelper.TwoPi),
                                        new Rectangle(336, 64 * (int)(time % 500 / 125), 64, 64), Color.White, 0, new Vector2(32), 1, SpriteEffects.None, 0);
                                }
                            }
                        }
                    }
                    #endregion

                    #region 説明
                    if (currentAct.TypeInt % 100 == 0 || currentAct.type == ActType.AddMinusSympton)
                    {
                        switch ((SymptonMinus)currentAct.sympton)
                        {
                            case SymptonMinus.Damage:
                                Helper.DrawWindowBottom1("継続のマイナス症状！ターン始めにダメージ！");
                                break;
                            case SymptonMinus.Distract:
                                Helper.DrawWindowBottom1("散漫のマイナス症状！行動が失敗しやすくなる！");
                                break;
                            case SymptonMinus.Restraint:
                                Helper.DrawWindowBottom1("束縛のマイナス症状！移動のAPが多くなる！");
                                break;
                            case SymptonMinus.Stop:
                                Helper.DrawWindowBottom1("停止のマイナス症状！APが失われ行動が出来ない！");
                                break;
                            case SymptonMinus.Confuse:
                                Helper.DrawWindowBottom1("混乱のマイナス症状！行動が決められない！");
                                break;
                            case SymptonMinus.Dedodge:
                                Helper.DrawWindowBottom1("回避不能のマイナス症状！回避が出来ない！");
                                break;
                            case SymptonMinus.Deguard:
                                Helper.DrawWindowBottom1("防御不能のマイナス症状！防御が出来ない！");
                                break;
                            case SymptonMinus.FixInside:
                                Helper.DrawWindowBottom1(calcResult[0].sympPower + "グリッド以上離れられない！");
                                break;
                            case SymptonMinus.FixOutside:
                                Helper.DrawWindowBottom1(calcResult[0].sympPower + "グリッド以内に近づくことが出来ない！");
                                break;
                            case SymptonMinus.CarvedSeal:
                                Helper.DrawWindowBottom1("刻印のマイナス症状！対負攻撃に弱くなる！");
                                break;
                            case SymptonMinus.Stigmata:
                                Helper.DrawWindowBottom1("聖痕のプラス症状！対正攻撃に弱くなる！");
                                break;
                        }
                    }
                    else if (currentAct.type == ActType.AddPlusSympton)
                    {
                        switch ((SymptonPlus)currentAct.sympton)
                        {
                            case SymptonPlus.Heal:
                                Helper.DrawWindowBottom1("再生のプラス症状！ターン始めに回復！");
                                break;
                            case SymptonPlus.Charge:
                                Helper.DrawWindowBottom1("活気のプラス症状！APがアップする！");
                                break;
                            case SymptonPlus.Concentrate:
                                Helper.DrawWindowBottom1("集中のプラス症状！攻撃の成功がアップ！");
                                break;
                            case SymptonPlus.Swift:
                                Helper.DrawWindowBottom1("俊足のプラス症状！移動のAPが少なくなる！");
                                break;
                            case SymptonPlus.ActAgain:
                                Helper.DrawWindowBottom1("1ターンに複数回行動できる！");
                                break;
                        }
                    }
                    else if (currentAct.type == ActType.AddDoubleSympton)
                        Helper.DrawWindowBottom1("聖痕と刻印を刻んだ！対正対負攻撃に弱くなる！");
                    else if (currentAct.type == ActType.SPUp)
                        Helper.DrawWindowBottom1("SPが" + calcResult[0].damage + "ポイント増加した！");
                    else if (currentAct.type == ActType.Revive)
                        Helper.DrawWindowBottom1("倒れた味方を復活させる！");
                    else if (currentAct.type == ActType.Revive2)
                        Helper.DrawWindowBottom1("死者も生者も全てを癒す！");
                    #endregion
                }
                else if (actState == ActState.ClearSymp) // プラスマイナストラップクリア
                {
                    #region アニメーション
                    Vector2 v = mapOrigin + new Vector2(32) + 64 * (Vector2)target.postion;
                    if (currentAct.type == ActType.ClearMinusSympton || currentAct.type == ActType.ClearPlusSympton)
                    {
                        int x = currentAct.type == ActType.ClearMinusSympton ? 96 : 256;
                        if (time <= 500)
                            spriteBatch.Draw(t_icon3, v, new Rectangle(x, 0, 96, 96), Color.White, time / 500 * MathHelper.TwoPi, new Vector2(64), 0.25f + time / 1500, SpriteEffects.None, 0);
                        else if (time <= 900)
                            spriteBatch.Draw(t_icon3, v, new Rectangle(x, 96 + 96 * (int)((time - 500) / 134), 96, 96), Color.White, 0, new Vector2(64), 1, SpriteEffects.None, 0);
                        else if (time <= 1000)
                        {// なし
                        }
                        else if (time <= 1750)
                        {
                            Vector2 p;
                            if (currentAct.type == ActType.ClearMinusSympton)
                            {
                                x = 256;
                                if (target.ff == FrendOfFoe.Ally)
                                    p = mapOrigin + 64 * (Vector2)target.postion;
                                else
                                    p = mapOrigin + 64 * (Vector2)target.postion + new Vector2(32, 0);
                            }
                            else
                            {
                                x = 288;
                                if (target.ff == FrendOfFoe.Ally)
                                    p = mapOrigin + 64 * (Vector2)target.postion + new Vector2(32, 0);
                                else
                                    p = mapOrigin + 64 * (Vector2)target.postion;
                            }
                            spriteBatch.Draw(t_icon1, p, new Rectangle(x, 64 + 32 * (int)((time - 1000) / 250), 32, 32), Color.White);
                        }
                    }
                    else
                    {
                        Vector2 vc = mapOrigin + new Vector2(288);
                        Rectangle rect = new Rectangle(0, 672, 96, 96);
                        if (time < 1000)
                        {
                            float t = time / 1000;
                            float s = 1 + t;
                            spriteBatch.Draw(t_icon4, Vector2.Lerp(v, vc, t), rect, Color.White, 0, new Vector2(64), s, SpriteEffects.None, 0);
                        }
                        else if (time < 3000)
                        {
                            spriteBatch.Draw(t_icon4, vc, rect, Color.White, 0, new Vector2(64), 2, SpriteEffects.None, 0);
                        }
                        else if (target.trap.turn <= 0)
                        {
                        }
                        else
                        {
                            if (time < 4000)
                            {
                                float t = (time - 3000) / 1000;
                                float s = 2 - t;
                                spriteBatch.Draw(t_icon4, Vector2.Lerp(vc, v, t), rect, Color.White, 0, new Vector2(64), s, SpriteEffects.None, 0);
                            }
                            else
                            {
                                float a = 1 - (time - 4000) / 500;
                                spriteBatch.Draw(t_icon4, v, rect, new Color(a, a, a, a), 0, new Vector2(64), 1, SpriteEffects.None, 0);
                            }
                        }
                    }
                    #endregion
                }
                else if (actState == ActState.ClearResult)
                {
                    #region 説明
                    switch (currentAct.type)
                    {
                        case ActType.ClearMinusSympton:
                            if (!IsAlly(target) && target.symptonMinus.sympton == SymptonMinus.None)
                                Helper.DrawWindowBottom1("症状クリアをクリア！症状をつけられる！");
                            else if (IsAlly(target) && target.symptonMinus.sympton == SymptonMinus.Invalid)
                                Helper.DrawWindowBottom1("マイナス症状をクリア！症状をつけられない！");
                            else
                                Helper.DrawWindowBottom1("マイナス症状にダメージを与えた！");
                            break;
                        case ActType.ClearPlusSympton:
                            if (IsAlly(target) && target.symptonPlus.sympton == SymptonPlus.None)
                                Helper.DrawWindowBottom1("症状クリアをクリア！症状をつけられる！");
                            else if (!IsAlly(target) && target.symptonPlus.sympton == SymptonPlus.Invalid)
                                Helper.DrawWindowBottom1("プラス症状をクリア！症状をつけられない！");
                            else
                                Helper.DrawWindowBottom1("プラス症状にダメージを与えた！");
                            break;
                        case ActType.ClearTrap:
                            if (IsAlly(target) && target.trap.sympton == Trap.None)
                                Helper.DrawWindowBottom1("トラップクリアをクリア！トラップをセットできる！");
                            else if (!IsAlly(target) && target.trap.sympton == Trap.TrapClear)
                                Helper.DrawWindowBottom1("トラップをクリア！トラップをセットできなくなった！");
                            else
                                Helper.DrawWindowBottom1("トラップにダメージを与えた！");
                            break;
                    }
                    #endregion
                }
                else if (actState == ActState.Drain) // SP、レベルドレイン
                {
                    #region
                    Vector2 v = mapOrigin + new Vector2(32) + 64 * (Vector2)unitCursor;
                    Rectangle rect = new Rectangle();
                    if (currentAct.type == ActType.SPDrain)
                    {
                        rect = new Rectangle(384, 256, 64, 64);
                        str = "SPを" + calcResult[0].damage + "ポイント吸収した！";
                    }
                    else if (currentAct.type == ActType.LevelDrain)
                    {
                        rect = new Rectangle(384, 320, 64, 64);
                        str = "相手のレベルを奪いとった！";
                    }
                    for (int i = 0; i < targetList.Count; i++)
                    {
                        UnitGadget tar = currentAct.IsTargetAll ? targetList[i] : target;
                        for (int j = 0; j < 8; j++)
                        {
                            float t = time - i * 200 - j * 125;
                            if (t >= 0 && t < 1000)
                            {
                                Vector2 s = mapOrigin + 64 * (Vector2)tar.postion;
                                Vector2 r = s + Helper.GetPolarCoord(96, MathHelper.PiOver4 * j - MathHelper.PiOver2);
                                Vector2 g = mapOrigin + 64 * (Vector2)currentUnit.postion;
                                Vector2 pos;
                                if (t < 500)
                                    pos = Vector2.Lerp(s, r, (float)Math.Sin(MathHelper.PiOver2 * t / 500));
                                else
                                    pos = Vector2.Lerp(g, r, (float)Math.Cos(MathHelper.PiOver2 * (t - 500) / 500));
                                spriteBatch.Draw(t_icon3, pos, rect, Color.White);
                            }
                        }
                        if (!currentAct.IsTargetAll)
                            break;
                    }
                    Helper.DrawWindowBottom1(str);
                    #endregion
                }
                else if (actState == ActState.SetTrap) // トラップ設置
                {
                    #region アニメーション
                    Rectangle rect = new Rectangle(0, 96 * (currentAct.sympton - 1), 96, 96);
                    if (time < 1000)
                    {
                        float t = time / 1000;
                        Vector2 p1 = mapOrigin + new Vector2(currentUnit.postion.X * 64 + 32, currentUnit.postion.Y * 64 + 32);
                        Vector2 p2 = mapOrigin + new Vector2(288);
                        float s = 1 + t;
                        spriteBatch.Draw(t_icon4, Vector2.Lerp(p1, p2, t), rect, Color.White, 2 * MathHelper.TwoPi * t, new Vector2(64), s, SpriteEffects.None, 0);
                    }
                    else if (time < 1500)
                    {
                        spriteBatch.Draw(t_icon4, mapOrigin + new Vector2(288), rect, Color.White, 0, new Vector2(64), 2, SpriteEffects.None, 0);
                    }
                    else if (time < 2500)
                    {
                        float t = (time - 1500) / 1000;
                        Vector2 p1 = mapOrigin + new Vector2(288);
                        float s = 2 - t;
                        if (!currentAct.IsTargetAll)
                        {
                            Vector2 p2 = mapOrigin + new Vector2(target.postion.X * 64 + 32, target.postion.Y * 64 + 32);
                            spriteBatch.Draw(t_icon4, Vector2.Lerp(p1, p2, t), rect, Color.White, 2 * MathHelper.TwoPi * t, new Vector2(64), s, SpriteEffects.None, 0);
                        }
                        else
                        {
                            foreach (UnitGadget ug in targetList)
                            {
                                Vector2 p2 = mapOrigin + new Vector2(ug.postion.X * 64 + 32, ug.postion.Y * 64 + 32);
                                spriteBatch.Draw(t_icon4, Vector2.Lerp(p1, p2, t), rect, Color.White, 2 * MathHelper.TwoPi * t, new Vector2(64), s, SpriteEffects.None, 0);
                            }
                        }
                    }
                    else if (time < 3000)
                    {
                        float t = (time - 2500) / 500;
                        float a = 1 - t;
                        if (!currentAct.IsTargetAll)
                        {
                            Vector2 p = mapOrigin + new Vector2(target.postion.X * 64 + 32, target.postion.Y * 64 + 32);
                            spriteBatch.Draw(t_icon4, p, rect, new Color(a, a, a, a), 0, new Vector2(64), 1, SpriteEffects.None, 0);
                        }
                        else
                        {
                            foreach (UnitGadget ug in targetList)
                            {
                                Vector2 p = mapOrigin + new Vector2(ug.postion.X * 64 + 32, ug.postion.Y * 64 + 32);
                                spriteBatch.Draw(t_icon4, p, rect, new Color(a, a, a, a), 0, new Vector2(64), 1, SpriteEffects.None, 0);
                            }
                        }
                    }
                    #endregion

                    #region 説明
                    switch ((Trap)currentAct.sympton)
                    {
                        case Trap.GrappleTrap:
                            Helper.DrawWindowBottom1("格闘トラップで格闘攻撃にダメージ！");
                            break;
                        case Trap.ShotTrap:
                            Helper.DrawWindowBottom1("射撃トラップで射撃攻撃にダメージ！");
                            break;
                        case Trap.AttackTrap:
                            Helper.DrawWindowBottom1("攻撃トラップで全ての攻撃にダメージ！");
                            break;
                        case Trap.OnceClear:
                            Helper.DrawWindowBottom1("単発クリアで1回だけ攻撃を無効にする！");
                            break;
                        case Trap.SPPlant:
                            Helper.DrawWindowBottom1("SPプラントで行動するたびSPがアップ！");
                            break;
                        case Trap.HitPromise:
                            Helper.DrawWindowBottom1("絶対命中で1回だけ攻撃が必中！");
                            break;
                        case Trap.MagicCharge:
                            Helper.DrawWindowBottom1(currentUnit.unit.nickname + "から大きな力を感じる！");
                            break;
                    }
                    #endregion
                }
                else if (actState == ActState.SetCrystal) // 結晶解放
                {
                    #region アニメーション
                    if (time <= 4000)
                    {
                        Vector2 pos = mapOrigin + new Vector2(288);
                        if (currentAct.fact == '氷')
                        {
                            float d;
                            if (time < 1200)
                                d = 160 * (float)Math.Sin(time / 1200 * MathHelper.PiOver2);
                            else if (time < 2400)
                                d = 100 + 20 * (float)Math.Cos((time - 1200) / 1200 * MathHelper.Pi);
                            else
                                d = 140 + 60 * -(float)Math.Cos((time - 2400) / 1600 * MathHelper.Pi);
                            for (int i = 0; i < 6; i++)
                            {
                                spriteBatch.Draw(t_icon3, pos + Helper.GetPolarCoord(d, (time / 2000 + 0.166f * i) * MathHelper.TwoPi),
                                    new Rectangle(432, 528, 64, 64), Color.White, 0, new Vector2(32), 1, SpriteEffects.None, 0);
                            }
                        }
                    }
                    #endregion

                    #region 説明
                    switch ((FieldEffect)currentAct.sympton)
                    {
                        case FieldEffect.HPDamage:
                            Helper.DrawWindowBottom1("HPダメージの結晶効果！ターン始めにダメージ！");
                            break;
                        case FieldEffect.HPHeal:
                            Helper.DrawWindowBottom1("HP回復の結晶効果！ターン始めに回復！");
                            break;
                        case FieldEffect.APUp:
                            Helper.DrawWindowBottom1("APアップの結晶効果！APがアップする！");
                            break;
                        case FieldEffect.APDown:
                            Helper.DrawWindowBottom1("APダウンの結晶効果！APがダウンする！");
                            break;
                        case FieldEffect.HitUp:
                            Helper.DrawWindowBottom1("成功アップの結晶効果！全ての成功がアップ！");
                            break;
                        case FieldEffect.CostUp:
                            Helper.DrawWindowBottom1("コスト増加の結晶効果！移動コストが増える！");
                            break;
                        case FieldEffect.AffinityDown:
                            Helper.DrawWindowBottom1("適正ダウンの結晶効果！地形相性がダウンする！");
                            break;
                        case FieldEffect.AffinityReverse:
                            Helper.DrawWindowBottom1("相性反転の結晶効果！地形相性が逆になる！");
                            break;
                        case FieldEffect.SympInvalid:
                            Helper.DrawWindowBottom1("症状クリアの結晶効果！症状を付加できない！");
                            break;
                        case FieldEffect.DamageFix:
                            Helper.DrawWindowBottom1("ダメージ固定の結晶効果！攻撃ダメージが" + currentAct.power + "になる！");
                            break;
                        case FieldEffect.TimeStop:
                            Helper.DrawWindowBottom1("時間停止の結晶効果！時間が止まり行動できない！");
                            break;
                    }
                    #endregion
                }
                else if (actState == ActState.Search_Hide) // 索敵隠蔽能力アップ
                {
                    #region アニメーション
                    #endregion

                    #region 説明
                    switch (currentAct.type)
                    {
                        case ActType.SearchEnemy:
                            Helper.DrawWindowBottom1("索敵がアップ！敵に攻撃を当てやすくなった！");
                            break;
                        case ActType.Hide:
                            Helper.DrawWindowBottom1("隠蔽がアップ！敵の攻撃を避けやすくなった！");
                            break;
                        case ActType.UpSpeed:
                            Helper.DrawWindowBottom1("推進の能力値が" + calcResult[0].damage + "アップ！");
                            break;
                        case ActType.UpClose:
                            Helper.DrawWindowBottom1("近接の能力値が" + calcResult[0].damage + "アップ！");
                            break;
                        case ActType.UpFar:
                            Helper.DrawWindowBottom1("遠隔の能力値が" + calcResult[0].damage + "アップ！");
                            break;
                        case ActType.UpReact:
                            Helper.DrawWindowBottom1("反応がアップ！索敵と隠蔽が上がった！");
                            break;
                        case ActType.ClearParameter:
                            Helper.DrawWindowBottom1("能力をクリア！索敵や隠蔽も元に戻った！");
                            break;
                    }
                    #endregion
                }
                else if (actState == ActState.Cover_Stance) // 援護構え
                {
                    #region アニメーション
                    Vector2 v = mapOrigin + 64 * (Vector2)currentUnit.postion + new Vector2(32);
                    int x = currentAct.IsCover ? 480 : 528;
                    if (time < 2000)
                    {
                        float t = time / 2000;
                        int y = time < 100 || time >= 1900 ? 0 : (time < 200 || time >= 1800 ? 96 : 256);
                        float r = time < 1800 ? -MathHelper.TwoPi * time / 1800 : 0;
                        if (time < 500)
                            spriteBatch.Draw(t_icon3, v, new Rectangle(0, 256, 96, 96), new Color(128, 128, 128, 128), 0, new Vector2(64), time / 500, SpriteEffects.None, 0);
                        else if (time < 1500)
                            spriteBatch.Draw(t_icon3, v, new Rectangle(0, 288 + 96 * (int)((time - 500) / 200), 96, 96), new Color(128, 128, 128, 128), 0, new Vector2(64), 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(t_icon3, v, new Rectangle(480, y, 96, 96), Color.White, r, new Vector2(32, 96), 1, SpriteEffects.None, 0);
                    }
                    else if (time < 2500)
                    {
                        float t = (time - 2000) / 500;
                        spriteBatch.Draw(t_icon3, v, new Rectangle(x, 288, 64, 64), Color.White, 0, new Vector2(32), 2 - t, SpriteEffects.None, 0);
                    }
                    else
                    {
                        float t = (time - 2500) / 500;
                        spriteBatch.Draw(t_icon3, v - new Vector2(32), new Rectangle(x, 288 + 64 * ((int)(t * 5) % 4), 64, 64), Color.White);
                    }
                    #endregion

                    #region 説明
                    switch (currentAct.type)
                    {
                        case ActType.Guard:
                        case ActType.LessGuard:
                        case ActType.Utsusemi:
                            Helper.DrawWindowBottom1("敵の攻撃から味方を援護します！");
                            break;
                        case ActType.Counter:
                            Helper.DrawWindowBottom1("構えの体勢を取り敵の攻撃に備えます！");
                            break;
                        case ActType.BarrierDefense:
                            Helper.DrawWindowBottom1("結界を張り味方の防御を高めます！");
                            break;
                        case ActType.BarrierSpirit:
                            Helper.DrawWindowBottom1("結界を張り味方の反応を高めます！");
                            break;
                    }
                    #endregion
                }
                else if (actState == ActState.CoverRun) // 援護発生
                {
                    Vector2 vc = mapOrigin + new Vector2(288);
                    if (time < 300)
                    {
                        float t = time / 300;
                        Vector2 v = mapOrigin + 64 * (Vector2)target.postion + new Vector2(32);
                        spriteBatch.Draw(t_icon3, Vector2.Lerp(v, vc, t), new Rectangle(480, 288, 64, 64), Color.White, 0, new Vector2(32), 1 + t, SpriteEffects.None, 0);
                    }
                    else if (time < 800)
                    {
                        float t = (time - 300) / 500;
                        spriteBatch.Draw(t_icon3, vc, new Rectangle(480, 288 + 64 * ((int)(t * 5) % 4), 64, 64), Color.White, 0, new Vector2(32), 2, SpriteEffects.None, 0);
                    }
                    else if (time < 1100)
                    {
                        float t = (time - 800) / 300;
                        Vector2 v = mapOrigin + 64 * (Vector2)target_old.postion + new Vector2(32);
                        spriteBatch.Draw(t_icon3, Vector2.Lerp(vc, v, t), new Rectangle(480, 288, 64, 64), Color.White, 0, new Vector2(32), 2 - t, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(t_icon3, mapOrigin + 64 * (Vector2)target_old.postion, new Rectangle(480, 288, 64, 64), Color.White);
                    }
                    Helper.DrawWindowBottom1(target.unit.nickname + "が援護に入った！");
                }
                else if (actState == ActState.TransSpace1 || actState == ActState.TransSpace2) // 空間移動
                {
                    bool on = false;
                    foreach(UnitGadget ug in allUnitGadget)
                        if (ug != currentUnit && (ug.postion == targetCursor || ug.postion == unitCursor))
                        {
                            on = true;
                            break;
                        }
                    if (!on)
                        Helper.DrawWindowBottom1(currentUnit.unit.nickname + "は空間を超えて移動した！");
                    else
                        Helper.DrawWindowBottom1("対象と位置を入れ替えた！");
                }
                else if (actState == ActState.DefeatEnemy) // 敵撃破
                {
                    if (time < 750)
                    {
                        for (int i = 0; i < targetList.Count; i++)
                        {
                            if (targetList[i].HP > 0)
                                continue;
                            Vector2 pos = mapOrigin + new Vector2(32) + 64 * (Vector2)targetList[i].postion;
                            for (int j = 0; j < 8; j++)
                            {
                                spriteBatch.Draw(t_icon1, pos + Helper.GetPolarCoord(64 * time / 750, j * MathHelper.PiOver4),
                                    new Rectangle(224, 160, 16, 16), Color.White, 0, new Vector2(8), 1, SpriteEffects.None, 0);
                            }
                        }
                    }
                    if (!currentAct.IsTargetAll)
                        str = target.unit.nickname;
                    else
                        str = "ユニット";
                    Helper.DrawWindowBottom1(str + "は戦闘不能になった！");
                }
                else if (actState == ActState.Defeated) // 自滅
                {
                    if (time < 750)
                    {
                        Vector2 pos = mapOrigin + new Vector2(32) + 64 * (Vector2)currentUnit.postion;
                        for (int j = 0; j < 8; j++)
                        {
                            spriteBatch.Draw(t_icon1, pos + Helper.GetPolarCoord(64 * time / 750, j * MathHelper.PiOver4),
                                new Rectangle(224, 160, 16, 16), Color.White, 0, new Vector2(8), 1, SpriteEffects.None, 0);
                        }
                    }
                    Helper.DrawWindowBottom1(currentUnit.unit.nickname + "は戦闘不能になった！");
                }
                else if (actState == ActState.SymActAgain) // 連続行動
                {
                    if (currentUnit.symptonPlus.sympton == SymptonPlus.ActAgain)
                        Helper.DrawWindowBottom1("プラス症状によりもう一度行動が可能になった！");
                    else
                        Helper.DrawWindowBottom1("アビリティによりもう一度行動が可能になった！");
                }
                else if (actState == ActState.LeakDP) // SP減少
                {
                    if (currentTime.TotalMilliseconds < 1500)
                    {
                        DrawSPCharge(currentUnit.postion, 1 - ((float)currentTime.TotalMilliseconds / 1500));
                    }
                    currentUnit.spChargeFact = (float)(currentTime.TotalMilliseconds / 2000);
                }
                else if (actState == ActState.DriveOff) // ドライヴ解除
                {
                    Helper.DrawWindowBottom1("ドライヴが解除された！");
                }
                else if (actState == ActState.SPCharge) // SP溜め
                {
                    if (currentTime.TotalMilliseconds < 2000)
                        DrawSPCharge(currentUnit.postion, (float)(currentTime.TotalMilliseconds / 1000) % 1);
                    currentUnit.spChargeFact = (float)(currentTime.TotalMilliseconds / 3000);

                    Helper.DrawWindowBottom1("SPが" + currentUnit.AP + "ポイント増加した！");
                }
                else if (actState == ActState.Failure1 || actState == ActState.Failure2) // 失敗
                {
                    #region
                    Vector2 pos = mapOrigin + new Vector2(currentUnit.postion.X * 64, currentUnit.postion.Y * 64 + 8 - (int)GetIconFact());
                    spriteBatch.Draw(t_icon1, pos, new Rectangle(192, 0, 64, 32), Color.White);
                    if (actState == ActState.Failure1)
                        Helper.DrawWindowBottom1(currentUnit.unit.nickname + "は行動に失敗した！");
                    else
                    {
                        switch (currentAct.type)
                        {
                            case ActType.Heal:
                                Helper.DrawWindowBottom1(target.unit.nickname + "はHPを回復する必要がない！");
                                break;
                            case ActType.AddMinusSympton:
                            case ActType.AddPlusSympton:
                            case ActType.AddDoubleSympton:
                                if (Crystal.sympton == FieldEffect.SympInvalid)
                                    Helper.DrawWindowBottom1("結晶効果により症状のセットが妨害された！");
                                else
                                    Helper.DrawWindowBottom1("症状クリアにより症状のセットが妨害された！");
                                break;
                            case ActType.SetTrap:
                                Helper.DrawWindowBottom1("トラップクリアによりトラップのセットが妨害された！");
                                break;
                            case ActType.ClearMinusSympton:
                            case ActType.ClearPlusSympton:
                                Helper.DrawWindowBottom1("クリアする症状がなかった！");
                                break;
                            case ActType.ClearTrap:
                                Helper.DrawWindowBottom1("クリアするトラップがなかった！");
                                break;
                            case ActType.SPDrain:
                                Helper.DrawWindowBottom1("奪い取るSPがなかった！");
                                break;
                            case ActType.LevelDrain:
                                Helper.DrawWindowBottom1("奪い取るレベルがなかった！");
                                break;
                            case ActType.Guard:
                            case ActType.LessGuard:
                                Helper.DrawWindowBottom1("防御ができない時は援護が出来ない！");
                                break;
                            case ActType.Utsusemi:
                                Helper.DrawWindowBottom1("回避ができない時は援護が出来ない！");
                                break;
                            case ActType.Counter:
                            case ActType.BarrierDefense:
                            case ActType.BarrierSpirit:
                                Helper.DrawWindowBottom1("防御ができない時は構えが出来ない！");
                                break;
                            case ActType.Booster:
                            case ActType.Scope:
                            case ActType.DualBoost:
                            case ActType.Charge:
                                Helper.DrawWindowBottom1("このスキルは装備するだけで効果を発揮する！");
                                break;
                        }
                    }
                    #endregion
                }
                else if (actState == ActState.TakeTrap) // トラップ発動
                {
                    #region
                    if (target.trap.sympton == Trap.OnceClear)
                    {
                        if (time < timeSpan - 1000)
                        {
                            Vector2 pos1 = mapOrigin + 64 * (Vector2)currentUnit.postion - new Vector2(32);
                            Vector2 pos2 = mapOrigin + 64 * (Vector2)target.postion - new Vector2(32);
                            spriteBatch.Draw(t_icon4, Vector2.Lerp(pos2, pos1, time / (timeSpan - 1000)), new Rectangle(0, 288, 96, 96), Color.White);
                        }
                        else
                        {
                            float t = time - (timeSpan - 1000);
                            Vector2 pos = mapOrigin + 64 * (Vector2)currentUnit.postion - new Vector2(32);
                            spriteBatch.Draw(t_icon4, pos, new Rectangle(96, 288 + 96 * (int)(t % 250 / 125), 96, 96), Color.White);
                        }
                        Helper.DrawWindowBottom1("単発クリアにより攻撃が打ち消された！");
                    }
                    else
                    {
                        if (time < timeSpan - 3000)
                        {
                            Vector2 pos1 = mapOrigin + 64 * (Vector2)currentUnit.postion - new Vector2(32);
                            Vector2 pos2 = mapOrigin + 64 * (Vector2)target.postion - new Vector2(32);
                            if (currentAct.type == ActType.Grapple)
                                spriteBatch.Draw(t_icon4, Vector2.Lerp(pos2, pos1, time / (timeSpan - 3000)), new Rectangle(0, 0, 96, 96), Color.White);
                            else
                                spriteBatch.Draw(t_icon4, Vector2.Lerp(pos2, pos1, time / (timeSpan - 3000)), new Rectangle(0, 96, 96, 96), Color.White);
                        }
                        else if (time < timeSpan - 2000)
                        {
                            float t = time - (timeSpan - 3000);
                            Vector2 pos = mapOrigin + 64 * (Vector2)currentUnit.postion - new Vector2(32);
                            if (currentAct.type == ActType.Grapple)
                                spriteBatch.Draw(t_icon4, pos, new Rectangle(96 * (int)(t % 250 / 125), 0, 96, 96), Color.White);
                            else
                                spriteBatch.Draw(t_icon4, pos, new Rectangle(96, 96 + 96 * (int)(t % 125 / 62.5), 96, 96), Color.White);
                        }
                        else
                        {
                            Vector2 pos = mapOrigin + 64 * (Vector2)currentUnit.postion + new Vector2(0, 32 - (int)GetIconFact(timeSpan - 2000));
                            int damage = Math.Abs(currentUnit.HP - currentUnit.HPold);
                            if (damage >= 100)
                                pos += new Vector2(6, 0);
                            else if (damage < 10)
                                pos += new Vector2(-6, 0);
                            spriteBatch.Draw(t_icon1, pos, new Rectangle(132, 160, 16, 32), Color.White);
                            if (damage >= 100)
                                spriteBatch.Draw(t_icon1, pos, new Rectangle(16 * (damage / 100), 160, 16, 32), Color.White);
                            if (damage >= 10)
                                spriteBatch.Draw(t_icon1, pos + new Vector2(16, 0), new Rectangle(16 * ((damage % 100) / 10), 160, 16, 32), Color.White);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(32, 0), new Rectangle(16 * (damage % 10), 160, 16, 32), Color.White);

                            pos = mapOrigin + (Vector2)currentUnit.postion * 64;
                            spriteBatch.Draw(tw, new Rectangle((int)pos.X, (int)pos.Y + 64, 64, 6), Color.Black);
                            float hp = 46 * MathHelper.Lerp((float)currentUnit.HPold / currentUnit.HPmax,
                                (float)currentUnit.HP / currentUnit.HPmax, MathHelper.Clamp((time - 2000) / 1000, 0, 1));
                            spriteBatch.Draw(tw, new Rectangle((int)pos.X + 1, (int)pos.Y + 49, (int)hp, 4), Color.LimeGreen);
                        }
                        Helper.DrawWindowBottom1(currentUnit.unit.nickname + "はトラップにかかった！");
                    }
                    #endregion
                }
                else if (actState == ActState.DisarmTrap) // トラップ解除
                {
                    Helper.DrawWindowBottom1(currentUnit.unit.nickname + "はトラップを解除した！");
                }
                else if (actState == ActState.Force) // 強制移動
                {
                    bool success = false;
                    if (!currentAct.IsTargetAll)
                        success = countered ? (currentUnit.postion != calcResult[1].forcedPos) : (target.postion != calcResult[0].forcedPos);
                    else
                    {
                        for (int i = 0; i < targetList.Count; i++)
                            if (!targetList[i].dead && targetList[i].postion != calcResult[i].forcedPos)
                                success = true;
                    }

                    if (!success)
                        Helper.DrawWindowBottom1("敵を動かすことに失敗した！");
                    else if (currentAct.IsHaveAbility(ActAbility.Shock))
                        Helper.DrawWindowBottom1("敵を奥に押しのけた！");
                    else
                        Helper.DrawWindowBottom1("敵を手前に引き寄せた！");
                }
                else if (actState == ActState.HPSPgain) // 吸収精神侵蝕犠牲
                {
                    if (currentAct.IsHaveAbility(ActAbility.Drain))
                        Helper.DrawWindowBottom1("相手のHPを吸収した！");
                    else if (currentAct.IsHaveAbility(ActAbility.Spirit))
                        Helper.DrawWindowBottom1("相手のSPを減少させた！");
                    else
                        Helper.DrawWindowBottom1("体力の半分を失った！");
                }
                #endregion

                #endregion
            }
            else if (!battleStart)
            {
               # region 状態確認
                if (actState == ActState.ListAlly || actState == ActState.ListEnemy)
                {
                    if (!IsMapMoving())
                    {
                        Positon p = new Positon();
                        if (actState == ActState.ListAlly)
                            p.X = 24;
                        else
                            p.X = 582;
                        if (selectedAct == 0)
                            p.Y = 12;
                        else
                            p.Y = 8 + selectedAct * 96;
                        Helper.DrawSquare(new Rectangle(p.X, p.Y, 250, 40), 3, Color.White);

                        if (selectedAct > 0)
                        {
                            float f = GetCursorFact(gameTime.TotalGameTime, 1, 0, 1200);
                            Vector2 pos = new Positon();
                            if (actState == ActState.ListAlly)
                                pos = allyUnitGadget[selectedAct - 1].postion;
                            else
                                pos = enemyUnitGadget[selectedAct - 1].postion;
                            if (f < 0.5)
                                spriteBatch.Draw(t_icon1, mapOrigin + pos * 64, new Rectangle(64, 0, 64, 64), Color.White);
                        }
                    }

                    Helper.DrawStringWithOutLine("戦闘開始", mapOrigin + new Vector2(-344, 6));
                    for (int i = 0; i < allyUnitGadget.Count; i++)
                    {
                        UnitGadget ug = allyUnitGadget[i];
                        int y = 98 + i * 96;
                        Helper.DrawStringWithOutline(ug.unit.name, mapOrigin + new Vector2(-406, y), ug.HP > 0 ? Color.White : Color.Gray);
                        int x = mapOrigin.X - 154;
                        str = ug.HP.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(x + 40 - font.MeasureString(str).X, y + 6));
                        spriteBatch.Draw(tw, new Rectangle(x + 48, y + 15, 100, 18), Color.White);
                        spriteBatch.Draw(tw, new Rectangle(x + 51, y + 18, 94, 12), Color.Black);
                        spriteBatch.Draw(tw, new Rectangle(x + 51, y + 18, 94 * ug.HP / ug.HPmax, 12), Color.LimeGreen);
                    }
                    for (int i = 0; i < enemyUnitGadget.Count; i++)
                    {
                        UnitGadget ug = enemyUnitGadget[i];
                        int y = 98 + i * 96;
                        Helper.DrawStringWithOutline(ug.unit.name, mapOrigin + new Vector2(596, y), ug.HP > 0 ? Color.White : Color.Gray);
                        int x = mapOrigin.X + 848;
                        str = ug.HP.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(x + 40 - font.MeasureString(str).X, y + 6));
                        spriteBatch.Draw(tw, new Rectangle(x + 48, y + 15, 100, 18), Color.White);
                        spriteBatch.Draw(tw, new Rectangle(x + 51, y + 18, 94, 12), Color.Black);
                        spriteBatch.Draw(tw, new Rectangle(x + 51, y + 18, 94 * ug.HP / ug.HPmax, 12), Color.LimeGreen);
                    }
                    Helper.DrawWindowBottom1("startボタンで戦闘開始");
                }
                if (actState == ActState.ShowAlly || actState == ActState.ShowEnemy)
                {
                    UnitGadget ug;
                    int x1, x2, x3, x4; // 基本、行動、マイナス症状(トラップ)、プラス症状(構え)
                    if (actState == ActState.ShowAlly)
                    {
                        ug = allyUnitGadget[selectedAct - 1];
                        x1 = 0;
                        x2 = 512;
                        x3 = 4;
                        x4 = 284;
                    }
                    else
                    {
                        ug = enemyUnitGadget[selectedAct - 1];
                        x1 = 608;
                        x2 = 40;
                        x3 = 284;
                        x4 = 4;
                    }
                    DrawUnit(ug, gameTime);

                    spriteBatch.Draw(t_icon1, new Vector2(x1 + 10, 10), new Rectangle((int)ug.unit.type * 48, 96, 48, 48), Color.White);
                    if (ug.drive)
                        spriteBatch.Draw(t_icon1, new Vector2(x1 + 34, 34), new Rectangle((int)ug.unit.type2 * 24, 144, 24, 24), Color.White);
                    Helper.DrawStringWithOutLine(ug.unit.name, new Vector2(x1 + 64, 16));
                    Helper.DrawStringWithOutLine("HP:", new Vector2(x1 + 10, 56));
                    str = ug.HP.ToString();
                    Helper.DrawStringWithOutLine(str, new Vector2(x1 + 95 - font.MeasureString(str).X, 56));
                    spriteBatch.Draw(tw, new Rectangle(x1 + 106, 64, 160, 18), Color.White);
                    spriteBatch.Draw(tw, new Rectangle(x1 + 109, 67, 154, 12), Color.Black);
                    spriteBatch.Draw(tw, new Rectangle(x1 + 109, 67, 154 * ug.HP / ug.HPmax, 12), Color.LimeGreen);
                    Helper.DrawStringWithOutLine("SP:", new Vector2(x1 + 10, 80));
                    str = ug.SP.ToString();
                    Helper.DrawStringWithOutLine(str, new Vector2(x1 + 95 - font.MeasureString(str).X, 80));
                    spriteBatch.Draw(tw, new Rectangle(x1 + 106, 88, 160, 18), Color.White);
                    spriteBatch.Draw(tw, new Rectangle(x1 + 109, 91, 154, 12), Color.Black);
                    spriteBatch.Draw(tw, new Rectangle(x1 + 109, 91, 154 * ug.SP / ug.SPmax, 12), Color.SkyBlue);
                    Helper.DrawStringWithOutLine("AP: 0/", new Vector2(x1 + 10, 104));
                    str = ug.AP.ToString();
                    Helper.DrawStringWithOutLine(str, new Vector2(x1 + 122 - font.MeasureString(str).X, 104));

                    if (ug.symptonMinus.sympton > 0)
                        spriteBatch.Draw(t_icon2, new Vector2(x1 + x3, 160), new Rectangle(0, 64 * ((int)ug.symptonMinus.sympton - 1), 128, 64), Color.White);
                    if (ug.symptonPlus.sympton > 0)
                        spriteBatch.Draw(t_icon2, new Vector2(x1 + x4, 160), new Rectangle(128, 64 * ((int)ug.symptonPlus.sympton - 1), 128, 64), Color.White);
                    if (ug.trap.sympton > 0)
                        spriteBatch.Draw(t_icon2, new Vector2(x1 + x3, 556), new Rectangle(256, 64 * ((int)ug.trap.sympton - 1), 128, 64), Color.White);
                    if (ug.stance >= 0)
                    {
                        switch (ug.Acts[ug.stance].type)
                        {
                            case ActType.Guard:
                                spriteBatch.Draw(t_icon2, new Vector2(x1 + x4, 556), new Rectangle(384, 0, 128, 64), Color.White);
                                break;
                            case ActType.LessGuard:
                                spriteBatch.Draw(t_icon2, new Vector2(x1 + x4, 556), new Rectangle(384, 64, 128, 64), Color.White);
                                break;
                            case ActType.Utsusemi:
                                spriteBatch.Draw(t_icon2, new Vector2(x1 + x4, 556), new Rectangle(384, 128, 128, 64), Color.White);
                                break;
                            case ActType.Counter:
                                spriteBatch.Draw(t_icon2, new Vector2(x1 + x4, 556), new Rectangle(384, 192, 128, 64), Color.White);
                                break;
                            case ActType.BarrierDefense:
                                spriteBatch.Draw(t_icon2, new Vector2(x1 + x4, 556), new Rectangle(384, 256, 128, 64), Color.White);
                                break;
                            case ActType.BarrierSpirit:
                                spriteBatch.Draw(t_icon2, new Vector2(x1 + x4, 556), new Rectangle(384, 320, 128, 64), Color.White);
                                break;
                        }
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Act a = ug.Acts[i];
                        if (a != null)// && (!a.lastSpell || ug.unit.IsHaveAbility(Ability.Drive)))
                        {
                            spriteBatch.Draw(t_icon1, new Vector2(x2, 46 + i * 90), new Rectangle(GetIconFact(a) * 32, 64, 32, 32), Color.White);
                            Helper.DrawStringWithOutLine(a.name, new Vector2(x2 + 40, 45 + i * 90));
                            if (a.IsLimited)
                            {
                                Helper.DrawStringWithOutLine("/", new Vector2(x2 + 20, 75 + i * 90));
                                str = ug.actCount[i].ToString();
                                Helper.DrawStringWithOutLine(str, new Vector2(x2 + 20 - font.MeasureString(str).X, 75 + i * 90));
                                str = a.count.ToString();
                                Helper.DrawStringWithOutLine(str, new Vector2(x2 + 50 - font.MeasureString(str).X, 75 + i * 90));
                            }
                            Helper.DrawStringWithOutLine(Helper.GetStringActType(a), new Vector2(x2 + 70, 75 + i * 90));
                        }
                        else
                            spriteBatch.Draw(t_icon1, new Vector2(x2, 50 + i * 90), new Rectangle(5 * 32, 64, 32, 32), Color.White);
                    }
                }
                #endregion
            }

            spriteBatch.End();
        }

        static void DrawUnit(UnitGadget ug, GameTime gameTime)
        {
            int x;
            bool reverse;
            if (ug.ff == FrendOfFoe.Ally)
            {
                x = 0;
                reverse = false;
            }
            else
            {
                x = 1024 - 208 * 2;
                reverse = true;
            }
            spriteBatch.Draw(t_shadow, new Vector2(x + 160, 420), Color.White);
            if (actState != ActState.Move)
            {
                Vector2 pos = new Vector2(x + 208, 435);
                ug.unit.DrawBattle(spriteBatch, pos, Color.White, 0.98f + GetUnitFact(gameTime.TotalGameTime, 0.02f, 2000), reverse);
            }
            else
            {
                Vector2 pos = new Vector2(x + 208, 435 + GetUnitFact(gameTime.TotalGameTime, 10, 500));
                ug.unit.DrawBattle(spriteBatch, pos, Color.White, reverse);
            }
        }

        static void DrawNumber(Positon position, int fact, bool positive, int num, Color color)
        {
            Vector2 pos = mapOrigin + 64 * (Vector2)position + new Vector2(0, 32 - fact);
            if (num < 100)
                pos.X += 8;
            if (positive)
                spriteBatch.Draw(t_icon1, pos, new Rectangle(160, 192, 16, 32), color);
            else
                spriteBatch.Draw(t_icon1, pos, new Rectangle(176, 192, 16, 32), color);
            pos.X += 16;
            if (num >= 100)
            {
                spriteBatch.Draw(t_icon1, pos, new Rectangle(16 * (num / 100), 192, 16, 32), color);
                pos.X += 16;
            }
            if (num >= 10)
                spriteBatch.Draw(t_icon1, pos, new Rectangle(16 * ((num % 100) / 10), 192, 16, 32), color);
            pos.X += 16;
            spriteBatch.Draw(t_icon1, pos, new Rectangle(16 * (num % 10), 192, 16, 32), color);
        }

        static void SetTurnOrder()
        {
            SortedDictionary<float, UnitGadget> order = new SortedDictionary<float, UnitGadget>(new CaseInsensitiveReverseFloatComparer());
            foreach (UnitGadget ug in allUnitGadget)
            {
                if (ug.dead)
                    continue;
                float d;
                do
                {
                    d = ug.Parameter.speed + ((float)GameBody.rand.NextDouble() * 10 - 5) + (ug.leader ? 60 : 0);
                } while (order.Keys.Contains(d));
                order.Add(d, ug);
            }
            foreach (UnitGadget ug in order.Values)
            {
                unitOrder.Add(ug);
            }
        }

        static void CheckSymptonState()
        {
            bool trans = false;
            if (actState < ActState.CrystalEffect)
            {
                if ((crystal_face.sympton > 0 && crystal_face.turn <= 0)
                    || Crystal.sympton == FieldEffect.ChangeTerrain)
                {
                    timeSpan = 1000;
                    actState = ActState.CrystalEffect;
                    return;
                }
                if (Crystal.sympton == FieldEffect.HPHeal || Crystal.sympton == FieldEffect.HPDamage)
                {
                    foreach (UnitGadget ug in allUnitGadget)
                    {
                        if (!ug.dead && IsTakeFieldEffect(Crystal.sympton, ug))
                        {
                            if (Crystal.sympton == FieldEffect.HPDamage)
                                AddHP(ug, Crystal.power);
                            else
                                AddHP(ug, -Crystal.power, 1);
                            trans = true;
                        }
                    }
                    if (trans)
                    {
                        timeSpan = 2000;
                        actState = ActState.CrystalEffect;
                        return;
                    }
                }
            }
            if (actState < ActState.DisPlus)
            {
                foreach (UnitGadget ug in allUnitGadget)
                {
                    if (ug.symptonPlus.sympton > 0 && ug.symptonPlus.turn <= 0)
                    {
                        actState = ActState.DisPlus;
                        return;
                    }
                }
            }
            if (actState < ActState.DisMinus)
            {
                foreach (UnitGadget ug in allUnitGadget)
                {
                    if (ug.symptonMinus.sympton > 0 && ug.symptonMinus.turn <= 0)
                    {
                        actState = ActState.DisMinus;
                        return;
                    }
                }
            }
            if (actState < ActState.SymCharge)
            {
                foreach (UnitGadget ug in allUnitGadget)
                {
                    if (ug.symptonPlus.sympton == SymptonPlus.Charge)
                    {
                        actState = ActState.SymCharge;
                        return;
                    }
                }
            }
            if (actState < ActState.SymHeal)
            {
                foreach (UnitGadget ug in allUnitGadget)
                {
                    if (ug.symptonPlus.sympton == SymptonPlus.Heal && ug.HP < ug.HPmax)
                    {
                        AddHP(ug, ug.symptonPlus.power);
                        trans = true;
                    }
                }
                if (trans)
                {
                    actState = ActState.SymHeal;
                    return;
                }
            }
            if (actState < ActState.SymDamage)
            {
                foreach (UnitGadget ug in allUnitGadget)
                {
                    if (ug.symptonMinus.sympton == SymptonMinus.Damage && ug.HP > 1)
                    {
                        AddHP(ug, -ug.symptonMinus.power, 1);
                        trans = true;
                    }
                }
                if (trans)
                {
                    actState = ActState.SymDamage;
                    return;
                }
            }
            actState = ActState.SelectAct;
        }

        public static void MapSet(string[] data, FieldEffect effect = FieldEffect.None)
        {
            for (int i = 0; i < MapSize; i++)
                for (int j = 0; j < MapSize; j++)
                {
                    switch (data[j][i])
                    {
                        case 'P':
                            map[i, j] = Terrain.Plain;
                            break;
                        case 'F':
                            map[i, j] = Terrain.Forest;
                            break;
                        case 'M':
                            map[i, j] = Terrain.Mountain;
                            break;
                        case 'W':
                            map[i, j] = Terrain.Waterside;
                            break;
                        case 'I':
                            map[i, j] = Terrain.Indoor;
                            break;
                        case 'R':
                            map[i, j] = Terrain.Red_hot;
                            break;
                        case 'S':
                            map[i, j] = Terrain.Sanctuary;
                            break;
                        case 'D':
                            map[i, j] = Terrain.Miasma;
                            break;
                        case 'C':
                            map[i, j] = Terrain.Crystal;
                            break;
                        case 'B':
                        default:
                            map[i, j] = Terrain.Banned;
                            break;
                    }
                }
            crystal_base = new Condition<FieldEffect>(effect, 0, 0);
            crystal_face = new Condition<FieldEffect>(FieldEffect.None, 0, 0);

            graphics.SetRenderTarget(r_map);
            spriteBatch.Begin();
            for (int i = 0; i < MapSize; i++)
                for (int j = 0; j < MapSize; j++)
                    spriteBatch.Draw(t_map, new Vector2(j * 64, i * 64), new Rectangle((int)map[j, i] * 64, 0, 64, 64), Color.White);
            spriteBatch.End();
            graphics.SetRenderTarget(null);
        }

        static int GetMapMoveGoal()
        {
            if (battleStart)
            {
                if (actState < ActState.Dis2 || actState >= ActState.Win)
                {
                    return 218;
                }
                else if (currentUnit == null)
                {
                    return mapOrigin.X;
                }
                else if (currentUnit.ff == FrendOfFoe.Ally)
                {
                    return 438;
                }
                else
                {
                    return 2;
                }
            }
            else
            {
                if (actState == ActState.Confront)
                {
                    return 218;
                }
                else if (actState == ActState.ListAlly || actState == ActState.ShowAlly)
                {
                    return 438;
                }
                else if (actState == ActState.BackBlackout)
                {
                    return mapOrigin.X;
                }
                else
                {
                    return 2;
                }
            }
        }

        static bool IsMapMoving()
        {
            return mapOrigin.X != GetMapMoveGoal();
        }

        static void CalcMoveCost()
        {
            Positon p = currentUnit.postion;
            Condition<SymptonMinus> con = currentUnit.symptonMinus;
            float power = con.power;
            if (con.sympton == SymptonMinus.FixInside && Positon.Distance(p, con.doer.postion) > power)
                power = Positon.Distance(p, con.doer.postion);
            else if (con.sympton == SymptonMinus.FixOutside && Positon.Distance(p, con.doer.postion) < power)
                power = Positon.Distance(p, con.doer.postion);
            List<Positon> lp;
            // コストマップの初期設定
            for (int i = 0; i < MapSize; i++)
                for (int j = 0; j < MapSize; j++)
                {
                    if (map[i, j] == Terrain.Banned
                        || (con.sympton == SymptonMinus.FixInside && Positon.Distance(new Positon(i, j), con.doer.postion) > power)
                        || (con.sympton == SymptonMinus.FixOutside && Positon.Distance(new Positon(i, j), con.doer.postion) < power))
                        mapCost[i, j].movable = false;
                    else
                        mapCost[i, j].movable = true;
                    mapCost[i, j].cost = 99;
                    mapCost[i, j].zocCost = 0;
                }
            foreach (UnitGadget ug in allUnitGadget)
                if (ug != currentUnit && !ug.dead)
                {
                    mapCost[ug.postion.X, ug.postion.Y].movable = false;
                    if (!IsAlly(ug))// ZOC
                        for (int i = 0; i < MapSize; i++)
                            for (int j = 0; j < MapSize; j++)
                                if (Positon.Distance(ug.postion, new Positon(i, j)) <= ug.ZOCrange)
                                    mapCost[i, j].zocCost += ug.ZOCpower;
                }
            int assist = 0;
            foreach (Act act in currentUnit.Acts)
                if (act != null && (act.type == ActType.MoveAssist))
                    assist = act.power;

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
                    if (IsTakeFieldEffect(FieldEffect.CostUp, currentUnit)) // 結晶効果：コスト増加
                        cost += Crystal.power;
                    if (currentUnit.symptonPlus.sympton == SymptonPlus.Swift) // プラス症状：俊足
                        cost = (int)Math.Ceiling(cost * 0.5);
                    if (currentUnit.symptonMinus.sympton == SymptonMinus.Restraint) // マイナス症状：束縛
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

        static void DrawMoveBackGround()
        {
            int n = 3;
            Positon p = unitCursor;
            while (p != currentUnit.postion)
            {
                p = mapCost[p.X, p.Y].parent;
                n++;
            }
            r_bgMove = new RenderTarget2D(graphics, 512 * n, 640);
            p = unitCursor;
            graphics.SetRenderTarget(r_bgMove);
            spriteBatch.Begin();
            if (currentUnit.ff == FrendOfFoe.Ally)
            {
                spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(512 * --n, 0), Color.White);
                while (p != currentUnit.postion)
                {
                    spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(512 * --n, 0), Color.White);
                    p = mapCost[p.X, p.Y].parent;
                }
                spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(512 * --n, 0), Color.White);
                spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(512 * --n, 0), Color.White);
            }
            else
            {
                n = 0;
                spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(512 * n++, 0), Color.White);
                while (p != currentUnit.postion)
                {
                    spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(512 * n++, 0), Color.White);
                    p = mapCost[p.X, p.Y].parent;
                }
                spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(512 * n++, 0), Color.White);
                spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(512 * n++, 0), Color.White);
            }
            spriteBatch.End();
            graphics.SetRenderTarget(null);
            bgMoveOffset = 0;
        }

        static List<Positon> GetNeighbor(Positon p)
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
                    if (mapCost[i, j].movable)
                        lp.Add(new Positon(i, j));
                }
            }

            return lp;
        }

        static int GetTerrainCost(Positon pos)
        {
            Terrain tera = GetTip(pos);
            if (tera == Terrain.Banned)
                return 99;
            switch (GetAffinity(currentUnit, pos))
            {
                case Affinity.VeryGood:
                    return 4;
                case Affinity.Good:
                    switch (tera)
                    {
                        case Terrain.Plain:
                        case Terrain.Mountain:
                        case Terrain.Indoor:
                        case Terrain.Sanctuary:
                        case Terrain.Crystal:
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
                        case Terrain.Crystal:
                            return 10;
                        default:
                            return 16;
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
            if (a.IsCover || a.IsStance)
                lug.Add(currentUnit);

            List<UnitGadget> serchList;

            if (a.target == ActTarget.AllyEnemy1 || a.target == ActTarget.All)
                serchList = allUnitGadget;
            else if (((a.target == ActTarget.Enemy1 || a.target == ActTarget.EnemyAll) && currentUnit.ff == FrendOfFoe.Ally)
                || ((a.target == ActTarget.Ally1 || a.target == ActTarget.AllyAll) && currentUnit.ff == FrendOfFoe.Enemy))
                serchList = enemyUnitGadget;
            else if (((a.target == ActTarget.Enemy1 || a.target == ActTarget.EnemyAll) && currentUnit.ff == FrendOfFoe.Enemy)
                || ((a.target == ActTarget.Ally1 || a.target == ActTarget.AllyAll) && currentUnit.ff == FrendOfFoe.Ally))
                serchList = allyUnitGadget;
            else
                serchList = new List<UnitGadget>();

            for (int i = 0; i < serchList.Count; i++)
            {
                if ((a.type != ActType.Revive && a.type != ActType.Revive2 && serchList[i].dead)
                    || (a.type == ActType.Revive && !serchList[i].dead && serchList[i].HPold != 0))
                    continue;
                float d = Positon.Distance(unitCursor, serchList[i].postion);
                if (d >= a.rangeMin && d <= a.rangeMax && !lug.Contains(serchList[i]))
                    lug.Add(serchList[i]);
            }
            currentUnit.postion = p;
            

            return lug;
        }

        static bool IsCanAct()
        {
            return selectedAct == 0 || (currentAct != null && currentUnit.AP >= GetUsingAP(false) &&
                ((!currentAct.IsSpell && (!currentAct.IsLimited || currentActCount >= currentAct.count))
                || currentUnit.SP >= currentAct.sp && (currentAct.lastSpell? drive: true)));
        }

        static int GetUsingAP(bool mapcost)
        {
            if (currentUnit == null)
                return 0;
            return ((!currentUnit.drive && drive) ? 6 : 0) + (selectedAct == 0 || currentAct == null ? 0 : currentAct.ap)
                + (mapcost ? mapCost[unitCursor.X, unitCursor.Y].cost : 0);
        }

        static void GetActSuccessRate(UnitGadget target, out int success, out bool hit, out int avoid)
        {
            Act a = currentAct;
            List<UnitGadget> lug = GetActTarget();

            int level = currentUnit.unit.level;
            level += (currentUnit.level - level) / 2;

            success = a.success;
            if (a.TypeInt % 100 == 0)
            {
                if ((a.IsHaveAbility(ActAbility.AntiMinus) && target.symptonMinus.sympton == SymptonMinus.CarvedSeal) // 対負正人妖
                    || (a.IsHaveAbility(ActAbility.AntiPlus) && target.symptonPlus.sympton == SymptonPlus.Stigmata))
                    success *= 4;
                else if ((a.IsHaveAbility(ActAbility.AntiMinus) && target.symptonMinus.sympton > 0 && target.symptonMinus.sympton != SymptonMinus.Invalid)
                    || (a.IsHaveAbility(ActAbility.AntiPlus) && target.symptonPlus.sympton > 0 && target.symptonPlus.sympton != SymptonPlus.Invalid))
                    success *= 2;
                else if ((a.IsHaveAbility(ActAbility.AntiHuman) && (int)GetAffinity(target, Terrain.Sanctuary, true) > 1)
                    || (a.IsHaveAbility(ActAbility.AntiMonster) && (int)GetAffinity(target, Terrain.Miasma, true) > 1))
                    success = (int)(success * 1.5);

                float mag = (Positon.Distance(currentUnit.postion, target.postion) - a.rangeMin) / (a.rangeMax - a.rangeMin + 1);
                if (a.IsHaveAbility(ActAbility.Cold)) // 低温特性補正
                {
                    if (a.TypeInt < 100)
                        success = (int)(success * (1 + (1 - mag)));
                    else
                        success = (int)(success * (1 + mag));
                }
                if (a.IsHaveAbility(ActAbility.Laser)) // 光学特性補正
                    success = (int)(success * (1 + 0.6 * (1 - mag)));
            }
            success += level * 2; // レベル補正
            if (a.TypeInt < 100) // 近接遠隔補正
                success += currentUnit.Parameter.close;
            else
                success += currentUnit.Parameter.far;
            // 地形相性補正
            if (a.TypeInt % 100 != 0)
                success += (int)GetAffinity(currentUnit, unitCursor) * 10;
            else if (a.IsHaveAbility(ActAbility.Summon))
                success += (int)GetAffinity(currentUnit, unitCursor) * 15;
            else if (a.type == ActType.Grapple)
                success += (int)GetAffinity(currentUnit, unitCursor) * 5
                    + (int)GetAffinity(currentUnit, target.postion) * 10;
            else if (a.type == ActType.Shot)
                success += (int)GetAffinity(currentUnit, unitCursor) * 10
                    + (int)GetAffinity(currentUnit, target.postion) * 5;
            if (a.TypeInt % 100 == 0)
            {
                if (a.IsHaveAbility(ActAbility.Geographic)) // 地生特性補正
                {
                    switch (GetAffinity(currentUnit, unitCursor))
                    {
                        case Affinity.VeryGood:
                            success += 30;
                            break;
                        case Affinity.Good:
                            success += 15;
                            break;
                        case Affinity.Normal:
                            success -= 10;
                            break;
                        case Affinity.Bad:
                        default:
                            success -= 40;
                            break;
                    }
                }
                if (!moved && a.type == ActType.Shot) // スコープ
                {
                    foreach (Act act in currentUnit.Acts)
                        if (act != null && (act.type == ActType.Scope || act.type == ActType.DualBoost))
                            success += act.success;
                }
                if (currentUnit.IsType(UnitType.Technic)) // 技属性補正
                    success += currentUnit.SP / 4;
                success += currentUnit.upParameter.search; // 索敵補正
                if (currentUnit.symptonPlus.sympton == SymptonPlus.Concentrate) // プラス症状：集中
                    success += currentUnit.symptonPlus.power;
                // 神羅結界補正
                foreach (UnitGadget ug in allUnitGadget)
                {
                    if (ug.stance >= 0 && ug.StanceAct.type == ActType.BarrierSpirit && ug.ff == currentUnit.ff
                        && Positon.Distance(ug.postion, currentUnit.postion) >= ug.StanceAct.rangeMin
                        && Positon.Distance(ug.postion, currentUnit.postion) <= ug.StanceAct.rangeMax)
                        success += ug.StanceAct.power;
                }
            }
            if (IsTakeFieldEffect(FieldEffect.HitUp, currentUnit)) // 結晶効果：成功アップ
                success += Crystal.power;
            if (currentUnit.symptonMinus.sympton == SymptonMinus.Distract) // マイナス症状：散漫
                success -= currentUnit.symptonMinus.power;

            hit = false;
            avoid = 0;
            if (a.TypeInt % 100 == 0)
            {
                int elevel = target.unit.level;
                elevel += (target.level - elevel) / 2;
                
                avoid = target.Parameter.avoid + elevel * 2;
                avoid += (int)GetAffinity(target, target.postion) * 5; // 地形相性補正
                avoid += target.upParameter.hide; // 隠蔽補正

                // 神羅結界補正
                foreach (UnitGadget ug in allUnitGadget)
                {
                    if (ug.stance >= 0 && ug.StanceAct.type == ActType.BarrierSpirit && ug.ff == target.ff
                        && Positon.Distance(ug.postion, target.postion) >= ug.StanceAct.rangeMin
                        && Positon.Distance(ug.postion, target.postion) <= ug.StanceAct.rangeMax)
                        avoid += ug.StanceAct.power / 2;
                }

                if (target.Dedodge || a.IsHaveAbility(ActAbility.Hit) || a.IsHaveAbility(ActAbility.Thunder)
                    || (target.stance >= 0 && target.StanceAct.IsStance) || currentUnit.trap.sympton == Trap.HitPromise)
                    hit = true;
            }
        }

        static void CalcAttackResult()
        {
            Act a = currentAct;

            for (int i = 0; i < 5; i++)
            {
                calcResult[i].hit = false;
                calcResult[i].critical = false;
                calcResult[i].guard = false;
                calcResult[i].damage = 0;
                calcResult[i].sympTurn = 0;
                calcResult[i].sympPower = 0;
            }

            for (int i = 0; i < GetActTarget().Count; i++)
            {
                UnitGadget target;
                if (!a.IsTargetAll)
                    target = BattleScene.target;
                else
                    target = GetActTarget()[i];
                // 自分パラメータ
                // 成功
                int success, avoid;
                bool hit;
                GetActSuccessRate(target, out success, out hit, out avoid);

                // 威力
                float mag;
                int power = a.power;
                if (a.TypeInt % 100 == 0)
                {
                    mag = (Positon.Distance(currentUnit.postion, target.postion) - a.rangeMin) / (a.rangeMax - a.rangeMin + 1);
                    if (a.IsHaveAbility(ActAbility.Heat)) // 高熱特性補正
                    {
                        if (a.TypeInt < 100)
                            power = (int)(power * (1 + (1 - mag)));
                        else
                            power = (int)(power * (1 + mag));
                    }
                    else if (a.IsHaveAbility(ActAbility.Laser)) // 光学特性補正
                        power = (int)(power * (1 + 0.6 * mag));
                    if (a.IsHaveAbility(ActAbility.Rush)) // 特攻特性補正
                        power += currentUnit.Parameter.speed / 2;
                    if (a.IsHaveAbility(ActAbility.Repeat)) // 反復特性補正
                    {
                        currentUnit.actCount[selectedAct - 1]++;
                        power += 8 * currentActCount;
                    }
                    else if (a.IsHaveAbility(ActAbility.Time)) // 時間特性補正
                        power += 5 * turn;
                    if (a.IsHaveAbility(ActAbility.Whole)) // 全霊特性補正
                    {
                        power += currentUnit.SP;
                        currentUnit.SP = 0;
                    }
                    if (a.IsHaveAbility(ActAbility.Revenge)) // 報復特性補正
                        power += (currentUnit.HPmax - currentUnit.HP) / 2;
                    if (a.IsHaveAbility(ActAbility.Sacrifice) && currentUnit.HP > 1) // 犠牲特性補正
                        power += currentUnit.HP / 2;
                    if (a.IsHaveAbility(ActAbility.Sanctio)) // 制裁特性補正
                    {
                        power += 20 * currentUnit.trap.power;
                        currentUnit.trap.sympton = Trap.None;
                    }
                    if (moved && a.type == ActType.Grapple) // ブースター
                    {
                        foreach (Act act in currentUnit.Acts)
                            if (act != null && (act.type == ActType.Booster || act.type == ActType.DualBoost))
                                power += act.power;
                    }
                    if (currentUnit.IsType(UnitType.Power)) // 力属性補正
                        power += currentUnit.SP / 5;
                }

                // 敵パラメータ
                // レベル
                int elevel = target.unit.level;
                elevel += (target.level - elevel) / 2;
                // 回避
                if (a.TypeInt % 100 == 0)
                {
                    // 空蝉補正
                    if (covered && target.StanceAct.type == ActType.Utsusemi)
                        avoid += target.StanceAct.power;
                }
                // 防御
                int defence = target.Parameter.defense + elevel;

                // 命中判定
                if (a.TypeInt % 100 == 0)
                {
                    if (hit || (covered && target.StanceAct.type != ActType.Utsusemi)
                        || (target.IsType(UnitType.Fortune) ? Helper.GetProbability(success - avoid, 125) : Helper.GetProbability(success - avoid)))
                        calcResult[i].hit = true;
                }
                else
                {
                    if (!target.IsMinusSymptonInvalid((SymptonMinus)a.sympton) && Helper.GetProbability(success))
                        calcResult[i].hit = true;
                }

                // 防御判定
                if (!target.Deguard && Helper.GetProbability(defence, 50) && !covered)
                    calcResult[i].guard = true;

                // クリティカル判定
                mag = a.IsHaveAbility(ActAbility.Fast) ? (a.TypeInt == 0 ? 5 : 2) : (a.IsHaveAbility(ActAbility.Thunder) ? 5 : 10);
                if (a.IsHaveAbility(ActAbility.Proficient)) // 練達特性補正
                    mag *= 1 - (0.4f * MathHelper.Clamp(target.level - currentUnit.level, 0, 8) / 8);
                if (a.TypeInt % 100 == 0 && !covered
                    && (target.symptonMinus.sympton == SymptonMinus.Stop || a.IsHaveAbility(ActAbility.Assassin)
                        || (target.Dedodge && target.Deguard) || Helper.GetProbability(1d * success / avoid - mag)))
                {
                    calcResult[i].hit = true;
                    calcResult[i].critical = true;
                    calcResult[i].guard = false;
                }

                if (calcResult[i].hit)
                {
                    // ダメージ計算
                    int damage = (int)((success / 4) * GetRandomFact());
                    if (!calcResult[i].critical && !target.Dedodge && !covered)
                        damage -= avoid / 4;
                    if (calcResult[i].guard)
                        damage -= defence / 3;
                    if (damage < 0)
                        damage = 0;
                    damage += power;
                    if (a.TypeInt % 100 == 0)
                    {
                        if (a.IsHaveAbility(ActAbility.Diffuse)) // 拡散特性補正
                            damage = (int)(damage * GetRandomFact(0.5));
                        if (target.IsType(UnitType.Guard)) // 護属性補正
                            if (target.SP < 60)
                                damage -= 5;
                            else
                                damage -= 10;
                        if (target.leader) // リーダー補正
                        {
                            int n = (target.ff == FrendOfFoe.Ally) ? GetFightableNum(allyUnitGadget) : GetFightableNum(enemyUnitGadget);
                            damage -= --n * 5;
                        }
                        if (covered && target.StanceAct.type == ActType.Guard) // 援護防御補正
                            damage -= target.StanceAct.power;
                        // 防御結界補正
                        foreach (UnitGadget ug in allUnitGadget)
                        {
                            if (ug.stance >= 0 && ug.StanceAct.type == ActType.BarrierDefense && ug.ff == target.ff
                                && Positon.Distance(ug.postion, target.postion) >= ug.StanceAct.rangeMin
                                && Positon.Distance(ug.postion, target.postion) <= ug.StanceAct.rangeMax)
                                damage -= ug.StanceAct.power;
                        }
                        // ダメージキャップ
                        int cap = 80;
                        if (damage > cap && !a.IsSpell && !calcResult[i].critical && !a.IsBreakthrough)
                            damage = cap;
                        if (damage < 1) // ダメージ最低値
                            damage = 1;
                        if (IsTakeFieldEffect(FieldEffect.DamageFix, currentUnit)) // 結晶効果：ダメージ固定
                            damage = Crystal.power;
                        if (a.IsHaveAbility(ActAbility.Destroy)) // 破壊特性
                            damage = 999;
                        if (covered && target.StanceAct.type == ActType.LessGuard && damage < target.StanceAct.power) // 未満防御補正
                            damage = 0;
                        if (damage > target.HP)
                            damage = target.HP;
                    }
                    calcResult[i].damage = damage;

                    // トラップ：魔力充填
                    if (a.TypeInt % 100 == 0 && target.trap.sympton == Trap.MagicCharge && damage > 0)
                    {
                        target.trap.power--;
                        if (target.trap.power <= 0)
                            target.trap.sympton = Trap.None;
                    }

                    // ふっとび計算
                    #region
                    if (a.IsHaveAbility(ActAbility.Shock) || a.IsHaveAbility(ActAbility.Vacuum))
                    {
                        float force = damage / 20f;
                        Positon p = target.postion - unitCursor;
                        Positon pp = new Positon(Math.Abs(p.X), Math.Abs(p.Y));
                        Positon forceDir;
                        if (Math.Max(pp.X, pp.Y) > Math.Min(pp.X, pp.Y) * 2) // 縦横
                        {
                            if (pp.X > pp.Y)
                            {
                                forceDir.X = p.X / pp.X;
                                forceDir.Y = 0;
                            }
                            else
                            {
                                forceDir.X = 0;
                                forceDir.Y = p.Y / pp.Y;
                            }
                        }
                        else // 斜め
                        {
                            forceDir.X = p.X / pp.X;
                            forceDir.Y = p.Y / pp.Y;
                        }
                        if (a.IsHaveAbility(ActAbility.Vacuum))
                            forceDir = -forceDir;

                        calcResult[i].forceDir = forceDir;
                        calcResult[i].forcedPos = target.postion;
                        float d = Positon.Distance(forceDir, Positon.Zero);
                        while (force >= d)
                        {
                            Positon tmp = calcResult[i].forcedPos + forceDir;
                            if (tmp.X < 0 || tmp.Y < 0 || tmp.X >= MapSize || tmp.Y >= MapSize || GetTip(tmp) == Terrain.Banned)
                                break;
                            bool impossible = false;
                            foreach (UnitGadget ug in allUnitGadget)
                                if (ug != target && ug.postion == tmp)
                                    impossible = true;
                            if (impossible)
                                break;
                            calcResult[i].forcedPos = tmp;
                            force -= d;
                        }

                        if (!a.IsTargetAll)
                        {
                            force = damage / 20f;
                            calcResult[1].forceDir = -forceDir;
                            calcResult[1].forcedPos = currentUnit.postion;
                            d = Positon.Distance(forceDir, Positon.Zero);
                            while (force >= d)
                            {
                                Positon tmp = calcResult[1].forcedPos + forceDir;
                                if (tmp.X < 0 || tmp.Y < 0 || tmp.X >= MapSize || tmp.Y >= MapSize || GetTip(tmp) == Terrain.Banned)
                                    break;
                                bool impossible = false;
                                foreach (UnitGadget ug in allUnitGadget)
                                    if (ug != currentUnit && ug.postion == tmp)
                                        impossible = true;
                                if (impossible)
                                    break;
                                calcResult[1].forcedPos = tmp;
                                force -= d;
                            }
                        }
                    }
                    #endregion

                    // 吸収精神犠牲
                    if (a.IsHaveAbility(ActAbility.Drain))
                        AddHP(currentUnit, damage / 2);
                    else if (a.IsHaveAbility(ActAbility.Spirit))
                        AddSP(target, -damage / 2);
                    if (a.IsHaveAbility(ActAbility.Sacrifice) && currentUnit.HP > 1)
                        AddHP(currentUnit, -currentUnit.HP / 2, 1);

                    // 症状計算
                    if (a.sympton > 0)
                    {
                        calcResult[i].sympTurn = (success - avoid);
                        if (calcResult[i].sympTurn > 128)
                            calcResult[i].sympTurn = 128;

                        switch ((SymptonMinus)a.sympton)
                        {
                            case SymptonMinus.Damage:
                                calcResult[i].sympPower = damage / 2;
                                break;
                            case SymptonMinus.Distract:
                                calcResult[i].sympPower = damage * 2;
                                break;
                            case SymptonMinus.Restraint:
                                calcResult[i].sympPower = damage / 5;
                                break;
                            case SymptonMinus.Stop:
                                calcResult[i].sympPower = damage / 10 + calcResult[i].sympTurn;
                                break;
                            case SymptonMinus.FixInside:
                                if (a.type != ActType.AddMinusSympton)
                                    calcResult[i].sympPower = a.rangeMax;
                                else
                                    calcResult[i].sympPower = power;
                                break;
                            case SymptonMinus.FixOutside:
                                if (a.type != ActType.AddMinusSympton)
                                    calcResult[i].sympPower = a.rangeMin;
                                else
                                    calcResult[i].sympPower = power;
                                break;
                            default:
                                calcResult[i].sympPower = power;
                                break;
                        }
                    }
                }

                if (!a.IsTargetAll)
                    break;
            }

            // 高速、特攻ペナルティ
            if (a.TypeInt == 0)
            {
                if (a.IsHaveAbility(ActAbility.Fast))
                    currentUnit.deguard = true;

                if (a.IsHaveAbility(ActAbility.Rush) || a.IsHaveAbility(ActAbility.Assassin))
                {
                    currentUnit.dedodge = true;
                    currentUnit.deguard = true;
                }
            }
            else if (a.TypeInt == 100)
            {
                if (a.IsHaveAbility(ActAbility.Fast))
                    currentUnit.dedodge = true;
            }
        }

        static void CalcSupportResult()
        {
            Act a = currentAct;

            calcResult[0].hit = false;
            calcResult[0].critical = false;
            calcResult[0].guard = false;
            calcResult[0].damage = 0;
            calcResult[0].sympTurn = 0;
            calcResult[0].sympPower = 0;

            // パラメータ補正
            // 成功
            int success, avoid;
            bool hit;
            GetActSuccessRate(target, out success, out hit, out avoid);
            // 威力
            int power = a.power;

            // 成功判定
            if (Helper.GetProbability(success))
                calcResult[0].hit = true;

            if (calcResult[0].hit)
            {
                calcResult[0].sympTurn = success;
                if (calcResult[0].sympTurn > 128)
                    calcResult[0].sympTurn = 128;
                if (a.type == ActType.SetField && crystal_face.sympton > 0 && calcResult[0].sympTurn < crystal_face.turn)
                    calcResult[0].hit = false;

                int tmp = 0;
                switch (a.type)
                {
                    case ActType.AddPlusSympton:
                        switch ((SymptonPlus)a.sympton)
                        {
                            case SymptonPlus.Heal:
                            case SymptonPlus.Swift:
                                tmp = success / 16;
                                break;
                            case SymptonPlus.Charge:
                                tmp = success / 64;
                                break;
                        }
                        break;
                    case ActType.SetTrap:
                        if (a.sympton == (int)Trap.SPPlant)
                            tmp = success / 16;
                        else
                            tmp = success / 8;
                        break;
                    case ActType.SetField:
                        switch ((FieldEffect)a.sympton)
                        {
                            case FieldEffect.HPHeal:
                            case FieldEffect.HPDamage:
                                tmp = success / 16;
                                break;
                            case FieldEffect.APUp:
                            case FieldEffect.APDown:
                                tmp = success / 64;
                                break;
                            case FieldEffect.HitUp:
                                tmp = success / 5;
                                break;
                            case FieldEffect.CostUp:
                                tmp = success / 64;
                                break;
                        }
                        break;
                    default:
                        tmp = success / 8;
                        break;
                }
                tmp = (int)(tmp * GetRandomFact());
                if (tmp < 0)
                    tmp = 0;
                calcResult[0].damage = tmp + power;
                calcResult[0].sympPower = tmp + power;

                if (a.type == ActType.Heal || a.type == ActType.Heal2 || a.type == ActType.Revive || a.type == ActType.Revive2)
                {
                    calcResult[9] = calcResult[0];
                    if (a.IsTargetAll)
                    {
                        for (int i = 0; i < GetActTarget().Count; i++)
                        {
                            calcResult[i].damage = calcResult[9].damage;
                            if (calcResult[i].damage > GetActTarget()[i].HPmax - GetActTarget()[i].HP)
                                calcResult[i].damage = GetActTarget()[i].HPmax - GetActTarget()[i].HP;
                        }
                    }
                    else if (calcResult[0].damage > target.HPmax - target.HP)
                        calcResult[0].damage = target.HPmax - target.HP;
                }
                else if (a.type == ActType.LevelDrain)
                {
                    if (a.IsTargetAll)
                    {
                        int drain = calcResult[0].damage;
                        for (int i = 0; i < GetActTarget().Count; i++)
                        {
                            calcResult[i].damage = drain;
                            if (calcResult[i].damage > GetActTarget()[i].level)
                                calcResult[i].damage = GetActTarget()[i].level;
                        }
                    }
                    else if (calcResult[0].damage > target.level)
                        calcResult[0].damage = target.level;
                }
                else if (a.type == ActType.SPDrain && calcResult[0].damage > target.SP)
                    calcResult[0].damage = target.SP;
            }
        }

        static void SetSympFact()
        {
            if (currentAct.type == ActType.ClearMinusSympton && (!IsAlly(target) ? target.symptonMinus.sympton == SymptonMinus.Invalid :
                (target.symptonMinus.sympton > 0 && target.symptonMinus.sympton != SymptonMinus.Invalid)))
            {
                if (target.symptonMinus.turn <= 0)
                    timeSpan = 2000;
                else
                    timeSpan = 1000;
                actState = ActState.ClearSymp;
            }
            else if (currentAct.type == ActType.ClearPlusSympton && (IsAlly(target) ? target.symptonPlus.sympton == SymptonPlus.Invalid :
            (target.symptonPlus.sympton > 0 && target.symptonPlus.sympton != SymptonPlus.Invalid)))
            {
                if (target.symptonPlus.turn <= 0)
                    timeSpan = 2000;
                else
                    timeSpan = 1000;
                actState = ActState.ClearSymp;
            }
            else if (currentAct.type == ActType.ClearTrap && (IsAlly(target) ? target.trap.sympton == Trap.TrapClear :
            (target.trap.sympton > 0 && target.trap.sympton != Trap.TrapClear)))
            {
                if (target.trap.turn <= 0)
                    timeSpan = 5000;
                else
                    timeSpan = 4500;
                actState = ActState.ClearSymp;
            }
            else
            {
                switch (currentAct.type)
                {
                    case ActType.AddMinusSympton:
                        switch ((SymptonMinus)currentAct.sympton)
                        {
                            case SymptonMinus.Damage:
                            case SymptonMinus.Distract:
                            case SymptonMinus.Restraint:
                            case SymptonMinus.Stop:
                                break;
                            case SymptonMinus.Confuse:
                                timeSpan = 1000;
                                break;
                            case SymptonMinus.Deguard:
                            case SymptonMinus.Dedodge:
                                timeSpan = 1500;
                                break;
                            case SymptonMinus.FixInside:
                                timeSpan = 1400;
                                break;
                            case SymptonMinus.FixOutside:
                                timeSpan = 2000;
                                break;
                        }
                        break;
                    case ActType.AddPlusSympton:
                        switch ((SymptonPlus)currentAct.sympton)
                        {
                            case SymptonPlus.Heal:
                                timeSpan = 1000;
                                break;
                            case SymptonPlus.Charge:
                            case SymptonPlus.ActAgain:
                                timeSpan = 1000;
                                break;
                            case SymptonPlus.Concentrate:
                            case SymptonPlus.Swift:
                                timeSpan = 2000;
                                break;
                        }
                        break;
                    case ActType.ClearMinusSympton:
                    case ActType.ClearPlusSympton:
                        timeSpan = 1500;
                        break;
                    case ActType.ClearTrap:
                        timeSpan = 1000;
                        break;
                    case ActType.SPUp:
                        timeSpan = 3000;
                        break;
                    case ActType.Revive:
                        timeSpan = 1000;
                        break;
                }
                actState = ActState.AddSymp;
            }
        }

        static void AttackEnd()
        {
            if (countered && currentUnit.HP <= 0)
            {
                currentUnit.drive = false;
                currentUnit.symptonMinus.sympton = SymptonMinus.None;
                currentUnit.symptonPlus.sympton = SymptonPlus.None;
                currentUnit.trap.sympton = Trap.None;
                currentUnit.stance = -1;
                currentUnit.dedodge = false;
                currentUnit.deguard = false;
                actState = ActState.Defeated;
                return;
            }

            if (attackStage < 1)
            {
                foreach (UnitGadget ug in targetList)
                    if (ug.HP <= 0 && !ug.dead)
                    {
                        attackStage = 1;
                        actState = ActState.DefeatEnemy;
                        return;
                    }
            }

            bool hit = false;
            if (!currentAct.IsTargetAll)
                hit = countered || (calcResult[0].hit && calcResult[0].damage > 0 && !target.dead);
            else
                for (int i = 0; i < targetList.Count; i++)
                    if (calcResult[i].hit && calcResult[i].damage > 0 && !targetList[i].dead)
                        hit = true;
            if (attackStage < 2 && hit)
            {
                if (currentAct.IsHaveAbility(ActAbility.Shock) || currentAct.IsHaveAbility(ActAbility.Vacuum))
                {
                    attackStage = 2;
                    actState = ActState.Force;
                    return;
                }
            }
            if (attackStage < 3 && hit)
            {
                if (currentAct.sympton > 0 && (target.symptonMinus.sympton == (SymptonMinus)currentAct.sympton
                    || (currentAct.sympton == (int)SymptonMinus.Stigmata && target.symptonPlus.sympton == SymptonPlus.Stigmata)))
                {
                    attackStage = 3;
                    actState = ActState.AddSympFromAttack;
                    return;
                }
            }
            if (attackStage < 4)
            {
                if (currentAct.IsHaveAbility(ActAbility.Sacrifice)
                    || (hit && (currentAct.IsHaveAbility(ActAbility.Drain)
                        || currentAct.IsHaveAbility(ActAbility.Spirit))))
                {
                    attackStage = 4;
                    actState = ActState.HPSPgain;
                    return;
                }
            }
            attackStage = 0;
            TurnEnd();
        }

        static void TurnEnd()
        {
            if (enemyUnitGadget[0].dead)
            {
                actState = ActState.Win;
                return;
            }
            else if (allyUnitGadget[0].dead)
            {
                actState = ActState.Lose;
                return;
            }
            else if (actState != ActState.SPCharge)
            {
                if (actState != ActState.LeakDP && actState != ActState.DriveOff
                    && actState != ActState.HPSPgain && selectedAct > 0 && currentAct.TypeInt % 100 == 0
                    && (currentAct.IsHaveAbility(ActAbility.Drain) || currentAct.IsHaveAbility(ActAbility.Spirit)
                        || currentAct.IsHaveAbility(ActAbility.Sacrifice)))
                {
                    actState = ActState.HPSPgain;
                    return;
                }
                if (currentUnit.symptonPlus.sympton == SymptonPlus.ActAgain)
                {
                    currentUnit.symptonPlus.power--;
                    currentUnit.AP -= usingAP;
                    actState = ActState.SymActAgain;
                    return;
                }
                if (actState != ActState.LeakDP && actState != ActState.DriveOff && currentUnit.drive)
                {
                    actState = ActState.LeakDP;
                    return;
                }
                if (selectedAct == 0 && !moved && usingAP == 0)
                {
                    actState = ActState.SPCharge;
                    return;
                }
            }
            AddSP(currentUnit, currentUnit.AP - usingAP);
            currentUnit.AP = 0;
            actState = ActState.SelectAct;
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

        static double GetIconFact(int delay = 0)
        {
            double d = (currentTime.TotalMilliseconds - delay) * 0.4;
            if (d > 40)
                d = 0;
            else if (d > 20)
                d = 40 - d;
            return d;
        }

        public static int GetIconFact(Act act)
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

        static void DrawSPCharge(Positon pos, float t)
        {
            Vector2 v = mapOrigin + 64 * (Vector2)pos + new Vector2(32);
            float p = 0;
            float d = 0;
            float s = 0;
            float a = t * 4;
            for (int i = 0; i < 16; i++)
            {
                p += MathHelper.TwoPi / 16;
                d += 13;
                s += 0.367f;
                float dd = 10 + (15 + d % 5) * (1 - t);
                float ss = (0.3f + s % 0.6f) * (1 - t);
                Vector2 vv = v + dd * new Vector2((float)Math.Cos(p), (float)Math.Sin(p));
                spriteBatch.Draw(t_icon1, vv, new Rectangle(224, 192, 32, 32), new Color(a, a, a, a), 0, new Vector2(16), ss, SpriteEffects.None, 0);
            }
        }

        static Terrain GetTip(Positon pos)
        {
            return map[pos.X, pos.Y];
        }

        static Affinity GetAffinity(UnitGadget ug, Positon pos)
        {
            return GetAffinity(ug, GetTip(pos));
        }

        static Affinity GetAffinity(UnitGadget ug, Terrain tera, bool _static = false)
        {
            if (tera == Terrain.Banned)
                return Affinity.Bad;

            if (tera == Terrain.Crystal)
                return Affinity.Bad;

            Affinity affinity = ug.unit.affinity[(int)tera];
            if (_static)
                return affinity;
            if (IsTakeFieldEffect(FieldEffect.AffinityDown, ug)) // 結晶効果：適正ダウン
            {
                int af = (int)affinity;
                af -= Crystal.power;
                if (af < 0)
                    af = 0;
                affinity = (Affinity)af;
            }
            else if (IsTakeFieldEffect(FieldEffect.AffinityReverse, ug)) // 結晶効果：適正反転
            {
                switch (affinity)
                {
                    case Affinity.VeryGood:
                        affinity = Affinity.Bad;
                        break;
                    case Affinity.Good:
                        affinity = Affinity.Normal;
                        break;
                    case Affinity.Normal:
                        affinity = Affinity.Good;
                        break;
                    case Affinity.Bad:
                        affinity = Affinity.VeryGood;
                        break;
                }
            }
            return affinity;
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
            double d = GameBody.rand.NextDouble() * (width * 2);
            return 1 + d - width;
        }

        static bool IsAlly(UnitGadget unit)
        {
            return unit.ff == currentUnit.ff;
        }

        static bool IsTakeFieldEffect(FieldEffect effect, UnitGadget ug)
        {
            return Crystal.sympton == effect && !ug.IsFieldEffectInvalid(effect);
        }

        static void AddHP(UnitGadget ug, int fact, int min = 0)
        {
            ug.HPold = ug.HP;
            ug.HP += fact;
            if (ug.HP > ug.HPmax)
                ug.HP = ug.HPmax;
            else if (ug.HP < min)
                ug.HP = min;
        }

        static void AddSP(UnitGadget ug, int fact)
        {
            ug.SP += fact;
            if (ug.SP > ug.SPmax)
                ug.SP = ug.SPmax;
            else if (ug.SP < 0)
                ug.SP = 0;
        }

        static void AddDP(UnitGadget ug, int fact)
        {
            ug.DP += fact;
            if (ug.DP > ug.DPmax)
                ug.DP = ug.DPmax;
            else if (ug.DP < 0)
                ug.DP = 0;
        }

        static void AddLevel(UnitGadget ug, int fact)
        {
            ug.level += fact;
            if (ug.level > 100)
                ug.level = 100;
            else if (ug.level < 0)
                ug.level = 0;
        }

        static Act currentAct
        {
            get { return selectedAct > 0 ? currentUnit.Acts[selectedAct - 1] : null; }
        }

        static int currentActCount
        {
            get { return selectedAct > 0 ? currentUnit.actCount[selectedAct - 1] : 0; }
        }

        static bool battleStart
        {
            get { return actState >= ActState.TurnStart; }
        }

        static Condition<FieldEffect> Crystal
        {
            get { return crystal_face.sympton > 0 ? crystal_face : crystal_base; }
        }
    }

    class CaseInsensitiveReverseFloatComparer : IComparer<float>
    {
        public int Compare(float x, float y)
        {
            return (int)Math.Round(y - x, MidpointRounding.AwayFromZero);
        }
    }
}
