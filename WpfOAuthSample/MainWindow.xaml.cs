using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfOAuthSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IPublicClientApplication _app;
        private readonly HttpClientFactory _httpClientFactory = new HttpClientFactory();
        private string _token;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Autenticate and get token

            var clientId = clientIdTextBox.Text;
            var tenantId = tenantIdTextBox.Text;

            _app = PublicClientApplicationBuilder
                .Create(clientId)
                .WithTenantId(tenantId)
                .WithDefaultRedirectUri()
                .WithHttpClientFactory(_httpClientFactory)
                .Build();

            var scopes = new string[] { "openid", "profile", "email", "offline_access" };

            var result = _app.AcquireTokenInteractive(scopes)
                        .ExecuteAsync()
                        .Result;

            _token = result.AccessToken;

            if (!result.Account.Username.EndsWith("@becomex.com.br"))
            {
                MessageBox.Show("Invalid domain!", "Alert", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            // Get user from token

            var httpClient = _httpClientFactory.GetHttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            HttpResponseMessage response = httpClient.GetAsync("https://graph.microsoft.com/oidc/userinfo").Result;

            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                dynamic responseObject = JsonConvert.DeserializeObject(content);
                var email = responseObject.email.Value;
            }
        }
    }
}
