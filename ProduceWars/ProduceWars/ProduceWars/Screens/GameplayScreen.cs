#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
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
using FarseerPhysics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Common;
using ProduceWars.Managers;

#endregion

namespace ProduceWars
{

    public class GameplayScreen : GameScreen
    {
        #region Fields
        ContentManager content;
        SpriteBatch spriteBatch;
        KeyboardState keyboardState;
        GamePadState gamePadState;
        PlayerIndex controllingPlayer;
        PlayerIndex responsePlayer;
        Vector2 cameraMovement;
        Vector2 thumbstickL = Vector2.Zero;
        Vector2 lastThumbstick = Vector2.Zero;
        Vector2 invertYThumbstick = new Vector2(1, -1);

        PresentationParameters pp; //aa
        RenderTarget2D renderTarget; //aa
        RenderTarget2D renderTarget2; //aa
        float blurAlpha = 0.25f;
        float fadeout = 1f;

        //UI asset variables
        //SpriteFont xboxButtonFont;
        Texture2D menuBackground, menuCorner, menuTopSide, pellet, fruitBar, smLock,cursor,pixel;

        //applejack stuff
        Texture2D appleJack;
        Vector2 appleJackPos = Vector2.Zero;
        bool isAppleJack = false;
        
        
        //shot window variables
        Texture2D shotWindow, medalGold, medalSilver, medalBronze,noMedal;
        float medalFrame = 0f;
        Vector2 shotWindowLocation = new Vector2(128,72);
        int shotNumber = 0;
        Sprite star;
        Color shotWinTint = Color.White;
        float failAlpha = 0f;
        bool isFailed = false;
        float bossdeadFade = 0f;
        //string score, combo = "";

        //shotbar variables
        Texture2D powerBar, powerFrame, powerMarkerTarget;
        Vector2 powerBarLocation = new Vector2(130, 610);
        float powerBarFill = 0.0f; //value is 0 to 1 representing % full on power bar
        float powerBarFillRate = 0.6f; //change the speed of the power bar movement
        bool isPowerFillDirectionUp = true;
        Vector2 powerTargetLocation; //initialized in load content, position relative to powerBarLocation
        Vector2 powerSetLocation;
        float powerSetBarFill = 0.0f;
        float powerSlope;
        float powerMod;
        float powerTargetFill;
        int misfireAngle = 45; //the maximum off angle for misfire
        float preciseTimePellet = 0;

        //level name variables
        string levelName;
        Vector2 levelNameSize;

        //physics variables
        DebugViewXNA oDebugView;
        World physicsWorld = new World(new Vector2(0,GameSettings.Gravity)); //sets the gravity
        int physicsScale = GameSettings.PhysicsScale;
        int physicsActivationIntervalMS = 1000;
        int physicsActivationTimer = 0;


        float pauseAlpha;
        Random random = new Random();

        //update managers
        ParallaxManager parallaxEngine;
        ContactListener contactListener;
        ContactSolver contactSolver;
        TerrainManager terrainManager;
        BlockManager blockManager;
        EnemyManager enemyManager;
        ShotManager shotManager;
        HazardManager hazardManager;
        ExplosiveManager explosiveManager;
        EffectManager effectManager;
        DecoManager decoManager;
        IntroManager introManager;
        
  
        //world variables
        public int worldWidth = 0;
        public int worldHeight = 0;
        public int world = 0;
        public int level = 0;
        public int interactLayer = 0;

        //Camera control variables
        public Vector2 firingLocation = Vector2.Zero;
        public Vector2 shotLocation = Vector2.Zero;
        public Vector2 barrelEndLocation = Vector2.Zero;
      
        //for FPS counter, can remove at publish
        int averagingCount = 0;
        float averagingTotal = 0.0f;
        float averagedFPS = 0.0f;

        //for when shot is off camera
        bool showCursor = false;
        bool isCursorUp = false;
        bool isCursorDown = false;
        bool isCursorLeft = false;
        bool isCursorRight = false;
        float cursorRotation = 0.0f;  
        Vector2 cursorPosition = Vector2.Zero;
        Vector2 cursorOrigin = Vector2.Zero;
        Rectangle shotRect;

        Sprite apple= new Sprite(28, 0, Vector2.Zero, true);

        //game management variables
        public enum LevelState {Intro, Countdown, Aim, Power, Fire, PowerUpAim, PowerUpFire, Win };
        public LevelState levelState = LevelState.Intro;
        public LevelState lastState = LevelState.Intro;
        bool isVictoryScreen = false;
        float countdown = GameSettings.shotTimer;
        int countdownInt = 0;
        string countdownStr = ""; 
        float firingAngle = 45f;
        float firingPower = (GameSettings.MaxFiringPower+GameSettings.MinFiringPower) * 0.5f;
        float actualPower = 0.0f;
        Vector2 firingForce = Vector2.Zero;
        Vector2 firingDirection = new Vector2(1,-1);
        bool thisisBoss = false;


        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(int world, int level)
        {
            this.world = world;
            this.level = level;
            if (world == 5 && level == 15)
            {
                thisisBoss = true;
                GameSettings.isBoss = true;
            }
            else GameSettings.isBoss = false;

            if (world == 6 && level == 10) physicsWorld.Gravity = new Vector2(0, -1f*GameSettings.Gravity);

            TransitionOnTime = TimeSpan.FromSeconds(1f);
            TransitionOffTime = TimeSpan.FromSeconds(1f);
            
        }
        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent(ContentManager _content)  
        {
            base.LoadContent(_content);

            Camera.Score = 0;
            Camera.ScoreCombo = 1;

            //setup physics world
            ConvertUnits.SetDisplayUnitToSimUnitRatio((float)physicsScale);
            
            spriteBatch = ScreenManager.SpriteBatch;
            pp = ScreenManager.GraphicsDevice.PresentationParameters; //aa
            renderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false, pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents); //aa
            renderTarget2 = new RenderTarget2D(ScreenManager.GraphicsDevice, pp.BackBufferWidth/2, pp.BackBufferHeight/2, false, pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents); //aa

            parallaxEngine = new ParallaxManager();
            contactListener = new ContactListener(physicsWorld);
            contactSolver = new ContactSolver(physicsWorld, contactListener);
            contactListener.PowerUpActivated += new ContactListener.EffectEventHandler(contactListener_PowerUpActivated);
 
            //gameplay instances a new content manager for UI
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");
            
            //level data manager instances its own content managers for level textures and effects
            LevelDataManager.Initialize(ScreenManager.Game, parallaxEngine, world, level);
            SoundManager.InitializeSoundEvents(contactListener);
            SoundManager.LoadMusic(LevelDataManager.levelContent, world);

            //xboxButtonFont = content.Load <SpriteFont>("GameUI\\xboxControllerSpriteFont");
            menuBackground = content.Load<Texture2D>("GameUI\\MenuBack");
            menuCorner = content.Load<Texture2D>("GameUI\\MenuCorner");
            menuTopSide = content.Load<Texture2D>("GameUI\\MenuSide");
            pellet = content.Load<Texture2D>("GameUI\\pellet");
            powerFrame = content.Load<Texture2D>("GameUI\\PowerFrame");
            powerBar = content.Load<Texture2D>("GameUI\\PowerBar");
            powerMarkerTarget = content.Load<Texture2D>("GameUI\\Power01");
            fruitBar = content.Load<Texture2D>("GameUI\\FruitBar");
            shotWindow = content.Load<Texture2D>("GameUI\\ShotWindow");
            smLock = content.Load<Texture2D>("GameUI\\Small_Lock");
            LevelDataManager.UItextures.TryGetValue("MedalBronze", out medalBronze);
            LevelDataManager.UItextures.TryGetValue("MedalSilver", out medalSilver);
            LevelDataManager.UItextures.TryGetValue("MedalGold", out medalGold);
            LevelDataManager.UItextures.TryGetValue("NoMedal", out noMedal);
            LevelDataManager.UItextures.TryGetValue("Cursor", out cursor);
            LevelDataManager.UItextures.TryGetValue("Pixel", out pixel);
            LevelDataManager.UItextures.TryGetValue("AppleJack", out appleJack);
            cursorOrigin = new Vector2(cursor.Width / 2, cursor.Height / 2);

            star = new Sprite(51, 0, Vector2.Zero, false);
            star.TintColor = new Color (0,0,0,0);
            star.Location = new Vector2(shotWindowLocation.X + (shotWindow.Width * 0.5f) - (star.SpriteRectWidth * 0.5f),
                                         shotWindowLocation.Y + (shotWindow.Height * 0.5f) - (star.SpriteRectHeight * 0.5f));
            
            terrainManager = new TerrainManager(physicsWorld);
            blockManager = new BlockManager(physicsWorld, contactListener);
            enemyManager = new EnemyManager(physicsWorld, contactListener);
            shotManager = new ShotManager(physicsWorld, contactListener);
            hazardManager = new HazardManager(physicsWorld, contactListener);
            explosiveManager = new ExplosiveManager(physicsWorld, contactListener);
            introManager = new IntroManager(ScreenManager.buxtonFont,ScreenManager.smallFont, world,level);
            
            
            #region  ACTIVE BARREL
            for (int i = 0; i < parallaxEngine.worldLayers.Count; i++) 
            {
                for (int j = 0; j < parallaxEngine.worldLayers[i].SpriteCount; j++) 
                {

                    if (parallaxEngine.worldLayers[i].layerSprites[j].SpriteType == Sprite.Type.PowerUp) 
                    {
                        //find the barrel with variable set to -1
                        if (parallaxEngine.worldLayers[i].layerSprites[j].HitPoints == -1 )
                        {
                            shotManager.ShotStartBarrel = parallaxEngine.worldLayers[i].layerSprites[j];
                            shotManager.ActivePowerUpBarrel = shotManager.ShotStartBarrel;
                            interactLayer = i;
                        }
                        
                    }
                }
            }
            #endregion

            
           
            //different objects are updated through their manager classes therefore the play area layer is set to not awake so that update is skipped for the layer
            for ( int i = 0 ; i < parallaxEngine.worldLayers.Count; i++)
            {
                parallaxEngine.worldLayers[i].IsAwake = false;
            }


            //create physical bodies for all objects, for parallax 1.o layers
            for (int j = 0; j < parallaxEngine.worldLayers.Count; j++)
            {
                if (parallaxEngine.worldLayers[j].LayerParallax == Vector2.One)
                {
                    for (int i = 0; i < parallaxEngine.worldLayers[j].SpriteCount; i++)
                    {
                        CreateBody(physicsWorld, parallaxEngine.worldLayers[j].layerSprites[i]);
                    }
                }
            }

