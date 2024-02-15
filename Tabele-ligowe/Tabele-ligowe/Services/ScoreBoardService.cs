﻿using Tabele_ligowe.Models;

namespace Tabele_ligowe.Services
{
    public class ScoreBoardService
    {
        public ScoreBoardService() { }

        public TeamViewModel MapTeamViewModel(Team team)
        {
            var result = new TeamViewModel{ Name = team.Name };

            foreach (var match in team.HomeMatches)
            {
                result.GoalsScored += match.HomeTeamGoals;
                result.GoalsConceded += match.AwayTeamGoals;
                result.MatchesPlayed += 1;

                if (match.HomeTeamGoals > match.AwayTeamGoals) 
                {
                    result.Wins += 1;
                    result.Points += 3;
                }
                else if (match.HomeTeamGoals == match.AwayTeamGoals)
                {
                    result.Draws += 1;
                    result.Points += 1;
                }
                else
                {
                    result.Losses += 1;
                }
            }

            foreach (var match in team.AwayMatches)
            {
                result.GoalsScored += match.AwayTeamGoals;
                result.GoalsConceded += match.HomeTeamGoals;
                result.MatchesPlayed += 1;

                if (match.HomeTeamGoals < match.AwayTeamGoals) 
                {
                    result.Wins += 1;
                    result.Points += 3;
                }
                else if (match.HomeTeamGoals == match.AwayTeamGoals)
                {
                    result.Draws += 1;
                    result.Points += 1;
                }
                else
                {
                    result.Losses += 1;
                }
            }

            result.GoalsDifference = result.GoalsScored - result.GoalsConceded;

            return result;
        }
    }
}