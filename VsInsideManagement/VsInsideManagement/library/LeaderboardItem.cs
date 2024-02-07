﻿namespace VsInsideManagement.library
{
    public class LeaderboardItem
    {
        public string Name { get; }
        public int Score { get; }

        public LeaderboardItem(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }
}