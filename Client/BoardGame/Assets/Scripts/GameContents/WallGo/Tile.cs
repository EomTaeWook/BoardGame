using Assets.Scripts.GameContents.Share;

namespace Assets.Scripts.GameContents.WallGo
{
    public class Tile
    {
        public Point GridPosition { get; set; }

        public bool WallRight { get; set; }
        public bool WallLeft { get; set; }
        public bool WallTop { get; set; }
        public bool WallBottom { get; set; }

        public bool CanMove(Tile from, Tile to)
        {
            Point diff = to.GridPosition - from.GridPosition;

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
        public bool HasWall(Direction direction)
        {
            if(direction == Direction.Down)
            {
                return WallBottom;
            }
            else if(direction == Direction.Left)
            {
                return WallLeft;
            }
            else if(direction == Direction.Right)
            {
                return WallRight;
            }
            else if(direction == Direction.Up)
            {
                return WallTop;
            }
            return false;
        }
    }
}
