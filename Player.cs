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

        private SpriteFont _playerStatFont;

        private Crosshair crosshair = new Crosshair();
        private List<Grenade> _grenades = new List<Grenade>();
        
        private Targets _targets;
        const int NumberOfTargets = 5;

        private RoundTimer _rounds;
        private int _numberOfRounds = 3;
        private float _roundLength = 36;
        private float _countDownLength = 3;
        private bool _gameOver = false;
        private bool _allTargetsDead = false;

        private int Score = 0;

        private float _grenadePower;

        public const int PLAYER_SPEED = 100;
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

        private ActiveState State;
        private Direction Facing;

        public void LoadContent(ContentManager content)
        {
            _contentManager = content;

            Position = new Vector2(START_POSITION_X, START_POSITION_Y);
            _currentKeyboardState = Keyboard.GetState();
            _previousKeyboardState = _currentKeyboardState;
            crosshair.LoadContent(content);

            _targets = new Targets(NumberOfTargets);
            _targets.LoadContent(content);

            _rounds = new RoundTimer(_numberOfRounds, _roundLength, _countDownLength);
            _rounds.LoadContent(content);

            _playerStatFont = content.Load<SpriteFont>("StatFont");

            foreach (var grenade in _grenades)
                grenade.LoadContent(content);

            LoadContent(content, _fileName);
        }

        public void Update(GameTime gameTime)
        {
            _currentKeyboardState = Keyboard.GetState();

            UpdateMovement(_currentKeyboardState, gameTime);
            crosshair.UpdateCrosshair(gameTime, _currentKeyboardState, CalculateOrigin(Position));

            if (_rounds.roundActive)
                ChargeGrenadeThrow(_currentKeyboardState, _previousKeyboardState, gameTime);

            _rounds.update(gameTime);

            if (_rounds.CurrentRound > _numberOfRounds)
            {
                _targets.KillTargets();
                _gameOver = true;
            }

            _targets.UpdateTargets(gameTime);

            if (_targets.ActiveTargets < 1)
            {
                _allTargetsDead = true;
                _rounds.newRound = true;
            }

            if (_rounds.newRound)
            {
                if (_allTargetsDead)
                    Score += 500;

                _rounds.newRound = false;
                _allTargetsDead = false;
                _targets.ActiveTargets = NumberOfTargets;
                _targets.ResetTargets();
            }

            foreach (var grenade in _grenades)
            {
                grenade.UpdateGrenade(gameTime);
                if (_targets.UpdateTargetCollisions(grenade))
                {
                    //if (grenade.InMotion)
                    Score += (int)_rounds.TimeScoreMultiplier() * 5;
                    grenade.ThrowPower = grenade.ThrowPower / 10;
                    grenade.InitialTime = gameTime.TotalGameTime;
                    grenade.InitialPosition = grenade.Position - grenade.Origin;

                    //grenade.InMotion = false;
                }
            }

            _previousKeyboardState = _currentKeyboardState;
        }

        public void UpdateMovement(KeyboardState currentKeyboardState, GameTime gameTime)
        {
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                Position.X -= PLAYER_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (Position.X < 308)
                    Position.X = 308;

                State = ActiveState.Walking;
                Facing = Direction.Left;
            }
                
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                Position.X += PLAYER_SPEED * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (Position.X > 460)
                    Position.X = 460;

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

            //Vector2 objectText = new Vector2(10, 20);

            foreach (var grenade in _grenades)
            {
                grenade.Draw(spriteBatch);
                //spriteBatch.DrawString(_playerStatFont, "x: " + (int)grenade.Position.X + " y: " + (int)grenade.Position.Y, objectText, Color.Orange);
                //objectText.Y += 10;
            }

            spriteBatch.DrawString(_playerStatFont, "Score: " + Score, new Vector2(WizardGrenadeGame.SCREEN_WIDTH - 140, WizardGrenadeGame.SCREEN_HEIGHT - 80), Color.Turquoise);
            spriteBatch.DrawString(_playerStatFont, "power: " + (int)_grenadePower, new Vector2(WizardGrenadeGame.SCREEN_WIDTH - 140, WizardGrenadeGame.SCREEN_HEIGHT - 50), Color.Yellow);

            if (_gameOver)
                spriteBatch.DrawString(_playerStatFont, "FINAL SCORE: " + Score, new Vector2(WizardGrenadeGame.SCREEN_WIDTH / 2 - 120, WizardGrenadeGame.SCREEN_HEIGHT / 2), Color.HotPink);
            else
                _rounds.DrawTimer(spriteBatch);

            _targets.DrawTargets(spriteBatch);
        }

    }
}
