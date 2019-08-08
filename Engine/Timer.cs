using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class Timer
    {
        private DateTime currentTime;
        private DateTime previousTime;

        public Timer()
        {
            this.Reset();
        }

        /// <summary>
        /// Updates the timer
        /// 
        /// 
        /// 
        /// </summary>
        public void Update()
        {
            previousTime = currentTime;
            currentTime = DateTime.Now;
        }

        /// <summary>
        /// Resets the timer
        /// </summary>
        public void Reset()
        {
            previousTime = DateTime.Now;
            currentTime = DateTime.Now;
        }

        /// <summary>
        /// Gets time elapsed since last update
        /// </summary>
        /// <returns>A TimeSpan with the time elapsed since last update</returns>
        public TimeSpan GetDeltaTimeInSeconds()
        {
            return currentTime - previousTime;
        }

    }
}
