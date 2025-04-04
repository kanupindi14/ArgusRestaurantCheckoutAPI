using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using Microsoft.Playwright;

namespace RestaurantCheckoutTests.Hooks
{
    [Binding]
    public sealed class TestHooks
    {
        public static IPlaywright PlaywrightInstance { get; set; } = null!;
        public static IAPIRequestContext ApiContext { get; set; } = null!;
        public static TestConfig Config { get; set; } = null!;

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            // Load configuration from Utilities/config.json
            string configContent = File.ReadAllText("Utilities/config.json");
            Config = JsonSerializer.Deserialize<TestConfig>(configContent) ?? new TestConfig();
        }

        [AfterTestRun]
        public static async Task AfterTestRun()
        {
            if (ApiContext != null)
                await ApiContext.DisposeAsync();
            if (PlaywrightInstance != null)
                PlaywrightInstance.Dispose();
        }

        [BeforeScenario]
        public static async Task BeforeScenario()
        {
            PlaywrightInstance = await Playwright.CreateAsync();
            ApiContext = await PlaywrightInstance.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = Config.baseUrl
            });
        }

        [AfterScenario]
        public static async Task AfterScenario()
        {
            await ApiContext.DisposeAsync();
            PlaywrightInstance.Dispose();
        }
    }

    public class TestConfig
    {
        public string baseUrl { get; set; } = "";
    }

}
