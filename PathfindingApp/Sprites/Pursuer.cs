using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using PathfindingApp.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace PathfindingApp.Sprites
{
    public enum PursueAlgorithm
    {
        BFS,
        DFS,
        AStar
    }
    public class Pursuer : Sprite
    {
        public PursueAlgorithm Algorithm;

        public override Point Coordinates 
        { 
            get => base.Coordinates;
            set 
            { 
                HasRunAlgorithms = false;

                base.Coordinates = value;
                StartPoint = value;
            } 
        }
        public Point StartPoint;
        public Point CurrentPoint;

        public List<Point> SearchCells; // Cells that are being searched by algorithm
        public List<Point> PathToGoal; // Path found to goal
        public bool PathUpdated;
        public bool HasRunAlgorithms;
        public bool Running;

        public bool StatsNeedUpdate;
        public bool ShowStats;

        private Vector2 _statsPosition;
        private bool _statsDrawn;
        private RenderTarget2D _statsTarget;
        private float _elapsedRunTime;

        private PursuerPath _searchPath;

        private BitmapFont _font;

        private Dictionary<string, Rectangle> _faceDirections = new Dictionary<string, Rectangle>()
        {
            { "down", new Rectangle(0, 0, 16, 16) },
            { "up", new Rectangle(0, 16, 16, 16 ) },
            { "right", new Rectangle(16, 0, 16, 16) },
            { "left", new Rectangle(16, 16, 16, 16) }
        };

        protected Effect _outlineEffect;

        public Pursuer()
        {

        }
        public Pursuer(PursueAlgorithm algorithm)
            :base()
        {
            Algorithm = algorithm;

            SetUp();
        }
        public Pursuer(Vector2 position, PursueAlgorithm algorithm)
            : base(position)
        {
            Algorithm = algorithm;
            SetUp();
        }

        public override void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("Graphics/Enemy");
            _font = content.Load<BitmapFont>("UI/statsFont");

            SourceRect = _faceDirections["down"];

            _outlineEffect = content.Load<Effect>("Shaders/Outline");
            _outlineEffect.Parameters["texelSize"].SetValue(new Vector2(1f / (SourceRect.Width - 1f), 1f / (SourceRect.Height - 1f)));
            _outlineEffect.Parameters["outlineColor"].SetValue(new Vector4(1.0f, 1.0f, 1.0f, 1.0f));

        }
        public override void Update(GameTime gameTime)
        {
            if (Running)
            {
                float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds * Game1.GameSpeed;
                _elapsedRunTime += elapsed;

                _searchPath.Update(elapsed);
                if (_searchPath.SearchFinished && !StatsNeedUpdate && !_statsDrawn)
                {
                    StatsNeedUpdate = true;
                }
            }

        }
        public void UpdateSearchCells(Cursor cursor)
        {
            if (_searchPath.NeedsMapUpdate)
            {
                _searchPath.NeedsMapUpdate = false;
                PathUpdated = true;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (_statsDrawn)
            {
                if (ShowStats)
                    spriteBatch.Draw(_statsTarget, _statsPosition, Color.White);
            }
        }
        public void DrawPath(SpriteBatch spriteBatch)
        {
            if (_searchPath != null)
                _searchPath.Draw(spriteBatch);
            
        }
        public void DrawStats(GraphicsDevice graphics, SpriteBatch spriteBatch, Map map)
        {
            if (_statsDrawn) return;

            graphics.SetRenderTarget(_statsTarget);
            graphics.Clear(Color.Transparent);

            spriteBatch.Begin();
            spriteBatch.Draw(Game1.SquareTexture, _statsTarget.Bounds, Color.Black * .7f);
            spriteBatch.DrawString(_font, GetAlgorithmType(), new Vector2(5, 5), Color.White);
            spriteBatch.DrawString(_font, "Cells Searched: " + SearchCells.Count, new Vector2(5, 20), Color.White);
            spriteBatch.DrawString(_font, "Search time: " + _elapsedRunTime.ToString("n2"), new Vector2(5, 35), Color.White);

            spriteBatch.End();

            graphics.SetRenderTarget(null);

            _statsDrawn = true;

            float newX = Position.X + 24;
            float newY = Position.Y + 24;
            if (newX < 0) newX = 0;
            if (newX + _statsTarget.Width > map.MapTarget.Width) newX = map.MapTarget.Width - _statsTarget.Width;
            if (newY < 0) newY = 0;
            if (newY + _statsTarget.Height > map.MapTarget.Height) newY = map.MapTarget.Height - _statsTarget.Height;

            _statsPosition = new Vector2(newX, newY);
        }

        public override object Copy()
        {
            Pursuer pursuer = new Pursuer();
            pursuer.SetAttributes(Texture, Position, SourceRect, Scale, Rotation, Color, Bounds, Algorithm, _font);
            return pursuer;
        }
        public void SetAttributes(Texture2D texture, Vector2 position, Rectangle sourceRect, Vector2 scale, float rotation, Color color, Rectangle bounds, PursueAlgorithm algorithm, BitmapFont font)
        {
            Position = position;
            base.SetAttributes(texture, sourceRect, scale, rotation, color, bounds);
            Algorithm = algorithm;
            _font = font;
        }
        public void SetTarget(GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
            if (_statsDrawn) return;

            _statsTarget = new RenderTarget2D(graphics, 100, 50);
        }

        public void RunAlgorithm(Map map, Point goal)
        {
            SearchCells = new List<Point>();
            HasRunAlgorithms = true;
            switch (Algorithm)
            {
                case PursueAlgorithm.BFS:
                    PathToGoal = Algorithms.BFS(map, StartPoint, goal, SearchCells);
                    break;
                case PursueAlgorithm.DFS:
                    PathToGoal = Algorithms.DFS(map, StartPoint, goal, SearchCells);
                    break;
                case PursueAlgorithm.AStar:
                    PathToGoal = Algorithms.AStar(map, StartPoint, goal, SearchCells);
                    break;
            }

            _searchPath = new PursuerPath(Color, SearchCells, PathToGoal, null, null);
        }
        public void Reset()
        {
            Running = false;
            HasRunAlgorithms = false;
            _algorithmsFinished = false;
            _elapsedRunTime = 0f;

            StatsNeedUpdate = false;
            _statsDrawn = false;
        }

        private string GetAlgorithmType()
        {
            switch (Algorithm)
            {
                case PursueAlgorithm.BFS:
                    return "Breadth First Search";
                case PursueAlgorithm.DFS:
                    return "Depth First Search";
                case PursueAlgorithm.AStar:
                    return "A*";
                default:
                    return "";
            }
        }
        private void SetUp()
        {
            switch (Algorithm)
            {
                case PursueAlgorithm.BFS:
                    Color = Color.LightSkyBlue;
                    break;
                case PursueAlgorithm.DFS:
                    Color = Color.Orange;
                    break;
                case PursueAlgorithm.AStar:
                    Color = Color.ForestGreen;
                    break;
            }
        }
        private void MoveToNextSpot()
        {

        }
    }
    /// <summary>
    /// Visual Representation of the pursuers path to the goal
    /// </summary>
    public class PursuerPath : Sprite
    {
        public bool NeedsMapUpdate;

        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;

        public bool SearchFinished;

        private bool _needsUpdate;

        private Color _pathColor;
        List<Point> _searchPoints;
        private int _currentIndex = 0;
        List<Sprite> _searchBlocks;
        List<Point> _pathPoints;
        List<Sprite> _pathBlocks;

        private RenderTarget2D _renderTarget;
        private Timer _pathTimer;
        private float _pathTimerMax = .1f;

        public PursuerPath(Color color, List<Point> searchPoints, List<Point> pathPoints, GraphicsDevice graphics, SpriteBatch spriteBatch)
            : base()
        {
            _pathColor = color;
            _searchPoints = searchPoints;
            _pathPoints = pathPoints;
            _graphics = graphics;
            _spriteBatch = spriteBatch;

            _searchBlocks = new List<Sprite>();
            _pathBlocks = new List<Sprite>();
            _pathTimer = new Timer(_pathTimerMax);
            Texture = Game1.SquareTexture;
        }
        public void Update(float elapsed)
        {
            if (!SearchFinished) {
                if (_pathTimer.Finished)
                {
                    AddNextBlock();
                    _pathTimer.Reset();
                }
                else
                {
                    _pathTimer.AddTime(elapsed);
                }
            }

        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite block in _searchBlocks)
            {
                block.Draw(spriteBatch);
            }
            if (_pathBlocks != null)
            {
                foreach (Sprite block in _pathBlocks)
                {
                    block.Draw(spriteBatch);
                }
            }
        }

        private void AddNextBlock()
        {
            Sprite block = MakeBlock(_searchPoints[_currentIndex]);
            _searchBlocks.Add(block);

            NeedsMapUpdate = true;

            if (_currentIndex < _searchPoints.Count - 1)
                _currentIndex++;
            else
            {
                SearchFinished = true;
                SetPathBlocks();
            }
        }
        private Sprite MakeBlock(Point point)
        {
            Sprite block = new Sprite();
            block.Position = point.ToVector2() * 16;
            block.Alpha = .8f;
            block.SetAttributes(
                Texture,
                new Rectangle(0, 0, 1, 1),
                new Vector2(16, 16), 0f,
                _pathColor,
                new Rectangle(block.Position.ToPoint(),
                new Point(16, 16)));
            return block;
        }
        private void SetPathBlocks()
        {
            // If pathPoints is null then there was no path to the goal
            if (_pathPoints == null)
                return;
            for (int i = 0; i < _pathPoints.Count; i++)
            {
                Sprite block = MakeBlock(_pathPoints[i]);
                block.Color = new Color(255 - _pathColor.R, 255 - _pathColor.G, 255 - _pathColor.B);
                _pathBlocks.Add(block);
            }

            NeedsMapUpdate = true;
        }
        
    }
}
