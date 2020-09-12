﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace WizardGrenade
{
    class TerrainSprite : Polygon
    {
        private Texture2D _spriteTexture;
        public Rectangle size;
        public Vector2 relativeOrigin;
        public Vector2 position;
        public float rotation = 0.0f;
        private List<Vector2> _collisionPoints = new List<Vector2>();
        private Vector2 _rotationOffset = Vector2.Zero;
        private float _offsetLength;

        public Color Colour = Color.LimeGreen;
        public SpriteEffects SpriteEffect = SpriteEffects.None;
        public bool unlocked = true;

        private KeyboardState _currentKBState;
        private KeyboardState _previousKBState;
        private MouseState _currentMouseState;

        public void LoadContent(ContentManager contentManager, string fileName)
        {
            _spriteTexture = contentManager.Load<Texture2D>(fileName);
            size = new Rectangle(0, 0, _spriteTexture.Width, _spriteTexture.Height);
            relativeOrigin = new Vector2(_spriteTexture.Width / 2, _spriteTexture.Height / 2);
            _offsetLength = (float)Math.Sqrt(Math.Pow((_spriteTexture.Width / 2), 2) + Math.Pow((_spriteTexture.Height / 2), 2));
            polyPoints = Collision.CalcRectangleCollisionPoints(_spriteTexture.Width, _spriteTexture.Height);
            LoadPolyContent(contentManager);
        }

        public void SetBlocks(GameTime gameTime)
        {
            if (unlocked)
            {
                _currentKBState = Keyboard.GetState();
                _currentMouseState = Mouse.GetState();

                position.X = _currentMouseState.X;
                position.Y = _currentMouseState.Y;

                if (Utility.KeysReleased(_currentKBState, _previousKBState, Keys.R))
                    rotation -= (float)Math.PI / 4;
                if (Utility.KeysReleased(_currentKBState, _previousKBState, Keys.T))
                    rotation += (float)Math.PI / 4;

                _rotationOffset.X = _offsetLength * (float)Math.Sin(rotation) - (_spriteTexture.Width / 2 * (float)Math.Cos(rotation));
                _rotationOffset.Y = -_offsetLength * (float)Math.Cos(rotation) - (_spriteTexture.Width / 2 * (float)Math.Sin(rotation));

                _previousKBState = _currentKBState;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_spriteTexture, position + _rotationOffset,
                size, Colour, rotation, Vector2.Zero, 1, SpriteEffect, 0);

            DrawCollisionPoints(spriteBatch, position);
        }
    }
}
