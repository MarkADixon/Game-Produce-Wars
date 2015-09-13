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
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;

namespace ProduceWars.Managers
{
    public class EnemyManager
    {
        public List<Sprite> FruitSprites;
        private List<Sprite> VeggieSprites;
        public Sprite boss;
        public Layer bossLayer, cageLayer;
        private World physicsWorld;
        private ContactListener contactListener;
        private float worldCheckTimer = 0.0f;
        private float animationSpeed = 6.0f; //fps
        private int blinkChance = 20; //%chance for blink

        public Vector2 bossPos = new Vector2(1216,240);
        private Vector2 bossPosOffset = Vector2.Zero;
        private int posX = -1;
        private int posY = 1;
        private int veggieThrowTimer = 1600;  //ms
        private int veggieThrowTotal = 1600; //ms
        private int nextVeggie = 0;
        private int nextVeggieDir = 1;
        private float bossSpeedMultiplier = 0.2f;
        private bool isBossHit = false;
        private int bossHitTimer = 0; //ms  counts up
        public bool isBossDefeated = false;
        public int bossDefeatedTimer = 0; //counts up
        public bool isBossFailed = false;
        private int bossHP = 10;

        public EnemyManager(World _world, ContactListener _contactListener)
        {
            FruitSprites = new List<Sprite>();
            VeggieSprites = new List<Sprite>();
            physicsWorld = _world;
            contactListener = _contactListener;

            contactListener.DamageVeggie += new ContactListener.DamageEventHandler(contactListener_DamageVeggie);
            contactListener.VeggieExploded += new ContactListener.EffectEventHandler(contactListener_VeggieExploded);
            contactListener.DamageBoss += new ContactListener.DamageEventHandler(contactListener_DamageBoss);
        }

        void contactListener_DamageBoss(object sender, DamageEventArgs e)
        {
            if (isBossHit || isBossDefeated) return;
            contactListener.ExplodeLitShot(e.sprite);
            SoundManager.Play(SoundManager.Sound.pw_docbroc, false, true);
            isBossHit = true;
            bossHitTimer = 0;
            bossSpeedMultiplier *= 1.05f;
            veggieThrowTotal = (int)(veggieThrowTotal * 0.9f);
        }

        void contactListener_DamageVeggie(object sender, DamageEventArgs e)
        {
            if (e.damage <= 0) return;
            if (e.damage >= e.sprite.HitPoints)
            {
                if (!GameSettings.isBoss)
                {
                    Camera.AddScore(e.sprite.HitPoints);
                    Camera.AddScoreCombo();
                }
                e.sprite.HitPoints = 0;
                e.sprite.IsHit = true;
            }
            else
            {
                Scream(e.sprite);
                Camera.AddScore(e.damage);
                e.sprite.HitPoints -= e.damage;
            }
        }

        void contactListener_VeggieExploded(object sender, EffectEventArgs e)
        {
            if (GameSettings.CheatGoodToBeBad) contactListener.ExplodeBomb(e.spriteA);
            e.spriteA.IsExpired = true;
            physicsWorld.RemoveBody(e.spriteA.spriteBody);
            VeggieSprites.Remove(e.spriteA);
        }

        public void Update(GameTime gameTime)
        {
            //update placed fruit sprites
            if (FruitSprites.Count > 0)
            {
                for (int i = FruitSprites.Count - 1; i >= 0; i--)
                {
                    FruitSprites[i].Update(gameTime);
                    UpdateAnimation(FruitSprites[i], gameTime);
                    if (FruitSprites[i].IsHit) contactListener.ExplodeFruit(FruitSprites[i]);
                }
            }


            if (VeggieSprites.Count > 0)
            {
                for (int i = VeggieSprites.Count - 1; i >= 0; i--)
                {
                    VeggieSprites[i].Update(gameTime);
                    UpdateAnimation(VeggieSprites[i], gameTime);
                    if (VeggieSprites[i].IsHit) contactListener.ExplodeVeggie(VeggieSprites[i]);
                }

                if (GameSettings.isBoss && !boss.IsExpired) UpdateBoss(gameTime);

                worldCheckTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (worldCheckTimer >= 1.0f)
                {
                    for (int i = VeggieSprites.Count - 1; i >= 0; i--)
                    {
                        if (!Camera.WorldRectangleBuffered.Contains(VeggieSprites[i].SpriteRectangle))
                        {
                            if (!GameSettings.isBoss)
                            {
                                Camera.AddScore(VeggieSprites[i].HitPoints);
                                Camera.AddScoreCombo();
                            }
                            VeggieSprites[i].IsExpired = true;
                            physicsWorld.RemoveBody(VeggieSprites[i].spriteBody);
                            VeggieSprites.Remove(VeggieSprites[i]);
                        }
                    }
                    worldCheckTimer = 0.0f;
                }
            }
            return;
        }

