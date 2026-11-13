using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Playwright;

namespace MoodLog.E2E;

public sealed class MoodLogBrowserTests(MoodLogWebFixture fixture) : IClassFixture<MoodLogWebFixture>
{
    [Fact]
    public async Task MoodCheckIn_ShouldSaveAndRemainVisibleAfterReload()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        await NavigateAndWaitForInteractiveAsync(page, $"{fixture.BaseUrl}/mood");

        await page.Locator("#entry-level").SelectOptionAsync("Good");
        await page.Locator("#entry-tags").FillAsync("browser, e2e");
        await page.Locator("#entry-note").FillAsync($"Saved by browser test {Guid.NewGuid():N}");
        var note = await page.Locator("#entry-note").InputValueAsync();

        await page.GetByRole(AriaRole.Button, new() { Name = "Save check-in" }).ClickAsync();

        await Assertions.Expect(page.GetByRole(AriaRole.Status))
            .ToHaveTextAsync("Your check-in has been saved.");
        await Assertions.Expect(page.Locator(".mood-entry"))
            .ToContainTextAsync(note);
        await Assertions.Expect(page.Locator(".mood-tags"))
            .ToContainTextAsync("browser");

        await page.ReloadAsync();

        await Assertions.Expect(page.Locator(".mood-entry"))
            .ToContainTextAsync(note);
    }

    [Fact]
    public async Task ProfileSettings_ShouldUpdateDashboardGreeting()
    {
        await using var context = await fixture.Browser.NewContextAsync();
        var page = await context.NewPageAsync();
        var displayName = $"E2E-{Guid.NewGuid():N}";

        await NavigateAndWaitForInteractiveAsync(page, $"{fixture.BaseUrl}/profile");
        await page.Locator("#profile-name").FillAsync(displayName);
        await page.GetByRole(AriaRole.Button, new() { Name = "Save settings" }).ClickAsync();

        var status = page.GetByRole(AriaRole.Status);
        await Assertions.Expect(status)
            .ToHaveTextAsync("Your profile and goals have been saved.");

        await NavigateAndWaitForInteractiveAsync(page, fixture.BaseUrl);
        await Assertions.Expect(page.Locator("h1"))
            .ToContainTextAsync(displayName);
    }

    private static Task<IResponse?> NavigateAndWaitForInteractiveAsync(IPage page, string url) =>
        page.GotoAsync(url, new PageGotoOptions { WaitUntil = WaitUntilState.NetworkIdle });
}

public sealed class MoodLogWebFixture : IAsyncLifetime
{
    private readonly string _databaseDirectory = Path.Combine(
        Path.GetTempPath(),
        $"moodlog-e2e-{Guid.NewGuid():N}");
    private readonly StringBuilder _serverOutput = new();
    private Process? _server;
    private IPlaywright? _playwright;

    public string BaseUrl { get; private set; } = string.Empty;
    public IBrowser Browser { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Directory.CreateDirectory(_databaseDirectory);
        var port = GetAvailablePort();
        BaseUrl = $"http://127.0.0.1:{port}";

        var repositoryRoot = FindRepositoryRoot();
        var webProject = Path.Combine(
            repositoryRoot,
            "src",
            "MoodLog.Web",
            "MoodLog.Web.csproj");
        var processInfo = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = repositoryRoot,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        processInfo.ArgumentList.Add("run");
        processInfo.ArgumentList.Add("--no-build");
        processInfo.ArgumentList.Add("--no-launch-profile");
        processInfo.ArgumentList.Add("--project");
        processInfo.ArgumentList.Add(webProject);
        processInfo.Environment["ASPNETCORE_ENVIRONMENT"] = "Development";
        processInfo.Environment["ASPNETCORE_URLS"] = BaseUrl;
        processInfo.Environment["ConnectionStrings__MoodLog"] =
            $"Data Source={Path.Combine(_databaseDirectory, "moodlog.db")}";

        _server = new Process { StartInfo = processInfo, EnableRaisingEvents = true };
        _server.OutputDataReceived += CaptureServerOutput;
        _server.ErrorDataReceived += CaptureServerOutput;
        if (!_server.Start())
        {
            throw new InvalidOperationException("The MoodLog web application could not be started.");
        }

        _server.BeginOutputReadLine();
        _server.BeginErrorReadLine();
        await WaitForServerAsync();

        _playwright = await Playwright.CreateAsync();
        Browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
    }

    public async Task DisposeAsync()
    {
        if (Browser is not null)
        {
            await Browser.CloseAsync();
        }

        _playwright?.Dispose();

        if (_server is { HasExited: false })
        {
            _server.Kill(entireProcessTree: true);
            await _server.WaitForExitAsync();
        }

        _server?.Dispose();

        if (Directory.Exists(_databaseDirectory))
        {
            Directory.Delete(_databaseDirectory, recursive: true);
        }
    }

    private async Task WaitForServerAsync()
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
        var timeout = Stopwatch.StartNew();
        while (timeout.Elapsed < TimeSpan.FromSeconds(45))
        {
            if (_server is { HasExited: true })
            {
                throw new InvalidOperationException(
                    $"The MoodLog web application exited before becoming ready.{Environment.NewLine}{_serverOutput}");
            }

            try
            {
                using var response = await client.GetAsync(BaseUrl);
                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (Exception exception) when (
                exception is HttpRequestException or TaskCanceledException)
            {
            }

            await Task.Delay(250);
        }

        throw new TimeoutException(
            $"The MoodLog web application did not become ready in time.{Environment.NewLine}{_serverOutput}");
    }

    private void CaptureServerOutput(object sender, DataReceivedEventArgs args)
    {
        if (args.Data is null)
        {
            return;
        }

        lock (_serverOutput)
        {
            _serverOutput.AppendLine(args.Data);
        }
    }

    private static int GetAvailablePort()
    {
        using var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        return ((IPEndPoint)listener.LocalEndpoint).Port;
    }

    private static string FindRepositoryRoot()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
             directory is not null;
             directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "MoodLog.slnx")))
            {
                return directory.FullName;
            }
        }

        throw new DirectoryNotFoundException(
            "Could not locate MoodLog.slnx from the browser-test output directory.");
    }
}
