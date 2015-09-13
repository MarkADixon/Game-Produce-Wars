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
    class WorldSelectScreen : MenuScreen
    {
        Texture2D World0, World1, World2, World3, World4, World5, World6;

        #region CONSTRUCTOR

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public WorldSelectScreen()
            : base("Stage Select")
        {
        }
        #endregion

        public override void LoadContent(ContentManager _content)
        {
            LevelDataManager.UItextures.TryGetValue("World0", out World0);
            LevelDataManager.UItextures.TryGetValue("World1", out World1);
            LevelDataManager.UItextures.TryGetValue("World2", out World2);
            LevelDataManager.UItextures.TryGetValue("World3", out World3);
            LevelDataManager.UItextures.TryGetValue("World4", out World4);
            LevelDataManager.UItextures.TryGetValue("World5", out World5);
            LevelDataManager.UItextures.TryGetValue("World6", out World6);
            LevelDataManager.UItextures.TryGetValue("MainSingle", out menuTitleGraphic);
            isMenuTitleGraphic = true;

            //create textured menu entries
            MenuEntry World0MenuEntry = new MenuEntry(World0);
            MenuEntry World1MenuEntry = new MenuEntry(World1);
            MenuEntry World2MenuEntry = new MenuEntry(World2);
            MenuEntry World3MenuEntry = new MenuEntry(World3);
            MenuEntry World4MenuEntry = new MenuEntry(World4);
            MenuEntry World5MenuEntry = new MenuEntry(World5);
            MenuEntry World6MenuEntry = new MenuEntry(World6);

            // Hook up menu event handlers.
            World0MenuEntry.Selected += World0MenuEntrySelected;
            World1MenuEntry.Selected += World1MenuEntrySelected;
            World2MenuEntry.Selected += World2MenuEntrySelected;
            World3MenuEntry.Selected += World3MenuEntrySelected;
            World4MenuEntry.Selected += World4MenuEntrySelected;
            World5MenuEntry.Selected += World5MenuEntrySelected;
            World6MenuEntry.Selected += World6MenuEntrySelected;


            // Add entries to the menu.
            MenuEntries.Add(World0MenuEntry);
            MenuEntries.Add(World1MenuEntry);
            MenuEntries.Add(World2MenuEntry);
            MenuEntries.Add(World3MenuEntry);
            MenuEntries.Add(World4MenuEntry);
            MenuEntries.Add(World5MenuEntry);
            MenuEntries.Add(World6MenuEntry);
        }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
        }


        #region MENU EVENTS
        void World0MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LevelDataManager.nextworld = 0;
            ScreenManager.AddScreen(new LevelSelectScreen(), e.PlayerIndex);
        }

        void World1MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LevelDataManager.nextworld = 1;
            ScreenManager.AddScreen(new LevelSelectScreen(), e.PlayerIndex);
        }

        void World2MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LevelDataManager.nextworld = 2;
            ScreenManager.AddScreen(new LevelSelectScreen(), e.PlayerIndex);
        }

        void World3MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LevelDataManager.nextworld = 3;
            ScreenManager.AddScreen(new LevelSelectScreen(), e.PlayerIndex);
        }

        void World4MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LevelDataManager.nextworld = 4;
            ScreenManager.AddScreen(new LevelSelectScreen(), e.PlayerIndex);
        }

        void World5MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LevelDataManager.nextworld = 5;
            ScreenManager.AddScreen(new LevelSelectScreen(), e.PlayerIndex);
        }

        void World6MenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LevelDataManager.nextworld = 6;
            ScreenManager.AddScreen(new LevelSelectScreen(), e.PlayerIndex);
        }
        #endregion




    }
}
