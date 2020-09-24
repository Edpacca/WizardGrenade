﻿using Microsoft.Xna.Framework;
using System;

namespace WizardGrenade
{
    public class Mechanics
    {
        public const float GRAVITY = 9.8f;

        public static Vector2 VectorComponents(float magnitude, float angle)
        {
            return new Vector2((float)Math.Sin(angle) * magnitude, (float)Math.Cos(angle) * magnitude);
        }

        public static float VectorMagnitude(Vector2 vector)
        {
            return (float)Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2));
        }

        public static Vector2 NormaliseVector(Vector2 vector)
        {
            return vector / (VectorMagnitude(vector));
        }

        public static float DotProduct(Vector2 vector1, Vector2 vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y;
        }

        public static float VectorAngle(Vector2 vector1, Vector2 vector2)
        {
            float v1Mag = VectorMagnitude(vector1);
            float v2Mag = VectorMagnitude(vector2);

            return (float)Math.Acos(DotProduct(vector1, vector2) / (v1Mag * v2Mag));
        }

        public static Vector2 ReflectionVector(Vector2 incident, Vector2 normal)
        {
            return incident + (-2 * normal * (DotProduct(incident, normal)) / (float)Math.Pow(VectorMagnitude(normal), 2));
        }

        public static Vector2 RelativeProjectilePosition(Vector2 vectorComponents, GameTime gameTime, TimeSpan startTime, float mass)
        {
            float deltaX = vectorComponents.X * (float)(gameTime.TotalGameTime.TotalSeconds - startTime.TotalSeconds);
            float deltaY = (vectorComponents.Y * (float)(gameTime.TotalGameTime.TotalSeconds - startTime.TotalSeconds)
                + (GRAVITY * mass / 2 * (float)Math.Pow((float)(gameTime.TotalGameTime.TotalSeconds - startTime.TotalSeconds), 2)));

            return new Vector2(deltaX, deltaY);
        }
    }
}
