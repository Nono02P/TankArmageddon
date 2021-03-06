﻿using System;
using IA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TankArmageddon.GUI;

namespace TankArmageddon
{
    public partial class Tank : Sprite
    {
        #region Constantes
        private const float SPEED_ROTATION = 0.02f;
        private const float SPEED_MAX = 8f;
        private const float SPEED = 8f;
        private const float FRICTION = 4f;
        private const float GRAVITY = 1f;
        private const float FUEL_CONSUMPTION = 0.1f;
        #endregion

        #region Variables privées
        private float _originalLayer;
        private bool _parachute = true;
        private Image _imgParachute;
        private bool _onFloor = false;
        private bool _disableCCD = false;
        private eDirection _direction = eDirection.Right;
        private float _minCannonAngle = MathHelper.ToRadians(-90);
        private float _maxCannonAngle = MathHelper.ToRadians(0);
        private Vector2 _positionCannon;
        private Vector2 _originCannon;
        private Rectangle _imgCannon;
        private Vector2 _originWheel;
        private Vector2 _originWheelOffset;
        private Vector2 _originWheelNormal;
        private Rectangle _imgWheel;
        private Textbox _textBox;
        private int _life = 100;
        private float _fuel = 100;
        private BarGraph _lifeBar;
        private BarGraph _fuelBar;
        private TimeSpan _barSpeed = new TimeSpan(0, 0, 0, 0, 250);
        private Group _group;
        private Image _guiGameplayIndex;
        private Action.eActions _selectedAction;
        private NormalMove _normalMove;
        private HelicoTank _helicoTank;
        private OneShootFromTank _oneShootFromTank;
        private MultipleShootFromTank _multipleShootFromTank;
        private OneShootFromAirplane _oneShootFromAirplane;
        private LetOnFloor _letOnFloor;
        #endregion

        #region Propriétés
        public Team Parent { get; set; }
        public Color TeamColor { get; private set; }
        public IAction Action { get; private set; }
        public string Name { get; set; }
        public float AngleCannon { get; set; } = MathHelper.ToRadians(360);
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Space { get; set; }
        public bool IsControlled { get; set; }
        public bool Parachute { get => _parachute; private set { if (_parachute != value) { _parachute = value; _imgParachute.Visible = value; } } }
        public Action.eActions SelectedAction { get => _selectedAction; set { if (!Action.BlockAction) { _selectedAction = value; RefreshActionClass(); } } }
        public int Life { get => _life; set { _life = MathHelper.Clamp(value, 0, 100); _lifeBar.SetProgressiveValue(value, _barSpeed); RefreshGUITextbox(); } }
        public float Fuel { get => _fuel; set { _fuel = MathHelper.Clamp(value, 0, 100); _fuelBar.SetProgressiveValue(value, _barSpeed); } }
        #endregion

