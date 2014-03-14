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
        static Condition<CrystalEffect> crystal_base;
        static Condition<CrystalEffect> crystal_face;
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
        static Texture2D[] t_bg;
        static int bgNum;
        static float bgBlack = 0;
        static RenderTarget2D r_bgMove;
        static float bgMoveOffset;
        static Positon drawMapOrigin;
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
            /// 結晶解放
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
            /// 反撃
            /// </summary>
            Counter,
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
            /// 吸収精神侵蝕犠牲
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

        static int actTimes; // 連続行動関連
        static bool drive;
        static bool moved;

        static int targetNo;
        static UnitGadget target;
        static UnitGadget target_old;

        struct CalcResult
        {
            public bool hit;
            public bool guard;
            public bool critical;
            public int damage;
            public int sympTurn;
            public int sympPower;
        }
        static CalcResult[] calcResult;
        static Positon forceDir;
        static Positon forcedPos;

        public static void Init(GraphicsDevice g, SpriteBatch s, ContentManager c)
        {
            graphics = g;
            spriteBatch = s;
            content = c;
            font = content.Load<SpriteFont>("font\\CommonFont");
            e_dot = content.Load<Effect>("effect\\dot");
            e_desaturate = content.Load<Effect>("effect\\desaturate");
            t_shadow = content.Load<Texture2D>("img\\battle\\shadow");
            t_icon1 = content.Load<Texture2D>("img\\icon\\system\\icon001");
            t_icon2 = content.Load<Texture2D>("img\\icon\\system\\icon002");
            t_icon3 = content.Load<Texture2D>("img\\icon\\system\\icon003");
            t_icon4 = content.Load<Texture2D>("img\\icon\\system\\icon004");

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

            r_map = new RenderTarget2D(graphics, 432, 432);
            r_map_2 = new RenderTarget2D(graphics, 432, 432);

            unitOrder = new List<UnitGadget>();

            calcResult = new CalcResult[10];

            ListsReset();
        }

        public static void ListsReset()
        {
            allyUnitGadget.Clear();
            enemyUnitGadget.Clear();
            drawMapOrigin = new Positon(147, 6);
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
                            if (GetTip(ug.postion) == Terrain.Crystal) // 結晶地形ボーナス
                            {
                                ap += 6;
                                AddSP(ug, 10);
                            }
                            if (IsTakeCrystal(CrystalEffect.APUp, ug)) // 結晶効果：APアップ
                                ap += Crystal.power;
                            else if (IsTakeCrystal(CrystalEffect.APDown, ug)) // 結晶効果：APダウン
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
                            crystal_face.sympton = CrystalEffect.None;
                        else
                        {
                            switch (Crystal.sympton)
                            {
                                case CrystalEffect.HPHeal:
                                    foreach (UnitGadget ug in allUnitGadget)
                                    {
                                        if (ug.dead)
                                            continue;
                                        if (!ug.IsCrystalEffectInvalid(Crystal.sympton))
                                            AddHP(ug, Crystal.power);
                                    }
                                    break;
                                case CrystalEffect.HPDamage:
                                    foreach (UnitGadget ug in allUnitGadget)
                                    {
                                        if (ug.dead)
                                            continue;
                                        if (!ug.IsCrystalEffectInvalid(Crystal.sympton))
                                            AddHP(ug, -Crystal.power);
                                    }
                                    break;
                                case CrystalEffect.ChangeTerrain:
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
                                            spriteBatch.Draw(t_map, new Vector2(j * 48, i * 48), new Rectangle((int)map[j, i] * 48, 0, 48, 48), Color.White);
                                        }
                                    spriteBatch.End();
                                    graphics.SetRenderTarget(null);
                                    break;
                            }
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
                        foreach (UnitGadget ug in allUnitGadget)
                        {
                            if (ug.symptonPlus.sympton == SymptonPlus.Heal)
                                AddHP(ug, ug.symptonPlus.power);
                        }
                        CheckSymptonState();
                    }
                    if (actState == ActState.SymDamage && currentTime >= TimeSpan.FromMilliseconds(2000))
                    {// 継続発動
                        currentTime = TimeSpan.Zero;
                        foreach (UnitGadget ug in allUnitGadget)
                        {
                            if (ug.symptonMinus.sympton == SymptonMinus.Damage)
                                AddHP(ug, -ug.symptonMinus.power, 1);
                        }
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
                        actTimes = 0;
                        drive = currentUnit.drive;
                        moved = false;
                        targetNo = 0;
                        currentUnit.dedodge = false;
                        currentUnit.deguard = false;
                        currentUnit.stance = -1;
                        if (currentUnit.IsType(Type.Intelligence))
                            AddSP(currentUnit, 10);
                        if (currentUnit.trap.sympton == Trap.SPPlant)
                            AddSP(currentUnit, currentUnit.trap.power);

                        // 結晶侵食度増減
                        if (Crystal.sympton == CrystalEffect.Suppression)
                            AddCrystalErosion(currentUnit, -Crystal.power);
                        else if (crystal_face.sympton > 0)
                            AddCrystalErosion(currentUnit, 5);
                        else if (crystal_base.sympton > 0)
                            AddCrystalErosion(currentUnit, 2);
                        if (GetTip(currentUnit.postion) == Terrain.Crystal)
                            AddCrystalErosion(currentUnit, 5);
                        else if (Crystal.sympton == 0)
                            AddCrystalErosion(currentUnit, -10);

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
                                actState--;
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
                                        targetNo = 0;
                                    }
                                }
                                else
                                {
                                    for (int i = 0; i < lug.Count; i++ )
                                        if (lug[i] == currentUnit ? targetCursor == unitCursor : targetCursor == lug[i].postion)
                                        {
                                            targetExist = true;
                                            targetNo = i;
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
                                target = GetActTarget()[targetNo];
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
                                    || (Crystal.sympton == CrystalEffect.SympInvalid
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
                                    || (currentAct.type == ActType.CrystalSubsided && target.crystalErosion == 0)
                                    || (currentAct.type == ActType.CrystalDrain && crystal_face.sympton == CrystalEffect.None)
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
                                timeSpan = (int)(250 * Vector2.Distance(unitCursor, target.postion));
                                if (currentAct.IsTargetAll)
                                    actState = ActState.AddSymp;
                                else if (calcResult[0].hit)
                                    actState = ActState.AddSympSub;
                                else
                                    actState = ActState.Failure1;
                                if (currentAct.TypeInt % 100 == 0)
                                {
                                    if (currentAct.IsTargetAll)
                                        for (int i = 0; i < GetActTarget().Count; i++)
                                        {
                                            if (calcResult[i].hit && calcResult[i].damage > 0)
                                            {
                                                AddHP(GetActTarget()[i], -calcResult[i].damage);
                                                AddSP(GetActTarget()[i], 10);
                                            }
                                        }
                                    else if (calcResult[0].hit && calcResult[0].damage > 0)
                                    {
                                        if (target.stance >= 0 && target.StanceAct.type == ActType.Counter && calcResult[0].damage >= target.StanceAct.power)
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
                                    timeSpan = (int)(250 * Vector2.Distance(unitCursor, target.postion));
                                    if (currentAct.type == ActType.ClearTrap)
                                        timeSpan /= 2;
                                    switch (currentAct.type)
                                    {
                                        case ActType.Heal:
                                        case ActType.Heal2:
                                            for (int i = 0; i < GetActTarget().Count; i++)
                                            {
                                                UnitGadget tar = currentAct.IsTargetAll ? GetActTarget()[i] : target;
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
                                        case ActType.SetCrystal:
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
                                                timeSpan = 1800 + 200 * GetActTarget().Count;
                                            else
                                                timeSpan = 2000;
                                            actState = ActState.Drain;
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
                                    for (int i = 0; i < GetActTarget().Count; i++)
                                    {
                                        if (currentAct.IsTargetAll)
                                            target = GetActTarget()[i];
                                        if (!currentAct.IsTargetAll && calcResult[i].hit && calcResult[i].damage > 0)
                                        {
                                            UnitGadget tar = countered ? currentUnit : target;
                                            if (currentAct.sympton > 0 && Crystal.sympton != CrystalEffect.SympInvalid
                                                && !tar.IsMinusSymptonInvalid((SymptonMinus)currentAct.sympton))
                                            {
                                                actState = ActState.AddSymp;
                                                timeSpan = 1000;
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
                                            else if (currentAct.IsHaveAbility(ActAbility.Shock) || currentAct.IsHaveAbility(ActAbility.Vacuum))
                                            {
                                                actState = ActState.Force;
                                            }
                                        }
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
                                        }
                                        else if (target.HP <= 0)
                                        {
                                            target.drive = false;
                                            target.symptonMinus.sympton = SymptonMinus.None;
                                            target.symptonPlus.sympton = SymptonPlus.None;
                                            target.trap.sympton = Trap.None;
                                            target.stance = -1;
                                            target.dedodge = false;
                                            target.deguard = false;
                                            unitOrder.Remove(target);
                                            actState = ActState.DefeatEnemy;
                                        }
                                        if (!currentAct.IsTargetAll)
                                            break;
                                    }
                                    if (actState == ActState.Attack_Heal)
                                        TurnEnd();
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
                                                timeSpan = (int)(125 * Vector2.Distance(unitCursor, target.postion));
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
                                        for (int i = 0; i < GetActTarget().Count; i++)
                                        {
                                            AddLevel(GetActTarget()[i], -calcResult[i].damage);
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
                                    foreach (UnitGadget ug in GetActTarget())
                                        ug.trap = new Condition<Trap>((Trap)currentAct.sympton, calcResult[0].sympTurn, calcResult[0].sympPower);
                                TurnEnd();
                            }
                        }
                        else if (actState == ActState.SetCrystal) // 結晶解放
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(6000))
                            {
                                currentTime = TimeSpan.Zero;
                                crystal_face = new Condition<CrystalEffect>((CrystalEffect)currentAct.sympton, calcResult[0].sympTurn, calcResult[0].sympPower);
                                if (crystal_face.sympton == CrystalEffect.SympInvalid)
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
                                    foreach (UnitGadget ug in GetActTarget())
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
                        else if (actState == ActState.DefeatEnemy) // 敵撃破
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(3000))
                            {
                                currentTime = TimeSpan.Zero;
                                List<UnitGadget> lug = GetActTarget();
                                for (int i = 0; i < lug.Count; i++)
                                    if (lug[i].HP <= 0)
                                        lug[i].dead = true;
                                TurnEnd();
                            }
                        }
                        else if (actState == ActState.Defeated) // 自滅
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(3000))
                            {
                                currentTime = TimeSpan.Zero;
                                currentUnit.dead = true;
                                currentUnit.AP = 0;
                                actState = ActState.SelectAct;
                                currentUnit = null;
                            }
                        }
                        else if (actState == ActState.SymActAgain) // 連続行動
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(2000))
                            {
                                currentTime = TimeSpan.Zero;
                                actTimes++;
                                CalcMoveCost();
                                selectedAct = 0;
                                targetCursor = unitCursor = currentUnit.postion;
                                drive = currentUnit.drive;
                                moved = false;
                                targetNo = 0;
                                currentUnit.dedodge = false;
                                currentUnit.deguard = false;
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
                                    TurnEnd();
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
                                target.postion = forcedPos;
                                TurnEnd();
                            }
                        }
                        else if (actState == ActState.HPSPgain) // 吸収精神侵蝕犠牲
                        {
                            if (currentTime >= TimeSpan.FromMilliseconds(2000))
                            {
                                currentTime = TimeSpan.Zero;
                                if (currentAct.IsHaveAbility(ActAbility.Sacrifice) && currentUnit.HP > 1)
                                    AddHP(currentUnit, -currentUnit.HP / 2);
                                else
                                {
                                    for (int i = 0; i < GetActTarget().Count; i++)
                                    {
                                        UnitGadget target;
                                        if (!currentAct.IsTargetAll)
                                            target = BattleScene.target;
                                        else
                                            target = GetActTarget()[i];

                                        if (currentAct.IsHaveAbility(ActAbility.Drain))
                                            AddHP(currentUnit, calcResult[i].damage / 2);
                                        else if (currentAct.IsHaveAbility(ActAbility.Spirit))
                                            AddSP(target, -calcResult[i].damage / 2);
                                        else if (currentAct.IsHaveAbility(ActAbility.Erosion))
                                            AddCrystalErosion(target, calcResult[i].damage / 4);

                                        if (!currentAct.IsTargetAll)
                                            break;
                                    }
                                }
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
            string str = "";

            Vector2 mapOrigin = drawMapOrigin + new Vector2(-3, 3);

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

            #region 全体
            // 背景描画
            if (actState != ActState.Move)
            {
                spriteBatch.Draw(t_bg[bgNum], new Vector2(-24, 0), Color.White);
                spriteBatch.Draw(t_bg[bgNum], new Vector2(360, 0), Color.White);
            }
            else
            {
                bgMoveOffset += 384 * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 550;
                if (currentUnit.ff == FrendOfFoe.Ally)
                    spriteBatch.Draw(r_bgMove, new Vector2(-24 - bgMoveOffset, 0), Color.White);
                else
                    spriteBatch.Draw(r_bgMove, new Vector2(-r_bgMove.Width + 744 + bgMoveOffset, 0), Color.White);
            }
            if (bgBlack > 0)
                spriteBatch.Draw(tw, new Rectangle(0, 0, 720, 480), new Color(0, 0, 0, bgBlack));

            if (battleStart && actState >= ActState.SelectAct && !IsMapMoving() && currentUnit != null)
            {
                // ユニット描画
                DrawUnit(currentUnit, gameTime);
            }

            if (battleStart || (actState != ActState.ShowAlly && actState != ActState.ShowEnemy))
            {
                // マップ描画
                spriteBatch.Draw(tw, new Rectangle(drawMapOrigin.X, drawMapOrigin.Y, 420, 426), new Color(40, 56, 32));
                spriteBatch.Draw(tw, new Rectangle(drawMapOrigin.X + 3, drawMapOrigin.Y + 3, 3, 3), Color.White);
                spriteBatch.Draw(tw, new Rectangle(drawMapOrigin.X + 9, drawMapOrigin.Y + 3, 408, 3), Color.Red);
                spriteBatch.Draw(r_map_2, drawMapOrigin + new Vector2(3, 9), new Rectangle(6, 6, 414, 414), Color.White);

                // SPチャージ
                foreach (UnitGadget ug in allUnitGadget)
                {
                    if (battleStart && ug.spChargeFact > 0)
                    {
                        Vector2 v = mapOrigin + new Vector2(ug.postion.X * 48 + 24, ug.postion.Y * 48 + 24);
                        float s = 1;
                        if (ug.spChargeFact < 0.8)
                            s = 0.5f + 0.625f * ug.spChargeFact;
                        float a = 0.5f;
                        if (ug.spChargeFact > 0.8)
                            a = (1 - ug.spChargeFact) * 2.5f;
                        if (gameTime.TotalGameTime.TotalMilliseconds % 100 >= 50)
                            spriteBatch.Draw(t_icon3, v, new Rectangle(0, 0, 96, 96), new Color(a, a, a, a), 0, new Vector2(48), s, SpriteEffects.None, 0);
                        else
                            spriteBatch.Draw(t_icon3, v, new Rectangle(0, 96, 96, 96), new Color(a, a, a, a), 0, new Vector2(48), s, SpriteEffects.None, 0);
                        ug.spChargeFact = 0;
                    }
                }

                // ユニットアイコン描画
                foreach (UnitGadget ug in allUnitGadget)
                {
                    if (ug.HP <= 0 && ug.HPold <= 0)
                        continue;
                    if (actState == ActState.Force && ug == target)
                    {
                        Vector2 s = mapOrigin + new Vector2(ug.postion.X * 48, ug.postion.Y * 48);
                        if (ug.postion != forcedPos)
                        {
                            Vector2 g = mapOrigin + new Vector2(forcedPos.X * 48, forcedPos.Y * 48);
                            float d = Vector2.Distance(s, g);
                            float t = (float)currentTime.TotalMilliseconds / (d * 5);
                            t = MathHelper.Clamp(t, 0, 1);
                            spriteBatch.Draw(ug.unit.t_icon, Vector2.Lerp(s, g, t), Color.White);
                        }
                        else
                        {
                            Vector2 g = s + (Vector2)forceDir * 12;
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
                        spriteBatch.Draw(ug.unit.t_icon, mapOrigin + new Vector2(ug.postion.X * 48, ug.postion.Y * 48), Color.White);
                    if ((!battleStart || (actState >= ActState.SelectAct && actState <= ActState.ShowActExplain))
                        && gameTime.TotalGameTime.TotalMilliseconds % 1500 >= 750)
                    {
                        if (ug.symptonMinus.sympton > 0 && ug.symptonMinus.turn > 0)
                        {
                            Vector2 p;
                            if (ug.ff == FrendOfFoe.Ally)
                                p = mapOrigin + new Vector2(ug.postion.X * 48, ug.postion.Y * 48);
                            else
                                p = mapOrigin + new Vector2(ug.postion.X * 48 + 24, ug.postion.Y * 48);
                            spriteBatch.Draw(t_icon1, p, new Rectangle((int)(ug.symptonMinus.sympton - 1) * 24, 144, 24, 24), Color.White);
                        }
                        if (ug.stance >= 0 && actState != ActState.DisPlus)
                        {
                            Vector2 p;
                            if (ug.ff == FrendOfFoe.Ally)
                                p = mapOrigin + new Vector2(ug.postion.X * 48 + 24, ug.postion.Y * 48);
                            else
                                p = mapOrigin + new Vector2(ug.postion.X * 48, ug.postion.Y * 48);
                            switch (ug.unit.acts[ug.stance].type)
                            {
                                case ActType.Guard:
                                case ActType.LessGuard:
                                case ActType.Utsusemi:
                                    spriteBatch.Draw(t_icon1, p, new Rectangle(192, 168, 24, 24), Color.White);
                                    break;
                                case ActType.Counter:
                                    spriteBatch.Draw(t_icon1, p, new Rectangle(216, 168, 24, 24), Color.White);
                                    break;
                                case ActType.BarrierDefense:
                                case ActType.BarrierSpirit:
                                    spriteBatch.Draw(t_icon1, p, new Rectangle(240, 168, 24, 24), Color.White);
                                    break;
                            }
                        }
                        else if (ug.symptonPlus.sympton > 0 && ug.symptonPlus.turn > 0)
                        {
                            Vector2 p;
                            if (ug.ff == FrendOfFoe.Ally)
                                p = mapOrigin + new Vector2(ug.postion.X * 48 + 24, ug.postion.Y * 48);
                            else
                                p = mapOrigin + new Vector2(ug.postion.X * 48, ug.postion.Y * 48);
                            spriteBatch.Draw(t_icon1, p, new Rectangle((int)(ug.symptonPlus.sympton - 1) * 24, 168, 24, 24), Color.White);
                        }
                        if (ug.leader)
                        {
                            Vector2 p = mapOrigin + new Vector2(ug.postion.X * 48, ug.postion.Y * 48 + 24);
                            if (ug.ff == FrendOfFoe.Ally)
                                spriteBatch.Draw(t_icon1, p, new Rectangle(192, 24, 48, 24), Color.White);
                            else
                                spriteBatch.Draw(t_icon1, p, new Rectangle(192, 0, 48, 24), Color.White);
                        }
                    }
                }
            }
            #endregion

            if (battleStart && !IsMapMoving())
            {
                #region 戦闘

                float time = (float)currentTime.TotalMilliseconds;

                if (currentUnit != null)
                {
                    // 地形情報
                    Terrain tera = GetTip(targetCursor);
                    Helper.DrawStringWithOutLine(Helper.GetStringCrystalEffect(Crystal.sympton, Crystal.power), new Vector2(303, 440));
                    Helper.DrawStringWithOutLine(Helper.GetStringTerrain(tera), new Vector2(570, 440));
                    Helper.DrawStringWithOutLine(Helper.GetStringAffinity(GetAffinity(currentUnit, tera)), new Vector2(680, 440));
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
                        Vector2 pos = mapOrigin + new Vector2(ug.postion.X * 48 + 6, ug.postion.Y * 48 + 24 - (int)GetIconFact());
                        spriteBatch.Draw(t_icon1, pos, new Rectangle(120, 120, 12, 24), Color.White);
                        if (ug.AP > 10)
                            spriteBatch.Draw(t_icon1, pos + new Vector2(12, 0), new Rectangle(12 * (ug.AP / 10), 120, 12, 24), Color.White);
                        spriteBatch.Draw(t_icon1, pos + new Vector2(24, 0), new Rectangle(12 * (ug.AP % 10), 120, 12, 24), Color.White);
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
                            case CrystalEffect.HPHeal:
                                Helper.DrawWindowBottom1("結晶の力が傷を癒やす！");
                                break;
                            case CrystalEffect.HPDamage:
                                Helper.DrawWindowBottom1("結晶の力が体を蝕む！");
                                break;
                            case CrystalEffect.ChangeTerrain:
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
                                    p = mapOrigin + new Vector2(ug.postion.X * 48 + 24, ug.postion.Y * 48);
                                else
                                    p = mapOrigin + new Vector2(ug.postion.X * 48, ug.postion.Y * 48);
                                spriteBatch.Draw(t_icon1, p, new Rectangle(216, 48 + 24 * (int)(time / 250), 24, 24), Color.White);
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
                                    p = mapOrigin + new Vector2(ug.postion.X * 48, ug.postion.Y * 48);
                                else
                                    p = mapOrigin + new Vector2(ug.postion.X * 48 + 24, ug.postion.Y * 48);
                                spriteBatch.Draw(t_icon1, p, new Rectangle(192, 48 + 24 * (int)(time / 250), 24, 24), Color.White);
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

                        Vector2 pos = mapOrigin + new Vector2(ug.postion.X * 48 + 6, ug.postion.Y * 48 + 24 - (int)GetIconFact());
                        int ap = ug.symptonPlus.power;
                        spriteBatch.Draw(t_icon1, pos, new Rectangle(120, 120, 12, 24), Color.White);
                        if (ap > 10)
                            spriteBatch.Draw(t_icon1, pos + new Vector2(12, 0), new Rectangle(12 * (ap / 10), 120, 12, 24), Color.White);
                        spriteBatch.Draw(t_icon1, pos + new Vector2(24, 0), new Rectangle(12 * (ap % 10), 120, 12, 24), Color.White);
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
                            Vector2 v = mapOrigin + new Vector2(24) + 48 * (Vector2)ug.postion;
                            spriteBatch.Draw(t_icon1, v + new Vector2(-24, -24 - 16 * tt), new Rectangle(384, 96 + 48 * (int)(time % 500 / 125), 48, 48), Color.White);
                            ug.spChargeFact = tt;
                        }
                        else
                        {
                            Vector2 pos = mapOrigin + new Vector2(ug.postion.X * 48 + 6, ug.postion.Y * 48 + 24 - (int)GetIconFact(1000));
                            int ap = ug.symptonPlus.power;
                            if (ap > ug.HPmax - ug.HP)
                                ap = ug.HPmax - ug.HP;
                            spriteBatch.Draw(t_icon1, pos, new Rectangle(120, 120, 12, 24), Color.White);
                            if (ap > 10)
                                spriteBatch.Draw(t_icon1, pos + new Vector2(12, 0), new Rectangle(12 * (ap / 10), 120, 12, 24), Color.White);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(24, 0), new Rectangle(12 * (ap % 10), 120, 12, 24), Color.White);
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
                        }
                        else
                        {
                            Vector2 pos = mapOrigin + new Vector2(ug.postion.X * 48 + 6, ug.postion.Y * 48 + 24 - (int)GetIconFact(1000));
                            int ap = ug.symptonPlus.power;
                            if (ap > ug.HP - 1)
                                ap = ug.HP - 1;
                            spriteBatch.Draw(t_icon1, pos, new Rectangle(132, 120, 12, 24), Color.White);
                            if (ap > 10)
                                spriteBatch.Draw(t_icon1, pos + new Vector2(12, 0), new Rectangle(12 * (ap / 10), 120, 12, 24), Color.White);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(24, 0), new Rectangle(12 * (ap % 10), 120, 12, 24), Color.White);
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
                        spriteBatch.Draw(t_icon1, new Vector2(10, 10), new Rectangle((int)currentUnit.unit.type * 32, 72, 32, 32), Color.White);
                        if (currentUnit.drive)
                            spriteBatch.Draw(t_icon1, new Vector2(26, 26), new Rectangle((int)currentUnit.unit.type2 * 16, 104, 16, 16), Color.White);
                        Helper.DrawStringWithOutLine(currentUnit.unit.name, new Vector2(52, 10));
                        Helper.DrawStringWithOutLine("HP:", new Vector2(10, 39));
                        str = currentUnit.HP.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(95 - font.MeasureString(str).X, 39));
                        spriteBatch.Draw(tw, new Rectangle(106, 48, 160, 18), Color.White);
                        spriteBatch.Draw(tw, new Rectangle(108, 51, 156, 12), Color.Black);
                        spriteBatch.Draw(tw, new Rectangle(108, 51, 156 * currentUnit.HP / currentUnit.HPmax, 12), Color.LimeGreen);
                        Helper.DrawStringWithOutLine("SP:", new Vector2(10, 63));
                        str = currentUnit.SP.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(95 - font.MeasureString(str).X, 63));
                        spriteBatch.Draw(tw, new Rectangle(106, 72, 160, 18), Color.White);
                        spriteBatch.Draw(tw, new Rectangle(108, 75, 156, 12), Color.Black);
                        spriteBatch.Draw(tw, new Rectangle(108, 75, 156 * currentUnit.SP / currentUnit.SPmax, 12), Color.SkyBlue);
                        Helper.DrawStringWithOutLine("AP:  /", new Vector2(10, 87));
                        str = GetUsingAP(true).ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(80 - font.MeasureString(str).X, 87));
                        str = currentUnit.AP.ToString();
                        Helper.DrawStringWithOutLine(str, new Vector2(122 - font.MeasureString(str).X, 87));

                        Rectangle rect;
                        #region 行動アイコン
                        if (actState == ActState.SelectAct)
                        {
                            if (currentUnit.unit.IsHaveAbility(Ability.Drive))
                            {
                                if (drive)
                                {
                                    spriteBatch.Draw(t_icon1, new Vector2(210, 120), new Rectangle(240, 72, 48, 24), Color.White);
                                    spriteBatch.Draw(t_icon1, new Vector2(220, 140), new Rectangle(240, 120, 48, 24), Color.White);
                                }
                                else if ((currentUnit.SP >= 10 && currentUnit.AP >= 6) || currentUnit.drive)
                                    spriteBatch.Draw(t_icon1, new Vector2(210, 120), new Rectangle(240, 48, 48, 24), Color.White);
                                else
                                    spriteBatch.Draw(t_icon1, new Vector2(210, 120), new Rectangle(240, 96, 48, 24), Color.White);
                            }

                            rect = new Rectangle(96, 0, 48, 48);
                            Color color;
                            if (selectedAct == 0)
                                color = Color.White;
                            else
                                color = Color.Gray;
                            spriteBatch.Draw(t_icon1, new Vector2(120, 276), rect, color);
                            spriteBatch.Draw(t_icon1, new Vector2(132, 288), new Rectangle(0, 48, 24, 24), color);

                            Vector2[] pos = { new Vector2(120, 106), new Vector2(6, 148), new Vector2(234, 148),
                                                new Vector2(6, 233), new Vector2(234, 233) };

                            for (int i = 0; i < 5; i++)
                            {
                                Act a = currentUnit.unit.acts[i];
                                if (selectedAct == i + 1)
                                    color = Color.White;
                                else
                                    color = Color.Gray;
                                spriteBatch.Draw(t_icon1, pos[i], rect, color);
                                spriteBatch.Draw(t_icon1, pos[i] + new Vector2(12), new Rectangle(GetIconFact(a) * 24, 48, 24, 24), color);
                            }
                            //if (drive)
                            //{new Vector2(120, 191)
                            //    if (selectedAct == 6)
                            //        color = Color.White;
                            //    else
                            //        color = Color.Gray;
                            //    spriteBatch.Draw(t_icon, pos[i], rect, color);
                            //    spriteBatch.Draw(t_icon, pos[i] + new Vector2(12), new Rectangle(GetIconFact(currentUnit.unit.acts[7]) * 24, 48, 24, 24), color);
                            //}
                        }
                        #endregion

                        // 行動詳細
                        if (selectedAct == 0)
                        {
                            Helper.DrawStringWithOutLine("移動", new Vector2(40, 320));
                        }
                        else
                        {
                            if (currentAct != null)
                            {
                                spriteBatch.Draw(t_icon1, new Vector2(8, 325), new Rectangle(GetIconFact(currentAct) * 24, 48, 24, 24), Color.White);
                                Helper.DrawStringWithOutLine(currentAct.name, new Vector2(40, 320));
                                Helper.DrawStringWithOutLine(Helper.GetStringActType(currentAct), new Vector2(60, 360));
                                if (!currentAct.IsLimited)
                                {
                                    Helper.DrawStringWithOutLine("AP:", new Vector2(60, 400));
                                    str = currentAct.ap.ToString();
                                    if (currentUnit.AP >= currentAct.ap)
                                        Helper.DrawStringWithOutLine(str, new Vector2(130 - font.MeasureString(str).X, 400));
                                    else
                                        Helper.DrawStringWithOutline(str, new Vector2(130 - font.MeasureString(str).X, 400), Color.Red);
                                }
                                else
                                {
                                    Helper.DrawStringWithOutLine("回数:  /", new Vector2(60, 400));
                                    str = currentActCount.ToString();
                                    if (currentActCount > 0)
                                        Helper.DrawStringWithOutLine(str, new Vector2(150 - font.MeasureString(str).X, 400));
                                    else
                                        Helper.DrawStringWithOutline(str, new Vector2(150 - font.MeasureString(str).X, 400), Color.Red);
                                    str = currentAct.count.ToString();
                                    Helper.DrawStringWithOutLine(str, new Vector2(192 - font.MeasureString(str).X, 400));
                                }
                                if (currentAct.IsSpell)
                                {
                                    Helper.DrawStringWithOutLine("SP:", new Vector2(180, 400));
                                    if (currentUnit.SP >= currentAct.sp)
                                        Helper.DrawStringWithOutLine(currentAct.sp.ToString(), new Vector2(222, 400));
                                    else
                                        Helper.DrawStringWithOutline(currentAct.sp.ToString(), new Vector2(222, 400), Color.Red);
                                }
                                Helper.DrawStringWithOutLine(Helper.GetStringActTarget(currentAct.target), new Vector2(60, 440));
                            }
                            else
                                Helper.DrawStringWithOutLine("覚えていない", new Vector2(40, 320));
                        }

                        #region カーソル表示
                        if (actState == ActState.SelectAct)
                        {
                            Vector2 pos = mapOrigin + new Vector2(24 + currentUnit.postion.X * 48, 24 + currentUnit.postion.Y * 48);
                            float r = GetCursorFact(gameTime.TotalGameTime, 30, 8, 300);
                            rect = new Rectangle(0, 0, 24, 24);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(-r, -r), rect, Color.White, 0, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(r, -r), rect, Color.White, MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(r, r), rect, Color.White, MathHelper.Pi, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(-r, r), rect, Color.White, -MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                        }
                        else if (actState == ActState.SelectMove)
                        {
                            Vector2 pos = mapOrigin + new Vector2(24 + unitCursor.X * 48, 24 + unitCursor.Y * 48);
                            float r = GetCursorFact(gameTime.TotalGameTime, 45, 15, 600);
                            rect = new Rectangle(0, 24, 24, 24);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(0, -r), rect, Color.White, 0, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(r, 0), rect, Color.White, MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(0, r), rect, Color.White, MathHelper.Pi, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(-r, 0), rect, Color.White, -MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                        }
                        else if (actState == ActState.SelectTarget)
                        {
                            Vector2 pos = unitCursor;
                            if (unitCursor != currentUnit.postion)
                                spriteBatch.Draw(currentUnit.unit.t_icon, mapOrigin + pos * 48, new Color(128, 128, 128, 128));
                            float f;
                            bool aim = false;
                            rect = new Rectangle(24, 0, 24, 24);
                            foreach (UnitGadget ug in GetActTarget())
                            {
                                Positon p = ug.postion;
                                if (ug == currentUnit)
                                    p = unitCursor;
                                pos = mapOrigin + new Vector2(p.X * 48, p.Y * 48);
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
                                            if (ug.IsType(Type.Fortune))
                                                success = (int)(success * 0.8);
                                        }
                                        else if (currentAct.type == ActType.AddMinusSympton)
                                            success -= avoid / 2;

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
                                    spriteBatch.Draw(t_icon1, pos, new Rectangle(48, 0, 48, 48), color);
                                }
                                if (p == targetCursor || currentAct.IsTargetAll)
                                {
                                    pos += new Vector2(24, 24);
                                    f = GetCursorFact2(gameTime.TotalGameTime, 15, 8, 450);
                                    spriteBatch.Draw(t_icon1, pos + new Vector2(-f, -f), rect, Color.White, 0, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                                    spriteBatch.Draw(t_icon1, pos + new Vector2(f, -f), rect, Color.White, MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                                    spriteBatch.Draw(t_icon1, pos + new Vector2(f, f), rect, Color.White, MathHelper.Pi, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                                    spriteBatch.Draw(t_icon1, pos + new Vector2(-f, f), rect, Color.White, -MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                                    aim = true;
                                }
                            }
                            if (!aim)
                            {
                                pos = mapOrigin + new Vector2(24 + targetCursor.X * 48, 24 + targetCursor.Y * 48);
                                f = GetCursorFact(gameTime.TotalGameTime, 35, 8, 450);
                                spriteBatch.Draw(t_icon1, pos + new Vector2(-f, -f), rect, Color.White, 0, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                                spriteBatch.Draw(t_icon1, pos + new Vector2(f, -f), rect, Color.White, MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                                spriteBatch.Draw(t_icon1, pos + new Vector2(f, f), rect, Color.White, MathHelper.Pi, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                                spriteBatch.Draw(t_icon1, pos + new Vector2(-f, f), rect, Color.White, -MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
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
                        spriteBatch.Draw(currentUnit.unit.t_icon, mapOrigin + new Vector2(p.X * 48, p.Y * 48), new Color(128, 128, 128, 128));
                        spriteBatch.Draw(t_icon1, mapOrigin + new Vector2(p.X * 48 + 12, p.Y * 48 + 12), new Rectangle(144, 120, 24, 24), Color.White);
                        p = mapCost[p.X, p.Y].parent;
                    }
                    Vector2 pos = mapOrigin + new Vector2(24 + targetCursor.X * 48, 24 + targetCursor.Y * 48);
                    float r = GetCursorFact(gameTime.TotalGameTime, 35, 8, 300);
                    Rectangle rect = new Rectangle(0, 0, 24, 24);
                    spriteBatch.Draw(t_icon1, pos + new Vector2(-r, -r), rect, Color.White, 0, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(t_icon1, pos + new Vector2(r, -r), rect, Color.White, MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(t_icon1, pos + new Vector2(r, r), rect, Color.White, MathHelper.Pi, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                    spriteBatch.Draw(t_icon1, pos + new Vector2(-r, r), rect, Color.White, -MathHelper.PiOver2, new Vector2(12, 12), 1, SpriteEffects.None, 0);
                }
                else if (actState == ActState.TargetCursor) // 対象表示
                {
                    float tt = (time % 400) / 400;
                    if (tt > 0.1)
                    {
                        tt = (tt - 0.1f) * 1.11111f;
                        if (currentAct.IsTargetAll)
                        {
                            foreach(UnitGadget ug in GetActTarget())
                            {
                                if (currentAct.IsCover && currentUnit.ff == FrendOfFoe.Enemy && ug != currentUnit)
                                    continue;

                                Positon tar = ug.postion;
                                Vector2 v = mapOrigin + new Vector2(48 * MathHelper.Lerp(currentUnit.postion.X, tar.X, tt),
                                    48 * MathHelper.Lerp(currentUnit.postion.Y, tar.Y, tt));

                                spriteBatch.Draw(t_icon1, v, new Rectangle(48, 0, 48, 48), Color.White);
                            }
                        }
                        else
                        {
                            Vector2 v = mapOrigin + new Vector2(48 * MathHelper.Lerp(currentUnit.postion.X, targetCursor.X, tt),
                                48 * MathHelper.Lerp(currentUnit.postion.Y, targetCursor.Y, tt));

                            spriteBatch.Draw(t_icon1, v, new Rectangle(48, 0, 48, 48), Color.White);
                        }
                    }
                }
                else if (actState == ActState.ShowActExplain) // 行動表示
                {
                    Helper.DrawWindowBottom2(Helper.GetStringActType(currentAct), currentAct.name);
                }
                else if (actState == ActState.CalcAct) // 行動計算
                {
                }
                else if (actState == ActState.Attack_Heal) // 攻撃回復
                {
                    #region
                    for (int i = 0; i < GetActTarget().Count; i++)
                    {
                        if (currentAct.IsTargetAll)
                            target = GetActTarget()[i];
                        if (currentAct.TypeInt % 100 == 0 && !calcResult[i].hit)
                        {
                            Vector2 pos = mapOrigin + new Vector2(target.postion.X * 48, target.postion.Y * 48 + 8 - (int)GetIconFact());
                            spriteBatch.Draw(t_icon1, pos, new Rectangle(144, 32, 48, 32), Color.White);
                            if (!currentAct.IsTargetAll)
                                Helper.DrawWindowBottom1(target.unit.nickname + "は攻撃を回避した！");
                        }
                        else if (currentAct.TypeInt % 100 == 0 && calcResult[i].damage == 0)
                        {
                            Vector2 pos = mapOrigin + new Vector2(target.postion.X * 48, target.postion.Y * 48 + 8 - (int)GetIconFact());
                            spriteBatch.Draw(t_icon1, pos, new Rectangle(144, 0, 48, 32), Color.White);
                            if (!currentAct.IsTargetAll)
                                Helper.DrawWindowBottom1(target.unit.nickname + "は攻撃を無効化した！");
                        }
                        else
                        {
                            UnitGadget tar = countered ? currentUnit : target;
                            Vector2 pos = mapOrigin + 48 * (Vector2)tar.postion + new Vector2(12, 24 - (int)GetIconFact());
                            int damage = Math.Abs(tar.HP - tar.HPold);
                            if (damage >= 100)
                                pos.X -= 6;
                            else if (damage < 10)
                                pos.X += 6;
                            Color color = Color.White;
                            if (calcResult[i].critical)
                                color = new Color(1, 0.64f, 0.3f);
                            if (currentAct.TypeInt % 100 != 0)
                            {
                                pos.X += 6;
                                spriteBatch.Draw(t_icon1, pos - new Vector2(-12, 0), new Rectangle(120, 120, 12, 24), color);
                            }
                            if (damage >= 100)
                            {
                                spriteBatch.Draw(t_icon1, pos, new Rectangle(12 * (damage / 100), 120, 12, 24), color);
                                pos.X += 12;
                            }
                            if (damage >= 10)
                            {
                                spriteBatch.Draw(t_icon1, pos, new Rectangle(12 * ((damage % 100) / 10), 120, 12, 24), color);
                                pos.X += 12;
                            }
                            spriteBatch.Draw(t_icon1, pos, new Rectangle(12 * (damage % 10), 120, 12, 24), color);

                            pos = mapOrigin + (Vector2)tar.postion * 48;
                            spriteBatch.Draw(tw, new Rectangle((int)pos.X, (int)pos.Y + 48, 48, 6), Color.Black);
                            float hp = 46 * MathHelper.Lerp((float)tar.HPold / tar.HPmax, (float)tar.HP / tar.HPmax, MathHelper.Clamp(time / 1000, 0, 1));
                            spriteBatch.Draw(tw, new Rectangle((int)pos.X + 1, (int)pos.Y + 49, (int)hp, 4), Color.LimeGreen);
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
                                    p = mapOrigin + new Vector2(ug.postion.X * 48, ug.postion.Y * 48);
                                else
                                    p = mapOrigin + new Vector2(ug.postion.X * 48 + 24, ug.postion.Y * 48);
                                spriteBatch.Draw(t_icon1, p, new Rectangle(192, 48 + 24 * (int)(time / 250), 24, 24), Color.White);
                            }
                        }
                    }

                    Helper.DrawWindowBottom1("マイナス症状にダメージを与えた！");
                    #endregion
                }
                else if (actState >= ActState.AddSympSub && actState <= ActState.AddSymp) // 付加
                {
                    #region アニメーション
                    float t = time / timeSpan;
                    if (actState == ActState.AddSympSub) // 付加飛ばし
                    {
                        Vector2 v = mapOrigin + new Vector2(24) + 48 * Vector2.Lerp(unitCursor, target.postion, t);
                        if (currentAct.type == ActType.AddPlusSympton)
                        {
                            switch ((SymptonPlus)currentAct.sympton)
                            {
                                case SymptonPlus.Heal:
                                    spriteBatch.Draw(t_icon3, v - new Vector2(24), new Rectangle(288, 48 * (int)(time % 500 / 125), 48, 48), Color.White);
                                    break;
                                case SymptonPlus.Charge:
                                    if (time % 500 <= 250)
                                        spriteBatch.Draw(t_icon3, v - new Vector2(24), new Rectangle(384, 48 * (int)(time % 500 / 62.5), 48, 48), Color.White);
                                    else
                                        spriteBatch.Draw(t_icon3, v - new Vector2(24), new Rectangle(384, 48 * (int)((500 - time % 500) / 62.5), 48, 48), Color.White);
                                    break;
                                case SymptonPlus.Concentrate:
                                    spriteBatch.Draw(t_icon3, v, new Rectangle(336, 240, 48, 48), Color.White, -time / 500 * MathHelper.TwoPi, new Vector2(24), 1, SpriteEffects.None, 0);
                                    spriteBatch.Draw(t_icon3, v, new Rectangle(288, 240, 48, 48), Color.White, time / 500 * MathHelper.TwoPi, new Vector2(24), 1, SpriteEffects.None, 0);
                                    break;
                                case SymptonPlus.Swift:
                                    spriteBatch.Draw(t_icon3, v - new Vector2(24), new Rectangle(432, 48 * (int)(time % 500 / 125), 48, 48), Color.White);
                                    break;
                                case SymptonPlus.ActAgain:
                                    spriteBatch.Draw(t_icon3, v, new Rectangle(384, 48, 48, 48), Color.White, time / 500 * MathHelper.TwoPi, new Vector2(24), 1, SpriteEffects.None, 0);
                                    break;
                            }
                        }
                        else if (currentAct.type == ActType.ClearMinusSympton)
                        {
                            spriteBatch.Draw(t_icon3, v, new Rectangle(96, 0, 96, 96), Color.White, time / 500 * MathHelper.TwoPi, new Vector2(48), 0.5f, SpriteEffects.None, 0);
                        }
                        else if (currentAct.type == ActType.ClearPlusSympton)
                        {
                            spriteBatch.Draw(t_icon3, v, new Rectangle(192, 0, 96, 96), Color.White, time / 500 * MathHelper.TwoPi, new Vector2(48), 0.5f, SpriteEffects.None, 0);
                        }
                        else if (currentAct.type == ActType.ClearTrap)
                        {
                            spriteBatch.Draw(t_icon4, v, new Rectangle(0, 672, 96, 96), Color.White, 0, new Vector2(48), 1, SpriteEffects.None, 0);
                        }
                        else if (currentAct.type == ActType.SPUp)
                        {
                            spriteBatch.Draw(t_icon3, v, new Rectangle(336, 192, 48, 48), Color.White, 0, new Vector2(24), 1 - ((time % 500) / 500), SpriteEffects.None, 0);
                            spriteBatch.Draw(t_icon3, v, new Rectangle(288, 192, 48, 48), Color.White, -time / 500 * MathHelper.TwoPi, new Vector2(24), 1, SpriteEffects.None, 0);
                        }
                        else if (currentAct.type == ActType.Revive || currentAct.type == ActType.Revive2)
                        {
                            spriteBatch.Draw(t_icon3, v - new Vector2(24), new Rectangle(336, 48 * (int)(time % 500 / 125), 48, 48), Color.White);
                        }
                    }
                    else if (actState == ActState.AddSymp) // 付加
                    {
                        Vector2 v = mapOrigin + new Vector2(24) + 48 * (Vector2)target.postion;
                        if (currentAct.type == ActType.AddPlusSympton)
                        {
                            switch ((SymptonPlus)currentAct.sympton)
                            {
                                case SymptonPlus.Heal:
                                    spriteBatch.Draw(t_icon3, v + new Vector2(-24, -24 - 16 * t), new Rectangle(288, 48 * (int)(time % 500 / 125), 48, 48), Color.White);
                                    target.spChargeFact = time / 1000;
                                    break;
                                case SymptonPlus.Charge:
                                    if (time <= 500)
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(0, 192, 96, 96), new Color(128, 128, 128, 128), 0, new Vector2(48), time / 500, SpriteEffects.None, 0);
                                    else if (time <= 1500)
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(0, 288 + 96 * (int)((time - 500) / 200), 96, 96), new Color(128, 128, 128, 128), 0, new Vector2(48), 1, SpriteEffects.None, 0);
                                    else if (time <= 2500)
                                        target.spChargeFact = (time - 1500) / 1000;
                                    break;
                                case SymptonPlus.Concentrate:
                                    spriteBatch.Draw(t_icon3, v, new Rectangle(336, 240, 48, 48), Color.White, -time / 500 * MathHelper.TwoPi, new Vector2(24), 1.5f - t, SpriteEffects.None, 0);
                                    spriteBatch.Draw(t_icon3, v, new Rectangle(288, 240, 48, 48), Color.White, time / 500 * MathHelper.TwoPi, new Vector2(24), 1, SpriteEffects.None, 0);
                                    target.spChargeFact = time / 2000;
                                    break;
                                case SymptonPlus.Swift:
                                    if (time <= 500)
                                        spriteBatch.Draw(t_icon3, v + new Vector2(-24), new Rectangle(432, 192, 48, 48), Color.White);
                                    else if (time <= 1500)
                                        spriteBatch.Draw(t_icon3, v + new Vector2(-24), new Rectangle(432, 192 + 48 * (int)((time - 500) % 1000 / 200), 48, 48), Color.White);
                                    else
                                        spriteBatch.Draw(t_icon3, v + new Vector2(-24), new Rectangle(432, 384, 48, 48), Color.White);
                                    target.spChargeFact = time / 2000;
                                    break;
                                case SymptonPlus.ActAgain:
                                    if (time <= 500)
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(0, 192, 96, 96), new Color(128, 128, 128, 128), 0, new Vector2(48), time / 500, SpriteEffects.None, 0);
                                    else if (time <= 1500)
                                        spriteBatch.Draw(t_icon3, v, new Rectangle(0, 288 + 96 * (int)((time - 500) / 200), 96, 96), new Color(128, 128, 128, 128), 0, new Vector2(48), 1, SpriteEffects.None, 0);
                                    else if (time <= 2500)
                                    {
                                        Vector2 pos = mapOrigin + new Vector2(targetCursor.X * 48 + 12, targetCursor.Y * 48 + 24 - (int)GetIconFact(1500));
                                        spriteBatch.Draw(t_icon1, pos, new Rectangle(120, 120, 12, 24), Color.White);
                                        spriteBatch.Draw(t_icon1, pos + new Vector2(12, 0), new Rectangle(12, 120, 12, 24), Color.White);
                                    }
                                    break;
                            }
                        }
                        else if (currentAct.type == ActType.ClearMinusSympton || currentAct.type == ActType.ClearPlusSympton)
                        {
                            int x = currentAct.type == ActType.ClearMinusSympton ? 96 : 192;
                            if (time <= 500)
                                spriteBatch.Draw(t_icon3, v, new Rectangle(x, 0, 96, 96), Color.White, time / 500 * MathHelper.TwoPi, new Vector2(48), 0.25f + time / 1500, SpriteEffects.None, 0);
                            else if (time <= 900)
                                spriteBatch.Draw(t_icon3, v, new Rectangle(x, 96 + 96 * (int)((time - 500) / 134), 96, 96), Color.White, 0, new Vector2(48), 1, SpriteEffects.None, 0);
                            else if (time <= 1000)
                            {// なし
                            }
                            else
                                spriteBatch.Draw(t_icon3, v, new Rectangle(x, 384 + 96 * (int)((time - 1000) / 125), 96, 96), Color.White, 0, new Vector2(48), 1, SpriteEffects.None, 0);
                        }
                        else if (currentAct.type == ActType.ClearTrap)
                        {
                            if (time < 500)
                                spriteBatch.Draw(t_icon4, v, new Rectangle(96 * (int)((time % 250) / 125), 672, 96, 96), Color.White, 0, new Vector2(48), 1, SpriteEffects.None, 0);
                            else
                            {
                                float a = 1 - (time - 500) / 500;
                                spriteBatch.Draw(t_icon4, v, new Rectangle(0, 672, 96, 96), new Color(a, a, a, a), 0, new Vector2(48), 1, SpriteEffects.None, 0);
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
                                spriteBatch.Draw(t_icon3, v + new Vector2(-24, -24 - 16 * t), new Rectangle(336, 48 * (int)(time % 500 / 125), 48, 48), Color.White);
                                target.spChargeFact = time / 1000;
                            }
                            else
                            {
                                float d;
                                if (time < 1200)
                                    d = 120 * (float)Math.Sin(time / 1200 * MathHelper.PiOver2);
                                else if (time < 2400)
                                    d = 100 + 20 * (float)Math.Cos((time - 1200) / 1200 * MathHelper.Pi);
                                else
                                    d = 140 + 60 * -(float)Math.Cos((time - 2400) / 1600 * MathHelper.Pi);
                                for (int i = 0; i < 6; i++)
                                {
                                    spriteBatch.Draw(t_icon3, mapOrigin + new Vector2(216) + Helper.GetPolarCoord(d, (time / 2000 + 0.166f * i) * MathHelper.TwoPi),
                                        new Rectangle(336, 48 * (int)(time % 500 / 125), 48, 48), Color.White, 0, new Vector2(24), 1, SpriteEffects.None, 0);
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
                                Helper.DrawWindowBottom1("集中のプラス症状！成功と回避がアップ！");
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
                    Vector2 v = mapOrigin + new Vector2(24) + 48 * (Vector2)target.postion;
                    if (currentAct.type == ActType.ClearMinusSympton || currentAct.type == ActType.ClearPlusSympton)
                    {
                        int x = currentAct.type == ActType.ClearMinusSympton ? 96 : 192;
                        if (time <= 500)
                            spriteBatch.Draw(t_icon3, v, new Rectangle(x, 0, 96, 96), Color.White, time / 500 * MathHelper.TwoPi, new Vector2(48), 0.25f + time / 1500, SpriteEffects.None, 0);
                        else if (time <= 900)
                            spriteBatch.Draw(t_icon3, v, new Rectangle(x, 96 + 96 * (int)((time - 500) / 134), 96, 96), Color.White, 0, new Vector2(48), 1, SpriteEffects.None, 0);
                        else if (time <= 1000)
                        {// なし
                        }
                        else if (time <= 1750)
                        {
                            Vector2 p;
                            if (currentAct.type == ActType.ClearMinusSympton)
                            {
                                x = 192;
                                if (target.ff == FrendOfFoe.Ally)
                                    p = mapOrigin + 48 * (Vector2)target.postion;
                                else
                                    p = mapOrigin + 48 * (Vector2)target.postion + new Vector2(24, 0);
                            }
                            else
                            {
                                x = 216;
                                if (target.ff == FrendOfFoe.Ally)
                                    p = mapOrigin + 48 * (Vector2)target.postion + new Vector2(24, 0);
                                else
                                    p = mapOrigin + 48 * (Vector2)target.postion;
                            }
                            spriteBatch.Draw(t_icon1, p, new Rectangle(x, 48 + 24 * (int)((time - 1000) / 250), 24, 24), Color.White);
                        }
                    }
                    else
                    {
                        Vector2 vc = mapOrigin + new Vector2(216);
                        Rectangle rect = new Rectangle(0, 672, 96, 96);
                        if (time < 1000)
                        {
                            float t = time / 1000;
                            float s = 1 + t;
                            spriteBatch.Draw(t_icon4, Vector2.Lerp(v, vc, t), rect, Color.White, 0, new Vector2(48), s, SpriteEffects.None, 0);
                        }
                        else if (time < 3000)
                        {
                            spriteBatch.Draw(t_icon4, vc, rect, Color.White, 0, new Vector2(48), 2, SpriteEffects.None, 0);
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
                                spriteBatch.Draw(t_icon4, Vector2.Lerp(vc, v, t), rect, Color.White, 0, new Vector2(48), s, SpriteEffects.None, 0);
                            }
                            else
                            {
                                float a = 1 - (time - 4000) / 500;
                                spriteBatch.Draw(t_icon4, v, rect, new Color(a, a, a, a), 0, new Vector2(48), 1, SpriteEffects.None, 0);
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
                    Vector2 v = mapOrigin + new Vector2(24) + 48 * (Vector2)unitCursor;
                    Rectangle rect = new Rectangle();
                    if (currentAct.type == ActType.SPDrain)
                    {
                        rect = new Rectangle(384, 192, 48, 48);
                        str = "SPを" + calcResult[0].damage + "ポイント吸収した！";
                    }
                    else if (currentAct.type == ActType.LevelDrain)
                    {
                        rect = new Rectangle(384, 240, 48, 48);
                        str = "相手のレベルを奪いとった！";
                    }
                    for (int i = 0; i < GetActTarget().Count; i++)
                    {
                        UnitGadget tar = currentAct.IsTargetAll ? GetActTarget()[i] : target;
                        for (int j = 0; j < 8; j++)
                        {
                            float t = time - i * 200 - j * 125;
                            if (t >= 0 && t < 1000)
                            {
                                Vector2 s = mapOrigin + 48 * (Vector2)tar.postion;
                                Vector2 r = s + Helper.GetPolarCoord(96, MathHelper.PiOver4 * j - MathHelper.PiOver2);
                                Vector2 g = mapOrigin + 48 * (Vector2)currentUnit.postion;
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
                        Vector2 p1 = mapOrigin + new Vector2(currentUnit.postion.X * 48 + 24, currentUnit.postion.Y * 48 + 24);
                        Vector2 p2 = mapOrigin + new Vector2(216);
                        float s = 1 + t;
                        spriteBatch.Draw(t_icon4, Vector2.Lerp(p1, p2, t), rect, Color.White, 2 * MathHelper.TwoPi * t, new Vector2(48), s, SpriteEffects.None, 0);
                    }
                    else if (time < 1500)
                    {
                        spriteBatch.Draw(t_icon4, mapOrigin + new Vector2(216), rect, Color.White, 0, new Vector2(48), 2, SpriteEffects.None, 0);
                    }
                    else if (time < 2500)
                    {
                        float t = (time - 1500) / 1000;
                        Vector2 p1 = mapOrigin + new Vector2(216);
                        float s = 2 - t;
                        if (!currentAct.IsTargetAll)
                        {
                            Vector2 p2 = mapOrigin + new Vector2(target.postion.X * 48 + 24, target.postion.Y * 48 + 24);
                            spriteBatch.Draw(t_icon4, Vector2.Lerp(p1, p2, t), rect, Color.White, 2 * MathHelper.TwoPi * t, new Vector2(48), s, SpriteEffects.None, 0);
                        }
                        else
                        {
                            foreach (UnitGadget ug in GetActTarget())
                            {
                                Vector2 p2 = mapOrigin + new Vector2(ug.postion.X * 48 + 24, ug.postion.Y * 48 + 24);
                                spriteBatch.Draw(t_icon4, Vector2.Lerp(p1, p2, t), rect, Color.White, 2 * MathHelper.TwoPi * t, new Vector2(48), s, SpriteEffects.None, 0);
                            }
                        }
                    }
                    else if (time < 3000)
                    {
                        float t = (time - 2500) / 500;
                        float a = 1 - t;
                        if (!currentAct.IsTargetAll)
                        {
                            Vector2 p = mapOrigin + new Vector2(target.postion.X * 48 + 24, target.postion.Y * 48 + 24);
                            spriteBatch.Draw(t_icon4, p, rect, new Color(a, a, a, a), 0, new Vector2(48), 1, SpriteEffects.None, 0);
                        }
                        else
                        {
                            foreach (UnitGadget ug in GetActTarget())
                            {
                                Vector2 p = mapOrigin + new Vector2(ug.postion.X * 48 + 24, ug.postion.Y * 48 + 24);
                                spriteBatch.Draw(t_icon4, p, rect, new Color(a, a, a, a), 0, new Vector2(48), 1, SpriteEffects.None, 0);
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
                        Vector2 pos = mapOrigin + new Vector2(216);
                        if (currentAct.fact == '氷')
                        {
                            float d;
                            if (time < 1200)
                                d = 120 * (float)Math.Sin(time / 1200 * MathHelper.PiOver2);
                            else if (time < 2400)
                                d = 100 + 20 * (float)Math.Cos((time - 1200) / 1200 * MathHelper.Pi);
                            else
                                d = 140 + 60 * -(float)Math.Cos((time - 2400) / 1600 * MathHelper.Pi);
                            for (int i = 0; i < 6; i++)
                            {
                                spriteBatch.Draw(t_icon3, pos + Helper.GetPolarCoord(d, (time / 2000 + 0.166f * i) * MathHelper.TwoPi),
                                    new Rectangle(432, 624, 48, 48), Color.White, 0, new Vector2(24), 1, SpriteEffects.None, 0);
                            }
                        }
                    }
                    #endregion

                    #region 説明
                    switch ((CrystalEffect)currentAct.sympton)
                    {
                        case CrystalEffect.HPDamage:
                            Helper.DrawWindowBottom1("HPダメージの結晶効果！ターン始めにダメージ！");
                            break;
                        case CrystalEffect.HPHeal:
                            Helper.DrawWindowBottom1("HP回復の結晶効果！ターン始めに回復！");
                            break;
                        case CrystalEffect.APUp:
                            Helper.DrawWindowBottom1("APアップの結晶効果！APがアップする！");
                            break;
                        case CrystalEffect.APDown:
                            Helper.DrawWindowBottom1("APダウンの結晶効果！APがダウンする！");
                            break;
                        case CrystalEffect.HitUp:
                            Helper.DrawWindowBottom1("成功アップの結晶効果！全ての成功がアップ！");
                            break;
                        case CrystalEffect.CostUp:
                            Helper.DrawWindowBottom1("コスト増加の結晶効果！移動コストが増える！");
                            break;
                        case CrystalEffect.Suppression:
                            Helper.DrawWindowBottom1("侵蝕抑制の結晶効果！結晶侵食度がダウンする！");
                            break;
                        case CrystalEffect.AffinityDown:
                            Helper.DrawWindowBottom1("適正ダウンの結晶効果！地形相性がダウンする！");
                            break;
                        case CrystalEffect.AffinityReverse:
                            Helper.DrawWindowBottom1("相性反転の結晶効果！地形相性が逆になる！");
                            break;
                        case CrystalEffect.SympInvalid:
                            Helper.DrawWindowBottom1("症状クリアの結晶効果！症状を付加できない！");
                            break;
                        case CrystalEffect.DamageFix:
                            Helper.DrawWindowBottom1("ダメージ固定の結晶効果！攻撃ダメージが" + currentAct.power + "になる！");
                            break;
                        case CrystalEffect.TimeStop:
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
                    Vector2 v = mapOrigin + 48 * (Vector2)currentUnit.postion + new Vector2(24);
                    int x = currentAct.IsCover ? 480 : 528;
                    if (time < 2000)
                    {
                        float t = time / 2000;
                        int y = time < 100 || time >= 1900 ? 0 : (time < 200 || time >= 1800 ? 96 : 192);
                        float r = time < 1800 ? -MathHelper.TwoPi * time / 1800 : 0;
                        if (time < 500)
                            spriteBatch.Draw(t_icon3, v, new Rectangle(0, 192, 96, 96), new Color(128, 128, 128, 128), 0, new Vector2(48), time / 500, SpriteEffects.None, 0);
                        else if (time < 1500)
                            spriteBatch.Draw(t_icon3, v, new Rectangle(0, 288 + 96 * (int)((time - 500) / 200), 96, 96), new Color(128, 128, 128, 128), 0, new Vector2(48), 1, SpriteEffects.None, 0);
                        spriteBatch.Draw(t_icon3, v, new Rectangle(480, y, 96, 96), Color.White, r, new Vector2(24, 72), 1, SpriteEffects.None, 0);
                    }
                    else if (time < 2500)
                    {
                        float t = (time - 2000) / 500;
                        spriteBatch.Draw(t_icon3, v, new Rectangle(x, 288, 48, 48), Color.White, 0, new Vector2(24), 2 - t, SpriteEffects.None, 0);
                    }
                    else
                    {
                        float t = (time - 2500) / 500;
                        spriteBatch.Draw(t_icon3, v - new Vector2(24), new Rectangle(x, 288 + 48 * ((int)(t * 5) % 4), 48, 48), Color.White);
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
                    Vector2 vc = mapOrigin + new Vector2(216);
                    if (time < 300)
                    {
                        float t = time / 300;
                        Vector2 v = mapOrigin + 48 * (Vector2)target.postion + new Vector2(24);
                        spriteBatch.Draw(t_icon3, Vector2.Lerp(v, vc, t), new Rectangle(480, 288, 48, 48), Color.White, 0, new Vector2(24), 1 + t, SpriteEffects.None, 0);
                    }
                    else if (time < 800)
                    {
                        float t = (time - 300) / 500;
                        spriteBatch.Draw(t_icon3, vc, new Rectangle(480, 288 + 48 * ((int)(t * 5) % 4), 48, 48), Color.White, 0, new Vector2(24), 2, SpriteEffects.None, 0);
                    }
                    else if (time < 1100)
                    {
                        float t = (time - 800) / 300;
                        Vector2 v = mapOrigin + 48 * (Vector2)target_old.postion + new Vector2(24);
                        spriteBatch.Draw(t_icon3, Vector2.Lerp(vc, v, t), new Rectangle(480, 288, 48, 48), Color.White, 0, new Vector2(24), 2 - t, SpriteEffects.None, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(t_icon3, mapOrigin + 48 * (Vector2)target_old.postion, new Rectangle(480, 288, 48, 48), Color.White);
                    }
                    Helper.DrawWindowBottom1(target.unit.nickname + "が援護に入った！");
                }
                else if (actState == ActState.DefeatEnemy) // 敵撃破
                {
                    if (time < 2000)
                    {
                        List<UnitGadget> lug = GetActTarget();
                        for (int i = 0; i < lug.Count; i++)
                        {
                            if (lug[i].HP > 0)
                                continue;
                            Vector2 pos = mapOrigin + new Vector2(24) + 48 * (Vector2)lug[i].postion;
                            for (int j = 0; j < 8; j++)
                            {
                                spriteBatch.Draw(t_icon1, pos + Helper.GetPolarCoord(time * 0.024f, j * MathHelper.PiOver4),
                                    new Rectangle(168, 120, 16, 16), Color.White, 0, new Vector2(8), time % 200 > 100 ? 1 : 0.75f, SpriteEffects.None, 0);
                            }
                        }
                    }
                    else
                    {
                        if (!currentAct.IsTargetAll)
                            str = GetActTarget()[targetNo].unit.nickname;
                        else
                        {
                            str = "ユニット";
                        }
                        Helper.DrawWindowBottom1(str + "は戦闘不能になった！");
                    }
                }
                else if (actState == ActState.Defeated) // 自滅
                {
                    if (time < 2000)
                    {
                        Vector2 pos = mapOrigin + new Vector2(24) + 48 * (Vector2)currentUnit.postion;
                        for (int j = 0; j < 8; j++)
                        {
                            spriteBatch.Draw(t_icon1, pos + Helper.GetPolarCoord(time * 0.024f, j * MathHelper.PiOver4),
                                new Rectangle(168, 120, 16, 16), Color.White, 0, new Vector2(8), time % 200 > 100 ? 1 : 0.75f, SpriteEffects.None, 0);
                        }
                    }
                    else
                    {
                        Helper.DrawWindowBottom1(currentUnit.unit.nickname + "は戦闘不能になった！");
                    }
                }
                else if (actState == ActState.SymActAgain) // 連続行動
                {
                    if (actTimes == 0 && currentUnit.unit.IsHaveAbility(Ability.ActAgain))
                        Helper.DrawWindowBottom1("アビリティによりもう一度行動が可能になった！");
                    else
                        Helper.DrawWindowBottom1("プラス症状によりもう一度行動が可能になった！");
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
                    Vector2 pos = mapOrigin + new Vector2(currentUnit.postion.X * 48, currentUnit.postion.Y * 48 + 8 - (int)GetIconFact());
                    spriteBatch.Draw(t_icon1, pos, new Rectangle(144, 0, 48, 32), Color.White);
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
                                if (Crystal.sympton == CrystalEffect.SympInvalid)
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
                            case ActType.CrystalDrain:
                                if (crystal_base.sympton == CrystalEffect.None)
                                    Helper.DrawWindowBottom1("吸収する結晶効果がなかった！");
                                else
                                    Helper.DrawWindowBottom1("この結晶効果は吸収できない！");
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
                            Vector2 pos1 = mapOrigin + 48 * (Vector2)currentUnit.postion - new Vector2(24);
                            Vector2 pos2 = mapOrigin + 48 * (Vector2)target.postion - new Vector2(24);
                            spriteBatch.Draw(t_icon4, Vector2.Lerp(pos2, pos1, time / (timeSpan - 1000)), new Rectangle(0, 288, 96, 96), Color.White);
                        }
                        else
                        {
                            float t = time - (timeSpan - 1000);
                            Vector2 pos = mapOrigin + 48 * (Vector2)currentUnit.postion - new Vector2(24);
                            spriteBatch.Draw(t_icon4, pos, new Rectangle(96, 288 + 96 * (int)(t % 250 / 125), 96, 96), Color.White);
                        }
                        Helper.DrawWindowBottom1("単発クリアにより攻撃が打ち消された！");
                    }
                    else
                    {
                        if (time < timeSpan - 3000)
                        {
                            Vector2 pos1 = mapOrigin + 48 * (Vector2)currentUnit.postion - new Vector2(24);
                            Vector2 pos2 = mapOrigin + 48 * (Vector2)target.postion - new Vector2(24);
                            if (currentAct.type == ActType.Grapple)
                                spriteBatch.Draw(t_icon4, Vector2.Lerp(pos2, pos1, time / (timeSpan - 3000)), new Rectangle(0, 0, 96, 96), Color.White);
                            else
                                spriteBatch.Draw(t_icon4, Vector2.Lerp(pos2, pos1, time / (timeSpan - 3000)), new Rectangle(0, 96, 96, 96), Color.White);
                        }
                        else if (time < timeSpan - 2000)
                        {
                            float t = time - (timeSpan - 3000);
                            Vector2 pos = mapOrigin + 48 * (Vector2)currentUnit.postion - new Vector2(24);
                            if (currentAct.type == ActType.Grapple)
                                spriteBatch.Draw(t_icon4, pos, new Rectangle(96 * (int)(t % 250 / 125), 0, 96, 96), Color.White);
                            else
                                spriteBatch.Draw(t_icon4, pos, new Rectangle(96, 96 + 96 * (int)(t % 125 / 62.5), 96, 96), Color.White);
                        }
                        else
                        {
                            Vector2 pos = mapOrigin + 48 * (Vector2)currentUnit.postion + new Vector2(0, 24 - (int)GetIconFact(timeSpan - 2000));
                            int damage = Math.Abs(currentUnit.HP - currentUnit.HPold);
                            if (damage >= 100)
                                pos += new Vector2(6, 0);
                            else if (damage < 10)
                                pos += new Vector2(-6, 0);
                            spriteBatch.Draw(t_icon1, pos, new Rectangle(132, 120, 12, 24), Color.White);
                            if (damage >= 100)
                                spriteBatch.Draw(t_icon1, pos, new Rectangle(12 * (damage / 100), 120, 12, 24), Color.White);
                            if (damage >= 10)
                                spriteBatch.Draw(t_icon1, pos + new Vector2(12, 0), new Rectangle(12 * ((damage % 100) / 10), 120, 12, 24), Color.White);
                            spriteBatch.Draw(t_icon1, pos + new Vector2(24, 0), new Rectangle(12 * (damage % 10), 120, 12, 24), Color.White);

                            pos = mapOrigin + (Vector2)currentUnit.postion * 48;
                            spriteBatch.Draw(tw, new Rectangle((int)pos.X, (int)pos.Y + 48, 48, 6), Color.Black);
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
                    if (target.postion == forcedPos)
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
                    else if (currentAct.IsHaveAbility(ActAbility.Erosion))
                        Helper.DrawWindowBottom1("相手の結晶侵食度を増加させた！");
                    else
                        Helper.DrawWindowBottom1("体力の半分が減少した！");
                }
                #endregion

                #endregion
            }
            else if (!battleStart)
            {
                #region 状態確認
                if (actState == ActState.ListAlly || actState == ActState.ListEnemy)
                {
                    if (!IsMapMoving())
                    {
                        Positon p = new Positon();
                        if (actState == ActState.ListAlly)
                            p.X = 23;
                        else
                            p.X = 441;
                        if (selectedAct == 0)
                            p.Y = 10;
                        else
                            p.Y = -12 + selectedAct * 72;
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
                                spriteBatch.Draw(t_icon1, mapOrigin + pos * 48, new Rectangle(48, 0, 48, 48), Color.White);
                        }
                    }

                    Helper.DrawStringWithOutLine("戦闘開始", drawMapOrigin + new Vector2(-202, 7));
                    for (int i = 0; i < allyUnitGadget.Count; i++)
                    {
                        Helper.DrawStringWithOutline(allyUnitGadget[i].unit.name, drawMapOrigin + new Vector2(-263, 59 + i * 72),
                            allyUnitGadget[i].HP > 0 ? Color.White : Color.Gray);
                    }
                    for (int i = 0; i < enemyUnitGadget.Count; i++)
                    {
                        Helper.DrawStringWithOutline(enemyUnitGadget[i].unit.name, drawMapOrigin + new Vector2(443, 59 + i * 72),
                            enemyUnitGadget[i].HP > 0 ? Color.White : Color.Gray);
                    }
                    Helper.DrawWindowBottom1("startボタンで戦闘開始");
                }
                if (actState == ActState.ShowAlly || actState == ActState.ShowEnemy)
                {
                    UnitGadget ug;
                    int x1, x2, x3, x4;
                    if (actState == ActState.ShowAlly)
                    {
                        ug = allyUnitGadget[selectedAct - 1];
                        x1 = 0;
                        x2 = 320;
                        x3 = 3;
                        x4 = 211;
                    }
                    else
                    {
                        ug = enemyUnitGadget[selectedAct - 1];
                        x1 = 410;
                        x2 = 40;
                        x3 = 211;
                        x4 = 3;
                    }
                    DrawUnit(ug, gameTime);

                    spriteBatch.Draw(t_icon1, new Vector2(x1 + 10, 10), new Rectangle((int)ug.unit.type * 32, 72, 32, 32), Color.White);
                    if (ug.drive)
                        spriteBatch.Draw(t_icon1, new Vector2(x1 + 26, 26), new Rectangle((int)ug.unit.type2 * 16, 104, 16, 16), Color.White);
                    Helper.DrawStringWithOutLine(ug.unit.name, new Vector2(x1 + 52, 10));
                    Helper.DrawStringWithOutLine("HP:", new Vector2(x1 + 10, 39));
                    str = ug.HP.ToString();
                    Helper.DrawStringWithOutLine(str, new Vector2(x1 + 95 - font.MeasureString(str).X, 39));
                    spriteBatch.Draw(tw, new Rectangle(x1 + 106, 48, 160, 18), Color.White);
                    spriteBatch.Draw(tw, new Rectangle(x1 + 108, 51, 156, 12), Color.Black);
                    spriteBatch.Draw(tw, new Rectangle(x1 + 108, 51, 156 * ug.HP / ug.HPmax, 12), Color.LimeGreen);
                    Helper.DrawStringWithOutLine("SP:", new Vector2(x1 + 10, 63));
                    str = ug.SP.ToString();
                    Helper.DrawStringWithOutLine(str, new Vector2(x1 + 95 - font.MeasureString(str).X, 63));
                    spriteBatch.Draw(tw, new Rectangle(x1 + 106, 72, 160, 18), Color.White);
                    spriteBatch.Draw(tw, new Rectangle(x1 + 108, 75, 156, 12), Color.Black);
                    spriteBatch.Draw(tw, new Rectangle(x1 + 108, 75, 156 * ug.SP / ug.SPmax, 12), Color.SkyBlue);
                    Helper.DrawStringWithOutLine("AP: 0/", new Vector2(x1 + 10, 87));
                    str = ug.AP.ToString();
                    Helper.DrawStringWithOutLine(str, new Vector2(x1 + 122 - font.MeasureString(str).X, 87));

                    if (ug.symptonMinus.sympton > 0)
                        spriteBatch.Draw(t_icon2, new Vector2(x1 + x3, 120), new Rectangle(0,  48 * ((int)ug.symptonMinus.sympton - 1), 96, 48), Color.White);
                    if (ug.symptonPlus.sympton > 0)
                        spriteBatch.Draw(t_icon2, new Vector2(x1 + x4, 120), new Rectangle(96, 48 * ((int)ug.symptonPlus.sympton - 1), 96, 48), Color.White);
                    if (ug.trap.sympton > 0)
                        spriteBatch.Draw(t_icon2, new Vector2(x1 + x3, 429), new Rectangle(192, 48 * ((int)ug.trap.sympton - 1), 96, 48), Color.White);
                    if (ug.stance >= 0)
                    {
                        switch (ug.unit.acts[ug.stance].type)
                        {
                            case ActType.Guard:
                                spriteBatch.Draw(t_icon2, new Vector2(x1 + x4, 429), new Rectangle(288, 0, 96, 48), Color.White);
                                break;
                            case ActType.LessGuard:
                                spriteBatch.Draw(t_icon2, new Vector2(x1 + x4, 429), new Rectangle(288, 48, 96, 48), Color.White);
                                break;
                            case ActType.Utsusemi:
                                spriteBatch.Draw(t_icon2, new Vector2(x1 + x4, 429), new Rectangle(288, 96, 96, 48), Color.White);
                                break;
                            case ActType.Counter:
                                spriteBatch.Draw(t_icon2, new Vector2(x1 + x4, 429), new Rectangle(288, 144, 96, 48), Color.White);
                                break;
                            case ActType.BarrierDefense:
                                spriteBatch.Draw(t_icon2, new Vector2(x1 + x4, 429), new Rectangle(288, 192, 96, 48), Color.White);
                                break;
                            case ActType.BarrierSpirit:
                                spriteBatch.Draw(t_icon2, new Vector2(x1 + x4, 429), new Rectangle(288, 240, 96, 48), Color.White);
                                break;
                        }
                    }

                    for (int i = 0; i < 6; i++)
                    {
                        Act a = ug.unit.acts[i];
                        if (a != null)// && (!a.lastSpell || ug.unit.IsHaveAbility(Ability.Drive)))
                        {
                            spriteBatch.Draw(t_icon1, new Vector2(x2, 40 + i * 70), new Rectangle(GetIconFact(a) * 24, 48, 24, 24), Color.White);
                            Helper.DrawStringWithOutLine(a.name, new Vector2(x2 + 40, 35 + i * 70));
                            if (a.IsLimited)
                            {
                                Helper.DrawStringWithOutLine("/", new Vector2(x2 + 20, 65 + i * 70));
                                str = ug.actCount[i].ToString();
                                Helper.DrawStringWithOutLine(str, new Vector2(x2 + 20 - font.MeasureString(str).X, 65 + i * 70));
                                str = a.count.ToString();
                                Helper.DrawStringWithOutLine(str, new Vector2(x2 + 50 - font.MeasureString(str).X, 65 + i * 70));
                            }
                            Helper.DrawStringWithOutLine(Helper.GetStringActType(a), new Vector2(x2 + 70, 65 + i * 70));
                        }
                        else
                            spriteBatch.Draw(t_icon1, new Vector2(x2, 40 + i * 70), new Rectangle(5 * 24, 48, 24, 24), Color.White);
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
                x = 720 - 144 * 2;
                reverse = true;
            }
            spriteBatch.Draw(t_shadow, new Vector2(x + 96, 320), Color.White);
            if (actState != ActState.Move)
            {
                Vector2 pos = new Vector2(x + 144, 335);
                ug.unit.DrawBattle(spriteBatch, pos, Color.White, 0.98f + GetUnitFact(gameTime.TotalGameTime, 0.02f, 2000), reverse);
            }
            else
            {
                Vector2 pos = new Vector2(x + 144, 325 + GetUnitFact(gameTime.TotalGameTime, 10, 500));
                ug.unit.DrawBattle(spriteBatch, pos, Color.White, reverse);
            }
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
            if (actState < ActState.CrystalEffect)
            {
                if ((crystal_face.sympton > 0 && crystal_face.turn <= 0)
                    || Crystal.sympton == CrystalEffect.HPHeal
                    || Crystal.sympton == CrystalEffect.HPDamage
                    || Crystal.sympton == CrystalEffect.ChangeTerrain)
                {
                    timeSpan = 1000;
                    actState = ActState.CrystalEffect;
                    return;
                }
                if (Crystal.sympton == CrystalEffect.HPDamage || Crystal.sympton == CrystalEffect.HPHeal)
                {
                    foreach (UnitGadget ug in allUnitGadget)
                    {
                        if (!ug.dead && IsTakeCrystal(Crystal.sympton, ug))
                        {
                            timeSpan = 2000;
                            actState = ActState.CrystalEffect;
                            return;
                        }
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
                        actState = ActState.SymHeal;
                        return;
                    }
                }
            }
            if (actState < ActState.SymDamage)
            {
                foreach (UnitGadget ug in allUnitGadget)
                {
                    if (ug.symptonMinus.sympton == SymptonMinus.Damage && ug.HP > 1)
                    {
                        actState = ActState.SymDamage;
                        return;
                    }
                }
            }
            actState = ActState.SelectAct;
        }

        public static void MapSet(string[] data, CrystalEffect effect = CrystalEffect.None)
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
            crystal_base = new Condition<CrystalEffect>(effect, 0, 0);
            crystal_face = new Condition<CrystalEffect>(CrystalEffect.None, 0, 0);

            graphics.SetRenderTarget(r_map);
            spriteBatch.Begin();
            for (int i = 0; i < MapSize; i++)
                for (int j = 0; j < MapSize; j++)
                    spriteBatch.Draw(t_map, new Vector2(j * 48, i * 48), new Rectangle((int)map[j, i] * 48, 0, 48, 48), Color.White);
            spriteBatch.End();
            graphics.SetRenderTarget(null);
        }

        static int GetMapMoveGoal()
        {
            if (battleStart)
            {
                if (actState < ActState.Dis2 || actState >= ActState.Win)
                {
                    return 147;
                }
                else if (currentUnit == null)
                {
                    return drawMapOrigin.X;
                }
                else if (currentUnit.ff == FrendOfFoe.Ally)
                {
                    return 294;
                }
                else
                {
                    return 6;
                }
            }
            else
            {
                if (actState == ActState.Confront)
                {
                    return 147;
                }
                else if (actState == ActState.ListAlly || actState == ActState.ShowAlly)
                {
                    return 294;
                }
                else if (actState == ActState.BackBlackout)
                {
                    return drawMapOrigin.X;
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
                    Condition<SymptonMinus> con = currentUnit.symptonMinus;
                    if (map[i, j] == Terrain.Banned
                        || (con.sympton == SymptonMinus.FixInside && Positon.Distance(new Positon(i, j), con.doer.postion) > con.power)
                        || (con.sympton == SymptonMinus.FixOutside && Positon.Distance(new Positon(i, j), con.doer.postion) < con.power))
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
                    {
                        lp = GetZocRange(ug);
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
                    if (IsTakeCrystal(CrystalEffect.CostUp, currentUnit)) // 結晶効果：コスト増加
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
            r_bgMove = new RenderTarget2D(graphics, 384 * n, 480);
            p = unitCursor;
            graphics.SetRenderTarget(r_bgMove);
            spriteBatch.Begin();
            if (currentUnit.ff == FrendOfFoe.Ally)
            {
                spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(384 * --n, 0), Color.White);
                while (p != currentUnit.postion)
                {
                    spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(384 * --n, 0), Color.White);
                    p = mapCost[p.X, p.Y].parent;
                }
                spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(384 * --n, 0), Color.White);
                spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(384 * --n, 0), Color.White);
            }
            else
            {
                n = 0;
                spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(384 * n++, 0), Color.White);
                while (p != currentUnit.postion)
                {
                    spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(384 * n++, 0), Color.White);
                    p = mapCost[p.X, p.Y].parent;
                }
                spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(384 * n++, 0), Color.White);
                spriteBatch.Draw(t_bg[(int)GetTip(p)], new Vector2(384 * n++, 0), Color.White);
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

        static List<Positon> GetZocRange(UnitGadget unit)
        {
            List<Positon> lp = new List<Positon>();
            Positon p = unit.postion;

            for (int i = p.X - 1; i <= p.X + 1; i++)
            {
                if (i < 0 || i >= MapSize)
                    continue;
                for (int j = p.Y - 1; j <= p.Y + 1; j++)
                {
                    if (j < 0 || j >= MapSize)
                        continue;
                    if (mapCost[i, j].movable && (i == p.X || j == p.Y))
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
                if (a.IsHaveAbility(ActAbility.Heat)) // 低温特性補正
                    success = (int)(success * (1 + (1 - mag)));
                if (a.IsHaveAbility(ActAbility.Heat)) // 光学特性補正
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
                    foreach (Act act in currentUnit.unit.acts)
                        if (act != null && (act.type == ActType.Scope || act.type == ActType.DualBoost))
                            success += act.success;
                }
                if (currentUnit.IsType(Type.Technic)) // 技属性補正
                    success += currentUnit.SP / 4;
                success += currentUnit.upParameter.search; // 索敵補正
                // 神羅結界補正
                foreach (UnitGadget ug in allUnitGadget)
                {
                    if (ug.stance >= 0 && ug.StanceAct.type == ActType.BarrierSpirit && ug.ff == currentUnit.ff
                        && Positon.Distance(ug.postion, currentUnit.postion) >= ug.StanceAct.rangeMin
                        && Positon.Distance(ug.postion, currentUnit.postion) <= ug.StanceAct.rangeMax)
                        success += ug.StanceAct.power;
                }
            }
            if (IsTakeCrystal(CrystalEffect.HitUp, currentUnit)) // 結晶効果：成功アップ
                success += Crystal.power;
            if (currentUnit.symptonMinus.sympton == SymptonMinus.Distract) // マイナス症状：散漫
                success -= currentUnit.symptonMinus.power;

            hit = false;
            avoid = 0;
            if (a.TypeInt % 100 == 0 || a.type == ActType.AddMinusSympton)
            {
                int elevel = target.unit.level;
                elevel += (target.level - elevel) / 2;
                
                avoid = target.Parameter.avoid + elevel * 2;
                avoid += (int)GetAffinity(target, target.postion) * 5; // 地形相性補正
                
                if (a.TypeInt % 100 == 0)
                {
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
                        power = (int)(power * (1 + (1 - mag)));
                    else if (a.IsHaveAbility(ActAbility.Heat)) // 光学特性補正
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
                        foreach (Act act in currentUnit.unit.acts)
                            if (act != null && (act.type == ActType.Booster || act.type == ActType.DualBoost))
                                power += act.power;
                    }
                    if (currentUnit.IsType(Type.Power)) // 力属性補正
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
                        || (target.IsType(Type.Fortune) ? Helper.GetProbability(success - avoid, 125) : Helper.GetProbability(success - avoid)))
                        calcResult[i].hit = true;
                }
                else
                {
                    if (!target.IsMinusSymptonInvalid((SymptonMinus)a.sympton) && Helper.GetProbability(success - avoid / 2))
                        calcResult[i].hit = true;
                }

                // 防御判定
                if (!target.Deguard && Helper.GetProbability(defence, 50) && !covered)
                    calcResult[i].guard = true;

                // クリティカル判定
                mag = a.IsHaveAbility(ActAbility.Fast) ? 2 : 5;
                if (currentUnit.symptonPlus.sympton == SymptonPlus.Concentrate) // プラス症状：集中
                    mag *= 0.8f;
                if (a.IsHaveAbility(ActAbility.Proficient)) // 練達特性補正
                    mag *= 1 - (0.375f * MathHelper.Clamp(target.level - currentUnit.level, 0, 8) / 8);
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
                        if (target.IsType(Type.Guard)) // 護属性補正
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
                        if (IsTakeCrystal(CrystalEffect.DamageFix, currentUnit)) // 結晶効果：ダメージ固定
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
                    if (a.IsHaveAbility(ActAbility.Shock) || a.IsHaveAbility(ActAbility.Vacuum))
                    {
                        float force = 1f * damage / 20;
                        Positon p = target.postion - unitCursor;
                        Positon pp = new Positon(Math.Abs(p.X), Math.Abs(p.Y));
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
                        {
                            forceDir.X = -forceDir.X;
                            forceDir.Y = -forceDir.Y;
                        }
                        forcedPos = target.postion;
                        float d = Positon.Distance(forceDir, Positon.Zero);
                        while (force >= d)
                        {
                            Positon tmp = forcedPos + forceDir;
                            if (tmp.X < 0 || tmp.Y < 0 || tmp.X >= MapSize || tmp.Y >= MapSize || GetTip(tmp) == Terrain.Banned)
                                break;
                            bool impossible = false;
                            foreach (UnitGadget ug in allUnitGadget)
                                if (ug != target && ug.postion == tmp)
                                    impossible = true;
                            if (impossible)
                                break;
                            forcedPos = tmp;
                            force -= d;
                        }
                    }

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
                        if (currentUnit.IsType(Type.Apparition))
                        {
                            calcResult[i].sympPower = (int)(calcResult[i].sympPower * 1.5);
                            calcResult[i].sympTurn = (int)(calcResult[i].sympTurn * 1.5);
                        }
                        if (target.IsType(Type.Apparition))
                        {
                            calcResult[i].sympPower = (int)Math.Ceiling(calcResult[i].sympPower * 0.75);
                            calcResult[i].sympTurn = (int)Math.Ceiling(calcResult[i].sympTurn * 0.75);
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
                if (a.type == ActType.SetCrystal && crystal_face.sympton > 0 && calcResult[0].sympTurn < crystal_face.turn)
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
                                tmp = success / 48;
                                break;
                        }
                        break;
                    case ActType.SetCrystal:
                        switch ((CrystalEffect)a.sympton)
                        {
                            case CrystalEffect.HPHeal:
                            case CrystalEffect.HPDamage:
                                tmp = success / 16;
                                break;
                            case CrystalEffect.APUp:
                            case CrystalEffect.APDown:
                                tmp = success / 48;
                                break;
                            case CrystalEffect.HitUp:
                                tmp = success / 5;
                                break;
                            case CrystalEffect.CostUp:
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
                if (currentAct.type == ActType.SPUp)
                    timeSpan = 3000;
                else if (currentAct.type == ActType.ClearMinusSympton || currentAct.type == ActType.ClearPlusSympton)
                    timeSpan = 1500;
                else if (currentAct.sympton == (int)SymptonPlus.Charge || currentAct.sympton == (int)SymptonPlus.ActAgain)
                    timeSpan = 2500;
                else if (currentAct.sympton == (int)SymptonPlus.Heal || currentAct.type == ActType.ClearTrap || currentAct.type == ActType.Revive)
                    timeSpan = 1000;
                else
                    timeSpan = 2000;
                actState = ActState.AddSymp;
            }
        }

        static void TurnEnd()
        {
            foreach(UnitGadget ug in allUnitGadget)
            {
                Condition<SymptonMinus> con = ug.symptonMinus;
                if ((con.sympton == SymptonMinus.FixInside && Positon.Distance(ug.postion, con.doer.postion) > con.power)
                    || (con.sympton == SymptonMinus.FixOutside && Positon.Distance(ug.postion, con.doer.postion) < con.power))
                    ug.symptonMinus.sympton = SymptonMinus.None;
            }

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
                if ((currentUnit.unit.IsHaveAbility(Ability.ActAgain) && actTimes == 0) ||
                    (currentUnit.symptonPlus.sympton == SymptonPlus.ActAgain &&
                    (actTimes == 0 || (currentUnit.unit.IsHaveAbility(Ability.ActAgain) && actTimes == 1))))
                {
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
            Vector2 v = drawMapOrigin + new Vector2(pos.X * 48 + 21, pos.Y * 48 + 27);
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
                spriteBatch.Draw(t_icon1, vv, new Rectangle(168, 120, 24, 24), new Color(a, a, a, a), 0, new Vector2(12), ss, SpriteEffects.None, 0);
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
            if (IsTakeCrystal(CrystalEffect.AffinityDown, ug)) // 結晶効果：適正ダウン
            {
                int af = (int)affinity;
                af -= Crystal.power;
                if (af < 0)
                    af = 0;
                affinity = (Affinity)af;
            }
            else if (IsTakeCrystal(CrystalEffect.AffinityReverse, ug)) // 結晶効果：適正反転
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

        static bool IsTakeCrystal(CrystalEffect effect, UnitGadget ug)
        {
            return Crystal.sympton == effect && !ug.IsCrystalEffectInvalid(effect);
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

        static void AddCrystalErosion(UnitGadget ug, int fact)
        {
            ug.crystalErosion += fact;
            if (ug.crystalErosion > 100)
                ug.crystalErosion = 100;
            else if (ug.crystalErosion < 0)
                ug.crystalErosion = 0;
        }

        static Act currentAct
        {
            get { return selectedAct > 0? currentUnit.unit.acts[selectedAct - 1]: null; }
        }

        static int currentActCount
        {
            get { return selectedAct > 0 ? currentUnit.actCount[selectedAct - 1] : 0; }
        }

        static bool battleStart
        {
            get { return actState >= ActState.TurnStart; }
        }

        static Condition<CrystalEffect> Crystal
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
