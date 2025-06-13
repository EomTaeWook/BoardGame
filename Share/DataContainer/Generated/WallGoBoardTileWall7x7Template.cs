using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DataContainer;

namespace DataContainer.Generated
{
    public partial class WallGoBoardTileWall7x7Template : TemplateBase
    {
        public float X { get; set; }
        public List<float> Y { get; set; } = new List<float>();
        public DataContainer.TileWallDirectionType TileWallDirectionType { get; set; }
    }
}