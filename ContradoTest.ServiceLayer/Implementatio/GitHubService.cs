using ContradoTest.Models;
using ContradoTest.ServiceLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

public class GitHubService : IGitHubService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GitHubService> _logger;

    public GitHubService(HttpClient httpClient, IConfiguration configuration, ILogger<GitHubService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;

        InitializeHttpClient();
    }

    private void InitializeHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_configuration["GitHubApi:BaseAddress"]);
        _httpClient.DefaultRequestHeaders.Add("User-Agent", _configuration["GitHubApi:UserAgent"]);
    }

    public async Task<UserAndRepositories> Search(string username)
    {
        try
        {
            var userInfo = await GetUserInformationAsync(username);
            if (userInfo != null)
            {
                var repositories = await GetTopStarredRepositories(userInfo.Repos_Url);

                return new UserAndRepositories
                {
                    User = userInfo,
                    Repositories = repositories.Take(5).ToList()
                };
            }

            return null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "GitHub API request failed.");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error.");
            throw;
        }
    }

    private async Task<UserInfo> GetUserInformationAsync(string username)
    {
        var response = await _httpClient.GetAsync(username);
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserInfo>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        else if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null; 
        }
        else
        {
            throw new HttpRequestException($"GitHub API error: {response.StatusCode}");
        }
    }

    private async Task<List<RepositoryInfo>> GetTopStarredRepositories(string reposUrl)
    {
        var response = await _httpClient.GetAsync(reposUrl);
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var repositories = JsonSerializer.Deserialize<List<RepositoryInfo>>(json);
            return repositories.OrderByDescending(r => r.stargazers_count).ToList();
        }
        else {
            throw new HttpRequestException($"GitHub API error: {response.StatusCode}");
        }
    }
}
