using Assets.Scripts.GameContents.Share;

namespace Assets.Scripts.GameContents.WallGo
{
    public class Piece
    {
        public int Id { get; private set; }
        public IPlayer Owner { get; private set; }
        public Point GridPos { get; set; }
        public bool Spawned { get; private set; }

        public Piece(int id, IPlayer player)
        {
            Id = id;
            Owner = player;
        }
        public void SetSpawn(Point spawnPoint)
        {
            this.GridPos = spawnPoint;
            Spawned = true;
        }
    }
}
