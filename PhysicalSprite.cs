﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace WizardGrenade
{
    class PhysicalSprite
    {
        private Texture2D _spriteTexture;
        private Texture2D _pixelTexture;
        public float layerDepth = 0.0f;
        public SpriteEffects spriteEffect = SpriteEffects.None;
        
        public Vector2 relativeOrigin { get; set; }
        public Rectangle size { get; set; }
        public Vector2 position;
        public float rotation = 0.0f;
        public Vector2 velocity = new Vector2(0, 0);
        public Vector2 acceleration = new Vector2(0, 0);
        public bool isStable;

        private Vector2 _rotationOffset = Vector2.Zero;
        private float _mass;
        private float _radius;
        private float _offsetRadius;
        private float _friction;
        public List<Vector2> _collisionPoints = new List<Vector2>();
        private bool _canRotate;

        public PhysicalSprite(Vector2 initialPosition, float mass, float friction, bool canRotate)
        {
            position.X = initialPosition.X;
            position.Y = initialPosition.Y;
            _mass = mass;
            _canRotate = canRotate;
            _friction = friction;
        }

        public void LoadContent (ContentManager contentManager, string fileName)
        {
            _pixelTexture = contentManager.Load<Texture2D>("pixel");
            _spriteTexture = contentManager.Load<Texture2D>(fileName);
            size = new Rectangle(0, 0, _spriteTexture.Width, _spriteTexture.Height);
            relativeOrigin = new Vector2(_spriteTexture.Width / 2, _spriteTexture.Height / 2);
            _radius = Math.Max(_spriteTexture.Width, _spriteTexture.Height) / 2;
            _offsetRadius = (float)Math.Sqrt(2 * (_radius * _radius));
            _collisionPoints = Collision.CalcCircleCollisionPoints(_radius, 2, Vector2.Zero);
        }

        public virtual void Update(GameTime gameTime)
        {
            acceleration.Y += Physics.GRAVITY * _mass;
            
            velocity.X += acceleration.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity.Y += acceleration.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_canRotate)
                rotation = (float)(Math.Atan2(velocity.Y, velocity.X));
            if (rotation > 2 * Math.PI)
                rotation -= (float)(2 * Math.PI);
            if (rotation < 0)
                rotation += (float)(2 * Math.PI);

            float potentialX = position.X + velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
            float potentialY = position.Y + velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

            acceleration.X = 0;
            acceleration.Y = 0;
            isStable = false;

            position.X = potentialX;
            position.Y = potentialY;

            _rotationOffset.X = _offsetRadius * (float)Math.Sin(rotation - (Math.PI / 4));
            _rotationOffset.Y = -_offsetRadius * (float)Math.Cos(rotation - (Math.PI / 4));

            velocity.X *= _friction;

            if (velocity.X > 0 && velocity.X < 0.01f || velocity.X < 0 && velocity.X > -0.01f)
                velocity.X = 0;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_spriteTexture, position + _rotationOffset,
                size, Color.White, rotation, Vector2.Zero, 1, spriteEffect, layerDepth);
        }

        public void DrawCollisionPoints(SpriteBatch spriteBatch)
        {
            // Draw collision circle as 1 pixel dots
            foreach (var point in _collisionPoints)
                spriteBatch.Draw(_pixelTexture, point + position, new Rectangle(0, 0, 1, 1), Color.White);

            // Draw position as 1 pixel dot
            spriteBatch.Draw(_pixelTexture, position, new Rectangle(0, 0, 1, 1), Color.Magenta);
        }

    }
}