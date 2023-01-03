using System.Text;

using IdentityModel.OidcClient;


namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        readonly OidcClient client;
        string currentAccessToken;

        public MainPage(OidcClient client)
        {
            this.client = client;
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            var result = await client.LoginAsync();

            if (result.IsError)
            {
                editor.Text = result.Error;
                return;
            }

            currentAccessToken = result.AccessToken;

            var sb = new StringBuilder(128);

            sb.AppendLine("claims:");
            foreach (var claim in result.User.Claims)
            {
                sb.AppendLine($"{claim.Type}: {claim.Value}");
            }

            sb.AppendLine();
            sb.AppendLine("access token:");
            sb.AppendLine(result.AccessToken);

            if (!string.IsNullOrWhiteSpace(result.RefreshToken))
            {
                sb.AppendLine();
                sb.AppendLine("access token:");
                sb.AppendLine(result.AccessToken);
            }

            editor.Text = sb.ToString();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}