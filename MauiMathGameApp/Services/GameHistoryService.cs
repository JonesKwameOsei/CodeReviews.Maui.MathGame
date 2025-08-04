using MauiMathGameApp.Models;
using System.Text.Json;

namespace MauiMathGameApp.Services
{
  public class GameHistoryService
  {
    private const string GAME_HISTORY_KEY = "game_history";
    private static int _nextId = 1;

    public async Task SaveGameAsync(string gameType, int score, int totalQuestions)
    {
      // * Convert starting game type to enum && Safe enum conversion
      if (!Enum.TryParse<GameOperation>(gameType, out GameOperation operation))
      {
        throw new ArgumentException($"Invalid game type: {gameType}");
      }

      // * Create game result
      Game game = new Game
      {
        Id = _nextId++,
        Type = operation,
        Score = score,
        TotalQuestions = totalQuestions,
        DatePlayed = DateTime.Now
      };

      // * Get existing games
      var games = await GetGameHistoryAsync();

      // * Add new game
      games.Add(game);

      // * Save back to preferences && Convert objects to JSON
      var json = JsonSerializer.Serialize(games);
      Preferences.Set(GAME_HISTORY_KEY, json);
    }

    public async Task<List<Game>> GetGameHistoryAsync()
    {
      await Task.Delay(1);
      var json = Preferences.Get(GAME_HISTORY_KEY, string.Empty);

      if (string.IsNullOrEmpty(json))
      {
        return new List<Game>();
      }

      try
      {
        var games = JsonSerializer.Deserialize<List<Game>>(json) ?? new List<Game>();

        if (games.Any())
        {
          _nextId = games.Max(g => g.Id) + 1;
        }

        return games.OrderByDescending(g => g.DatePlayed).ToList();
      }
      catch (JsonException)
      {
        Preferences.Remove(GAME_HISTORY_KEY);
        return new List<Game>();
      }
    }

    public async Task DeleteGameAsync(int gameId)
    {
      var games = await GetGameHistoryAsync();
      var gameToRemove = games.FirstOrDefault(g => g.Id == gameId);

      if (gameToRemove != null)
      {
        games.Remove(gameToRemove);

        // Save updated list back to preferences
        var json = JsonSerializer.Serialize(games);
        Preferences.Set(GAME_HISTORY_KEY, json);
      }
    }

    public void ClearHistory()
    {
      Preferences.Remove(GAME_HISTORY_KEY);
      _nextId = 1;
    }

    public async Task<GameStatistics> GetStatisticsAsync()
    {
      var games = await GetGameHistoryAsync();

      return new GameStatistics
      {
        TotalGames = games.Count,
        AverageScore = games.Any() ? games.Average(g => g.PercentageScore) : 0,
        BestScore = games.Any() ? games.Max(g => g.PercentageScore) : 0,
        FavouriteGameType = games.GroupBy(g => g.Type)
                               .OrderByDescending(g => g.Count())
                               .FirstOrDefault()?.Key.ToString() ?? "None"
      };
    }
  }

  public class GameStatistics
  {
    public int TotalGames { get; set; }
    public double AverageScore { get; set; }
    public double BestScore { get; set; }
    public string FavouriteGameType { get; set; } = string.Empty;
  }
}