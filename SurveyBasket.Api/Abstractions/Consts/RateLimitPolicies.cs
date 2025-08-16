namespace SurveyBasket.Api.Abstractions.Consts;

public static class RateLimitPolicies
{
    public const string Concurrency =  "concurrency";
    public const string TokenBucket =  "token-bucket";
    public const string FixedWindow =  "fixed_window";
    public const string SlidingWindow =  "sliding-window";
    public const string IpLimiter =  "ipLimiter";
    public const string UserLimiter =  "userLimiter";
        
}