using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParallaxEngine;

namespace ProduceWars.Managers
{
    public class DecoManager
    {
        private List<Sprite> DecoSprites;
        private List<Sprite> StarSprites;
        private List<Sprite> CloudSprites;
        private List<Sprite> WeatherSprites;
        private ParallaxManager parallaxEngine;
        private bool IsNight = false;
        private bool IsSunset = false;
        private Color nightTint = new Color(140, 140, 190);
        private Color duskTint = new Color(240, 180, 150);
        private int numberOfStars = 150; 
        private int starInterval = 0;
        private int starPerCycle = 0;
        private int numberOfClouds = LevelDataManager.rand.Next(1,11) * 10; 
        private int cloudInterval = 0;
        private int cloudsPerCycle = 0;
        private int numberOfWeatherParticles = 0;
        private int weatherInterval = 0;
        private int weatherPartPerCycle = 0;
        public Layer weatherLayer;
        private enum Weather { None, Rain, Snow, Leaf, Pollen };
        private Vector2 weatherParallax = new Vector2(1.2f,1.2f);
        private Weather weather = Weather.None;
        private Vector2 currentWLocation = Vector2.Zero;
        private bool IsOutOfScreen = false;
        private Vector2 newLocation = Vector2.Zero;
        private bool IsSound = false;
        public Color Tint = Color.White;

        public DecoManager(ParallaxManager _parallaxManager, bool _isSound)
        {
            IsSound = _isSound;

            DecoSprites = new List<Sprite>();
            StarSprites = new List<Sprite>();
            CloudSprites = new List<Sprite>();
            WeatherSprites = new List<Sprite>();
            parallaxEngine = _parallaxManager;
            SoundManager.ClearSounds();

            int decoworld = LevelDataManager.world;

            if (decoworld == 6) //bonus world
            {
                switch (LevelDataManager.level)
                {
                    case 1:
                    case 2:
                    case 4:
                        {
                            decoworld = 1; //play grass
                            break;
                        }
                    case 5:
                    case 13:
                        {
                            decoworld = 2; //forest
                            break;
                        }
                    case 6:
                    case 7:
                    case 8:
                        {
                            decoworld = 3; //desert
                            break;
                        }
                    case 3:
                    case 14:
                        {
                            decoworld = 4; //snow
                            break;
                        }
                    case 9:
                    case 10:
                    case 11:
                    case 12:
                    case 15:
                        {
                            decoworld = 5; //factory
                            break;
                        }
                    default:
                        break;
                }
            }

            if (decoworld != 5)
            {
                #region ROLL RANDOM TIME DAY, SUNSET, NIGHT
                int timeRoll = LevelDataManager.rand.Next(1, 11);
                if (timeRoll < 5) //day 40%
                {
                    for (int i = 0; i < parallaxEngine.worldLayers[0].layerSprites.Count; i++)
                    {
                        parallaxEngine.worldLayers[0].layerSprites[i].TextureID = 34;
                    }
                    IsSunset = false;
                    IsNight = false;
                }
                if (timeRoll == 5 || timeRoll == 6) //sunset 20%
                {
                    for (int i = 0; i < parallaxEngine.worldLayers[0].layerSprites.Count; i++)
                    {
                        parallaxEngine.worldLayers[0].layerSprites[i].TextureID = 35;
                    }
                    IsSunset = true;
                    IsNight = false;
                }
                if (timeRoll > 6) //night 40%
                {
                    for (int i = 0; i < parallaxEngine.worldLayers[0].layerSprites.Count; i++)
                    {
                        parallaxEngine.worldLayers[0].layerSprites[i].TextureID = 36;
                    }
                    IsNight = true;
                    IsSunset = false;
                    if (IsSound) SoundManager.Play(SoundManager.Sound.EffectCricketsLoop, true, false);
                }
                #endregion
            }

            if (LevelDataManager.world == 5 && LevelDataManager.level == 5)
            {
                for (int i = 0; i < parallaxEngine.worldLayers[0].layerSprites.Count; i++)
                {
                    parallaxEngine.worldLayers[0].layerSprites[i].TextureID = 36;
                }
                IsNight = true;
            }

            #region REMOVE PLACED MOON OR ADD MOON
            if (!IsNight && parallaxEngine.worldLayers[1].layerSprites[0].TextureID == 33) parallaxEngine.worldLayers.RemoveAt(1);
            if (IsNight && parallaxEngine.worldLayers[1].layerSprites[0].TextureID != 33)
            {
                int maxMoonHeight = (int)Math.Sqrt(Camera.ViewportHeight) * 70;
                Layer moonLayer = new Layer("moon", Vector2.Zero, false, true, false, 0f, Vector2.Zero, false, 0f, Vector2.Zero);
                Vector2 moonLocation = new Vector2( LevelDataManager.rand.Next(-400,Camera.ViewportWidth+150), (float)Math.Pow((float)LevelDataManager.rand.Next(0, maxMoonHeight) / 100f, (2.0)));
                Sprite moon = new Sprite(33, 0, moonLocation, false);
                moonLayer.AddSpriteToLayer(moon);
                parallaxEngine.worldLayers.Insert(1, moonLayer);
            }
            #endregion

            #region CREATE STARS
            if (IsNight)
            {
                starPerCycle = numberOfStars / 10;
                int maxStarHeight = (int)Math.Sqrt(Camera.ViewportHeight) * 70;
                Layer starLayer = new Layer("stars", Vector2.Zero, false, true, false, 0f, Vector2.Zero, false, 0f, Vector2.Zero);
                for (int i = 0; i < numberOfStars; i++)
                {
                    int starID = 21 + LevelDataManager.rand.Next(0, 2);
                    Vector2 starLocation = new Vector2(LevelDataManager.rand.Next(0, Camera.ViewportWidth), (float)Math.Pow((float)LevelDataManager.rand.Next(0, maxStarHeight) / 100f, (2.0)));
                    Sprite star = new Sprite(starID, 0, starLocation, true);
                    star.IsAwake = false;
                    star.Scale = (float)LevelDataManager.rand.Next(50, 101) / 100f;
                    star.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 361));
                    star.TintColor *= (float)LevelDataManager.rand.Next(0, 101) / 100f;
                    StarSprites.Add(star);
                    starLayer.AddSpriteToLayer(star);
                }
                parallaxEngine.worldLayers.Insert(1, starLayer);
            }

