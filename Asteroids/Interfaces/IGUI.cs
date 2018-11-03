using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Asteroids
{
    public interface IGUI
    {
        void UpdateLaserCharges(int count);
        void UpdateScore(int score);
    }
}
