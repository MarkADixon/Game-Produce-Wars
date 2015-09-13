using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using ParallaxEngine;

namespace ProduceWars
{
    class LevelSelectScreen : MenuScreen
    {
        Texture2D levelNoStar, levelWithStar, numbers, levelLock, levelStar, pixel;
        Texture2D bronze, silver, gold;
        Vector2 backgroundPosition = Vector2.Zero;
        int displayStars = 0;
        int displayBronze = 0;
        int displaySilver = 0;
        int displayGold = 0;
        string lockedLevel = "?????";
        string levelNameDisplay = "";
        int numberOfLevels = 15;
        string divisor = "/15";
        int earnedStars = 0;
        float medalFrame = 0f;

        #region CONSTRUCTOR

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public LevelSelectScreen()
            : base("Level Select", MenuType.LevelGrid)
        {
        }
        #endregion

        public override void LoadContent(ContentManager _content)
        {
            switch (LevelDataManager.nextworld)
            {
                case 0:
                    {
                        LevelDataManager.UItextures.TryGetValue("World0", out menuTitleGraphic);
                        numberOfLevels = 10;
                        divisor = "/10";
                        break;
                    }
                case 1:
                    {
                        LevelDataManager.UItextures.TryGetValue("World1", out menuTitleGraphic);
                        break;
                    }
                case 2:
                    {
                        LevelDataManager.UItextures.TryGetValue("World2", out menuTitleGraphic);
                        break;
                    }
                case 3:
                    {
                        LevelDataManager.UItextures.TryGetValue("World3", out menuTitleGraphic);
                        break;
                    }
                case 4:
                    {
                        LevelDataManager.UItextures.TryGetValue("World4", out menuTitleGraphic);
                        break;
                    }
                case 5:
                    {
                        LevelDataManager.UItextures.TryGetValue("World5", out menuTitleGraphic);
                        break;
                    }
                case 6:
                    {
                        LevelDataManager.UItextures.TryGetValue("World6", out menuTitleGraphic);
                        break;
                    }
                default:
                    break;
            }
            isMenuTitleGraphic = true;
            LevelDataManager.UItextures.TryGetValue("MenuBack", out menuBackground);

            //create textured menu entries
            LevelDataManager.UItextures.TryGetValue("LevelNoStar", out levelNoStar);
            LevelDataManager.UItextures.TryGetValue("LevelWithStar", out levelWithStar);
            LevelDataManager.UItextures.TryGetValue("Numbers", out numbers);
            LevelDataManager.UItextures.TryGetValue("NumberLock", out levelLock);
            LevelDataManager.UItextures.TryGetValue("Star", out levelStar);
            LevelDataManager.UItextures.TryGetValue("Pixel", out pixel);
            LevelDataManager.UItextures.TryGetValue("MedalBronze", out bronze);
            LevelDataManager.UItextures.TryGetValue("MedalSilver", out silver);
            LevelDataManager.UItextures.TryGetValue("MedalGold", out gold);
  
            for (int i = 0; i < numberOfLevels; i++)
            {
               // MenuEntry LevelMenuEntry = new MenuEntry(levelNoStar,levelWithStar, numbers, i, levelLock, levelStar, levelBronze,levelSilver,levelGold);
                MenuEntry LevelMenuEntry = new MenuEntry(levelNoStar,levelWithStar, numbers, i, levelLock, levelStar, bronze,silver,gold);
                LevelMenuEntry.Selected += LevelMenuEntrySelected;
                if (LevelMenuEntry.isStarCollected) displayStars += 1;
                if (LevelMenuEntry.isBronze) displayBronze += 1;
                if (LevelMenuEntry.isSilver) displaySilver += 1;
                if (LevelMenuEntry.isGold) displayGold += 1;
                MenuEntries.Add(LevelMenuEntry);
            }

            earnedStars = LevelDataManager.EarnedStars();
            base.LoadContent(_content);
        }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();

