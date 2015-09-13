using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ParallaxEngine;
using ProduceWars.Managers;

namespace ProduceWars
{
    class MainOptionScreen : MenuScreen
    {
        Texture2D pixel, controls, help;
        Color backColor, barColor;
        bool isGamePause = false;
        int pauseOffset = 30;  //set to 0 after beta when erasing the menu entries for reset save and locks on/off

        MenuEntry Music, Sound, ShotPath, HelpGraphic;

        //beta
        MenuEntry resetSaveMenuEntry, locksMenuEntry;

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainOptionScreen(bool _isGamePause)
            : base("Game Paused")
        {
            isGamePause = _isGamePause;
            if (isGamePause) pauseOffset = 60;

            // Create our menu entries.
            Music = new MenuEntry(string.Empty);
            Sound = new MenuEntry(string.Empty);
            ShotPath = new MenuEntry(string.Empty);
            HelpGraphic = new MenuEntry(string.Empty);

            // Hook up menu event handlers.
            Music.Plus += new EventHandler<PlayerIndexEventArgs>(Music_Plus);
            Music.Minus += new EventHandler<PlayerIndexEventArgs>(Music_Minus);
            Sound.Plus += new EventHandler<PlayerIndexEventArgs>(Sound_Plus);
            Sound.Minus += new EventHandler<PlayerIndexEventArgs>(Sound_Minus);
            ShotPath.Plus += new EventHandler<PlayerIndexEventArgs>(ShotPath_Plus);
            ShotPath.Minus += new EventHandler<PlayerIndexEventArgs>(ShotPath_Minus);
            HelpGraphic.Plus += new EventHandler<PlayerIndexEventArgs>(HelpGraphic_Plus);
            HelpGraphic.Minus += new EventHandler<PlayerIndexEventArgs>(HelpGraphic_Minus);
            HelpGraphic.Selected += new EventHandler<PlayerIndexEventArgs>(HelpGraphic_Selected);
     
            // Add entries to the menu.
            MenuEntries.Add(Music);
            MenuEntries.Add(Sound);
            MenuEntries.Add(ShotPath);
            MenuEntries.Add(HelpGraphic);


            if (!isGamePause && GameSettings.isTestingMode)
            {
                resetSaveMenuEntry = new MenuEntry("Reset Save Data (Beta Testing)"); //beat
                locksMenuEntry = new MenuEntry("Level/Fruit Locks (Beta Testing): "); //beta
                resetSaveMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(resetSaveMenuEntry_Selected); //beta
                locksMenuEntry.Selected += new System.EventHandler<PlayerIndexEventArgs>(locksMenuEntry_Selected); //beta
                MenuEntries.Add(resetSaveMenuEntry); //beta
                MenuEntries.Add(locksMenuEntry);  //beta
            }


            SetMenuEntryText();
        }



        public override void LoadContent(ContentManager _content)
        {
            LevelDataManager.UItextures.TryGetValue("MainOption", out menuTitleGraphic);
            isMenuTitleGraphic = true;
 
            //get other textures
            LevelDataManager.UItextures.TryGetValue("WPixel", out pixel);
            LevelDataManager.UItextures.TryGetValue("Controls", out controls);

            base.LoadContent(_content);
        }

        void SetMenuEntryText()
        {
            SoundManager.SetVolume(1f);
            Music.Text = "Music Track Volume : " + GameSettings.MusicVolume.ToString();
            Sound.Text = "Sound Effect Volume : " + GameSettings.EffectVolume.ToString();
            ShotPath.Text = "Targeting Color : " + GameSettings.ColorName[GameSettings.ShotColorIndex];
            HelpGraphic.Text = "Show Help : " + GameSettings.HelpGraphicName[GameSettings.HelpGraphicIndex];

            if(!isGamePause && GameSettings.isTestingMode) locksMenuEntry.Text = "Level/Fruit Locks (Beta Testing): " + (GameSettings.LocksOn ? " On" : " Off"); //beta
        }


        //beta
        void locksMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.LocksOn = !GameSettings.LocksOn;
            locksMenuEntry.Text = "Level/Fruit Locks (Beta Testing): " + (GameSettings.LocksOn ? " On" : " Off");
        }

        //beta
        void resetSaveMenuEntry_Selected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Beta Test Feature:  Reset save data?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        //beta for reset save
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            for (int i = 0; i < 7; i++)
            {
                for (int j = 1; j < 16; j++)
                {
                    LevelDataManager.levelData[i, j].unlocked = false;
                    LevelDataManager.levelData[i, j].starCollected = false;
                    LevelDataManager.levelData[i, j].shots = 0;
                    LevelDataManager.levelData[i, j].bestScore = 0;
                }
            }