        private void UpdateBoss(GameTime gameTime)
        {
            if (!isBossDefeated && !isBossFailed)
            {
                for (int i = 0; i < cageLayer.SpriteCount; i++ )
                {
                    cageLayer.LayerSprites[i].Location = new Vector2(cageLayer.LayerSprites[i].Location.X, cageLayer.LayerSprites[i].Location.Y + 0.15f);
                    if (cageLayer.LayerSprites[i].TextureID == 20 && cageLayer.LayerSprites[i].Location.Y > 1250 && !cageLayer.LayerSprites[i].IsExpired)
                    {
                        contactListener.ExplodeFruit(cageLayer.LayerSprites[i]);
                        cageLayer.LayerSprites[i].IsExpired = true;
                        isBossFailed = true;
                    }
                }
            }


            //smoke
            int num_smoke = 8 - boss.HitPoints;
            int smokeX = (int)(boss.SpriteCenterInWorld.X);
            for (int i = 0; i < num_smoke; i++)
            {
                 if (LevelDataManager.rand.Next(0, 2) == 1)
                 {
                     if (boss.IsFlippedHorizontally) smokeX += LevelDataManager.rand.Next(-30, 10);
                     else smokeX += LevelDataManager.rand.Next(-10, 30);
                     contactListener.DoSmallPoof(new Vector2(smokeX,
                                             LevelDataManager.rand.Next(10, 60) + boss.SpriteCenterInWorld.Y), boss);
                 }
            }
 
            if (isBossHit)
            {
                bossHitTimer += (int)(gameTime.ElapsedGameTime.Milliseconds);
                if (bossHitTimer > 200 || bossHitTimer < 600) boss.CurrentFrame = 5;
                else boss.CurrentFrame = 3;
                if (bossHitTimer >= 800)
                {
                    bossHitTimer = 0;
                    isBossHit = false;
                    boss.CurrentFrame = 0;
                    boss.HitPoints += -1;
                    if (boss.HitPoints <= 0) isBossDefeated = true;
                }
                return;
            }

            if (isBossDefeated)
            {
                boss.CurrentFrame = 5;
                veggieThrowTotal = 40;
                bossSpeedMultiplier = 1.8f;

                if (bossDefeatedTimer < 300)
                {
                    bossDefeatedTimer += 1;
                    bossPosOffset += new Vector2(posX * (float)gameTime.ElapsedGameTime.TotalMilliseconds * bossSpeedMultiplier, posY * 3f * bossSpeedMultiplier);
                    if (bossPosOffset.Y > 20) posY = -1;
                    if (bossPosOffset.Y < -60) posY = 1;
                    if (bossPosOffset.X > 1000)
                    {
                        posX = -1;
                        boss.IsFlippedHorizontally = false;
                    }
                    if (bossPosOffset.X < -1000)
                    {
                        posX = 1;
                        boss.IsFlippedHorizontally = true;
                    }
                    boss.Location = bossPos + bossPosOffset;
                    boss.spriteBody.SetTransform(ConvertUnits.ToSimUnits(boss.SpriteCenterInWorld), boss.TotalRotation);
                }
                if (bossDefeatedTimer >= 300)
                {
                    bossPosOffset += new Vector2(posX * (float)gameTime.ElapsedGameTime.TotalMilliseconds * bossSpeedMultiplier, 5f * bossSpeedMultiplier);
                    if (bossPosOffset.X > 1000)
                    {
                        posX = -1;
                        boss.IsFlippedHorizontally = false;
                    }
                    if (bossPosOffset.X < -1000)
                    {
                        posX = 1;
                        boss.IsFlippedHorizontally = true;
                    }
                    boss.Location = bossPos + bossPosOffset;
                    boss.spriteBody.SetTransform(ConvertUnits.ToSimUnits(boss.SpriteCenterInWorld), boss.TotalRotation);
                    if (bossDefeatedTimer > 310)
                    {
                        contactListener.ExplodeFireShot(boss);
                        bossDefeatedTimer -= 10;
                    }
                    bossDefeatedTimer += 1;
                    if (boss.Location.Y > 1440)
                    {
                        boss.IsExpired = true;
                        physicsWorld.RemoveBody(boss.spriteBody);
                    }
                }
            }
            else
            {
                bossPosOffset += new Vector2(posX * (float)gameTime.ElapsedGameTime.TotalMilliseconds * bossSpeedMultiplier, posY * 5f * bossSpeedMultiplier);
                if (bossPosOffset.Y > 20) posY = -1;
                if (bossPosOffset.Y < -60) posY = 1;
                if (bossPosOffset.X > 1000)
                {
                    posX = -1;
                    boss.IsFlippedHorizontally = false;
                }
                if (bossPosOffset.X < -1000)
                {
                    posX = 1;
                    boss.IsFlippedHorizontally = true;
                }
                boss.Location = bossPos + bossPosOffset;
                boss.spriteBody.SetTransform(ConvertUnits.ToSimUnits(boss.SpriteCenterInWorld), boss.TotalRotation);
            }




            veggieThrowTimer -= (int)(gameTime.ElapsedGameTime.Milliseconds);
            if (veggieThrowTimer <= 0 && bossDefeatedTimer <300) //if timer reaches 0 throw a veggie
            {
                veggieThrowTimer += veggieThrowTotal;
                nextVeggie = LevelDataManager.rand.Next(0,12)*7;
                Sprite veggie = new Sprite(21, nextVeggie, boss.Location, false);
                veggie.SpriteType = Sprite.Type.Veggie;
                CreateVeggie(veggie,physicsWorld);
                if (nextVeggie >= 77 && nextVeggie < 84) veggie.spriteBody.ApplyLinearImpulse(new Vector2(LevelDataManager.rand.Next(6, 10) * nextVeggieDir *3, LevelDataManager.rand.Next(-7, -3)*3));
                else veggie.spriteBody.ApplyLinearImpulse(new Vector2(LevelDataManager.rand.Next(6,10)*nextVeggieDir,LevelDataManager.rand.Next(-7,-3)));
                veggie.HitPoints *= 5;
                veggie.spriteBody.Mass *= 2.5f;
                nextVeggieDir *= -1;
                bossLayer.AddSpriteToLayer(veggie);
            }

            return;
        }



