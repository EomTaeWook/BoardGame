using Assets.Scripts.GameContents.WallGo;
using Dignus.Unity.Framework;
using System.Collections.Generic;

namespace Assets.Scripts.Scene.WallGo
{
    public class WallGoSceneModel : ISceneModel
    {
        public List<WallGoPlayer> Players { get; set; }

        public WallGoPlayer CurrentPlayer { get; set; }
    }
}