            #endregion

            if (parallaxEngine.worldLayers[0].layerSprites[0].TextureID == 36 || parallaxEngine.worldLayers[0].layerSprites[0].TextureID == 35 || parallaxEngine.worldLayers[0].layerSprites[0].TextureID == 34)
            {
                #region CREATE CLOUD LAYER
                cloudsPerCycle = numberOfClouds / 10;
                int maxCloudHeight = 400;
                Layer cloudLayer = new Layer("clouds", Vector2.Zero, false, true, false, 0f, Vector2.Zero, false, 0f, Vector2.Zero);
                for (int i = 0; i < numberOfClouds; i++)
                {
                    Vector2 cloudLocation = new Vector2(LevelDataManager.rand.Next(-200, Camera.ViewportWidth + 200), LevelDataManager.rand.Next(-100, maxCloudHeight));
                    Sprite cloud = new Sprite(23, LevelDataManager.rand.Next(0, 3), cloudLocation, true);
                    cloud.IsAwake = false;
                    if (IsSunset) cloud.TextureIndex += 3;
                    if (IsNight) cloud.TextureIndex += 6;
                    cloud.Scale = (float)LevelDataManager.rand.Next(100, 401) / 100f;
                    if (LevelDataManager.rand.Next(0, 2) == 0) cloud.IsFlippedHorizontally = true;
                    if (IsSunset) cloud.TintColor = duskTint;
                    if (IsNight) cloud.TintColor = nightTint;
                    cloud.TintColor *= (float)LevelDataManager.rand.Next(0, 51) / 100f;
                    cloud.Velocity = LevelDataManager.rand.Next(20, 60);
                    if (LevelDataManager.rand.Next(0, 2) == 0) cloud.Velocity *= -1;
                    CloudSprites.Add(cloud);
                    cloudLayer.AddSpriteToLayer(cloud);
                }
                if (IsNight) parallaxEngine.worldLayers.Insert(3, cloudLayer);
                if (IsSunset) parallaxEngine.worldLayers.Insert(1, cloudLayer);
                if (!IsSunset && !IsNight) parallaxEngine.worldLayers.Insert(1, cloudLayer);
                #endregion
            }

