using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading.Tasks;

namespace BookGym.Better
{
    public class BetterClient : IDisposable
    {
        private IWebDriver? _driver;
        private bool _isLoggedIn = false;

        public BetterClient()
        {
        }

        public async Task<bool> LoginAsync(string username, string password, Action<string>? updateStatus = null)
        {
            try
            {
                // Initialize Chrome driver
                updateStatus?.Invoke("Initializing Chrome driver...");
                var chromeOptions = new ChromeOptions();
                // Uncomment the line below to run in headless mode
                chromeOptions.AddArgument("--headless");

                _driver = new ChromeDriver(chromeOptions);
                _driver.Manage().Window.Maximize();

                // Navigate to Better website
                updateStatus?.Invoke("Navigating to Better.org.uk...");
                _driver.Navigate().GoToUrl("https://www.better.org.uk/");

                // Wait for page to load
                await Task.Delay(2000);

                // Check for cookie consent and accept if present
                try
                {
                    var cookieButton = _driver.FindElement(By.Id("onetrust-accept-btn-handler"));
                    if (cookieButton != null && cookieButton.Displayed)
                    {
                        updateStatus?.Invoke("Accepting cookies...");

                        // Click using JavaScript for better headless compatibility
                        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", cookieButton);
                        await Task.Delay(1000);
                        Console.WriteLine("Accepted cookies");
                    }
                }
                catch
                {
                    // Cookie button not present, continue
                }

                // Find and click the "Book activity / login" button
                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

                var loginButton = wait.Until(d =>
                {
                    try
                    {
                        // Look for the specific "Book activity / login" link
                        return d.FindElement(By.CssSelector("a[href='/book-activity']")) ??
                               d.FindElement(By.PartialLinkText("Book activity"));
                    }
                    catch
                    {
                        return null;
                    }
                });

                if (loginButton != null)
                {
                    updateStatus?.Invoke("Clicking 'Book activity / login' button...");
                    Console.WriteLine("Clicking 'Book activity / login' button");

                    // Scroll into view and click using JavaScript for better headless compatibility
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", loginButton);
                    await Task.Delay(500);
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", loginButton);
                    await Task.Delay(2000);

                    // Wait for page to be ready
                    wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                }
                else
                {
                    throw new Exception("Could not find 'Book activity / login' button");
                }

                // Click on Blaydon Leisure Centre
                var blaydonsButton = wait.Until(d =>
                {
                    try
                    {
                        // Look for the Blaydon Leisure Centre link
                        return d.FindElement(By.CssSelector("a[href='https://bookings.better.org.uk/location/blaydon-leisure-centre']")) ??
                               d.FindElement(By.CssSelector("a[href*='/location/blaydon-leisure-centre']")) ??
                               d.FindElement(By.XPath("//a[.//p[@class='result-card__title' and contains(text(),'Blaydon Leisure Centre')]]"));
                    }
                    catch
                    {
                        return null;
                    }
                });

                if (blaydonsButton != null)
                {
                    updateStatus?.Invoke("Selecting Blaydon Leisure Centre...");
                    Console.WriteLine("Clicking 'Blaydon Leisure Centre' link");

                    // Scroll into view and click using JavaScript for better headless compatibility
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", blaydonsButton);
                    await Task.Delay(500);
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", blaydonsButton);
                    await Task.Delay(2000);

                    // Wait for page to be ready
                    wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                }
                else
                {
                    throw new Exception("Could not find 'Blaydon Leisure Centre' link");
                }

                // Click on Fitness Classes
                var fitnessClassesButton = wait.Until(d =>
                {
                    try
                    {
                        // Look for the Fitness Classes link by text content
                        return d.FindElement(By.XPath("//div[@class='ActivityComponent__ActivityName-sc-o8727s-3 fxUnWE' and text()='Fitness Classes']")) ??
                               d.FindElement(By.XPath("//div[contains(@class,'ActivityComponent__ActivityName') and text()='Fitness Classes']")) ??
                               d.FindElement(By.XPath("//a[.//div[text()='Fitness Classes']]"));
                    }
                    catch
                    {
                        return null;
                    }
                });

                if (fitnessClassesButton != null)
                {
                    updateStatus?.Invoke("Clicking 'Fitness Classes' button...");
                    Console.WriteLine("Clicking 'Fitness Classes' button");

                    // Scroll into view and click using JavaScript for better headless compatibility
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", fitnessClassesButton);
                    await Task.Delay(500);
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", fitnessClassesButton);
                    await Task.Delay(2000);

                    // Wait for page to be ready
                    wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                }
                else
                {
                    throw new Exception("Could not find 'Fitness Classes' button");
                }

                // Click on Fitness Classes link
                var fitnessClassesLink = wait.Until(d =>
                {
                    try
                    {
                        // Look for the Fitness Classes link
                        return d.FindElement(By.CssSelector("a[href='/location/blaydon-leisure-centre/fitness-classes-c']")) ??
                               d.FindElement(By.CssSelector("a.SubActivityComponent__StyledLink-sc-1n9r15f-1")) ??
                               d.FindElement(By.XPath("//a[contains(@href,'fitness-classes-c')]"));
                    }
                    catch
                    {
                        return null;
                    }
                });

                if (fitnessClassesLink != null)
                {
                    updateStatus?.Invoke("Opening Fitness Classes...");
                    Console.WriteLine("Clicking 'Fitness Classes' link");

                    // Scroll into view and click using JavaScript for better headless compatibility
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", fitnessClassesLink);
                    await Task.Delay(500);
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", fitnessClassesLink);

                    // Wait longer for page to load in headless mode
                    await Task.Delay(5000);

                    // Wait for page to be ready
                    wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

                    // Additional wait for dynamic content
                    await Task.Delay(2000);
                }
                else
                {
                    throw new Exception("Could not find 'Fitness Classes' link");
                }

                // Click the login button
                updateStatus?.Invoke("Waiting for login button...");
                Console.WriteLine("Looking for login button...");

                // Use a retry approach to handle dynamic DOM
                bool loginButtonClicked = false;
                for (int retryCount = 0; retryCount < 3 && !loginButtonClicked; retryCount++)
                {
                    try
                    {
                        if (retryCount > 0)
                        {
                            Console.WriteLine($"Retry attempt {retryCount} for login button...");
                            await Task.Delay(1000);
                        }

                        var actualLoginButton = wait.Until(d =>
                        {
                            try
                            {
                                // Try multiple selectors for the login button
                                var button = d.FindElement(By.CssSelector("button[data-testid='login']"));
                                if (button != null && button.Displayed && button.Enabled) return button;

                                button = d.FindElement(By.XPath("//button[@data-testid='login']"));
                                if (button != null && button.Displayed && button.Enabled) return button;

                                button = d.FindElement(By.XPath("//button[.//span[contains(text(),'Log in')]]"));
                                if (button != null && button.Displayed && button.Enabled) return button;

                                return null;
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Login button search attempt failed: {ex.Message}");
                                return null;
                            }
                        });

                        if (actualLoginButton != null)
                        {
                            updateStatus?.Invoke("Opening login form...");
                            Console.WriteLine("Clicking 'Log in' button");

                            // Click immediately using JavaScript to avoid stale element
                            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", actualLoginButton);
                            loginButtonClicked = true;
                        }
                    }
                    catch (StaleElementReferenceException ex)
                    {
                        Console.WriteLine($"Stale element on attempt {retryCount + 1}: {ex.Message}");
                        if (retryCount == 2)
                        {
                            throw new Exception("Failed to click login button after 3 attempts due to stale element");
                        }
                    }
                }

                if (loginButtonClicked)
                {

                    // Wait longer for login form to appear and DOM to stabilize
                    await Task.Delay(3000);

                    // Wait for page to be ready
                    wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

                    Console.WriteLine("Login form should be visible now");
                }
                else
                {
                    throw new Exception("Could not find 'Log in' button");
                }

                // Wait for login form and enter credentials
                updateStatus?.Invoke("Waiting for username field...");
                Console.WriteLine("Looking for username field...");

                var usernameField = wait.Until(d =>
                {
                    try
                    {
                        var field = d.FindElement(By.Id("username"));
                        // Make sure the field is displayed and enabled
                        if (field != null && field.Displayed && field.Enabled)
                        {
                            return field;
                        }
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Username field search attempt: {ex.Message}");
                        return null;
                    }
                });

                if (usernameField != null)
                {
                    updateStatus?.Invoke("Entering username...");
                    Console.WriteLine("Entering username");

                    // Use JavaScript to set the value to avoid stale element issues
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].value = '';", usernameField);
                    await Task.Delay(200);
                    ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{username}';", usernameField);
                    await Task.Delay(1000);
                }
                else
                {
                    throw new Exception("Could not find username field");
                }

                // Enter password
                updateStatus?.Invoke("Waiting for password field...");
                Console.WriteLine("Looking for password field...");

                var passwordField = wait.Until(d =>
                {
                    try
                    {
                        var field = d.FindElement(By.Id("password"));
                        // Make sure the field is displayed and enabled
                        if (field != null && field.Displayed && field.Enabled)
                        {
                            return field;
                        }
                        return null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Password field search attempt: {ex.Message}");
                        return null;
                    }
                });

                if (passwordField != null)
                {
                    updateStatus?.Invoke("Entering password...");
                    Console.WriteLine("Entering password");

                    // Use JavaScript to set the value to avoid stale element issues
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].value = '';", passwordField);
                    await Task.Delay(200);
                    ((IJavaScriptExecutor)_driver).ExecuteScript($"arguments[0].value = '{password}';", passwordField);
                    await Task.Delay(1000);
                }
                else
                {
                    throw new Exception("Could not find password field");
                }

