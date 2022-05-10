


namespace Web3TrustFund.Pages
{
    public partial class Index
    {
        private static string LatestTweetText { get; set; }

        private static string bearerToken;

        private static string userId;
        protected override async Task OnInitializedAsync()
        {
            bearerToken = _config.GetValue<string>("Tokens:BearerToken");
            userId = _config.GetValue<string>("Tokens:UserId");
            LatestTweetText = await GetLatestTweetByUserId();
        }

        private void NavigateToTwitter()
        {
            navManager.NavigateTo("https://twitter.com/web3_trust");
        }

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