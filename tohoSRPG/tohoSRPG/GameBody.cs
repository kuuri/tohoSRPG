using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;

namespace tohoSRPG
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリー ポイントです。
        /// </summary>
        static void Main(string[] args)
        {
            using (GameBody game = new GameBody())
            {
                game.Run();
            }
        }
    }

    /// <summary>
    /// 基底 Game クラスから派生した、ゲームのメイン クラスです。
    /// </summary>
    public class GameBody : Microsoft.Xna.Framework.Game
    {
        string version = "ver 0.1";
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;

        public static Random rand;

        SaveData save;
        enum request { non, save, load }
        IAsyncResult result;
        request StorageRequested = request.non;
        int selectedStorage = 0;

        Scene currentScene;
        static Scene nextScene;
        float alpha;

        public GameBody()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 480;

            //Form MyForm = (Form)Form.FromHandle(this.Window.Handle);
            //MyForm.MaximizeBox = false;
            //MyForm.MinimizeBox = false;
        }

        protected override void Initialize()
        {
            base.Window.Title = "tohoSRPG " + version;
            InputManager.Init();
            rand = new Random();

            currentScene = Scene.World;
            nextScene = Scene.None;
            alpha = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            WorldScene.Init(GraphicsDevice, spriteBatch, Content);
            MenuScene.Init(GraphicsDevice, spriteBatch, Content);
            ConfrontScene.Init(GraphicsDevice, spriteBatch, Content);
            BattleScene.Init(GraphicsDevice, spriteBatch, Content);
            font = Content.Load<SpriteFont>("font\\CommonFont");

            Helper.Init(spriteBatch, font);

            UnitSetting.Init(Content);

            WorldScene.AddNode(new EventNode("博麗神社", new Vector2(300, 400), 0));
            WorldScene.AddNode(new EventNode("紅魔館", new Vector2(200, 100), 0));

            BattleScene.allyUnit.Add(UnitSetting.SetUnit(CharaID.Reimu, 60));
            BattleScene.allyUnit.Add(UnitSetting.SetUnit(CharaID.Marisa, 30));
            BattleScene.allyUnit.Add(UnitSetting.SetUnit(CharaID.Sanae, 30));
            BattleScene.allyUnit.Add(UnitSetting.SetUnit(CharaID.Cirno, 30));
            BattleScene.allyUnit.Add(UnitSetting.SetUnit(CharaID.Meirin, 30));
            BattleScene.allyUnit.Add(UnitSetting.SetUnit(CharaID.Flandre, 30));
            BattleScene.allyUnit.Add(UnitSetting.SetUnit(CharaID.Udonge, 30));
            BattleScene.allyUnit.Add(UnitSetting.SetUnit(CharaID.Aya, 30));
            BattleScene.allyUnit.Add(UnitSetting.SetUnit(CharaID.Rin, 30));
            BattleScene.allyUnit.Add(UnitSetting.SetUnit(CharaID.Oku, 30));
            BattleScene.allyUnit.Add(UnitSetting.SetUnit(CharaID.Kokoro, 30));
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (StorageRequested == request.non && IsActive)
            {
                if (Keyboard.GetState().IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                    this.Exit();

                // 入力の更新
                InputManager.Update(Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));

                // シーン切り替え
                if (nextScene != Scene.None)
                {
                    alpha += (float)gameTime.ElapsedGameTime.TotalMilliseconds / 500;
                    if (alpha >= 1)
                    {
                        alpha = 1;
                        currentScene = nextScene;
                        nextScene = Scene.None;
                    }
                }
                else if (alpha > 0)
                {
                    alpha -= (float)gameTime.ElapsedGameTime.TotalMilliseconds / 500;
                    if (alpha < 0)
                        alpha = 0;
                }
                else
                {
                    switch (currentScene)
                    {
                        case Scene.Title:
                            break;
                        case Scene.World:
                            WorldScene.Update(gameTime);
                            break;
                        case Scene.Talk:
                            break;
                        case Scene.Confront:
                            ConfrontScene.Update(gameTime);
                            break;
                        case Scene.Battle:
                            BattleScene.Update(gameTime);
                            break;
                        case Scene.Menu:
                            MenuScene.Update(gameTime);
                            break;
                    }
                }
            }
            else
            {
                if ((StorageRequested == request.load) && (result.IsCompleted))
                {
                    StorageDevice device = StorageDevice.EndShowSelector(result);
                    if (device != null && device.IsConnected)
                    {
                        LoadFromStorage(device);
                    }
                    StorageRequested = request.non;
                }
                else if ((StorageRequested == request.save) && (result.IsCompleted))
                {
                    StorageDevice device = StorageDevice.EndShowSelector(result);
                    if (device != null && device.IsConnected)
                    {
                        SaveToStorage(device);
                    }
                    StorageRequested = request.non;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Texture2D tw = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
            Color[] c = { Color.White };
            tw.SetData(c);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (StorageRequested == request.non)
            {
                switch (currentScene)
                {
                    case Scene.Title:
                        break;
                    case Scene.World:
                        WorldScene.Draw(gameTime);
                        break;
                    case Scene.Talk:
                        break;
                    case Scene.Confront:
                        ConfrontScene.Draw(gameTime);
                        break;
                    case Scene.Battle:
                        BattleScene.Draw(gameTime);
                        break;
                    case Scene.Menu:
                        MenuScene.Draw(gameTime);
                        break;
                }

                spriteBatch.Begin();
                spriteBatch.Draw(tw, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight),
                    new Color(0, 0, 0, alpha));
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public static void ChangeScene(Scene scene)
        {
            nextScene = scene;
        }

        #region セーブデータ管理
        private void StorageSet(request order, int storage)
        {
            // Set the request flag
            if (!Guide.IsVisible && StorageRequested == request.non)
            {
                StorageRequested = order;
                selectedStorage = storage;
                result = StorageDevice.BeginShowSelector(
                        PlayerIndex.One, null, null);
            }
        }

        private void SaveToStorage(StorageDevice device)
        {
            IAsyncResult result =
                device.BeginOpenContainer("SaveData", null, null);

            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            result.AsyncWaitHandle.Close();

            string filename = "Save" + selectedStorage.ToString("D3") + ".sav";

            Stream file;
            if (container.FileExists(filename))
                container.DeleteFile(filename);
            file = container.CreateFile(filename);

            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
            serializer.Serialize(file, save);

            file.Close();
            container.Dispose();
        }

        private void LoadFromStorage(StorageDevice device)
        {
            IAsyncResult result =
                device.BeginOpenContainer("SaveData", null, null);

            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            result.AsyncWaitHandle.Close();

            string filename = "Save" + selectedStorage.ToString("D3") + ".sav";
            if (!container.FileExists(filename))
                return;

            Stream file = container.OpenFile(filename, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(SaveData));

            save = (SaveData)serializer.Deserialize(file);

            file.Close();
            container.Dispose();
        }

        #endregion
    }

    struct SaveData
    {
        int a;
    }
}
