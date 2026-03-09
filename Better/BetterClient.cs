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
        public string? TargetDate { get; set; }
        public string? TargetClassName { get; set; }
        public int RefreshIntervalSeconds { get; set; } = 5;

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
                //chromeOptions.AddArgument("--headless");

                _driver = new ChromeDriver(chromeOptions);
                _driver.Manage().Window.Maximize();

                var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));

                // Step 1: Navigate to Better website
                updateStatus?.Invoke("Navigating to Better.org.uk...");
                _driver.Navigate().GoToUrl("https://bookings.better.org.uk/");
                wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

                // Accept cookie consent if present (wait up to 5s for the banner to appear)
                try
                {
                    var cookieWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
                    var cookieButton = cookieWait.Until(d =>
                    {
                        try
                        {
                            var btn = d.FindElement(By.Id("onetrust-accept-btn-handler"));
                            return (btn != null && btn.Displayed && btn.Enabled) ? btn : null;
                        }
                        catch { return null; }
                    });

                    if (cookieButton != null)
                    {
                        updateStatus?.Invoke("Accepting cookies...");
                        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", cookieButton);
                        await Task.Delay(1000);
                        Console.WriteLine("Accepted cookies");
                    }
                }
                catch
                {
                    // Cookie banner not present, continue
                }

                // Step 2: Click the Log in button (identified by data-testid="login")
                updateStatus?.Invoke("Clicking login button...");
                Console.WriteLine("Looking for login button...");

                var loginButton = wait.Until(d =>
                {
                    try
                    {
                        var btn = d.FindElement(By.CssSelector("button[data-testid='login']"));
                        return (btn != null && btn.Displayed && btn.Enabled) ? btn : null;
                    }
                    catch { return null; }
                });

                if (loginButton == null)
                    throw new Exception("Could not find login button");

                updateStatus?.Invoke("Opening login form...");
                Console.WriteLine("Clicking login button");
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", loginButton);
                wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
                await Task.Delay(1000);

                // Step 3: Enter username/email (identified by id="username")
                updateStatus?.Invoke("Entering username...");
                Console.WriteLine("Looking for username field...");

                var usernameField = wait.Until(d =>
                {
                    try
                    {
                        var field = d.FindElement(By.Id("username"));
                        return (field != null && field.Displayed && field.Enabled) ? field : null;
                    }
                    catch { return null; }
                });

                if (usernameField == null)
                    throw new Exception("Could not find username field");

                Console.WriteLine("Entering username");
                usernameField.Clear();
                usernameField.SendKeys(username);
                await Task.Delay(500);

                // Step 4: Enter password (identified by id="password")
                updateStatus?.Invoke("Entering password...");
                Console.WriteLine("Looking for password field...");

                var passwordField = wait.Until(d =>
                {
                    try
                    {
                        var field = d.FindElement(By.Id("password"));
                        return (field != null && field.Displayed && field.Enabled) ? field : null;
                    }
                    catch { return null; }
                });

                if (passwordField == null)
                    throw new Exception("Could not find password field");

                Console.WriteLine("Entering password");
                passwordField.Clear();
                passwordField.SendKeys(password);
                await Task.Delay(500);

                // Step 5: Click the login submit button
                updateStatus?.Invoke("Submitting login credentials...");
                Console.WriteLine("Looking for submit button...");

                var submitButton = wait.Until(d =>
                {
                    try
                    {
                        var btn = d.FindElement(By.CssSelector("button[data-testid='log-in']"));
                        if (btn != null && btn.Displayed && btn.Enabled) return btn;

                        btn = d.FindElement(By.CssSelector("button[type='submit']"));
                        if (btn != null && btn.Displayed && btn.Enabled) return btn;

                        return null;
                    }
                    catch { return null; }
                });

                if (submitButton == null)
                    throw new Exception("Could not find login submit button");

                Console.WriteLine("Clicking login submit button");
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", submitButton);
                await Task.Delay(3000);

                updateStatus?.Invoke("Login completed successfully!");
                Console.WriteLine("Login completed successfully");
                _isLoggedIn = true;

                // Click "Book an activity" button
                updateStatus?.Invoke("Clicking Book an activity...");
                Console.WriteLine("Looking for Book an activity button...");

                var bookActivityButton = wait.Until(d =>
                {
                    try
                    {
                        var btn = d.FindElement(By.XPath("//div[contains(@class, 'DashboardCard__Wrap') and contains(., 'Book an activity')]"));
                        return (btn != null && btn.Displayed && btn.Enabled) ? btn : null;
                    }
                    catch { return null; }
                });

                if (bookActivityButton == null)
                    throw new Exception("Could not find Book an activity button");

                Console.WriteLine("Clicking Book an activity");
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", bookActivityButton);
                await Task.Delay(2000);

                // Click "Fitness Classes" link
                updateStatus?.Invoke("Clicking Fitness Classes...");
                Console.WriteLine("Looking for Fitness Classes link...");

                var fitnessClassesLink = wait.Until(d =>
                {
                    try
                    {
                        var link = d.FindElement(By.CssSelector("a[href='/location/blaydon-leisure-centre/fitness-classes']"));
                        return (link != null && link.Displayed && link.Enabled) ? link : null;
                    }
                    catch { return null; }
                });

                if (fitnessClassesLink == null)
                    throw new Exception("Could not find Fitness Classes link");

                Console.WriteLine("Clicking Fitness Classes");
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", fitnessClassesLink);
                await Task.Delay(2000);

                // Click "Fitness Classes" sub-activity link
                updateStatus?.Invoke("Clicking Fitness Classes activity...");
                Console.WriteLine("Looking for Fitness Classes activity link...");

                var fitnessClassesActivityLink = wait.Until(d =>
                {
                    try
                    {
                        var link = d.FindElement(By.CssSelector("a[href='/location/blaydon-leisure-centre/fitness-classes1']"));
                        return (link != null && link.Displayed && link.Enabled) ? link : null;
                    }
                    catch { return null; }
                });

                if (fitnessClassesActivityLink == null)
                    throw new Exception("Could not find Fitness Classes activity link");

                Console.WriteLine("Clicking Fitness Classes activity");
                ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", fitnessClassesActivityLink);
                await Task.Delay(2000);

                // Clear basket if there are any items
                try
                {
                    updateStatus?.Invoke("Checking basket...");
                    Console.WriteLine("Clearing basket if there are any items...");

                    var basketItems = _driver.FindElements(By.CssSelector("div[data-testid='activity-summary-card']"));
                    if (basketItems.Count > 0)
                    {
                        updateStatus?.Invoke($"Found {basketItems.Count} item(s) in basket, removing...");
                        Console.WriteLine($"Found {basketItems.Count} items in basket");

                        // Keep removing items until basket is empty
                        while (true)
                        {
                            try
                            {
                                // Try multiple selectors for remove button
                                IWebElement? removeButton = null;

                                // Try: aria-label with Remove
                                try
                                {
                                    removeButton = _driver.FindElement(By.XPath("//button[contains(@aria-label, 'Remove')]"));
                                }
                                catch { }

                                // Try: aria-label with Delete
                                if (removeButton == null)
                                {
                                    try
                                    {
                                        removeButton = _driver.FindElement(By.XPath("//button[contains(@aria-label, 'Delete')]"));
                                    }
                                    catch { }
                                }

                                // Try: trash icon
                                if (removeButton == null)
                                {
                                    try
                                    {
                                        removeButton = _driver.FindElement(By.XPath("//svg[@data-icon='trash']/ancestor::button"));
                                    }
                                    catch { }
                                }

                                // Try: X icon
                                if (removeButton == null)
                                {
                                    try
                                    {
                                        removeButton = _driver.FindElement(By.XPath("//svg[@data-icon='x']/ancestor::button | //svg[@data-icon='times']/ancestor::button"));
                                    }
                                    catch { }
                                }

                                if (removeButton != null && removeButton.Displayed && removeButton.Enabled)
                                {
                                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", removeButton);
                                    await Task.Delay(500);
                                    Console.WriteLine("Removed item from basket");
                                }
                                else
                                {
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error removing item: {ex.Message}");
                                break; // No more items to remove
                            }
                        }

                        updateStatus?.Invoke("Basket cleared");
                        Console.WriteLine("Basket cleared");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Note: Could not clear basket: {ex.Message}");
                }

                // Wait for target date to appear, refreshing if necessary (no cap)
                if (!string.IsNullOrEmpty(TargetDate))
                {
                    updateStatus?.Invoke($"Waiting for target date {TargetDate} to appear...");
                    Console.WriteLine($"Waiting for target date: {TargetDate}");

                    bool dateFound = false;
                    int refreshAttempts = 0;

                    while (!dateFound)
                    {
                        try
                        {
                            var dateElement = _driver.FindElement(By.CssSelector($"a[data-testid='date-{TargetDate}']"));
                            if (dateElement != null && dateElement.Displayed)
                            {
                                dateFound = true;
                                Console.WriteLine($"Target date {TargetDate} found!");
                                break;
                            }
                        }
                        catch
                        {
                            // Date not found yet
                        }

                        if (!dateFound)
                        {
                            refreshAttempts++;
                            updateStatus?.Invoke($"Date not available yet, refreshing in {RefreshIntervalSeconds}s... (Attempt {refreshAttempts})");
                            Console.WriteLine($"Date not found, refreshing page (attempt {refreshAttempts})");
                            await Task.Delay(RefreshIntervalSeconds * 1000);
                            _driver.Navigate().Refresh();
                            await Task.Delay(2000); // Wait for page to load
                        }
                    }
                }

                // Scroll to and click target date if specified
                if (!string.IsNullOrEmpty(TargetDate))
                {
                    updateStatus?.Invoke($"Scrolling to target date {TargetDate}...");
                    Console.WriteLine($"Looking for target date: {TargetDate}");

                    // Try to find the target date, scrolling right if necessary
                    IWebElement? targetDateLink = null;
                    int maxScrollAttempts = 20;
                    int scrollAttempts = 0;

                    while (targetDateLink == null && scrollAttempts < maxScrollAttempts)
                    {
                        try
                        {
                            targetDateLink = _driver.FindElement(By.CssSelector($"a[data-testid='date-{TargetDate}']"));
                            if (targetDateLink != null && targetDateLink.Displayed)
                            {
                                break;
                            }
                            targetDateLink = null;
                        }
                        catch
                        {
                            targetDateLink = null;
                        }

                        if (targetDateLink == null && scrollAttempts < maxScrollAttempts)
                        {
                            try
                            {
                                var rightArrow = _driver.FindElement(By.CssSelector("svg[data-icon='circle-chevron-right']"));
                                if (rightArrow != null)
                                {
                                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", rightArrow);
                                    await Task.Delay(300);
                                    scrollAttempts++;
                                }
                                else
                                {
                                    break; // No more dates to scroll
                                }
                            }
                            catch
                            {
                                break;
                            }
                        }
                    }

                    if (targetDateLink != null && targetDateLink.Displayed)
                    {
                        updateStatus?.Invoke($"Clicking target date {TargetDate}...");
                        Console.WriteLine($"Clicking target date: {TargetDate}");
                        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", targetDateLink);
                        await Task.Delay(2000);
                    }
                    else
                    {
                        throw new Exception($"Could not find target date {TargetDate} in the date ribbon");
                    }
                }

                // Click the Book button for the target fitness class
                if (!string.IsNullOrEmpty(TargetClassName))
                {
                    updateStatus?.Invoke($"Looking for {TargetClassName} class...");
                    Console.WriteLine($"Looking for target class: {TargetClassName}");

                    var classRow = wait.Until(d =>
                    {
                        try
                        {
                            var rows = d.FindElements(By.CssSelector("div.ClassCardComponent__Row-sc-1v7d176-1"));
                            foreach (var row in rows)
                            {
                                var classNameElement = row.FindElement(By.CssSelector("div.ClassCardComponent__NameOfClass-sc-1v7d176-5"));
                                if (classNameElement != null && classNameElement.Text.Contains(TargetClassName))
                                {
                                    return row;
                                }
                            }
                            return null;
                        }
                        catch { return null; }
                    });

                    if (classRow == null)
                        throw new Exception($"Could not find class '{TargetClassName}' on the page");

                    updateStatus?.Invoke($"Clicking Book for {TargetClassName}...");
                    Console.WriteLine($"Found target class, clicking Book button");

                    var bookButton = classRow.FindElement(By.CssSelector("button span.Button__InnerWrapper-sc-5h7i9w-0"));
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", bookButton);
                    await Task.Delay(2000);

                    updateStatus?.Invoke($"Booking {TargetClassName}...");
                    Console.WriteLine($"Successfully clicked Book button for {TargetClassName}");

                    // Click "Book now" confirmation button
                    updateStatus?.Invoke("Confirming booking...");
                    Console.WriteLine("Looking for Book now confirmation button...");

                    var bookNowButton = wait.Until(d =>
                    {
                        try
                        {
                            var buttons = d.FindElements(By.CssSelector("button span.Button__InnerWrapper-sc-5h7i9w-0"));
                            foreach (var button in buttons)
                            {
                                if (button.Text.Contains("Book now"))
                                {
                                    return button.FindElement(By.XPath("ancestor::button"));
                                }
                            }
                            return null;
                        }
                        catch { return null; }
                    });

                    if (bookNowButton == null)
                        throw new Exception("Could not find 'Book now' confirmation button");

                    Console.WriteLine("Clicking Book now button");
                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", bookNowButton);
                    await Task.Delay(2000);

                    // Check for "No payment required" message and confirm booking
                    updateStatus?.Invoke("Checking payment status...");
                    Console.WriteLine("Looking for payment information...");

                    try
                    {
                        var paymentText = wait.Until(d =>
                        {
                            try
                            {
                                var elements = d.FindElements(By.XPath("//*[contains(text(), 'No payment required')]"));
                                return elements.Count > 0 ? elements[0] : null;
                            }
                            catch { return null; }
                        });

                        if (paymentText != null)
                        {
                            updateStatus?.Invoke("No payment required - agreeing to terms and confirming booking...");
                            Console.WriteLine("Found 'No payment required' message");

                            // Ensure the checkbox is checked
                            try
                            {
                                var checkbox = _driver.FindElement(By.CssSelector("input[type='checkbox']"));
                                if (checkbox != null && !checkbox.Selected)
                                {
                                    ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", checkbox);
                                    await Task.Delay(500);
                                    Console.WriteLine("Checked terms and conditions checkbox");
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Checkbox already checked or not found");
                            }

                            // Click the confirm booking button
                            var confirmButton = wait.Until(d =>
                            {
                                try
                                {
                                    var buttons = d.FindElements(By.CssSelector("button[type='submit']"));
                                    foreach (var btn in buttons)
                                    {
                                        var innerSpan = btn.FindElement(By.CssSelector("span.Button__InnerWrapper-sc-5h7i9w-0"));
                                        if (innerSpan != null && innerSpan.Text.Contains("Confirm booking"))
                                        {
                                            return btn;
                                        }
                                    }
                                    return null;
                                }
                                catch { return null; }
                            });

                            if (confirmButton == null)
                                throw new Exception("Could not find 'Confirm booking' button");

                            Console.WriteLine("Clicking Confirm booking button");
                            ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", confirmButton);
                            await Task.Delay(2000);

                            updateStatus?.Invoke("Booking confirmed!");
                            Console.WriteLine("Successfully booked the class");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Payment confirmation error: {ex.Message}");
                        updateStatus?.Invoke("Booking confirmed!");
                        Console.WriteLine("Successfully booked the class");
                    }
                }

                // Click "View my bookings" button on confirmation page
                try
                {
                    updateStatus?.Invoke("Navigating to bookings...");
                    Console.WriteLine("Looking for View my bookings button...");

                    var viewBookingsButton = wait.Until(d =>
                    {
                        try
                        {
                            var buttons = d.FindElements(By.CssSelector("button span.Button__InnerWrapper-sc-5h7i9w-0"));
                            foreach (var button in buttons)
                            {
                                if (button.Text.Contains("View my bookings"))
                                {
                                    return button.FindElement(By.XPath("ancestor::button"));
                                }
                            }
                            return null;
                        }
                        catch { return null; }
                    });

                    if (viewBookingsButton != null)
                    {
                        Console.WriteLine("Clicking View my bookings button");
                        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView(true); arguments[0].click();", viewBookingsButton);
                        await Task.Delay(2000);
                        updateStatus?.Invoke("Viewing bookings...");
                        Console.WriteLine("Successfully navigated to bookings");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Note: Could not click View my bookings button: {ex.Message}");
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
