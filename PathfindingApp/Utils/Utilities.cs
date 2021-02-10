using System;
using System.Collections.Generic;
using System.Text;

namespace PathfindingApp.Utils
{
    public class Utilities
    {
    }
    
    public class Timer
    {
        public bool Finished;

        float _maxTime;
        float _currentTime;

        public Timer(float maxTime)
        {
            _maxTime = maxTime;
        }

        public void AddTime(float elapsed)
        {
            _currentTime += elapsed;

            if (_currentTime >= _maxTime)
                Finished = true;
        }
        public void Reset()
        {
            _currentTime = 0f;
            Finished = false;
        }
    }
}
