﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace WizardGrenade
{
    class Player : Sprite
    {
        private readonly string _fileName = "wizard_solo";
             
        private Crosshair crosshair = new Crosshair();
        private List<Grenade> _grenades = new List<Grenade>();
        public float _grenadePower;

        private const int PLAYER_SPEED = 100;
        private const int POWER_COEFFICIENT = 400;

        private int START_POSITION_X;
        private int START_POSITION_Y;

        private ContentManager _contentManager;
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        public Player(int startx, int starty)
        {
            START_POSITION_X = startx;
            START_POSITION_Y = starty;
        }

        private enum ActiveState
        {
            Walking,
            Idle,
        }

        private enum Direction
        {
            Left,
            Right,
        }

        private ActiveState State { get; set; }
        private Direction Facing { get; set; }

        public void LoadContent(ContentManager contentManager)
        {
            _contentManager = contentManager;

            Position = new Vector2(START_POSITION_X, START_POSITION_Y);
            _currentKeyboardState = Keyboard.GetState();
            _previousKeyboardState = _currentKeyboardState;
            crosshair.LoadContent(contentManager);

            foreach (var grenade in _grenades)
                grenade.LoadContent(contentManager);

            LoadContent(contentManager, _fileName);
        }

        public void Update(GameTime gameTime)
        {
            _currentKeyboardState = Keyboard.GetState();

            UpdateMovement(_currentKeyboardState, gameTime);
            ChargeGrenadeThrow(_currentKeyboardState, _previousKeyboardState, gameTime);
            
            foreach (var grenade in _grenades)
            {
                grenade.UpdateGrenade(gameTime);
            }



            crosshair.UpdateCrosshair(gameTime, _currentKeyboardState, CalculateOrigin(Position));

            _previousKeyboardState = _currentKeyboardState;
        }

        public void UpdateMovement(KeyboardState currentKeyboardState, GameTime gameTime)
        {
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                Position.X -= PLAYER_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
                State = ActiveState.Walking;
                Facing = Direction.Left;

            }
                
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                Position.X += PLAYER_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
                State = ActiveState.Walking;
                Facing = Direction.Right;
            }

        }

        public void ChargeGrenadeThrow(KeyboardState currentKeyboardState, KeyboardState previousKeyboardState, GameTime gameTime)
        {
            if (currentKeyboardState.IsKeyDown(Keys.Space) && _grenadePower < 500)
                _grenadePower += (float)gameTime.ElapsedGameTime.TotalSeconds * POWER_COEFFICIENT;

            if (WizardGrenadeGame.KeysReleased(currentKeyboardState, previousKeyboardState, Keys.Space))
            {
                ThrowGrenade(_grenadePower, gameTime);
                _grenadePower = 0;
            }
        }

        // getter and setters
        public void ThrowGrenade(float grenadePower, GameTime gameTime)
        {
            foreach (var dormantGrenade in _grenades)
            {
                if (!dormantGrenade.InMotion)
                {
                    dormantGrenade.ThrowPower = grenadePower;
                    dormantGrenade.ThrowAngle = crosshair.crosshairAngle;
                    dormantGrenade.InitialTime = gameTime.TotalGameTime;
                    dormantGrenade.InitialPosition = Position + Origin - dormantGrenade.Origin;
                    dormantGrenade.InMotion = true;
                    return;
                }
            }

            Grenade grenade = new Grenade(grenadePower, crosshair.crosshairAngle, Position + Origin, gameTime.TotalGameTime);
            grenade.LoadContent(_contentManager);
            _grenades.Add(grenade);
            return;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            crosshair.Draw(spriteBatch);

            foreach (var grenade in _grenades)
                grenade.Draw(spriteBatch);

        }

    }
}
