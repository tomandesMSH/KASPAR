using Velopack;
using Velopack.Sources;

namespace KASPAR.Services;

public sealed class UpdateService
{
    private const string RepoUrl = "https://github.com/tomandesMSH/KASPAR";

    private readonly UpdateManager _manager = new(new GithubSource(RepoUrl, null, false));

    public async Task<UpdateInfo?> CheckForUpdatesAsync()
    {
        try
        {
            return await _manager.CheckForUpdatesAsync();
        }
        catch
        {
            // No network / no releases published yet / GitHub unreachable - treat as "no update".
            return null;
        }
    }

    public Task DownloadUpdatesAsync(UpdateInfo update, Action<int> onProgress) =>
        _manager.DownloadUpdatesAsync(update, onProgress);

    public void ApplyUpdatesAndRestart(UpdateInfo update) =>
        _manager.ApplyUpdatesAndRestart(update);
}