            //UI setup
            levelName = "WORLD " + LevelDataManager.world.ToString() + "-" + LevelDataManager.level.ToString() + "  :  " + LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].name;
            levelNameSize = ScreenManager.font.MeasureString(levelName);
            

            //create debug view
            oDebugView = new DebugViewXNA(physicsWorld)
            {
                DefaultShapeColor = Color.Magenta,
                SleepingShapeColor = Color.Pink,
                StaticShapeColor = Color.Yellow,
            };
            this.oDebugView.LoadContent(ScreenManager.GraphicsDevice, content, ScreenManager.smallFont);
            this.oDebugView.AppendFlags(DebugViewFlags.Shape);
            this.oDebugView.AppendFlags(DebugViewFlags.PolygonPoints);
            this.oDebugView.AppendFlags(DebugViewFlags.CenterOfMass);

            powerTargetLocation = new Vector2(powerBarLocation.X - 24, powerBarLocation.Y - 36);
            powerSlope = (GameSettings.MaxFiringPower - GameSettings.MinFiringPower) / 0.8f;
            powerMod = GameSettings.MaxFiringPower - powerSlope;


            PlayArea.AddSpriteToLayer(shotManager.shot);
            firingPower = (GameSettings.MaxFiringPower + GameSettings.MinFiringPower) * 0.5f;
            firingDirection = new Vector2(1, -1);
            firingAngle = 45f;
            firingForce = firingDirection * firingPower;
            firingLocation = shotManager.ActivePowerUpBarrel.SpriteCenterInWorld;
            shotLocation = shotManager.shot.SpriteCenterInWorld;

            shotManager.shot.spriteBody = BodyFactory.CreateCircle(physicsWorld, ConvertUnits.ToSimUnits(32f), 1f, shotManager.ShotStartBarrel.spriteBody.Position, shotManager.shot);
            shotManager.shot.spriteBody.Enabled = false;
            shotManager.shot.spriteBody.BodyType = BodyType.Dynamic;
            
            #region INITIALIZE DECO SPRITES TO MANAGER
            decoManager = new DecoManager(parallaxEngine, true);
            for (int i = 0; i < parallaxEngine.worldLayers.Count; i++)
            {
                for (int j = 0; j < parallaxEngine.worldLayers[i].SpriteCount; j++)
                {

                    if (parallaxEngine.worldLayers[i].layerSprites[j].SpriteType == Sprite.Type.Deco)
                    {
                        decoManager.InitializeDeco(parallaxEngine.worldLayers[i].layerSprites[j]);
                    }
                    if (parallaxEngine.worldLayers[i].layerSprites[j].SpriteType == Sprite.Type.Boss)
                    {
                        enemyManager.bossLayer = parallaxEngine.worldLayers[i];
                    }
                    if (parallaxEngine.worldLayers[i].layerSprites[j].TextureID == 3) //cage
                    {
                        enemyManager.cageLayer = parallaxEngine.worldLayers[i];
                    }
                }
            }
            #endregion

            effectManager = new EffectManager(physicsWorld, contactListener, decoManager.Tint);

            #region SETUP 5-15 boss level
            if (world == 5 && level == 15)
            {                
                LevelDataManager.levelData[world, level].safety = true;
            }
            #endregion

            #region SETUP APPLEJACK LEVELS
            if (world == 6 && level == 3)
            {
                appleJackPos = new Vector2(2914, 1344);
                isAppleJack = true;
            }
            if (world == 6 && level == 5)
            {
                appleJackPos = new Vector2(2880, 1344);
                isAppleJack = true;
            }
            if (world == 6 && level == 7)
            {
                appleJackPos = new Vector2(3512, 1344);
                isAppleJack = true;
            }
            #endregion

            //set starting level state
            if (introManager.IntroFinished())
            {
                Camera.ScrollTo(firingLocation, 0);
                Camera.IsScrolling = true;
                if (!LevelDataManager.levelData[world, level].safety)
                {
                    shotManager.shot.spriteBody.Enabled = true;
                    shotManager.shot.spriteBody.IsSensor = true;
                    levelState = LevelState.Aim;
                }
                else levelState = LevelState.Countdown;
            }
            else levelState = LevelState.Intro;

            if (GameSettings.isBoss) Camera.ZoomTo(0.6f);

            #region APPLY CHEATS
            if (GameSettings.CheatTNTBarrel) shotManager.ShotStartBarrel.TextureIndex = 4;
            if (GameSettings.CheatFireBarrel) shotManager.ShotStartBarrel.TextureIndex = 1;
            if (GameSettings.CheatLightningBarrel) shotManager.ShotStartBarrel.TextureIndex = 3;
            if (GameSettings.CheatGrowthBarrel) shotManager.ShotStartBarrel.TextureIndex = 7;
            if (GameSettings.CheatCannonBarrel) shotManager.ShotStartBarrel.TextureIndex = 8;
            if (GameSettings.CheatSawBarrel) shotManager.ShotStartBarrel.TextureIndex = 9;
            