            LevelDataManager.LockSetup();
            LevelDataManager.WriteSaveGameData();
        }


        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);

            if (!isGamePause) return;  //only check input (past this point) if we are on pause screen and not main menu

            if (input.IsAPressed(ControllingPlayer, out playerIndex) || input.IsBPressed(ControllingPlayer, out playerIndex) || input.IsPauseGame(ControllingPlayer, out playerIndex) )
            {
                saveOptions();
                ExitScreen();
            }

            if (input.IsYPressed(ControllingPlayer, out playerIndex))
            {
                saveOptions();
                LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new GameplayScreen(LevelDataManager.world, LevelDataManager.level));
                ExitScreen();
            }

            if (input.IsXPressed(ControllingPlayer, out playerIndex))
            {
                saveOptions();
                LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen(), new LevelSelectScreen());
                ExitScreen();
            }
            return;
        }

        void ShotPath_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.ShotColorIndex -= 1;
            if (GameSettings.ShotColorIndex < 0) GameSettings.ShotColorIndex = GameSettings.Colors.Count() - 1;
            GameSettings.ShotPathColor = GameSettings.Colors[GameSettings.ShotColorIndex];
            SetMenuEntryText();
        }

        void ShotPath_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.ShotColorIndex += 1;
            if (GameSettings.ShotColorIndex >= GameSettings.Colors.Count()) GameSettings.ShotColorIndex = 0;
            GameSettings.ShotPathColor = GameSettings.Colors[GameSettings.ShotColorIndex];
            SetMenuEntryText();
        }

        void Sound_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.EffectVolume -= 5;
            SetMenuEntryText();
        }

        void Sound_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.EffectVolume += 5;
            SetMenuEntryText();
        }

        void Music_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MusicVolume -= 5;
            SetMenuEntryText();
        }

        void Music_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.MusicVolume += 5;
            SetMenuEntryText();
        }

        void HelpGraphic_Plus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.HelpGraphicIndex +=1;
            if (GameSettings.HelpGraphicIndex > 6) GameSettings.HelpGraphicIndex = 0;
            SetMenuEntryText();
        }

        void HelpGraphic_Minus(object sender, PlayerIndexEventArgs e)
        {
            GameSettings.HelpGraphicIndex -= 1;
            if (GameSettings.HelpGraphicIndex < 0) GameSettings.HelpGraphicIndex = 6;
            SetMenuEntryText();
        }

        void HelpGraphic_Selected(object sender, PlayerIndexEventArgs e)
        {
            switch (GameSettings.HelpGraphicIndex)
            {
                case 0:
                    {
                        LevelDataManager.UItextures.TryGetValue("TutApple", out help);
                        break;
                    }
                case 1:
                    {
                        LevelDataManager.UItextures.TryGetValue("TutOrange", out help);
                        break;
                    }
                case 2:
                    {
                        LevelDataManager.UItextures.TryGetValue("TutStraw", out help);
                        break;
                    }
                case 3:
                    {
                        LevelDataManager.UItextures.TryGetValue("TutCherries", out help);
                        break;
                    }
                case 4:
                    {
                        LevelDataManager.UItextures.TryGetValue("TutBanana", out help);
                        break;
                    }
                case 5:
                    {
                        LevelDataManager.UItextures.TryGetValue("TutLemon", out help);
                        break;
                    }
                case 6:
                    {
                        LevelDataManager.UItextures.TryGetValue("TutWater", out help);
                        break;
                    }
                default:
                    {
                        help = controls;
                        break;
                    }
            }

            ScreenManager.AddScreen(new PopUpGraphicScreen(help,Vector2.Zero,PopUpGraphicScreen.TransitionType.Fade, TimeSpan.FromSeconds(0.5)),ControllingPlayer);
        }

        protected override void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            base.OnSelectEntry(entryIndex, playerIndex);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            saveOptions();
            base.OnCancel(playerIndex);
        } 


        private void saveOptions()
        {
            // make sure the device is ready
            if (LevelDataManager.SaveDevice.IsReady)
            {
                try
                {
                    // save a file asynchronously. this will trigger IsBusy to return true
                    // for the duration of the save process.
                    LevelDataManager.SaveDevice.SaveAsync(
                        LevelDataManager.containerName,
                        LevelDataManager.fileName_options,
                        stream =>
                        {
                            using (StreamWriter writer = new StreamWriter(stream))
                            {
                                writer.WriteLine(GameSettings.MusicOn);
                                writer.WriteLine(GameSettings.MusicVolume);
                                writer.WriteLine(GameSettings.EffectVolume);
                                writer.WriteLine(GameSettings.ShotColorIndex);
                            }
                        });
                }
                catch
                {
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            int halfW = Camera.Viewport.Width / 2;
            backColor = Color.White * TransitionAlpha;
            barColor = new Color(0, 0, 0, 128) * TransitionAlpha;
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(pixel, new Rectangle(halfW - 310, 0, 620, Camera.ViewportHeight), barColor);
            ScreenManager.SpriteBatch.Draw(controls, new Rectangle((int)(Camera.ViewportWidth/2 - controls.Width*0.375f), 200+pauseOffset, (int)(controls.Width*0.75f),(int)(controls.Height*0.75f)), backColor);

            if (isGamePause)
            {
                ScreenManager.SpriteBatch.Draw(A, new Rectangle((int)halfW - 284, 238, 48, 48), backColor);
                ScreenManager.SpriteBatch.Draw(B, new Rectangle((int)halfW - 242, 238, 48, 48), backColor);
                ScreenManager.SpriteBatch.Draw(X, new Rectangle((int)halfW - 56, 238, 48, 48), backColor);
                ScreenManager.SpriteBatch.Draw(Y, new Rectangle((int)halfW + 96, 238, 48, 48), backColor);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, "Resume", new Vector2(halfW - 192, 246), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, "Menu", new Vector2(halfW -6, 246), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, "Restart", new Vector2(halfW + 146, 246), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);
            }

            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

    }
}