            #region CREATE WEATHER
            if (decoworld == 0) weather = Weather.Pollen;
            if (decoworld == 1) weather = Weather.Pollen;
            if (decoworld == 2) weather = Weather.Leaf;   
            if (numberOfClouds > 70 && LevelDataManager.rand.Next(0, 2) == 0) weather = Weather.Rain; //50% chance of rain if cloud cover in top 20% (10% chance total)
            if (decoworld == 4) weather = Weather.Snow;
            if (decoworld == 3) weather = Weather.None;
            if (decoworld == 5) weather = Weather.None;

            int partID = 0;
            int tiles = 1;
            weatherLayer = new Layer("weather",weatherParallax,false,true,false,0f,Vector2.Zero,false,0f,Vector2.Zero);
            if (weather == Weather.Rain)
            {
                partID = 26;
                numberOfWeatherParticles = Math.Max(0, numberOfClouds - 60) * LevelDataManager.rand.Next(6,13);
                if (IsSound) SoundManager.Play(SoundManager.Sound.EffectRainLoop, true, false);
            }
            if (weather == Weather.Snow)
            {
                numberOfWeatherParticles = Math.Max(0, numberOfClouds - 30) * LevelDataManager.rand.Next(2,5);
                partID = 27;
                tiles = 5;
            }
            if (weather == Weather.Pollen)
            {
                numberOfWeatherParticles = LevelDataManager.rand.Next(10, 21);
                partID = 25;
                tiles = 3;
            }
            if (weather == Weather.Leaf)
            {
                numberOfWeatherParticles = LevelDataManager.rand.Next(8, 16);
                partID = 24;
                tiles = 9;
            }
            //weatherPartPerCycle = numberOfWeatherParticles / 10;
            weatherPartPerCycle = numberOfWeatherParticles;
            for (int i = 0; i < numberOfWeatherParticles; i++)
            {
                Vector2 weatherParticleLocation = Camera.ScreenToWorld(new Vector2(LevelDataManager.rand.Next(0, Camera.Viewport.Width),
                                                              LevelDataManager.rand.Next(0, Camera.Viewport.Height)),weatherLayer.LayerParallax);
                Sprite weatherP = new Sprite(partID, LevelDataManager.rand.Next(0, tiles), Vector2.Zero, true);
                weatherP.IsAwake = false;
                if (weather == Weather.Rain) NewRain(weatherP, weatherParticleLocation);
                if (weather == Weather.Snow) NewSnow(weatherP,weatherParticleLocation);
                if (weather == Weather.Pollen) NewPollen(weatherP,weatherParticleLocation);
                if (weather == Weather.Leaf) NewLeaf(weatherP,weatherParticleLocation);
                WeatherSprites.Add(weatherP);
                weatherLayer.AddSpriteToLayer(weatherP);
            }

            #endregion

