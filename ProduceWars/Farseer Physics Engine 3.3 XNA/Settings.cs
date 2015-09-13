﻿/*
* Farseer Physics Engine based on Box2D.XNA port:
* Copyright (c) 2010 Ian Qvist
* 
* Box2D.XNA port of Box2D:
* Copyright (c) 2009 Brandon Furtwangler, Nathan Furtwangler
*
* Original source Box2D:
* Copyright (c) 2006-2009 Erin Catto http://www.gphysics.com 
* 
* This software is provided 'as-is', without any express or implied 
* warranty.  In no event will the authors be held liable for any damages 
* arising from the use of this software. 
* Permission is granted to anyone to use this software for any purpose, 
* including commercial applications, and to alter it and redistribute it 
* freely, subject to the following restrictions: 
* 1. The origin of this software must not be misrepresented; you must not 
* claim that you wrote the original software. If you use this software 
* in a product, an acknowledgment in the product documentation would be 
* appreciated but is not required. 
* 2. Altered source versions must be plainly marked as such, and must not be 
* misrepresented as being the original software. 
* 3. This notice may not be removed or altered from any source distribution. 
*/

using System;

namespace FarseerPhysics
{
    public static class Settings
    {
        public const float MaxFloat = 3.402823466e+38f;
        public const float Epsilon = 1.192092896e-07f;
        public const float Pi = 3.14159265359f;

        // Common

        /// <summary>
        /// Enabling diagnistics causes the engine to gather timing information.
        /// You can see how much time it took to solve the contacts, solve CCD
        /// and update the controllers.
        /// NOTE: If you are using a debug view that shows performance counters,
        /// you might want to enable this.
        /// </summary>
        public static bool EnableDiagnostics = false;

        /// <summary>
        /// The number of velocity iterations used in the solver.
        /// </summary>
        public static int VelocityIterations = 6;

        /// <summary>
        /// The number of position iterations used in the solver.
        /// </summary>
        public static int PositionIterations = 6;

        /// <summary>
        /// Enable/Disable Continuous Collision Detection (CCD)
        /// </summary>
        public static bool ContinuousPhysics = false;

        /// <summary>
        /// The number of velocity iterations in the TOI solver
        /// </summary>
        public static int TOIVelocityIterations = 1;

        /// <summary>
        /// The number of position iterations in the TOI solver
        /// </summary>
        public static int TOIPositionIterations = 1;

        /// <summary>
        /// Maximum number of sub-steps per contact in continuous physics simulation.
        /// </summary>
        public const int MaxSubSteps = 1;

        /// <summary>
        /// Enable/Disable warmstarting
        /// </summary>
        public static bool EnableWarmstarting = true;

        /// <summary>
        /// Enable/Disable sleeping
        /// </summary>
        public static bool AllowSleep = true;

        /// <summary>
        /// The maximum number of vertices on a convex polygon.
        /// </summary>
        public static int MaxPolygonVertices = 12;

        /// <summary>
        /// Farseer Physics Engine has a different way of filtering fixtures than Box2d.
        /// We have both FPE and Box2D filtering in the engine. If you are upgrading
        /// from earlier versions of FPE, set this to true.
        /// </summary>
        public static bool UseFPECollisionCategories=false;

        /// <summary>
        /// Conserve memory makes sure that objects are used by reference instead of cloned.
        /// When you give a vertices collection to a PolygonShape, it will by default copy the vertices
        /// instead of using the original reference. This is to ensure that objects modified outside the engine
        /// does not affect the engine itself, however, this uses extra memory. This behavior
        /// can be turned off by setting ConserveMemory to true.
        /// </summary>
        public const bool ConserveMemory = true;

        /// <summary>
        /// The maximum number of contact points between two convex shapes.
        /// </summary>
        public const int MaxManifoldPoints = 2;

        /// <summary>
        /// This is used to fatten AABBs in the dynamic tree. This allows proxies
        /// to move by a small amount without triggering a tree adjustment.
        /// This is in meters.
        /// </summary>
        public const float AABBExtension = 0.2f;

