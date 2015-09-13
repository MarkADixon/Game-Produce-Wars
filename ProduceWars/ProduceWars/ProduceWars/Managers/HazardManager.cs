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
    public class HazardManager
    {
        private List<Sprite> HazardSprites;
        private List<Sprite> TowerSprites;
        private int towerTimer = 2000; //ms
        private int towerTimerTotal = 2000; //ms the reset to value
        private World physicsWorld;
        private ContactListener contactListener;
        private const float fanSpeed = 24.0f; //fps of fan animation
        private const float sawSpeed = 0.27f; //rotation speed of saw blades
        private const float springEnergyReflect = 0.75f;
        private const float springDirectionEnergy = 6f; //velocity (in physics) of energy added in direction of the spring as a push
        private int towerNumber;
        private bool rightTower = true;

        public HazardManager(World _world, ContactListener _contactListener)
        {
            HazardSprites = new List<Sprite>();
            TowerSprites = new List<Sprite>();
            physicsWorld = _world;
            contactListener = _contactListener;
            contactListener.DamageSwitch += new ContactListener.DamageEventHandler(contactListener_DamageSwitch);
            contactListener.DamageCreature += new ContactListener.DamageEventHandler(contactListener_DamageCreature);
            contactListener.DamageSaw += new ContactListener.DamageEventHandler(contactListener_DamageSaw);
            contactListener.DamageFan += new ContactListener.DamageEventHandler(contactListener_DamageFan);
            contactListener.DamageSpike += new ContactListener.DamageEventHandler(contactListener_DamageSpike);
            contactListener.DamageBee += new ContactListener.DamageEventHandler(contactListener_DamageBee);
            contactListener.BeeDeflection += new ContactListener.EffectEventHandler(contactListener_BeeDeflection);
            contactListener.SpringDeflection += new ContactListener.EffectEventHandler(contactListener_SpringDeflection);
            contactListener.WindDeflection += new ContactListener.EffectEventHandler(contactListener_WindDeflection);
        }







        void contactListener_DamageSwitch(object sender, DamageEventArgs e)
        {
            //the switch has already been hit this shot if it is displaying frame 2, if timer has started, switch cannot be hit again for 1 sec
            if (e.sprite.CurrentFrame == 2 && e.sprite.Timer != 0) return;

            e.sprite.CurrentFrame = 2;
            e.sprite.Timer = 0;
       
            //turn the spring fade off or on
            int switchColor = (e.sprite.TextureIndex / 3);
            

            foreach (Sprite sprite in HazardSprites)
            {
                if (sprite.SpriteType == Sprite.Type.Spring && sprite.TextureIndex > 4)
                {
                    int springColor = (sprite.TextureIndex / 5) -1;

                    if (switchColor == springColor)
                    {
                        if (sprite.IsHit) sprite.IsHit = false;
                        else sprite.IsHit = true;
                    }
                }
            }
        }

        void contactListener_DamageCreature(object sender, DamageEventArgs e)
        {
            e.sprite.HitPoints -= e.damage;
            if (e.sprite.HitPoints <= 0) e.sprite.IsHit = true; 
        }

        void contactListener_DamageSaw(object sender, DamageEventArgs e)
        {
            if (e.damage == int.MaxValue)
            {
                e.sprite.IsRotating = false;
                e.sprite.spriteBody.Enabled = false;
            }
            return;
        }

        void contactListener_DamageFan(object sender, DamageEventArgs e)
        {
            if (e.damage == int.MaxValue)
            {
                e.sprite.IsAnimated = false;
                e.sprite.IsAwake = true;
            }
            return;
        }

        void contactListener_DamageSpike(object sender, DamageEventArgs e)
        {
            if (e.damage == int.MaxValue) e.sprite.IsHit = true;
        }

        void contactListener_DamageBee(object sender, DamageEventArgs e)
        {
            if (e.damage == int.MaxValue) e.sprite.IsHit = true;
        }

        void contactListener_BeeDeflection(object sender, EffectEventArgs e)
        {
            e.spriteB.spriteBody.LinearVelocity = new Vector2(e.spriteB.spriteBody.LinearVelocity.X + ConvertUnits.ToSimUnits(LevelDataManager.rand.Next(-200, 201)),
                                                                e.spriteB.spriteBody.LinearVelocity.Y + ConvertUnits.ToSimUnits(LevelDataManager.rand.Next(-200, 201)));
            e.spriteB.spriteBody.AngularVelocity += ((float)LevelDataManager.rand.Next(-200, 200) / 100.0f);
            return;
        }

        void contactListener_SpringDeflection(object sender, EffectEventArgs e)
        {
            if (e.spriteA.spriteBody.IsBullet) return;
            if (e.spriteA.IsAnimated) return;

            Vector2 springDirectionVector = new Vector2((float)Math.Sin(e.spriteA.TotalRotation),(float)-Math.Cos(e.spriteA.TotalRotation));

            e.spriteA.TotalRotation = (float)NormalizeRotation(e.spriteA.TotalRotation);
            Vector2 directionVector = e.spriteB.spriteBody.LinearVelocity;
            directionVector.Normalize();
            float velocity = e.spriteB.spriteBody.LinearVelocity.Length();

            //calc angle from vertical
            double thetaAngle = Math.Asin(directionVector.X);
            if (directionVector.Y < 0 && directionVector.X > 0) thetaAngle = Math.PI - thetaAngle;
            if (directionVector.Y < 0 && directionVector.X < 0) thetaAngle = -Math.PI - thetaAngle;
            
            //subtract springrotation
            thetaAngle = thetaAngle + e.spriteA.TotalRotation;
            thetaAngle = NormalizeRotation(thetaAngle);
            
            //final angle calc
            double finalAngle = 0.0;
            double piOverTwo = Math.PI * 0.5;
            if (thetaAngle < -piOverTwo) finalAngle = thetaAngle + Math.PI;
            if (thetaAngle >= -piOverTwo && thetaAngle <= piOverTwo) finalAngle = -thetaAngle;
            if (thetaAngle > piOverTwo) finalAngle = thetaAngle - Math.PI;

            //addspring rotation
            finalAngle = finalAngle - e.spriteA.TotalRotation;
            //finalAngle = -finalAngle; //reverse the invert for calcs

            //calc final direction vector
            directionVector = new Vector2(-(float)Math.Sin(finalAngle), -(float)Math.Cos(finalAngle));

            //apply magnitude
            e.spriteB.spriteBody.LinearVelocity = directionVector * (velocity* springEnergyReflect); //some energy loss with the factor
            e.spriteB.spriteBody.LinearVelocity += springDirectionVector * springDirectionEnergy; //spring bump            
            e.spriteB.spriteBody.AngularVelocity += ((float)LevelDataManager.rand.Next(-200, 200) / 100.0f);
            
            e.spriteA.IsAnimated = true;
            e.spriteA.IsAnimationDirectionForward = true;
            return;
        }

        //takes any rotation value (in radians) and returns the same rotation with value from -pi to pi
        double NormalizeRotation(double rotation)
        {
            if (rotation > Math.PI)
            {
                rotation -= (Math.PI * 2f);
                rotation = NormalizeRotation(rotation);
            }
            if (rotation < -Math.PI)
            {
                rotation += (Math.PI * 2f);
                rotation = NormalizeRotation(rotation);
            }
               
            return rotation;
        }

        void contactListener_WindDeflection(object sender, EffectEventArgs e)
        {
            if (e.spriteA.IsBounceAnimated)
            {
                Vector2 distanceVector = e.spriteA.spriteBody.Position - e.spriteB.spriteBody.Position;
                float windPowerFactor = 0.4f / distanceVector.LengthSquared();
                e.spriteB.spriteBody.LinearVelocity += e.spriteA.Direction * LevelDataManager.rand.Next(1, 11) * windPowerFactor;
                e.spriteB.spriteBody.AngularVelocity += ((float)LevelDataManager.rand.Next(-100, 100) *0.001f);
            }
            return;
        }

        public void Update(GameTime gameTime, GameplayScreen.LevelState levelState, Vector2 fruitShotLocation, bool shotExpired)
        {
            if (GameSettings.isBoss) UpdateTowers(gameTime);

            if (HazardSprites.Count > 0)
            {
                for (int i = HazardSprites.Count - 1; i >= 0; i--)
                {
                    HazardSprites[i].Update(gameTime);

                    switch (HazardSprites[i].SpriteType)
                    {
                        case Sprite.Type.Windmill:
                            {
                                HazardSprites[i].TotalRotation += 0.005f;
                                HazardSprites[i].spriteBody.Rotation = HazardSprites[i].TotalRotation;
                                break;
                            }
                        case Sprite.Type.Fan:
                            {
                                if (HazardSprites[i].spriteBody.Enabled && HazardSprites[i].IsAwake)
                                {
                                    HazardSprites[i].Velocity += (int)(gameTime.ElapsedGameTime.Milliseconds);

                                    if (HazardSprites[i].Velocity > HazardSprites[i].HitPoints) //is timer over the fan on threshold
                                    {
                                        if (HazardSprites[i].IsAnimated == false) //if fan coming from off state
                                        {
                                            //turn fan on
                                            HazardSprites[i].IsAnimated = true;
                                            HazardSprites[i].Velocity = HazardSprites[i].HitPoints; //sync to effect

                                            //resets contacts, so things starting in fan wind will throw the contact event
                                            HazardSprites[i].spriteBody.Enabled = false;
                                            HazardSprites[i].spriteBody.Enabled = true;

                                            //indefinately animated fan setup
                                            if (HazardSprites[i].HitPoints == 0)
                                            {
                                                HazardSprites[i].Velocity = int.MaxValue;
                                            }

                                            contactListener.ActivateFan(HazardSprites[i]);//the effect manager listens for this event and creates/manages the wind effect
                                        }
                                        else //fan is already on, and running--generate wind
                                        {
                                            if(LevelDataManager.rand.Next(0,2) == 0) contactListener.CreateWindParticle(HazardSprites[i]);    
                                        }
                                    }
                                   
                                    //timer at threshold to turn fan off
                                    if (HazardSprites[i].Velocity >= 2 * HazardSprites[i].HitPoints && HazardSprites[i].HitPoints != 0)
                                    {
                                        //turn fan off--code is in effect sprite to turn effect off
                                        HazardSprites[i].IsAnimated = false;
                                        HazardSprites[i].CurrentFrame = 0;
                                        HazardSprites[i].Velocity = 0f; //reset hp
                                    }
                                }
                                if (HazardSprites[i].pathing == Sprite.Pathing.None) HazardSprites[i].spriteBody.ResetDynamics();
                                break;
                            }
                        case Sprite.Type.Saw:
                            {
                                if (HazardSprites[i].IsRotating)
                                {
                                    HazardSprites[i].TotalRotation += sawSpeed;
                                    HazardSprites[i].spriteBody.SetTransform(ConvertUnits.ToSimUnits(HazardSprites[i].SpriteCenterInWorld), HazardSprites[i].TotalRotation);
                                }

                                if (HazardSprites[i].pathing == Sprite.Pathing.None) HazardSprites[i].spriteBody.ResetDynamics();

                                break;
                            }
                        case Sprite.Type.Smasher:
                            {
                                #region SMAHSER (SPIDER) PATHING
                                float distance = Math.Abs(HazardSprites[i].PathingSpeed) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                float rangeX = HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X - HazardSprites[i].Location.X;
                                float rangeY = HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y - HazardSprites[i].Location.Y;
                                float range = (float)Math.Sqrt((double)((rangeX * rangeX) + (rangeY * rangeY)));
                                Vector2 direction = new Vector2(rangeX / range, rangeY / range);

                                if (HazardSprites[i].IsPathingInertia)
                                {
                                    if ((range >= (distance * 20f)) && (HazardSprites[i].pathingTravelled >= (distance * 20f)))
                                    {
                                        HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                        HazardSprites[i].pathingTravelled += distance;
                                    }
                                    else
                                    {
                                        if (range < (distance * 20f)) distance = range / 20f;
                                        if (HazardSprites[i].pathingTravelled < (distance * 20f)) distance = Math.Max(HazardSprites[i].pathingTravelled / 20f, 0.5f);
                                        HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                        HazardSprites[i].pathingTravelled += distance;
                                        if (range <= 0.5f)
                                        {
                                            HazardSprites[i].Location = new Vector2(HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X,
                                                                                    HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y);

                                            HazardSprites[i].Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                                            if (HazardSprites[i].Timer >= HazardSprites[i].TimeDelay)
                                            {
                                                HazardSprites[i].Timer = 0f;
                                                HazardSprites[i].pathingTravelled = 0f;
                                                if (HazardSprites[i].pathingPointsDestinationIndex == 1)
                                                {
                                                    HazardSprites[i].pathingPointsDestinationIndex = 0;
                                                    return;
                                                }
                                                if (HazardSprites[i].pathingPointsDestinationIndex == 0)
                                                {
                                                    HazardSprites[i].pathingPointsDestinationIndex = 1;
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!HazardSprites[i].IsPathingInertia)
                                {
                                    if (range >= distance)
                                    {
                                        HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                    }
                                    else
                                    {
                                        HazardSprites[i].Location = new Vector2(HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X,
                                                                                HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y);

                                        HazardSprites[i].Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                                        if (HazardSprites[i].Timer >= HazardSprites[i].TimeDelay)
                                        {
                                            HazardSprites[i].Timer = 0f;
                                            HazardSprites[i].pathingTravelled = 0f;
                                            if (HazardSprites[i].pathingPointsDestinationIndex == 1)
                                            {
                                                HazardSprites[i].pathingPointsDestinationIndex = 0;
                                                return;
                                            }
                                            if (HazardSprites[i].pathingPointsDestinationIndex == 0)
                                            {
                                                HazardSprites[i].pathingPointsDestinationIndex = 1;
                                                return;
                                            }
                                        }
                                    }
                                }
                                HazardSprites[i].spriteBody.SetTransform(ConvertUnits.ToSimUnits(HazardSprites[i].SpriteCenterInWorld), HazardSprites[i].TotalRotation);

                                if (HazardSprites[i].IsHit) contactListener.DoPoof(HazardSprites[i]);
                                break;
                                #endregion
                            }
                        case Sprite.Type.Bat:
                            {
                                #region BAT PATHING
                                float distance = Math.Abs(HazardSprites[i].PathingSpeed) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                float rangeX = HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X - HazardSprites[i].Location.X;
                                float rangeY = HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y - HazardSprites[i].Location.Y;
                                float range = (float)Math.Sqrt((double)((rangeX * rangeX) + (rangeY * rangeY)));
                                Vector2 direction = new Vector2(rangeX / range, rangeY / range);

                                //wobble
                                HazardSprites[i].Velocity += (float)gameTime.ElapsedGameTime.TotalSeconds;
                                if (HazardSprites[i].Velocity < 0.25f)
                                {
                                    HazardSprites[i].Direction = new Vector2 (0,1);
                                }
                                else
                                {
                                    HazardSprites[i].Direction = new Vector2(0,-1);
                                    if (HazardSprites[i].Velocity >= 0.50) HazardSprites[i].Velocity = 0f;
                                }

                                //facing
                                if (rangeX < 0) HazardSprites[i].IsFlippedHorizontally = true;
                                else HazardSprites[i].IsFlippedHorizontally = false;

                                if (HazardSprites[i].IsPathingInertia)
                                {
                                    if ((range >= (distance * 20f)) && (HazardSprites[i].pathingTravelled >= (distance * 20f)))
                                    {
                                        HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                        HazardSprites[i].pathingTravelled += distance;
                                    }
                                    else
                                    {
                                        if (range < (distance * 20f)) distance = range / 20f;
                                        if (HazardSprites[i].pathingTravelled < (distance * 20f)) distance = Math.Max(HazardSprites[i].pathingTravelled / 20f, 0.5f);
                                        HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                        HazardSprites[i].pathingTravelled += distance;
                                        if (range <= 0.5f)
                                        {
                                            HazardSprites[i].Location = new Vector2(HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X,
                                                                                    HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y);

                                            HazardSprites[i].Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                                            if (HazardSprites[i].Timer >= HazardSprites[i].TimeDelay)
                                            {
                                                HazardSprites[i].Timer = 0f;
                                                HazardSprites[i].pathingTravelled = 0f;
                                                if (HazardSprites[i].pathingPointsDestinationIndex == 1)
                                                {
                                                    HazardSprites[i].pathingPointsDestinationIndex = 0;
                                                    return;
                                                }
                                                if (HazardSprites[i].pathingPointsDestinationIndex == 0)
                                                {
                                                    HazardSprites[i].pathingPointsDestinationIndex = 1;
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!HazardSprites[i].IsPathingInertia)
                                {
                                    if (range >= distance)
                                    {
                                        HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                    }
                                    else
                                    {
                                        HazardSprites[i].Location = new Vector2(HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X,
                                                                                HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y);

                                        HazardSprites[i].Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                                        if (HazardSprites[i].Timer >= HazardSprites[i].TimeDelay)
                                        {
                                            HazardSprites[i].Timer = 0f;
                                            HazardSprites[i].pathingTravelled = 0f;
                                            if (HazardSprites[i].pathingPointsDestinationIndex == 1)
                                            {
                                                HazardSprites[i].pathingPointsDestinationIndex = 0;
                                                return;
                                            }
                                            if (HazardSprites[i].pathingPointsDestinationIndex == 0)
                                            {
                                                HazardSprites[i].pathingPointsDestinationIndex = 1;
                                                return;
                                            }
                                        }
                                    }
                                }
                                HazardSprites[i].Location += new Vector2(0, HazardSprites[i].Direction.Y);
                                HazardSprites[i].spriteBody.SetTransform(ConvertUnits.ToSimUnits(HazardSprites[i].SpriteCenterInWorld), HazardSprites[i].TotalRotation);
                                if (HazardSprites[i].IsHit) contactListener.ExplodeCreature(HazardSprites[i]);
                                break;
                                #endregion
                            }
                        case Sprite.Type.Spider:
                            {
                                #region SPIDER PATHING
                                float distance = Math.Abs(HazardSprites[i].PathingSpeed) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                float rangeX = HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X - HazardSprites[i].Location.X;
                                float rangeY = HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y - HazardSprites[i].Location.Y;
                                float range = (float)Math.Sqrt((double)((rangeX * rangeX) + (rangeY * rangeY)));
                                Vector2 direction = new Vector2(rangeX / range, rangeY / range);

                                if (HazardSprites[i].IsPathingInertia)
                                {
                                    if ((range >= (distance * 20f)) && (HazardSprites[i].pathingTravelled >= (distance * 20f)))
                                    {
                                        HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                        HazardSprites[i].pathingTravelled += distance;
                                    }
                                    else
                                    {
                                        if (range < (distance * 20f)) distance = range / 20f;
                                        if (HazardSprites[i].pathingTravelled < (distance * 20f)) distance = Math.Max(HazardSprites[i].pathingTravelled / 20f, 0.5f);
                                        HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                        HazardSprites[i].pathingTravelled += distance;
                                        if (range <= 0.5f)
                                        {
                                            HazardSprites[i].Location = new Vector2(HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X,
                                                                                    HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y);

                                            HazardSprites[i].Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                                            if (HazardSprites[i].Timer >= HazardSprites[i].TimeDelay)
                                            {
                                                HazardSprites[i].Timer = 0f;
                                                HazardSprites[i].pathingTravelled = 0f;
                                                if (HazardSprites[i].pathingPointsDestinationIndex == 1)
                                                {
                                                    HazardSprites[i].pathingPointsDestinationIndex = 0;
                                                    return;
                                                }
                                                if (HazardSprites[i].pathingPointsDestinationIndex == 0)
                                                {
                                                    HazardSprites[i].pathingPointsDestinationIndex = 1;
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!HazardSprites[i].IsPathingInertia)
                                {
                                    if (range >= distance)
                                    {
                                        HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                    }
                                    else
                                    {
                                        HazardSprites[i].Location = new Vector2(HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X,
                                                                                HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y);

                                        HazardSprites[i].Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                                        if (HazardSprites[i].Timer >= HazardSprites[i].TimeDelay)
                                        {
                                            HazardSprites[i].Timer = 0f;
                                            HazardSprites[i].pathingTravelled = 0f;
                                            if (HazardSprites[i].pathingPointsDestinationIndex == 1)
                                            {
                                                HazardSprites[i].pathingPointsDestinationIndex = 0;
                                                return;
                                            }
                                            if (HazardSprites[i].pathingPointsDestinationIndex == 0)
                                            {
                                                HazardSprites[i].pathingPointsDestinationIndex = 1;
                                                return;
                                            }
                                        }
                                    }
                                }
                                HazardSprites[i].spriteBody.SetTransform(ConvertUnits.ToSimUnits(HazardSprites[i].SpriteCenterInWorld), HazardSprites[i].TotalRotation);

                                if (HazardSprites[i].IsHit) contactListener.ExplodeCreature(HazardSprites[i]);

                                //create spider thread each tick
                                contactListener.CreateSpiderThread(HazardSprites[i]);
                                break;
                                #endregion
                            }

                        case Sprite.Type.Fish:
                        case Sprite.Type.FireBoo:
                            {
                                #region FISH PATHING
                                //jump timer
                                HazardSprites[i].Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                                if (HazardSprites[i].Timer > HazardSprites[i].TimeDelay)
                                {
                                    //initialize values for jump
                                    if (HazardSprites[i].pathingPoints[2] == Vector2.Zero)
                                    {
                                        HazardSprites[i].pathingPoints[2] = HazardSprites[i].Location;
                                        HazardSprites[i].pathingPoints[4] = new Vector2(HazardSprites[i].PathingRadiusX, HazardSprites[i].PathingRadiusY);
                                        HazardSprites[i].pathingPoints[5] = new Vector2(HazardSprites[i].PathingSpeed, HazardSprites[i].pathingTravelled);
                                        HazardSprites[i].pathingTravelled = 3.1416f;
                                        HazardSprites[i].PathingSpeed = 50 + (30000 / HazardSprites[i].PathingRadiusY);
                                        HazardSprites[i].PathingRadiusX = (int)(HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X - HazardSprites[i].Location.X);
                                        HazardSprites[i].pathingPoints[3] = new Vector2(HazardSprites[i].Location.X + HazardSprites[i].PathingRadiusX / 2,
                                                                                        HazardSprites[i].Location.Y);

                                        if (HazardSprites[i].SpriteType == Sprite.Type.Fish)
                                        {
                                            if (Camera.IsObjectVisible(HazardSprites[i].SpriteRectangle, Vector2.One)) contactListener.SplashWater(HazardSprites[i]);
                                        }
                                        if (HazardSprites[i].SpriteType == Sprite.Type.FireBoo)
                                        {
                                            if (Camera.IsObjectVisible(HazardSprites[i].SpriteRectangle, Vector2.One)) contactListener.SplashLava(HazardSprites[i]);
                                        }
                                    }


                                    //jump pathing
                                    float distance = HazardSprites[i].PathingSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                    HazardSprites[i].pathingTravelled += MathHelper.ToRadians(distance);
                                    HazardSprites[i].Location = new Vector2(HazardSprites[i].pathingPoints[3].X + (float)(Math.Cos(HazardSprites[i].pathingTravelled) * HazardSprites[i].PathingRadiusX / 2f),
                                                            HazardSprites[i].pathingPoints[3].Y + (float)(Math.Sin(HazardSprites[i].pathingTravelled) * HazardSprites[i].PathingRadiusY / 2f));
                                    if (HazardSprites[i].PathingRadiusX > 0) HazardSprites[i].TotalRotation = HazardSprites[i].pathingTravelled - MathHelper.ToRadians(180);
                                    else HazardSprites[i].TotalRotation = (HazardSprites[i].pathingTravelled - MathHelper.ToRadians(180)) * -1;
                                    
                                    if (HazardSprites[i].pathingTravelled > 6.2832f)
                                    {
                                        //jump deinitialize
                                        HazardSprites[i].Timer = 0f;
                                        HazardSprites[i].pathingPoints[2] = Vector2.Zero;
                                        HazardSprites[i].PathingRadiusX = (int)HazardSprites[i].pathingPoints[4].X;
                                        HazardSprites[i].PathingSpeed = (int)HazardSprites[i].pathingPoints[5].X;
                                        HazardSprites[i].TotalRotation = 0f;
                                        if (HazardSprites[i].SpriteType == Sprite.Type.Fish)
                                        {
                                            if (Camera.IsObjectVisible(HazardSprites[i].SpriteRectangle, Vector2.One)) contactListener.SplashWater(HazardSprites[i]);
                                        }
                                        if (HazardSprites[i].SpriteType == Sprite.Type.FireBoo)
                                        {
                                            if (Camera.IsObjectVisible(HazardSprites[i].SpriteRectangle, Vector2.One)) contactListener.SplashLava(HazardSprites[i]);
                                        }
                                    }
                                    HazardSprites[i].spriteBody.SetTransform(ConvertUnits.ToSimUnits(HazardSprites[i].SpriteCenterInWorld), HazardSprites[i].TotalRotation);
                                    if (HazardSprites[i].AnimationFramePrecise < 0.2f)
                                    {
                                        if (HazardSprites[i].SpriteType == Sprite.Type.Fish) contactListener.CreateDrop(HazardSprites[i]);
                                        if (HazardSprites[i].SpriteType == Sprite.Type.FireBoo)
                                        {
                                            contactListener.CreateEmber(HazardSprites[i]);
                                            contactListener.DoPoof(HazardSprites[i]);
                                        }
                                    }
                                }

                                //swimming horizonally (linear) behaivior
                                if (HazardSprites[i].Timer <= HazardSprites[i].TimeDelay)
                                {
                                    float distance = Math.Abs(HazardSprites[i].PathingSpeed) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                    float rangeX = HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X - HazardSprites[i].Location.X;
                                    float rangeY = HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y - HazardSprites[i].Location.Y;
                                    float range = (float)Math.Sqrt((double)((rangeX * rangeX) + (rangeY * rangeY)));
                                    Vector2 direction = new Vector2(rangeX / range, rangeY / range);

                                    //facing
                                    if ((HazardSprites[i].Location.X - HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X) > 0) HazardSprites[i].IsFlippedHorizontally = true;
                                    else HazardSprites[i].IsFlippedHorizontally = false;

                                    if (HazardSprites[i].IsPathingInertia)
                                    {
                                        if ((range >= (distance * 20f)) && (HazardSprites[i].pathingTravelled >= (distance * 20f)))
                                        {
                                            HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                            HazardSprites[i].pathingTravelled += distance;
                                        }
                                        else
                                        {
                                            if (range < (distance * 20f)) distance = range / 20f;
                                            if (HazardSprites[i].pathingTravelled < (distance * 20f)) distance = Math.Max(HazardSprites[i].pathingTravelled / 20f, 0.5f);
                                            HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                            HazardSprites[i].pathingTravelled += distance;
                                            if (range <= 0.5f)
                                            {
                                                HazardSprites[i].Location = new Vector2(HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X,
                                                                                        HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y);
                                                HazardSprites[i].pathingTravelled = 0f;
                                                if (HazardSprites[i].pathingPointsDestinationIndex == 1)
                                                {
                                                    HazardSprites[i].pathingPointsDestinationIndex = 0;
                                                    return;
                                                }
                                                if (HazardSprites[i].pathingPointsDestinationIndex == 0)
                                                {
                                                    HazardSprites[i].pathingPointsDestinationIndex = 1;
                                                    return;
                                                }
                                            }
                                        }

                                    }
                                    if (!HazardSprites[i].IsPathingInertia)
                                    {
                                        if (range >= distance)
                                        {
                                            HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                        }
                                        else
                                        {
                                            HazardSprites[i].Location = new Vector2(HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X,
                                                                                     HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y);
                                            HazardSprites[i].pathingTravelled = 0f;
                                            if (HazardSprites[i].pathingPointsDestinationIndex == 1)
                                            {
                                                HazardSprites[i].pathingPointsDestinationIndex = 0;
                                                return;
                                            }
                                            if (HazardSprites[i].pathingPointsDestinationIndex == 0)
                                            {
                                                HazardSprites[i].pathingPointsDestinationIndex = 1;
                                                return;
                                            }
                                        }
                                    }
                                }
                                HazardSprites[i].spriteBody.SetTransform(ConvertUnits.ToSimUnits(HazardSprites[i].SpriteCenterInWorld), HazardSprites[i].TotalRotation);
                                if (HazardSprites[i].IsHit) contactListener.ExplodeCreature(HazardSprites[i]);
                                break;
                                #endregion
                            }

                        case Sprite.Type.Bird:
                            {
                                #region BIRD PATHING
                                //swoop timer
                                if (levelState == GameplayScreen.LevelState.Fire)
                                {
                                    float rangeX = fruitShotLocation.X - HazardSprites[i].Location.X;
                                    float rangeY = fruitShotLocation.Y - HazardSprites[i].Location.Y;
                                    //trigger a swoop if the fruit is near and below
                                    if ((Math.Abs(rangeX) < 200) && rangeY > 0) HazardSprites[i].Timer = HazardSprites[i].TimeDelay + 1;
                                }
                                else
                                {
                                    HazardSprites[i].Timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                                }

                                //swoop
                                if (HazardSprites[i].Timer > HazardSprites[i].TimeDelay)
                                {

                                    //initialize values for swoop
                                    if (HazardSprites[i].pathingPoints[2] == Vector2.Zero)
                                    {
                                        HazardSprites[i].pathingPoints[2] = HazardSprites[i].Location;
                                        HazardSprites[i].pathingPoints[4] = new Vector2(HazardSprites[i].PathingRadiusX, HazardSprites[i].PathingRadiusY);
                                        HazardSprites[i].pathingPoints[5] = new Vector2(HazardSprites[i].PathingSpeed, HazardSprites[i].pathingTravelled);
                                        HazardSprites[i].pathingTravelled = -3.14f;
                                        HazardSprites[i].PathingSpeed = (HazardSprites[i].PathingSpeed * 3) / 4;
                                        if ((HazardSprites[i].Location.X - HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X) >= 0)
                                            HazardSprites[i].PathingRadiusX = -200;
                                        else HazardSprites[i].PathingRadiusX = 200;
                                        HazardSprites[i].pathingPoints[3] = new Vector2(HazardSprites[i].Location.X + HazardSprites[i].PathingRadiusX / 2,
                                                                                        HazardSprites[i].Location.Y);

                                        if (Camera.IsObjectVisible(HazardSprites[i].SpriteRectangle, Vector2.One)) SoundManager.Play(SoundManager.Sound.AnimalBird, false, true);
                                    }


                                    //swoop pathing
                                    float distance = HazardSprites[i].PathingSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                    HazardSprites[i].pathingTravelled -= MathHelper.ToRadians(distance);
                                    HazardSprites[i].Location = new Vector2(HazardSprites[i].pathingPoints[3].X + (float)(Math.Cos(HazardSprites[i].pathingTravelled) * HazardSprites[i].PathingRadiusX / 2f),
                                                            HazardSprites[i].pathingPoints[3].Y + (float)(Math.Sin(HazardSprites[i].pathingTravelled) * HazardSprites[i].PathingRadiusY / 2f));

                                    if (HazardSprites[i].pathingTravelled < -6.28f)
                                    {
                                        //swoop deinitialize
                                        HazardSprites[i].Timer = 0f;
                                        HazardSprites[i].TimeDelay = LevelDataManager.rand.Next(0, 10);
                                        HazardSprites[i].pathingPoints[2] = Vector2.Zero;
                                        HazardSprites[i].PathingRadiusX = (int)HazardSprites[i].pathingPoints[4].X;
                                        HazardSprites[i].PathingSpeed = (int)HazardSprites[i].pathingPoints[5].X;
                                        HazardSprites[i].TotalRotation = 0f;
                                    }
                                    HazardSprites[i].spriteBody.SetTransform(ConvertUnits.ToSimUnits(HazardSprites[i].SpriteCenterInWorld), HazardSprites[i].TotalRotation);
                                }

                                //flying horizonally (linear) behaivior
                                if (HazardSprites[i].Timer <= HazardSprites[i].TimeDelay)
                                {
                                    float distance = Math.Abs(HazardSprites[i].PathingSpeed) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                    float rangeX = HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X - HazardSprites[i].Location.X;
                                    float rangeY = HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y - HazardSprites[i].Location.Y;
                                    float range = (float)Math.Sqrt((double)((rangeX * rangeX) + (rangeY * rangeY)));
                                    Vector2 direction = new Vector2(rangeX / range, rangeY / range);

                                    //facing
                                    if ((HazardSprites[i].Location.X - HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X) > 0) HazardSprites[i].IsFlippedHorizontally = true;
                                    else HazardSprites[i].IsFlippedHorizontally = false;

                                    if (HazardSprites[i].IsPathingInertia)
                                    {
                                        if ((range >= (distance * 20f)) && (HazardSprites[i].pathingTravelled >= (distance * 20f)))
                                        {
                                            HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                            HazardSprites[i].pathingTravelled += distance;
                                        }
                                        else
                                        {
                                            if (range < (distance * 20f)) distance = range / 20f;
                                            if (HazardSprites[i].pathingTravelled < (distance * 20f)) distance = Math.Max(HazardSprites[i].pathingTravelled / 20f, 0.5f);
                                            HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                            HazardSprites[i].pathingTravelled += distance;
                                            if (range <= 0.5f)
                                            {
                                                HazardSprites[i].Location = new Vector2(HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X,
                                                                                        HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y);
                                                HazardSprites[i].pathingTravelled = 0f;
                                                if (HazardSprites[i].pathingPointsDestinationIndex == 1)
                                                {
                                                    HazardSprites[i].pathingPointsDestinationIndex = 0;
                                                    return;
                                                }
                                                if (HazardSprites[i].pathingPointsDestinationIndex == 0)
                                                {
                                                    HazardSprites[i].pathingPointsDestinationIndex = 1;
                                                    return;
                                                }
                                            }
                                        }

                                    }
                                    if (!HazardSprites[i].IsPathingInertia)
                                    {
                                        if (range >= distance)
                                        {
                                            HazardSprites[i].Location += new Vector2((direction.X * distance), (direction.Y * distance));
                                        }
                                        else
                                        {
                                            HazardSprites[i].Location = new Vector2(HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].X,
                                                                                     HazardSprites[i].pathingPoints[HazardSprites[i].pathingPointsDestinationIndex].Y);
                                            HazardSprites[i].pathingTravelled = 0f;
                                            if (HazardSprites[i].pathingPointsDestinationIndex == 1)
                                            {
                                                HazardSprites[i].pathingPointsDestinationIndex = 0;
                                                return;
                                            }
                                            if (HazardSprites[i].pathingPointsDestinationIndex == 0)
                                            {
                                                HazardSprites[i].pathingPointsDestinationIndex = 1;
                                                return;
                                            }
                                        }
                                    }
                                }
                                HazardSprites[i].spriteBody.SetTransform(ConvertUnits.ToSimUnits(HazardSprites[i].SpriteCenterInWorld), HazardSprites[i].TotalRotation);
                                if (HazardSprites[i].IsHit) contactListener.ExplodeCreature(HazardSprites[i]);
                                break;
                                #endregion
                            }

                        case Sprite.Type.Gremlin:
                            {
                                #region GREMLIN PATHING
                                if ((levelState == GameplayScreen.LevelState.Fire || levelState == GameplayScreen.LevelState.PowerUpFire) && !shotExpired)
                                {
                                    float distance = Math.Abs(HazardSprites[i].PathingSpeed) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                    float rangeX = fruitShotLocation.X - HazardSprites[i].Location.X;
                                    float rangeY = fruitShotLocation.Y - HazardSprites[i].Location.Y;
                                    float range = (float)Math.Sqrt((double)((rangeX * rangeX) + (rangeY * rangeY)));
                                    Vector2 direction = new Vector2(rangeX / range, rangeY / range);
                                    Vector2 actualMovement = new Vector2((direction.X * distance), (direction.Y * distance));
                                    HazardSprites[i].Location += actualMovement;
                                    HazardSprites[i].pathingPoints[1] += actualMovement;
                                    HazardSprites[i].spriteBody.SetTransform(ConvertUnits.ToSimUnits(HazardSprites[i].SpriteCenterInWorld), HazardSprites[i].TotalRotation);

                                    //faces the correct direction for movement
                                    if (actualMovement.X < 0) HazardSprites[i].IsFlippedHorizontally = true;
                                    else HazardSprites[i].IsFlippedHorizontally = false;
                                }
                                else
                                {
                                    float distance = HazardSprites[i].PathingSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                                    HazardSprites[i].pathingTravelled += MathHelper.ToRadians(distance);
                                    HazardSprites[i].Location = new Vector2(HazardSprites[i].pathingPoints[1].X + (float)(Math.Cos(HazardSprites[i].pathingTravelled) * HazardSprites[i].PathingRadiusX / 2f),
                                                            HazardSprites[i].pathingPoints[1].Y + (float)(Math.Sin(HazardSprites[i].pathingTravelled) * HazardSprites[i].PathingRadiusY / 2f));
                                    if (HazardSprites[i].pathingTravelled > 10f) HazardSprites[i].pathingTravelled -= MathHelper.ToRadians(360);
                                    if (HazardSprites[i].pathingTravelled < -10f) HazardSprites[i].pathingTravelled += MathHelper.ToRadians(360);
                                    HazardSprites[i].spriteBody.SetTransform(ConvertUnits.ToSimUnits(HazardSprites[i].SpriteCenterInWorld), HazardSprites[i].TotalRotation);

                                    //faces the correct direction of movement
                                    if (Math.Sin(HazardSprites[i].pathingTravelled) > 0) HazardSprites[i].IsFlippedHorizontally = true;
                                    else HazardSprites[i].IsFlippedHorizontally = false;
                                }

                                if (HazardSprites[i].IsHit) contactListener.ExplodeCreature(HazardSprites[i]);
                                break;
                                #endregion
                            }
                        case Sprite.Type.Switch:
                            {
                                AnimateSwitch(HazardSprites[i], gameTime, levelState);
                                break;
                            }
                        case Sprite.Type.Spring:
                            {
                                //deactivate animation if finished 
                                if (HazardSprites[i].IsAnimated)
                                {
                                    if (HazardSprites[i].CurrentFrame == 0 && HazardSprites[i].IsAnimationDirectionForward == false)
                                    {
                                        HazardSprites[i].IsAnimated = false;
                                    }
                                }

                                //update color spring switchting
                                if (HazardSprites[i].TextureIndex >= 4) UpdateFadeAnimation(gameTime, HazardSprites[i]); //hit springs are for switches, fade them out or in, hit is out, not hit is in
                                break;
                            }
                        case Sprite.Type.Flame:
                        case Sprite.Type.FireballBlast:
                            {
                                if (HazardSprites[i].spriteBody.Enabled && HazardSprites[i].IsAwake)
                                {
                                    HazardSprites[i].Velocity += (int)(gameTime.ElapsedGameTime.Milliseconds);
                                    if (HazardSprites[i].Velocity > HazardSprites[i].HitPoints && HazardSprites[i].IsAnimated == false)
                                    {
                                        //turn flame on
                                        HazardSprites[i].IsAnimated = true;
                                        HazardSprites[i].CurrentFrame = 0;
                                        HazardSprites[i].Velocity = HazardSprites[i].HitPoints; //sync to effect
                                        HazardSprites[i].SpriteType = Sprite.Type.FireballBlast;

                                        //indefinately animated setup
                                        if (HazardSprites[i].HitPoints == 0)
                                        {
                                            HazardSprites[i].Velocity = int.MaxValue;
                                        }

                                    }
                                    if (HazardSprites[i].Velocity >= 2 * HazardSprites[i].HitPoints && HazardSprites[i].HitPoints != 0)
                                    {
                                        //turn flame off--code is in effect sprite to turn effect off
                                        HazardSprites[i].IsAnimated = false;
                                        HazardSprites[i].CurrentFrame = 4;
                                        HazardSprites[i].Velocity = 0f; //reset hp
                                        HazardSprites[i].SpriteType = Sprite.Type.Flame;
                                    }
                                }
                                break;
                            }
                        case Sprite.Type.Snowman:
                            {
                                if (HazardSprites[i].IsAnimated == true && HazardSprites[i].CurrentFrame == 0)
                                {
                                    //reset snowman if throw animation finished
                                    HazardSprites[i].IsAnimated = false;
                                    HazardSprites[i].CurrentFrame = 0;
                                    HazardSprites[i].AnimationFramePrecise = 0f;
                                    HazardSprites[i].Velocity = 0f;
                                }

                                HazardSprites[i].Velocity += (int)(gameTime.ElapsedGameTime.Milliseconds);
                                if (HazardSprites[i].Velocity > HazardSprites[i].HitPoints && HazardSprites[i].IsAnimated == false)
                                    {
                                        //throw snowball
                                        contactListener.ThrowSnowball(HazardSprites[i]);
                                        HazardSprites[i].IsAnimated = true;
                                        HazardSprites[i].CurrentFrame = 1;
                                        HazardSprites[i].AnimationFramePrecise = 1f;
                                    }
                                break;
                            }
                        case Sprite.Type.Beehive:
                            {
                                if (HazardSprites[i].IsHit) contactListener.ExplodeCreature(HazardSprites[i]);
                                break;
                            }
                        case Sprite.Type.Tower:
                            {
                                if (HazardSprites[i].TextureID == 10) //lights on tower block
                                {
                                    HazardSprites[i].HitPoints -= (int)(gameTime.ElapsedGameTime.Milliseconds);
                                    if (HazardSprites[i].HitPoints <= 0) HazardSprites[i].TextureID = 11;
                                }
                                //if (HazardSprites[i].TextureID == 11) //lights off tower block handled by updateTower

                                //if (HazardSprites[i].TextureID == 12) //excliaimation tower block
                                //{
                                //    HazardSprites[i].HitPoints -= (int)(gameTime.ElapsedGameTime.Milliseconds);
                                //    if (HazardSprites[i].HitPoints < 0)
                                //    {
                                //        if (LevelDataManager.rand.Next(0, 200) == 0) //fire TNT barrel
                                //        {
                                //            HazardSprites[i].HitPoints = 5000;
                                //            contactListener.DoCreateTNTBarrel(HazardSprites[i]);
                                //        }
                                //    }
                                //}
                                break;
                            }
                        default:
                            break;
                    }

                    //remove hazards that are hit, spring exempted
                    if (HazardSprites[i].IsHit)
                    {
                        if (HazardSprites[i].SpriteType != Sprite.Type.Spring) UpdateDeathAnimation(gameTime, HazardSprites[i]);
                    }
                }
            }
            return;
        }


        public void UpdateTowers(GameTime gameTime)
        {
            towerTimer -= (int)(gameTime.ElapsedGameTime.Milliseconds);
            if (towerTimer <= 0) //if timer reaches 0 fire towers
            {
                if (rightTower)
                {
                    towerNumber = (int)LevelDataManager.rand.Next(0, 6);
                    rightTower = false;
                }
                else
                {
                    towerNumber = (int)LevelDataManager.rand.Next(6, 12);
                    rightTower = true;
                }

                TowerSprites[towerNumber].TextureID = 10;
                TowerSprites[towerNumber].HitPoints = 500;
                contactListener.ShootTower(TowerSprites[towerNumber]);
                towerTimer += towerTimerTotal;
            }
            return;
        }

        public int SpriteCount
        {
            get { return HazardSprites.Count; }
        }

        private void UpdateFadeAnimation(GameTime gameTime, Sprite sprite)
        {
            if (sprite.IsHit && sprite.spriteBody.IsBullet) return; //switch block is off
            if (!sprite.IsHit && !sprite.spriteBody.IsBullet) return; //switch block is on
            if (sprite.IsHit && !sprite.spriteBody.IsBullet) //switch block is turning off
            {
                int alpha = sprite.TintColor.A;
                alpha -= (int)gameTime.ElapsedGameTime.TotalMilliseconds / 2;
                if (alpha < 50)
                {
                    alpha = 50;
                    sprite.spriteBody.IsBullet = true;
                }
                sprite.TintColor = new Color(alpha, alpha, alpha, alpha);
            }
            if (!sprite.IsHit && sprite.spriteBody.IsBullet) //switch block coming on
            {
                int alpha = sprite.TintColor.A;
                alpha += (int)gameTime.ElapsedGameTime.TotalMilliseconds / 2;
                if (alpha > 255)
                {
                    alpha = 255;
                    sprite.spriteBody.IsBullet = false;
                }
                sprite.TintColor = new Color(alpha, alpha, alpha, alpha);
            }
            return;
        }

        public void UpdateDeathAnimation(GameTime gameTime, Sprite sprite)
        {
            sprite.IsExpired = true;
            physicsWorld.RemoveBody(sprite.spriteBody);
            HazardSprites.Remove(sprite);
            return;
        }

        public void AnimateSwitch(Sprite sprite, GameTime gameTime, GameplayScreen.LevelState _levelState)
        {
            sprite.Timer += (float)gameTime.ElapsedGameTime.TotalSeconds * sprite.AnimationFPS;

            //process frame change if enough time has elapsed
            if (sprite.Timer >= 1.0f)
            {
                if (sprite.CurrentFrame == 0)
                {
                    if (LevelDataManager.rand.Next(0, 100) < 8)
                    {
                        sprite.CurrentFrame = 1;
                        sprite.Timer -= 1.0f;
                        return;
                    }
                    else sprite.Timer -= 1.0f;
                }

                if (sprite.CurrentFrame == 1)
                {
                    sprite.CurrentFrame = 0;
                    sprite.Timer -= 1.0f;
                    return;
                }

                if (sprite.CurrentFrame == 2)
                {
                    //ensures once switch hit once per second (12 frames)
                    if (sprite.Timer >= 12.0f)
                    {
                        sprite.CurrentFrame = 0;
                        sprite.Timer = 0f;
                    }
                    return;
                }

            }
            return;
        }

        #region CREATE OBJECTS
        public void CreateCreature(Sprite sprite, World physicsWorld)
        {
            HazardSprites.Add(sprite);
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            
            string filename = LevelDataManager.GetFileNameByTextureID(sprite.TextureID);
            if (filename.Contains("Hazards"))
            {
                Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
                sprite.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits((sprite.SpriteRectWidth / 2.0f) - 10), ConvertUnits.ToSimUnits((sprite.SpriteRectWidth / 2.0f) - 10), 8, 0.9f, position, sprite);
                sprite.spriteBody.Mass = 0.15f;
                sprite.spriteBody.Rotation = sprite.TotalRotation;
                sprite.spriteBody.BodyType = BodyType.Kinematic;
                sprite.spriteBody.IgnoreGravity = true;
                sprite.spriteBody.Restitution = 0.5f;
                sprite.spriteBody.Friction = 0.5f;
                if (sprite.HitPoints == 0) sprite.HitPoints = 50;
                sprite.IsAnimated = true;
                sprite.IsAnimatedWhileStopped = true;
                sprite.AnimationFPS = 12.0f;
                sprite.IsBounceAnimated = true;
                sprite.CurrentFrame = LevelDataManager.rand.Next(0, 5);

                switch ((int)(sprite.TextureIndex / 5))
                {
                    case 0: //fish
                        {
                            sprite.SpriteType = Sprite.Type.Fish;
                            if (sprite.pathing != Sprite.Pathing.Fish)
                            {
                                sprite.pathing = Sprite.Pathing.Fish;
                                sprite.IsPathingInertia = false;
                                sprite.PathingRadiusX = 200;
                                sprite.PathingRadiusY = 800;
                                sprite.PathingSpeed = 200;
                                sprite.TimeDelay = 4;
                                sprite.Timer = (float)LevelDataManager.rand.Next(0, 300)/100f;
                                sprite.InitializePathing();
                            }
                            if (sprite.PathingRadiusY == 0) sprite.PathingRadiusY = 600;
                            break;
                        }
                    case 1: //beehive
                        {
                            sprite.SpriteType = Sprite.Type.Beehive;
                            sprite.spriteBody.IsSensor = true;
                            break;
                        }
                    case 2: //bat
                        {
                            sprite.SpriteType = Sprite.Type.Bat;
                            if (sprite.pathing != Sprite.Pathing.Bat)
                            {
                                sprite.pathing = Sprite.Pathing.Bat;
                                sprite.IsPathingInertia = false;
                                sprite.PathingRadiusX = 400;
                                sprite.PathingRadiusY = 0;
                                sprite.PathingSpeed = 200;
                                sprite.TimeDelay = 0;
                                sprite.InitializePathing();
                            }
                            sprite.AnimationFPS = 20f;
                            break;
                        }
                    case 3: //spider
                        {
                            sprite.SpriteType = Sprite.Type.Spider;
                            if (sprite.pathing != Sprite.Pathing.Spider)
                            {
                                sprite.pathing = Sprite.Pathing.Spider;
                                sprite.IsPathingInertia = false;
                                sprite.PathingRadiusX = 0;
                                sprite.PathingRadiusY = 400;
                                sprite.PathingSpeed = 100;
                                sprite.TimeDelay = 3;
                                sprite.Timer = (float)LevelDataManager.rand.Next(0, 300) / 100f;
                                sprite.InitializePathing();
                            }
                            break;
                        }
                    case 4: //bird
                        {
                            sprite.SpriteType = Sprite.Type.Bird;
                            if (sprite.pathing != Sprite.Pathing.Bird)
                            {
                                sprite.pathing = Sprite.Pathing.Bird;
                                sprite.IsPathingInertia = false;
                                sprite.PathingRadiusX = 400;
                                sprite.PathingRadiusY = 600;
                                sprite.PathingSpeed = 200;
                                sprite.TimeDelay = 3;
                                sprite.InitializePathing();
                            }
                            if (sprite.PathingRadiusY == 0) sprite.PathingRadiusY = 600;
                            sprite.AnimationFPS = 20f;
                            break;
                        }
                    case 5: //gremlin
                        {
                            sprite.SpriteType = Sprite.Type.Gremlin;
                            if (sprite.pathing != Sprite.Pathing.Gremlin)
                            {
                                sprite.pathing = Sprite.Pathing.Gremlin;
                                sprite.IsPathingInertia = false;
                                sprite.PathingRadiusX = 100;
                                sprite.PathingRadiusY = 100;
                                sprite.PathingSpeed = 100;
                                sprite.TimeDelay = 0;
                                sprite.InitializePathing();
                            }
                            break;
                        }
                    case 6: //fireboo
                        {
                            sprite.SpriteType = Sprite.Type.FireBoo;
                            sprite.spriteBody.IsSensor = true;
                            if (sprite.pathing != Sprite.Pathing.Fish)
                            {
                                sprite.pathing = Sprite.Pathing.Fish;
                                sprite.IsPathingInertia = false;
                                sprite.PathingRadiusX = 200;
                                sprite.PathingRadiusY = 800;
                                sprite.PathingSpeed = 200;
                                sprite.TimeDelay = 4;
                                sprite.Timer = (float)LevelDataManager.rand.Next(0, 300) / 100f;
                                sprite.InitializePathing();
                            }
                            if (sprite.PathingRadiusY == 0) sprite.PathingRadiusY = 600;
                            break;
                        }
                    default:
                        break;
                }


            }

            if (filename.Contains("Snowman"))
            {
                sprite.SpriteType = Sprite.Type.Snowman;
                sprite.CurrentFrame = 0;
                sprite.IsAnimated = false;
                sprite.IsAnimatedWhileStopped = true;
                sprite.AnimationFPS = 12.0f;
                sprite.IsBounceAnimated = false;
                Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
                sprite.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits((sprite.SpriteRectWidth / 2.0f) - 10), ConvertUnits.ToSimUnits((sprite.SpriteRectHeight / 2.0f) - 10), 8, 0.9f, position, sprite);
                sprite.spriteBody.Mass = 0.5f;
                sprite.spriteBody.Rotation = sprite.TotalRotation;
                sprite.spriteBody.BodyType = BodyType.Kinematic;
                sprite.spriteBody.IgnoreGravity = true;
                sprite.spriteBody.Restitution = 0.5f;
                sprite.spriteBody.Friction = 0.5f;
            }
            return;
        }

        public void CreateWindmillBlade(Sprite sprite, World physicsWorld)
        {
            sprite.SpriteType = Sprite.Type.Windmill;
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            HazardSprites.Add(sprite);
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(4), 0.9f, position, sprite);
            FixtureFactory.AttachRectangle(ConvertUnits.ToSimUnits(4), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 0.9f, Vector2.Zero, sprite.spriteBody);
            sprite.spriteBody.Rotation = sprite.TotalRotation;
            sprite.spriteBody.BodyType = BodyType.Kinematic;
            sprite.spriteBody.Restitution = 0.5f;
            sprite.spriteBody.Friction = 0.5f;
        }

        public void CreateTower(Sprite sprite, World physicsWorld)
        {
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            HazardSprites.Add(sprite);
            if (sprite.TextureID == 11) TowerSprites.Add(sprite);
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 0.9f, position, sprite);
            sprite.spriteBody.Rotation = 0;
            sprite.spriteBody.BodyType = BodyType.Static;
            sprite.spriteBody.IgnoreGravity = true;
            sprite.spriteBody.Restitution = 0.5f;
            sprite.spriteBody.Friction = 0.5f;
            if (sprite.TextureID == 12) sprite.HitPoints = 5000; //barrel shooter 5 sec delay
        }

        public void CreateFan(Sprite sprite, World physicsWorld)
        {
            //creates fan
            sprite.Velocity = 0f;
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            sprite.IsAnimated = false;
            sprite.IsAnimatedWhileStopped = true;
            sprite.AnimationFPS = fanSpeed;
            sprite.SpriteType = Sprite.Type.Fan;
            HazardSprites.Add(sprite);
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 0.9f, position, sprite);
            sprite.spriteBody.Rotation = sprite.TotalRotation;
            sprite.spriteBody.BodyType = BodyType.Kinematic;
            sprite.spriteBody.IgnoreGravity = true;
            sprite.spriteBody.Restitution = 0.5f;
            sprite.spriteBody.Friction = 0.5f;

            //creates the hitbox for wind
            if (sprite.spriteBody.FixtureList.Count == 1)
            {
                Vertices windVertices = new Vertices();
                windVertices.Add(ConvertUnits.ToSimUnits(new Vector2(-32, -332)));
                windVertices.Add(ConvertUnits.ToSimUnits(new Vector2(32, -332)));
                windVertices.Add(ConvertUnits.ToSimUnits(new Vector2(32, 12)));
                windVertices.Add(ConvertUnits.ToSimUnits(new Vector2(-32, 12)));
                sprite.spriteBody.CreateFixture(new PolygonShape(windVertices, 0f));
                sprite.spriteBody.FixtureList[1].IsSensor = true;
            }

            return;   
        }

        public void CreateSaw(Sprite sprite, World physicsWorld)
        {
            sprite.SpriteType = Sprite.Type.Saw;
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            sprite.IsRotating = true;
            HazardSprites.Add(sprite);
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            sprite.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth / 2.0f), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight / 2.0f), 10, 0.9f, position, sprite);
            sprite.spriteBody.Rotation = 0;
            sprite.spriteBody.BodyType = BodyType.Kinematic;
            sprite.spriteBody.IgnoreGravity = true;
            sprite.spriteBody.IsSensor = true;
            return;
        }

        public void CreateSmasher(Sprite sprite, World physicsWorld)
        {
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            sprite.IsRotating = false;
            HazardSprites.Add(sprite);
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(120), ConvertUnits.ToSimUnits(120), 0.9f, position, sprite);
            sprite.spriteBody.Rotation = 0;
            sprite.spriteBody.BodyType = BodyType.Kinematic;
            sprite.spriteBody.IgnoreGravity = true;
            sprite.spriteBody.IsSensor = true;
            return;
        }

        public void CreateSwitch(Sprite sprite, World physicsWorld)
        {
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            HazardSprites.Add(sprite);
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(64), ConvertUnits.ToSimUnits(64), 0.9f, position, sprite);
            sprite.spriteBody.Rotation = 0;
            sprite.spriteBody.Restitution = 0.5f;
            sprite.spriteBody.Friction = 0.5f;
            sprite.spriteBody.BodyType = BodyType.Kinematic;
            sprite.spriteBody.IgnoreGravity = true;
            sprite.AnimationFPS = 8f;
            sprite.AnimationFramePrecise = 0;
            return;
        }

        public void CreateSpike(Sprite sprite, World physicsWorld)
        {
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            HazardSprites.Add(sprite);
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(32), ConvertUnits.ToSimUnits(32), 0.9f, position, sprite);
            sprite.spriteBody.Rotation = sprite.TotalRotation;
            sprite.spriteBody.BodyType = BodyType.Kinematic;
            sprite.spriteBody.IgnoreGravity = true;
            sprite.spriteBody.IsSensor = true;
            return;
        }

        public void CreateSpring(Sprite sprite, World physicsWorld)
        {
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            sprite.AnimationFPS = 24;
            sprite.IsAnimationDirectionForward = true;
            sprite.IsAnimatedWhileStopped = true;
            sprite.IsBounceAnimated = true;
            sprite.SpriteType = Sprite.Type.Spring;
            HazardSprites.Add(sprite);
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight/2), 0.9f, position, sprite);
            sprite.spriteBody.Rotation = sprite.TotalRotation;
            sprite.spriteBody.BodyType = BodyType.Kinematic;
            sprite.spriteBody.IgnoreGravity = true;
            sprite.spriteBody.IsSensor = true;
            sprite.spriteBody.Friction = 0f;
            sprite.spriteBody.Restitution = 1f;
            return;
        }

        public void CreateFlamethrower(Sprite sprite, World physicsWorld)
        {
            sprite.Velocity = 0f;
            sprite.IsAnimated = false;
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            sprite.AnimationFPS = 16;
            sprite.IsAnimationDirectionForward = true;
            sprite.IsAnimatedWhileStopped = true;
            sprite.IsBounceAnimated = false;
            sprite.CurrentFrame = 4;
            sprite.SpriteType = Sprite.Type.Flame;
            HazardSprites.Add(sprite);
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 0.9f, position, sprite);
            sprite.spriteBody.Rotation = sprite.TotalRotation;
            sprite.spriteBody.BodyType = BodyType.Kinematic;
            sprite.spriteBody.IgnoreGravity = true;
            sprite.spriteBody.IsSensor = true;
            sprite.spriteBody.Friction = 0f;
            sprite.spriteBody.Restitution = 0f;
            return;
        }

        #endregion
    }
}
