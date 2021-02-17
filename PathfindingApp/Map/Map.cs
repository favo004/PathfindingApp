using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PathfindingApp.Settings;
using PathfindingApp.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingApp
{
    /// <summary>
    /// This class reads in a text file and converts it into a 2D array that represents 
    /// the map
    /// </summary>
    public class Map : Sprite
    {
        public RenderTarget2D MapTarget;
        public char[][] MapKeys;
        public bool NeedsUpdate;

        public List<Pursuer> Pursuers;
        public Goal Goal;

        public bool Running;
        public bool Paused;

        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;
        private List<List<Cell>> _mapCells;
        private Cell _highlightedCell;

        private Cursor _cursor;
        private bool _cursorOnMap;

        private const int MAX_PURSUERS = 4;

        public Map(string filePath)
        {
            GenerateMapFromFile(filePath);
        }
        public Map(string filePath, Cursor cursor, GraphicsDevice graphics, SpriteBatch spriteBatch)
        {
            _graphics = graphics;
            _spriteBatch = spriteBatch;
            _cursor = cursor;
            _cursor.PursuerMax = MAX_PURSUERS;

            GenerateMapFromFile(filePath);

            Scale = Vector2.One;
            Origin = new Vector2(MapTarget.Bounds.Width / 2, MapTarget.Bounds.Height / 2);

            Pursuers = new List<Pursuer>();
        }

        /// <summary>
        /// Loads map data graphically
        /// </summary>
        /// <param name="content"></param>
        public void LoadMap(ContentManager content)
        {
            Position = new Vector2(ScreenSettings.GameWidth / 2 - MapTarget.Width / 2, ScreenSettings.GameHeight / 2 - MapTarget.Height / 2);
            Bounds = new Rectangle(Position.ToPoint(), new Point(MapTarget.Bounds.Width, MapTarget.Bounds.Height));

            _mapCells = new List<List<Cell>>();

            for (int i = 0; i < MapKeys.Length; i++)
            {
                List<Cell> rowCells = new List<Cell>();
                for (int j = 0; j < MapKeys[i].Length; j++)
                {
                    Vector2 position = new Vector2(j * 16, i * 16);
                    string key = MapKeys[i][j] == '*' ? "wall" : "floor";

                    Cell cell = new Cell(position, Position, key);
                    cell.Coordinates = new Point(j, i);
                    cell.LoadContent(content);

                    rowCells.Add(cell);
                }
                _mapCells.Add(rowCells);
            }
        }

        public override void Update(GameTime gameTime)
        {
            CheckForCursorOnMap();
            CheckForCursorHighlight();

            if (Running)
            {
                if (!Paused)
                {
                    CheckForPursuerHover();
                    CheckForPursuerStats();

                    Run(gameTime);

                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            // If map has been changed, we need to redraw the map to the target before
            // we draw the target again
            if (NeedsUpdate)
            {
                DrawMapToTarget(spriteBatch);
            }
            Vector2 centerPosition = Position + Origin;
            spriteBatch.Draw(MapTarget, centerPosition, MapTarget.Bounds, Color.White, Rotation, Origin, Scale, SpriteEffects.None, 0.5f);
        }

        public void DrawMapToTarget(SpriteBatch spriteBatch)
        {
            _graphics.SetRenderTarget(MapTarget);
            _graphics.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate,
                    BlendState.AlphaBlend,
                    SamplerState.PointClamp,
                    null,
                    null);

            for (int i = 0; i < _mapCells.Count; i++)
            {
                for (int j = 0; j < _mapCells[i].Count; j++)
                {
                    _mapCells[i][j].Draw(spriteBatch);
                }
            }
            // Draw pursuer paths
            foreach (Pursuer pursuer in Pursuers)
            {
                pursuer.DrawPath(spriteBatch);
            }
            // Draw pursuer on top of paths
            foreach (Pursuer pursuer in Pursuers)
            {
                pursuer.Draw(spriteBatch);
            }

            if(Goal != null)
                Goal.Draw(spriteBatch);

            foreach (Pursuer pursuer in Pursuers)
            {
                if (pursuer.ShowStats)
                    pursuer.DrawStats(spriteBatch);
            }

            spriteBatch.End();

            _graphics.SetRenderTarget(null);

            NeedsUpdate = false;
        }

        public void Run(GameTime gameTime)
        {
            foreach (Pursuer pursuer in Pursuers)
            {
                if (!pursuer.HasRunAlgorithms)
                {
                    pursuer.RunAlgorithm(this, Goal.Coordinates);
                }

                pursuer.Update(gameTime);
                pursuer.UpdateSearchCells(_cursor);
                if (pursuer.PathUpdated)
                {
                    pursuer.PathUpdated = false;
                    NeedsUpdate = true;
                }
            }
        }
        /// <summary>
        /// Toggles the map to run through the pursuers algorithms to find the path to
        /// </summary>
        /// <returns></returns>
        public string ToggleRun()
        {
            if (Running)
            {
                Stop();
            }
            else
            {
                if (Goal == null)
                {
                    return "Goal needs to be placed";
                }
                if (Pursuers.Count == 0)
                {
                    return "Place a pursuer on the map";
                }

                Running = true;
                foreach (Pursuer pursuer in Pursuers)
                {
                    pursuer.Running = true;
                }
            }

            return "";
        }
        public void Stop()
        {
            Running = false;
            foreach (Pursuer pursuer in Pursuers)
            {
                pursuer.Reset();
            }
            NeedsUpdate = true;
        }

        /// <summary>
        /// Resets map back to original clean state
        /// </summary>
        public void Reset()
        {
            ResetMapToOriginal();
            Pursuers.Clear();
            Goal = null;
            NeedsUpdate = true;
        }
        /// <summary>
        /// Highlights floor cell at given coordinate
        /// </summary>
        /// <param name="coordinates"></param>
        public void HighlightCell(Point coordinate)
        {
            Cell cell = _mapCells.SelectMany(l => l).FirstOrDefault(c => c.Coordinates == coordinate);

            if(cell != null)
            {
                cell.Highlight();
                NeedsUpdate = true;
            }
        }
        public void UnHighlightCell(Point coordinate)
        {
            Cell cell = _mapCells.SelectMany(l => l).FirstOrDefault(c => c.Coordinates == coordinate);

            if (cell != null)
            {
                cell.UnHighlight();
                NeedsUpdate = true;
            }
        }
        /// <summary>
        /// Checks to see if cursor is hovered over floor cell
        /// Highlights cell if it is
        /// </summary>
        /// <param name="cell"></param>
        public void CheckForHighlight(Cell cell)
        {
            if (cell.Bounds.Intersects(_cursor.ClickBounds))
            {
                if (!cell.Highlighted)
                {
                    cell.Highlight();
                    NeedsUpdate = true;
                    _highlightedCell = cell;
                }

                CheckForItemPlacement(cell);
            }
            else
            {
                if (cell.Highlighted)
                {
                    cell.UnHighlight();
                    NeedsUpdate = true;
                    _highlightedCell = null;
                }
            }
        }
        public void CheckForItemPlacement(Cell cell)
        {
            if (_cursor.HeldItem != null && _cursor.HasClicked())
            {
                if (_cursor.HeldItem.GetType() == typeof(Pursuer))
                {
                    // If we have too many pursuers we don't bother adding more
                    if (Pursuers.Count >= MAX_PURSUERS)
                        return;

                    if (_cursor.HasClickedOn(cell.Bounds))
                    {
                        CanPlaceCell(cell);
                    }

                    return;
                }
                else if (_cursor.HeldItem.GetType() == typeof(Goal))
                {
                    if (_cursor.HasClickedOn(cell.Bounds))
                    {
                        CanPlaceCell(cell);
                    }
                    return;
                }
                else if (_cursor.HeldItem.GetType() == typeof(Cell))
                {
                    // If held cell is the same as checked cell then we just return
                    Cell check = _cursor.HeldItem as Cell;
                    if (cell.CellType == check.CellType)
                        return;

                    if (check.CellType == CellType.Wall)
                    {
                        // If pursuer is on this block then we can't place a wall
                        if (Pursuers.FirstOrDefault(p => p.Coordinates == cell.Coordinates) != null)
                            return;
                        // If goal is on this block then we can't place a wall
                        if (Goal != null && Goal.Coordinates == cell.Coordinates)
                            return;
                    }

                }

                if (_cursor.HasHeldClickOn(cell.Bounds))
                {
                    CanPlaceCell(cell);
                }

            }
        }
        public bool IsCellAWall(Point coords)
        {
            return MapKeys[coords.Y][coords.X] == '*';
            //return _mapCells[coords.Y][coords.X].CellType == CellType.Wall;
        }
        /// <summary>
        /// Checks to see if cursor is hovering over map
        /// </summary>
        private void CheckForCursorOnMap()
        {
            if (_cursor.ClickBounds.Intersects(Bounds))
                _cursorOnMap = true;
            else
            {
                _cursorOnMap = false;
                // Unhighlight any highlighted cells if cursor is not hovering over map
                List<Cell> highlighted = _mapCells.SelectMany(l => l.Where(c => c.Highlighted == true)).ToList();
                foreach (Cell cell in highlighted)
                {
                    cell.UnHighlight();
                    NeedsUpdate = true;
                }
            }
        }
        /// <summary>
        /// Checks to see if cursor is hovered over floor cell
        /// Highlights cell if it is
        /// </summary>
        private void CheckForCursorHighlight()
        {
            if (!_cursorOnMap)
            {
                return;
            }

            for (int i = 0; i < _mapCells.Count; i++)
            {
                for (int j = 0; j < _mapCells[i].Count; j++)
                {
                    CheckForHighlight(_mapCells[i][j]);
                }
            }
        }
        /// <summary>
        /// Checks to see if cell is allowed to be placed on cell
        /// </summary>
        /// <param name="cell"></param>
        /// 
        private void CanPlaceCell(Cell cell)
        {
            if (cell.Coordinates.Y > 0 && cell.Coordinates.Y < _mapCells.Count-1)
            {
                if (cell.Coordinates.X > 0 && cell.Coordinates.X < _mapCells[0].Count-1)
                {
                    // Isn't a border cell so that cool

                    if(_cursor.HeldItem.GetType() == typeof(Pursuer))
                    {
                        // We can't place a pursuer on a wall
                        if (cell.CellType == CellType.Wall) 
                            return;
                        // If pursuer already exists at this point
                        if (DoesPursuerExistAtCoordinates(cell.Coordinates)) 
                            return;

                        Pursuer tmp = (Pursuer)_cursor.HeldItem.Copy();
                        SetNewCellParams(tmp, cell);
                        tmp.SetTarget(_graphics, _spriteBatch);
                        Pursuers.Add(tmp);
                        _cursor.UpdatePursuerCount(Pursuers.Count);
                        NeedsUpdate = true;
                    }
                    else if(_cursor.HeldItem.GetType() == typeof(Cell))
                    {
                        // Replace cell with held cell
                        Cell tmp = (Cell)_cursor.HeldItem.Copy();
                        SetNewCellParams(tmp, cell);
                        _mapCells[cell.Coordinates.Y][cell.Coordinates.X] = tmp;

                        if (tmp.CellType == CellType.Floor) 
                            MapKeys[cell.Coordinates.Y][cell.Coordinates.X] = '.';
                        else 
                            MapKeys[cell.Coordinates.Y][cell.Coordinates.X] = '*';

                        NeedsUpdate = true;
                    }
                    else if(_cursor.HeldItem.GetType() == typeof(Goal))
                    {
                        // We can't place a pursuer on a wall
                        if (cell.CellType == CellType.Wall)
                            return;
                        // If pursuer already exists at this point
                        if (DoesPursuerExistAtCoordinates(cell.Coordinates))
                            return;

                        Goal goal = (Goal)_cursor.HeldItem.Copy();
                        SetNewCellParams(goal, cell);
                        Goal = goal;
                        MoveGoal();
                        NeedsUpdate = true;
                    }
                    else
                    {
                        // Eraser
                        if (cell.CellType == CellType.Wall)
                        {
                            cell.ChangeCellType(CellType.Floor);
                            MapKeys[cell.Coordinates.Y][cell.Coordinates.X] = '.';
                            NeedsUpdate = true;
                            return;
                        }

                        Pursuer pursuer = Pursuers.FirstOrDefault(p => p.Coordinates == cell.Coordinates);
                        if(pursuer != null)
                        {
                            Pursuers.Remove(pursuer);
                            NeedsUpdate = true;
                            _cursor.UpdatePursuerCount(Pursuers.Count);
                            return;
                        }

                        if(Goal != null && cell.Coordinates == Goal.Coordinates)
                        {
                            Goal = null;
                            NeedsUpdate = true;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Sets the new incomming sprite to the map with the old sprite parameters
        /// </summary>
        /// <param name="newSprite"></param>
        /// <param name="oldSprite"></param>
        private void SetNewCellParams(Sprite newSprite, Sprite oldSprite)
        {
            newSprite.Position = oldSprite.Position;
            newSprite.Coordinates = oldSprite.Coordinates;
            newSprite.Bounds = oldSprite.Bounds;
        }
        /// <summary>
        /// Checks to see if user hovered over pursuer during pathfinding
        /// Pushes pursuer to top of draw call
        /// </summary>
        private void CheckForPursuerHover()
        {
            foreach (Pursuer pursuer in Pursuers)
            {
                if (_cursor.ClickBounds.Intersects(pursuer.Bounds))
                {
                    MovePursuerToTop(pursuer);

                    if (pursuer.StatsNeedUpdate) 
                    { 
                        pursuer.ShowStats = true;
                        NeedsUpdate = true;
                    }
                    return;
                }
                else
                {
                    if (pursuer.ShowStats) pursuer.ShowStats = false;
                    NeedsUpdate = true;
                }
            }
        }
        private void CheckForPursuerStats()
        {
            foreach (Pursuer pursuer in Pursuers)
            {
                if (pursuer.StatsNeedUpdate)
                {
                    pursuer.DrawStatsToTarget(_graphics, _spriteBatch, this);
                }
            }
        }
        /// <summary>
        /// Returns true if Pursuer already exists at given coordinates
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool DoesPursuerExistAtCoordinates(Point point)
        {
            return Pursuers.Find(p => p.Coordinates == point) != null;
        }
        /// <summary>
        /// Reads in map data from text file and converts it to 2D array
        /// </summary>
        /// <param name="filePath"></param>
        private void GenerateMapFromFile(string filePath)
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines(filePath);

                // Convert text data into 2d char array
                MapKeys = lines.Select(line => line.ToArray()).ToArray();
                InitializeTarget();
            }
            catch (Exception e)
            {
                // Log error
                System.Diagnostics.Debug.Write("Error with generating map from text file! ", e.ToString());

            }

        }
        /// <summary>
        /// Initialize render target to the size of the map
        /// </summary>
        private void InitializeTarget()
        {
            int height = MapKeys.Length * 16;
            int width = MapKeys[0].Length * 16;

            if(_graphics != null)
                MapTarget = new RenderTarget2D(_graphics, width, height);
        }
        /// <summary>
        /// Sets maps graphical data from key data
        /// </summary>
        private void ResetMapToOriginal()
        {
            for (int i = 1; i < MapKeys.Length-1; i++)
            {
                for (int j = 1; j < MapKeys[i].Length-1; j++)
                {
                    if (_mapCells[i][j].CellType == CellType.Wall)
                    {
                        _mapCells[i][j].ChangeCellType(CellType.Floor);
                        MapKeys[i][j] = '.';
                    }
                }
            }
        }

        private void MoveGoal()
        {
            foreach (Pursuer pursuer in Pursuers)
            {
                pursuer.Reset();
            }
        }

        /// <summary>
        /// Moves pursuer to be drawn on top
        /// </summary>
        /// <param name="pursuer"></param>
        private void MovePursuerToTop(Pursuer pursuer)
        {
            if(Pursuers.Count > 1)
            {
                int thisIndex = Pursuers.IndexOf(pursuer);
                if(thisIndex != Pursuers.Count - 1)
                {
                    Pursuer topPursuer = Pursuers.Last();

                    Pursuers[Pursuers.Count - 1] = pursuer;
                    Pursuers[thisIndex] = topPursuer;
                }
            }
            
        }

    }
}
