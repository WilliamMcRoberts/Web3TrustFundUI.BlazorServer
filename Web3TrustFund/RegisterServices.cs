using MetaMask.Blazor;
using Syncfusion.Blazor;

namespace Web3TrustFund
{
    public static class RegisterServices
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddSyncfusionBlazor();
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddMetaMaskBlazor();
        }
    }
}
