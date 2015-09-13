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
    public class TerrainManager
    {

        private List<Sprite> TerrainSprites;
        private World physicsWorld;

        public TerrainManager(World _world)
        {
            TerrainSprites = new List<Sprite>();
            physicsWorld = _world;
        }

        public void Update(GameTime gameTime)
        {
            if (TerrainSprites.Count > 0)
            {
                for (int i = TerrainSprites.Count - 1; i >= 0; i--)
                {
                    TerrainSprites[i].Update(gameTime);
                }
            }
            return;
        }

        public int SpriteCount
        {
            get { return TerrainSprites.Count; }
        }

        public void CreateTerrain(Sprite sprite, World physicsWorld)
        {
            TerrainSprites.Add(sprite);
            sprite.IsCollidable = true;
            sprite.HitPoints = int.MaxValue;
            if (sprite.pathing == Sprite.Pathing.None) sprite.IsAwake = false;  //no need to call update on terrain unless it has pathing
            else sprite.IsAwake = true;

            switch (sprite.TextureIndex)
            {
                case 14:
                    {
                        Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
                        Vertices vertices = new Vertices(3);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-sprite.SpriteRectWidth / 2.0f, sprite.SpriteRectHeight / 2.0f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(sprite.SpriteRectWidth / 2.0f, -sprite.SpriteRectHeight / 2.0f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(sprite.SpriteRectWidth / 2.0f, sprite.SpriteRectHeight / 2.0f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 100.0f, position,sprite);
                        sprite.spriteBody.Rotation = sprite.TotalRotation;
                        SetGroundPhysics(sprite.spriteBody);
                        break;
                    }
                case 15:
                    {
                        Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
                        Vertices vertices = new Vertices(3);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-sprite.SpriteRectWidth / 2.0f, sprite.SpriteRectHeight / 2.0f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-sprite.SpriteRectWidth / 2.0f, -sprite.SpriteRectHeight / 2.0f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(sprite.SpriteRectWidth / 2.0f, sprite.SpriteRectHeight / 2.0f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 100.0f, position,sprite);
                        sprite.spriteBody.Rotation = sprite.TotalRotation;
                        SetGroundPhysics(sprite.spriteBody);
                        break;
                    }
                case 16:
                    {
                        Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
                        Vertices vertices = new Vertices(3);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-sprite.SpriteRectWidth / 2.0f, -sprite.SpriteRectHeight / 2.0f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(sprite.SpriteRectWidth / 2.0f, -sprite.SpriteRectHeight / 2.0f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(sprite.SpriteRectWidth / 2.0f, sprite.SpriteRectHeight / 2.0f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 100.0f, position,sprite);
                        sprite.spriteBody.Rotation = sprite.TotalRotation;
                        SetGroundPhysics(sprite.spriteBody);
                        break;
                    }
                case 17:
                    {
                        Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
                        Vertices vertices = new Vertices(3);
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-sprite.SpriteRectWidth / 2.0f, -sprite.SpriteRectHeight / 2.0f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(sprite.SpriteRectWidth / 2.0f, -sprite.SpriteRectHeight / 2.0f)));
                        vertices.Add(ConvertUnits.ToSimUnits(new Vector2(-sprite.SpriteRectWidth / 2.0f, sprite.SpriteRectHeight / 2.0f)));
                        sprite.spriteBody = BodyFactory.CreatePolygon(physicsWorld, vertices, 100.0f, position,sprite);
                        sprite.spriteBody.Rotation = sprite.TotalRotation;
                        SetGroundPhysics(sprite.spriteBody);
                        break;
                    }
               
                //all other blocks on TERRAIN type sheet are solid
                default:
                    {
                        Vector2 position = ConvertUnits.ToSimUnits(sprite.Location + sprite.SpriteOrigin);
                        sprite.spriteBody = BodyFactory.CreateRectangle(physicsWorld, ConvertUnits.ToSimUnits(sprite.SpriteRectWidth), ConvertUnits.ToSimUnits(sprite.SpriteRectHeight), 1.0f, position, sprite);
                        sprite.spriteBody.Rotation = sprite.TotalRotation;
                        SetGroundPhysics(sprite.spriteBody);
                        break;
                    }
            }
            return;
        }

        private void SetGroundPhysics(Body ground)
        {
            ground.BodyType = BodyType.Static;
            ground.IgnoreGravity = true;
            ground.Restitution = 0.0f;
            ground.Friction = 0.8f;
            return;
        }
    }
}