            #region TINT 
            for (int i = 0; i < parallaxEngine.worldLayers.Count; i++)
            {
                for (int j = 0; j < parallaxEngine.worldLayers[i].SpriteCount; j++)
                {
                    if (parallaxEngine.worldLayers[i].layerSprites[j].TextureID < 20 || 
                        (parallaxEngine.worldLayers[i].layerSprites[j].IsEffect && parallaxEngine.worldLayers[i].layerSprites[j].TextureID >= 24))
                    {
                        TintSprite(parallaxEngine.worldLayers[i].layerSprites[j]);
                    }

                    //tint for rain
                    if (weather == Weather.Rain)
                    {
                        if (parallaxEngine.worldLayers[i].layerSprites[j].TextureID < 20 || parallaxEngine.worldLayers[i].layerSprites[j].SpriteType == Sprite.Type.Deco ||
                            parallaxEngine.worldLayers[i].layerSprites[j].TextureID == 34 || parallaxEngine.worldLayers[i].layerSprites[j].TextureID == 35 || parallaxEngine.worldLayers[i].layerSprites[j].TextureID == 36 ||
                            parallaxEngine.worldLayers[i].layerSprites[j].IsEffect)
                        {
                            parallaxEngine.worldLayers[i].layerSprites[j].TintColor = Color.Lerp(parallaxEngine.worldLayers[i].layerSprites[j].TintColor, Color.Gray,0.5f);
                        }
                        Tint = Color.Lerp(Tint, Color.Gray, 0.5f);
                    }
                }
            }
            #endregion

        }

        public void NewLeaf(Sprite sprite, Vector2 location)
        {
            sprite.Location = location;
            sprite.Scale = (float)LevelDataManager.rand.Next(50, 100) / 100f;
            sprite.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            sprite.RotationSpeed = LevelDataManager.rand.Next(-100, 100) * 0.0005f;
            sprite.PathingDirection = new Vector2(LevelDataManager.rand.Next(-100, 100) * 0.01f, LevelDataManager.rand.Next(50, 100) * 0.01f) * sprite.Scale;
        }

        public void NewPollen(Sprite sprite, Vector2 location)
        {
            sprite.Location = location;
            sprite.Scale = (float)LevelDataManager.rand.Next(50, 100) / 100f;
            sprite.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            sprite.RotationSpeed = LevelDataManager.rand.Next(-100, 100) * 0.0005f;
            sprite.PathingDirection = new Vector2(LevelDataManager.rand.Next(-100, 100) * 0.01f, LevelDataManager.rand.Next(-50, 100) * 0.01f) * sprite.Scale;
        }

        public void NewSnow(Sprite sprite, Vector2 location)
        {
            sprite.Location = location;
            sprite.Scale = (float)LevelDataManager.rand.Next(50, 125) / 100f;
            sprite.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            sprite.RotationSpeed = LevelDataManager.rand.Next(-100, 100) * 0.0005f;
            sprite.PathingDirection = new Vector2(LevelDataManager.rand.Next(-20, 20) * 0.01f, LevelDataManager.rand.Next(50, 150) * 0.01f) * sprite.Scale;
        }

        public void NewRain(Sprite sprite, Vector2 location)
        {
            sprite.Location = location;
            sprite.Scale = (float)LevelDataManager.rand.Next(75, 150) / 100f;
            sprite.TintColor = new Color (128,128,128,128);
            sprite.PathingDirection = new Vector2(LevelDataManager.rand.Next(-20, 20) * 0.01f, LevelDataManager.rand.Next(130, 171) * 0.1f) * sprite.Scale;
        }

        public void Update(GameTime gameTime)
        {
            if (DecoSprites.Count > 0)
            {
                for (int i = DecoSprites.Count - 1; i >= 0; i--)
                {
                    if (DecoSprites[i].IsAnimated) UpdateAnimation(gameTime, DecoSprites[i]);
                    else UpdateArtificialAnimation(gameTime, DecoSprites[i]);
                }
            }

            if (StarSprites.Count > 0)
            {
                #region STAR UPDATE
                for (int i = 0; i < starPerCycle; i++)
                {
                    int alpha = StarSprites[i + (starPerCycle * starInterval)].TintColor.A;
                    alpha = alpha + LevelDataManager.rand.Next(-25, 26);
                    if (alpha > 255) alpha = 255;
                    if (alpha < 0) alpha = 0;
                    StarSprites[i + (starPerCycle * starInterval)].TintColor = new Color(alpha, alpha, alpha, alpha);
                }
                starInterval += 1;
                if (starInterval == 10) starInterval = 0;
                #endregion
            }

            if (CloudSprites.Count > 0)
            {
                #region CLOUD UPDATE
                for (int i = 0; i < cloudsPerCycle; i++)
                {
                    CloudSprites[i + (cloudsPerCycle * cloudInterval)].Location += new Vector2((float)gameTime.ElapsedGameTime.TotalSeconds * CloudSprites[i + (cloudsPerCycle * cloudInterval)].Velocity, 0);
                    if (CloudSprites[i + (cloudsPerCycle * cloudInterval)].Location.X < -300)
                    {
                        CloudSprites[i + (cloudsPerCycle * cloudInterval)].Location = new Vector2(-299f, (float)LevelDataManager.rand.Next(-200, 501));
                        CloudSprites[i + (cloudsPerCycle * cloudInterval)].Velocity = (float)LevelDataManager.rand.Next(5, 61);
                        if (LevelDataManager.rand.Next(0, 2) == 0) CloudSprites[i + (cloudsPerCycle * cloudInterval)].IsFlippedHorizontally = true;
                        else CloudSprites[i + (cloudsPerCycle * cloudInterval)].IsFlippedHorizontally = false;
                        CloudSprites[i + (cloudsPerCycle * cloudInterval)].Scale = (float)LevelDataManager.rand.Next(100, 601) / 100f;
                    }
                    if (CloudSprites[i + (cloudsPerCycle * cloudInterval)].Location.X > 1500)
                    {
                        CloudSprites[i + (cloudsPerCycle * cloudInterval)].Location = new Vector2(1499f, (float)LevelDataManager.rand.Next(-200, 501));
                        CloudSprites[i + (cloudsPerCycle * cloudInterval)].Velocity = (float)LevelDataManager.rand.Next(-60, 5);
                        if (LevelDataManager.rand.Next(0, 2) == 0) CloudSprites[i + (cloudsPerCycle * cloudInterval)].IsFlippedHorizontally = true;
                        else CloudSprites[i + (cloudsPerCycle * cloudInterval)].IsFlippedHorizontally = false;
                        CloudSprites[i + (cloudsPerCycle * cloudInterval)].Scale = (float)LevelDataManager.rand.Next(100, 401) / 100f;
                    }

                    int delta = LevelDataManager.rand.Next(-1, 2);
                    int alpha = CloudSprites[i + (cloudsPerCycle * cloudInterval)].TintColor.A;
                    alpha = alpha + delta;
                    if (alpha > 200) alpha = 199;
                    if (alpha < 0) alpha = 0;
                    int r = CloudSprites[i + (cloudsPerCycle * cloudInterval)].TintColor.R;
                    r = r + delta;
                    if (r > 200) r = 199;
                    if (r < 0) r = 0;
                    int g = CloudSprites[i + (cloudsPerCycle * cloudInterval)].TintColor.G;
                    g = g + delta;
                    if (g > 200) g = 199;
                    if (g < 0) g = 0;
                    int b = CloudSprites[i + (cloudsPerCycle * cloudInterval)].TintColor.B;
                    b = b + delta;
                    if (b > 200) b = 199;
                    if (b < 0) b = 0;
                    CloudSprites[i + (cloudsPerCycle * cloudInterval)].TintColor = new Color(r, g, b, alpha);
                }
                cloudInterval += 1;
                if (cloudInterval == 10) cloudInterval = 0;
                #endregion
            }

            if (WeatherSprites.Count > 0)
            {
                
                #region WEATHER UPDATE
                for (int i = 0; i < weatherPartPerCycle; i++)
                {
                    IsOutOfScreen = false;
                    newLocation = Vector2.Zero;

                    //movement and rotaion
                    WeatherSprites[i + (weatherPartPerCycle * weatherInterval)].Location += WeatherSprites[i + (weatherPartPerCycle * weatherInterval)].PathingDirection;
                    if (weather != Weather.Rain) WeatherSprites[i + (weatherPartPerCycle * weatherInterval)].TotalRotation += WeatherSprites[i + (weatherPartPerCycle * weatherInterval)].RotationSpeed;
                    
                    //check bounds
                    currentWLocation = Camera.WorldToScreen(WeatherSprites[i + (weatherPartPerCycle * weatherInterval)].Location, weatherParallax);

                    if (currentWLocation.Y > Camera.ViewportHeight + 32)
                    {
                       newLocation = Camera.ScreenToWorld(new Vector2(LevelDataManager.rand.Next(0, Camera.ViewportWidth),
                                                                           currentWLocation.Y - Camera.ViewportHeight - LevelDataManager.rand.Next(32, 64)), weatherParallax);
                       IsOutOfScreen = true;
                    }
                    if (!IsOutOfScreen)
                    {
                        if (currentWLocation.X > Camera.ViewportWidth + 32)
                        {
                            newLocation = Camera.ScreenToWorld(new Vector2(currentWLocation.X - Camera.ViewportWidth - LevelDataManager.rand.Next(32, 64),
                                                                            LevelDataManager.rand.Next(0, Camera.ViewportHeight)), weatherParallax);
                            IsOutOfScreen = true;
                        }
                    }
                    if (!IsOutOfScreen)
                    {
                        if (currentWLocation.X < -32)
                        {
                            newLocation = Camera.ScreenToWorld(new Vector2(currentWLocation.X + Camera.ViewportWidth + LevelDataManager.rand.Next(32, 64),
                                                                           LevelDataManager.rand.Next(0, Camera.ViewportHeight)), weatherParallax);
                            IsOutOfScreen = true;
                        }
                    }
                    if (!IsOutOfScreen)
                    {
                        if (currentWLocation.Y < -32)
                        {
                            newLocation = Camera.ScreenToWorld(new Vector2(LevelDataManager.rand.Next(0, Camera.ViewportWidth),
                                                                           currentWLocation.Y + Camera.ViewportHeight + LevelDataManager.rand.Next(32, 64)), weatherParallax);
                            IsOutOfScreen = true;
                        }
                    }

                    if (IsOutOfScreen)
                    {
                        if (weather == Weather.Rain) NewRain(WeatherSprites[i + (weatherPartPerCycle * weatherInterval)],newLocation);
                        if (weather == Weather.Snow) NewSnow(WeatherSprites[i + (weatherPartPerCycle * weatherInterval)],newLocation);
                        if (weather == Weather.Leaf) NewLeaf(WeatherSprites[i + (weatherPartPerCycle * weatherInterval)],newLocation);
                        if (weather == Weather.Pollen) NewPollen(WeatherSprites[i + (weatherPartPerCycle * weatherInterval)],newLocation);
                    }
                }
                //weatherInterval += 1;
                //if (weatherInterval == 10) weatherInterval = 0;
                #endregion
            }

            return;
        }

        public void DrawWeather(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.GetViewMatrix(weatherLayer.LayerParallax));

            for (int i = 0; i < weatherLayer.layerSprites.Count; i++)
            {
                weatherLayer.layerSprites[i].Draw(gameTime, spriteBatch, weatherLayer.LayerParallax);
            }
            spriteBatch.End();
        }


        public void InitializeDeco(Sprite sprite)
        {
            DecoSprites.Add(sprite);
            sprite.IsAwake = false;
            if (IsSunset) sprite.TintColor = duskTint;
            if (IsNight) sprite.TintColor = nightTint;
            if (weather == Weather.Rain)sprite.TintColor = Color.Lerp(sprite.TintColor, Color.Gray,0.5f);

            if (LevelDataManager.levelTextures[sprite.TextureID].IsAnimated) //for deco animated on sprite sheets
            {
                if (sprite.TextureID == 54) //water
                {
                    sprite.IsAnimated = true;
                    sprite.IsAnimatedWhileStopped = true;
                    sprite.IsAnimationDirectionForward = true;
                    sprite.AnimationFPS = 12;
                    sprite.IsBounceAnimated = false;
                }

                if (sprite.TextureID == 53) //sunflower
                {
                    sprite.IsAnimated = true;
                    sprite.IsAnimatedWhileStopped = true;
                    sprite.IsAnimationDirectionForward = true;
                    sprite.IsBounceAnimated = true;
                    sprite.AnimationFPS = LevelDataManager.rand.Next(8, 11);
                    sprite.AnimationFramePrecise = LevelDataManager.rand.Next(0, 3);
                }
            }
            else  //for deco sprites not animated on sprite sheets 
            {
                sprite.IsAnimated = false;
                
                sprite.IsBounceAnimated = true;
                sprite.AnimationFPS = LevelDataManager.rand.Next(4, 8);
                sprite.AnimationFramePrecise = (float)LevelDataManager.rand.Next(0, 5);
                if (LevelDataManager.rand.Next(0, 2) == 0) sprite.IsAnimationDirectionForward = true;
                else sprite.IsAnimationDirectionForward = false;
            }
            return;
        }

        private void UpdateAnimation(GameTime gameTime, Sprite sprite)
        {
            sprite.AnimationFramePrecise += (float)gameTime.ElapsedGameTime.TotalSeconds * sprite.AnimationFPS;
            if (sprite.AnimationFramePrecise >= 1.0f)
            {
                sprite.AnimationFramePrecise -= 1.0f;
                if (sprite.IsAnimationDirectionForward)
                {

                    if (sprite.CurrentFrame + 1 == LevelDataManager.SpritesInRow(sprite))
                    {
                        if (!sprite.IsBounceAnimated) sprite.CurrentFrame = 0; //loop if not bounce animated
                        else
                        {
                            sprite.IsAnimationDirectionForward = false; //send the other direction
                            sprite.CurrentFrame -= 1; //move to frame before 
                        }
                    }
                    else sprite.CurrentFrame += 1;
                }
                else
                {
                    if (sprite.CurrentFrame == 0)
                    {
                        if (!sprite.IsBounceAnimated) sprite.CurrentFrame = LevelDataManager.SpritesInRow(sprite); //loop if not bounce animated
                        else
                        {
                            sprite.IsAnimationDirectionForward = true; //send the other direction
                            sprite.CurrentFrame += 1; //move to frame before 
                        }
                    }
                    else sprite.CurrentFrame -= 1;
                }
            }
        }

        private void UpdateArtificialAnimation(GameTime gameTime, Sprite sprite)
        {
            if (sprite.IsAnimationDirectionForward) sprite.AnimationFramePrecise += (float)gameTime.ElapsedGameTime.TotalSeconds * sprite.AnimationFPS;
            else sprite.AnimationFramePrecise -= (float)gameTime.ElapsedGameTime.TotalSeconds * sprite.AnimationFPS;
            if (sprite.AnimationFramePrecise >= 5f)
            {
                 sprite.IsAnimationDirectionForward = false; //send the other direction
            }
            if (sprite.AnimationFramePrecise <= 0f)
            {
                sprite.AnimationFramePrecise = 0f;
                 sprite.IsAnimationDirectionForward = true; //send the other direction
            }

            sprite.Scale = 1f + (sprite.AnimationFramePrecise/ (float)sprite.SpriteRectHeight); 
            
            return;
        }

        public void TintSprite(Sprite sprite)
        {
            if (IsSunset)
            {
                sprite.TintColor = duskTint;
                Tint = duskTint;
            }
            if (IsNight)
            {
                sprite.TintColor = nightTint;
                Tint = nightTint;
            }
            return;
        }
    }
}
