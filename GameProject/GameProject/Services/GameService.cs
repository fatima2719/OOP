using GameProject.Models;
using GameProject.Repositories;
using System;
using System.Collections.Generic;

namespace GameProject.Services
{
    // The service contains rules and calculations, but no Console code.
    public class GameService
    {
        private readonly ScoreRepository _repository;
        private readonly Random _random = new Random();

        public GameService(ScoreRepository repository) { _repository = repository; }
        public Player CreatePlayer(string name) { return new Player(name); }

        public List<Monster> CreateMonsters()
        {
            List<Monster> monsters = new List<Monster>();
            monsters.Add(new Monster("Monster-1", 40, 5, 10));
            monsters.Add(new Monster("Monster-2", 65, 20, 30));
            monsters.Add(new Monster("Monster-3", 70, 30, 50));
            return monsters;
        }

        public int PlayerAttack(Player player, Monster monster)
        {
            int damage = CalculateDamage(player.AttackPower);
            monster.TakeDamage(damage);
            return damage;
        }

        public int MonsterAttack(Monster monster, Player player)
        {
            int damage = CalculateDamage(monster.AttackPower);
            player.TakeDamage(damage);
            return damage;
        }

        private int CalculateDamage(int attackPower)
        {
            return _random.Next(attackPower - 3, attackPower + 4);
        }

        public void GiveMonsterReward(Player player, Monster monster)
        {
            player.AddScore(monster.ScoreValue);
        }

        public int CalculateVictoryBonus(Player player)
        {
           
            if (player == null)
            {
                return 0;
            }

           
            int potionBonus = player.Health * 10;

           
            int totalBonus = player.Health + potionBonus;

            return totalBonus;
        }

        public void SaveResult(Player player, bool won)
        {
            string result = won ? "Won" : "Lost";
            _repository.Save(new GameResult(player.Name, player.Score, result));
        }

        public List<GameResult> GetScores() { return _repository.GetAll(); }

        public List<GameResult> GetTopScores(int numberOfScores)
        {
            
            List<GameResult> allResults = _repository.GetAll();

            if (allResults == null || allResults.Count == 0 || numberOfScores <= 0)
            {
                return new List<GameResult>();
            }

            List<GameResult> workingList = new List<GameResult>();
            for (int i = 0; i < allResults.Count; i++)
            {
                workingList.Add(allResults[i]);
            }

            for (int i = 0; i < workingList.Count - 1; i++)
            {
                int bestIndex = i;

                for (int j = i + 1; j < workingList.Count; j++)
                {
                    GameResult currentBest = workingList[bestIndex];
                    GameResult candidate = workingList[j];

                    if (candidate.Score > currentBest.Score)
                    {
                        bestIndex = j;
                    }
                   
                    else if (candidate.Score == currentBest.Score)
                    {
                       
                        if (string.Compare(candidate.PlayerName, currentBest.PlayerName, true) < 0)
                        {
                            bestIndex = j;
                        }
                    }
                }

                if (bestIndex != i)
                {
                    GameResult temp = workingList[i];
                    workingList[i] = workingList[bestIndex];
                    workingList[bestIndex] = temp;
                }
            }

            int limit = numberOfScores;
            if (workingList.Count < limit)
            {
                limit = workingList.Count;
            }

            List<GameResult> topResults = new List<GameResult>();
            for (int i = 0; i < limit; i++)
            {
                topResults.Add(workingList[i]);
            }

            return topResults;
        }
    }
}
