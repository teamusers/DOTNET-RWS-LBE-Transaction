using RWS_LBE_Transaction.Services;

namespace RWS_LBE_Transaction.Helpers
{
    public interface IApiHttpClient
    {
        Task<(T? Result, string RawResponse)> DoApiRequestAsync<T>(ApiRequestOptions opts);

    }
}