        public int VeggieSpriteCount
        {
            get 
            { 
                return VeggieSprites.Count;
            }
        }

        public void UpdateAnimation(Sprite sprite, GameTime gameTime)
        {
            sprite.AnimationFramePrecise += (float)gameTime.ElapsedGameTime.TotalSeconds * sprite.AnimationFPS;
            #region FRUIT AND VEGGIE ANIMATION CODE
            if (sprite.TextureID == 20 || sprite.TextureID == 21) //fruit or veggie animation block
            {
                //scream at high speed
                if (sprite.spriteBody != null && sprite.CurrentFrame < 5)
                {
                    if (sprite.spriteBody.LinearVelocity.Length() >= 4)
                    {
                        Scream(sprite);
                        return;
                    }
                }

                //process frame change if enough time has elapsed
                if (sprite.AnimationFramePrecise >= 1.0f && sprite.IsAnimationDirectionForward)
                {
                    if (sprite.CurrentFrame == 0)
                    {
                        if (LevelDataManager.rand.Next(0, 100) < blinkChance) //cause a blink (frame 4) on random chance
                        {
                            sprite.CurrentFrame = 4;
                            sprite.AnimationFramePrecise -= 1.0f;
                            return;
                        }
                    }

                    if (sprite.CurrentFrame == 4) //unblink after a frame
                    {
                        sprite.CurrentFrame = 0;
                        sprite.AnimationFramePrecise -= 1.0f;
                        return;
                    }

                    if (sprite.CurrentFrame == 5) //screaming animation is frames 5 and 6 alternating
                    {
                        sprite.CurrentFrame = 6;
                        sprite.AnimationFramePrecise -= 1.0f;
                        return;
                    }

                    if (sprite.CurrentFrame == 6) //screaming animation is frames 5 and 6 alternating
                    {
                        if (LevelDataManager.rand.Next(0, 100) < blinkChance)
                        {
                            sprite.CurrentFrame = 0;
                            sprite.AnimationFPS = 6f;
                            sprite.AnimationFramePrecise -= 1.0f;
                            sprite.IsAnimationDirectionForward = true;
                        }
                        else
                        {
                            sprite.CurrentFrame = 5;
                            sprite.AnimationFramePrecise -= 1.0f;
                        }
                        return;
                    }


                    if (sprite.CurrentFrame == 3) sprite.IsAnimationDirectionForward = false;
                    else sprite.CurrentFrame += 1;
                    sprite.AnimationFramePrecise -= 1.0f;
                    return;
                }

                if (sprite.AnimationFramePrecise >= 1.0f && !sprite.IsAnimationDirectionForward)
                {
                    if (sprite.CurrentFrame == 0) sprite.IsAnimationDirectionForward = true;
                    else sprite.CurrentFrame -= 1;
                    sprite.AnimationFramePrecise -= 1.0f;
                    return;
                }
            }
            #endregion
            
            return;
        }

