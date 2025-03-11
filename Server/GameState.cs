using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Collections.Specialized;
using Server;

namespace ConsoleApp1
{
    internal class GameState
    {
        public struct FlagState
        {
            public Vec3 position;
            public uint currentOwner;
        }

        public FlagState flagState;
        Vec3 flagDefaultPosition = new Vec3( 0.0f, 0.0f, 0.0f );
        public void resetFlagState()
        {
            flagState.position = flagDefaultPosition;
            flagState.currentOwner = 0;
        }

         public void updateFlagState(Vec3 newPos)
        {
            if (flagState.currentOwner != 0)
            {
                flagState.position = newPos;
            }
        }

    }
}
