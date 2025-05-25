using Assets.Scripts.GameContents;

namespace Assets.Scripts.Internals
{
    public class GamePlayer : IPlayer
    {
        public string AccountId { get; set; }

        public string Nickname { get; set; }

        public bool IsHost { get; set; }
    }
}
