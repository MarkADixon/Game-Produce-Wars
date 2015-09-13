using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using ParallaxEngine;
using FarseerPhysics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Common.PhysicsLogic;

namespace ProduceWars.Managers
{
    public class ShotManager
    {

        public List<Sprite> ammoUI;
        public List<Sprite> splitShot;
        public bool isSplit = false;
        public Sprite shot;

        private List<Sprite> PowerUpSprites;
        public Sprite ShotStartBarrel;
        public Sprite ActivePowerUpBarrel;
        public Sprite powerStar;
        public bool powerStarCollected = false;

        private World physicsWorld;
        private ContactListener contactListener;
        private float animationSpeed = 6.0f; //fps
        private float fireicelitanimationspeed = 16.0f;
        private int blinkChance = 20; //%chance for blink
        public Vector2 ammoUILocation = new Vector2(316, 72);
        public Vector2 bossAmmoLocation = new Vector2(172, 620);
        public int selectedAmmo = 0;
        private const float shotTerminalVelocity = 26f;
        private Vector2 fireicelitFixedVelocity = Vector2.Zero;

        public bool isAppleCopter = false;
        public int appleCopterTimer = 0; //in ms
        private int appleCopterTimerTick = 250; //threshold to tick the sound and frame (ms)

        public ShotManager(World _world, ContactListener _contactListener)
        {

            ammoUI = new List<Sprite>();

            splitShot = new List<Sprite>(3);
            splitShot.Add(new Sprite(20, 63, Vector2.Zero,false));
            splitShot.Add(new Sprite(20, 63, Vector2.Zero,false));
            splitShot.Add(new Sprite(20, 63, Vector2.Zero,false));
            for (int i=0; i < 3; i++)
            {
                splitShot[i].SpriteType = Sprite.Type.FruitShot;
                splitShot[i].IsVisible = false;
                splitShot[i].IsExpired = false;
                splitShot[i].IsAwake = false;
                splitShot[i].IsHit = false;
                splitShot[i].TintColor = Color.White;
                splitShot[i].AnimationFPS = animationSpeed;
                splitShot[i].CurrentFrame = LevelDataManager.rand.Next(0, 3);
                splitShot[i].pathing = Sprite.Pathing.None;
            }


            shot = new Sprite(20, 0, Vector2.Zero,false);
            shot.IsExpired = false;
            shot.HitPoints = 1;
            shot.IsVisible = false;
            shot.SpriteType = Sprite.Type.FruitShot;

            PowerUpSprites = new List<Sprite>();
            ActivePowerUpBarrel = new Sprite(0, 0, Vector2.Zero, false);
            ActivePowerUpBarrel.IsExpired = true;
            ShotStartBarrel = new Sprite(0, 0, Vector2.Zero, false);
            ShotStartBarrel.IsExpired = true;

            physicsWorld = _world;
            contactListener = _contactListener;
            contactListener.PowerUpActivated += new ContactListener.EffectEventHandler(contactListener_PowerUpActivated);
            contactListener.DamageShot += new ContactListener.DamageEventHandler(contactListener_DamageShot);
            contactListener.StarCollected += new ContactListener.EffectEventHandler(contactListener_StarCollected);
            contactListener.BananaActivated += new ContactListener.EffectEventHandler(contactListener_BananaActivated);
            contactListener.LemonActivated += new ContactListener.EffectEventHandler(contactListener_LemonActivated);
            contactListener.AddPowerBarrel += new ContactListener.EffectEventHandler(contactListener_AddPowerBarrel);
            
            #region setup fruit in the Ammo UI, detect unlocked
            if (GameSettings.isBoss)
            {
                ammoUI.Add(new Sprite(20, 0, new Vector2(bossAmmoLocation.X + 6, bossAmmoLocation.Y + 0), false));
                ammoUI.Add(new Sprite(20, 21, new Vector2(bossAmmoLocation.X + 70, bossAmmoLocation.Y + 0), false));
                ammoUI.Add(new Sprite(20, 28, new Vector2(bossAmmoLocation.X + 134, bossAmmoLocation.Y + 0), false));
                ammoUI.Add(new Sprite(20, 35, new Vector2(bossAmmoLocation.X + 198, bossAmmoLocation.Y + 0), false));
                ammoUI.Add(new Sprite(20, 42, new Vector2(bossAmmoLocation.X + 262, bossAmmoLocation.Y + 0), false));
                ammoUI.Add(new Sprite(20, 49, new Vector2(bossAmmoLocation.X + 326, bossAmmoLocation.Y + 0), false));
                ammoUI.Add(new Sprite(20, 56, new Vector2(bossAmmoLocation.X + 390, bossAmmoLocation.Y + 0), false));
            }
            else
            {
                ammoUI.Add(new Sprite(20, 0, new Vector2(ammoUILocation.X + 6, ammoUILocation.Y + 0), false));
                ammoUI.Add(new Sprite(20, 21, new Vector2(ammoUILocation.X + 70, ammoUILocation.Y + 0), false));
                ammoUI.Add(new Sprite(20, 28, new Vector2(ammoUILocation.X + 134, ammoUILocation.Y + 0), false));
                ammoUI.Add(new Sprite(20, 35, new Vector2(ammoUILocation.X + 198, ammoUILocation.Y + 0), false));
                ammoUI.Add(new Sprite(20, 42, new Vector2(ammoUILocation.X + 262, ammoUILocation.Y + 0), false));
                ammoUI.Add(new Sprite(20, 49, new Vector2(ammoUILocation.X + 326, ammoUILocation.Y + 0), false));
                ammoUI.Add(new Sprite(20, 56, new Vector2(ammoUILocation.X + 390, ammoUILocation.Y + 0), false));
            }

#if XBOX    
            if (!Guide.IsTrialMode)
#endif
            {
                if (GameSettings.LocksOn)
                {
                    int count = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (LevelDataManager.levelData[1, 6 + i].IsBronze()) count++;
                    }
                    if (count < 2)
                    {
                        ammoUI[1].IsVisible = false;
                        ammoUI[1].TintColor = Color.Gray;
                    }
                }
                if (!LevelDataManager.levelData[2, 1].unlocked && GameSettings.LocksOn)
                {
                    ammoUI[2].IsVisible = false;
                    ammoUI[2].TintColor = Color.Gray;
                }
                if (GameSettings.LocksOn)
                {
                    int count = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        if (LevelDataManager.levelData[2, 6 + i].IsBronze()) count++;
                    }
                    if (count < 2)
                    {
                        ammoUI[3].IsVisible = false;
                        ammoUI[3].TintColor = Color.Gray;
                    }
                }
                if (!LevelDataManager.levelData[3, 1].unlocked && GameSettings.LocksOn)
                {
                    ammoUI[4].IsVisible = false;
                    ammoUI[4].TintColor = Color.Gray;
                }
                if (!LevelDataManager.levelData[4, 1].unlocked && GameSettings.LocksOn)
                {
                    ammoUI[5].IsVisible = false;
                    ammoUI[5].TintColor = Color.Gray;
                }
                if (!LevelDataManager.levelData[5, 1].unlocked && GameSettings.LocksOn)
                {
                    ammoUI[6].IsVisible = false;
                    ammoUI[6].TintColor = Color.Gray;
                }
            }