                // Click the login submit button
                updateStatus?.Invoke("Looking for submit button...");
                Console.WriteLine("Looking for submit button...");

                var submitButton = wait.Until(d =>
                {
                    try
                    {
                        // Find the login button by data-testid attribute
                        var button = d.FindElement(By.CssSelector("button[data-testid='log-in']"));
                        if (button != null && button.Displayed && button.Enabled) return button;

                        button = d.FindElement(By.CssSelector("button[type='submit']"));
                        if (button != null && button.Displayed && button.Enabled) return button;

                        return null;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Submit button search attempt: {ex.Message}");
                        return null;
                    }
                });

                if (submitButton != null)
                {
                    updateStatus?.Invoke("Submitting login credentials...");
                    Console.WriteLine("Clicking 'Log in' submit button");

                    // Scroll into view and click using JavaScript for better headless compatibility
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", submitButton);
                    await Task.Delay(500);
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", submitButton);
                    await Task.Delay(3000);
                }
                else
                {
                    throw new Exception("Could not find 'Log in' submit button");
                }

                updateStatus?.Invoke("Login completed successfully!");
                Console.WriteLine("Login completed successfully");
                _isLoggedIn = true;

                // After login, more dates may appear - scroll to the latest date
                updateStatus?.Invoke("Checking for additional available dates...");
                Console.WriteLine("Scrolling to latest dates after login");
                for (int i = 0; i < 10; i++) // Click up to 10 times to reach the end
                {
                    try
                    {
                        var rightArrow = _driver.FindElement(By.CssSelector("svg[data-icon='circle-chevron-right']"));
                        if (rightArrow != null && rightArrow.Displayed && rightArrow.Enabled)
                        {
                            // Click using JavaScript for better headless compatibility
                            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", rightArrow);
                            await Task.Delay(500);
                        }
                        else
                        {
                            // Arrow is disabled or not visible, we've reached the end
                            break;
                        }
                    }
                    catch
                    {
                        // Arrow not found or can't be clicked, we've reached the end
                        break;
                    }
                }

