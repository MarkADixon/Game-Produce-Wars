using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParallaxEngine
{
    public static class Camera
    {
        #region Camera Declarations

        private static Vector2 position = Vector2.Zero;
        private static Vector2 targetPosition = Vector2.Zero;
        private static Vector2 lastPosition = Vector2.Zero;
        public static bool IsScrolling = false;
        public static bool AutoScroll = false;
        private static int scrollDelay = 0; //in ms
        private static Vector2 viewportSize = Vector2.Zero;
        private static Rectangle worldRectangle = new Rectangle(0, 0, 0, 0);
        private static Rectangle worldRectangleBuffered = new Rectangle(0, 0, 0, 0);
        private const int WorldEdgeBuffer = 200;
        private static Rectangle screenRectangle;
        private static Rectangle? cameraPositionLimits;
        public static float zoom = 1.0f;
        private static float targetZoom = 1.0f;
        private static float rotation = 0.0f;
        public static int gameTimeMS;
        private static Matrix viewMatrix = new Matrix();
        public static bool viewMatrixCalculated = false;
        private static Vector2 lastParallax = Vector2.Zero;


        #endregion

        #region GAME SCORING
        private static int score = 0;
        private static int scoreCombo = 1;
        private static int scoreComboResetTimer = 9999; //in ms
        private const int scoreComboResetThreshold = 2500; //in ms

        public static int Score
        {
            get { return score; }
            set { score = value; }
        }

        public static int ScoreCombo
        {
            get { return scoreCombo; }
            set { scoreCombo = value; }
        }

        public static void AddScore(int value)
        {
            score = score + (int)(value * scoreCombo * 0.1f);
            scoreComboResetTimer = 0;
            return;
        }

        public static void AddScoreCombo()
        {
            scoreCombo += 1;
            scoreComboResetTimer = 0;
            return;
        }

        public static bool ScoreTimedOut()
        {
            return (scoreComboResetTimer > scoreComboResetThreshold);
        }

        #endregion

        public static void Update(GameTime gameTime)
        {
            gameTimeMS = (int)(gameTime.ElapsedGameTime.TotalMilliseconds);
            ValidateZoom();
            ValidatePosition();

            if (targetZoom != zoom) SmoothZoom();
            //if (targetPosition != position)
            //{
                if (AutoScroll)
                {
                    if (scrollDelay <= 0)
                    {
                        SmoothScroll();
                    }
                    else
                    {
                        scrollDelay -= gameTimeMS;
                    }
                }
                //else targetPosition = position;
            //}
            //else AutoScroll = false;


            //score update
            if(!ScoreTimedOut()) scoreComboResetTimer += gameTimeMS;
            if (scoreCombo != 1)
            {
                if (ScoreTimedOut())
                {
                    scoreCombo = 1;
                    scoreComboResetTimer = 9999;
                }
            }

            return;
        }


        #region Camera Properties

        public static Vector2 Position
        {
            get { return position; }
            set
            {
                //if (cameraPositionLimits == null && Rotation == 0.0f)
               // {
                //    position = new Vector2(
                //        MathHelper.Clamp(value.X, worldRectangle.X, worldRectangle.Width - ViewportWidth),
                //        MathHelper.Clamp(value.Y, worldRectangle.Y, worldRectangle.Height - ViewportHeight));
                //}
                //
                //if (cameraPositionLimits != null && Rotation == 0.0f)
                //{
                //    position = new Vector2(
                //        MathHelper.Clamp(value.X, cameraPositionLimits.Value.X, cameraPositionLimits.Value.X + cameraPositionLimits.Value.Width - Viewport.Width),
                //        MathHelper.Clamp(value.Y, cameraPositionLimits.Value.Y, cameraPositionLimits.Value.Y + cameraPositionLimits.Value.Height - Viewport.Height));
               // }
                position = value;
                ValidatePosition();
            }
        }

        //property gets and sets the rectangle representing the limits of the camera position, allows binding of the camera to only portions of the map at a time if needed
        public static Rectangle? CameraPositionLimits
        {
            get
            {
                return cameraPositionLimits;
            }
            set
            {
                //if (value != null)
                //{
                //    // Assign limit but make sure it's always bigger than the viewport
                //    cameraPositionLimits = new Rectangle
                //    {
                //        //without zoom
                //        X = value.Value.X,
                //        Y = value.Value.Y,
                //        Width = System.Math.Max(Viewport.Width, value.Value.Width),
                //        Height = System.Math.Max(Viewport.Height, value.Value.Height)
                //
                //    };

                    // Validate camera position with new limit
                //    Position = Position;
                //}
                //else
                //{
                //    cameraPositionLimits = null;
                //}

                cameraPositionLimits = value;
                ValidateZoom();
                ValidatePosition();

            }
        }

        public static Rectangle WorldRectangle
        {
            get { return worldRectangle; }
            set 
            {
                ResetZoom();
                worldRectangle = value;
                CameraPositionLimits = worldRectangle;
                worldRectangleBuffered = new Rectangle(worldRectangle.X - WorldEdgeBuffer, worldRectangle.Y - WorldEdgeBuffer,
                                                        worldRectangle.Width + (WorldEdgeBuffer * 2), worldRectangle.Height + (WorldEdgeBuffer * 2)); 
            }
        }

        public static Rectangle WorldRectangleBuffered
        {
            get { return worldRectangleBuffered; }
        }

        public static int ViewportWidth
        {
            get { return (int)viewportSize.X; }
            set { viewportSize.X = (int)value; }
        }

        public static int ViewportHeight
        {
            get { return (int)viewportSize.Y; }
            set { viewportSize.Y = (int)value; }
        }

        public static Rectangle Viewport
        {
            get { return new Rectangle((int)Position.X, (int)Position.Y, ViewportWidth, ViewportHeight); }
        }

        public static void SetScreenRectangle()
        {
            screenRectangle = new Rectangle(0, 0, ViewportWidth, ViewportHeight);
        }

        public static Vector2 Origin 
        {
            get { return new Vector2((Viewport.Width * 0.5f), (Viewport.Height *0.5f)); }    
        }

        public static void ZoomTo(float value)
        {
            if (value > 0.1f) targetZoom = value;
            ValidateZoom();
            ValidatePosition();
        }

        public static void ResetZoom()
        {
            zoom = 1;
            targetZoom = 1;
            rotation = 0;
            ValidateZoom();
            ValidatePosition();
        }

        private static void SmoothZoom()
        {
            if (Math.Abs(zoom - targetZoom) <= 0.005f) zoom = targetZoom;
            else zoom = zoom + ((targetZoom - zoom) / 20.0f);

            ValidateZoom();
            ValidatePosition();
            return;
        }


       public static float Rotation 
       {
            get { return rotation; }
            set
            {
                rotation = value; 
            }
       }

        #endregion

        #region Public Camera Methods

        public static Matrix GetViewMatrix(Vector2 parallax)
        {
            //optimization, create way to cache the matrix and not recalculate constantly
            if (parallax != lastParallax) viewMatrixCalculated = false;
            lastParallax = parallax;

            if (parallax.X == 1.0f)
            {
                if (!viewMatrixCalculated)
                {
                    viewMatrix = Matrix.CreateTranslation(new Vector3(-Position * parallax, 1.0f)) *
                       Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                       Matrix.CreateRotationZ(Rotation) *
                       Matrix.CreateScale(zoom, zoom, 1.0f) *
                       Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
                    viewMatrixCalculated = true;
                }
                return viewMatrix;
            }
            if (parallax.X < 1f)
            {
                if (!viewMatrixCalculated)
                {
                    viewMatrix = Matrix.CreateTranslation(new Vector3(-Position * parallax, 1.0f)) *
                       Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                       Matrix.CreateRotationZ(Rotation) *
                       Matrix.CreateScale(((zoom-1.0f) * parallax.X) + 1.0f, ((zoom-1.0f) * parallax.Y) + 1.0f, 1.0f) *
                       Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
                    viewMatrixCalculated = true;
                }
                return viewMatrix;
            }
            if (parallax.X > 1f)
            {
                if (!viewMatrixCalculated)
                {
                    viewMatrix = Matrix.CreateTranslation(new Vector3(-Position * parallax, 1.0f)) *
                       Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                       Matrix.CreateRotationZ(Rotation) *
                       //Matrix.CreateScale(((zoom-1.0f) * parallax.X) + 1.0f, ((zoom-1.0f) * parallax.Y) + 1.0f, 1.0f) *
                       Matrix.CreateScale(Vector3.One) *
                      Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
                    viewMatrixCalculated = true;
                }
                return viewMatrix;
            }

            return viewMatrix;
        }

        public static Matrix GetDebugMatrix(Vector2 parallax)
        {
            // To add parallax, simply multiply it by the position
            return Matrix.CreateTranslation(new Vector3(-Position * parallax, 0.0f)) *
                   Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(zoom, zoom, 1) *
                   Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        private static void ValidateZoom()
        {
            if (cameraPositionLimits.HasValue)
            {
                Camera.viewMatrixCalculated = false;
                float minZoomX = (float)Viewport.Width / cameraPositionLimits.Value.Width;
                float minZoomY = (float)Viewport.Height / cameraPositionLimits.Value.Height;
                zoom = MathHelper.Max(zoom, MathHelper.Max(minZoomX, minZoomY));
            }
        }

        private static void ValidatePosition()
        {
            if (cameraPositionLimits.HasValue)
            {
                Camera.viewMatrixCalculated = false;
                Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(GetViewMatrix(new Vector2(1.0f,1.0f))));
                Vector2 cameraSize = new Vector2(Viewport.Width, Viewport.Height) / zoom;
                Vector2 limitWorldMin = new Vector2(cameraPositionLimits.Value.Left, cameraPositionLimits.Value.Top);
                Vector2 limitWorldMax = new Vector2(cameraPositionLimits.Value.Right, cameraPositionLimits.Value.Bottom);
                Vector2 positionOffset = position - cameraWorldMin;
                position = Vector2.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize) + positionOffset;
            }
        }

        private static Vector2 ValidateTargetPosition(Vector2 _position)
        {
            if (cameraPositionLimits.HasValue)
            {
                Vector2 cameraSize = new Vector2(Viewport.Width, Viewport.Height) / zoom;
                Vector2 limitWorldMin = new Vector2(cameraPositionLimits.Value.Left, cameraPositionLimits.Value.Top);
                Vector2 limitWorldMax = new Vector2(cameraPositionLimits.Value.Right, cameraPositionLimits.Value.Bottom);
                _position = Vector2.Clamp(_position, limitWorldMin, limitWorldMax - cameraSize);
            }
            return _position;
        }

        public static void Move(Vector2 displacement)
        {
            Position += displacement;
        }

        public static void Move(Vector2 displacement, bool respectRotation)
        {
            if (respectRotation)
            {
                displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(-Rotation));
            }

            Position += displacement;
            targetPosition = position;
        }

        public static void LookAt(Vector2 position)
        {
            Position = position - Origin;
            targetPosition = position;
        }

        public static void ScrollTo(Vector2 _position, int delay)
        {
            //ResetZoom();
            targetPosition = _position - Origin;
            targetPosition = ValidateTargetPosition(targetPosition);
            AutoScroll = true;
            scrollDelay = delay;
        }

        public static void changeTarget(Vector2 newTargetPos)
        {
            targetPosition = newTargetPos - Origin;
            targetPosition = ValidateTargetPosition(targetPosition);
        }

       public static void SmoothScroll()
       { 
            Vector2 dpos = new Vector2((targetPosition.X - position.X),(targetPosition.Y - position.Y));
            Position = new Vector2( position.X + MathHelper.Clamp(dpos.X *0.06f, -20,20),
                                    position.Y + MathHelper.Clamp(dpos.Y *0.06f, -20,20));
            if (dpos.Length() < 2)
            {
                Position = targetPosition;
                AutoScroll = false;
            }
        }
        
        public static bool IsObjectVisible(Rectangle bounds, Vector2 parallax)
        {
            return (screenRectangle.Intersects(WorldToScreen(bounds,parallax)));
        }

        public static Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, GetViewMatrix(new Vector2 (1.0f,1.0f)));
        }
        
        public static Vector2 WorldToScreen(Vector2 worldPosition, Vector2 parallax)
        {
            return Vector2.Transform(worldPosition, GetViewMatrix(parallax));
        }

        public static Rectangle WorldToScreen(Rectangle worldPosition, Vector2 parallax)
        {
            Vector2 transformedVector = Vector2.Transform(new Vector2 (worldPosition.X,worldPosition.Y), GetViewMatrix(parallax));
            return new Rectangle((int)transformedVector.X, (int)transformedVector.Y, worldPosition.Width, worldPosition.Height);
        }

        public static Rectangle WorldToScreen(Rectangle worldPosition)
        {
            Vector2 transformedVector = Vector2.Transform(new Vector2(worldPosition.X, worldPosition.Y), GetViewMatrix(new Vector2 (1.0f,1.0f)));
            return new Rectangle((int)transformedVector.X, (int)transformedVector.Y, worldPosition.Width, worldPosition.Height);
        }

        public static Vector2 ScreenToWorld(Vector2 screenPosition, Vector2 parallax)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(GetViewMatrix(parallax)));
        }

        #endregion







    }
}
