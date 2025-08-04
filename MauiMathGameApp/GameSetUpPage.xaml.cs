namespace MauiMathGameApp;

public partial class GameSetUpPage : ContentPage
{
  private readonly string? gameType;

  public GameSetUpPage(string selectedGameType)
  {
    InitializeComponent();
    gameType = selectedGameType;
    GameTypeLabel.Text = $"Selected Game Type: {gameType}";
  }

  private void OnStartGameClicked(object sender, EventArgs e)
  {
    if (string.IsNullOrEmpty(gameType))
    {
      DisplayAlert("Error", "Game type not set. Please return to main menu and select a game type.", "OK");
      return;
    }

    if (int.TryParse(QuestionsEntry.Text, out int numberOfQuestions) && numberOfQuestions > 0)
    {
      Navigation.PushAsync(new GamePage(gameType, numberOfQuestions));
    }
    else
    {
      DisplayAlert("Invalid Input", "Please enter a valid number of questions.", "OK");
    }
  }

  private void OnQuickSelectClicked(object sender, EventArgs e)
  {
    if (sender is Button button)
    {
      QuestionsEntry.Text = button.Text;
    }
  }

}