                // Click on the latest date after login
                var latestDateAfterLogin = wait.Until(d =>
                {
                    try
                    {
                        // Find all date links in the ribbon, excluding disabled and undefined dates
                        var dateLinks = d.FindElements(By.CssSelector("a[data-testid^='date-']"))
                            .Where(link => !link.GetAttribute("disabled")?.Equals("") ?? true)
                            .Where(link => !link.GetAttribute("data-testid").Contains("undefined"))
                            .ToList();

                        // Return the last one (latest date)
                        return dateLinks.Count > 0 ? dateLinks[dateLinks.Count - 1] : null;
                    }
                    catch
                    {
                        return null;
                    }
                });

                if (latestDateAfterLogin != null)
                {
                    updateStatus?.Invoke("Selecting final date...");
                    Console.WriteLine($"Clicking latest date after login: {latestDateAfterLogin.GetAttribute("data-testid")}");

                    // Scroll into view and click using JavaScript for better headless compatibility
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true);", latestDateAfterLogin);
                    await Task.Delay(500);
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", latestDateAfterLogin);
                    await Task.Delay(2000);
                }
                else
                {
                    Console.WriteLine("Warning: Could not find date ribbon after login");
                }

                // Continuous refresh loop to monitor for available classes
                updateStatus?.Invoke("Starting class monitoring...");
                Console.WriteLine("Starting refresh loop to monitor for classes...");
                while (true)
                {
                    // Refresh the page
                    updateStatus?.Invoke("Refreshing page to check for classes...");
                    _driver.Navigate().Refresh();

                    // Wait for page to load
                    await Task.Delay(2000);

                    // Wait 1s - placeholder for future logic to check for a class
                    await Task.Delay(1000);

                    updateStatus?.Invoke("Checking for available classes...");
                    Console.WriteLine("Page refreshed, checking for classes...");

                    // TODO: Add logic here to check for available classes
                    // If class found, break out of loop
                }

                return true;
            }
            catch (Exception ex)
            {
                updateStatus?.Invoke($"Error: {ex.Message}");
                Console.WriteLine($"Login failed: {ex.Message}");
                return false;
            }
        }

        public IWebDriver? GetDriver()
        {
            return _driver;
        }

        public void Dispose()
        {
            _driver?.Quit();
            _driver?.Dispose();
        }
    }
}
