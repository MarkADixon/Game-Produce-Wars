using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParallaxEngine;
using FarseerPhysics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.PhysicsLogic;


namespace ProduceWars.Managers
{
    public class EffectManager
    {
        private World physicsWorld;
        private ContactListener contactListener;
        public List<Sprite> EffectSprites;
        private Vector2 layerParallax = new Vector2(1.0f, 1.0f);
        private Vector2 aDirection = new Vector2(0f, 1f); //the acceleration direction of fruit chunks
        private Explosion explosion;
        private Color tint;
        private float towerSawSpeed = 6f;

        public EffectManager(World _world, ContactListener _contactListener, Color _tint)
        {            
            physicsWorld = _world;
            contactListener = _contactListener;
            tint = _tint;
            EffectSprites = new List<Sprite>();

            contactListener.FruitExploded += new ContactListener.EffectEventHandler(contactListener_FruitExploded);
            contactListener.VeggieExploded += new ContactListener.EffectEventHandler(contactListener_VeggieExploded);
            contactListener.BlockExploded += new ContactListener.EffectEventHandler(contactListener_BlockExploded);
            contactListener.BombExploded += new ContactListener.EffectEventHandler(contactListener_BombExploded);
            contactListener.ShotFired += new ContactListener.EffectEventHandler(contactListener_ShotFired);
            contactListener.FanActivated += new ContactListener.EffectEventHandler(contactListener_FanActivated);
            contactListener.CreatureExploded += new ContactListener.EffectEventHandler(contactListener_CreatureExploded);
            contactListener.FireShotExploded += new ContactListener.EffectEventHandler(contactListener_FireShotExploded);
            contactListener.IceShotExploded += new ContactListener.EffectEventHandler(contactListener_IceShotExploded);
            contactListener.LitShotExploded += new ContactListener.EffectEventHandler(contactListener_LitShotExploded);
            //contactListener.DamageBlock += new ContactListener.DamageEventHandler(contactListener_DamageBlock);
            //contactListener.DamageFruit += new ContactListener.DamageEventHandler(contactListener_DamageFruit);
            //contactListener.DamageVeggie += new ContactListener.DamageEventHandler(contactListener_DamageVeggie);
            //contactListener.DamageExplosive += new ContactListener.DamageEventHandler(contactListener_DamageExplosive);
            contactListener.DamageSnowball += new ContactListener.DamageEventHandler(contactListener_DamageSnowball);
            contactListener.Poof += new ContactListener.EffectEventHandler(contactListener_Poof);
            contactListener.SmallPoof += new ContactListener.EffectEventHandler(contactListener_SmallPoof);
            contactListener.ImpactPoofs += new ContactListener.EffectEventHandler(contactListener_ImpactPoofs);
            contactListener.SnowballThrown += new ContactListener.EffectEventHandler(contactListener_SnowballThrown);
            contactListener.TowerSawShot += new ContactListener.EffectEventHandler(contactListener_TowerSawShot);
            contactListener.NewSpiderThread += new ContactListener.EffectEventHandler(contactListener_NewSpiderThread);
            contactListener.LemonActivated += new ContactListener.EffectEventHandler(contactListener_LemonActivated);
            contactListener.WaterSplash += new ContactListener.EffectEventHandler(contactListener_WaterSplash);
            contactListener.LavaSplash += new ContactListener.EffectEventHandler(contactListener_LavaSplash);
            contactListener.EmberCreated += new ContactListener.EffectEventHandler(contactListener_EmberCreated);
            contactListener.DropCreated += new ContactListener.EffectEventHandler(contactListener_DropCreated);
            contactListener.WindParticleCreated += new ContactListener.EffectEventHandler(contactListener_WindParticleCreated);
            contactListener.FruitMotionBlur += new ContactListener.EffectEventHandler(contactListener_FruitMotionBlur);
            contactListener.CreateTNTBarrel += new ContactListener.EffectEventHandler(contactListener_CreateTNTBarrel);
        }



        void contactListener_LemonActivated(object sender, EffectEventArgs e)
        {
            Sprite acid = new Sprite(13, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(126, 126), true);
            acid.SpriteType = Sprite.Type.Acid;
            acid.AnimationFPS = 10;
            acid.Scale = 1.8f*e.spriteA.Scale;
            acid.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            acid.TintColor = new Color(30, 30, 0, 10);
            acid.IsCollidable = true;
            acid.spriteBody = BodyFactory.CreateCircle(physicsWorld, ConvertUnits.ToSimUnits(acid.SpriteRectangle.Width * 0.66f * e.spriteA.Scale), 1.0f, ConvertUnits.ToSimUnits(acid.SpriteCenterInWorld), acid);
            acid.spriteBody.BodyType = BodyType.Dynamic;
            acid.spriteBody.IsSensor = true;
            acid.spriteBody.IgnoreGravity = true;
            EffectSprites.Add(acid);
            Sprite smoke = new Sprite(13, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(126, 126) - new Vector2(-32, 0), true);
            smoke.SpriteType = Sprite.Type.Acid;
            smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
            smoke.Scale = 1.4f * e.spriteA.Scale;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.TintColor = new Color(30, 30, 0, 10);
            EffectSprites.Add(smoke);
            smoke = new Sprite(13, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(126, 126) - new Vector2(32, 0), true);
            smoke.SpriteType = Sprite.Type.Acid;
            smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
            smoke.Scale = 1.4f * e.spriteA.Scale;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.TintColor = new Color(30, 30, 0, 10);
            EffectSprites.Add(smoke);
            smoke = new Sprite(13, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(126, 126) - new Vector2(0, 32), true);
            smoke.SpriteType = Sprite.Type.Acid;
            smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
            smoke.Scale = 1.4f * e.spriteA.Scale;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.TintColor = new Color(30, 30, 0, 10);
            EffectSprites.Add(smoke);
            smoke = new Sprite(13, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(126, 126) - new Vector2(0, -32), true);
            smoke.SpriteType = Sprite.Type.Acid;
            smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
            smoke.Scale = 1.4f * e.spriteA.Scale;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.TintColor = new Color(30, 30, 0, 10);
            EffectSprites.Add(smoke);
            smoke = new Sprite(13, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(126, 126) - new Vector2(32, 32), true);
            smoke.SpriteType = Sprite.Type.Acid;
            smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
            smoke.Scale = 1.0f * e.spriteA.Scale;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.TintColor = new Color(30, 30, 0, 10);
            EffectSprites.Add(smoke);
            smoke = new Sprite(13, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(126, 126) - new Vector2(32, -32), true);
            smoke.SpriteType = Sprite.Type.Acid;
            smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
            smoke.Scale = 1.0f * e.spriteA.Scale;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.TintColor = new Color(30, 30, 0, 10);
            EffectSprites.Add(smoke);
            smoke = new Sprite(13, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(126, 126) - new Vector2(-32, 32), true);
            smoke.SpriteType = Sprite.Type.Acid;
            smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
            smoke.Scale = 1.0f * e.spriteA.Scale;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.TintColor = new Color(30, 30, 0, 10);
            EffectSprites.Add(smoke);
            smoke = new Sprite(13, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(126, 126) - new Vector2(-32, -32), true);
            smoke.SpriteType = Sprite.Type.Acid;
            smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
            smoke.Scale = 1.0f * e.spriteA.Scale;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.TintColor = new Color(30, 30, 0, 10);
            EffectSprites.Add(smoke);
            return;
        }

        #region DAMAGE NUMBER EFFECT HANDLER

        //void contactListener_DamageVeggie(object sender, DamageEventArgs e)
        //{
        //    if (e.damageType == Sprite.Type.Acid) return;
        //    CreateDamageNumberEffect(e.sprite.SpriteCenterInWorld, e.damage);
        //}

        //void contactListener_DamageFruit(object sender, DamageEventArgs e)
        //{
        //    if (e.damageType == Sprite.Type.Acid) return;
        //    CreateDamageNumberEffect(e.sprite.SpriteCenterInWorld, e.damage);
        //}

        //void contactListener_DamageBlock(object sender, DamageEventArgs e)
        //{
         //   if (e.damageType == Sprite.Type.Acid) return;
        //    CreateDamageNumberEffect(e.sprite.SpriteCenterInWorld, e.damage);
        //}

        //void contactListener_DamageExplosive(object sender, DamageEventArgs e)
        //{
        //    CreateDamageNumberEffect(e.sprite.SpriteCenterInWorld, e.damage);
        //}

        //void CreateDamageNumberEffect(Vector2 center, int damage)
        //{
        //    if(!GameSettings.DebugViewEnabled)  return;
        //    if (damage == 0) return;
        //    if (!GameSettings.BlockDeathEnabled) return;
        //     Sprite damageHP = new Sprite(28, 0, center - new Vector2 (8,8), true);
        //    damageHP.HitPoints = damage;
        //    damageHP.IsVisible = true;
        //    damageHP.Scale = 0.5f;
        //    EffectSprites.Add(damageHP); 
        //}
        #endregion

