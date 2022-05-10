


namespace Web3TrustFund.Shared
{
    public partial class MainLayout
    {
        SfSidebar Sidebar;
        private bool SidebarToggle = false;
        private bool SidebarToggleFixed = false;
        [Inject]
        private MetaMaskService MetaMaskService { get; set; } = default !;
        private string? SelectedAddress { get; set; }

        private string? SelectedNetwork { get; set; }

        private Chain? Chain { get; set; }

        private string? SelectedChain { get; set; }

        private bool IsSiteConnected { get; set; }

        private bool HasMetaMask { get; set; } = true;
        private string? Nonce { get; set; }

        private string SelectedAddressSub { get; set; }

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

        private string GetSelectedAddressSub(string selectedAddress)
        {
            return SelectedAddressSub = selectedAddress.Substring(0, 6) + "******" + selectedAddress.Substring(38, 4);
        }

        private async Task MetaMaskService_ChainChangedEvent((long, Chain) arg)
        {
            await GetSelectedAddress();
            StateHasChanged();
        }

        private async Task MetaMaskService_AccountChangedEvent(string arg)
        {
            await GetSelectedAddress();
            StateHasChanged();
        }

        private async Task<string> GetSelectedAddress()
        {
            return SelectedAddress = await MetaMaskService.GetSelectedAddress();
        }

        private async Task GetSelectedNetwork()
        {
            var chainInfo = await MetaMaskService.GetSelectedChain();
            Chain = chainInfo.chain;
            SelectedChain = chainInfo.chainId.ToString();
            SelectedNetwork = chainInfo.chain.ToString();
            StateHasChanged();
        }

        private async Task ConnectMetaMask()
        {
            await MetaMaskService.ConnectMetaMask();
        }

        private void ToggleNavMenu()
        {
            SidebarToggle = !SidebarToggle;
            SidebarToggleFixed = !SidebarToggleFixed;
        }

        private async Task MouseIn(MouseEventArgs args)
        {
            var isSidebar = await JS.InvokeAsync<bool>("isSidebar", args.ClientX, args.ClientY);
            if (!SidebarToggle && isSidebar)
            {
                SidebarToggle = true;
            }
        }

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