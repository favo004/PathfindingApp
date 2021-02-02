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
    public class Map
    {
        char[][] _map;

        public Map(string filePath)
        {
            GenerateMapFromFile(filePath);
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
                for (int i = 0; i < lines.Length; i++)
                {
                    for (int j = 0; j < lines[i].Length; j++)
                    {
                        _map[i][j] = lines[i][j];
                    }
                }

                // Simplified with LINQ
                _map = lines.Select(line => line.ToArray()).ToArray();

            }
            catch (Exception e)
            {
                // Log error
                System.Diagnostics.Debug.Write("Error with generating map from text file! ", filePath);
            }

        }
    }
}
