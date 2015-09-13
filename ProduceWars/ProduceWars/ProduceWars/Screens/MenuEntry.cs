#region File Description
//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParallaxEngine;
#endregion

namespace ProduceWars
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    class MenuEntry
    {
        #region Fields

        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        string text;
        bool isText = false;

        //picture rendered for this entry
        Texture2D texture;
        bool isTexture = false;

        //entire level box for this entry
        Texture2D levelWithStar, numbers, levelLock, levelStar, levelBronze, levelSilver, levelGold;
        bool isLevelBox = false;
        int number;
        public bool isStarCollected = false;
        float starRotation = 0f;
        public bool isBronze = false;
        public bool isSilver = false;
        public bool isGold = false;
        bool isLock = true;
        float medalFrame = 0f;
        Vector2 origin, boxCenter;
        float time, xpos, ypos, fadeSpeed;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        float selectionFade;

        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        Vector2 position;
        public float scale = 0.8f;

        Color color = Color.White;
        ScreenManager screenManager;

        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get { return text; }
            set 
            { 
                text = value;
                isText = true;
                isTexture = false;
            }
        }

        //get or set the texture for menu entry
        public Texture2D Texture
        {
            get { return texture; }
            set
            {
                texture = value;
                isTexture = true;
                isText = false;
            }
        }


        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }


        #endregion

        #region Events


        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;
        public event EventHandler<PlayerIndexEventArgs> Plus;
        public event EventHandler<PlayerIndexEventArgs> Minus;


        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }

        protected internal virtual void OnRightEntry(PlayerIndex playerIndex)
        {
            if (Plus != null)
                Plus(this, new PlayerIndexEventArgs(playerIndex));
        }

        protected internal virtual void OnLeftEntry(PlayerIndex playerIndex)
        {
            if (Minus != null)
                Minus(this, new PlayerIndexEventArgs(playerIndex));
        }
        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string text)
        {
            this.text = text;
            this.isText = true;
        }

        public MenuEntry(Texture2D _texture)
        {
            this.texture = _texture;
            this.isTexture = true;
        }

        public MenuEntry(Texture2D _levelBox, Texture2D _levelWithStar, Texture2D _numbers, int _number, Texture2D _levelLock, Texture2D _levelStar, Texture2D _levelBronze, Texture2D _levelSilver, Texture2D _levelGold)
        {
            this.texture = _levelBox;
            this.isTexture = true;

            this.isLevelBox = true;
            this.levelWithStar = _levelWithStar;
            this.numbers = _numbers;
            this.number = _number;

            if (LevelDataManager.levelData[LevelDataManager.nextworld, number + 1] != null)
            {
                isStarCollected = LevelDataManager.levelData[LevelDataManager.nextworld, number + 1].starCollected;
                isLock = !LevelDataManager.levelData[LevelDataManager.nextworld, number + 1].unlocked;
                if (LevelDataManager.levelData[LevelDataManager.nextworld, number + 1].IsBronze()) isBronze = true;
                if (LevelDataManager.levelData[LevelDataManager.nextworld, number + 1].IsSilver()) isSilver = true;
                if (LevelDataManager.levelData[LevelDataManager.nextworld, number + 1].IsGold()) isGold = true;
            }
            
            this.levelLock = _levelLock;
            this.levelStar = _levelStar;
            this.levelBronze = _levelBronze;
            this.levelSilver = _levelSilver;
            this.levelGold = _levelGold;

            this.scale = 1.0f;
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime, float alpha)
        {
            // there is no such thing as a selected item on Windows Phone, so we always
            // force isSelected to be false
#if WINDOWS_PHONE
            isSelected = false;
#endif

            // Draw the selected entry in color, otherwise white.
            if (isText)
            {
                color = isSelected ? GameSettings.ShotPathColor : Color.White;
            }
            else color = Color.White;


            // Pulsate the size of the selected menu entry.
            time = (float)gameTime.TotalGameTime.TotalSeconds;
            
            //float pulsate = (float)Math.Sin(time * 6)+1;
            //scale = Math.Max(1.0f, (1 + pulsate * 0.05f * selectionFade));
            //if (!isSelected) scale = Math.Min(scale, lastscale); //ensures that while transitioning away from being seleced, the scale cant increase
            //lastscale = scale;

            // Modify the alpha to fade text out during transitions.
            color *= screen.TransitionAlpha;

            // Draw text, centered on the middle of each line.
            screenManager = screen.ScreenManager;

            if (isText)
            {
                //this origin line broke it?
                //Vector2 origin = new Vector2(-screen.ScreenManager.SmallFont.MeasureString(Text).X/2, -screen.ScreenManager.SmallFont.MeasureString(Text).Y/2);
                DrawStringHelper(screenManager.SpriteBatch, screenManager.smallFont, text, position, color, Vector2.Zero, scale, SpriteEffects.None, alpha);
            }

            if (isTexture)
            {
                origin = new Vector2((float)texture.Width / 2f, (float)texture.Height / 2f);
                xpos = position.X + origin.X - (float)texture.Width / 2f;
                ypos = position.Y + origin.Y - (float)texture.Height / 2f;

                //shadow
                if (isSelected)
                {
                    screenManager.SpriteBatch.Draw(texture, new Rectangle((int)xpos - 5, (int)ypos - 5, (int)((float)texture.Width * scale + 10), (int)((float)texture.Height * scale + 10)),
                                     new Rectangle(0, 0, texture.Width, texture.Height), new Color(0, 0, 0, 30));
                    screenManager.SpriteBatch.Draw(texture, new Rectangle((int)xpos - 4, (int)ypos - 4, (int)((float)texture.Width * scale + 8), (int)((float)texture.Height * scale + 8)),
                                     new Rectangle(0, 0, texture.Width, texture.Height), new Color(0, 0, 0, 60));
                    screenManager.SpriteBatch.Draw(texture, new Rectangle((int)xpos - 3, (int)ypos - 3, (int)((float)texture.Width * scale + 6), (int)((float)texture.Height * scale + 6)),
                                     new Rectangle(0, 0, texture.Width, texture.Height), new Color(0, 0, 0, 90));
                    screenManager.SpriteBatch.Draw(texture, new Rectangle((int)xpos - 2, (int)ypos - 2, (int)((float)texture.Width * scale + 4), (int)((float)texture.Height * scale + 4)),
                                     new Rectangle(0, 0, texture.Width, texture.Height), new Color(0, 0, 0, 120));
                    screenManager.SpriteBatch.Draw(texture, new Rectangle((int)xpos - 1, (int)ypos - 1, (int)((float)texture.Width * scale + 2), (int)((float)texture.Height * scale + 2)),
                                     new Rectangle(0, 0, texture.Width, texture.Height), new Color(0, 0, 0, 150));
                }

                screenManager.SpriteBatch.Draw(texture, new Rectangle((int)xpos, (int)ypos, (int)((float)texture.Width * scale), (int)((float)texture.Height * scale)),
                                     new Rectangle(0, 0, texture.Width, texture.Height), color);

                //level box elements
                if (isLevelBox)
                {
                    boxCenter = new Vector2(position.X + origin.X, position.Y + origin.Y);
                    medalFrame += 12 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (medalFrame >= 10f) medalFrame -= 10f;

                    //draw star
                    if (isStarCollected)
                    {
                        screenManager.SpriteBatch.Draw(levelWithStar, new Rectangle((int)xpos, (int)ypos, levelWithStar.Width, levelWithStar.Height),
                            new Rectangle(0, 0, levelWithStar.Width, levelWithStar.Height), color);
                        float starXpos, starYpos;
                        starXpos = boxCenter.X +36 - (levelStar.Width * 1.25f * 0.5f);
                        starYpos = boxCenter.Y +20 - (levelStar.Height * 1.25f * 0.5f);
                        starRotation += 0.04f;
                        screenManager.SpriteBatch.Draw(levelStar, new Vector2((int)starXpos, (int)starYpos),
                        new Rectangle(0, 0, levelStar.Width, levelStar.Height), color, starRotation, new Vector2(31,33), 1.25f, SpriteEffects.None, 0f);
                    }

                    //draw number
                    int row = (int)number / 5;
                    int column = (int)(number - (row * 5));
                    int boss = 0;
                    if (LevelDataManager.nextworld == 5 && number == 14) boss = 1;
                    screenManager.SpriteBatch.Draw(numbers, new Rectangle((int)boxCenter.X-75, (int)boxCenter.Y-75, 144, 144),new Rectangle(column*160, (row+boss)*160, 160, 160), color);

                    if (isBronze) screenManager.SpriteBatch.Draw(levelBronze, new Rectangle((int)boxCenter.X - 59, (int)boxCenter.Y + 25, 32, 32),new Rectangle (4+(68*(int)medalFrame),4,64,64), color);
                    if (isSilver) screenManager.SpriteBatch.Draw(levelSilver, new Rectangle((int)boxCenter.X-19, (int)boxCenter.Y+25, 32, 32),new Rectangle (4+(68*(int)medalFrame),4,64,64), color);
                    if (isGold) screenManager.SpriteBatch.Draw(levelGold, new Rectangle((int)boxCenter.X+21, (int)boxCenter.Y+25, 32, 32),new Rectangle (4+(68*(int)medalFrame),4,64,64), color);
                    if (isLock && GameSettings.LocksOn) screenManager.SpriteBatch.Draw(levelLock, new Rectangle((int)boxCenter.X-72, (int)boxCenter.Y-72, 144, 144), color);
                }

                if (!isSelected)
                {
                    screenManager.SpriteBatch.Draw(texture, new Rectangle((int)xpos, (int)ypos, (int)((float)texture.Width * scale), (int)((float)texture.Height * scale)),
                                     new Rectangle(0, 0, texture.Width, texture.Height), new Color(0, 0, 0, 128));
                }
            }

        }


        private void DrawStringHelper(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position,Color color, Vector2 origin, float scale, SpriteEffects effect, float alpha)
        {
           spriteBatch.DrawString(font, text, new Vector2(2, 2) + position, Color.Black*alpha, 0, origin, scale, effect, 0);
            spriteBatch.DrawString(font, text, new Vector2(-2, 2) + position, Color.Black*alpha, 0, origin, scale, effect, 0);
            spriteBatch.DrawString(font, text, new Vector2(2, -2) + position, Color.Black*alpha, 0, origin, scale, effect, 0);
            spriteBatch.DrawString(font, text, new Vector2(-2, -2) + position, Color.Black*alpha, 0, origin, scale, effect, 0);
            spriteBatch.DrawString(font, text, position, color, 0 , origin, scale, effect, 0 );
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
            if (isText) return (int)(screen.ScreenManager.smallFont.MeasureString(Text).Y * scale);
            if (isTexture) return (int)(texture.Height * scale);
            return 0;
        }


        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(MenuScreen screen)
        {
            if (isText) return (int)(screen.ScreenManager.smallFont.MeasureString(Text).X * scale);
            if (isTexture) return (int)(texture.Width *scale);
            return 0;
        }

        public virtual Rectangle MenuRectangle(MenuScreen screen)
        {
            //rectangle with a little x direction padding to help select
            if (isText)
            {
                int width = (int)screen.ScreenManager.smallFont.MeasureString(Text).X;
                int height = (int)screen.ScreenManager.smallFont.MeasureString(Text).Y;
                return new Rectangle((int)(this.Position.X + (width * 0.5f) - (width * scale * 0.5f)),
                                  (int)(this.Position.Y + (height * 0.5f) - (height * scale * 0.5f)),
                                  (int)(width * scale),
                                  (int)(height * scale));
            }
            if (isTexture)
            {
                int width = texture.Width;
                int height = texture.Height;
                return new Rectangle((int)(this.Position.X + (width*0.5f) - (width * scale * 0.5f)),
                                     (int)(this.Position.Y + (height * 0.5f) - (height * scale *0.5f)),
                                     (int)(width * scale),
                                     (int)(height * scale));
            }
            return new Rectangle(0, 0, 0, 0);
        }

        #endregion
    }
}