        /// <summary>
        /// This is used to fatten AABBs in the dynamic tree. This is used to predict
        /// the future position based on the current displacement.
        /// This is a dimensionless multiplier.
        /// </summary>
        public const float AABBMultiplier = 2.0f;

        /// <summary>
        /// A small length used as a collision and constraint tolerance. Usually it is
        /// chosen to be numerically significant, but visually insignificant.
        /// </summary>
        public const float LinearSlop = 0.001f;

        /// <summary>
        /// A small angle used as a collision and constraint tolerance. Usually it is
        /// chosen to be numerically significant, but visually insignificant.
        /// </summary>
        public const float AngularSlop = (0.001f * Pi);

        /// <summary>
        /// The radius of the polygon/edge shape skin. This should not be modified. Making
        /// this smaller means polygons will have an insufficient buffer for continuous collision.
        /// Making it larger may create artifacts for vertex collision.
        /// </summary>
        public const float PolygonRadius = (2.0f * LinearSlop);

        // Dynamics

        /// <summary>
        /// Maximum number of contacts to be handled to solve a TOI impact.
        /// </summary>
        public const int MaxTOIContacts = 8;

        /// <summary>
        /// A velocity threshold for elastic collisions. Any collision with a relative linear
        /// velocity below this threshold will be treated as inelastic.
        /// </summary>
        public const float VelocityThreshold = 0.5f;

        /// <summary>
        /// The maximum linear position correction used when solving constraints. This helps to
        /// prevent overshoot.
        /// </summary>
        public const float MaxLinearCorrection = 0.1f;

        /// <summary>
        /// The maximum angular position correction used when solving constraints. This helps to
        /// prevent overshoot.
        /// </summary>
        public const float MaxAngularCorrection = (0.01f * Pi);

        /// <summary>
        /// This scale factor controls how fast overlap is resolved. Ideally this would be 1 so
        /// that overlap is removed in one time step. However using values close to 1 often lead
        /// to overshoot.
        /// </summary>
        public const float ContactBaumgarte = 0.1f;

        // Sleep

        /// <summary>
        /// The time that a body must be still before it will go to sleep.
        /// </summary>
        public const float TimeToSleep = 0.06f;

        /// <summary>
        /// A body cannot sleep if its linear velocity is above this tolerance.
        /// </summary>
        public const float LinearSleepTolerance = 0.1f;

        /// <summary>
        /// A body cannot sleep if its angular velocity is above this tolerance.
        /// </summary>
        public const float AngularSleepTolerance = (0.01f * Pi);

        /// <summary>
        /// The maximum linear velocity of a body. This limit is very large and is used
        /// to prevent numerical problems. You shouldn't need to adjust this.
        /// </summary>
        //public const float MaxTranslation = 2.0f;
        public const float MaxTranslation = 0.5f;

        public const float MaxTranslationSquared = (MaxTranslation * MaxTranslation);

        /// <summary>
        /// The maximum angular velocity of a body. This limit is very large and is used
        /// to prevent numerical problems. You shouldn't need to adjust this.
        /// </summary>
        //public const float MaxRotation = (0.5f * Pi);
        public const float MaxRotation = (0.1f * Pi);

        public const float MaxRotationSquared = (MaxRotation * MaxRotation);

        /// <summary>
        /// Friction mixing law. Feel free to customize this.
        /// </summary>
        /// <param name="friction1">The friction1.</param>
        /// <param name="friction2">The friction2.</param>
        /// <returns></returns>
        public static float MixFriction(float friction1, float friction2)
        {
            return (float) Math.Sqrt(friction1 * friction2);
        }

        /// <summary>
        /// Restitution mixing law. Feel free to customize this.
        /// </summary>
        /// <param name="restitution1">The restitution1.</param>
        /// <param name="restitution2">The restitution2.</param>
        /// <returns></returns>
        public static float MixRestitution(float restitution1, float restitution2)
        {
            if (restitution1 == 1f || restitution2 == 1f) return 1f;
            else return (restitution1 + restitution2) / 2f;
        }
    }
}