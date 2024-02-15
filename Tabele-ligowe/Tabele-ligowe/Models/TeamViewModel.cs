﻿namespace Tabele_ligowe.Models
{
    public class TeamViewModel
    {
        public string Name { get; set; }
        public int MatchesPlayed { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsScored { get; set; }
        public int GoalsConceded { get; set; }
        public int GoalsDifference { get; set; }
        public int Points { get; set; }
    }
}
