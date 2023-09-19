using Newtonsoft.Json;

namespace Weather_App;

public partial class MainPage : ContentPage
{

	string apiKey = "b6011e05867a4a51ac2223658231608"; // Your API key goes here

	public MainPage()
	{
		InitializeComponent();
	}

	// A method that makes a call to the api to get the weather data
	private async void GetWeatherData(object sender, EventArgs e)
	{
		// Get the postal code from the entry field
		string postalCode = postalCodeEntry.Text;

		// Build the url given the postal code
		string url = $"https://api.weatherapi.com/v1/current.json?key={apiKey}&q={postalCode}&aqi=no";

		// Make the call to the api
		HttpClient client = new HttpClient();
		HttpResponseMessage response = await client.GetAsync(url);

		// If the response is successful, parse the JSON data
		if (response.IsSuccessStatusCode)
		{
			string json = await response.Content.ReadAsStringAsync();
			
			// Parse the JSON data
			dynamic data = JsonConvert.DeserializeObject(json);

			// Get the current temperature
			CurrentWeather.temp_c = data.current.temp_c;

			// Get the current condition
			CurrentWeather.condition = data.current.condition.text;

			// Get the current condition icon url
			string icon = data.current.condition.icon;

			// Make a call to the api to get the icon
			HttpClient iconClient = new HttpClient();
			HttpResponseMessage iconResponse = await iconClient.GetAsync($"https:{icon}");

			// If the response is successful, set the image source
			if (iconResponse.IsSuccessStatusCode)
			{
                weatherImage.Source = ImageSource.FromStream(() => iconResponse.Content.ReadAsStreamAsync().Result);
            }
			
			// Get the current feels like temperature
			CurrentWeather.feelslike_c = data.current.feelslike_c;

            // Update the label with the current temperature
            weatherLabel.Text = CurrentWeather.condition;

            temperatureLabel.Text = CurrentWeather.temp_c.ToString() + "°C" +
                                    " (Feels like " + CurrentWeather.feelslike_c + "°C)";


        }
        // If the response is unsuccessful, display an error message
        else
		{
			await DisplayAlert("Error", "Invalid postal code", "OK");
		}
    }

	// When the button is clicked, call the GetWeatherData method
	private void OnGetWeatherButtonClicked(object sender, EventArgs e)
	{
        GetWeatherData(sender, e);
    }

}

internal class CurrentWeather
{
	public static double temp_c { get; set; }
	public static string condition { get; set; }
	public static string feelslike_c { get; set; }
}