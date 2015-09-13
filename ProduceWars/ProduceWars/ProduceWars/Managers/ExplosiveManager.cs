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
using FarseerPhysics.Common.PhysicsLogic;

namespace ProduceWars.Managers
{
    public class ExplosiveManager
    {
        private List<Sprite> ExplosiveSprites;
        private World physicsWorld;
        private ContactListener contactListener;


        public ExplosiveManager(World _world, ContactListener _contactListener)
        {
            ExplosiveSprites = new List<Sprite>();
            physicsWorld = _world;
            contactListener = _contactListener;
            contactListener.DamageExplosive += new ContactListener.DamageEventHandler(contactListener_DamageExplosive);
        }

        void contactListener_DamageExplosive(object sender, DamageEventArgs e)
        {
            if (e.damageType == Sprite.Type.LightningShot || e.damageType == Sprite.Type.FireballShot || e.damageType == Sprite.Type.CannonballShot ||
                e.damageType == Sprite.Type.VeggieShot || e.damageType == Sprite.Type.FruitShot) e.damage += 10;
            if (e.damage > 14) e.sprite.HitPoints = -1;
            else e.sprite.HitPoints -= (e.damage*e.damage);

            if (e.sprite.HitPoints <= 0) e.sprite.IsHit = true;
        }

        public void Update(GameTime gameTime)
        {
            if (ExplosiveSprites.Count > 0)
            {
                for (int i = ExplosiveSprites.Count - 1; i >= 0; i--)
                {
                    ExplosiveSprites[i].Update(gameTime);
                    if (ExplosiveSprites[i].IsHit)
                    {
                        contactListener.ExplodeBomb(ExplosiveSprites[i]);
                        ExplosiveSprites[i].IsExpired = true;
                        physicsWorld.RemoveBody(ExplosiveSprites[i].spriteBody);
                        ExplosiveSprites.Remove(ExplosiveSprites[i]);
                    }
                }
            }
            return;
        }

        public int SpriteCount
        {
            get { return ExplosiveSprites.Count; }
        }

        public void CreateExplosive(Sprite sprite, World physicsWorld)
        {
            ExplosiveSprites.Add(sprite);
            sprite.SpriteType = Sprite.Type.Explosive;
            sprite.HitPoints = sprite.SpriteRectWidth*sprite.SpriteRectHeight;
            sprite.IsCollidable = true;
            sprite.IsAwake = true;
            Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
            sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), GameSettings.WoodDensity, position, sprite);
            sprite.spriteBody.BodyType = BodyType.Dynamic;
            sprite.spriteBody.Restitution = GameSettings.WoodRestitution;
            sprite.spriteBody.Friction = GameSettings.WoodFriction;
            return;
        }

    }

}