using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace app
{
    public class BrowserSettings
    {
        public static FirefoxDriverService firefoxDriverService;
        public static ChromeDriverService chromeDriverService;

        public static FirefoxDriverService FirefoxDriverService
        {
            get
            {
                firefoxDriverService = FirefoxDriverService.CreateDefaultService();
                firefoxDriverService.HideCommandPromptWindow = true;
                return firefoxDriverService;
            }
        }
        public static ChromeDriverService ChromeDriverService
        {
            get
            {
                chromeDriverService = ChromeDriverService.CreateDefaultService();
                chromeDriverService.HideCommandPromptWindow = true;
                return chromeDriverService;
            }
        }

        public static FirefoxOptions FirefoxOptions()
        {
            FirefoxOptions Profile = new FirefoxOptions();
            firefoxDriverService = FirefoxDriverService.CreateDefaultService();
            firefoxDriverService.HideCommandPromptWindow = true;
            Profile.SetPreference("browser.download.folderList", 2);
            Profile.SetPreference("browser.helperApps.neverAsk.saveToDisk", ".mp3 audio/mpeg3");
            Profile.SetPreference("browser.helperApps.alwaysAsk.force", false);
            //Profile.SetPreference("browser.download.dir", pathFolder);
            Profile.SetPreference("browser.download.manager.showWhenStarting", false);
            Profile.SetPreference("browser.tabs.loadInBackground", false);
            return Profile;
        }
    }
}