        #region Constructeur
        public Tank(Team pParent, Color pTeamColor, string pName, Texture2D pImage, Rectangle pTankImage, Rectangle pCannonImage, Rectangle pWheelImage, Vector2 pPosition, Vector2 pOrigin, Vector2 pScale) : base(pImage, pTankImage, pPosition, pOrigin, pScale)
        {
            #region Initialise les valeurs
            _originalLayer = Layer;
            Angle = MathHelper.ToRadians(360);
            Parent = pParent;
            TeamColor = pTeamColor;
            Name = pName;
            _imgCannon = pCannonImage;
            _originCannon = new Vector2(0, _imgCannon.Height / 2);
            _imgWheel = pWheelImage;
            _originWheelOffset = new Vector2(ImgBox.Value.Width * 0.04f, 0);
            _originWheelNormal = new Vector2(_imgWheel.Width / 2, _imgWheel.Height / 2 - ImgBox.Value.Height / 2.5f);
            _originWheel = _originWheelNormal + _originWheelOffset;
            BoundingBox = new Circle(Position.ToPoint(), ImgBox.Value.Width / 2 * Scale.X * 0.9f, 50, Color.Red);
            #endregion

            #region Créé la GUI
            _group = new Group();
            _textBox = new Textbox(new Vector2(0, - ImgBox.Value.Height * 1.5f), AssetManager.MainFont, Name + " : " + Life);
            _textBox.ApplyColor(pTeamColor, Color.Black);
            _textBox.SetOriginToCenter();
            _group.AddElement(_textBox);

            Vector2 s = new Vector2(100, 15);
            _lifeBar = new BarGraph(Life, 0, Life, new Vector2(0, -ImgBox.Value.Height * 1.25f), new Vector2(s.X / 2, 0), s, Color.Blue, Color.Green);
            _group.AddElement(_lifeBar);

            _fuelBar = new BarGraph(Fuel, 0, Fuel, new Vector2(0, -ImgBox.Value.Height), new Vector2(s.X / 2, 0), s, Color.Blue, new Color(240, 105, 105));
            _group.AddElement(_fuelBar);
            
            Rectangle paraImgBox = AssetManager.ParachutesImgBox[utils.MathRnd(0, AssetManager.ParachutesImgBox.Count)];
            _imgParachute = new Image(AssetManager.Parachute, paraImgBox, new Vector2(0, -ImgBox.Value.Height / 2 * Scale.Y), new Vector2(paraImgBox.Width / 2, paraImgBox.Height));
            _imgParachute.Layer = Layer + 0.1f;
            _group.AddElement(_imgParachute);
            
            Texture2D cursor = new Texture2D(MainGame.graphics.GraphicsDevice, 15, 10);
            Color[] data = new Color[cursor.Width * cursor.Height];
            Color teamColor = Parent.TeamColor;
            for (int pixel = 0; pixel < data.Length; pixel++)
            {
                data[pixel] = teamColor;
            }
            cursor.SetData(data);

            _guiGameplayIndex = new Image(cursor, Parent.Parent.GetPositionOnMinimap(pPosition.X), new Vector2(cursor.Width / 2, cursor.Height * 1.65f));
            Parent.Parent.GUIGroup.AddElement(_guiGameplayIndex);
            #endregion

            #region Instanciation des Actions
            _normalMove = new NormalMove(this);
            _helicoTank = new HelicoTank(this);
            _oneShootFromTank = new OneShootFromTank(this);
            _multipleShootFromTank = new MultipleShootFromTank(this);
            _oneShootFromAirplane = new OneShootFromAirplane(this);
            _letOnFloor = new LetOnFloor(this);
            Action = _normalMove;
            #endregion

            #region Abonnement aux évènements
            OnSpriteEffectsChange += Sprite_OnSpriteEffectsChange;
            Parent.Parent.OnExplosion += Gameplay_OnExplosion;
            Parent.Parent.OnTourTimerEnd += Gameplay_OnTourTimerEnd;
            #endregion
        }
        #endregion

        #region Changement de direction du tank (Gauche <--> Droite)
        /// <summary>
        /// Sur changement du SpriteEffects (flip horizontal), gère les positions d'images et les angles de canon.
        /// </summary>
        public void Sprite_OnSpriteEffectsChange(object sender, SpriteEffects previous, SpriteEffects after)
        {
            switch (after)
            {
                case SpriteEffects.None:
                    _direction = eDirection.Right;
                    _minCannonAngle = MathHelper.ToRadians(-90);
                    _maxCannonAngle = MathHelper.ToRadians(0);
                    _originWheel = _originWheelNormal + _originWheelOffset;
                    break;
                case SpriteEffects.FlipHorizontally:
                    _direction = eDirection.Left;
                    _minCannonAngle = MathHelper.ToRadians(-180);
                    _maxCannonAngle = MathHelper.ToRadians(-90);
                    _originWheel = _originWheelNormal - _originWheelOffset;
                    break;
                default:
                    break;
            }
            AngleCannon = -MathHelper.ToRadians(180) - AngleCannon;
        }
        #endregion

