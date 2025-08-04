namespace MauiMathGameApp;

public partial class MainPage : ContentPage
{
  public MainPage()
  {
    InitializeComponent();
  }

  private void OnGameChosen(object sender, EventArgs e)
  {
    if (e is TappedEventArgs tappedEventArgs && tappedEventArgs.Parameter is string selectedGameType)
    {
      Navigation.PushAsync(new GameSetUpPage(selectedGameType));
    }
  }

  private void OnViewPreviousGamesChosen(object sender, EventArgs e)
  {
    Navigation.PushAsync(new PreviousGames());
  }
}