            foreach (Sprite sprite in ammoUI)
            {
                sprite.AnimationFPS = animationSpeed;
                sprite.CurrentFrame = LevelDataManager.rand.Next(0, 3);
                sprite.HitPoints = 0;
            }
            #endregion

            if (LevelDataManager.levelData[LevelDataManager.world, LevelDataManager.level].starCollected) powerStarCollected = true;

        }

        void contactListener_AddPowerBarrel(object sender, EffectEventArgs e)
        {
            PowerUpSprites.Add(e.spriteA);
        }
        void contactListener_LemonActivated(object sender, EffectEventArgs e)
        {
            shot.IsHit = true;
        }
        void contactListener_BananaActivated(object sender, EffectEventArgs e)
        {
            shot.pathing = Sprite.Pathing.Round;
            shot.PathingRadiusX = 768;
            shot.PathingRadiusY = 192;
            shot.PathingSpeed = 100;
            if (shot.spriteBody.LinearVelocity.X < 0) shot.PathingSpeed *= -1; //enable backwards banana
            shot.spriteBody.ResetDynamics();
            shot.spriteBody.LinearVelocity = ConvertUnits.ToSimUnits(new Vector2(10f, 10f)); //so it isnt motionless in physics and shot ends
            shot.spriteBody.AngularVelocity = 30f;
            shot.spriteBody.IgnoreGravity = false;
            shot.InitializePathing();
        }
        void contactListener_DamageStar(object sender, DamageEventArgs e)
        {
            if (powerStar == e.sprite)
            {
                powerStar.IsExpired = true;
                if (powerStar.spriteBody != null) physicsWorld.RemoveBody(powerStar.spriteBody);
            }
        }
        void contactListener_StarCollected(object sender, EffectEventArgs e)
        {
            if (powerStar == e.spriteA)
            {
                powerStar.spriteBody.Enabled = false;
                powerStar.IsHit = true;
                powerStarCollected = true;
            }
        }
        void contactListener_PowerUpActivated(object sender, EffectEventArgs e)
        {
            //sprite a is shot, sprite b is barrel
            if (e.spriteB == ActivePowerUpBarrel) return; //cant go in barrel shot from
            if (e.spriteA.TextureID == 20 && e.spriteA.TextureIndex > 62) return; //fruitshot already split, cancel contact        

            //disables physical collision while in barrel(from impact damage and allows physical pass through)
            e.spriteA.spriteBody.IsSensor = true;
            //enable camera to follow until cameara movement pushed
            Camera.IsScrolling = true;
            appleCopterTimer = 0;
            isAppleCopter = false;

            //turns off activated ability behavior
            if (e.spriteA.pathing != Sprite.Pathing.None)
            {
                e.spriteA.pathing = Sprite.Pathing.None;
                e.spriteA.spriteBody.IgnoreGravity = false;
            }

            ActivePowerUpBarrel = e.spriteB;
            if (e.spriteB.TextureIndex == 6) ShotStartBarrel = e.spriteB;

            //teleport barrel
            if (e.spriteB.TextureIndex == 5)
            {
                //find destination
                int teleportDestination = -1;
                for (int i = 0; i < PowerUpSprites.Count; i++)
                {
                    if (PowerUpSprites[i].HitPoints == ActivePowerUpBarrel.HitPoints && PowerUpSprites[i] != ActivePowerUpBarrel)
                    {
                        teleportDestination = i;
                        ActivePowerUpBarrel = PowerUpSprites[i];
                        break;
                    }
                }

                //move the shot 
                Vector2 teleportLocation = ActivePowerUpBarrel.spriteBody.Position;
                shot.spriteBody.SetTransformIgnoreContacts(ref teleportLocation, 0);
            }

            shot.TotalRotation = 0;
            return;
        }
        void contactListener_DamageShot(object sender, DamageEventArgs e)
        {
            if (GameSettings.CheatInvincibility) e.damage = 0;

            //turns off activated ability behavior
            #region pathing ability behavior on damage
            if (e.sprite.pathing != Sprite.Pathing.None)
            {

                if (e.sprite.TextureIndex >= 42 && e.sprite.TextureIndex < 49) 
                {
                    #region Banana pathing off conditions and carry through ability
                    //turn banana pathing off on these collisions
                    if (e.damageType == Sprite.Type.Spring || e.damageType == Sprite.Type.Terrain || e.damageType == Sprite.Type.StaticBlock ||
                        e.damageType == Sprite.Type.PowerUp || e.damageType == Sprite.Type.Smasher || e.damageType == Sprite.Type.Switch)
                    {
                        e.sprite.pathing = Sprite.Pathing.None;
                        if (e.sprite.spriteBody != null) e.sprite.spriteBody.IgnoreGravity = false;
                    }
                    else
                    {
                        //turn off banana if its not on its return arc
                        //if ( (shot.PathingSpeed > 0 && shot.pathingTravelled < MathHelper.ToRadians(0)) || 
                        //     (shot.PathingSpeed < 0 && shot.pathingTravelled > MathHelper.ToRadians(-180)) )
                        //{
                        //    e.sprite.pathing = Sprite.Pathing.None;
                        //    if (e.sprite.spriteBody != null) e.sprite.spriteBody.IgnoreGravity = false;
                        //}
                        //else
                        {
                            //e.sprite.PathingSpeed = 90;
                            //e.sprite.spriteBody.AngularVelocity = 30f;
                            if (LevelDataManager.rand.Next(0,3) != 0) e.sprite.HitPoints += 1;
                        }
                    }
                    #endregion
                }
                else
                {
                    e.sprite.pathing = Sprite.Pathing.None;
                    appleCopterTimer = 0;
                    isAppleCopter = false;
                    if (e.sprite.spriteBody != null) e.sprite.spriteBody.IgnoreGravity = false;
                }

                #region strawberry/orange force impact
                if (e.sprite.SpriteType == Sprite.Type.FruitShot)
                {
                    if (e.sprite.TextureIndex >= 28 && e.sprite.TextureIndex < 35) //strawberry in any frame
                    {
                        Explosion explosion = new Explosion(physicsWorld);
                        Vector2 explosionLocation = e.sprite.spriteBody.Position;
                        float explosionRadius = ConvertUnits.ToSimUnits(GameSettings.ExplosiveForceRadiusMultiplier * e.sprite.Scale * 0.5f);
                        float explosionForce = GameSettings.ExplosivePower * 0.5f * e.sprite.Scale;
                        explosion.Activate(explosionLocation + ConvertUnits.ToSimUnits(new Vector2(32,48)), explosionRadius, explosionForce);
                        explosion.Activate(explosionLocation + ConvertUnits.ToSimUnits(new Vector2(-32,48)), explosionRadius, explosionForce);
                        e.sprite.spriteBody.ResetDynamics();
                        e.sprite.spriteBody.Rotation = 0;
                        contactListener.DoImpactPoofs(e.sprite.Location + new Vector2(0, 32), e.sprite);
                        e.sprite.IsHit = true;
                    }
                    if (e.sprite.TextureIndex >= 21 && e.sprite.TextureIndex < 28) //orange in any frame
                    {
                        Explosion explosion = new Explosion(physicsWorld);
                        Vector2 explosionLocation = e.sprite.spriteBody.Position;
                        float explosionRadius = ConvertUnits.ToSimUnits(GameSettings.ExplosiveForceRadiusMultiplier * e.sprite.Scale);
                        float explosionForce = GameSettings.ExplosivePower * 0.5f * e.sprite.Scale;
                        explosion.Activate(explosionLocation, explosionRadius, explosionForce);
                        e.sprite.spriteBody.ResetDynamics();
                        e.sprite.spriteBody.Rotation = 0;
                        contactListener.DoImpactPoofs(e.sprite.Location, e.sprite);
                        e.sprite.IsHit = true;
                    }
                }
                #endregion
            }
            #endregion

            if (e.damage > 0)
            {
                if (shot.SpriteType == Sprite.Type.FruitShot || shot.SpriteType == Sprite.Type.VeggieShot) Scream(e.sprite);
                if (shot.SpriteType == Sprite.Type.ExplosiveShot) e.sprite.IsHit = true;
                e.sprite.HitPoints -= e.damage;
                if (e.sprite.HitPoints <= 0) e.sprite.IsHit = true; 
            }
        }


        public void Update(GameTime gameTime)
        {

            #region Update Ammo UI
            //update fruit in ammo UI
            for (int i = ammoUI.Count - 1; i >= 0; i--)
            {
                UpdateAnimation(ammoUI[i], gameTime);
                UpdateSelectedAmmo(selectedAmmo, gameTime);
            }
            #endregion

            #region Update shot
            shot.Update(gameTime);
            if (shot != null && !shot.IsExpired)
            {
                #region terminal velocity and fixed vel fire/ice/lit
                //terminal on shot to avoid being blown off screen at incredible velocities
                //if (shot.spriteBody != null)
                //{
                //    if (shot.spriteBody.LinearVelocity.Length() > shotTerminalVelocity)
                //    {
                //        Vector2 newVel = shot.spriteBody.LinearVelocity;
                //        newVel.Normalize();
                //        newVel *= shotTerminalVelocity;
                //        shot.spriteBody.LinearVelocity = newVel;
                //    }
                //}

                //fixed velocity on fire ice lit shot (avoid explosion effects)
                if (shot.SpriteType == Sprite.Type.FireballShot || shot.SpriteType == Sprite.Type.LightningShot || shot.SpriteType == Sprite.Type.IceShot) shot.spriteBody.LinearVelocity = fireicelitFixedVelocity;
                #endregion

                if (shot.pathing != Sprite.Pathing.None)
                {
                    if (shot.TextureIndex >= 21) contactListener.CreateFruitMotionBlur(shot);

                    #region Banana boomerang, turn off if arc done
                    if (shot.TextureIndex >= 42 && shot.TextureIndex < 49) 
                    {
                        if (shot.PathingSpeed > 0 && shot.pathingTravelled > MathHelper.ToRadians(120))
                        {
                            shot.pathing = Sprite.Pathing.None;
                            if (shot.spriteBody != null) shot.spriteBody.IgnoreGravity = false;
                        }
                        if (shot.PathingSpeed < 0 && shot.pathingTravelled < MathHelper.ToRadians(-300))
                        {
                            shot.pathing = Sprite.Pathing.None;
                            if (shot.spriteBody != null) shot.spriteBody.IgnoreGravity = false;
                        }
                    }
                    #endregion

                    #region applecopter active
                    if (shot.TextureIndex < 21)
                    {
                        appleCopterTimer += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
                        if (appleCopterTimer > appleCopterTimerTick)
                        {
                            SoundManager.Play(SoundManager.Sound.AppleSpin, false, false);
                            appleCopterTimer -= appleCopterTimerTick;
                        }
                    }
                    #endregion
                }

                UpdateAnimation(shot, gameTime);

                #region SHOT IS HIT - TRIGGER EVENT, REMOVE SHOT
                if (shot.IsHit)
                {
                    switch (shot.SpriteType)
                    {
                        case Sprite.Type.FruitShot:
                            {
                                contactListener.ExplodeFruit(shot);
                                break;
                            }
                        case Sprite.Type.VeggieShot:
                            {
                                contactListener.ExplodeVeggie(shot);
                                break;
                            }
                        case Sprite.Type.ExplosiveShot:
                            {
                                contactListener.ExplodeBomb(shot);
                                break;
                            }
                        case Sprite.Type.SawShot:
                        case Sprite.Type.CannonballShot:
                            {
                                contactListener.DoPoof(shot);
                                break;
                            }
                        case Sprite.Type.FireballShot:
                            {
                                contactListener.ExplodeFireShot(shot);
                                break;
                            }
                        case Sprite.Type.IceShot:
                            {
                                contactListener.ExplodeIceShot(shot);
                                break;
                            }
                        case Sprite.Type.LightningShot:
                            {
                                contactListener.ExplodeLitShot(shot);
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                    shot.IsExpired = true;
                    if (shot.spriteBody != null) physicsWorld.RemoveBody(shot.spriteBody);
                    shot.spriteBody = BodyFactory.CreateCircle(physicsWorld, ConvertUnits.ToSimUnits(32f), 1f, ShotStartBarrel.spriteBody.Position,shot);
                    shot.spriteBody.Enabled = false;
                    shot.spriteBody.BodyType = BodyType.Dynamic;
                    shot.TextureID = 20;
                    shot.TextureIndex = 0;
                    shot.CurrentFrame = 0;
                    shot.IsEffect = false;
                    shot.pathing = Sprite.Pathing.None;
                    shot.SpriteType = Sprite.Type.FruitShot;
                    appleCopterTimer = 0;
                    isAppleCopter = false;
                    
                }
                #endregion
            }
            #endregion

            #region update split shot
            if (isSplit)
            {
                for (int i = 0; i < 3; i++)
                {
                    if (splitShot[i].IsVisible && splitShot[i] != null && !splitShot[i].IsExpired)
                    {
                        splitShot[i].Update(gameTime);

                        #region Terminal velocity
                        //terminal on shot to avoid being blown off screen at incredible velocities
                        //if (splitShot[i].spriteBody != null)
                        //{
                        //    if (splitShot[i].spriteBody.LinearVelocity.Length() > shotTerminalVelocity)
                        //    {
                        //        Vector2 newVel = splitShot[i].spriteBody.LinearVelocity;
                        //        newVel.Normalize();
                        //        newVel *= shotTerminalVelocity;
                        //        splitShot[i].spriteBody.LinearVelocity = newVel;
                        //    }
                        //}
                        #endregion

                        UpdateAnimation(splitShot[i], gameTime);
                        if (splitShot[i].IsHit)
                        {
                            contactListener.ExplodeFruit(splitShot[i]);
                            contactListener.ExplodeBomb(splitShot[i]);
                            splitShot[i].IsExpired = true;
                            physicsWorld.RemoveBody(splitShot[i].spriteBody);
                        }
                    }
                }

                if (shot.TextureIndex == 35 && (splitShot[0].IsExpired) && (splitShot[1].IsExpired) && (splitShot[2].IsExpired))
                {
                    splitShot[0].IsExpired = false;
                    shot.IsExpired = true; //if the splitshot is dead, this will trigger motionless shot expiration
                }
                else //split shot != dead, update avg position for camera tracking
                {
                    if (shot.TextureIndex == 35 && (splitShot[0].IsVisible || splitShot[1].IsVisible || splitShot[2].IsVisible))
                    {
                        shot.Location = new Vector2((splitShot[0].Location.X + splitShot[1].Location.X + splitShot[2].Location.X) / 3f,
                                                          (splitShot[0].Location.Y + splitShot[1].Location.Y + splitShot[2].Location.Y) / 3f);
                    }
                }
            }
            #endregion

            #region STAR UPDATE
            if (powerStar != null && powerStar.IsExpired != true)
            {
                powerStar.Update(gameTime);
                if (powerStar.IsHit)
                {
                    float alpha = powerStar.TintColor.A;
                    alpha = (alpha - (510f* (float)gameTime.ElapsedGameTime.TotalSeconds))/255f;
                    powerStar.TintColor = Color.White * alpha;
                    if (alpha <= 0)
                    {
                        alpha = 0;
                        powerStar.IsExpired = true;
                        if (powerStar.spriteBody != null) physicsWorld.RemoveBody(powerStar.spriteBody);
                    }
                }
            }
            #endregion

            #region Update Power Barrels
            if (PowerUpSprites.Count > 0)
            {
                for (int i = PowerUpSprites.Count - 1; i >= 0; i--)
                {
                    PowerUpSprites[i].Update(gameTime);

                    if (PowerUpSprites[i].IsHit)
                    {
                        PowerUpSprites[i].IsHit = false;
                        PowerUpSprites[i].IsCollidable = false;
                        if (PowerUpSprites[i].spriteBody != null) physicsWorld.RemoveBody(PowerUpSprites[i].spriteBody);
                        contactListener.DoPoof(PowerUpSprites[i]);
                        PowerUpSprites[i].IsExpired = true;
                    }
                }
            }
            #endregion


            return;
        }

        //pulses the selected fruit on the shot UI
        public void UpdateSelectedAmmo(int _selectedAmmo, GameTime gameTime)
        {
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (ammoUI[_selectedAmmo].HitPoints == 0)
            {
                ammoUI[_selectedAmmo].Scale += elapsedTime*0.05f;
                if (ammoUI[_selectedAmmo].Scale > 1.15f) ammoUI[_selectedAmmo].HitPoints = 1;
                return;
            }
            if (ammoUI[_selectedAmmo].HitPoints == 1)
            {
                ammoUI[_selectedAmmo].Scale -= elapsedTime*0.05f;
                if (ammoUI[_selectedAmmo].Scale < 1.05f) ammoUI[_selectedAmmo].HitPoints = 0;
                return;
            }
          
        }

        //changes the fruit selected for next shot 
        public void ChangeSelectedAmmo(int direction)
        {
            ammoUI[selectedAmmo].Scale = 1f;
            if (direction == -1)
            {
                if (selectedAmmo == 0) selectedAmmo = 6;
                else selectedAmmo -= 1;
            }
            if (direction == 1)
            {
                if (selectedAmmo == 6) selectedAmmo = 0;
                else selectedAmmo += 1;
            }
            if (ammoUI[selectedAmmo].TintColor.B != 255 && GameSettings.LocksOn) ChangeSelectedAmmo(direction);
            return;
        }

        //changes powerup on the start barrel
        public void ChangePowerBarrel(int direction)
        {
            //if (!GameSettings.CheatBarrelOn) return;

            if (direction == -1)
            {
                if (ShotStartBarrel.TextureIndex == 1) ShotStartBarrel.TextureIndex = 9;
                else ShotStartBarrel.TextureIndex -= 1;
            }
            if (direction == 1)
            {
                if (ShotStartBarrel.TextureIndex == 9) ShotStartBarrel.TextureIndex = 1;
                else ShotStartBarrel.TextureIndex += 1;
            }
            if (ShotStartBarrel.TextureIndex == 5) ShotStartBarrel.TextureIndex += direction; //skip teleport barrel in order
            return;
        }


        public void UpdateAnimation(Sprite sprite, GameTime gameTime)
        {
            if (sprite.SpriteType == Sprite.Type.ExplosiveShot || sprite.SpriteType == Sprite.Type.CannonballShot) return;

            if (sprite.SpriteType == Sprite.Type.SawShot)
            {
                sprite.TotalRotation += 0.27f;
                sprite.spriteBody.SetTransform(ConvertUnits.ToSimUnits(sprite.SpriteCenterInWorld), sprite.TotalRotation);
                return;
            }

            #region ANIMATE LITSHOT
            if (sprite.SpriteType == Sprite.Type.LightningShot)
            {
                sprite.AnimationFramePrecise += (float)gameTime.ElapsedGameTime.TotalSeconds * sprite.AnimationFPS;
                if (sprite.AnimationFramePrecise >= 1.0f)
                {
                    sprite.AnimationFramePrecise -= 1.0f;
                    sprite.CurrentFrame += 1;
                    if (sprite.CurrentFrame == 10) sprite.CurrentFrame = 0; //loop
                }
            }
            #endregion

            #region ANIMATE Fireshot
            if (sprite.SpriteType == Sprite.Type.FireballShot)
            {
                contactListener.CreateEmber(sprite);
                contactListener.CreateEmber(sprite);
                sprite.AnimationFramePrecise += (float)gameTime.ElapsedGameTime.TotalSeconds * sprite.AnimationFPS;
                if (sprite.AnimationFramePrecise >= 1.0f)
                {
                    contactListener.DoPoof(sprite); //fireballSmoke experiment
                    sprite.AnimationFramePrecise -= 1.0f;
                    sprite.CurrentFrame += 1;
                    if (sprite.CurrentFrame == 6) sprite.CurrentFrame = 0; //loop   
                }
            }
            #endregion

            #region FRUIT AND VEGGIE ANIMATION CODE
            if (sprite.SpriteType == Sprite.Type.FruitShot || sprite.SpriteType == Sprite.Type.VeggieShot) //fruit or veggie animation block
            {
                sprite.AnimationFramePrecise += (float)gameTime.ElapsedGameTime.TotalSeconds * sprite.AnimationFPS;
                //process frame change if enough time has elapsed
                if (sprite.AnimationFramePrecise >= 1.0f && sprite.IsAnimationDirectionForward)
                {
                    sprite.spriteBody.Awake = true;
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
                    sprite.spriteBody.Awake = true;
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

        public void CreateFruitShot(Vector2 firingLocation, World physicsWorld)
        {
            appleCopterTimer = 0;
            isAppleCopter = false;
            shot.TextureID = 20;
            shot.TextureIndex = 0;
            shot.CurrentFrame = 0;
            shot.IsEffect = false;
            shot.SpriteType = Sprite.Type.FruitShot;
            if (selectedAmmo == 0) shot.TextureIndex = LevelDataManager.rand.Next(0, 3) * 7; //gets random color of apple (sets 0,7,14 texture index on fruit sheet) if its an apple
            if (selectedAmmo == 1) shot.TextureIndex = 21;
            if (selectedAmmo == 2) shot.TextureIndex = 28;
            if (selectedAmmo == 3) shot.TextureIndex = 35;
            if (selectedAmmo == 4) shot.TextureIndex = 42;
            if (selectedAmmo == 5) shot.TextureIndex = 49;
            if (selectedAmmo == 6) shot.TextureIndex = 56;

            shot.TotalRotation = 0;
            shot.SpriteRectangle = new Rectangle ((int)firingLocation.X - 32, (int)firingLocation.Y -32, 64, 64);
            shot.IsExpired = false;
            shot.IsCollidable = true;
            shot.IsAwake = true;
            shot.IsVisible = true;
            shot.IsHit = false;
            shot.TintColor = new Color(255f,255f,255f,255f);
            shot.Scale = 1.0f;
            shot.IsAnimated = false;
            shot.AnimationFPS = animationSpeed;
            shot.CurrentFrame = LevelDataManager.rand.Next(0, 3);
            shot.pathing = Sprite.Pathing.None;
            shot.InitPathingPoint();
            shot.pathingTravelled = 0f;

            //create new body
            physicsWorld.RemoveBody(shot.spriteBody);
            Vector2 position = ConvertUnits.ToSimUnits(shot.Location + shot.SpriteOrigin);
            int spriteRow = shot.TextureIndex / LevelDataManager.SpritesInRow(shot);
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
                        shot.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, shot);
                        break;
                    }
                case 3:
                    {
                        //orange
                        shot.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits(30f), ConvertUnits.ToSimUnits(30f), 10, 1.0f, position, shot);
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
                        shot.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, shot);
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
                        shot.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, shot);
                        splitShot[0].IsExpired = false;
                        splitShot[1].IsExpired = false;
                        splitShot[2].IsExpired = false;
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
                        shot.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, shot);
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
                        shot.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 1.0f, position, shot);
                        break;
                    }
                case 8:
                    {
                        //watermelon
                        shot.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits(30f), ConvertUnits.ToSimUnits(30f), 10, 1.0f, position, shot);
                        break;
                    }
                default:
                    break;
            }
            shot.spriteBody.Position = position;
            shot.spriteBody.ResetDynamics();
            shot.spriteBody.Mass = GameSettings.FruitMass;
            shot.spriteBody.Rotation = shot.TotalRotation;
            shot.spriteBody.BodyType = BodyType.Dynamic;
            shot.spriteBody.IgnoreGravity = false;
            shot.spriteBody.IsBullet = true;
            shot.spriteBody.Restitution = GameSettings.FruitBouncy;
            shot.spriteBody.Friction = GameSettings.FruitFriction;
            shot.HitPoints = GameSettings.FruitHP;

            //watermelon adjustment
            if (spriteRow == 8) shot.spriteBody.Mass *= 3;
            if (spriteRow == 8) shot.HitPoints *= 3;

            //if the shot is not an apple, use the fruit up (if it was a star,relay,or grow barrel)
            if (selectedAmmo != 0)
            {
                if (ActivePowerUpBarrel.TextureIndex == 0 || ActivePowerUpBarrel.TextureIndex == 6 || ActivePowerUpBarrel.TextureIndex == 7)
                {
                    if (!GameSettings.CheatLegion) //except cheat
                    {
                        ammoUI[selectedAmmo].TintColor = Color.Gray;
                        ammoUI[selectedAmmo].Scale = 1f;
                        selectedAmmo = 0;
                    }
                }
            }
            return;
        }

        public void CreateSplitShot(World physics, Layer playLayer)
        {
            for (int i = 0; i < 3; i++)
            {
                splitShot[i].IsExpired = false;
                splitShot[i].IsVisible = true;
                splitShot[i].IsHit = false;
                splitShot[i].IsAwake = true;
                splitShot[i].IsCollidable = true;
                splitShot[i].Scale = shot.Scale;
                splitShot[i].SpriteType = Sprite.Type.FruitShot;
                splitShot[i].TintColor = new Color(255f, 255f, 255f, 255f);
                Vector2 position = ConvertUnits.ToSimUnits(shot.Location + shot.SpriteOrigin);
                if (i == 0) position = new Vector2(position.X - ConvertUnits.ToSimUnits(24f), position.Y);
                if (i == 2) position = new Vector2(position.X + ConvertUnits.ToSimUnits(24f), position.Y);
                splitShot[i].spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits(16f), ConvertUnits.ToSimUnits(16f), 6, 1.0f, position, splitShot[i]);
                splitShot[i].spriteBody.BodyType = BodyType.Dynamic;
                splitShot[i].spriteBody.Mass = GameSettings.FruitMass/3f;
                splitShot[i].Location = ConvertUnits.ToDisplayUnits( splitShot[i].spriteBody.Position);
                splitShot[i].spriteBody.Rotation = shot.spriteBody.Rotation;
                splitShot[i].spriteBody.LinearVelocity = shot.spriteBody.LinearVelocity;
                if (i == 0) splitShot[i].spriteBody.LinearVelocity = new Vector2(splitShot[i].spriteBody.LinearVelocity.X - ConvertUnits.ToSimUnits(100f), splitShot[i].spriteBody.LinearVelocity.Y);
                if (i == 2) splitShot[i].spriteBody.LinearVelocity = new Vector2(splitShot[i].spriteBody.LinearVelocity.X + ConvertUnits.ToSimUnits(100f), splitShot[i].spriteBody.LinearVelocity.Y);
                splitShot[i].spriteBody.AngularVelocity = shot.spriteBody.AngularVelocity+LevelDataManager.rand.Next(-10,11);
                splitShot[i].spriteBody.IgnoreGravity = false;
                splitShot[i].spriteBody.IsBullet = true;
                splitShot[i].spriteBody.Restitution = GameSettings.FruitBouncy;
                splitShot[i].spriteBody.Friction = GameSettings.FruitFriction;
                splitShot[i].HitPoints = (GameSettings.FruitHP/3)+1;
                playLayer.AddSpriteToLayer(splitShot[i]);
            }

            shot.spriteBody.ResetDynamics();
            shot.spriteBody.Enabled = false;
            shot.IsVisible = false;
            shot.spriteBody.Position = ConvertUnits.ToSimUnits(new Vector2 (-100f,-100f));
            shot.spriteBody.IgnoreGravity = true;
            shot.IsCollidable = false;
            isSplit = true;
            return;
        }

        public void CreateStar(Sprite sprite, World physicsWorld)
        {
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            sprite.IsAnimated = true;
            sprite.IsAnimatedWhileStopped = true;
            sprite.IsBounceAnimated = false;
            sprite.AnimationFPS = 8f;
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            sprite.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth / 2.0f), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight / 2.0f), 10, 0.9f, position, sprite);
            sprite.spriteBody.Rotation = 0;
            sprite.spriteBody.BodyType = BodyType.Kinematic;
            sprite.spriteBody.IsSensor = true;
            sprite.spriteBody.IgnoreGravity = true;
            powerStar = sprite;
            return;
        }

        public void CreateBarrel(Sprite sprite, World physicsWorld)
        {
            PowerUpSprites.Add(sprite);
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(32f), ConvertUnits.ToSimUnits(48f), 0.9f, position, sprite);
            sprite.spriteBody.Rotation = sprite.TotalRotation;
            sprite.spriteBody.BodyType = BodyType.Kinematic;
            sprite.spriteBody.IsSensor = true;
            return;
        }

        public void CreatePowerUpShot(Sprite _shot, Vector2 firingLocation, World physicsWorld, Vector2 linearVelocity)
        {
            appleCopterTimer = 0;
            isAppleCopter = false;
            _shot.spriteBody.IsSensor = false; //makes physical body live to damage leaving barrel

            switch (ActivePowerUpBarrel.TextureIndex)
            {
                //relay barrel
                case 0:
                    {
                        _shot.TotalRotation = ActivePowerUpBarrel.TotalRotation;
                        _shot.Location = new Vector2((int)firingLocation.X - _shot.SpriteRectWidth / 2, (int)firingLocation.Y - _shot.SpriteRectHeight / 2);
                        _shot.spriteBody.Position = ConvertUnits.ToSimUnits(_shot.SpriteCenterInWorld);
                        _shot.spriteBody.Rotation = _shot.TotalRotation;
                        _shot.spriteBody.IgnoreGravity = false;
                        _shot.IsVisible = true;
                        break;
                    }
                //fire barrel
                case 1:
                    {
                        _shot.TextureID = 10;
                        _shot.TextureIndex = 0;
                        _shot.SpriteType = Sprite.Type.FireballShot;
                        _shot.SpriteRectangle = new Rectangle((int)firingLocation.X - 126, (int)firingLocation.Y - 126, 252, 252);
                        _shot.IsEffect = true;
                        _shot.TotalRotation = ActivePowerUpBarrel.TotalRotation + MathHelper.ToRadians(-90);
                        _shot.IsExpired = false;
                        _shot.IsCollidable = true;
                        _shot.HitPoints = 1;
                        _shot.IsAwake = true;
                        _shot.IsVisible = true;
                        _shot.IsHit = false;
                        _shot.TintColor = new Color(255f, 255f, 255f, 255f);
                        _shot.Scale = 1.0f;
                        _shot.IsAnimated = false;
                        _shot.AnimationFPS = fireicelitanimationspeed;
                        _shot.AnimationFramePrecise = 0.0f;
                        _shot.CurrentFrame = 0;
                        _shot.pathing = Sprite.Pathing.None;
                        _shot.InitPathingPoint();
                        _shot.pathingTravelled = 0.001f;

                        physicsWorld.RemoveBody(_shot.spriteBody);
                        Vector2 position = ConvertUnits.ToSimUnits(_shot.SpriteCenterInWorld);
                        _shot.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits(32f), ConvertUnits.ToSimUnits(32f), 8, 1.0f, position, _shot);

                        _shot.spriteBody.ResetDynamics();
                        _shot.spriteBody.Mass = 0.15f;
                        _shot.spriteBody.Rotation = _shot.TotalRotation;
                        _shot.spriteBody.BodyType = BodyType.Dynamic;
                        _shot.spriteBody.IgnoreGravity = true;
                        _shot.spriteBody.IsBullet = true;
                        _shot.spriteBody.IsSensor = true;
                        _shot.spriteBody.Restitution = 0.5f;
                        _shot.spriteBody.Friction = 0.5f;

                        //poof barrel if its not the starting barrel
                        if (ActivePowerUpBarrel.HitPoints >= 0 && ActivePowerUpBarrel != ShotStartBarrel) ActivePowerUpBarrel.IsHit = true;

                        break;
                    }
                //ice barrel
                case 2:
                    {
                        _shot.TextureID = 5;
                        _shot.TextureIndex = 0;
                        _shot.SpriteType = Sprite.Type.IceShot;
                        _shot.SpriteRectangle = new Rectangle((int)firingLocation.X - 126, (int)firingLocation.Y - 126, 252, 252);
                        _shot.IsEffect = true;
                        _shot.TotalRotation = ActivePowerUpBarrel.TotalRotation + MathHelper.ToRadians(-90);
                        _shot.IsExpired = false;
                        _shot.IsCollidable = true;
                        _shot.HitPoints = 1;
                        _shot.IsAwake = true;
                        _shot.IsVisible = true;
                        _shot.IsHit = false;
                        _shot.TintColor = new Color(255f, 255f, 255f, 255f);
                        _shot.Scale = 1.0f;
                        _shot.IsAnimated = true;
                        _shot.IsAnimatedWhileStopped = true;
                        _shot.IsBounceAnimated = false;
                        _shot.IsAnimationDirectionForward = true;
                        _shot.AnimationFPS = fireicelitanimationspeed;
                        _shot.AnimationFramePrecise = 0.0f;
                        _shot.CurrentFrame = 0;
                        _shot.pathing = Sprite.Pathing.None;
                        _shot.InitPathingPoint();
                        _shot.pathingTravelled = 0.001f;

                        physicsWorld.RemoveBody(_shot.spriteBody);
                        Vector2 position = ConvertUnits.ToSimUnits(_shot.SpriteCenterInWorld);
                        _shot.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits(32f), ConvertUnits.ToSimUnits(32f), 8, 1.0f, position, _shot);

                        _shot.spriteBody.ResetDynamics();
                        _shot.spriteBody.Mass = 0.15f;
                        _shot.spriteBody.Rotation = _shot.TotalRotation;
                        _shot.spriteBody.BodyType = BodyType.Dynamic;
                        _shot.spriteBody.IgnoreGravity = true;
                        _shot.spriteBody.IsBullet = true;
                        _shot.spriteBody.IsSensor = true;
                        _shot.spriteBody.Restitution = 0.5f;
                        _shot.spriteBody.Friction = 0.5f;

                        //poof barrel if its not the starting barrel
                        if (ActivePowerUpBarrel.HitPoints >= 0 && ActivePowerUpBarrel != ShotStartBarrel) ActivePowerUpBarrel.IsHit = true;
                        break;
                    }
                //lightning barrel
                case 3:
                    {
                        _shot.TextureID = 11; //lightning effect
                        _shot.TextureIndex = 0;
                        _shot.SpriteType = Sprite.Type.LightningShot;
                        _shot.SpriteRectangle = new Rectangle((int)firingLocation.X - 252, (int)firingLocation.Y - 252, 504, 504);
                        _shot.IsEffect = true;
                        _shot.TotalRotation = ActivePowerUpBarrel.TotalRotation + MathHelper.ToRadians(-90);
                        _shot.IsExpired = false;
                        _shot.IsCollidable = true;
                        _shot.HitPoints = 1;
                        _shot.IsAwake = true;
                        _shot.IsVisible = true;
                        _shot.IsHit = false;
                        _shot.TintColor = new Color(255f, 255f, 255f, 255f);
                        _shot.Scale = 1.0f;
                        _shot.IsAnimated = false;
                        _shot.AnimationFPS = fireicelitanimationspeed;
                        _shot.AnimationFramePrecise = 0.0f;
                        _shot.CurrentFrame = 0;
                        _shot.pathing = Sprite.Pathing.None;
                        _shot.InitPathingPoint();
                        _shot.pathingTravelled = 0.001f;

                        physicsWorld.RemoveBody(_shot.spriteBody);
                        Vector2 position = ConvertUnits.ToSimUnits(_shot.SpriteCenterInWorld);
                        _shot.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits(32f), ConvertUnits.ToSimUnits(32f), 8, 1.0f, position, _shot);

                        _shot.spriteBody.ResetDynamics();
                        _shot.spriteBody.Mass = 0.15f;
                        _shot.spriteBody.Rotation = _shot.TotalRotation;
                        _shot.spriteBody.BodyType = BodyType.Dynamic;
                        _shot.spriteBody.IgnoreGravity = true;
                        _shot.spriteBody.IsBullet = true;
                        _shot.spriteBody.IsSensor = true;
                        _shot.spriteBody.Restitution = 0.5f;
                        _shot.spriteBody.Friction = 0.5f;

                        //poof barrel if its not the starting barrel
                        if (ActivePowerUpBarrel.HitPoints >= 0 && ActivePowerUpBarrel != ShotStartBarrel) ActivePowerUpBarrel.IsHit = true;
                        break;
                    }
                //tnt barrel
                case 4:
                    {
                        _shot.TextureID = 49; //tnt
                        _shot.TextureIndex = 0;
                        _shot.IsEffect = false;
                        _shot.HitPoints = 10;
                        _shot.IsAnimated = false;
                        _shot.AnimationFramePrecise = 0.0f;
                        _shot.CurrentFrame = 0;
                        _shot.IsCollidable = true;
                        _shot.IsAwake = true;
                        _shot.IsVisible = true;
                        _shot.SpriteType = Sprite.Type.ExplosiveShot;
                        _shot.TotalRotation = ActivePowerUpBarrel.TotalRotation;
                        _shot.SpriteRectangle = new Rectangle((int)firingLocation.X - 32, (int)firingLocation.Y - 32, 64, 64);
                        _shot.spriteBody.Position = ConvertUnits.ToSimUnits(_shot.SpriteCenterInWorld);
                        physicsWorld.RemoveBody(_shot.spriteBody);
                        Vector2 position = ConvertUnits.ToSimUnits(_shot.SpriteCenterInWorld);
                        _shot.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(64f), ConvertUnits.ToSimUnits(64f), GameSettings.WoodDensity, position, _shot);
                        _shot.spriteBody.Rotation = _shot.TotalRotation;
                        _shot.spriteBody.IgnoreGravity = false;
                        _shot.spriteBody.BodyType = BodyType.Dynamic;
                        _shot.spriteBody.IsBullet = true;
                        _shot.spriteBody.IsSensor = false;
                        _shot.spriteBody.Mass = _shot.spriteBody.Mass * GameSettings.WoodDensity;
                        _shot.spriteBody.Restitution = GameSettings.WoodRestitution;
                        _shot.spriteBody.Friction = GameSettings.WoodFriction;

                        //poof barrel if its not the starting barrel
                        if (ActivePowerUpBarrel.HitPoints >= 0 && ActivePowerUpBarrel != ShotStartBarrel) ActivePowerUpBarrel.IsHit = true;
                        break;
                    }
                //teleport barrel //NOt imlimented
                case 5:
                    {
                        ActivePowerUpBarrel.TotalRotation = _shot.TotalRotation;
                        _shot.Location = new Vector2((int)firingLocation.X - _shot.SpriteRectWidth / 2, (int)firingLocation.Y - _shot.SpriteRectHeight / 2);
                        _shot.spriteBody.Position = ConvertUnits.ToSimUnits(_shot.SpriteCenterInWorld);
                        _shot.spriteBody.Rotation = _shot.TotalRotation;
                        _shot.spriteBody.IgnoreGravity = false;
                        _shot.IsVisible = true;
                        break;
                    }
                //star barrel
                case 6:
                    {
                        _shot.TotalRotation = ActivePowerUpBarrel.TotalRotation;
                        _shot.Location = new Vector2((int)firingLocation.X - _shot.SpriteRectWidth / 2, (int)firingLocation.Y - _shot.SpriteRectHeight / 2);
                        _shot.spriteBody.Position = ConvertUnits.ToSimUnits(_shot.SpriteCenterInWorld);
                        _shot.spriteBody.Rotation = _shot.TotalRotation;
                        _shot.spriteBody.IgnoreGravity = false;
                        _shot.IsVisible = true;
                        break;
                    }
                //grow barrel
                case 7:
                    {
                        _shot.TotalRotation = ActivePowerUpBarrel.TotalRotation;
                        _shot.SpriteRectangle = new Rectangle((int)firingLocation.X - 32, (int)firingLocation.Y - 32, 64, 64);
                        _shot.spriteBody.IgnoreGravity = false;
                        _shot.IsVisible = true;
                        _shot.Scale = 1.5f;
                        _shot.HitPoints = GameSettings.FruitHP * 5;
                        _shot.spriteBody.Mass = _shot.spriteBody.Mass * 4f;
                        _shot.spriteBody.ResetDynamics();
                        
                        _shot.spriteBody.Rotation = _shot.TotalRotation;
                        _shot.spriteBody.BodyType = BodyType.Dynamic;
                        _shot.spriteBody.IgnoreGravity = false;
                        _shot.spriteBody.IsBullet = true;
                        _shot.spriteBody.IsSensor = false;
                        _shot.spriteBody.Restitution = 0.5f;
                        _shot.spriteBody.Friction = 0.5f;

                        //poof barrel if its not the starting barrel
                        if (ActivePowerUpBarrel.HitPoints >= 0 && ActivePowerUpBarrel != ShotStartBarrel) ActivePowerUpBarrel.IsHit = true;
                        break;
                    }
                //cannonball barrel
                case 8:
                    {
                        _shot.TextureID = 30; //4x4 sphere
                        _shot.TextureIndex = 4;
                        _shot.IsEffect = false;
                        _shot.HitPoints = 10000;
                        _shot.IsAnimated = false;
                        _shot.AnimationFramePrecise = 0.0f;
                        _shot.CurrentFrame = 0;
                        _shot.IsCollidable = true;
                        _shot.IsAwake = true;
                        _shot.IsVisible = true;
                        _shot.SpriteType = Sprite.Type.CannonballShot;
                        _shot.TotalRotation = ActivePowerUpBarrel.TotalRotation;
                        _shot.SpriteRectangle = new Rectangle((int)firingLocation.X - 32, (int)firingLocation.Y - 32, 64, 64);
                        _shot.spriteBody.Position = ConvertUnits.ToSimUnits(_shot.SpriteCenterInWorld);
                        physicsWorld.RemoveBody(_shot.spriteBody);
                        Vector2 position = ConvertUnits.ToSimUnits(_shot.SpriteCenterInWorld);
                        _shot.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits(32f), ConvertUnits.ToSimUnits(32f), 12, 1.0f, position, _shot);
                        _shot.spriteBody.Rotation = _shot.TotalRotation;
                        _shot.spriteBody.IgnoreGravity = false;
                        _shot.spriteBody.BodyType = BodyType.Dynamic;
                        _shot.spriteBody.IsBullet = true;
                        _shot.spriteBody.IsSensor = false;
                        _shot.spriteBody.Mass = _shot.spriteBody.Mass * GameSettings.MetalDensity;
                        _shot.spriteBody.Restitution = GameSettings.MetalRestitution;
                        _shot.spriteBody.Friction = GameSettings.MetalFriction;

                        //poof barrel if its not the starting barrel
                        if (ActivePowerUpBarrel.HitPoints >= 0 && ActivePowerUpBarrel != ShotStartBarrel) ActivePowerUpBarrel.IsHit = true;
                        break;
                    }
                //sawshot barrel
                case 9:
                    {
                        _shot.TextureID = 47; //64x64 saw
                        _shot.TextureIndex = 0;
                        _shot.IsEffect = false;
                        _shot.HitPoints = 10000;
                        _shot.IsAnimated = false;
                        _shot.AnimationFramePrecise = 0.0f;
                        _shot.CurrentFrame = 0;
                        _shot.IsCollidable = true;
                        _shot.IsAwake = true;
                        _shot.IsVisible = true;
                        _shot.SpriteType = Sprite.Type.SawShot;
                        _shot.TotalRotation = ActivePowerUpBarrel.TotalRotation; 
                        _shot.SpriteRectangle = new Rectangle((int)firingLocation.X - 32, (int)firingLocation.Y - 32, 64, 64);
                        _shot.spriteBody.Position = ConvertUnits.ToSimUnits(_shot.SpriteCenterInWorld);
                        physicsWorld.RemoveBody(_shot.spriteBody);
                        Vector2 position = ConvertUnits.ToSimUnits(_shot.SpriteCenterInWorld);
                        _shot.spriteBody = BodyFactory.CreateEllipse(physicsWorld, ConvertUnits.ToSimUnits(32f), ConvertUnits.ToSimUnits(32f), 12, GameSettings.MetalDensity, position, _shot);
                        _shot.spriteBody.Rotation = _shot.TotalRotation;
                        _shot.spriteBody.IgnoreGravity = true;
                        _shot.spriteBody.BodyType = BodyType.Dynamic;
                        _shot.spriteBody.IsBullet = true;
                        _shot.spriteBody.IsSensor = true;
                        _shot.spriteBody.Restitution = GameSettings.MetalRestitution;
                        _shot.spriteBody.Friction = GameSettings.MetalFriction;

                        //poof barrel if its not the starting barrel
                        if (ActivePowerUpBarrel.HitPoints >= 0 && ActivePowerUpBarrel != ShotStartBarrel) ActivePowerUpBarrel.IsHit = true;
                        break;
                    }
                default:
                    break;
            }

            if (_shot.SpriteType == Sprite.Type.FireballShot || _shot.SpriteType == Sprite.Type.LightningShot || _shot.SpriteType == Sprite.Type.IceShot) fireicelitFixedVelocity = linearVelocity;

            return;
        }

    }
}