        #region Gestion des actions en fonction de l'item sélectionné
        /// <summary>
        /// Sélectionne la public classe à utiliser en fonction du type d'action.
        /// </summary>
        private void RefreshActionClass()
        {
            Action.BeforeActionChange();
            switch (SelectedAction)
            {
                case TankArmageddon.Action.eActions.None:
                    Action = _normalMove;
                    break;
                case TankArmageddon.Action.eActions.iGrayBullet:
                case TankArmageddon.Action.eActions.GoldBullet:
                    Action = _multipleShootFromTank;
                    break;
                case TankArmageddon.Action.eActions.iGrayBombshell:
                case TankArmageddon.Action.eActions.GoldBombshell:
                case TankArmageddon.Action.eActions.Grenada:
                case TankArmageddon.Action.eActions.SaintGrenada:
                    Action = _oneShootFromTank;
                    break;
                case TankArmageddon.Action.eActions.GrayMissile:
                case TankArmageddon.Action.eActions.GreenMissile:
                case TankArmageddon.Action.eActions.DropHealth:
                case TankArmageddon.Action.eActions.iDropFuel:
                    Action = _oneShootFromAirplane;
                    break;
                //case Action.eActions.iTankBaseBall:
                    // TODO : Jouer l'animation
                    //break;
                case TankArmageddon.Action.eActions.HelicoTank:
                    Action = _helicoTank;
                    break;
                case TankArmageddon.Action.eActions.Drilling:
                    // TODO : Jouer l'animation
                    break;
                case TankArmageddon.Action.eActions.iMine:
                    Action = _letOnFloor;
                    break;
                default:
                    break;
            }
            Action.Enable = true;
        }
        #endregion

        #region Mort du tank
        private void Die(bool pWithExplosion)
        {
            #region Création d'une explosion
            if (pWithExplosion)
            {
                Parent.Parent.CreateExplosion(this, new ExplosionEventArgs(Position, 50, 50));
            }
            #endregion

            #region Suppression du tank et de ses éléments de GUI
            _guiGameplayIndex.Remove = true;
            Parent.Parent.GUIGroup.RemoveElement(_guiGameplayIndex);
            Remove = true;
            _group.Remove = true;
            #endregion

            #region Fin de tour si le tank est celui sélectionné
            if (IsControlled)
            {
                Parent.Parent.FinnishTour(true);
            }
            #endregion

            #region Désabonnement aux évènements
            OnSpriteEffectsChange -= Sprite_OnSpriteEffectsChange;
            Parent.Parent.OnExplosion -= Gameplay_OnExplosion;
            Parent.Parent.OnTourTimerEnd -= Gameplay_OnTourTimerEnd;
            #endregion
        }
        #endregion