            #endregion

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            
            ScreenManager.Game.ResetElapsedTime();
        }

        public void CreateBody(World world, Sprite sprite)
        {

            switch (sprite.SpriteType)
            {
                case Sprite.Type.Terrain:
                    {
                        if (sprite.HitPoints == 0) terrainManager.CreateTerrain(sprite, world);
                        break;
                    }
                case Sprite.Type.Block:
                    {
                        blockManager.CreateBlock(sprite, world);
                        sprite.IsCollidable = true;
                        break;
                    }
                case Sprite.Type.Veggie:
                    {
                        enemyManager.CreateVeggie(sprite, world);
                        break;
                    }
                case Sprite.Type.Boss:
                    {
                        enemyManager.CreateBoss(sprite, world);
                        break;
                    }
                case Sprite.Type.Fruit:
                    {
                        enemyManager.CreateFruit(sprite, world);
                        sprite.IsCollidable = true;
                        break;
                    }
                case Sprite.Type.Star:
                    {
                        shotManager.CreateStar(sprite, world);
                        break;
                    }
                case Sprite.Type.Creature:
                    {
                        hazardManager.CreateCreature(sprite, world);
                        break;
                    }
                case Sprite.Type.Fan:
                    {
                        hazardManager.CreateFan(sprite, world);
                        break;
                    }
                case Sprite.Type.Saw:
                    {
                        hazardManager.CreateSaw(sprite, world);
                        break;
                    }
                case Sprite.Type.Smasher:
                    {
                        hazardManager.CreateSmasher(sprite, world);
                        break;
                    }
                case Sprite.Type.Windmill:
                    {
                        hazardManager.CreateWindmillBlade(sprite, world);
                        break;
                    }
                case Sprite.Type.Explosive:
                    {
                        explosiveManager.CreateExplosive(sprite, world);
                        break;
                    }
                case Sprite.Type.PowerUp:
                    {
                        shotManager.CreateBarrel(sprite, world);
                        break;
                    }
                case Sprite.Type.Switch:
                    {
                        hazardManager.CreateSwitch(sprite, world);
                        break;
                    }
                case Sprite.Type.Spike:
                    {
                        hazardManager.CreateSpike(sprite, world);
                        break;
                    }
                case Sprite.Type.Spring:
                    {
                        hazardManager.CreateSpring(sprite, world);
                        break;
                    }
                case Sprite.Type.Flame:
                    {
                        hazardManager.CreateFlamethrower(sprite, world);
                        break;
                    }
                case Sprite.Type.Tower:
                    {
                        hazardManager.CreateTower(sprite, world);
                        break;
                    }
                default:
                    break;
            }






        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {

            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f *0.03f, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f *0.03f, 0);

            if (!IsActive) return;  //if this screen is not the active one (game is puased  or victory screen) skip the update step from here

            if (levelState == LevelState.Intro)
            {
                //test text box
                introManager.Update(gameTime);
                decoManager.Update(gameTime);
                Camera.Update(gameTime);

                if (introManager.IntroFinished())
                {
                    Camera.ScrollTo(firingLocation, 0);
                    Camera.IsScrolling = true;
                    if (!LevelDataManager.levelData[world, level].safety)
                    {
                        shotManager.shot.spriteBody.Enabled = true;
                        shotManager.shot.spriteBody.IsSensor = true;
                        levelState = LevelState.Aim;
                    }
                    else levelState = LevelState.Countdown;
                }
                return;
            }

            #region PHYSICS UPDATE
            //periodically activate all bodies to keep any from floating while asleep
            physicsActivationTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (physicsActivationTimer > physicsActivationIntervalMS)
            {
                physicsActivationTimer = 0;
                for (int i = 0; i < physicsWorld.BodyList.Count; i++)
                {
                    physicsWorld.BodyList[i].Awake = true;
                }
            }
            physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
            #endregion

            parallaxEngine.Update(gameTime);
            effectManager.Update(gameTime);
            shotManager.Update(gameTime);
            enemyManager.Update(gameTime);
            blockManager.Update(gameTime);
            hazardManager.Update(gameTime, levelState, shotManager.shot.Location, shotManager.shot.IsExpired);
            terrainManager.Update(gameTime);
            explosiveManager.Update(gameTime);
            decoManager.Update(gameTime);

            firingLocation = shotManager.ActivePowerUpBarrel.SpriteCenterInWorld;
            shotLocation = shotManager.shot.SpriteCenterInWorld;


            //zoom based on height
            //if (shotLocation.Y < Camera.WorldRectangle.Height * 0.15f) Camera.Zoom = 0.5f;
            //if (shotLocation.Y >= Camera.WorldRectangle.Height * 0.15f) Camera.Zoom = 0.6f;
            //if (shotLocation.Y >= Camera.WorldRectangle.Height * 0.30f) Camera.Zoom = 0.7f;
            //if (shotLocation.Y >= Camera.WorldRectangle.Height * 0.45f) Camera.Zoom = 0.8f;
            //if (shotLocation.Y >= Camera.WorldRectangle.Height * 0.6f) Camera.Zoom = 0.9f;
            //if (shotLocation.Y >= Camera.WorldRectangle.Height * 0.75f) Camera.Zoom = 1.0f;

            if (levelState == LevelState.Countdown || levelState == LevelState.Aim || levelState == LevelState.Power || levelState == LevelState.PowerUpAim)
            {
                shotManager.ActivePowerUpBarrel.TotalRotation = MathHelper.ToRadians(90.0f - firingAngle);

                #region UPDATE FRUIT BODY POSITION IN MOVING BARREL
                shotManager.shot.spriteBody.SetTransform(ConvertUnits.ToSimUnits(shotManager.ActivePowerUpBarrel.SpriteCenterInWorld), shotManager.ActivePowerUpBarrel.TotalRotation);
                shotManager.shot.Location = ConvertUnits.ToDisplayUnits(shotManager.shot.spriteBody.Position) + new Vector2 (-32,-32);
                if (Camera.IsScrolling)
                {
                    if (!Camera.AutoScroll) Camera.LookAt(firingLocation);
                    else
                    {
                        Camera.ScrollTo(firingLocation, 0);
                        Camera.IsScrolling = true;
                    }
                }
            
                #endregion
            }
            if (levelState == LevelState.Fire || levelState == LevelState.PowerUpFire)
            {
                shotManager.ActivePowerUpBarrel.TotalRotation = MathHelper.ToRadians(90.0f - firingAngle);
                if (Camera.IsScrolling)
                {
                    Camera.LookAt(shotLocation);
                }
            }
            Camera.Update(gameTime);

            switch (levelState)
            {
                case LevelState.Countdown:
                    {
                        #region UPDATE COUNTDOWN TIMER, CHECK TO GO LIVE
                        countdown -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        if (countdown < 0 || !LevelDataManager.levelData[world,level].safety)
                        {
                            shotManager.shot.spriteBody.Enabled = true;
                            shotManager.shot.spriteBody.IsSensor = true;
                            levelState = LevelState.Aim;
                        }
                        #endregion
                        break;
                    }
                case LevelState.Aim:
                    {

                        break;
                    }
                case LevelState.Power:
                    {
                        #region CHANGE POWER BAR FILL
                        if (isPowerFillDirectionUp) powerBarFill += powerBarFillRate * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        else powerBarFill -= powerBarFillRate * (float)gameTime.ElapsedGameTime.TotalSeconds;
                        #endregion

                        #region IF POWERBAR FULL, SWITCH FILL DIRECTION
                        if (powerBarFill >= 1.0f)
                        {
                            powerBarFill = 1.0f;
                            isPowerFillDirectionUp = false;
                        }
                        #endregion

                        #region CANCEL SHOT IF POWERBAR FALLS TO ZERO
                        if (powerBarFill <= 0.0f)
                        {
                            powerBarFill = 0.0f;
                            isPowerFillDirectionUp = true;
                            levelState = LevelState.Aim;
                        }
                        #endregion

                        break;
                    }

                case LevelState.Fire:
                    {
                        #region CHECK SHOT BOUNDS/MOVEMENT FOR END
                        if (shotManager.shot.IsVisible)
                        {
                            if(shotManager.shot.spriteBody.LinearVelocity == Vector2.Zero) shotManager.shot.IsHit = true; 

                            //apple jump exemption
                            if (shotManager.shot.TextureIndex < 21 && shotManager.shot.pathing != Sprite.Pathing.None)
                            {
                                shotManager.shot.IsHit = false;
                            }

                            //bounds check
                            if((Camera.WorldRectangle.Bottom + 200 < shotManager.shot.SpriteRectangle.Top) ||
                                 (Camera.WorldRectangle.Left - 200 > shotManager.shot.SpriteRectangle.Right) ||
                                 (Camera.WorldRectangle.Right + 200 < shotManager.shot.SpriteRectangle.Left))
                            {
                                shotManager.shot.IsHit = true;
                            }

                            //banana on bommerang exception
                            if (shotManager.shot.TextureIndex >= 42 && shotManager.shot.TextureIndex < 49 &&
                                shotManager.shot.spriteBody.LinearVelocity != Vector2.Zero && shotManager.shot.pathing != Sprite.Pathing.None)
                            {
                                    shotManager.shot.IsHit = false;
                            }
                            
                        }

                        if (shotManager.isSplit)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                if (shotManager.splitShot[i].IsVisible)
                                {
                                    if ((shotManager.splitShot[i].spriteBody.LinearVelocity == Vector2.Zero) ||
                                     (Camera.WorldRectangle.Bottom + 200 < shotManager.splitShot[i].SpriteRectangle.Top) ||
                                     (Camera.WorldRectangle.Left - 200 > shotManager.splitShot[i].SpriteRectangle.Right) ||
                                     (Camera.WorldRectangle.Right + 200 < shotManager.splitShot[i].SpriteRectangle.Left))
                                    {
                                        shotManager.splitShot[i].IsHit = true;
                                    }
                                }
                            }
                        }
                        #endregion
                        break;
                    }
                case LevelState.PowerUpAim:
                    {
                        break;
                    }
                case LevelState.PowerUpFire:
                    {
                        #region CHECK SHOT BOUNDS/MOVEMENT FOR END
                        if (shotManager.shot.IsVisible)
                        {
                            if (shotManager.shot.spriteBody.LinearVelocity == Vector2.Zero) shotManager.shot.IsHit = true;

                            //apple jump exemption
                            if (shotManager.shot.TextureIndex < 21 && shotManager.shot.pathing != Sprite.Pathing.None)
                            {
                                shotManager.shot.IsHit = false;
                            }

                            //bounds check
                            if ((Camera.WorldRectangle.Bottom + 200 < shotManager.shot.SpriteRectangle.Top) ||
                                 (Camera.WorldRectangle.Left - 200 > shotManager.shot.SpriteRectangle.Right) ||
                                 (Camera.WorldRectangle.Right + 200 < shotManager.shot.SpriteRectangle.Left))
                            {
                                shotManager.shot.IsHit = true;
                            }

                            //banana on bommerang exception
                            if (shotManager.shot.TextureIndex >= 42 && shotManager.shot.TextureIndex < 49 &&
                                shotManager.shot.spriteBody.LinearVelocity != Vector2.Zero && shotManager.shot.pathing != Sprite.Pathing.None)
                            {
                                shotManager.shot.IsHit = false;
                            }

                        }

                        if (shotManager.isSplit)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                if (shotManager.splitShot[i].IsVisible)
                                {
                                    if ((shotManager.splitShot[i].spriteBody.LinearVelocity == Vector2.Zero) ||
                                     (Camera.WorldRectangle.Bottom + 200 < shotManager.splitShot[i].SpriteRectangle.Top) ||
                                     (Camera.WorldRectangle.Left - 200 > shotManager.splitShot[i].SpriteRectangle.Right) ||
                                     (Camera.WorldRectangle.Right + 200 < shotManager.splitShot[i].SpriteRectangle.Left))
                                    {
                                        shotManager.splitShot[i].IsHit = true;
                                    }
                                }
                            }
                        }
                        #endregion
                        break;
                    }
                case LevelState.Win:
                    {
                        if (!isVictoryScreen)
                        {
                            if (GameSettings.isBoss && !enemyManager.isBossFailed)
                            {
                                fadeout -= 0.0075f;
                                SoundManager.SetVolume(fadeout);
                                if (fadeout < 0f)
                                {
                                    SoundManager.SetVolume(1f);
                                    SoundManager.StopMusic();    
                                }
                            }

                            if (Camera.ScoreTimedOut())
                            {
                                isVictoryScreen = true;
                                if (GameSettings.isBoss)
                                {
                                    if (enemyManager.boss.IsExpired)
                                    {
                                        SoundManager.SetVolume(1f);
                                        SoundManager.StopMusic();
                                        ScreenManager.AddScreen(new CreditRollScreen(), null);
                                        ExitScreen();
                                        return;
                                    }
                                    if (enemyManager.isBossFailed)
                                    { 
                                        ScreenManager.AddScreen(new VictoryScreen(shotManager.powerStarCollected, shotNumber), controllingPlayer);
                                        return;
                                    }
                                }
                                else
                                {
                                    ScreenManager.AddScreen(new VictoryScreen(shotManager.powerStarCollected, shotNumber), controllingPlayer);
                                }
                            }
                        }
                        break;
                    }
                default:
                    break;
            }

            //check end of shot
            if (shotManager.shot.IsExpired)
            {
                EndOfShot();
            }

            //check victory
            if (enemyManager.VeggieSpriteCount == 0) levelState = LevelState.Win;
            if (GameSettings.isBoss)
            {
                if ((enemyManager.boss.IsExpired && bossdeadFade >= 1f) || enemyManager.isBossFailed)
                {
                    levelState = LevelState.Win;
                }
            }
            return;
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            #region GET STATE OF INPUTS
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            controllingPlayer = ControllingPlayer.Value;
#if WINDOWS
            keyboardState = input.CurrentKeyboardStates[(int)controllingPlayer];
#endif
            gamePadState = input.CurrentGamePadStates[(int)controllingPlayer];
            #endregion

            #region PAUSE GAME
            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[(int)controllingPlayer];
            if (input.IsPauseGame(ControllingPlayer, out responsePlayer) || gamePadDisconnected)
            {
                //ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                ScreenManager.AddScreen(new MainOptionScreen(true), ControllingPlayer);
            }
            #endregion

            #region RESTART LEVEL ON Y
            if (input.IsYPressed(ControllingPlayer, out responsePlayer))
            {
                LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new GameplayScreen(LevelDataManager.world, LevelDataManager.level));
                ExitScreen();
            }
            #endregion

            #region CAMERA ZOOM
            if (gamePadState.IsButtonDown(Buttons.LeftShoulder))
            {
                if (Camera.zoom > 0.6f)
                {
                    Camera.zoom = (Math.Max(Camera.zoom - 0.008f, 0.6f));
                    Camera.ZoomTo(Camera.zoom);
                    Camera.IsScrolling = false;
                    Camera.AutoScroll = false;
                }
            }
            //if (input.IsLShoulderPressed(controllingPlayer, out responsePlayer))
            //{
            //    if (Camera.zoom - 0.1f < 0.6f) Camera.ZoomTo(0.6f);
            //    else Camera.ZoomTo(Camera.zoom - 0.1f);
            //    Camera.IsScrolling = false;
            //    Camera.AutoScroll = false;
            //}

            if (gamePadState.IsButtonDown(Buttons.RightShoulder))
            {
                if (Camera.zoom < 1f)
                {
                    Camera.zoom = (Math.Min(Camera.zoom + 0.008f,1f));
                    Camera.ZoomTo(Camera.zoom);
                    Camera.IsScrolling = false;
                    Camera.AutoScroll = false;
                }
            }
            //if (input.IsRShoulderPressed(controllingPlayer, out responsePlayer))
            //{
            //    if (Camera.zoom + 0.1f > 1f) Camera.ZoomTo(1f);
            //    else Camera.ZoomTo(Camera.zoom + 0.1f);
            //     Camera.IsScrolling = false;
            //    Camera.AutoScroll = false;
            //}
            #endregion

            #region CAMERA MOVEMENT
            cameraMovement = Vector2.Zero;
#if WINDOWS
            if (keyboardState.IsKeyDown(Keys.Left))
                cameraMovement.X--;

            if (keyboardState.IsKeyDown(Keys.Right))
                cameraMovement.X++;

            if (keyboardState.IsKeyDown(Keys.Up))
                cameraMovement.Y--;

            if (keyboardState.IsKeyDown(Keys.Down))
                cameraMovement.Y++;
