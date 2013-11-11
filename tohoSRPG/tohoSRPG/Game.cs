using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
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
            using (Game game = new Game())
            {
                game.Run();
            }
        }
    }

    /// <summary>
    /// 基底 Game クラスから派生した、ゲームのメイン クラスです。
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
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

        public enum Scene
        {
            Title, World, Talk, Battle, Intermission
        }
        public Scene currentScene;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 480;
        }

        protected override void Initialize()
        {
            base.Window.Title = "tohoSRPG " + version;
            InputManager.Init();
            rand = new Random();

            currentScene = Scene.Battle;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            BattleScene.Init(GraphicsDevice, spriteBatch, Content);
            font = Content.Load<SpriteFont>("font\\CommonFont");

            Helper.Init(spriteBatch, font);

            UnitSetting.Init(Content);

            for (int i = 0; i < 5; i++ )
                BattleScene.allyUnit.Add(UnitSetting.SetUnit((CharaID)i, 30));

            BattleSetting.Init(Content);
            BattleSetting.BattleSet001();
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (StorageRequested == request.non && IsActive)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    this.Exit();

                // 入力の更新
                InputManager.Update(Keyboard.GetState(), GamePad.GetState(PlayerIndex.One));

                switch (currentScene)
                {
                    case Scene.Title:
                        break;
                    case Scene.World:
                        break;
                    case Scene.Talk:
                        break;
                    case Scene.Battle:
                        BattleScene.Update(gameTime.ElapsedGameTime);
                        break;
                    case Scene.Intermission:
                        break;
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (StorageRequested == request.non)
            {
                switch (currentScene)
                {
                    case Scene.Title:
                        break;
                    case Scene.World:
                        break;
                    case Scene.Talk:
                        break;
                    case Scene.Battle:
                        BattleScene.Draw(gameTime);
                        break;
                    case Scene.Intermission:
                        break;
                }
            }

            base.Draw(gameTime);
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