        #region Sur explosion sur la map
        private void Gameplay_OnExplosion(object sender, ExplosionEventArgs pExplosionEventArgs)
        {
            Circle c = pExplosionEventArgs.ExplosionCircle;
            if (c.Intersects(BoundingBox))
            {
                int force = pExplosionEventArgs.Force;
                Life -= force;

                // Si l'explosion provient d'une action d'un tank.
                ISentByTank sentByTank;
                Tank tank;
                if (sender is Tank)
                {
                    tank = (Tank)sender;
                }
                else
                {
                    sentByTank = (ISentByTank)sender;
                    tank = sentByTank.Sender;
                }
                
                if (tank != null)
                {
                    if (tank.Parent.Control is NeuralNetworkControl)
                    {
                        GeneticNeuralNetwork Genome = ((NeuralNetworkControl)tank.Parent.Control).Genome;
                        // Si le tank qui a provoqué l'explosion fait partie de la même équipe, provoque un malus
                        if (tank.Parent == Parent)
                        {
                            Genome.FitnessScore -= NeuralNetworkControl.MalusFriendlyFire + (force / 10);

                            // Si il est en plus à l'origine de sa mort, ajoute un autre malus
                            if (Life <= 0)
                            {
                                Genome.FitnessScore -= NeuralNetworkControl.MalusFriendKilled;
                            }
                        }
                        else
                        {
                            // Si le tank qui a provoqué l'explosion fait partie d'une autre équipe, lui ajoute un bonus
                            Genome.FitnessScore += force;

                            // Si il est en plus à l'origine de sa mort, ajoute un autre bonus
                            if (Life <= 0)
                            {
                                Genome.FitnessScore += NeuralNetworkControl.BonusTankKilled;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Sur fin de tour
        private void Gameplay_OnTourTimerEnd(object sender, EventArgs e)
        {
            Action.BlockAction = false;
            Action.EndOfTour();
            SelectedAction = TankArmageddon.Action.eActions.None;
            Parent.Parent.RefreshActionButtonSelection(SelectedAction);
        }
        #endregion

        #region BoundingBox
        public override void RefreshBoundingBox()
        {
            BoundingBox.Location = Position.ToPoint();
        }
        #endregion

        #region Mise à jour TextBox 
        private void RefreshGUITextbox()
        {
            _textBox.Text = Name + " : " + Life;
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            #region Contrôle des déplacements du canon, du tank et des tirs en fonction de l'action sélectionnée
            float vx = Velocity.X;
            float vy = Velocity.Y;
            if (IsControlled)
            {
                Action.Update(gameTime, ref vx, ref vy);
                Layer = _originalLayer + 0.1f;
            }
            else
            {
                Layer = _originalLayer;
            }
            #endregion

            Gameplay g = Parent.Parent;
            Vector2 mapSize = g.MapSize;

            #region Gestion de la gravité en fonction du parachute et de la chute dans l'eau
            // Applique la gravité uniquement si le tank ne touche pas le sol (permet au tank d'éviter de passer au travers le sol qui n'est pas totalement détruit)
            if (Parachute && !_onFloor)
            {
                vy = GRAVITY;
            }
            else if (!_onFloor)
            {
                vy += GRAVITY;
            }

            // Dans l'eau
            if (Position.Y > g.WaterLevel)
            {
                Velocity.Normalize();
                if (Position.Y > g.WaterLevel + g.WaterHeight)
                {
                    Die(false);
                    if (IsControlled && Parent.Control is NeuralNetworkControl)
                    {
                        NeuralNetworkControl nn = (NeuralNetworkControl)Parent.Control;
                        nn.Genome.FitnessScore -= NeuralNetworkControl.MalusFallInWater;
                    }
                }
            }
            #endregion

            #region Application de la friction
            if (vx > 0)
            {
                vx -= FRICTION;
                if (vx < 0)
                {
                    if (MathHelper.ToDegrees(Angle) < 0)
                    {
                        vx = 0;
                    }
                    else
                    {
                        vx = SPEED_MAX;
                    }
                }
            }
            else if (vx < 0)
            {
                vx += FRICTION;
                if (vx > 0)
                {
                    if (MathHelper.ToDegrees(Angle) > 0)
                    {
                        vx = 0;
                    }
                    else
                    {
                        vx = -SPEED_MAX;
                    }
                }
            }
            #endregion

            #region Limitations de la vélocité
            vx = MathHelper.Clamp(vx, -SPEED_MAX, SPEED_MAX);
            vy = MathHelper.Clamp(vy, -SPEED_MAX, SPEED_MAX);
            Velocity = new Vector2(vx, vy);
            #endregion

            #region Récupération de l'ancienne position en 3 points en bas (Gauche / Centre / Droite) pour vérifier les collisions
            Vector2 previousPosLeft = new Vector2(BoundingBox.Left, BoundingBox.Bottom);
            Vector2 previousPosMiddle = new Vector2(BoundingBox.Center.X, BoundingBox.Bottom);
            Vector2 previousPosRight = new Vector2(BoundingBox.Right, BoundingBox.Bottom);
            #endregion

            base.Update(gameTime);

            #region Empêche de sortir de la map
            Position = new Vector2(MathHelper.Clamp(Position.X, Origin.X, Parent.Parent.MapSize.X - Origin.X), Position.Y);
            #endregion

            #region Collisions avec le sol et angle du tank
            Vector2 newPosLeft = new Vector2(BoundingBox.Left, BoundingBox.Bottom);
            Vector2 newPosMiddle = new Vector2(BoundingBox.Center.X, BoundingBox.Bottom);
            Vector2 newPosRight = new Vector2(BoundingBox.Right, BoundingBox.Bottom);

            bool collision = false;
            
            Vector2 normalised = Vector2.Normalize(Velocity);
            Vector2 collisionPosLeft = previousPosLeft;
            Vector2 collisionPosMiddle = previousPosMiddle;
            Vector2 collisionPosRight = previousPosRight;
            do
            {
                collisionPosMiddle += normalised;
                collisionPosLeft += normalised;
                collisionPosRight += normalised;
                if (g.IsSolid(collisionPosLeft) || g.IsSolid(collisionPosMiddle) || g.IsSolid(collisionPosRight))
                {
                    collision = true;
                    collisionPosMiddle -= normalised;
                    collisionPosLeft -= normalised;
                    collisionPosRight -= normalised;
                }
            } while (!collision && 
            collisionPosMiddle.X > 0 && collisionPosMiddle.X < mapSize.X && collisionPosMiddle.Y > 0 && collisionPosMiddle.Y < mapSize.Y &&
            collisionPosLeft.X > 0 && collisionPosLeft.X < mapSize.X && collisionPosLeft.Y > 0 && collisionPosLeft.Y < mapSize.Y &&
            collisionPosRight.X > 0 && collisionPosRight.X < mapSize.X && collisionPosRight.Y > 0 && collisionPosRight.Y < mapSize.Y &&
            Math.Abs((collisionPosMiddle - newPosMiddle).X) >= Math.Abs(normalised.X) && Math.Abs((collisionPosMiddle - newPosMiddle).Y) >= Math.Abs(normalised.Y) &&
            Math.Abs((collisionPosLeft - newPosLeft).X) >= Math.Abs(normalised.X) && Math.Abs((collisionPosLeft - newPosLeft).Y) >= Math.Abs(normalised.Y) &&
            Math.Abs((collisionPosRight - newPosRight).X) >= Math.Abs(normalised.X) && Math.Abs((collisionPosRight - newPosRight).Y) >= Math.Abs(normalised.Y));
            
            // Désactivation de la Continuous Collision Detection (pour permettre le décollage de l'hélicotank)
            if (_disableCCD)
            {
                Vector2 p = new Vector2(BoundingBox.Center.X, BoundingBox.Bottom);
                collision = Parent.Parent.IsSolid(p);
            }

            if (collision)
            {
                Parachute = false;
                _onFloor = true;

                // Récupère l'altitude en Y à position.X -20 et +20 afin d'en déterminer l'angle à partir d'un vecteur tracé entre ces deux points.
                Vector2 center;
                Vector2 before;
                Vector2 after;
                int delta = 20;
                if (collisionPosMiddle.Y > previousPosMiddle.Y)
                {
                    center = g.FindHighestPoint(collisionPosMiddle, 0);
                    before = g.FindHighestPoint(collisionPosMiddle, -delta);
                    after = g.FindHighestPoint(collisionPosMiddle, delta);
                }
                else
                {
                    center = g.FindHighestPoint(previousPosMiddle, 0);
                    before = g.FindHighestPoint(previousPosMiddle, -delta);
                    after = g.FindHighestPoint(previousPosMiddle, delta);
                }
                Angle = (float)utils.MathAngle(after - before);

                // Vérifie que le point le plus haut retourné n'est pas plus grand que le tank.
                // Permet d'empêcher le tank de se téléporter au dessus quand il est dans un trou.
                if (center.Y > - BoundingBox.Height)
                {
                    Position += center;
                }
                else if (before.Y > -BoundingBox.Height)
                {
                    Position += before;
                }
                else if (after.Y > -BoundingBox.Height)
                {
                    Position += after;
                }
            }
            else
            {
                _onFloor = false;
            }
            #endregion

            #region Positionnement de GUI
            _group.Position = Position;
            _group.Layer = Layer;
            _guiGameplayIndex.Position = g.GetPositionOnMinimap(Position.X);
            #endregion

            #region Calcul de la position du canon par rapport à la position et l'angle du tank
            float hyp = ImgBox.Value.Height * Scale.Y * 0.35f;
            float x = (float)Math.Sin(Angle) * hyp;
            float y = (float)Math.Cos(Angle) * hyp;
            _positionCannon = new Vector2(Position.X + x, Position.Y - y);
            #endregion

            #region Mort du tank si vie à 0
            if (Life <= 0)
            {
                Die(Life <= 0);
            }
            #endregion
        }
        #endregion

        #region Draw
        public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(Image, Position, _imgWheel, Color.White, Angle, _originWheel, Scale, Effects, 0);
            spriteBatch.Draw(Image, _positionCannon, _imgCannon, Color.White, Angle + AngleCannon, _originCannon, Scale, SpriteEffects.None, 0);
            base.Draw(spriteBatch, gameTime);
            _group.Draw(spriteBatch, gameTime);
        }
        #endregion
    }
}