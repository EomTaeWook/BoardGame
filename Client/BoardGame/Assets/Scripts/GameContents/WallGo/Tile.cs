using Assets.Scripts.GameContents.Share;

namespace Assets.Scripts.GameContents.WallGo
{
    public class Tile
    {
        public Point GridPos { get; set; }

        public bool WallRight { get; set; }
        public bool WallLeft { get; set; }
        public bool WallTop { get; set; }
        public bool WallBottom { get; set; }

        public bool CanMove(Tile from, Tile to)
        {
            Point diff = to.GridPos - from.GridPos;

            if (diff == Point.Right)
            {
                return !from.WallRight && !to.WallLeft;
            }
            else if (diff == Point.Left)
            {
                return !from.WallLeft && !to.WallRight;
            }
            else if (diff == Point.Up)
            {
                return !from.WallTop && !to.WallBottom;
            }
            else if (diff == Point.Down)
            {
                return !from.WallBottom && !to.WallTop;
            }
            return false;
        }
    }
}