        void contactListener_DamageSnowball(object sender, DamageEventArgs e)
        {
            e.sprite.HitPoints -= e.damage;
            if (e.sprite.HitPoints <= 0)
            {
                e.sprite.IsHit = true;
            }
        }
        void contactListener_CreatureExploded(object sender, EffectEventArgs e)
        {
            int splatTextureIndex = 0;
            int chunkTextureIndex = 24;
            if (e.spriteA.SpriteType == Sprite.Type.Beehive) 
            {
                splatTextureIndex = 18;
                chunkTextureIndex = 21;
            }
            if (e.spriteA.SpriteType == Sprite.Type.FireBoo)
            {
                contactListener.ExplodeFireShot(e.spriteA);
                return;
            }


            int totalChunks = 36;
            //create chunky effects

            if (e.spriteA.Scale == 1.5f) totalChunks = 72;

            for (int i = 0; i < totalChunks; i++)
            {
                Sprite chunk = new Sprite(6, chunkTextureIndex + LevelDataManager.rand.Next(0, 3), e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(8, 8), true); //create a chunk matching to the veggie origin
                chunk.HitPoints = 2000; //ms timer
                chunk.IsAwake = true;
                chunk.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                chunk.Scale = (float)LevelDataManager.rand.Next(50, 151) / 100f * e.spriteA.Scale;

                //setup manual physics calculation variables
                chunk.RotationSpeed = LevelDataManager.rand.Next(-800, 801);
                Vector2 velocity = ConvertUnits.ToDisplayUnits(e.spriteA.spriteBody.LinearVelocity) + new Vector2(LevelDataManager.rand.Next(-400, 401), LevelDataManager.rand.Next(-400, 401));
                chunk.Velocity = velocity.Length();
                velocity.Normalize();
                chunk.Direction = velocity;

                EffectSprites.Add(chunk);
            }

            if (e.spriteA.SpriteType == Sprite.Type.Bird)
            {
                for (int i = 0; i < 10; i++)
                {
                    Sprite chunk = new Sprite(2, 2, e.spriteA.SpriteCenterInWorld - new Vector2(16, 16), true); //create a chunk bottom center of the sprite
                    chunk.HitPoints = 2000; //ms timer
                    chunk.IsAwake = true;
                    chunk.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                    chunk.Scale = (float)LevelDataManager.rand.Next(50, 101) * 0.01f * e.spriteA.Scale;

                    //setup manual physics calculation variables
                    chunk.RotationSpeed = LevelDataManager.rand.Next(-400, 401);
                    Vector2 velocity = ConvertUnits.ToDisplayUnits(e.spriteA.spriteBody.LinearVelocity) + new Vector2(LevelDataManager.rand.Next(-400, 401), LevelDataManager.rand.Next(-400,400));
                    chunk.Velocity = velocity.Length();
                    velocity.Normalize();
                    chunk.Direction = velocity;

                    EffectSprites.Add(chunk);
                }
            }


            Sprite splat = new Sprite(7, splatTextureIndex, e.spriteA.Location, true);
            splat.AnimationFPS = 14;
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.Scale = 1.4f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X - 68, (int)e.spriteA.SpriteRectangle.Y - 68, 200, 200);
            EffectSprites.Add(splat);
            splat = new Sprite(7, splatTextureIndex, e.spriteA.Location, true);
            splat.AnimationFPS = 12;
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.Scale = 1.2f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X - 68, (int)e.spriteA.SpriteRectangle.Y - 68, 200, 200);
            EffectSprites.Add(splat);
            splat = new Sprite(7, splatTextureIndex, e.spriteA.Location, true);
            splat.AnimationFPS = 10;
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.Scale = 1.0f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X - 68, (int)e.spriteA.SpriteRectangle.Y - 68, 200, 200);
            EffectSprites.Add(splat);
            splat = new Sprite(7, splatTextureIndex, e.spriteA.Location, true);
            splat.AnimationFPS = 12;
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.Scale = 0.8f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X - 68, (int)e.spriteA.SpriteRectangle.Y - 68, 200, 200);
            EffectSprites.Add(splat);
            splat = new Sprite(7, splatTextureIndex, e.spriteA.Location, true);
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.AnimationFPS = 14;
            splat.Scale = 0.6f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X - 68, (int)e.spriteA.SpriteRectangle.Y - 68, 200, 200);
            EffectSprites.Add(splat);
            return;
        }
        void contactListener_FanActivated(object sender, EffectEventArgs e)
        {
            //for fan wind, is bounce animated is the on off signal
            Sprite wind = (Sprite)e.spriteA.spriteBody.FixtureList[1].UserData;
            if (wind == null)
            {
                wind = new Sprite(21, 0, e.spriteA.Location, true);
                wind.TotalRotation = e.spriteA.TotalRotation;
                Vector2 windForce = new Vector2((float)Math.Cos(e.spriteA.TotalRotation + MathHelper.ToRadians(-90)), (float)Math.Sin(e.spriteA.TotalRotation + MathHelper.ToRadians(-90)));
                Vector2 windLocationCenter = new Vector2(e.spriteA.SpriteCenterInWorld.X + (windForce.X * 172), e.spriteA.SpriteCenterInWorld.Y + (windForce.Y * 172));
                wind.HitPoints = (int)e.spriteA.Velocity;
                wind.SpriteRectangle = new Rectangle((int)windLocationCenter.X - 32, (int)windLocationCenter.Y - 160, 64, 320);
                wind.SpriteType = Sprite.Type.FanWind;
                wind.IsAwake = true;
                wind.IsAnimated = true;
                wind.IsAnimatedWhileStopped = true;
                wind.IsBounceAnimated = true;
                wind.IsVisible = false;
                wind.IsCollidable = false;
                wind.AnimationFPS = 6;
                wind.TintColor = new Color(0.2f, 0.2f, 0.2f, 0.2f);
                wind.Direction = windForce;
                wind.spriteBody = e.spriteA.spriteBody;
                wind.spriteBody.FixtureList[1].UserData = wind;
                EffectSprites.Add(wind);
            }
            else
            {
                wind.TotalRotation = e.spriteA.TotalRotation;
                Vector2 windForce = new Vector2((float)Math.Cos(wind.TotalRotation + MathHelper.ToRadians(-90)), (float)Math.Sin(wind.TotalRotation + MathHelper.ToRadians(-90)));
                Vector2 windLocationCenter = new Vector2(ConvertUnits.ToDisplayUnits(wind.spriteBody.Position.X) + (windForce.X * 172),
                                                         ConvertUnits.ToDisplayUnits(wind.spriteBody.Position.Y) + (windForce.Y * 172));
                wind.SpriteRectangle = new Rectangle((int)windLocationCenter.X - 32, (int)windLocationCenter.Y - 160, 64, 320);
                wind.Direction = windForce;
                wind.IsBounceAnimated = true;
                wind.IsVisible = false;
                wind.HitPoints = (int)e.spriteA.Velocity;
            }
            return;
        }
        void contactListener_ShotFired(object sender, EffectEventArgs e)
        {
            Sprite smoke = new Sprite(1, 0, e.location, true);
            smoke.AnimationFPS = 12;
            smoke.TotalRotation = e.spriteA.TotalRotation + MathHelper.ToRadians(-90);
            smoke.SpriteRectangle = new Rectangle((int)e.location.X-62, (int)e.location.Y-62, 124, 124);
            smoke.Scale = e.spriteA.Scale;
            smoke.TintColor *= 0.33f;
            EffectSprites.Add(smoke);
            smoke = new Sprite(16, 0, e.location, true);
            smoke.AnimationFPS = 12;
            smoke.TotalRotation = e.spriteA.TotalRotation + MathHelper.ToRadians(0);
            smoke.SpriteRectangle = new Rectangle((int)e.location.X-62, (int)e.location.Y-62, 124, 124);
            smoke.Scale = e.spriteA.Scale;
            smoke.TintColor *= 0.4f;
            EffectSprites.Add(smoke);
        }
        void contactListener_BombExploded(object sender, EffectEventArgs e)
        {
            explosion = new Explosion(physicsWorld);
            Vector2 explosionLocation = e.spriteA.spriteBody.Position;
            float explosionRadius = 0f;
            float explosionForce = 0f;
            if (e.spriteA.TextureID == 48)
            {
                explosionRadius = ConvertUnits.ToSimUnits(GameSettings.ExplosiveForceRadiusMultiplier * 9f);
                explosionForce = GameSettings.ExplosivePower * 9f;  //large tnt
            }
            if (e.spriteA.TextureID == 49)
            {
                explosionRadius = ConvertUnits.ToSimUnits(GameSettings.ExplosiveForceRadiusMultiplier * 3f);
                explosionForce = GameSettings.ExplosivePower * 3f; //medium tnt
            }
            if (e.spriteA.TextureID == 50 || e.spriteA.SpriteType == Sprite.Type.Veggie)
            {
                explosionRadius = ConvertUnits.ToSimUnits(GameSettings.ExplosiveForceRadiusMultiplier);
                explosionForce = GameSettings.ExplosivePower; //small tnt
            }
            if (e.spriteA.TextureID == 20)
            {
                explosionRadius = ConvertUnits.ToSimUnits(GameSettings.ExplosiveForceRadiusMultiplier * e.spriteA.Scale);
                explosionForce = GameSettings.ExplosivePower * 0.5f * e.spriteA.Scale; //cherry
            }

            if (GameSettings.CheatFunsplosions)
            {
                explosionRadius *= 2f;
                explosionForce *= 2f;
            }

            explosion.Activate(explosionLocation, explosionRadius, explosionForce);




            contactListener.DoPoof(e.spriteA);
            Sprite smoke = new Sprite(13, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(126,126) ,true);
            smoke.AnimationFPS = 12;
            smoke.Scale = (float)e.spriteA.SpriteRectWidth / 64f;
            smoke.TintColor *= 0.33f;
            EffectSprites.Add(smoke);
            smoke = new Sprite(14, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(62, 62), true);
            smoke.AnimationFPS = LevelDataManager.rand.Next(6, 12);
            smoke.Scale = (float)e.spriteA.SpriteRectWidth / 64f;
            smoke.TotalRotation = (float)(LevelDataManager.rand.Next(0, 314)) / 100.0f;
            smoke.TintColor *= 0.33f;
            EffectSprites.Add(smoke);
            smoke = new Sprite(14, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(62, 62), true);
            smoke.AnimationFPS = LevelDataManager.rand.Next(6, 12);
            smoke.Scale = (float)e.spriteA.SpriteRectWidth / 64f;
            smoke.TotalRotation = (float)(LevelDataManager.rand.Next(0, 314)) / 100.0f;
            smoke.TintColor *= 0.33f;
            EffectSprites.Add(smoke);
            smoke = new Sprite(15, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(30, 30), true);
            smoke.AnimationFPS = LevelDataManager.rand.Next(6, 12);
            smoke.Scale = (float)e.spriteA.SpriteRectWidth / 64f;
            smoke.TotalRotation = (float)(LevelDataManager.rand.Next(0, 314)) / 100.0f;
            smoke.TintColor *= 0.33f;
            EffectSprites.Add(smoke);
            smoke = new Sprite(15, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(30, 30), true);
            smoke.AnimationFPS = LevelDataManager.rand.Next(6, 12);
            smoke.Scale = (float)e.spriteA.SpriteRectWidth / 64f;
            smoke.TotalRotation = (float)(LevelDataManager.rand.Next(0, 314)) / 100.0f;
            smoke.TintColor *= 0.33f;
            EffectSprites.Add(smoke);
            Sprite bomb = new Sprite(0, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(64, 64), true);
            bomb.HitPoints = 12;
            bomb.TotalRotation = 0.0001f;
            bomb.Scale = (float)e.spriteA.SpriteRectWidth / 64f;
            Vector2 position = ConvertUnits.ToSimUnits(bomb.SpriteCenterInWorld);
            float bombRadius = ConvertUnits.ToSimUnits(e.spriteA.SpriteRectWidth);
            if (e.spriteA.TextureID == 20)
            {
                bombRadius *= 0.5f *e.spriteA.Scale;
                bomb.Scale *= 0.5f *e.spriteA.Scale;
            }
            bomb.spriteBody = BodyFactory.CreateCircle(physicsWorld,bombRadius,1.0f,position, bomb);
            bomb.spriteBody.BodyType = BodyType.Dynamic;
            bomb.spriteBody.ResetDynamics();
            bomb.spriteBody.IsSensor = true;
            bomb.spriteBody.IgnoreGravity = true;
            bomb.SpriteType = Sprite.Type.Explosion;
            EffectSprites.Add(bomb);
            return;
        }
        void contactListener_FruitExploded(object sender, EffectEventArgs e)
        {
            int splatTextureIndex = 0;
            int chunkTextureIndex = 0;
            int totalChunks = 32;
            if (e.spriteA.Scale == 1.5f) totalChunks = 64;
            switch (e.spriteA.TextureIndex)
            {
                case 0:
                    {
                        splatTextureIndex = 0;
                        chunkTextureIndex = 0;
                        break;
                    }
                case 7:
                    {
                        splatTextureIndex = 6;
                        chunkTextureIndex = 3;
                        break;
                    }
                case 14:
                    {
                        splatTextureIndex = 12;
                        chunkTextureIndex = 6;
                        break;
                    }
                case 21:
                case 70:
                    {
                        splatTextureIndex = 18;
                        chunkTextureIndex = 9;
                        break;
                    }
                case 28:
                    {
                        splatTextureIndex = 0;
                        chunkTextureIndex = 12;
                        break;
                    }
                case 35:
                    {
                        splatTextureIndex = 0;
                        chunkTextureIndex = 24;
                        break;
                    }
                case 42:
                    {
                        splatTextureIndex = 12;
                        chunkTextureIndex = 15;
                        break;
                    }
                case 49:
                    {
                        splatTextureIndex = 24;
                        chunkTextureIndex = 21;
                        break;
                    }
                case 56:
                    {
                        splatTextureIndex = 30;
                        chunkTextureIndex = 18;
                        break;
                    }
                case 63:
                    {
                        splatTextureIndex = 0;
                        chunkTextureIndex = 24;
                        totalChunks = 8;
                        break;
                    }
                default:
                    break;
            }

            //create chunky effects
            for (int i = 0; i < totalChunks; i++)
            {
                Sprite chunk = new Sprite(6, chunkTextureIndex + LevelDataManager.rand.Next(0, 3), e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(8, 8),true); //create a chunk matching to the veggie origin;
                chunk.HitPoints = 2000; //ms timer
                chunk.IsAwake = true;
                chunk.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                chunk.Scale = (float)LevelDataManager.rand.Next(50, 151) *0.01f * e.spriteA.Scale;

                //setup manual physics calculation variables
                chunk.RotationSpeed = LevelDataManager.rand.Next(-800, 800);
                Vector2 velocity = new Vector2(LevelDataManager.rand.Next(-300, 301), LevelDataManager.rand.Next(-400, 201));
                if (e.spriteA.spriteBody != null)
                {
                    velocity += ConvertUnits.ToDisplayUnits(e.spriteA.spriteBody.LinearVelocity);
                }
                chunk.Velocity = Math.Min(2000,velocity.Length());
                velocity.Normalize();
                chunk.Direction = velocity;

                EffectSprites.Add(chunk);
            }



            Sprite splat = new Sprite(7, splatTextureIndex, e.spriteA.Location, true);
            splat.AnimationFPS = 14;
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.Scale = 1.4f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X - 68, (int)e.spriteA.SpriteRectangle.Y - 68, 200, 200);
            EffectSprites.Add(splat);
            splat = new Sprite(7, splatTextureIndex, e.spriteA.Location, true);
            splat.AnimationFPS = 12;
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.Scale = 1.2f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X - 68, (int)e.spriteA.SpriteRectangle.Y - 68, 200, 200);
            EffectSprites.Add(splat);
            splat = new Sprite(7, splatTextureIndex, e.spriteA.Location,true);
            splat.AnimationFPS = 10;
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.Scale = 1.0f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X-68,(int)e.spriteA.SpriteRectangle.Y-68, 200, 200);
            EffectSprites.Add(splat);
            splat = new Sprite(7, splatTextureIndex, e.spriteA.Location, true);
            splat.AnimationFPS = 12;
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.Scale = 0.8f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X - 68, (int)e.spriteA.SpriteRectangle.Y - 68, 200, 200);
            EffectSprites.Add(splat);
            splat = new Sprite(7, splatTextureIndex, e.spriteA.Location, true);
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.AnimationFPS = 14;
            splat.Scale = 0.6f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X - 68, (int)e.spriteA.SpriteRectangle.Y - 68, 200, 200);
            EffectSprites.Add(splat);

            return;
        }
        void contactListener_VeggieExploded(object sender, EffectEventArgs e)
        {
            int splatTextureIndex = 0;
            int chunkTextureIndex = 0;
            int totalChunks = 32;
            if (e.spriteA.Scale == 1.5f) totalChunks = 64;
            switch (e.spriteA.TextureIndex)
            {
                case 0:
                    {
                        splatTextureIndex = 0;
                        chunkTextureIndex = 0;
                        break;
                    }
                case 7:
                    {
                        splatTextureIndex = 6;
                        chunkTextureIndex = 3;
                        break;
                    }
                case 14:
                    {
                        splatTextureIndex = 12;
                        chunkTextureIndex = 6;
                        break;
                    }
                case 21:
                    {
                        splatTextureIndex = 18;
                        chunkTextureIndex = 9;
                        break;
                    }
                case 28:
                    {
                        splatTextureIndex = 24;
                        chunkTextureIndex = 12;
                        break;
                    }
                case 35:
                    {
                        splatTextureIndex = 30;
                        chunkTextureIndex = 15;
                        break;
                    }
                case 42:
                    {
                        splatTextureIndex = 36;
                        chunkTextureIndex = 18;
                        break;
                    }
                case 49:
                    {
                        splatTextureIndex = 42;
                        chunkTextureIndex = 21;
                        break;
                    }
                case 56:
                    {
                        splatTextureIndex = 42;
                        chunkTextureIndex = 21;
                        break;
                    }
                case 63:
                    {
                        splatTextureIndex = 48;
                        chunkTextureIndex = 24;
                        break;
                    }
                case 70:
                    {
                        splatTextureIndex = 30;
                        chunkTextureIndex = 27;
                        break;
                    }
                case 77:
                    {
                        splatTextureIndex = 36;
                        chunkTextureIndex = 30;
                        break;
                    }
                case 84: //patch thing
                    {
                        splatTextureIndex = 24;
                        chunkTextureIndex = 33;
                        break;
                    }
                case 91: //pea 
                    {
                        splatTextureIndex = 42;
                        chunkTextureIndex = 21;
                        totalChunks = 8;
                        break;
                    }
                case 98: //washer
                    {
                        contactListener.DoPoof(e.spriteA);
                        return;   //no chunks for washer
                        break;
                    }
                default:
                    break;
            }


            //create chunky effects
            for (int i = 0; i < totalChunks; i++)
            {
                Sprite chunk = new Sprite(18, chunkTextureIndex + LevelDataManager.rand.Next(0, 3), e.spriteA.Location + e.spriteA.SpriteOrigin - new Vector2(8, 8),true); //create a chunk matching to the origin
                chunk.HitPoints = 2000; //ms timer
                chunk.IsAwake = true;
                chunk.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                chunk.Scale = (float)LevelDataManager.rand.Next(50, 151) *0.01f * e.spriteA.Scale;
                
                //setup manual physics calculation variables
                chunk.RotationSpeed = LevelDataManager.rand.Next(-800, 800);
                Vector2 velocity = ConvertUnits.ToDisplayUnits(e.spriteA.spriteBody.LinearVelocity) + new Vector2(LevelDataManager.rand.Next(-300, 301), LevelDataManager.rand.Next(-400, 201));
                chunk.Velocity = Math.Min(2000, velocity.Length());
                velocity.Normalize();
                chunk.Direction = velocity;

                EffectSprites.Add(chunk);
            }

            Sprite splat = new Sprite(19, splatTextureIndex, e.spriteA.Location, true);
            splat.AnimationFPS = 14;
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.Scale = 1.4f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X - 68, (int)e.spriteA.SpriteRectangle.Y - 68, 200, 200);
            EffectSprites.Add(splat);
            splat = new Sprite(19, splatTextureIndex, e.spriteA.Location, true);
            splat.AnimationFPS = 12;
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.Scale = 1.2f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X - 68, (int)e.spriteA.SpriteRectangle.Y - 68, 200, 200);
            EffectSprites.Add(splat);
            splat = new Sprite(19, splatTextureIndex, e.spriteA.Location, true);
            splat.AnimationFPS = 10;
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.Scale = 1.0f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X - 68, (int)e.spriteA.SpriteRectangle.Y - 68, 200, 200);
            EffectSprites.Add(splat);
            splat = new Sprite(19, splatTextureIndex, e.spriteA.Location, true);
            splat.AnimationFPS = 12;
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.Scale = 0.8f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X - 68, (int)e.spriteA.SpriteRectangle.Y - 68, 200, 200);
            EffectSprites.Add(splat);
            splat = new Sprite(19, splatTextureIndex, e.spriteA.Location, true);
            splat.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            splat.AnimationFPS = 14;
            splat.Scale = 0.6f * e.spriteA.Scale;
            splat.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteRectangle.X - 68, (int)e.spriteA.SpriteRectangle.Y - 68, 200, 200);
            EffectSprites.Add(splat);
            return;
        }
        void contactListener_BlockExploded(object sender, EffectEventArgs e)
        {
            int chunkTextureIndex = 0;
            int totalChunks = (int)MathHelper.Max(4,(e.spriteA.SpriteRectWidth * e.spriteA.SpriteRectHeight/64)); //should generate 4 chunks per 16x16 area

            //create chunky effects

            if (e.spriteA.TextureIndex == 0) chunkTextureIndex = 0;
            if (e.spriteA.TextureIndex == 1) chunkTextureIndex = 2;
            if (e.spriteA.TextureIndex == 2) chunkTextureIndex = 6;
            if (e.spriteA.TextureIndex == 3) chunkTextureIndex = 4;

            int chunkPlacementRangeX = (int)MathHelper.Max(1,(e.spriteA.SpriteRectWidth - 16)/2); 
            int chunkPlacementRangeY = (int)MathHelper.Max(1,(e.spriteA.SpriteRectHeight - 16)/2);

            float cos = (float)Math.Cos(e.spriteA.TotalRotation);
            float sin = (float)Math.Sin(e.spriteA.TotalRotation);

            for (int i = 0; i < totalChunks; i++)
            {
                Sprite chunk = new Sprite(12, chunkTextureIndex + LevelDataManager.rand.Next(0, 2), e.spriteA.SpriteCenterInWorld - new Vector2(8,8), true); //create a chunk matching to the veggie origin

                chunk.HitPoints = 2000; //ms timer
                chunk.IsAwake = true;
                chunk.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                chunk.Scale = (float)LevelDataManager.rand.Next(50,151) *0.01f * e.spriteA.Scale;

                //chunk origin centered on origin of destructing sprite, add random offset so chunk can appear from anywhere inside the sprite
                Vector2 randomOffset = new Vector2(LevelDataManager.rand.Next(-chunkPlacementRangeX, chunkPlacementRangeX + 1), LevelDataManager.rand.Next(-chunkPlacementRangeY, chunkPlacementRangeY + 1));

                //add rotation offset factor to account for block rotation
                chunk.Location += new Vector2((randomOffset.X * cos) - (randomOffset.Y * sin), (randomOffset.X * sin) + (randomOffset.Y * cos));

                //setup manual physics calculation variables
                chunk.RotationSpeed = LevelDataManager.rand.Next(-800, 800);
                Vector2 velocity = ConvertUnits.ToDisplayUnits(e.spriteA.spriteBody.LinearVelocity) + new Vector2(LevelDataManager.rand.Next(-300, 301), LevelDataManager.rand.Next(-400, 201));
                chunk.Velocity = Math.Min(2000, velocity.Length());
                velocity.Normalize();
                chunk.Direction = velocity;

                EffectSprites.Add(chunk);
            }

            contactListener.DoPoof(e.spriteA);
            return;
        }
        void contactListener_Poof(object sender, EffectEventArgs e)
        {
            int smokeID = 15;
            Vector2 originOffset = new Vector2(30,30);
            float AlphaOffset = 0.33f;
            Sprite smoke;

            #region Block poof
            if (e.spriteA.SpriteType == Sprite.Type.Block)
            {
                if (e.spriteA.SpriteRectHeight == e.spriteA.SpriteRectWidth)
                {
                    smokeID = 14;
                    originOffset = new Vector2(62, 62);
                    float scale = e.spriteA.SpriteRectHeight *0.02f;
                    smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset, true);
                    smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
                    smoke.Scale = scale;
                    smoke.TintColor *= AlphaOffset;
                    smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                    EffectSprites.Add(smoke);
                    smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset - (new Vector2(16,0)*scale), true);
                    smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
                    smoke.Scale = scale;
                    smoke.TintColor *= AlphaOffset;
                    smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                    EffectSprites.Add(smoke);
                    smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset - (new Vector2(-16, 0)*scale), true);
                    smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
                    smoke.Scale = scale;
                    smoke.TintColor *= AlphaOffset;
                    smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                    EffectSprites.Add(smoke);
                    smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset - (new Vector2(0, 16)*scale), true);
                    smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
                    smoke.Scale = scale;
                    smoke.TintColor *= AlphaOffset;
                    smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                    EffectSprites.Add(smoke);
                    smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset - (new Vector2(0,-16)*scale), true);
                    smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
                    smoke.Scale = scale;
                    smoke.TintColor *= AlphaOffset;
                    smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                    EffectSprites.Add(smoke);
                    return;
                }
                else
                {
                    smokeID = 14;
                    originOffset = new Vector2(62, 62);
                    float scale = ((e.spriteA.SpriteRectWidth *0.25f) + (e.spriteA.SpriteRectHeight*0.75f)) *0.02f;
                    Vector2 rotationOffset = new Vector2((float)Math.Cos(e.spriteA.TotalRotation), (float)Math.Sin(e.spriteA.TotalRotation)) * e.spriteA.SpriteRectWidth*0.5f;
                    smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset, true);
                    smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
                    smoke.Scale = scale;
                    smoke.TintColor *= AlphaOffset;
                    smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                    EffectSprites.Add(smoke);
                    smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset - (rotationOffset), true);
                    smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
                    smoke.Scale = scale;
                    smoke.TintColor *= AlphaOffset;
                    smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                    EffectSprites.Add(smoke);
                    smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset - (rotationOffset*0.66f), true);
                    smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
                    smoke.Scale = scale;
                    smoke.TintColor *= AlphaOffset;
                    smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                    EffectSprites.Add(smoke);
                    smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset - (rotationOffset * 0.33f), true);
                    smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
                    smoke.Scale = scale;
                    smoke.TintColor *= AlphaOffset;
                    smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                    EffectSprites.Add(smoke);
                    smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset + (rotationOffset), true);
                    smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
                    smoke.Scale = scale;
                    smoke.TintColor *= AlphaOffset;
                    smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                    EffectSprites.Add(smoke);
                    smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset + (rotationOffset*0.66f), true);
                    smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
                    smoke.Scale = scale;
                    smoke.TintColor *= AlphaOffset;
                    smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                    EffectSprites.Add(smoke);
                    smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset + (rotationOffset * 0.33f), true);
                    smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
                    smoke.Scale = scale;
                    smoke.TintColor *= AlphaOffset;
                    smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
                    EffectSprites.Add(smoke);
                    return;
                }
            }
            #endregion


            if (e.spriteA.SpriteType == Sprite.Type.SawShot || e.spriteA.SpriteType == Sprite.Type.CannonballShot || e.spriteA.SpriteType == Sprite.Type.Veggie)
            {
                smokeID = 14;
                originOffset = new Vector2(62, 62);
            }
            if (e.spriteA.SpriteType == Sprite.Type.PowerUp)
            {
                smokeID = 13;
                originOffset = new Vector2(126, 126);
            }
            if (e.spriteA.SpriteType == Sprite.Type.FireballShot)
            {
                smokeID = 13;
                originOffset = new Vector2(126, 126);
                AlphaOffset = 0.2f;
            }
            if (e.spriteA.SpriteType == Sprite.Type.FireBoo)
            {
                AlphaOffset = 0.15f;
            }




            smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset - new Vector2(-16, 0), true);
            smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
            smoke.Scale = 1.0f;
            smoke.TintColor *= AlphaOffset;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            EffectSprites.Add(smoke);
            smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset - new Vector2(16, 0), true);
            smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
            smoke.Scale = 1.0f;
            smoke.TintColor *= AlphaOffset;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            EffectSprites.Add(smoke);
            smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset - new Vector2(0, 16), true);
            smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
            smoke.Scale = 1.0f;
            smoke.TintColor *= AlphaOffset;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            EffectSprites.Add(smoke);
            smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset - new Vector2(0, -16), true);
            smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
            smoke.Scale = 1.0f;
            smoke.TintColor *= AlphaOffset;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke = new Sprite(smokeID, 0, e.spriteA.Location + e.spriteA.SpriteOrigin - originOffset, true);
            smoke.AnimationFPS = LevelDataManager.rand.Next(8, 13);
            smoke.Scale = 1.2f;
            smoke.TintColor *= AlphaOffset;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            EffectSprites.Add(smoke);
            return;
        }
        void contactListener_SmallPoof(object sender, EffectEventArgs e)
        {
            int smokeID = 15;
            Vector2 originOffset = new Vector2(30,30);
            float alpha = 0.33f;

            Sprite smoke = new Sprite(smokeID, 0, e.location - originOffset, true);
            smoke.AnimationFPS = 12;
            smoke.Scale = 1.0f;
            smoke.TintColor *= alpha;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            EffectSprites.Add(smoke);
        }
        void contactListener_ImpactPoofs(object sender, EffectEventArgs e)
        {
            int smokeID = 14;
            Vector2 originOffset = new Vector2(62, 62);
            float alpha = 0.33f;

            Sprite smoke = new Sprite(smokeID, 0, e.location - originOffset, true);
            smoke.AnimationFPS = 12;
            smoke.Scale = 1.0f;
            smoke.TintColor *= alpha;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.Velocity = -300;
            smoke.Direction = Vector2.UnitX; 
            EffectSprites.Add(smoke);
            smoke = new Sprite(smokeID, 0, e.location - originOffset, true);
            smoke.AnimationFPS = 12;
            smoke.Scale = 1.0f;
            smoke.TintColor *= alpha;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.Velocity = -200;
            smoke.Direction = Vector2.UnitX;
            EffectSprites.Add(smoke);
            smoke = new Sprite(smokeID, 0, e.location - originOffset, true);
            smoke.AnimationFPS = 12;
            smoke.Scale = 1.0f;
            smoke.TintColor *= alpha;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.Velocity = -100;
            smoke.Direction = Vector2.UnitX;
            EffectSprites.Add(smoke);
            smoke = new Sprite(smokeID, 0, e.location - originOffset, true);
            smoke.AnimationFPS = 12;
            smoke.Scale = 1.0f;
            smoke.TintColor *= alpha;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.Velocity = 100;
            smoke.Direction = Vector2.UnitX;
            EffectSprites.Add(smoke);
            smoke = new Sprite(smokeID, 0, e.location - originOffset, true);
            smoke.AnimationFPS = 12;
            smoke.Scale = 1.0f;
            smoke.TintColor *= alpha;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.Velocity = 200;
            smoke.Direction = Vector2.UnitX;
            EffectSprites.Add(smoke);
            smoke = new Sprite(smokeID, 0, e.location - originOffset, true);
            smoke.AnimationFPS = 12;
            smoke.Scale = 1.0f;
            smoke.TintColor *= alpha;
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.Velocity = 300;
            smoke.Direction = Vector2.UnitX;
            EffectSprites.Add(smoke);
        }
        void contactListener_NewSpiderThread(object sender, EffectEventArgs e)
        {
            Sprite thread = new Sprite(20,0,e.spriteA.pathingPoints[0],true);
            thread.SpriteRectangle = new Rectangle((int)e.spriteA.pathingPoints[0].X + 32, (int)e.spriteA.pathingPoints[0].Y+6, 1, (int)(e.spriteA.Location.Y - e.spriteA.pathingPoints[0].Y));
            thread.TintColor *= 0.3f;
            EffectSprites.Add(thread);
            return;
        }
        void contactListener_FireShotExploded(object sender, EffectEventArgs e)
        {
            Sprite smoke = new Sprite(13, 0, e.spriteA.Location, true);
            smoke.AnimationFPS = LevelDataManager.rand.Next(6, 12);
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.Scale = 1.4f * e.spriteA.Scale;
            smoke.TintColor *= 0.33f;
            smoke.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteCenterInWorld.X - 126, (int)e.spriteA.SpriteCenterInWorld.Y - 126, 252, 252);
            EffectSprites.Add(smoke);
            smoke = new Sprite(13, 0, e.spriteA.Location, true);
            smoke.AnimationFPS = LevelDataManager.rand.Next(6, 12);
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.Scale = 1.2f * e.spriteA.Scale;
            smoke.TintColor *= 0.33f;
            smoke.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteCenterInWorld.X - 126, (int)e.spriteA.SpriteCenterInWorld.Y - 126, 252, 252);
            EffectSprites.Add(smoke);
            smoke = new Sprite(13, 0, e.spriteA.Location, true);
            smoke.AnimationFPS = LevelDataManager.rand.Next(6, 12);
            smoke.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(0, 360));
            smoke.Scale = 1.0f * e.spriteA.Scale;
            smoke.TintColor *= 0.33f;
            smoke.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteCenterInWorld.X - 126, (int)e.spriteA.SpriteCenterInWorld.Y - 126, 252, 252);
            EffectSprites.Add(smoke);

            Sprite fireblast = new Sprite(4, 0, e.spriteA.Location, true); ;
            fireblast.AnimationFPS = 12;
            fireblast.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(1, 360));
            fireblast.Scale = 1.2f * e.spriteA.Scale;
            fireblast.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteCenterInWorld.X - 126, (int)e.spriteA.SpriteCenterInWorld.Y - 126, 252, 252);
            EffectSprites.Add(fireblast);

            fireblast = new Sprite(4, 0, e.spriteA.Location, true);
            fireblast.AnimationFPS = 10;
            fireblast.TotalRotation = e.spriteA.TotalRotation;
            fireblast.Scale = 1.0f * e.spriteA.Scale;
            fireblast.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteCenterInWorld.X - 126, (int)e.spriteA.SpriteCenterInWorld.Y - 126, 252, 252);
            fireblast.HitPoints = 30;
            Vector2 position = ConvertUnits.ToSimUnits(fireblast.SpriteCenterInWorld);
            float explosionRadius = ConvertUnits.ToSimUnits(e.spriteA.SpriteRectWidth * fireblast.Scale *0.5f);
            fireblast.spriteBody = BodyFactory.CreateCircle(physicsWorld, explosionRadius, 1.0f, position, fireblast);
            fireblast.spriteBody.BodyType = BodyType.Dynamic;
            fireblast.spriteBody.ResetDynamics();
            fireblast.spriteBody.IsSensor = true;
            fireblast.spriteBody.IgnoreGravity = true;
            fireblast.SpriteType = Sprite.Type.FireballBlast;
            EffectSprites.Add(fireblast);

            fireblast = new Sprite(4, 0, e.spriteA.Location, true);
            fireblast.AnimationFPS = 12;
            fireblast.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(1, 360));
            fireblast.Scale = 0.8f * e.spriteA.Scale;
            fireblast.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteCenterInWorld.X - 126, (int)e.spriteA.SpriteCenterInWorld.Y - 126, 252, 252);
            EffectSprites.Add(fireblast);


            return;
        }
        void contactListener_IceShotExploded(object sender, EffectEventArgs e)
        {
            Color iceblastColor = new Color(100, 100, 255);

            Sprite iceblast = new Sprite(4, 0, e.spriteA.Location, true); ;
            iceblast.TintColor = iceblastColor;
            iceblast.AnimationFPS = 12;
            iceblast.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(1, 360));
            iceblast.Scale = 1.2f * e.spriteA.Scale;
            iceblast.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteCenterInWorld.X - 126, (int)e.spriteA.SpriteCenterInWorld.Y - 126, 252, 252);
            EffectSprites.Add(iceblast);

            iceblast = new Sprite(4, 0, e.spriteA.Location, true);
            iceblast.TintColor = iceblastColor;
            iceblast.AnimationFPS = 10;
            iceblast.TotalRotation = e.spriteA.TotalRotation;
            iceblast.Scale = 1.0f * e.spriteA.Scale;
            iceblast.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteCenterInWorld.X - 126, (int)e.spriteA.SpriteCenterInWorld.Y - 126, 252, 252);
            iceblast.HitPoints = 30;
            Vector2 position = ConvertUnits.ToSimUnits(iceblast.SpriteCenterInWorld);
            float explosionRadius = ConvertUnits.ToSimUnits(e.spriteA.SpriteRectWidth * iceblast.Scale * 0.5f);
            iceblast.spriteBody = BodyFactory.CreateCircle(physicsWorld, explosionRadius, 1.0f, position, iceblast);
            iceblast.spriteBody.BodyType = BodyType.Dynamic;
            iceblast.spriteBody.ResetDynamics();
            iceblast.spriteBody.IsSensor = true;
            iceblast.spriteBody.IgnoreGravity = true;
            iceblast.SpriteType = Sprite.Type.IceBlast;
            EffectSprites.Add(iceblast);

            iceblast = new Sprite(4, 0, e.spriteA.Location, true);
            iceblast.TintColor = iceblastColor;
            iceblast.AnimationFPS = 12;
            iceblast.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(1, 360));
            iceblast.Scale = 0.8f * e.spriteA.Scale;
            iceblast.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteCenterInWorld.X - 126, (int)e.spriteA.SpriteCenterInWorld.Y - 126, 252, 252);
            EffectSprites.Add(iceblast);
        }
        void contactListener_LitShotExploded(object sender, EffectEventArgs e)
        {
            Sprite litblast = new Sprite(8, 0, e.spriteA.Location, true); ;
            litblast.AnimationFPS = 12;
            litblast.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(1, 360));
            litblast.Scale = 1.2f * e.spriteA.Scale;
            litblast.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteCenterInWorld.X - 254, (int)e.spriteA.SpriteCenterInWorld.Y - 254, 508, 508);
            EffectSprites.Add(litblast);

            litblast = new Sprite(8, 0, e.spriteA.Location, true); ;
            litblast.AnimationFPS = 11;
            litblast.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(1, 360));
            litblast.Scale = 1.1f * e.spriteA.Scale;
            litblast.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteCenterInWorld.X - 254, (int)e.spriteA.SpriteCenterInWorld.Y - 254, 508, 508);
            EffectSprites.Add(litblast);

            litblast = new Sprite(8, 0, e.spriteA.Location, true);
            litblast.AnimationFPS = 10;
            litblast.TotalRotation = e.spriteA.TotalRotation;
            litblast.Scale = 1.0f * e.spriteA.Scale;
            litblast.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteCenterInWorld.X - 254, (int)e.spriteA.SpriteCenterInWorld.Y - 254, 508, 508);
            litblast.HitPoints = 30;
            Vector2 position = ConvertUnits.ToSimUnits(litblast.SpriteCenterInWorld);
            float explosionRadius = ConvertUnits.ToSimUnits(litblast.SpriteRectWidth * litblast.Scale * 0.4f);
            litblast.spriteBody = BodyFactory.CreateCircle(physicsWorld, explosionRadius, 1.0f, position, litblast);
            litblast.spriteBody.BodyType = BodyType.Dynamic;
            litblast.spriteBody.ResetDynamics();
            litblast.spriteBody.IsSensor = true;
            litblast.spriteBody.IgnoreGravity = true;
            litblast.SpriteType = Sprite.Type.LightningBlast;
            EffectSprites.Add(litblast);

            litblast = new Sprite(8, 0, e.spriteA.Location, true); ;
            litblast.AnimationFPS = 11;
            litblast.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(1, 360));
            litblast.Scale = 0.9f * e.spriteA.Scale;
            litblast.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteCenterInWorld.X - 254, (int)e.spriteA.SpriteCenterInWorld.Y - 254, 508, 508);
            EffectSprites.Add(litblast);

            litblast = new Sprite(8, 0, e.spriteA.Location, true);
            litblast.AnimationFPS = 12;
            litblast.TotalRotation = MathHelper.ToRadians(LevelDataManager.rand.Next(1, 360));
            litblast.Scale = 0.8f * e.spriteA.Scale;
            litblast.SpriteRectangle = new Rectangle((int)e.spriteA.SpriteCenterInWorld.X - 254, (int)e.spriteA.SpriteCenterInWorld.Y - 254, 508, 508);
            EffectSprites.Add(litblast);
        }
        void contactListener_SnowballThrown(object sender, EffectEventArgs e)
        {
            e.spriteA.spriteBody.Enabled = false;
            Sprite snowball = new Sprite(17, 0, e.spriteA.SpriteCenterInWorld - new Vector2(8,8), true);
            if (e.spriteA.IsFlippedHorizontally)
                snowball.Location += new Vector2(30, 10);
            else
                snowball.Location += new Vector2(-30, 10);
            snowball.SpriteType = Sprite.Type.Snowball;
            snowball.HitPoints = 1;
            snowball.AnimationFramePrecise = 5000; //timer to fade out in ms
            snowball.IsCollidable = true;
            snowball.IsAwake = true;
            snowball.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits(8),ConvertUnits.ToSimUnits(8), 8, 1.0f, ConvertUnits.ToSimUnits(snowball.SpriteCenterInWorld), snowball);
            snowball.spriteBody.BodyType = BodyType.Dynamic;
            snowball.spriteBody.Mass = 2f;
            snowball.spriteBody.IgnoreCollisionWith(e.spriteA.spriteBody);
            if (e.spriteA.IsFlippedHorizontally) snowball.spriteBody.LinearVelocity = new Vector2(-8, -6);
            else snowball.spriteBody.LinearVelocity = new Vector2(8, -6);
            EffectSprites.Add(snowball);

            e.spriteA.spriteBody.Enabled = true;
        }
        void contactListener_TowerSawShot(object sender, EffectEventArgs e)
        {
            Sprite sprite = new Sprite(46, 0, e.spriteA.Location, false);
            sprite.SpriteType = Sprite.Type.Saw;
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            sprite.IsRotating = true;
            sprite.IsVisible = true;
            EffectSprites.Add(sprite);
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            sprite.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth / 2.0f), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight / 2.0f), 10, 0.9f, position, sprite);
            sprite.spriteBody.Rotation = 0;
            sprite.spriteBody.BodyType = BodyType.Kinematic;
            sprite.spriteBody.IgnoreGravity = true;
            sprite.spriteBody.IsSensor = true;
            sprite.spriteBody.LinearVelocity = new Vector2(towerSawSpeed, 0);
            if (!e.spriteA.IsFlippedHorizontally) sprite.spriteBody.LinearVelocity *= -1f;
            sprite.HitPoints = -10000;

            Sprite tower = new Sprite(10, 0, e.spriteA.Location, false);
            tower.SpriteType = Sprite.Type.Tower;
            tower.HitPoints = -500;
            tower.IsFlippedHorizontally = e.spriteA.IsFlippedHorizontally;
            EffectSprites.Add(tower);
        }
        void contactListener_CreateTNTBarrel(object sender, EffectEventArgs e)
        {
            Sprite sprite = new Sprite(52, 4, e.spriteA.Location + new Vector2(16,16), false);
            sprite.SpriteType = Sprite.Type.PowerUp;
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(24f), ConvertUnits.ToSimUnits(36f), 0.9f, position, sprite);
            sprite.spriteBody.Rotation = sprite.TotalRotation;
            sprite.spriteBody.BodyType = BodyType.Kinematic;
            sprite.spriteBody.IsSensor = true;
            sprite.spriteBody.LinearVelocity = new Vector2(-5, 0);
            sprite.HitPoints = 10000;
            EffectSprites.Add(sprite);
            contactListener.AddThisPowerBarrel(sprite);

            Sprite tower = new Sprite(12, 0, e.spriteA.Location, false);
            tower.SpriteType = Sprite.Type.Tower;
            tower.HitPoints = -500;
            EffectSprites.Add(tower);
        }
        void contactListener_WaterSplash(object sender, EffectEventArgs e)
        {
            int totalChunks = 12;
            for (int i = 0; i < totalChunks; i++)
            {
                Sprite chunk = new Sprite(2, 0, e.spriteA.SpriteCenterInWorld - new Vector2(16, 16), true); //create a chunk bottom center of the sprite
                chunk.Location += new Vector2(LevelDataManager.rand.Next((int)(-e.spriteA.SpriteRectWidth *0.5f), (int)(e.spriteA.SpriteRectWidth *0.5f)),0);
                chunk.HitPoints = 1000; //ms timer
                chunk.IsAwake = true;
                chunk.Scale = (float)LevelDataManager.rand.Next(20, 51) * 0.01f * e.spriteA.Scale;
                Vector2 velocity = new Vector2(LevelDataManager.rand.Next(-150, 151), LevelDataManager.rand.Next(-300,-100));
                chunk.Velocity = velocity.Length();
                velocity.Normalize();
                chunk.Direction = velocity;
                chunk.TintColor = tint;
                EffectSprites.Add(chunk);
            }
        }
        void contactListener_LavaSplash(object sender, EffectEventArgs e)
        {
            int totalChunks = 12;
            for (int i = 0; i < totalChunks; i++)
            {
                Sprite chunk = new Sprite(2, 1, e.spriteA.SpriteCenterInWorld - new Vector2(16, 16), true); //create a chunk bottom center of the sprite
                chunk.Location += new Vector2(LevelDataManager.rand.Next((int)(-e.spriteA.SpriteRectWidth * 0.5f), (int)(e.spriteA.SpriteRectWidth * 0.5f)), 0);
                chunk.HitPoints = 1000; //ms timer
                chunk.IsAwake = true;
                chunk.Scale = (float)LevelDataManager.rand.Next(20, 51) * 0.01f * e.spriteA.Scale;
                Vector2 velocity = new Vector2(LevelDataManager.rand.Next(-150, 151), LevelDataManager.rand.Next(-300, -100));
                chunk.Velocity = velocity.Length();
                velocity.Normalize();
                chunk.Direction = velocity;
                EffectSprites.Add(chunk);
            }
        }
        void contactListener_EmberCreated(object sender, EffectEventArgs e)
        {
            
            Sprite ember = new Sprite(2, 4, e.spriteA.SpriteCenterInWorld - new Vector2(16, 16), true); //create bottom center of the sprite
            //range to create anywher inside the sprite area
            ember.Location += new Vector2(LevelDataManager.rand.Next((int)(-e.spriteA.SpriteRectWidth * 0.5f), (int)(e.spriteA.SpriteRectWidth * 0.5f)),
                                          LevelDataManager.rand.Next((int)(-e.spriteA.SpriteRectHeight * 0.5f), (int)(e.spriteA.SpriteRectHeight * 0.5f)));
            ember.HitPoints = 0; //ms timer
            ember.IsAwake = true;
            ember.Scale = (float)LevelDataManager.rand.Next(20, 41) * 0.01f * e.spriteA.Scale;
            EffectSprites.Add(ember);
            
        }
        void contactListener_DropCreated(object sender, EffectEventArgs e)
        {
            Sprite drop = new Sprite(2, 0, e.spriteA.SpriteCenterInWorld - new Vector2(16, 16), true); //create bottom center of the sprite
            //range to create anywher inside the sprite area
            drop.Location += new Vector2(LevelDataManager.rand.Next((int)(-e.spriteA.SpriteRectWidth * 0.5f), (int)(e.spriteA.SpriteRectWidth * 0.5f)),
                                          LevelDataManager.rand.Next((int)(-e.spriteA.SpriteRectHeight * 0.5f), (int)(e.spriteA.SpriteRectHeight * 0.5f)));
            drop.HitPoints = 1000; //ms timer
            drop.IsAwake = true;
            drop.Scale = (float)LevelDataManager.rand.Next(20, 41) * 0.01f * e.spriteA.Scale;
            drop.TintColor = tint * 0.5f ;
            EffectSprites.Add(drop);
        }
        void contactListener_WindParticleCreated(object sender, EffectEventArgs e)
        {
            if (LevelDataManager.rand.Next(0, 2) != 0) return;
            int windPlacementRangeX = (int)MathHelper.Max(1, e.spriteA.SpriteRectWidth / 2);
            float cos = (float)Math.Cos(e.spriteA.TotalRotation);
            float sin = (float)Math.Sin(e.spriteA.TotalRotation);
            Sprite wind = new Sprite(29, 0, e.spriteA.SpriteCenterInWorld, true); 
            wind.HitPoints = 400; //ms timer
            wind.IsAwake = true;
            wind.Scale = (float)LevelDataManager.rand.Next(250, 501) * 0.01f * e.spriteA.Scale;
            wind.RotationSpeed = LevelDataManager.rand.Next(-800, 800);
            //chunk origin centered on origin of destructing sprite, add random offset so chunk can appear from anywhere inside the sprite
            Vector2 randomOffset = new Vector2(LevelDataManager.rand.Next(-windPlacementRangeX, windPlacementRangeX + 1),-e.spriteA.SpriteRectHeight/2);

            //add rotation offset factor to account for fan rotation
            wind.Location += new Vector2((randomOffset.X * cos) - (randomOffset.Y * sin), (randomOffset.X * sin) + (randomOffset.Y * cos));
            wind.TintColor = Color.White * ((float)LevelDataManager.rand.Next(25,36) * 0.01f);
            wind.Direction = new Vector2((float)Math.Cos(e.spriteA.TotalRotation + MathHelper.ToRadians(-90)), (float)Math.Sin(e.spriteA.TotalRotation + MathHelper.ToRadians(-90)));
            wind.Velocity = 500;

            EffectSprites.Add(wind);
        }
        void contactListener_FruitMotionBlur(object sender, EffectEventArgs e)
        {

            Sprite blur = new Sprite(e.spriteA.TextureID, e.spriteA.TextureIndex, e.spriteA.Location, false);
            blur.SpriteType = Sprite.Type.None;
            blur.TintColor = Color.White * 0.12f;
            blur.Scale = e.spriteA.Scale;
            EffectSprites.Add(blur);
        }

        public void Update(GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (EffectSprites.Count > 0)
            {
                for (int i = EffectSprites.Count - 1; i >= 0; i--)
                {
                    EffectSprites[i].Update(gameTime);

                    if (!EffectSprites[i].IsEffect)
                    {
                        switch (EffectSprites[i].SpriteType)
                        {
                            case Sprite.Type.Saw:
                                {
                                    EffectSprites[i].TotalRotation += 0.27f;
                                    EffectSprites[i].spriteBody.SetTransform(ConvertUnits.ToSimUnits(EffectSprites[i].SpriteCenterInWorld), EffectSprites[i].TotalRotation);

                                    //negative up timer for saws shot
                                    if (EffectSprites[i].HitPoints < 0)
                                    {
                                        EffectSprites[i].HitPoints += (int)(gameTime.ElapsedGameTime.Milliseconds);
                                        if (EffectSprites[i].HitPoints >= 0)
                                        {
                                            EffectSprites[i].IsExpired = true;
                                            if (EffectSprites[i].spriteBody != null) physicsWorld.RemoveBody(EffectSprites[i].spriteBody);
                                            EffectSprites.Remove(EffectSprites[i]);
                                            break;
                                        }
                                    }
                                    break;
                                }
                            case Sprite.Type.Tower:
                                {
                                    //negative up timer for saws shot
                                    if (EffectSprites[i].HitPoints < 0)
                                    {
                                        EffectSprites[i].HitPoints += (int)(gameTime.ElapsedGameTime.Milliseconds);
                                        if (EffectSprites[i].HitPoints >= 0)
                                        {
                                            EffectSprites[i].IsExpired = true;
                                            EffectSprites.Remove(EffectSprites[i]);
                                            break;
                                        }
                                    }
                                    break;
                                }
                            case Sprite.Type.PowerUp:
                                {
                                    //negative up timer for saws shot
                                    if (EffectSprites[i].HitPoints > 0)
                                    {
                                        EffectSprites[i].HitPoints -= (int)(gameTime.ElapsedGameTime.Milliseconds);
                                        if (EffectSprites[i].HitPoints < 0)
                                        {
                                            EffectSprites[i].HitPoints = 0;
                                            EffectSprites[i].IsHit = true;
                                        }
                                        if (EffectSprites[i].IsExpired)
                                        {
                                            EffectSprites.Remove(EffectSprites[i]);
                                        }
                                    }
                                    break;
                                }
                            case Sprite.Type.None:
                                {
                                    //shot motion blur effect
                                    float alpha = EffectSprites[i].TintColor.A;
                                    alpha = (alpha - (150f * elapsedTime)) / 255f;
                                    if (alpha < 0)
                                    {
                                        EffectSprites[i].IsExpired = true;
                                        EffectSprites.Remove(EffectSprites[i]);
                                        break;
                                    }
                                    else EffectSprites[i].TintColor = Color.White * alpha;
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (EffectSprites[i].TextureID)
                        {
                            case 0: //bomb
                                {
                                    //the body of the explosion should only exist a tick and not persist
                                    EffectSprites[i].spriteBody.Enabled = false;

                                    if (EffectSprites[i].spriteBody != null) EffectSprites[i].spriteBody.ResetDynamics();

                                    float alpha = EffectSprites[i].TintColor.A;
                                    if (EffectSprites[i].HitPoints >= 0) EffectSprites[i].HitPoints -= 1;
                                    else
                                    {
                                        alpha = alpha - (500.0f * elapsedTime);
                                        EffectSprites[i].TintColor = new Color((int)alpha, (int)alpha, (int)alpha, (int)alpha);
                                        EffectSprites[i].Scale += elapsedTime;
                                    }
                                    //EffectSprites[i].SpriteRectangle = new Rectangle(EffectSprites[i].SpriteRectangle.X - 1, EffectSprites[i].SpriteRectangle.Y - 1, (int)(EffectSprites[i].SpriteRectWidth + 2), (int)(EffectSprites[i].SpriteRectHeight + 2));

                                    if (alpha <= 0)
                                    {
                                        alpha = 0;
                                        EffectSprites[i].IsExpired = true;
                                        physicsWorld.RemoveBody(EffectSprites[i].spriteBody);
                                        EffectSprites.Remove(EffectSprites[i]);
                                    }
                                    break;
                                }
                            case 1: //CannonSmoke
                                {
                                    float alpha = EffectSprites[i].TintColor.A;
                                    EffectSprites[i].AnimationFramePrecise += (float)gameTime.ElapsedGameTime.TotalSeconds * EffectSprites[i].AnimationFPS;
                                    if (EffectSprites[i].AnimationFramePrecise >= 1.0f)
                                    {
                                        EffectSprites[i].AnimationFramePrecise -= 1.0f;
                                        EffectSprites[i].CurrentFrame += 1;
                                    }

                                    alpha = alpha - (5f * elapsedTime * EffectSprites[i].AnimationFPS);
                                    EffectSprites[i].TintColor = new Color((int)alpha, (int)alpha, (int)alpha, (int)alpha);
                                    if (alpha <= 0 || EffectSprites[i].CurrentFrame == 12)
                                    {
                                        alpha = 0;
                                        EffectSprites[i].IsExpired = true;
                                        EffectSprites.Remove(EffectSprites[i]);
                                    }
                                    break;
                                }
                            case 2: //water droplets
                                {
                                    if (EffectSprites[i].TextureIndex == 0 || EffectSprites[i].TextureIndex == 1)
                                    {
                                        #region Manual gravity on chunk
                                        float dvelocity = GameSettings.Gravity * GameSettings.PhysicsScale * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                        float dxvel = (aDirection.X * dvelocity);
                                        float dyvel = (aDirection.Y * dvelocity);

                                        //update velocity of sprite
                                        float oldVelX = EffectSprites[i].Velocity * EffectSprites[i].Direction.X;
                                        float oldVelY = EffectSprites[i].Velocity * EffectSprites[i].Direction.Y;
                                        float newVelX = (oldVelX + dxvel);
                                        float newVelY = (oldVelY + dyvel);

                                        EffectSprites[i].Velocity = ((float)Math.Sqrt((newVelX * newVelX) + (newVelY * newVelY)));
                                        EffectSprites[i].Direction = new Vector2(newVelX, newVelY);
                                        EffectSprites[i].UpdatePosition(gameTime);
                                        #endregion

                                        if (EffectSprites[i].HitPoints < 0)
                                        {
                                            float alpha = EffectSprites[i].TintColor.A;
                                            alpha = alpha - (510f * elapsedTime);
                                            alpha = alpha / 255f;
                                            EffectSprites[i].TintColor = tint * alpha;
                                            if (alpha <= 0)
                                            {
                                                alpha = 0;
                                                EffectSprites[i].IsExpired = true;
                                                EffectSprites.Remove(EffectSprites[i]);
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            EffectSprites[i].HitPoints -= (int)(1000 * elapsedTime);
                                        }
                                        break;
                                    }

                                    if (EffectSprites[i].TextureIndex == 2 || EffectSprites[i].TextureIndex == 4) //feathers and embers
                                    {
                                        #region Manual gravity on chunk
                                        float dvelocity = GameSettings.Gravity * GameSettings.PhysicsScale * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                        float dxvel = (aDirection.X * dvelocity);
                                        float dyvel = (aDirection.Y * dvelocity);

                                        //update velocity of sprite
                                        float oldVelX = EffectSprites[i].Velocity * EffectSprites[i].Direction.X;
                                        float oldVelY = EffectSprites[i].Velocity * EffectSprites[i].Direction.Y;
                                        float newVelX = (oldVelX + dxvel);
                                        float newVelY = (oldVelY + dyvel);

                                        EffectSprites[i].Velocity = ((float)Math.Sqrt((newVelX * newVelX) + (newVelY * newVelY)));
                                        EffectSprites[i].Velocity *= 0.95f; //slow feathers
                                        EffectSprites[i].Direction = new Vector2(newVelX, newVelY);
                                        EffectSprites[i].UpdatePosition(gameTime);
                                        #endregion



                                        if (EffectSprites[i].HitPoints < 0)
                                        {
                                            float alpha = EffectSprites[i].TintColor.A;
                                            alpha = alpha - (510f * elapsedTime);
                                            alpha = alpha / 255f;
                                            EffectSprites[i].TintColor = Color.White * alpha;
                                            if (alpha <= 0)
                                            {
                                                alpha = 0;
                                                EffectSprites[i].IsExpired = true;
                                                EffectSprites.Remove(EffectSprites[i]);
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            EffectSprites[i].HitPoints -= (int)(1000 * elapsedTime);
                                        }
                                        break;
                                    }
                                    break;
                                }
                            case 4: //fireblast explosion
                                {
                                    if (EffectSprites[i].spriteBody != null) EffectSprites[i].spriteBody.ResetDynamics();

                                    EffectSprites[i].AnimationFramePrecise += (float)gameTime.ElapsedGameTime.TotalSeconds * EffectSprites[i].AnimationFPS;
                                    if (EffectSprites[i].AnimationFramePrecise >= 1.0f)
                                    {
                                        EffectSprites[i].AnimationFramePrecise -= 1.0f;
                                        EffectSprites[i].CurrentFrame += 1;
                                    }

                                    if (EffectSprites[i].CurrentFrame == 8)
                                    {
                                        EffectSprites[i].IsExpired = true;
                                        if (EffectSprites[i].spriteBody != null) physicsWorld.RemoveBody(EffectSprites[i].spriteBody);
                                        EffectSprites.Remove(EffectSprites[i]);
                                    }
                                    break;
                                }
                            case 6: //fruit chunks
                                {
                                    #region Manual gravity on chunk
                                    float dvelocity = GameSettings.Gravity * GameSettings.PhysicsScale * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                    float dxvel = (aDirection.X * dvelocity);
                                    float dyvel = (aDirection.Y * dvelocity);

                                    //update velocity of sprite
                                    float oldVelX = EffectSprites[i].Velocity * EffectSprites[i].Direction.X;
                                    float oldVelY = EffectSprites[i].Velocity * EffectSprites[i].Direction.Y;
                                    float newVelX = (oldVelX + dxvel);
                                    float newVelY = (oldVelY + dyvel);

                                    EffectSprites[i].Velocity = ((float)Math.Sqrt((newVelX * newVelX) + (newVelY * newVelY)));
                                    EffectSprites[i].Direction = new Vector2(newVelX, newVelY);
                                    EffectSprites[i].UpdatePosition(gameTime);
                                    #endregion


                                    if (EffectSprites[i].HitPoints < 0)
                                    {
                                        float alpha = EffectSprites[i].TintColor.A;
                                        alpha = alpha - (510f * elapsedTime);
                                        EffectSprites[i].TintColor = new Color((int)alpha, (int)alpha, (int)alpha, (int)alpha);
                                        if (alpha <= 0)
                                        {
                                            alpha = 0;
                                            EffectSprites[i].IsExpired = true;
                                            EffectSprites.Remove(EffectSprites[i]);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        EffectSprites[i].HitPoints -= (int)(1000 * elapsedTime);
                                    }

                                    break;
                                }
                            case 7: //fruitjuice
                                {
                                    EffectSprites[i].AnimationFramePrecise += (float)gameTime.ElapsedGameTime.TotalSeconds * EffectSprites[i].AnimationFPS;
                                    if (EffectSprites[i].AnimationFramePrecise >= 1.0f)
                                    {
                                        EffectSprites[i].AnimationFramePrecise -= 1.0f;
                                        EffectSprites[i].CurrentFrame += 1;
                                    }

                                    if (EffectSprites[i].CurrentFrame == 6)
                                    {
                                        EffectSprites[i].IsExpired = true;
                                        EffectSprites.Remove(EffectSprites[i]);
                                    }
                                    break;
                                }
                            case 8: //litblast
                                {
                                    if (EffectSprites[i].spriteBody != null) EffectSprites[i].spriteBody.ResetDynamics();

                                    EffectSprites[i].AnimationFramePrecise += (float)gameTime.ElapsedGameTime.TotalSeconds * EffectSprites[i].AnimationFPS;
                                    if (EffectSprites[i].AnimationFramePrecise >= 1.0f)
                                    {
                                        EffectSprites[i].AnimationFramePrecise -= 1.0f;
                                        EffectSprites[i].CurrentFrame += 1;
                                    }

                                    if (EffectSprites[i].CurrentFrame == 12)
                                    {
                                        EffectSprites[i].IsExpired = true;
                                        if (EffectSprites[i].spriteBody != null) physicsWorld.RemoveBody(EffectSprites[i].spriteBody);
                                        EffectSprites.Remove(EffectSprites[i]);
                                    }

                                    break;
                                }
                            case 12: //block chunks
                                {
                                    #region Manual gravity on chunk
                                    float dvelocity = GameSettings.Gravity * GameSettings.PhysicsScale * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                    float dxvel = (aDirection.X * dvelocity);
                                    float dyvel = (aDirection.Y * dvelocity);

                                    //update velocity of sprite
                                    float oldVelX = EffectSprites[i].Velocity * EffectSprites[i].Direction.X;
                                    float oldVelY = EffectSprites[i].Velocity * EffectSprites[i].Direction.Y;
                                    float newVelX = (oldVelX + dxvel);
                                    float newVelY = (oldVelY + dyvel);

                                    EffectSprites[i].Velocity = ((float)Math.Sqrt((newVelX * newVelX) + (newVelY * newVelY)));
                                    EffectSprites[i].Direction = new Vector2(newVelX, newVelY);
                                    EffectSprites[i].UpdatePosition(gameTime);
                                    #endregion

                                    if (EffectSprites[i].HitPoints < 0)
                                    {
                                        float alpha = EffectSprites[i].TintColor.A;
                                        alpha = alpha - (510f * elapsedTime);
                                        EffectSprites[i].TintColor = new Color((int)alpha, (int)alpha, (int)alpha, (int)alpha);
                                        if (alpha <= 0)
                                        {
                                            alpha = 0;
                                            EffectSprites[i].IsExpired = true;
                                            EffectSprites.Remove(EffectSprites[i]);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        EffectSprites[i].HitPoints -= (int)(1000 * elapsedTime);
                                    }

                                    break;
                                }
                            case 13:
                            case 14:
                            case 15:
                                {
                                    if (EffectSprites[i].SpriteType == Sprite.Type.Acid)
                                    {
                                        if (EffectSprites[i].spriteBody != null) EffectSprites[i].spriteBody.ResetDynamics();
                                        EffectSprites[i].AnimationFramePrecise += (float)gameTime.ElapsedGameTime.TotalSeconds * EffectSprites[i].AnimationFPS;
                                        EffectSprites[i].HitPoints = (int)(gameTime.ElapsedGameTime.TotalSeconds * 1000); //used to send the time passage to the contact solver for damage
                                        if (EffectSprites[i].AnimationFramePrecise <= 12.0f)
                                        {
                                            EffectSprites[i].CurrentFrame = (int)EffectSprites[i].AnimationFramePrecise;
                                        }
                                        else EffectSprites[i].CurrentFrame = 12;

                                        if (EffectSprites[i].AnimationFramePrecise >= 24.0f)
                                        {
                                            EffectSprites[i].IsExpired = true;
                                            if (EffectSprites[i].IsCollidable) physicsWorld.RemoveBody(EffectSprites[i].spriteBody);
                                            EffectSprites.Remove(EffectSprites[i]);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (EffectSprites[i].Velocity != 0) EffectSprites[i].Location += EffectSprites[i].Direction * EffectSprites[i].Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                        float alpha = EffectSprites[i].TintColor.A;
                                        EffectSprites[i].AnimationFramePrecise += (float)gameTime.ElapsedGameTime.TotalSeconds * EffectSprites[i].AnimationFPS;
                                        if (EffectSprites[i].AnimationFramePrecise >= 1.0f)
                                        {
                                            EffectSprites[i].AnimationFramePrecise -= 1.0f;
                                            EffectSprites[i].CurrentFrame += 1;
                                        }
                                        alpha = alpha - (6f * elapsedTime * EffectSprites[i].AnimationFPS);
                                        int intalpha = (int)Math.Max(0, alpha);
                                        EffectSprites[i].TintColor = new Color(intalpha, intalpha, intalpha, intalpha);
                                        if (EffectSprites[i].CurrentFrame > 12)
                                        {
                                            EffectSprites[i].IsExpired = true;
                                            EffectSprites.Remove(EffectSprites[i]);
                                        }
                                    }
                                    break;
                                }
                            case 16: //smoke up
                                {
                                    float alpha = EffectSprites[i].TintColor.A;
                                    EffectSprites[i].AnimationFramePrecise += (float)gameTime.ElapsedGameTime.TotalSeconds * EffectSprites[i].AnimationFPS;
                                    if (EffectSprites[i].AnimationFramePrecise >= 1.0f)
                                    {
                                        EffectSprites[i].AnimationFramePrecise -= 1.0f;
                                        EffectSprites[i].CurrentFrame += 1;
                                    }

                                    alpha = alpha - (5f * elapsedTime * EffectSprites[i].AnimationFPS);
                                    EffectSprites[i].TintColor = new Color((int)alpha, (int)alpha, (int)alpha, (int)alpha);
                                    if (alpha <= 0 || EffectSprites[i].CurrentFrame == 12)
                                    {
                                        alpha = 0;
                                        EffectSprites[i].IsExpired = true;
                                        EffectSprites.Remove(EffectSprites[i]);
                                    }
                                    break;
                                }
                            case 17: //snowball
                                {
                                    if (!EffectSprites[i].spriteBody.Awake) EffectSprites[i].AnimationFramePrecise = -1;
                                    else EffectSprites[i].AnimationFramePrecise -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                                    if (EffectSprites[i].IsHit)
                                    {
                                        contactListener.DoPoof(EffectSprites[i]);
                                        EffectSprites[i].IsExpired = true;
                                        if (EffectSprites[i].spriteBody != null) physicsWorld.RemoveBody(EffectSprites[i].spriteBody);
                                        EffectSprites.Remove(EffectSprites[i]);
                                        break;
                                    }

                                    if (EffectSprites[i].AnimationFramePrecise < 0)
                                    {
                                        EffectSprites[i].IsExpired = true;
                                        if (EffectSprites[i].spriteBody != null) physicsWorld.RemoveBody(EffectSprites[i].spriteBody);
                                        EffectSprites.Remove(EffectSprites[i]);
                                    }
                                    break;
                                }
                            case 18: //veggie chunkies
                                {
                                    #region Manual gravity on chunk
                                    float dvelocity = GameSettings.Gravity * GameSettings.PhysicsScale * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                    float dxvel = (aDirection.X * dvelocity);
                                    float dyvel = (aDirection.Y * dvelocity);

                                    //update velocity of sprite
                                    float oldVelX = EffectSprites[i].Velocity * EffectSprites[i].Direction.X;
                                    float oldVelY = EffectSprites[i].Velocity * EffectSprites[i].Direction.Y;
                                    float newVelX = (oldVelX + dxvel);
                                    float newVelY = (oldVelY + dyvel);

                                    EffectSprites[i].Velocity = ((float)Math.Sqrt((newVelX * newVelX) + (newVelY * newVelY)));
                                    EffectSprites[i].Direction = new Vector2(newVelX, newVelY);
                                    EffectSprites[i].UpdatePosition(gameTime);
                                    #endregion

                                    if (EffectSprites[i].HitPoints < 0)
                                    {
                                        float alpha = EffectSprites[i].TintColor.A;
                                        alpha = alpha - (510f * elapsedTime);
                                        EffectSprites[i].TintColor = new Color((int)alpha, (int)alpha, (int)alpha, (int)alpha);
                                        if (alpha <= 0)
                                        {
                                            alpha = 0;
                                            EffectSprites[i].IsExpired = true;
                                            EffectSprites.Remove(EffectSprites[i]);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        EffectSprites[i].HitPoints -= (int)(1000 * elapsedTime);
                                    }

                                    break;
                                }

                            case 19: //veggie splat
                                {
                                    EffectSprites[i].AnimationFramePrecise += (float)gameTime.ElapsedGameTime.TotalSeconds * EffectSprites[i].AnimationFPS;
                                    if (EffectSprites[i].AnimationFramePrecise >= 1.0f)
                                    {
                                        EffectSprites[i].AnimationFramePrecise -= 1.0f;
                                        EffectSprites[i].CurrentFrame += 1;
                                    }

                                    if (EffectSprites[i].CurrentFrame == 6)
                                    {
                                        EffectSprites[i].IsExpired = true;
                                        EffectSprites.Remove(EffectSprites[i]);
                                    }
                                    break;
                                }
                            case 20: //spider thread
                                {
                                    //thread lasts one tick, new one calculated by event each frame
                                    if (EffectSprites[i].IsHit) EffectSprites.Remove(EffectSprites[i]);
                                    else EffectSprites[i].IsHit = true;
                                    break;
                                }
                            case 21: //fan wind
                                {
                                    EffectSprites[i].HitPoints -= (int)(gameTime.ElapsedGameTime.TotalMilliseconds);
                                    if (EffectSprites[i].HitPoints <= 0)
                                    {
                                        EffectSprites[i].IsBounceAnimated = false;
                                    }
                                    //update position and force of wind
                                    if (EffectSprites[i].IsBounceAnimated)
                                    {
                                        EffectSprites[i].TotalRotation = EffectSprites[i].spriteBody.Rotation;
                                        Vector2 windForce = new Vector2((float)Math.Cos(EffectSprites[i].TotalRotation + MathHelper.ToRadians(-90)), (float)Math.Sin(EffectSprites[i].TotalRotation + MathHelper.ToRadians(-90)));
                                        Vector2 windLocationCenter = new Vector2(ConvertUnits.ToDisplayUnits(EffectSprites[i].spriteBody.Position.X) + (windForce.X * 172),
                                                                                 ConvertUnits.ToDisplayUnits(EffectSprites[i].spriteBody.Position.Y) + (windForce.Y * 172));
                                        EffectSprites[i].SpriteRectangle = new Rectangle((int)windLocationCenter.X - 32, (int)windLocationCenter.Y - 160, 64, 320);
                                        EffectSprites[i].Direction = windForce;
                                    }

                                    break;
                                }
                            case 28: //apple copter
                                {
                                    // lasts one tick, new one calculated by event each frame
                                    EffectSprites.Remove(EffectSprites[i]);
                                    //if (EffectSprites[i].IsHit) EffectSprites.Remove(EffectSprites[i]);
                                    //else EffectSprites[i].IsHit = true;
                                    break;
                                }
                            case 29: //fan wind particles
                                {
                                    EffectSprites[i].UpdatePosition(gameTime);

                                    EffectSprites[i].Location += new Vector2(LevelDataManager.rand.Next(-1, 2), LevelDataManager.rand.Next(-1, 2));
                                    if (EffectSprites[i].HitPoints < 0)
                                    {
                                        float alpha = EffectSprites[i].TintColor.A;
                                        alpha = alpha - (130f * elapsedTime);
                                        alpha = alpha / 255f;
                                        EffectSprites[i].TintColor = tint * alpha;
                                        if (alpha <= 0)
                                        {
                                            alpha = 0;
                                            EffectSprites[i].IsExpired = true;
                                            EffectSprites.Remove(EffectSprites[i]);
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        EffectSprites[i].HitPoints -= (int)(1000 * elapsedTime);
                                    }
                                    break;
                                }
                            default:
                                break;
                        }  //endswitch
                    } //end if sprite is effect
                }//end loop
            }//end if count > 0
            return;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.GetViewMatrix(layerParallax));

            for (int i = 0; i < EffectSprites.Count; i++)
            {
                EffectSprites[i].Draw(gameTime, spriteBatch, layerParallax);
                //if (EffectSprites[i].TextureID == 28)
                //{
                //    spriteBatch.DrawString(font, EffectSprites[i].HitPoints.ToString(), EffectSprites[i].Location + new Vector2 (1,1), Color.Black, 0, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
                //    spriteBatch.DrawString(font, EffectSprites[i].HitPoints.ToString(), EffectSprites[i].Location + new Vector2 (-1,1), Color.Black, 0, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
                //    spriteBatch.DrawString(font, EffectSprites[i].HitPoints.ToString(), EffectSprites[i].Location + new Vector2(1, -1), Color.Black, 0, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
                //    spriteBatch.DrawString(font, EffectSprites[i].HitPoints.ToString(), EffectSprites[i].Location + new Vector2(-1,- 1), Color.Black, 0, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
                //    spriteBatch.DrawString(font, EffectSprites[i].HitPoints.ToString(), EffectSprites[i].Location, Color.White, 0, Vector2.Zero, 1.25f, SpriteEffects.None, 0);
                //}
            }
            
            spriteBatch.End();
            return;
        }

        private void UpdateMotion(GameTime gameTime, Sprite sprite)
        {
                float distance = sprite.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                sprite.Location += new Vector2 ((sprite.Direction.X * distance),(sprite.Direction.Y * distance));
        }

        private void UpdateRotation(GameTime gameTime, Sprite sprite)
        {
                sprite.TotalRotation += (MathHelper.ToRadians(sprite.RotationSpeed) * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

    }
}