        public void Scream(Sprite sprite)
        {
            sprite.CurrentFrame = 5;
            sprite.IsAnimationDirectionForward = true;
            sprite.AnimationFPS = 12.0f;
            return;
        }

        public void CreateBoss(Sprite sprite, World physicsWorld)
        {
            boss = sprite;
            VeggieSprites.Add(sprite);
            sprite.IsAwake = true;
            sprite.IsCollidable = true;
            sprite.SpriteType = Sprite.Type.Boss;
            //bossPos = sprite.Location;
            Vector2 position = ConvertUnits.ToSimUnits(bossPos+sprite.SpriteOrigin);
            Vertices bossBubble = new Vertices(6);
            bossBubble.Add(ConvertUnits.ToSimUnits(new Vector2(48, 18)));
            bossBubble.Add(ConvertUnits.ToSimUnits(new Vector2(-48, 18)));
            bossBubble.Add(ConvertUnits.ToSimUnits(new Vector2(-30, -18)));
            bossBubble.Add(ConvertUnits.ToSimUnits(new Vector2(-12, -32)));
            bossBubble.Add(ConvertUnits.ToSimUnits(new Vector2(12, -32)));
            bossBubble.Add(ConvertUnits.ToSimUnits(new Vector2(30, -18)));
            sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld,bossBubble,GameSettings.MetalDensity, position, sprite);
            sprite.spriteBody.Rotation = 0;
            sprite.spriteBody.BodyType = BodyType.Kinematic;
            sprite.spriteBody.IgnoreGravity = true;
            sprite.spriteBody.IsSensor = true;
            sprite.HitPoints = bossHP;

            //creates the hitbox for dome
            if (sprite.spriteBody.FixtureList.Count == 1)
            {
                Vertices bossBase = new Vertices(4);
                bossBase.Add(ConvertUnits.ToSimUnits(new Vector2(-60, 20)));
                bossBase.Add(ConvertUnits.ToSimUnits(new Vector2(60, 20)));
                bossBase.Add(ConvertUnits.ToSimUnits(new Vector2(60, 54)));
                bossBase.Add(ConvertUnits.ToSimUnits(new Vector2(-60, 54)));
                sprite.spriteBody.CreateFixture(new PolygonShape(bossBase, 0f),sprite);
                sprite.spriteBody.FixtureList[1].Restitution = GameSettings.MetalRestitution;
                sprite.spriteBody.FixtureList[1].Friction = GameSettings.MetalFriction;
            }

        }

