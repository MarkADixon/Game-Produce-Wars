using System;
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
    class SoundMenuScreen : MenuScreen
    {
        Texture2D pixel,waterflame;
        Vector2 waterflamePos = Vector2.Zero; //set in Load content
        Color backColor, barColor;
        bool trackGrass1, trackGrass2, trackForest1, trackForest2, trackDesert1, trackDesert2, trackSnow1, trackSnow2, trackFactory1, trackFactory2, trackBoss = false;
        MenuEntry Title, Training, Grass1, Grass2, Forest1, Forest2, Desert1, Desert2, Snow1, Snow2, Factory1, Factory2, Boss, Credits;
        string unknown = "? ? ? ? ? ? ? ?";
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public SoundMenuScreen()
            : base("Sound Test Menu")
        {
            // Create our menu entries.
            Title = new MenuEntry("Super Swing-Bit Orchestra (Menu Loop)");
            Credits = new MenuEntry("Super Swing-Bit Orchestra (Extended)");
            Training = new MenuEntry("Clouds");
            Grass1 = new MenuEntry("Happyland");
            Grass2 = new MenuEntry("Cubic Adventures");
            Forest1 = new MenuEntry("Summer Rave");
            Forest2 = new MenuEntry("Summertime");
            Desert1 = new MenuEntry("Boss Hunter");
            Desert2 = new MenuEntry("Haunted Woods");
            Snow1 = new MenuEntry("Flight");
            Snow2 = new MenuEntry("Conclusion");
            Factory1 = new MenuEntry("Killstealer");
            Factory2 = new MenuEntry("Bassrift");
            Boss = new MenuEntry("Give Me A Break");
            

            // Hook up menu event handlers.
         
            // Add entries to the menu.
            MenuEntries.Add(Title);
            MenuEntries.Add(Credits);
            MenuEntries.Add(Training);
            MenuEntries.Add(Grass1);
            MenuEntries.Add(Grass2);
            MenuEntries.Add(Forest1);
            MenuEntries.Add(Forest2);
            MenuEntries.Add(Desert1);
            MenuEntries.Add(Desert2);
            MenuEntries.Add(Snow1);
            MenuEntries.Add(Snow2);
            MenuEntries.Add(Factory1);
            MenuEntries.Add(Factory2);
            MenuEntries.Add(Boss);

        }

        public override void LoadContent(ContentManager _content)
        {

            LevelDataManager.UItextures.TryGetValue("MainSound", out menuTitleGraphic);
            isMenuTitleGraphic = true;

            //get other textures
            LevelDataManager.UItextures.TryGetValue("Pixel", out pixel);
            LevelDataManager.UItextures.TryGetValue("Waterflame", out waterflame);
            waterflamePos = new Vector2((Camera.Viewport.Width / 2) - (waterflame.Width / 2), Camera.Viewport.Height - waterflame.Height);

            if (LevelDataManager.levelData[1, 6].unlocked || !GameSettings.LocksOn) trackGrass1 = true;
            else Grass1.Text = unknown;
            if (LevelDataManager.levelData[1, 11].unlocked || !GameSettings.LocksOn) trackGrass2 = true;
            else Grass2.Text = unknown;
            if (LevelDataManager.levelData[2, 6].unlocked || !GameSettings.LocksOn) trackForest1 = true;
            else Forest1.Text = unknown;
            if (LevelDataManager.levelData[2, 11].unlocked || !GameSettings.LocksOn) trackForest2 = true;
            else Forest2.Text = unknown;
            if (LevelDataManager.levelData[3, 6].unlocked || !GameSettings.LocksOn) trackDesert1 = true;
            else Desert1.Text = unknown;
            if (LevelDataManager.levelData[3, 11].unlocked || !GameSettings.LocksOn) trackDesert2 = true;
            else Desert2.Text = unknown;
            if (LevelDataManager.levelData[4, 6].unlocked || !GameSettings.LocksOn) trackSnow1 = true;
            else Snow1.Text = unknown;
            if (LevelDataManager.levelData[4, 11].unlocked || !GameSettings.LocksOn) trackSnow2 = true;
            else Snow2.Text = unknown;
            if (LevelDataManager.levelData[5, 1].unlocked || !GameSettings.LocksOn) trackFactory1 = true;
            else Factory1.Text = unknown;
            if (LevelDataManager.levelData[5, 6].unlocked || !GameSettings.LocksOn) trackFactory2 = true;
            else Factory2.Text = unknown;
            if (LevelDataManager.levelData[5, 11].unlocked || !GameSettings.LocksOn) trackBoss = true;
            else Boss.Text = unknown;

            base.LoadContent(_content);
        }

        public override void HandleInput(InputState input)
        {
            if (input.IsXPressed(ControllingPlayer, out playerIndex))
            {
                SoundManager.StopMusic();
            }   
            base.HandleInput(input);
        }

        protected override void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            switch (entryIndex)
            {
                case 3:
                    {
                        if (!trackGrass1) return;
                        break;
                    }
                case 4:
                    {
                        if (!trackGrass2) return;
                        break;
                    }
                case 5:
                    {
                        if (!trackForest1) return;
                        break;
                    }
                case 6:
                    {
                        if (!trackForest2) return;
                        break;
                    }
                case 7:
                    {
                        if (!trackDesert1) return;
                        break;
                    }
                case 8:
                    {
                        if (!trackDesert2) return;
                        break;
                    }
                case 9:
                    {
                        if (!trackSnow1) return;
                        break;
                    }
                case 10:
                    {
                        if (!trackSnow2) return;
                        break;
                    }
                case 11:
                    {
                        if (!trackFactory1) return;
                        break;
                    }
                case 12:
                    {
                        if (!trackFactory2) return;
                        break;
                    }
                case 13:
                    {
                        if (!trackBoss) return;
                        break;
                    }
                default:
                    break;
            }

            SoundManager.MusicSoundTest(entryIndex);
            base.OnSelectEntry(entryIndex, playerIndex);
        }

        public override void Draw(GameTime gameTime)
        {
            int halfW = Camera.Viewport.Width / 2;
            backColor = Color.White * TransitionAlpha;
            barColor = new Color(0, 0, 0, 128) * TransitionAlpha;
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(pixel, new Rectangle(halfW - 280, 0, 560, Camera.ViewportHeight), barColor);
            ScreenManager.SpriteBatch.Draw(waterflame, waterflamePos + new Vector2(0,-30), backColor);
            ScreenManager.SpriteBatch.Draw(A, new Rectangle((int)halfW - 200, (int)waterflamePos.Y-8, 48, 48), backColor);
            ScreenManager.SpriteBatch.Draw(X, new Rectangle((int)halfW -50, (int)waterflamePos.Y-8, 48, 48), backColor);
            ScreenManager.SpriteBatch.Draw(B, new Rectangle((int)halfW + 100, (int)waterflamePos.Y-8, 48, 48), backColor);

            DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, "PLAY", new Vector2(halfW - 150, waterflamePos.Y), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);
            DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, "STOP", new Vector2(halfW, waterflamePos.Y), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);
            DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, "BACK", new Vector2(halfW + 150, waterflamePos.Y), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);
            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            SoundManager.StopMusic();
            SoundManager.LoadMusic(LevelDataManager.levelContent, 100);
            SoundManager.LoadMusic(LevelDataManager.levelContent, -1);
            base.OnCancel(playerIndex);
        }

    }
}
