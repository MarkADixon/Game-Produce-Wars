#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ParallaxEngine;
using ProduceWars.Managers;
#endregion

namespace ProduceWars
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class BackgroundScreen : GameScreen
    {
        #region Fields

        Random random = new Random();

        //update managers
        ParallaxManager parallaxEngine;
        DecoManager decoManager;
        public int worldWidth = 0;
        public int worldHeight = 0;
        public int world = 1;
        public int level = 0;
        public string texturePack;
        public float layerSpeed = 0f;
        public bool isDirFor = true;

        Texture2D pixel;
        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
        {
            //GameSettings.isBoss = false;
            TransitionOnTime = TimeSpan.FromSeconds(0);
            TransitionOffTime = TimeSpan.FromSeconds(0);
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent(ContentManager _content)
        {
            
            //world = LevelDataManager.nextworld;
            //if (world < 1 || world > 6) world = 1;
            parallaxEngine = new ParallaxManager();
            Camera.ResetZoom();
            //if (content == null)
            //    content = new ContentManager(ScreenManager.Game.Services, "Content");
            
            //the Level data manager instances its own content manager
            LevelDataManager.Initialize(ScreenManager.Game, parallaxEngine, world, level);
            LevelDataManager.UItextures.TryGetValue("Pixel", out pixel);

            Camera.CameraPositionLimits = Camera.WorldRectangle;
            Camera.LookAt(new Vector2 (0, Camera.WorldRectangle.Bottom));

            foreach (Layer layer in parallaxEngine.worldLayers)
            {
                layer.IsRepreating = true;
                layer.IsRepreatingSeamless = true;
                layer.IsAwake = true;
                layer.LayerVelocity = 0f;
                layer.IsLayerMotion = false;
                layer.LayerVDirection = new Vector2(-1,0);

            }

            decoManager = new DecoManager(parallaxEngine, false);
            for (int i = 0; i < parallaxEngine.worldLayers.Count; i++)
            {
                for (int j = 0; j < parallaxEngine.worldLayers[i].layerSprites.Count; j++)
                {
                    if (parallaxEngine.worldLayers[i].layerSprites[j].SpriteType == Sprite.Type.Deco)
                    {
                        decoManager.InitializeDeco(parallaxEngine.worldLayers[i].layerSprites[j]);
                    }
                }
            }
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {

        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
            Camera.Update(gameTime);
            parallaxEngine.Update(gameTime);
            decoManager.Update(gameTime);

            if (layerSpeed < 360f && isDirFor)
            {
              layerSpeed += 2;
            }

            //if (layerSpeed >= 360f && isDirFor) isDirFor = false;

            //if (layerSpeed > -360f && !isDirFor)
            //{
            //    layerSpeed -= 2;
            //    foreach (Layer layer in parallaxEngine.worldLayers) layer.LayerVDirection = new Vector2(1, 0);
            //}

            float distance = layerSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < parallaxEngine.worldLayers.Count; i++)
            {
                    for (int j = 0; j < parallaxEngine.worldLayers[i].layerSprites.Count; j++)
                    {
                        parallaxEngine.worldLayers[i].layerSprites[j].Location += new Vector2(-distance * parallaxEngine.worldLayers[i].LayerParallax.Y, 0f);
                    }
            }

            for (int i=0; i < decoManager.weatherLayer.SpriteCount; i++)
            {
                decoManager.weatherLayer.LayerSprites[i].Location += new Vector2(-distance * decoManager.weatherLayer.LayerParallax.Y, 0f);
            }
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                   Color.CornflowerBlue, 0, 0);


            parallaxEngine.Draw(gameTime, spriteBatch);
            decoManager.DrawWeather(gameTime, spriteBatch);

            if (GameSettings.TitleSafe)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 128, 720), Color.Red * 0.5f);
                spriteBatch.Draw(pixel, new Rectangle(1280 - 128, 0, 128, 720), Color.Red * 0.5f);
                spriteBatch.Draw(pixel, new Rectangle(0, 0, 1280, 72), Color.Red * 0.5f);
                spriteBatch.Draw(pixel, new Rectangle(0, 720 - 72, 1280, 72), Color.Red * 0.5f);
                spriteBatch.End();
            }

            //spriteBatch.Begin();
            //spriteBatch.Draw(backgroundTexture, fullscreen,
            //                 new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            //spriteBatch.End();
        }


        #endregion

 
    }
}
