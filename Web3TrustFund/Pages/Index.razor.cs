
namespace Web3TrustFund.Pages
{
    public partial class Index
    {
        // Text of the latest tweet by Web 3 Trust Fund
        private static string LatestTweetText { get; set; }

        // Bearer token for authorization using the Twitter API v2
        private static string bearerToken;

        // User ID for autorization using the Twitter API v2
        private static string userId;

        /// <summary>
        /// On initialization sets bearer token value, userId value, calls Twitter API v2
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            bearerToken = _config.GetValue<string>("Tokens:BearerToken");
            userId = _config.GetValue<string>("Tokens:UserId");
            LatestTweetText = await GetLatestTweetByUserId();
        }
        
        /// <summary>
        /// Navigates to Web 3 Trust Fund profile on Twitter when button clicked
        /// </summary>
        private void NavigateToTwitter()
        {
            navManager.NavigateTo("https://twitter.com/web3_trust");
        }

        /// <summary>
        /// Calls the Twitter API v2
        /// </summary>
        private static async Task<string> GetLatestTweetByUserId()
        {
            var client = new TwitterSharp.Client.TwitterClient(bearerToken);
            var answer = await client.GetTweetsFromUserIdAsync(userId, new TwitterSharp.Request.Option.TweetSearchOptions{TweetOptions = new[]{TweetOption.Attachments}, MediaOptions = new[]{MediaOption.Preview_Image_Url}});
            List<string> tweets = new();
            for (int i = 0; i < answer.Length; i++)
            {
                var tweet = answer[i];
                tweets.Add(tweet.Text);
            }

            return tweets.First();
        }
    }
}