#endif
            cameraMovement += new Vector2(gamePadState.ThumbSticks.Right.X,-gamePadState.ThumbSticks.Right.Y);
            if(cameraMovement != Vector2.Zero)
            {
                Camera.IsScrolling = false;
                Camera.AutoScroll = false;
                Camera.Move(cameraMovement * 20f);
            }
            #endregion
            #region CLICK R STICK TO RESET CAMERA
            if(gamePadState.IsButtonDown(Buttons.RightStick))
            {
                Camera.ZoomTo(1f);
                Camera.ScrollTo(shotManager.shot.Location, 0);
                Camera.IsScrolling = true;
            }
            #endregion

            //if (keyboardState.IsKeyDown(Keys.U)) blurAlpha += 0.01f;  //TESTING
            //if (keyboardState.IsKeyDown(Keys.I)) blurAlpha -= 0.01f;  //TESTING
            //MathHelper.Clamp(blurAlpha, 0f, 1f);  //TESTINIG

            switch (levelState)
            {
                case LevelState.Intro:
                    {
                        if (input.IsAPressed(ControllingPlayer, out responsePlayer))
                        {
                            introManager.AisPressed();
                        }

                        if (input.IsBPressed(ControllingPlayer, out responsePlayer))
                        {
                            introManager.Finish();
                        }

                        break;
                    }
                case LevelState.Countdown:
                case LevelState.Aim:
                    {

                        #region CHANGE AMMO/POWERUP DPAD OR FTGH
                        //if (input.IsDpadLeftPressed(controllingPlayer, out responsePlayer)) shotManager.ChangeSelectedAmmo(-1);
                        if (input.IsXPressed(controllingPlayer, out responsePlayer)) shotManager.ChangeSelectedAmmo(1);
                        //if (input.IsYPressed(controllingPlayer, out responsePlayer)) shotManager.ChangePowerBarrel(1);
                        //if (input.IsYPressed(controllingPlayer, out responsePlayer)) isAimLocked = !isAimLocked;
                        //if (input.IsDpadDownPressed(controllingPlayer, out responsePlayer)) shotManager.ChangePowerBarrel(-1);
                        #endregion

                        #region FIRING POWER
#if WINDOWS
                        if (keyboardState.IsKeyDown(Keys.D))
                        {
                            firingPower += (GameSettings.MaxFiringPower - GameSettings.MinFiringPower) *0.02f;
                            firingPower = MathHelper.Clamp(firingPower, GameSettings.MinFiringPower, GameSettings.MaxFiringPower);
                        }
                        if (keyboardState.IsKeyDown(Keys.A))
                        {
                            firingPower -= (GameSettings.MaxFiringPower - GameSettings.MinFiringPower) *0.02f;
                            firingPower = MathHelper.Clamp(firingPower, GameSettings.MinFiringPower, GameSettings.MaxFiringPower);
                        }
#endif
                        if (gamePadState.IsButtonDown(Buttons.RightTrigger))
                        {
                            firingPower += (GameSettings.MaxFiringPower - GameSettings.MinFiringPower) * 0.02f;
                            firingPower = MathHelper.Clamp(firingPower, GameSettings.MinFiringPower, GameSettings.MaxFiringPower);
                        }
                        if (gamePadState.IsButtonDown(Buttons.LeftTrigger))
                        {
                            firingPower -= (GameSettings.MaxFiringPower - GameSettings.MinFiringPower) * 0.02f;
                            firingPower = MathHelper.Clamp(firingPower, GameSettings.MinFiringPower, GameSettings.MaxFiringPower);
                        }
                        if (gamePadState.IsButtonDown(Buttons.DPadUp))
                        {
                            firingPower++;
                            firingPower = MathHelper.Clamp(firingPower, GameSettings.MinFiringPower, GameSettings.MaxFiringPower);
                        }
                        if (gamePadState.IsButtonDown(Buttons.DPadDown))
                        {
                            firingPower--;
                            firingPower = MathHelper.Clamp(firingPower, GameSettings.MinFiringPower, GameSettings.MaxFiringPower);
                        }

                        #endregion

                        #region FIRING ANGLE/DIRECTION
#if WINDOWS
                        if (keyboardState.IsKeyDown(Keys.S)) firingAngle--;
                        if (keyboardState.IsKeyDown(Keys.W)) firingAngle++;
#endif
                        if (gamePadState.IsButtonDown(Buttons.DPadLeft)) firingAngle += 0.2f;
                        if (gamePadState.IsButtonDown(Buttons.DPadRight)) firingAngle -= 0.2f;
                        firingDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(firingAngle)), (float)-Math.Sin(MathHelper.ToRadians(firingAngle)));

                        thumbstickL = Vector2.Zero;
                        thumbstickL = gamePadState.ThumbSticks.Left;
                        if (thumbstickL != Vector2.Zero)
                        {
                            if (thumbstickL.Length() >= lastThumbstick.Length() - 0.1f)
                            {
                                lastThumbstick = thumbstickL;
                                thumbstickL.Normalize();
                                firingDirection = thumbstickL * invertYThumbstick;
                                if (firingDirection.Y <= 0) firingAngle = MathHelper.ToDegrees((float)Math.Acos(firingDirection.X));
                                if (firingDirection.Y > 0) firingAngle = -MathHelper.ToDegrees((float)Math.Acos(firingDirection.X));
                            }
                        }
                        #endregion

                        #region max power for powerup/relay
                        if (shotManager.ActivePowerUpBarrel.TextureIndex == 4 || shotManager.ActivePowerUpBarrel.TextureIndex == 6 ||
                            shotManager.ActivePowerUpBarrel.TextureIndex == 7 || shotManager.ActivePowerUpBarrel.TextureIndex == 8)
                        {
                            //normal 
                        }
                        else
                        {
                            firingPower = GameSettings.MaxFiringPower;
                        }
                        #endregion

                        #region CALCULATE POSITIONS FOR POWER BAR AND SHOT AIM PATH
                        firingForce = firingDirection * firingPower;
                        barrelEndLocation = new Vector2(firingLocation.X + (firingDirection.X * 96f), firingLocation.Y + (firingDirection.Y * 96f));
                        powerTargetFill = (firingPower - powerMod)/powerSlope;
                        powerTargetLocation = new Vector2((int)(powerBarLocation.X - 24 + (powerTargetFill * powerBar.Width)), powerBarLocation.Y - 36);
                        #endregion

                        #region ADVANCE GAME STATE (AND INITIALIZE POWER FILL) ON "A"
                        if (input.IsAPressed(ControllingPlayer, out responsePlayer))
                        {
                            if (countdown < 5 || levelState == LevelState.Aim)
                            {
                                lastThumbstick = Vector2.Zero;
                                powerBarFillRate = firingPower / 1500f;
                                powerBarFill = 0f;

                                if (levelState == LevelState.Countdown)
                                {
                                    countdown = -0.5f;
                                    shotManager.shot.spriteBody.Enabled = true;
                                }

                                levelState = LevelState.Power;
                            }
                        }
                        #endregion
                        break;
                    }
                case LevelState.Power:
                    {
                        #region CALCULATE POSITIONS FOR POWER BAR AND SHOT AIM PATH
                        firingForce = firingDirection * firingPower;
                        barrelEndLocation = new Vector2(firingLocation.X + (firingDirection.X * 96f), firingLocation.Y + (firingDirection.Y * 96f));
                        powerTargetFill = (firingPower - powerMod) / powerSlope;
                        powerTargetLocation = new Vector2((int)(powerBarLocation.X - 24 + (powerTargetFill * powerBar.Width)), powerBarLocation.Y - 36);
                        #endregion

                        //these barrels use the power bar
                        if (shotManager.ActivePowerUpBarrel.TextureIndex == 4 || shotManager.ActivePowerUpBarrel.TextureIndex == 6 ||
                            shotManager.ActivePowerUpBarrel.TextureIndex == 7 || shotManager.ActivePowerUpBarrel.TextureIndex == 8)
                        {
                            if (input.IsAPressed(ControllingPlayer, out responsePlayer))
                            {
                                powerSetBarFill = powerBarFill;
                                powerSetLocation = new Vector2((int)(powerBarLocation.X - 24 + (powerSetBarFill * powerBar.Width)), powerBarLocation.Y - 36);

                                //test of angle change on inaccuracy
                                actualPower = firingPower;
                                float misfireDirection = LevelDataManager.rand.Next(0,2);
                                if (misfireDirection == 0) misfireDirection = -1;
                                //if (shotManager.selectedAmmo == 0) misfireDirection *= 0.5f; //apple is twice as accurate
                                if (GameSettings.CheatHardMode) misfireDirection *= 2.0f;  //cheat
                                float actualFiringAngle = firingAngle + (misfireDirection * misfireAngle * (Math.Max(Math.Abs(powerTargetFill - powerSetBarFill)-0.025f,0))); //5% tolerance on firing
                                
                                //apply cheat
                                if (GameSettings.CheatSharpshooter) actualFiringAngle = firingAngle;

                                firingForce = new Vector2((float)Math.Cos(MathHelper.ToRadians(actualFiringAngle)), (float)-Math.Sin(MathHelper.ToRadians(actualFiringAngle))) * actualPower;
                                barrelEndLocation = new Vector2(firingLocation.X + (firingForce.X / actualPower) * 96, firingLocation.Y + (firingForce.Y / actualPower) * 96);
                                shotManager.ActivePowerUpBarrel.TotalRotation = MathHelper.ToRadians(90.0f - actualFiringAngle);

                                shotManager.CreateFruitShot(barrelEndLocation, physicsWorld);
                                if (shotManager.ActivePowerUpBarrel.TextureIndex != 6)
                                {
                                    shotManager.CreatePowerUpShot(shotManager.shot, barrelEndLocation, physicsWorld, ConvertUnits.ToSimUnits(firingForce));
                                }

                                //perfect shot effects
                                if (actualFiringAngle == firingAngle) contactListener.ShotWasPerfect(shotManager.shot, shotManager.ActivePowerUpBarrel);

                                //for fruit, tnt, cannonball generate rotation based on misfire level
                                if (shotManager.ActivePowerUpBarrel.TextureIndex == 6 || shotManager.ActivePowerUpBarrel.TextureIndex == 4 ||
                                    shotManager.ActivePowerUpBarrel.TextureIndex == 7 || shotManager.ActivePowerUpBarrel.TextureIndex == 8 || shotManager.ActivePowerUpBarrel.TextureIndex == 0)
                                {
                                    shotManager.shot.spriteBody.AngularVelocity = (powerTargetFill - powerSetBarFill) * -100f;
                                    shotManager.shot.spriteBody.AngularVelocity = Math.Max(-20, shotManager.shot.spriteBody.AngularVelocity);
                                    shotManager.shot.spriteBody.AngularVelocity = Math.Min(20, shotManager.shot.spriteBody.AngularVelocity);
                                }


                                if (GameSettings.CheatFaster) firingForce *= 2.0f; //cheat
                                shotManager.shot.spriteBody.LinearVelocity = ConvertUnits.ToSimUnits(firingForce);
                                
                                contactListener.FireShot(barrelEndLocation, shotManager.ActivePowerUpBarrel);
                                shotNumber += 1;
                                if (shotNumber > LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].bronze) isFailed = true;
                                Camera.IsScrolling = true;
                                levelState = LevelState.Fire;
                                break;
                            }
                            if (input.IsBPressed(ControllingPlayer, out responsePlayer))
                            {
                                powerBarFill = 0f;
                                levelState = LevelState.Aim;
                                break;
                            }
                        }
                        else 
                        {
                                actualPower = GameSettings.MaxFiringPower;

                                firingForce = new Vector2((float)Math.Cos(MathHelper.ToRadians(firingAngle)), (float)-Math.Sin(MathHelper.ToRadians(firingAngle))) * actualPower;
                                barrelEndLocation = new Vector2(firingLocation.X + (firingForce.X / actualPower) * 96, firingLocation.Y + (firingForce.Y / actualPower) * 96);
                                shotManager.ActivePowerUpBarrel.TotalRotation = MathHelper.ToRadians(90.0f - firingAngle);

                                shotManager.CreateFruitShot(barrelEndLocation,physicsWorld);
                                if (shotManager.ActivePowerUpBarrel.TextureIndex != 6) shotManager.CreatePowerUpShot(shotManager.shot, barrelEndLocation, physicsWorld, ConvertUnits.ToSimUnits(firingForce));

                                if (GameSettings.CheatFaster) firingForce *= 2.0f; //cheat
                                shotManager.shot.spriteBody.LinearVelocity = ConvertUnits.ToSimUnits(firingForce);

                                contactListener.FireShot(barrelEndLocation, shotManager.ActivePowerUpBarrel);
                                shotNumber += 1;
                                if (shotNumber > LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].bronze) isFailed = true;
                                Camera.IsScrolling = true;
                                levelState = LevelState.Fire;
                            break;
                        }
                        break;
                    }
  
                case LevelState.Fire:
                    {
                        // Press A to activate shot ability
                        if (input.IsAPressed(ControllingPlayer, out responsePlayer))
                        {
                            #region shot ability
                            //if not already activated
                            if (shotManager.shot.IsVisible && shotManager.shot.pathingTravelled == 0f && shotManager.shot.TextureID == 20 && shotManager.shot.spriteBody.LinearVelocity.Length() > 1f)
                            {
                                shotManager.shot.pathingTravelled = 0.001f;//ability now used and wont pass check again
                                int spriteRow = shotManager.shot.TextureIndex / LevelDataManager.SpritesInRow(shotManager.shot);
                                switch (spriteRow)
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                        {
                                            //apple
                                            if (GameSettings.isAppleJump)
                                            {
                                                shotManager.isAppleCopter = true;
                                                shotManager.shot.pathing = Sprite.Pathing.Linear;
                                                shotManager.shot.PathingSpeed = 0;
                                                shotManager.shot.spriteBody.ResetDynamics();
                                                shotManager.shot.spriteBody.ApplyLinearImpulse(new Vector2(0, -6));
                                                contactListener.DoPoof(shotManager.shot);
                                                shotManager.shot.spriteBody.AngularVelocity = 0;
                                                shotManager.shot.spriteBody.SetTransform(shotManager.shot.spriteBody.Position, 0f);                      
                                            }
                                            break;
                                        }
                                    case 3:
                                        {
                                            //orange
                                            shotManager.shot.pathing = Sprite.Pathing.Linear;
                                            shotManager.shot.spriteBody.LinearVelocity.Normalize();
                                            shotManager.shot.PathingRadiusY = (int)shotManager.shot.spriteBody.LinearVelocity.Y * 100000;
                                            shotManager.shot.PathingRadiusX = (int)shotManager.shot.spriteBody.LinearVelocity.X * 100000;
                                            shotManager.shot.PathingSpeed = 2000;
                                            shotManager.shot.spriteBody.ResetDynamics();
                                            shotManager.shot.spriteBody.LinearVelocity = ConvertUnits.ToSimUnits(new Vector2(10f, 10f)); //so it isnt motionless in physics and shot ends
                                            shotManager.shot.spriteBody.IgnoreGravity = true;
                                            shotManager.shot.InitializePathing();
                                            break;
                                        }
                                    case 4:
                                        {
                                            //strawberry
                                            shotManager.shot.spriteBody.Rotation = 0;
                                            shotManager.shot.spriteBody.AngularVelocity = 0;
                                            shotManager.shot.pathing = Sprite.Pathing.Linear;
                                            shotManager.shot.PathingRadiusY = 100000;
                                            shotManager.shot.PathingRadiusX = 0;
                                            shotManager.shot.PathingSpeed = 2000;
                                            shotManager.shot.spriteBody.ResetDynamics();
                                            shotManager.shot.spriteBody.LinearVelocity = ConvertUnits.ToSimUnits(new Vector2(10f, 10f)); //so it isnt motionless in physics and shot ends
                                            shotManager.shot.spriteBody.IgnoreGravity = true;
                                            shotManager.shot.InitializePathing();
                                            break;
                                        }
                                    case 5:
                                        {
                                            //cherry
                                            shotManager.CreateSplitShot(physicsWorld, PlayArea);
                                            break;
                                        }
                                    case 6:
                                        {
                                            contactListener.ActivateBanana(shotManager.shot);
                                            break;
                                        }
                                    case 7:
                                        {
                                            contactListener.ActivateLemon(shotManager.shot);
                                            break;
                                        }
                                    case 8:
                                        {
                                            //watermelon
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }//end if not already activated
                            #endregion
                        }//end if a pressed

                        if (input.IsBPressed(ControllingPlayer, out responsePlayer))
                        {
                            if (shotManager.shot.IsVisible) shotManager.shot.IsHit = true;
                            //if (shotManager.shot.TextureIndex == 35)
                            if(shotManager.isSplit)
                            {
                                if (shotManager.splitShot[0].spriteBody != null && shotManager.splitShot[0].IsVisible) shotManager.splitShot[0].IsHit = true;
                                if (shotManager.splitShot[1].spriteBody != null && shotManager.splitShot[1].IsVisible) shotManager.splitShot[1].IsHit = true;
                                if (shotManager.splitShot[2].spriteBody != null && shotManager.splitShot[2].IsVisible) shotManager.splitShot[2].IsHit = true;
                            }
                        }

                        #region if apple jump ability active
                        if (shotManager.isAppleCopter)
                        {
                            thumbstickL = Vector2.Zero;
                            thumbstickL = gamePadState.ThumbSticks.Left;
                            if (thumbstickL != Vector2.Zero)
                            {
                                thumbstickL.Normalize();
                                if (thumbstickL.Y > 0) thumbstickL = new Vector2(thumbstickL.X, 0);
                                thumbstickL *= invertYThumbstick;
                                if (shotManager.shot.Scale != 1f) thumbstickL *= 4f;      
                            }
                            shotManager.shot.spriteBody.ApplyLinearImpulse(new Vector2(thumbstickL.X * 0.4f, 0f));
                            //terminal velocity
                            shotManager.shot.spriteBody.LinearVelocity = new Vector2(MathHelper.Clamp(shotManager.shot.spriteBody.LinearVelocity.X, -12f, 12f), MathHelper.Clamp(shotManager.shot.spriteBody.LinearVelocity.Y, -12f, 2.5f));
                        }
                        #endregion

                        break;
                    }
                case LevelState.PowerUpAim:
                    {
                        #region FIRING ANGLE/DIRECTION
#if WINDOWS
                        if (keyboardState.IsKeyDown(Keys.S)) firingAngle--;
                        if (keyboardState.IsKeyDown(Keys.W)) firingAngle++;
#endif
                        if (gamePadState.IsButtonDown(Buttons.DPadLeft)) firingAngle += 0.2f;
                        if (gamePadState.IsButtonDown(Buttons.DPadRight)) firingAngle -= 0.2f;
                        firingDirection = new Vector2((float)Math.Cos(MathHelper.ToRadians(firingAngle)), (float)-Math.Sin(MathHelper.ToRadians(firingAngle)));

                        thumbstickL = Vector2.Zero;
                        thumbstickL = gamePadState.ThumbSticks.Left;
                        if (thumbstickL != Vector2.Zero)
                        {
                            if (thumbstickL.Length() >= lastThumbstick.Length() - 0.1f)
                            {
                                lastThumbstick = thumbstickL;
                                thumbstickL.Normalize();
                                firingDirection = thumbstickL * invertYThumbstick;
                                if (firingDirection.Y <= 0) firingAngle = MathHelper.ToDegrees((float)Math.Acos(firingDirection.X));
                                if (firingDirection.Y > 0) firingAngle = -MathHelper.ToDegrees((float)Math.Acos(firingDirection.X));
                            }
                        }
                        #endregion

                        firingPower = GameSettings.MaxFiringPower;
                        firingForce = firingDirection * firingPower;
                        barrelEndLocation = new Vector2(shotManager.ActivePowerUpBarrel.SpriteCenterInWorld.X + (firingDirection.X * 96f), shotManager.ActivePowerUpBarrel.SpriteCenterInWorld.Y + (firingDirection.Y * 96f));
                        
                        if (input.IsAPressed(ControllingPlayer, out responsePlayer))
                        {
                            actualPower = firingPower;
                            firingForce = new Vector2((float)Math.Cos(MathHelper.ToRadians(firingAngle)), (float)-Math.Sin(MathHelper.ToRadians(firingAngle))) * actualPower;
                            barrelEndLocation = new Vector2(shotManager.ActivePowerUpBarrel.SpriteCenterInWorld.X + (firingForce.X / actualPower) * 96, shotManager.ActivePowerUpBarrel.SpriteCenterInWorld.Y + (firingForce.Y / actualPower) * 96);
                            shotManager.ActivePowerUpBarrel.TotalRotation = MathHelper.ToRadians(90 - firingAngle);
                            shotManager.CreatePowerUpShot(shotManager.shot, barrelEndLocation, physicsWorld, ConvertUnits.ToSimUnits(firingForce));
                            
                            //for fruit, tnt, cannonball rotation
                            if (shotManager.ActivePowerUpBarrel.TextureIndex == 6 || shotManager.ActivePowerUpBarrel.TextureIndex == 4 ||
                                shotManager.ActivePowerUpBarrel.TextureIndex == 7 || shotManager.ActivePowerUpBarrel.TextureIndex == 8 || shotManager.ActivePowerUpBarrel.TextureIndex == 0)
                            {
                                shotManager.shot.spriteBody.AngularVelocity = LevelDataManager.rand.Next(-100, 100) * 0.1f;
                            }

                            if (GameSettings.CheatFaster) firingForce *= 2.0f; //cheat
                            shotManager.shot.spriteBody.LinearVelocity = ConvertUnits.ToSimUnits(firingForce);
                            contactListener.FireShot(barrelEndLocation,shotManager.ActivePowerUpBarrel);
                            Camera.IsScrolling = true;
                            lastThumbstick = Vector2.Zero;
                            levelState = LevelState.PowerUpFire;
                        }

                        break;
                    }
                case LevelState.PowerUpFire:
                    {
                        // Press A to activate shot ability
                        if (input.IsAPressed(ControllingPlayer, out responsePlayer))
                        {
                            #region shot ability
                            //if shot visible, not acivated, is a fruit and is minimum speed
                            if (shotManager.shot.IsVisible && shotManager.shot.pathingTravelled == 0f && shotManager.shot.TextureID == 20 && shotManager.shot.spriteBody.LinearVelocity.Length() > 1f)
                            {
                                shotManager.shot.pathingTravelled = 0.001f;//ability now used and wont pass check again
                                int spriteRow = shotManager.shot.TextureIndex / LevelDataManager.SpritesInRow(shotManager.shot);
                                switch (spriteRow)
                                {
                                    case 0:
                                    case 1:
                                    case 2:
                                        {
                                            //apple
                                            if (GameSettings.isAppleJump)
                                            {
                                                shotManager.isAppleCopter = true;
                                                shotManager.shot.pathing = Sprite.Pathing.Linear;
                                                shotManager.shot.PathingSpeed = 0;
                                                shotManager.shot.spriteBody.ResetDynamics();
                                                shotManager.shot.spriteBody.ApplyLinearImpulse(new Vector2(0, -6));
                                                contactListener.DoPoof(shotManager.shot);
                                                shotManager.shot.spriteBody.AngularVelocity = 0;
                                                shotManager.shot.spriteBody.SetTransform(shotManager.shot.spriteBody.Position, 0f);
                                            }
                                            break;
                                        }
                                    case 3:
                                        {
                                            //orange
                                            shotManager.shot.pathing = Sprite.Pathing.Linear;
                                            shotManager.shot.spriteBody.LinearVelocity.Normalize();
                                            shotManager.shot.PathingRadiusY = (int)shotManager.shot.spriteBody.LinearVelocity.Y * 100000;
                                            shotManager.shot.PathingRadiusX = (int)shotManager.shot.spriteBody.LinearVelocity.X * 100000;
                                            shotManager.shot.PathingSpeed = 2000;
                                            shotManager.shot.spriteBody.ResetDynamics();
                                            shotManager.shot.spriteBody.LinearVelocity = ConvertUnits.ToSimUnits(new Vector2(10f, 10f)); //so it isnt motionless in physics and shot ends
                                            shotManager.shot.spriteBody.IgnoreGravity = true;
                                            shotManager.shot.InitializePathing();
                                            break;
                                        }
                                    case 4:
                                        {
                                            //strawberry
                                            shotManager.shot.spriteBody.Rotation = 0;
                                            shotManager.shot.spriteBody.AngularVelocity = 0;
                                            shotManager.shot.pathing = Sprite.Pathing.Linear;
                                            shotManager.shot.PathingRadiusY = 100000;
                                            shotManager.shot.PathingRadiusX = 0;
                                            shotManager.shot.PathingSpeed = 2000;
                                            shotManager.shot.spriteBody.ResetDynamics();
                                            shotManager.shot.spriteBody.LinearVelocity = ConvertUnits.ToSimUnits(new Vector2(10f, 10f)); //so it isnt motionless in physics and shot ends
                                            shotManager.shot.spriteBody.IgnoreGravity = true;
                                            shotManager.shot.InitializePathing();
                                            break;
                                        }
                                    case 5:
                                        {
                                            //cherry
                                            shotManager.CreateSplitShot(physicsWorld, PlayArea);
                                            break;
                                        }
                                    case 6:
                                        {
                                            contactListener.ActivateBanana(shotManager.shot);
                                            break;
                                        }
                                    case 7:
                                        {
                                            contactListener.ActivateLemon(shotManager.shot);
                                            break;
                                        }
                                    case 8:
                                        {
                                            //watermelon
                                            break;
                                        }
                                    default:
                                        break;
                                }
                            }//end if not already activated
                            #endregion
                        }//end if a pressed

                        if (input.IsBPressed(ControllingPlayer, out responsePlayer))
                        {
                            if (shotManager.shot.IsVisible) shotManager.shot.IsHit = true;
                            //if (shotManager.shot.TextureIndex == 35)
                            if(shotManager.isSplit)
                            {
                                if (shotManager.splitShot[0].spriteBody != null && shotManager.splitShot[0].IsVisible) shotManager.splitShot[0].IsHit = true;
                                if (shotManager.splitShot[1].spriteBody != null && shotManager.splitShot[1].IsVisible) shotManager.splitShot[1].IsHit = true;
                                if (shotManager.splitShot[2].spriteBody != null && shotManager.splitShot[2].IsVisible) shotManager.splitShot[2].IsHit = true;
                            }
                        }

                        #region if apple jump ability active
                        if (shotManager.isAppleCopter)
                        {
                            thumbstickL = Vector2.Zero;
                            thumbstickL = gamePadState.ThumbSticks.Left;
                            if (thumbstickL != Vector2.Zero)
                            {
                                thumbstickL.Normalize();
                                if (thumbstickL.Y > 0) thumbstickL = new Vector2(thumbstickL.X, 0);
                                thumbstickL *= invertYThumbstick;
                                if (shotManager.shot.Scale != 1f) thumbstickL *= 4f;
                            }
                            shotManager.shot.spriteBody.ApplyLinearImpulse(new Vector2(thumbstickL.X * 0.4f, 0f));
                            //terminal velocity
                            shotManager.shot.spriteBody.LinearVelocity = new Vector2(MathHelper.Clamp(shotManager.shot.spriteBody.LinearVelocity.X, -12f, 12f), MathHelper.Clamp(shotManager.shot.spriteBody.LinearVelocity.Y, -12f, 2.5f));
                        }
                        #endregion

                        break;
                    }
                case LevelState.Win:
                    {


                        break;
                    }
                default:
                    break;
            }

            //check end of shot
            if (shotManager.shot.IsExpired)
            {
                if (input.IsAPressed(ControllingPlayer, out responsePlayer)) EndOfShot();
            }            
        }

        public override void Draw(GameTime gameTime)
        {
        
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.White, 0, 0);
            

            ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget); //aa
            if (shotManager.isAppleCopter) shotManager.shot.IsVisible = false; //comes back true in DrawAppleCopter, this prevents the shot draw in pEngine
            parallaxEngine.Draw(gameTime, spriteBatch);   
            effectManager.Draw(gameTime, spriteBatch, ScreenManager.smallFont);
            if (isAppleJack) DrawAppleJack(gameTime, spriteBatch);
            if (shotManager.isAppleCopter) DrawAppleCopter(gameTime, spriteBatch);
            decoManager.DrawWeather(gameTime, spriteBatch);
            ScreenManager.GraphicsDevice.SetRenderTarget(null); //aa

            if (GameSettings.MultiSampling)
            {
                ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget2); //aa
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null); //aa
                spriteBatch.Draw(renderTarget, new Rectangle(0,0,renderTarget2.Width,renderTarget2.Height), Color.White); //aa
                spriteBatch.End(); //aa
                ScreenManager.GraphicsDevice.SetRenderTarget(null); //aa

                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null); //aa
                spriteBatch.Draw(renderTarget, ScreenManager.GraphicsDevice.Viewport.Bounds, Color.White); //aa
                spriteBatch.End(); //aa

                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null); //aa
                spriteBatch.Draw(renderTarget2, ScreenManager.GraphicsDevice.Viewport.Bounds, Color.White*blurAlpha); //aa
                spriteBatch.End(); //aa
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null); //aa
                spriteBatch.Draw(renderTarget, ScreenManager.GraphicsDevice.Viewport.Bounds, Color.White); //aa
                spriteBatch.End(); //aa
            }

            #region Draw Debug view
            if (GameSettings.DebugViewEnabled)
            {
                Matrix proj = Matrix.CreateOrthographicOffCenter(0f, Camera.ViewportWidth, Camera.ViewportHeight, 0f, 0f, 1f);
                Matrix view2 = Matrix.CreateScale(physicsScale) * Camera.GetDebugMatrix(Vector2.One);
                oDebugView.RenderDebugData(ref proj, ref view2);
            }
            #endregion


            #region Draw UI stuff
            spriteBatch.Begin();
          
            #region Draw Countdown Timer
            if (levelState == LevelState.Countdown)
            {
                countdownInt = (int)countdown + 1;
                countdownStr = countdownInt.ToString();
                if (countdownInt <= 5) DrawStringHelper(spriteBatch, ScreenManager.largeFont, countdownStr, new Vector2(Camera.ViewportWidth * 0.5f - (ScreenManager.largeFont.MeasureString(countdownStr).X)*0.6f, 200), Color.White, Vector2.Zero, 1.2f, SpriteEffects.None, TransitionAlpha);
                else DrawStringHelper(spriteBatch, ScreenManager.largeFont, "READY", new Vector2(Camera.ViewportWidth * 0.5f - (ScreenManager.largeFont.MeasureString("READY").X)*0.6f, 200), Color.White, Vector2.Zero, 1.2f, SpriteEffects.None, TransitionAlpha);
            }
            #endregion

            #region Draw Shot Path
            if (levelState != LevelState.Fire && levelState != LevelState.PowerUpFire && levelState != LevelState.Win)
            {
                if (!GameSettings.CheatBlindfold)
                {
                    if(IsActive) preciseTimePellet += (float)gameTime.ElapsedGameTime.TotalSeconds *0.5f;
                    if (preciseTimePellet > 0.34f) preciseTimePellet -= 0.34f;

                    for (float i = -.07f; i < 6; i += 0.34f)
                    {
                        DrawShotPath(spriteBatch, preciseTimePellet + i, 8, GameSettings.ShotPathColor * 0.4f);
                    }
                }
            }
            #endregion

            if (!GameSettings.isBoss)
            {
                DrawPowerBar(spriteBatch);
                DrawShotWindowUI(gameTime, spriteBatch);
            }
            DrawAmmoUI(gameTime, spriteBatch);
            DrawShotCursor(spriteBatch);

            #region Draw Title safe area
            //if (GameSettings.TitleSafe)
            //{
            //    spriteBatch.Draw(pixel, new Rectangle(0, 0, 128, 720), Color.Red * 0.5f);
            //    spriteBatch.Draw(pixel, new Rectangle(1280 - 128, 0, 128, 720), Color.Red * 0.5f);
            //    spriteBatch.Draw(pixel, new Rectangle(0, 0, 1280, 72), Color.Red * 0.5f);
            //    spriteBatch.Draw(pixel, new Rectangle(0, 720 - 72, 1280, 72), Color.Red * 0.5f);
            //}
            #endregion

            #region Draw debug view
            //if (GameSettings.DebugViewEnabled)
            //{
            //    TextOverlayDraw(gameTime, spriteBatch);
            //    DrawFPSCounter(gameTime, spriteBatch);
            //}
            #endregion

            spriteBatch.End();
            #endregion

            if (levelState == LevelState.Intro) introManager.Draw(gameTime, spriteBatch);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                ScreenManager.FadeBackBufferToBlack(MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha*0.5f));
            }
            if (GameSettings.isBoss)
            {
                if (enemyManager.boss != null)
                {
                    if (enemyManager.boss.IsExpired)
                    {
                        bossdeadFade += 0.01f;
                        spriteBatch.Begin();
                        spriteBatch.Draw(pixel, ScreenManager.GraphicsDevice.Viewport.Bounds, null, Color.Black * bossdeadFade);
                        spriteBatch.End();
                    }
                }
            }

            lastState = levelState;
        }

        #region DRAW METHODS
        private void DrawAppleCopter(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            apple.TextureIndex = (int)(shotManager.shot.TextureIndex / 7) * 6;
            apple.Location = Camera.WorldToScreen(new Vector2(shotManager.shot.Location.X - 4, shotManager.shot.Location.Y - 4));
            apple.IsAnimated = true;
            apple.CurrentFrame = (int)MathHelper.Clamp((int)(shotManager.appleCopterTimer * 0.024f),0,5);
            apple.Scale = Camera.zoom * shotManager.shot.Scale;
            apple.Location += new Vector2(-32 + (Camera.zoom * 32), -32 + (Camera.zoom * 32));
            apple.Draw(gameTime, spriteBatch);
            spriteBatch.End();
            shotManager.shot.IsVisible = true;
        }

        private void DrawAppleJack(GameTime gameTime, SpriteBatch spriteBatch)
        {   
            spriteBatch.Begin();
            spriteBatch.Draw(appleJack, Camera.WorldToScreen(new Rectangle((int)appleJackPos.X,(int)appleJackPos.Y,(int)(64*Camera.zoom),(int)(128*Camera.zoom))),
                             null,Color.White, 0f,Vector2.Zero,SpriteEffects.FlipHorizontally,1f);
            spriteBatch.End();
        }

        private void DrawPowerBar(SpriteBatch spriteBatch)
        {
            switch (levelState)
            {
                case LevelState.Countdown:
                case LevelState.Aim:
                    {
                        if (shotManager.ActivePowerUpBarrel.TextureIndex == 0 || shotManager.ActivePowerUpBarrel.TextureIndex == 4 || shotManager.ActivePowerUpBarrel.TextureIndex == 6 ||
                            shotManager.ActivePowerUpBarrel.TextureIndex == 7 || shotManager.ActivePowerUpBarrel.TextureIndex == 8)
                        {
                            spriteBatch.Draw(powerFrame, powerBarLocation, null, Color.White*0.5f, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                            spriteBatch.Draw(powerBar, powerBarLocation, new Rectangle(0, 0, (int)(powerBarFill * powerBar.Width), powerBar.Height), Color.White * 0.5f, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                            spriteBatch.Draw(powerMarkerTarget, powerTargetLocation, null, Color.White * 0.5f, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                            spriteBatch.Draw(A, powerBarLocation-new Vector2(16,12), Color.White);
                        }
                        break;
                    }
                case LevelState.Power:
                    {
                        if (shotManager.ActivePowerUpBarrel.TextureIndex == 0 || shotManager.ActivePowerUpBarrel.TextureIndex == 4 || shotManager.ActivePowerUpBarrel.TextureIndex == 6 ||
                            shotManager.ActivePowerUpBarrel.TextureIndex == 7 || shotManager.ActivePowerUpBarrel.TextureIndex == 8)
                        {
                            spriteBatch.Draw(powerFrame, powerBarLocation, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                            spriteBatch.Draw(powerBar, powerBarLocation, new Rectangle(0, 0, (int)(powerBarFill * powerBar.Width), powerBar.Height), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                            spriteBatch.Draw(powerMarkerTarget, powerTargetLocation, null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);
                            spriteBatch.Draw(A, powerBarLocation- new Vector2(16,12), Color.White);
                        }
                        break;
                    }
                default:
                break;
            }
            return;
        }

        private void DrawAmmoUI(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (levelState == LevelState.Fire || levelState == LevelState.PowerUpFire)
            {
                if (GameSettings.isBoss)
                {
                    spriteBatch.Draw(B, new Rectangle((int)shotManager.bossAmmoLocation.X - 54, (int)shotManager.bossAmmoLocation.Y + 10, 48, 48), Color.White);
                    DrawStringHelper(spriteBatch, ScreenManager.smallFont, "END SHOT", new Vector2(shotManager.bossAmmoLocation.X - 4, shotManager.bossAmmoLocation.Y + 18), Color.White, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
                else
                {
                    spriteBatch.Draw(B, new Rectangle((int)shotManager.ammoUILocation.X - 54, (int)shotManager.ammoUILocation.Y + 10, 48, 48), Color.White);
                    DrawStringHelper(spriteBatch, ScreenManager.smallFont, "END SHOT", new Vector2(shotManager.ammoUILocation.X - 4, shotManager.ammoUILocation.Y + 18), Color.White, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                }
                return;
            }

            if (levelState == LevelState.Countdown || levelState == LevelState.Aim || levelState == LevelState.Power)
            {
                if (shotManager.ActivePowerUpBarrel.TextureIndex == 0 || shotManager.ActivePowerUpBarrel.TextureIndex == 6 || shotManager.ActivePowerUpBarrel.TextureIndex == 7)
                {
                    if (GameSettings.isBoss)
                    {
                        spriteBatch.Draw(fruitBar, new Rectangle((int)shotManager.bossAmmoLocation.X - 16, (int)shotManager.bossAmmoLocation.Y + 12, fruitBar.Width, fruitBar.Height), Color.White);
                        spriteBatch.Draw(X, new Rectangle((int)shotManager.bossAmmoLocation.X - 54, (int)shotManager.bossAmmoLocation.Y + 2, X.Width, X.Height), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(fruitBar, new Rectangle((int)shotManager.ammoUILocation.X - 16, (int)shotManager.ammoUILocation.Y + 12, fruitBar.Width, fruitBar.Height), Color.White);
                        spriteBatch.Draw(X, new Rectangle((int)shotManager.ammoUILocation.X - 54, (int)shotManager.ammoUILocation.Y + 2, X.Width, X.Height), Color.White);
                    }
                    foreach (Sprite sprite in shotManager.ammoUI)
                    {
                        sprite.Draw(gameTime, spriteBatch);
                        if (!sprite.IsVisible) spriteBatch.Draw(smLock, sprite.SpriteRectangle, Color.White);
                    }
                    shotManager.ammoUI[shotManager.selectedAmmo].Draw(gameTime, spriteBatch);
                }
            }
            return;
        }

        private void DrawShotCursor(SpriteBatch spriteBatch)
        {
            if (lastState == LevelState.Intro) return;
            if (lastState == LevelState.Fire || lastState == LevelState.PowerUpFire)
            {
                if (shotManager.shot.IsExpired || !shotManager.shot.IsVisible) return;
            }
            if (Camera.IsObjectVisible(shotManager.shot.SpriteRectangle,new Vector2(1.0f,1.0f))) return;

            showCursor = false;
            isCursorUp = false;
            isCursorDown = false;
            isCursorRight = false;
            isCursorLeft = false;

            shotRect = Camera.WorldToScreen(shotManager.shot.SpriteRectangle);

                                
            if(96  > shotRect.Right)
            {
                showCursor = true;
                cursorPosition = new Vector2(32, shotRect.Y);
                cursorRotation = MathHelper.ToRadians(90);
                isCursorLeft = true;
            }
            if (Camera.ViewportWidth -96< shotRect.Left)
            {
                showCursor = true;
                cursorPosition = new Vector2(1248 - cursor.Width, shotRect.Y);
                cursorRotation = MathHelper.ToRadians(270);
                isCursorRight = true;
            }
            if (Camera.ViewportHeight -96< shotRect.Top)
            {
                showCursor = true;
                cursorPosition = new Vector2(shotRect.X, 688 - cursor.Height);
                cursorRotation = 0f;
                isCursorDown = true;
            }
            if (96 > shotRect.Bottom)
            {
                showCursor = true;
                cursorPosition = new Vector2(shotRect.X, 32);
                cursorRotation = MathHelper.ToRadians(180);
                isCursorUp = true;
            }
            if (isCursorUp && isCursorLeft)
            {
                cursorPosition = new Vector2(32,32);
                cursorRotation = MathHelper.ToRadians(135);
            }
            if (isCursorUp && isCursorRight)
            {
                cursorPosition = new Vector2(1248-cursor.Width,32);
                cursorRotation = MathHelper.ToRadians(225);
            }
            if (isCursorDown && isCursorLeft)
            {
                cursorPosition = new Vector2(32, 688-cursor.Height);
                cursorRotation = MathHelper.ToRadians(45);
            }
            if (isCursorDown && isCursorRight)
            {
                cursorPosition = new Vector2(1248-cursor.Width, 688-cursor.Height);
                cursorRotation = MathHelper.ToRadians(315);
            }

            if (showCursor) spriteBatch.Draw(cursor, new Rectangle((int)(cursorPosition.X + cursorOrigin.X),(int)(cursorPosition.Y+cursorOrigin.Y),(int)cursor.Width,(int)cursor.Height), null, GameSettings.ShotPathColor,cursorRotation,cursorOrigin,SpriteEffects.None,1f);
            
            return;
        }

        private void DrawShotWindowUI(GameTime gameTime, SpriteBatch spriteBatch)
        {

            //draw level title
            DrawStringHelper(spriteBatch, ScreenManager.font, levelName, new Vector2(128, 42), Color.White, Vector2.Zero, 0.6f, SpriteEffects.None, TransitionAlpha);

            medalFrame += 12*(float)gameTime.ElapsedGameTime.TotalSeconds;
            if (medalFrame >= 10f) medalFrame -= 10f;

            spriteBatch.Draw(shotWindow, new Rectangle((int)shotWindowLocation.X, (int)shotWindowLocation.Y, shotWindow.Width, shotWindow.Height), shotWinTint);
            if (shotManager.powerStarCollected)
            {
                star.TotalRotation += 0.04f;
                if (star.TintColor.A != 255)
                {
                    float alpha = star.TintColor.A;
                    alpha = (alpha + (2f * (float)gameTime.ElapsedGameTime.TotalSeconds));
                    star.TintColor = Color.White * alpha;
                }
                star.Draw(gameTime, spriteBatch);
            }

            spriteBatch.Draw(medalGold, new Rectangle((int)shotWindowLocation.X + 8, (int)shotWindowLocation.Y + 34, 32, 32), new Rectangle (4+(68*(int)medalFrame),4,64,64), Color.White);
            DrawStringHelper(spriteBatch, ScreenManager.smallFont, LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].gold.ToString(), new Vector2((int)shotWindowLocation.X + 16, (int)shotWindowLocation.Y + 36), Color.White, Vector2.Zero, 0.8f, SpriteEffects.None, TransitionAlpha);
            spriteBatch.Draw(medalSilver, new Rectangle((int)shotWindowLocation.X + 48, (int)shotWindowLocation.Y + 34, 32, 32), new Rectangle (4+(68*(int)medalFrame),4,64,64), Color.White);
            DrawStringHelper(spriteBatch, ScreenManager.smallFont, LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].silver.ToString(), new Vector2((int)shotWindowLocation.X + 56, (int)shotWindowLocation.Y + 36), Color.White, Vector2.Zero, 0.8f, SpriteEffects.None, TransitionAlpha);
            spriteBatch.Draw(medalBronze, new Rectangle((int)shotWindowLocation.X + 88, (int)shotWindowLocation.Y + 34, 32, 32), new Rectangle (4+(68*(int)medalFrame),4,64,64), Color.White);
            DrawStringHelper(spriteBatch, ScreenManager.smallFont, LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].bronze.ToString(), new Vector2((int)shotWindowLocation.X + 96, (int)shotWindowLocation.Y + 36), Color.White, Vector2.Zero, 0.8f, SpriteEffects.None, TransitionAlpha);
            //if (!isVictoryScreen && !GameSettings.isBoss)
            //{
                //draw score and combo
            //    score = "SCORE: " + Camera.Score.ToString();
            //    DrawStringHelper(spriteBatch, ScreenManager.font, score, new Vector2((Camera.ViewportWidth) - 370, 72), Color.White, Vector2.Zero, 0.6f, SpriteEffects.None, TransitionAlpha);

            //    if (Camera.ScoreCombo != 1)
            //    {
            //        combo = "COMBO X" + Camera.ScoreCombo.ToString();
            //        DrawStringHelper(spriteBatch, ScreenManager.font, combo, new Vector2((Camera.ViewportWidth) - 370, 102), Color.White, Vector2.Zero, 0.6f, SpriteEffects.None, TransitionAlpha);
            //    }
            //}

            if (isFailed)
            {
                if (failAlpha < 1f) failAlpha += (float)gameTime.ElapsedGameTime.TotalSeconds * 2f;
                spriteBatch.Draw(noMedal, new Rectangle((int)shotWindowLocation.X+34,(int)shotWindowLocation.Y+3,64,64),Color.White * TransitionAlpha * failAlpha);
                spriteBatch.Draw(Y, new Rectangle((int)shotWindowLocation.X - 8, (int)shotWindowLocation.Y + 66, 48, 48), Color.White * TransitionAlpha * failAlpha);
                DrawStringHelper(spriteBatch, ScreenManager.smallFont, "RESTART", new Vector2((int)shotWindowLocation.X + 42, (int)shotWindowLocation.Y + 75), Color.White, Vector2.Zero, 1f, SpriteEffects.None, TransitionAlpha * failAlpha);
            }

            if (shotNumber < 10) DrawStringHelper(spriteBatch, ScreenManager.smallFont, "Shots: " + shotNumber, new Vector2((int)shotWindowLocation.X + 12, (int)shotWindowLocation.Y + 4), Color.White, Vector2.Zero, 0.8f, SpriteEffects.None, TransitionAlpha);
            else DrawStringHelper(spriteBatch, ScreenManager.smallFont, "Shots:" + shotNumber, new Vector2((int)shotWindowLocation.X + 12, (int)shotWindowLocation.Y + 4), Color.White, Vector2.Zero, 0.8f, SpriteEffects.None, TransitionAlpha);

            return;
        }

        private void DrawShotPath(SpriteBatch spriteBatch, float dt, int count, Color color)
        {
            float xloc = barrelEndLocation.X + (dt * firingForce.X); 
            float yloc = barrelEndLocation.Y + (dt * firingForce.Y);

            //add gravity's influence for some barrels
            if (shotManager.ActivePowerUpBarrel.TextureIndex == 0 || shotManager.ActivePowerUpBarrel.TextureIndex == 4 || shotManager.ActivePowerUpBarrel.TextureIndex == 6 ||
                shotManager.ActivePowerUpBarrel.TextureIndex == 7 || shotManager.ActivePowerUpBarrel.TextureIndex == 8)
            {
                yloc += (dt * dt * GameSettings.Gravity * GameSettings.PhysicsScale * 0.5f);
            }

            Vector2 drawlocation = new Vector2((xloc-8),(yloc-8));

            if (count == 0)
            {
                spriteBatch.Draw(LevelDataManager.effectTextures[17].Texture, Camera.WorldToScreen(drawlocation), new Rectangle(0, 0, 16, 16), color, 0.0f, Vector2.Zero, Camera.zoom, SpriteEffects.None, 0.0f);
            }
            else
            {
                spriteBatch.Draw(pellet, Camera.WorldToScreen(drawlocation), new Rectangle((16 * count), 0, 16, 16), color, 0.0f, Vector2.Zero, Camera.zoom, SpriteEffects.None, 0.0f);
                dt += 0.005f;
                DrawShotPath(spriteBatch, dt, count - 1, color*1.1f);
            }
            return;
        }

        private void TextOverlayDraw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            DrawStringHelper(spriteBatch, ScreenManager.font, "World Size : " + Camera.WorldRectangle.ToString(), new Vector2(64, 128), Color.White, Vector2.Zero, 0.7f, SpriteEffects.None, TransitionAlpha);
            DrawStringHelper(spriteBatch, ScreenManager.font, "Camera : " + Camera.Viewport.ToString(), new Vector2(64, 156), Color.White, Vector2.Zero, 0.7f, SpriteEffects.None, TransitionAlpha);
            DrawStringHelper(spriteBatch, ScreenManager.font, "Mouse : " + new Vector2(Mouse.GetState().X + Camera.Position.X, Mouse.GetState().Y + Camera.Position.Y).ToString(), new Vector2(250, 600), Color.White, Vector2.Zero, 0.7f, SpriteEffects.None, TransitionAlpha);
            DrawStringHelper(spriteBatch, ScreenManager.font, "LookingAt : " + new Vector2(Camera.Position.X + Camera.Origin.X, Camera.Position.Y + Camera.Origin.Y).ToString(), new Vector2(650, 600), Color.White, Vector2.Zero, 0.7f, SpriteEffects.None, TransitionAlpha);
            return;
        }
        private void DrawFPSCounter(GameTime gameTime, SpriteBatch spriteBatch)
        {
            float dTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Skip zero-time frames (infinity problem)
            if (dTime == 0.0f)
                return;

            //Compute instantaneous fps
            float instantaneousFps = (int)Math.Round(1.0f / dTime);

            //Averaging stuff
            ++averagingCount;
            averagingTotal += instantaneousFps;
       
            if (averagingCount >= 25)
            {
                averagedFPS = (int)Math.Round((float)averagingTotal / (float)averagingCount);
           
                averagingCount = 0;
                averagingTotal = 0;
            }
            int instFPS = (int)instantaneousFps;
            DrawStringHelper(spriteBatch, ScreenManager.font, "FPS : " + averagedFPS.ToString() + " : " + instFPS.ToString(), new Vector2(64, 600), Color.White, Vector2.Zero, 0.6f, SpriteEffects.None, TransitionAlpha);
            return;
        }
        #endregion


        #endregion


        #region PROPERTIES
        public Layer PlayArea 
        {
        get { return parallaxEngine.worldLayers[interactLayer];}
        }


        #endregion

        public override void UnloadContent()
        {

        }

        void contactListener_PowerUpActivated(object sender, EffectEventArgs e)
        {
            //sprite a is shot, sprite b is barrel
            if (e.spriteB == shotManager.ActivePowerUpBarrel) return; //cant go in barrel shot from
            //if (e.spriteA.TextureID == 20 && e.spriteA.TextureIndex > 62) return; //cherry already split, cancel contact
            if (shotManager.isSplit) return;//cherry already split, cancel contact
            if (e.spriteB.TextureIndex == 5) return; //teleport barrel does not change fire state or reset shot

            levelState = LevelState.PowerUpAim;
            firingLocation = e.spriteB.SpriteCenterInWorld;
            shotManager.shot.Location = firingLocation - shotManager.shot.SpriteOrigin;
            shotManager.shot.spriteBody.Position = ConvertUnits.ToSimUnits(shotManager.shot.SpriteCenterInWorld);
            shotManager.shot.spriteBody.ResetDynamics();
            shotManager.shot.spriteBody.IgnoreGravity = true;
            shotManager.shot.IsVisible = false;
        }

        public void EndOfShot()  //resets all variables and increases counter
        {
            if (levelState == LevelState.Countdown || levelState == LevelState.Aim || levelState == LevelState.Power || levelState == LevelState.PowerUpAim) shotNumber += 1;
            powerBarFill = 0.0f;
            actualPower = 0.0f;
            powerSetBarFill = 0.0f;
            shotManager.ActivePowerUpBarrel = shotManager.ShotStartBarrel;
            firingLocation = shotManager.ActivePowerUpBarrel.SpriteCenterInWorld;
            shotManager.shot.spriteBody.Position = ConvertUnits.ToSimUnits(firingLocation);
            countdown = GameSettings.shotTimer;
            shotManager.shot.spriteBody.Enabled = false;
            shotManager.shot.IsExpired = false;
            shotManager.shot.HitPoints = 1;
            shotManager.shot.IsHit = false;
            shotManager.shot.IsVisible = false;
            shotManager.isSplit = false;

            if (LevelDataManager.levelData[world, level].safety)
            {
                Camera.ScrollTo(firingLocation, 0);
                Camera.IsScrolling = true;
                levelState = LevelState.Countdown;
            }
            else
            {
                Camera.IsScrolling = false;
                shotManager.shot.spriteBody.Enabled = true;
                shotManager.shot.spriteBody.IsSensor = true;
                levelState = LevelState.Aim;
            }
            
            return;
        }

    }
}
