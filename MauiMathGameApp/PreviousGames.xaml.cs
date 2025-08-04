using System.Collections.ObjectModel;
using MauiMathGameApp.Models;
using MauiMathGameApp.Services;

namespace MauiMathGameApp;

public partial class PreviousGames : ContentPage
{
  private readonly GameHistoryService _gameHistoryService = new();
  public ObservableCollection<Game> GameHistory { get; set; } = new();

  public PreviousGames()
  {
    InitializeComponent();
    BindingContext = this; // Enable data binding
  }

  protected override async void OnAppearing()
  {
    base.OnAppearing();
    await LoadGameHistoryAsync();
  }

  private async Task LoadGameHistoryAsync()
  {
    try
    {
      var games = await _gameHistoryService.GetGameHistoryAsync();
      GameHistory.Clear();
      foreach (var game in games)
      {
        GameHistory.Add(game);
      }
      await UpdateStatisticsAsync();
      UpdateEmptyState();
    }
    catch (Exception ex)
    {
      await DisplayAlert("Error", $"Failed to load game history: {ex.Message}", "OK");
    }
  }

  private async Task UpdateStatisticsAsync()
  {
    try
    {
      var stats = await _gameHistoryService.GetStatisticsAsync();

      TotalGamesLabel.Text = $"Total Games Played: {stats.TotalGames}";
      AverageScoreLabel.Text = $"Average Score: {stats.AverageScore:F1}%";
      BestScoreLabel.Text = $"Best Score: {stats.BestScore:F1}%";
      FavouriteGameLabel.Text = $"Favorite Game: {stats.FavouriteGameType}";
    }
    catch (Exception ex)
    {
      System.Diagnostics.Debug.WriteLine($"Statistics error: {ex.Message}");
      TotalGamesLabel.Text = "Statistics unavailable";
      AverageScoreLabel.Text = "";
      BestScoreLabel.Text = "";
      FavouriteGameLabel.Text = "";
    }
  }

  private void UpdateEmptyState()
  {
    bool hasGames = GameHistory.Count > 0;
    EmptyStateLabel.IsVisible = !hasGames;
  }

  private async void OnDeleteGameClicked(object sender, EventArgs e)
  {
    if (sender is Button button && button.CommandParameter is Game gameToDelete)
    {
      bool confirm = await DisplayAlert(
          "Delete Game",
          $"Are you sure you want to delete this {gameToDelete.GameTypeDisplay} game?",
          "Delete",
          "Cancel"
      );

      if (confirm)
      {
        try
        {
          GameHistory.Remove(gameToDelete);

          await _gameHistoryService.DeleteGameAsync(gameToDelete.Id);

          UpdateEmptyState();
          await UpdateStatisticsAsync();

          await DisplayAlert("Success", "Game deleted successfully!", "OK");
        }
        catch (Exception ex)
        {
          await DisplayAlert("Error", $"Failed to delete game: {ex.Message}", "OK");
        }
      }
    }
  }

  public async Task RefreshAsync()
  {
    await LoadGameHistoryAsync();
  }
}