using Microsoft.Xna.Framework;
using PathfindingApp;
using PathfindingApp.Managers;
using System;
using System.Collections.Generic;
using Xunit;

namespace PathfindingTest
{
    public class AlgoTests
    {

        [Fact]
        public void AStarTest()
        {
            var _config = new Config();
            var map = new Map(_config.MapPath);

            List<Point> expectedReturn = new List<Point> {
                new Point(1, 1),
                new Point(2, 1),
                new Point(3, 1),
                new Point(4, 1),
                new Point(5, 1)
            };

            Assert.Equal(expectedReturn, Algorithms.AStar(map, new Point(1, 1), new Point(5, 1), null));
        }

        [Fact]
        public void BFSTest()
        {
            var _config = new Config();
            var map = new Map(_config.MapPath);

            List<Point> expectedReturn = new List<Point> {
                new Point(8, 5),
                new Point(8, 6),
                new Point(8, 7),
                new Point(9, 7),
                new Point(10, 7),
                new Point(11, 7),
                new Point(12, 7)
            };

            Assert.Equal(expectedReturn, Algorithms.BFS(map, new Point(8, 5), new Point(12, 7), null));
        }

        [Fact]
        public void DFSTest()
        {
            var _config = new Config();
            var map = new Map(_config.MapPath);

            List<Point> expectedReturn = new List<Point> {
                new Point(8, 5),
                new Point(8, 6),
                new Point(8, 7),
                new Point(9, 7),
                new Point(10, 7),
                new Point(11, 7),
                new Point(12, 7)
            };

            Assert.NotEqual(expectedReturn, Algorithms.DFS(map, new Point(8, 5), new Point(12, 7), null));
        }
    }
}
