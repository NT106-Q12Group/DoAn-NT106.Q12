using System;

namespace CaroGame_TCPServer.Player
{
    public class Player
    {
        public int PlayerID { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Birthday { get; set; }

        public Player() { }

        public Player(string name, string psw, string? email = null, string? birth = null)
        {
            PlayerName = name ?? throw new ArgumentNullException(nameof(name));
            Password = psw ?? throw new ArgumentNullException(nameof(psw));
            Email = email;
            Birthday = birth;
        }

        public Player(string name, string psw)
        {
            PlayerName = name ?? throw new ArgumentNullException(nameof(name));
            Password = psw ?? throw new ArgumentNullException(nameof(psw));
        }

        public override string ToString()
        {
            return $"PlayerID: {PlayerID}, Name: {PlayerName}, Email: {Email ?? "N/A"}, Birthday: {Birthday ?? "N/A"}";
        }
    }
}
