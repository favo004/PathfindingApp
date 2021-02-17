using Microsoft.Xna.Framework;
using Priority_Queue;
using System;
using System.Collections.Generic;

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
        private static readonly string[] directions = { "up", "right", "down", "left" };

        /// <summary>
        /// Uses Depth First Search to find the goal
        /// </summary>
        /// <param name="map"></param>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public static List<Point> DFS(Map map, Point start, Point goal, List<Point> movedTo)
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

                if (movedTo != null)
                    movedTo.Add(current);

                if (current == goal)
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
        public static List<Point> BFS(Map map, Point start, Point goal, List<Point> movedTo)
        {
            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(start);
            Dictionary<Point, Point?> predecessors = new Dictionary<Point, Point?>();
            predecessors.Add(start, null);

            while(queue.Count > 0)
            {
                Point current = queue.Dequeue();

                if(movedTo != null)
                    movedTo.Add(current);

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
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="start"></param>
        /// <param name="goal"></param>
        /// <param name="movedTo"></param>
        /// <returns></returns>
        public static List<Point>AStar(Map map, Point start, Point goal, List<Point> movedTo)
        {
            SimplePriorityQueue<Point> pq = new SimplePriorityQueue<Point>();
            pq.Enqueue(start, 0);
            Dictionary<Point, Point?> predecessors = new Dictionary<Point, Point?>();
            predecessors.Add(start, null);
            Dictionary<Point, int> gValues = new Dictionary<Point, int>();
            gValues.Add(start, 0);

            int cost = 0;
            int fValue = 0;

            while(pq.Count != 0)
            {
                Point current = pq.Dequeue();
                if(movedTo != null)
                    movedTo.Add(current);

                if(current == goal)
                {
                    return GetPath(start, goal, predecessors);
                }

                foreach (string dir in directions)
                {
                    Point offset = moveOffset[dir];
                    Point neighbor = new Point(current.X + offset.X, current.Y + offset.Y);
                    if (IsValidMove(map, neighbor) && !predecessors.ContainsKey(neighbor))
                    {
                        cost = gValues[current] + 1;
                        gValues.Add(neighbor, cost);
                        fValue = cost + DistanceFromPoints(goal, neighbor);
                        pq.Enqueue(neighbor, fValue);
                        predecessors.Add(neighbor, current);
                    }
                }
            }

            return null;
        }

        // Helpers

        /// <summary>
        /// Checks map to see if the move is not a wall
        /// </summary>
        /// <param name="map"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        private static bool IsValidMove(Map map, Point move)
        {
            return move.X >= 0 && move.X <= map.MapKeys.Length && move.Y >= 0 && move.Y <= map.MapKeys[0].Length && !map.IsCellAWall(move);
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

            while (current != start)
            {
                path.Add(current);

                current = (Point)predecessors[current];
            }

            path.Add(start);
            path.Reverse();

            return path;
        }

        /// <summary>
        /// Returns heuristic distance from two points
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        private static int DistanceFromPoints(Point from, Point to)
        {
            Point diff = from - to;
            return (int)(Math.Pow(diff.X, 2) + Math.Pow(diff.Y, 2));
        }
    }

    //Create node class for PriorityQueue
    public class PointNode : FastPriorityQueueNode
    {

    }
}
