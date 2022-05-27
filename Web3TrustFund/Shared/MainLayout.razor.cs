


namespace Web3TrustFund.Shared
{
    public partial class MainLayout
    {

        // Metamask wallet injection
        [Inject]
        private MetaMaskService MetaMaskService { get; set; } = default !;
        // Current ETH address of user
        private string? SelectedAddress { get; set; }
        // Current active network user is using

        private string? SelectedNetwork { get; set; }
        // Current network of the user

        private Chain? Chain { get; set; }
        // Chain id of the current network

        private string? SelectedChain { get; set; }
        // Boolean value to check if Metamask is connected

        private bool IsSiteConnected { get; set; }
        // Boolean value to check if user has a Metamask walllet

        private bool HasMetaMask { get; set; } = true;
        // Substring for display of user's address only showing first 6 and last 4

        SfSidebar Sidebar;
        private bool SidebarToggle = false;
        private bool SidebarToggleFixed = false;

        private string SelectedAddressSub { get; set; }
        /// <summary>
        /// On initialization checks for Metamask, address, network and sets values accordingly
        /// </summary>
        /// <returns></returns>

        protected override async Task OnInitializedAsync()
        {
            MetaMaskService.AccountChangedEvent += MetaMaskService_AccountChangedEvent;
            MetaMaskService.ChainChangedEvent += MetaMaskService_ChainChangedEvent;
            HasMetaMask = await MetaMaskService.HasMetaMask();
            if (!HasMetaMask)
            {
                await MetaMaskService.ListenToEvents();
            }

            IsSiteConnected = await MetaMaskService.IsSiteConnected();
            if (IsSiteConnected)
            {
                string SelectedAddress = await GetSelectedAddress();
                await GetSelectedNetwork();
                string SelectedAddressSub = GetSelectedAddressSub(SelectedAddress);
            }
            
        }

        /// <summary>
        /// Converts the user's address into a viewable substring
        /// </summary>
        /// <param name="selectedAddress">Address of the user</param>
        /// <returns>Substring of address</returns>
        private string GetSelectedAddressSub(string selectedAddress)
        {
            return SelectedAddressSub = selectedAddress.Substring(0, 6) + "******" + selectedAddress.Substring(38, 4);
        }


        /// <summary>
        /// Event for the changing of current network
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task MetaMaskService_ChainChangedEvent((long, Chain) arg)
        {
            await GetSelectedAddress();
            StateHasChanged();
        }
        /// <summary>
        /// Event for the changing of current address
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private async Task MetaMaskService_AccountChangedEvent(string arg)
        {
            await GetSelectedAddress();
            StateHasChanged();
        }

        /// <summary>
        /// Gets and sets the current address of the user
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetSelectedAddress()
        {
            return SelectedAddress = await MetaMaskService.GetSelectedAddress();
        }

        /// <summary>
        /// Gets and sets the current network of the user
        /// </summary>
        private async Task GetSelectedNetwork()
        {
            var chainInfo = await MetaMaskService.GetSelectedChain();
            Chain = chainInfo.chain;
            SelectedChain = chainInfo.chainId.ToString();
            SelectedNetwork = chainInfo.chain.ToString();
            StateHasChanged();
        }

        /// <summary>
        /// Connects to Metamask if not already connected
        /// </summary>
        private async Task ConnectMetaMask()
        {
            await MetaMaskService.ConnectMetaMask();
        }

        /// <summary>
        /// Toggles nav bar from open to closed
        /// </summary>
        private void ToggleNavMenu()
        {
            SidebarToggle = !SidebarToggle;
            SidebarToggleFixed = !SidebarToggleFixed;
        }

        /// <summary>
        /// Automatically toggles nav bar open when mouse enters
        /// </summary>
        /// <param name="args"></param>
        private async Task MouseIn(MouseEventArgs args)
        {
            var isSidebar = await JS.InvokeAsync<bool>("isSidebar", args.ClientX, args.ClientY);
            if (!SidebarToggle && isSidebar)
            {
                SidebarToggle = true;
            }
        }

        /// <summary>
        /// Automatically tolggles nab bar closed when mouse leaves area
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private async Task MouseOut(MouseEventArgs args)
        {
            var isSidebar = await JS.InvokeAsync<bool>("isSidebar", args.ClientX, args.ClientY);
            if (SidebarToggle && !isSidebar)
            {
                if (!SidebarToggleFixed)
                {
                    SidebarToggle = false;
                }
            }
        }
    }
}