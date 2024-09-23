using System;

namespace GameCore
{
    [Serializable]
    public class GameResult : IComparable<GameResult>
    {
        public string Time { get; private set; }
        public int Money { get; private set; }

        public GameResult(string time, int money)
        {
            Time = time;
            Money = money;
        }

        public int CompareTo(GameResult other)
        {
            int thisTimeInSeconds = ConvertTimeToSeconds(Time);
            int otherTimeInSeconds = ConvertTimeToSeconds(other.Time);
            int timeComparison = thisTimeInSeconds.CompareTo(otherTimeInSeconds);
            if (timeComparison != 0)
                return timeComparison;

            return other.Money.CompareTo(Money); 
        }

        private int ConvertTimeToSeconds(string time)
        {
            var parts = time.Split(':');
            int minutes = int.Parse(parts[0]);
            int seconds = int.Parse(parts[1]);
            return minutes * 60 + seconds;
        }
    }
}