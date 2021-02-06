using System.Collections.Generic;
using System.Drawing;

namespace PathfindingApp
{
    public static class Algorithms
    {
        private static readonly Dictionary<string, Point> moveOffset = new Dictionary<string, Point>()
        {
            { "left", new Point(0, -1) },
            { "right", new Point(0, 1) },
            { "up", new Point(-1, 0) },
            { "down", new Point(1, 0) }
        };
        private static readonly string[] directions = { "up", "down", "left", "right" };



        /// <summary>
        /// Uses Depth First Search to find the goal
        /// </summary>
        /// <param name="map"></param>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static List<Point> DFS(Map map, Point start, Point goal)
        {
            Stack<Point> points = new Stack<Point>();
            points.Push(start);
            Dictionary<Point, Point?> predecessors = new Dictionary<Point, Point?>()
            {
                { start, null }
            };

            while(points.Count != 0)
            {
                Point current = points.Pop();
                if(current == goal)
                {
                    return GetPath(start, goal, predecessors);
                }

                foreach (var direction in directions)
                {
                    Point offset = moveOffset[direction];
                    Point neighbor = new Point(current.X + offset.X, current.Y + offset.Y);
                    if (IsValidMove(map, neighbor) &&
                        !predecessors.ContainsKey(neighbor))
                    {
                        points.Push(neighbor);
                        predecessors.Add(neighbor, current);
                    }

                }
            }

            return null;
        }

        /// <summary>
        /// Uses Breadth First Search to find the goal
        /// </summary>
        /// <param name="map"></param>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static List<Point> BFS(Map map, Point start, Point goal)
        {
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(start);
            Dictionary<Point, Point?> predecessors = new Dictionary<Point, Point?>();
            predecessors.Add(start, null);

            while(queue.Count > 0)
            {
                Point current = queue.Dequeue();
                if(current == goal)
                {
                    return GetPath(start, goal, predecessors);
                }
                else
                {
                    foreach (string dir in directions)
                    {
                        Point offset = moveOffset[dir];
                        Point neighbor = new Point(current.X + offset.X, current.Y + offset.Y);
                        if(IsValidMove(map, neighbor) && !predecessors.ContainsKey(neighbor))
                        {
                            queue.Enqueue(neighbor);
                            predecessors.Add(neighbor, current);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Checks map to see if the move is not a wall
        /// </summary>
        /// <param name="map"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        private static bool IsValidMove(Map map, Point move)
        {
            return move.X >= 0 && move.X <= map.MapKeys.Length && move.Y >= 0 && move.Y <= map.MapKeys[0].Length && map.MapKeys[move.Y][move.X] != '*';
        }

        /// <summary>
        /// Returns the path taken from the starting point to the end point
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="predecessors"></param> 
        /// <returns></returns>
        private static List<Point> GetPath(Point start, Point end, Dictionary<Point, Point?> predecessors)
        {
            List<Point> path = new List<Point>();
            Point current = end;

            while (end != start)
            {
                path.Add(current);
                current = (Point)predecessors[current];
            }

            path.Add(start);
            path.Reverse();

            return path;
        }
    }
}