            medalFrame += 12 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (medalFrame >= 10f) medalFrame -= 10f;

            //set background square position
            if (menuType == MenuType.LevelGrid && menuBackground != null)
            {
                //centered horizontally
                backgroundPosition = new Vector2((ScreenManager.GraphicsDevice.Viewport.Width / 2f) - (menuBackground.Width / 2f), 10);

                if (ScreenState == ScreenState.TransitionOn)
                    backgroundPosition.X += transitionOffset * 512;
                else
                    backgroundPosition.X -= transitionOffset * 512;

                Color backColor = Color.White * TransitionAlpha;
                Color barColor = new Color(0, 0, 0, 128) * TransitionAlpha;

                //if the level is locked display ????? instead of the level name
                if (LevelDataManager.levelData[LevelDataManager.nextworld, this.selectedEntry + 1].unlocked || !GameSettings.LocksOn) levelNameDisplay = LevelDataManager.levelData[LevelDataManager.nextworld, this.selectedEntry + 1].name;
                else if (LevelDataManager.nextworld == 6) levelNameDisplay = "Collect " + ((selectedEntry+1)*5).ToString() + " stars to unlock. You have " + earnedStars.ToString() + " stars.";
                else levelNameDisplay = lockedLevel;

                //draw the background
                ScreenManager.SpriteBatch.Draw(menuBackground, new Rectangle((int)backgroundPosition.X, (int)backgroundPosition.Y+56, menuBackground.Width, menuBackground.Height), backColor);
                ScreenManager.SpriteBatch.Draw(LT, new Rectangle((int)backgroundPosition.X + 216, (int)backgroundPosition.Y + 66, LT.Width, LT.Height), backColor);
                ScreenManager.SpriteBatch.Draw(RT, new Rectangle((int)backgroundPosition.X + 624, (int)backgroundPosition.Y + 66, RT.Width, RT.Height), backColor);
                ScreenManager.SpriteBatch.Draw(pixel, new Rectangle((int)backgroundPosition.X + 85, (int)backgroundPosition.Y + 142, 730, 26), barColor);
                ScreenManager.SpriteBatch.Draw(pixel ,new Rectangle((int)backgroundPosition.X + 85, (int)backgroundPosition.Y + 608, 730, 26), barColor); 
                ScreenManager.SpriteBatch.Draw(bronze, new Rectangle((int)backgroundPosition.X + 80, (int)backgroundPosition.Y + 602, 40, 40), new Rectangle (4+(68*(int)medalFrame),4,64,64), backColor);
                ScreenManager.SpriteBatch.Draw(silver, new Rectangle((int)backgroundPosition.X + 200, (int)backgroundPosition.Y + 602, 40, 40),  new Rectangle (4+(68*(int)medalFrame),4,64,64), backColor);
                ScreenManager.SpriteBatch.Draw(gold, new Rectangle((int)backgroundPosition.X + 320, (int)backgroundPosition.Y + 602, 40, 40), new Rectangle (4+(68*(int)medalFrame),4,64,64), backColor);
                ScreenManager.SpriteBatch.Draw(levelStar, new Rectangle((int)backgroundPosition.X + 440, (int)backgroundPosition.Y + 600, 40, 40), backColor);
                ScreenManager.SpriteBatch.Draw(A, new Rectangle((int)backgroundPosition.X + 560, (int)backgroundPosition.Y + 597, 48, 48), backColor);
                ScreenManager.SpriteBatch.Draw(B, new Rectangle((int)backgroundPosition.X + 672, (int)backgroundPosition.Y + 597, 48, 48), backColor);
                Vector2 levelTitleLength = ScreenManager.smallFont.MeasureString(levelNameDisplay);
                //ScreenManager.SpriteBatch.DrawString(ScreenManager.font, levelNameDisplay, new Vector2(backgroundPosition.X + (menuBackground.Width / 2) - (levelTitleLength.X / 4), backgroundPosition.Y + 142), backColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);           
                //ScreenManager.SpriteBatch.DrawString(ScreenManager.font, displayBronze.ToString() + divisor, new Vector2(backgroundPosition.X + 130, backgroundPosition.Y + 608), backColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                //ScreenManager.SpriteBatch.DrawString(ScreenManager.font, displaySilver.ToString() + divisor, new Vector2(backgroundPosition.X + 250, backgroundPosition.Y + 608), backColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                //ScreenManager.SpriteBatch.DrawString(ScreenManager.font, displayGold.ToString() + divisor, new Vector2(backgroundPosition.X + 370, backgroundPosition.Y + 608), backColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                //ScreenManager.SpriteBatch.DrawString(ScreenManager.font, displayStars.ToString() + divisor, new Vector2(backgroundPosition.X + 490, backgroundPosition.Y + 608), backColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                //ScreenManager.SpriteBatch.DrawString(ScreenManager.font, "GO!", new Vector2(backgroundPosition.X + 610, backgroundPosition.Y + 608), backColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                //ScreenManager.SpriteBatch.DrawString(ScreenManager.font, "CANCEL", new Vector2(backgroundPosition.X + 710, backgroundPosition.Y + 608), backColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, levelNameDisplay, new Vector2(backgroundPosition.X + (menuBackground.Width / 2) - (levelTitleLength.X / 2), backgroundPosition.Y + 140), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, displayBronze.ToString() + divisor, new Vector2(backgroundPosition.X + 124, backgroundPosition.Y + 605), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, displaySilver.ToString() + divisor, new Vector2(backgroundPosition.X + 244, backgroundPosition.Y + 605), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, displayGold.ToString() + divisor, new Vector2(backgroundPosition.X + 364, backgroundPosition.Y + 605), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, displayStars.ToString() + divisor, new Vector2(backgroundPosition.X + 484, backgroundPosition.Y + 605), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, "GO!", new Vector2(backgroundPosition.X + 610, backgroundPosition.Y + 605), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.smallFont, "BACK", new Vector2(backgroundPosition.X + 722, backgroundPosition.Y + 605), backColor, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha);

                ScreenManager.SpriteBatch.End();
            }
            base.Draw(gameTime);
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);
            if (input.IsLTriggerPressed(ControllingPlayer, out playerIndex))
            {
                LevelDataManager.nextworld -= 1;
                if (LevelDataManager.nextworld < 0) LevelDataManager.nextworld = 6;
                ScreenManager.AddScreen(new LevelSelectScreen(), ControllingPlayer);
                ExitScreen();
            }
            if (input.IsRTriggerPressed(ControllingPlayer, out playerIndex))
            {
                LevelDataManager.nextworld += 1;
                if (LevelDataManager.nextworld > 6) LevelDataManager.nextworld = 0;
                ScreenManager.AddScreen(new LevelSelectScreen(), ControllingPlayer);
                ExitScreen();
            }

            // Move to the previous menu entry
            if (input.IsMenuUp(ControllingPlayer, out playerIndex))
            {
                if (menuType == MenuType.LevelGrid)
                {
                    selectedEntry -= 5;
                    if (selectedEntry < 0) selectedEntry += numberOfLevels;
                }
            }

            // Move to the next menu entry
            if (input.IsMenuDown(ControllingPlayer, out playerIndex))
            {
                if (menuType == MenuType.LevelGrid)
                {
                    selectedEntry += 5;
                    if (selectedEntry >= numberOfLevels) selectedEntry -= numberOfLevels;
                }
            }
        }


        #region MENU EVENTS
        void LevelMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            //enforce level Lock
            if (!LevelDataManager.levelData[LevelDataManager.nextworld, LevelDataManager.nextlevel].unlocked && GameSettings.LocksOn) return;

            //boss lock 
            //if (LevelDataManager.nextworld == 5 && LevelDataManager.nextlevel == 15) return; 

            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen(LevelDataManager.nextworld, LevelDataManager.nextlevel));
        }
        #endregion



    }
}
