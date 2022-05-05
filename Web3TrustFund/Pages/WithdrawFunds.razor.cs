


namespace Web3TrustFund.Pages
{
    public partial class WithdrawFunds
    {
        [Inject]
        public MetaMaskService MetaMaskService { get; set; } = default !;
        public decimal AmountEthWithdraw { get; set; }

        public string? FunctionResult { get; set; }

        public string? RpcResult { get; set; }

        public async void OnSubmitClicked()
        {
            await CallSmartContractFunctionWithdrawFunds(AmountEthWithdraw);
        }

        private string GetEncodedFunctionWithdrawFunds(BigInteger amount)
        {
            FunctionABI function = new FunctionABI("withdrawFunds", false);
            var inputsParameters = new[]{new Parameter("uint256", "_amount"), };
            function.InputParameters = inputsParameters;
            var functionCallEncoder = new FunctionCallEncoder();
            var data = functionCallEncoder.EncodeRequest(function.Sha3Signature, inputsParameters, amount);
            return data;
        }

        public async Task CallSmartContractFunctionWithdrawFunds(decimal amount)
        {
            try
            {
                BigInteger amountToWithdraw = Web3.Convert.ToWei(amount);
                string contractAddress = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AddressSettings")["ContractAddress"];
                BigInteger weiValue = 0;
                string data = GetEncodedFunctionWithdrawFunds(amountToWithdraw);
                data = data[2..]; //Remove the 0x from the generated string
                var result = await MetaMaskService.SendTransaction(contractAddress, weiValue, data);
                FunctionResult = $"TX Hash: {result}";
            }
            catch (UserDeniedException)
            {
                FunctionResult = "User Denied";
            }
            catch (Exception ex)
            {
                FunctionResult = $"Exception: {ex}";
            }
        }
    }
}