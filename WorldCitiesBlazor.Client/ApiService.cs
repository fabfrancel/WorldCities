using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace WorldCitiesBlazor.Client;

public class ApiResponse<T>
{
    public List<T> Data { get; set; } = new List<T>();
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}


public class ApiService(HttpClient httpClient, ILogger<ApiService> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<ApiService> _logger = logger;


    public async Task<ApiResponse<T>?> GetAsync<T>(string endpoint,
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = null,
        string? sortOrder = null,
        string? filterColumn = null,
        string? filterQuery = null,
        CancellationToken cancellationToken = default) where T : class
    {
        endpoint = SetParameters(endpoint, pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);

        var response = await _httpClient.GetFromJsonAsync<ApiResponse<T>>(endpoint);

        if (response is null)
        {
            _logger.LogError($"ApiService.GetAsync<{typeof(T)}> failed");
            return null;
        }

        return response;

    }

    private string SetParameters(
        string endpoint,
        int pageIndex,
        int pageSize,
        string? sortColumn,
        string? sortOrder,
        string? filterColumn,
        string? filterQuery)
    {
        var uriBuilder = new UriBuilder(_httpClient.BaseAddress + endpoint);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["pageIndex"] = pageIndex.ToString();
        query["pageSize"] = pageSize.ToString();

        if (!string.IsNullOrWhiteSpace(sortColumn))
            query["sortColumn"] = sortColumn;

        if (!string.IsNullOrWhiteSpace(sortOrder))
            query["sortOrder"] = sortOrder;

        if (!string.IsNullOrWhiteSpace(filterColumn))
            query["filterColumn"] = filterColumn;

        if (!string.IsNullOrWhiteSpace(filterQuery))
            query["filterQuery"] = filterQuery;

        uriBuilder.Query = query.ToString();
       
        return uriBuilder.ToString();
    }
}
