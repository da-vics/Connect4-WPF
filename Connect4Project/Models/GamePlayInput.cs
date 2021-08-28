namespace Connect4Project.Models
{
    public struct GamePlayInput
    {
        public string Player1 { get; set; }
        public string Player2 { get; set; }

        public GamePlayInput(string player1, string player2)
        {
            Player1 = player1;
            Player2 = player2;
        }
    }
}
