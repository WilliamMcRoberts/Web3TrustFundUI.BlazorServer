


namespace Web3TrustFund.Shared
{
    public partial class MainLayout
    {
        SfSidebar Sidebar;
        public bool SidebarToggle = false;
        public bool SidebarToggleFixed = false;
        [Inject]
        public MetaMaskService MetaMaskService { get; set; } = default !;
        public string? SelectedAddress { get; set; }

        public string? SelectedNetwork { get; set; }

        public Chain? Chain { get; set; }

        public string? SelectedChain { get; set; }

        public bool IsSiteConnected { get; set; }

        public bool HasMetaMask { get; set; } = true;
        public string? Nonce { get; set; }

        public string SelectedAddressSub { get; set; }

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
            else
            {
                await ConnectMetaMask();
            }

            StateHasChanged();
        }

        public string GetSelectedAddressSub(string selectedAddress)
        {
            return SelectedAddressSub = selectedAddress.Substring(0, 6) + "******" + selectedAddress.Substring(38, 4);
        }

        public async Task MetaMaskService_ChainChangedEvent((long, Chain) arg)
        {
            await GetSelectedAddress();
            StateHasChanged();
        }

        public async Task MetaMaskService_AccountChangedEvent(string arg)
        {
            await GetSelectedAddress();
            StateHasChanged();
        }

        public async Task<string> GetSelectedAddress()
        {
            return SelectedAddress = await MetaMaskService.GetSelectedAddress();
        }

        public async Task GetSelectedNetwork()
        {
            var chainInfo = await MetaMaskService.GetSelectedChain();
            Chain = chainInfo.chain;
            SelectedChain = chainInfo.chainId.ToString();
            SelectedNetwork = chainInfo.chain.ToString();
            StateHasChanged();
        }

        public async Task ConnectMetaMask()
        {
            await MetaMaskService.ConnectMetaMask();
        }

        public void ToggleNavMenu()
        {
            SidebarToggle = !SidebarToggle;
            SidebarToggleFixed = !SidebarToggleFixed;
        }

        public async Task MouseIn(MouseEventArgs args)
        {
            var isSidebar = await JS.InvokeAsync<bool>("isSidebar", args.ClientX, args.ClientY);
            if (!SidebarToggle && isSidebar)
            {
                SidebarToggle = true;
            }
        }

        public async Task MouseOut(MouseEventArgs args)
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