        public void CreateVeggie(Sprite sprite, World physicsWorld)
        {

            VeggieSprites.Add(sprite);
            sprite.IsAwake = true;
            sprite.IsCollidable = true;
            sprite.IsFlippedHorizontally = true;
            sprite.AnimationFPS = animationSpeed;
            sprite.CurrentFrame = LevelDataManager.rand.Next(0,3);
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            int spriteRow = sprite.TextureIndex / LevelDataManager.SpritesInRow(sprite);
            switch (spriteRow)
            {
                case 0:
                case 1:
                case 12:
                case 15:
                    {
                        //potato
                        Vertices vertices = new Vertices(7);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-26f, 16f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-17f, -21f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(1f, -27f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(24f, -12f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(25f, 19f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(8f, 29f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-12f, 29f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, sprite);
                        break;
                    }
                case 2:
                case 3:
                case 4:
                    {
                        //onion
                        Vertices vertices = new Vertices(8);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(12f, 28f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-7f, 28f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-26f, 15f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-25f, 3f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-10f, -10f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(5f, -12f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(25f, -2f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(26f, 15f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, sprite);
                        break;
                    }
                case 5:
                    {
                        //corn
                        Vertices vertices = new Vertices(7);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-13f, 20f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-10f, -11f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(6f, -28f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(20f, -26f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(17f, 21f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(8f, 30f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-5f, 30f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, sprite);
                        break;
                    }
                case 6:
                    {
                        //carrot
                        Vertices vertices = new Vertices(6);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(8f, 29f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-8f, 29f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-17f, 11f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-20f, -18f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(20f, -18f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(17f, 11f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, sprite);
                        break;
                    }
                case 7:
                    {
                        //peas
                        Vertices vertices = new Vertices(4);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(10f, 25f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-20f, 25f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(22f, -21f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(22f, 4f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, sprite);
                        break;
                    }
                case 8:
                case 9:
                case 10:
                    {
                        //pepper
                        Vertices vertices = new Vertices(6);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(17f, 26f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-17f, 26f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-26f, -5f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-18f, -17f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(18f, -17f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(26f, -5f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, sprite); 
                        break;
                    }
                case 11:
                    {
                        //pumpkin
                        Vertices vertices = new Vertices(8);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(16f, 28f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-16f, 28f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-30f, 12f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-30f, -6f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-14f, -18f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(14f, -18f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(28f, -6f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(28f, 12f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, sprite); 
                        break;
                    }
                case 14: //washer
                    {
                        sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 1.0f, position, sprite);
                        break;
                    }
                default:
                    break;
            }
            sprite.spriteBody.Position = position;
            sprite.spriteBody.Mass = GameSettings.VeggieMass;
            sprite.spriteBody.Rotation = sprite.TotalRotation;
            sprite.spriteBody.BodyType = BodyType.Dynamic;
            sprite.spriteBody.IgnoreGravity = false;
            sprite.spriteBody.Restitution = GameSettings.VeggieBouncy;
            sprite.spriteBody.Friction = GameSettings.VeggieFriction;

            if (sprite.HitPoints <= 0) sprite.HitPoints = GameSettings.VeggieHP;

            //pumpkin adjustment
            if (spriteRow == 11) sprite.spriteBody.Mass *= 3;
            if (spriteRow == 11) sprite.HitPoints *= 3;
        }

        public void CreateFruit(Sprite sprite, World physicsWorld)
        {
            FruitSprites.Add(sprite);            
            sprite.AnimationFPS = animationSpeed;
            if (sprite.TextureIndex == 70) return;
            sprite.IsCollidable = true;
            sprite.CurrentFrame = LevelDataManager.rand.Next(0, 3);
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            int spriteRow = sprite.TextureIndex / LevelDataManager.SpritesInRow(sprite);
            switch (spriteRow)
            {
                case 0:
                case 1:
                case 2:
                    {
                        //apple
                        Vertices vertices = new Vertices(9);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(14f, 29f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-12f, 29f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-24f, 16f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-30f, -4f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-30f, -8f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-18f, -18f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(18f, -18f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(26f, -10f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(26f, 12f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, sprite);
                        break;
                    }
                case 3:
                    {
                        //orange
                        sprite.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits(30f), ConvertUnits.ToSimUnits(30f), 10, 1.0f, position, sprite);
                        break;
                    }
                case 4:
                    {
                        //strawberry
                        Vertices vertices = new Vertices(8);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(8f, 30f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-8f, 30f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-24f, 6f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-24f, -8f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-10f, -17f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(10f, -17f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(24f, -8f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(24f, 10f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, sprite);
                        break;
                    }
                case 5:
                    {
                        //cherry
                        Vertices vertices = new Vertices(8);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(26f, 20f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(2f, 29f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-11f, 29f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-30f, 7f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-30f, -2f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-21f, -11f)));
                        //vertices.Add(ConvertUnits.ToSimUnits(new Vector2(0f, 0f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(23f, -1f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(28f, 7f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, sprite);
                        break;
                    }
                case 6:
                    {
                        //banana
                        Vertices vertices = new Vertices(9);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(11f, 30f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-6f, 30f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-25f, 14f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-26f, -8f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-10f, -25f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(1f, -25f)));
                        //vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-2f, 10f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(25f, 11f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(28f, 17f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, sprite);
                        break;
                    }
                case 7:
                    {
                        //lemon
                        Vertices vertices = new Vertices(8);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(10f, 29f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-10f, 29f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-25f, 15f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-25f, -7f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-10f, -22f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(10f, -22f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(23f, -7f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(23f, 15f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, sprite);
                        break;
                    }
                case 8:
                    {
                        //watermelon
                        sprite.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits(30f), ConvertUnits.ToSimUnits(30f), 10, 1.0f, position, sprite);
                        break;
                    }
                default:
                    break;
            }
            sprite.spriteBody.Position = position;
            sprite.spriteBody.Mass = GameSettings.FruitMass;
            sprite.spriteBody.Rotation = sprite.TotalRotation;
            sprite.spriteBody.BodyType = BodyType.Dynamic;
            sprite.spriteBody.IgnoreGravity = false;
            sprite.spriteBody.Restitution = GameSettings.FruitBouncy;
            sprite.spriteBody.Friction = GameSettings.FruitFriction;
            if (sprite.HitPoints <= 0) sprite.HitPoints = GameSettings.FruitHP;

            //watermelon adjustment
            if (spriteRow == 8) sprite.spriteBody.Mass *= 3;
            if (spriteRow == 8) sprite.HitPoints *= 3;
        }

    }
}
