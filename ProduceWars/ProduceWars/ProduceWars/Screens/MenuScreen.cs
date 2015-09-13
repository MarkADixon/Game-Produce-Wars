#region File Description
//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Input;
using ParallaxEngine;
using ProduceWars.Managers;

#endregion

namespace ProduceWars
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        #region Fields
        public PlayerIndex playerIndex;
        List<MenuEntry> menuEntries = new List<MenuEntry>();
        public int selectedEntry = 0;
        public float transitionOffset = 0f;
        string menuTitle;
        public float titleScale = 1.5f;
        public Texture2D menuTitleGraphic, menuBackground;
        public bool isMenuTitleGraphic = false;
        public float menuTitleGraphicScale = 1.0f;
        public float menuTitleGraphicBottom = 0f;
        Vector2 titlePosition = Vector2.Zero;
        Vector2 titleOrigin = Vector2.Zero;
        Color titleColor = Color.White;
        bool isInitialized = false;

        public enum MenuType { VerticalList, LevelGrid }
        public MenuType menuType = MenuType.VerticalList;



        

        #endregion

        #region Properties


        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public MenuScreen(string _menuTitle, MenuType _type)
        {
            this.menuTitle = _menuTitle;
            this.menuType = _type;  
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void LoadContent(Microsoft.Xna.Framework.Content.ContentManager _content)
        {
            base.LoadContent(_content);            
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            // Accept or cancel the menu? We pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            //PlayerIndex playerIndex;

            // Move to the previous menu entry?
            if (input.IsMenuUp(ControllingPlayer, out playerIndex))
            {
                SoundManager.Play(SoundManager.Sound.CursorMove, false,false);

                if (menuType == MenuType.VerticalList)
                {
                    selectedEntry--;
                    if (selectedEntry < 0)
                        selectedEntry = menuEntries.Count - 1;
                }
            }

            // Move to the next menu entry?
            if (input.IsMenuDown(ControllingPlayer, out playerIndex))
            {
                SoundManager.Play(SoundManager.Sound.CursorMove, false,false);

                if (menuType == MenuType.VerticalList)
                {
                    selectedEntry++;
                    if (selectedEntry >= menuEntries.Count)
                        selectedEntry = 0;
                }
            }

            if (input.IsMenuSelect(ControllingPlayer, out playerIndex))
            {
                SoundManager.Play(SoundManager.Sound.CursorSelect, false,false);

                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (input.IsMenuCancel(ControllingPlayer, out playerIndex))
            {
                SoundManager.Play(SoundManager.Sound.CursorSelect, false,false);

                OnCancel(playerIndex);
            }

            if (input.IsMenuRight(ControllingPlayer, out playerIndex))
            {
                SoundManager.Play(SoundManager.Sound.CursorMove, false,false);

                if (menuType == MenuType.VerticalList) OnRightEntry(selectedEntry, playerIndex);
                if (menuType == MenuType.LevelGrid)
                {
                    int row = selectedEntry/5;
                    selectedEntry++;
                    if (selectedEntry/5 != row) selectedEntry -= 5;
                }
            }

            if (input.IsMenuLeft(ControllingPlayer, out playerIndex))
            {
                SoundManager.Play(SoundManager.Sound.CursorMove, false,false);

                if (menuType == MenuType.VerticalList) OnLeftEntry(selectedEntry, playerIndex);
                if (menuType == MenuType.LevelGrid)
                {
                    int row = selectedEntry / 5;
                    if (selectedEntry == 0) selectedEntry = 4;
                    else selectedEntry--;
                    if (selectedEntry / 5 != row) selectedEntry += 5;
                }
            }
#if WINDOWS //handle mouse control for selection, right click set to cancel in InputState Class
            for (int i = 0; i < menuEntries.Count; i++)
            {
                if (input.IsMouseHover(menuEntries[i].MenuRectangle(this))) 
                {
                    if (selectedEntry != i)
                    {
                        SoundManager.Play(SoundManager.Sound.CursorMove, false,false);
                        selectedEntry = i;
                    }

                    if (input.IsMouseSelect(menuEntries[selectedEntry].MenuRectangle(this)))
                    {
                        SoundManager.Play(SoundManager.Sound.Click, false,false);
                        OnSelectEntry(selectedEntry, playerIndex);
                    }
                }
            }
#endif

        }


        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            if (menuType == MenuType.LevelGrid) LevelDataManager.nextlevel = entryIndex + 1; 
            menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }

        protected virtual void OnRightEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnRightEntry(playerIndex);
        }

        protected virtual void OnLeftEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnLeftEntry(playerIndex);
        }
        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }


        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations()
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            transitionOffset = (float)Math.Pow(TransitionPosition, 2);
            Vector2 position = Vector2.Zero;
            // start at Y = 175; each X value is generated per entry
            if (!isMenuTitleGraphic) position += new Vector2(0f, 140f);
            if (isMenuTitleGraphic) position += new Vector2(0f, menuTitleGraphicBottom);
            //if (isMenuTitleGraphic) position += new Vector2(0f, menuTitleGraphic.Height * menuTitleGraphicScale);
            // update each menu entry's location in turn
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];

                if (menuType == MenuType.VerticalList)
                {
                    // each entry is to be centered horizontally
                    position.X = (ScreenManager.GraphicsDevice.Viewport.Width *0.5f) - (menuEntry.GetWidth(this) *0.5f);

                    if (ScreenState == ScreenState.TransitionOn)
                        position.X -= transitionOffset * 512;
                    else
                        position.X += transitionOffset * 512;

                    // set the entry's position
                    menuEntry.Position = position;

                    // move down for the next entry the size of this entry
                    position.Y += menuEntry.GetHeight(this);
                }

                if (menuType == MenuType.LevelGrid)
                {
                    // each entry is to be centered horizontally
                    position.X = (ScreenManager.GraphicsDevice.Viewport.Width / 2f) - (menuEntry.GetWidth(this) *0.5f);

                    //row and column calculation
                    int row = (int)i / 5;
                    int column = (int)(i - (row * 5) - 2); //values -2,-1,0,1,2 for 5 columns

                    //then adjusted for column position
                    position.X += column * menuEntry.GetWidth(this);

                    //then adjusted for row position
                    position.Y = menuTitleGraphicBottom + (row * menuEntry.GetHeight(this)) + 28;

                    if (ScreenState == ScreenState.TransitionOn)
                        position.X -= transitionOffset * 512;
                    else
                        position.X += transitionOffset * 512;

                    // set the entry's position
                    menuEntry.Position = position;
                }
            }


        }


        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);

                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }


        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            if (!isInitialized) InitializeValues();

            // make sure our entries are in the right place before we draw them
            UpdateMenuEntryLocations();

            ScreenManager.SpriteBatch.Begin();

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);
                menuEntries[i].Draw(this, isSelected, gameTime, TransitionAlpha);
            }

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Draw the menu title centered on the screen
            if (!isMenuTitleGraphic)
            {
                titlePosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 72*titleScale);
                titleOrigin = ScreenManager.font.MeasureString(menuTitle) / 2;
                titleColor = Color.White * TransitionAlpha;
                titlePosition.Y -= transitionOffset * 100;
                DrawStringHelper(ScreenManager.SpriteBatch, ScreenManager.font, menuTitle, titlePosition, titleColor, titleOrigin, titleScale, SpriteEffects.None, TransitionAlpha);
            }

            if (isMenuTitleGraphic)
            {
                titleOrigin = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 72 + (menuTitleGraphic.Height * menuTitleGraphicScale *0.5f));
                titlePosition = new Vector2(titleOrigin.X - (menuTitleGraphic.Width * menuTitleGraphicScale/2), titleOrigin.Y - (menuTitleGraphic.Height * menuTitleGraphicScale * 0.5f));
                titleColor = Color.White * TransitionAlpha;
                menuTitleGraphicBottom = titlePosition.Y + (menuTitleGraphic.Height * menuTitleGraphicScale); 
                titlePosition.Y -= transitionOffset * 100;
                ScreenManager.SpriteBatch.Draw(menuTitleGraphic, new Rectangle((int)titlePosition.X,(int)titlePosition.Y,(int)(menuTitleGraphic.Width * menuTitleGraphicScale),(int)(menuTitleGraphic.Height * menuTitleGraphicScale)),titleColor);
            }

            
            ScreenManager.SpriteBatch.End();
        }

        #endregion

        private void InitializeValues()
        {
            isInitialized = true;
            if (!isMenuTitleGraphic)
            {
                titlePosition = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 72 * titleScale);
                titleOrigin = ScreenManager.font.MeasureString(menuTitle) / 2;
                titleColor = Color.White * TransitionAlpha;
            }

            if (isMenuTitleGraphic)
            {
                titleOrigin = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 72 + (menuTitleGraphic.Height * menuTitleGraphicScale * 0.5f));
                titlePosition = new Vector2(titleOrigin.X - (menuTitleGraphic.Width * menuTitleGraphicScale / 2), titleOrigin.Y - (menuTitleGraphic.Height * menuTitleGraphicScale * 0.5f));
                titleColor = Color.White * TransitionAlpha;
                menuTitleGraphicBottom = titlePosition.Y + (menuTitleGraphic.Height * menuTitleGraphicScale);
            }
        }
    }
}
