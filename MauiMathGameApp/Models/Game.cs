using System.Text.Json.Serialization;

namespace MauiMathGameApp.Models
{
  public enum GameOperation
  {
    Addition,
    Subtraction,
    Multiplication,
    Division
  }

  public class Game
  {
    public int Id { get; set; }

    [JsonPropertyName("type")]
    public GameOperation Type { get; set; }

    [JsonPropertyName("score")]
    public int Score { get; set; }

    [JsonPropertyName("totalQuestions")]
    public int TotalQuestions { get; set; }

    [JsonPropertyName("datePlayed")]
    public DateTime DatePlayed { get; set; }

    [JsonIgnore]
    public double PercentageScore => TotalQuestions > 0 ? (double)Score / TotalQuestions * 100 : 0;

    [JsonIgnore]
    public string GameTypeDisplay => Type.ToString();
  }
}
