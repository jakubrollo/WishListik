using System.Net.Http;
using System.Collections.ObjectModel;
using System.Text.Json;
using WishListik.Models;

namespace WishListik
{
    public partial class MainPage : ContentPage
    {
        private readonly HttpClient _httpClient;

        public ObservableCollection<Item> Items { get; set; }

        public MainPage()
        {
            InitializeComponent();

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _httpClient = new HttpClient(handler);

            Items = new ObservableCollection<Item>();

            BindingContext = this;

            LoadItems();
        }

        private async void LoadItems()
        {
            try
            {
                var response = await _httpClient.GetStringAsync("https://10.0.2.2:7191/api/items");
                //var response = await _httpClient.GetStringAsync("https://localhost:7191/api/items");

                var items = JsonSerializer.Deserialize<List<Item>>(response);

                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to load items: {ex.Message}", "OK");
            }
        }
    }
}
