


namespace Web3TrustFund.Pages
{
    public partial class WithdrawFunds
    {
        // Metamask wallet injection
        [Inject]
        private MetaMaskService MetaMaskService { get; set; } = default !;
        // Amount of ETH the beneficiary is withdrawing
        private decimal AmountEthWithdraw { get; set; }
        // transaction hash of a withraw
        private string? FunctionResult { get; set; }

        /// <summary>
        /// Calls the withdrawFunds function when "submit" button is clicked
        /// </summary>
        private async void OnSubmitClicked()
        {
            await CallSmartContractFunctionWithdrawFunds(AmountEthWithdraw);
        }

        /// <summary>
        /// Encodes the data for the withdrawFunds function
        /// </summary>
        /// <param name="amount">Amount of ETH the beneficiary is withdrawing</param>
        /// <returns>Data input for withdrawFunds function</returns>
        private string GetEncodedFunctionWithdrawFunds(BigInteger amount)
        {
            FunctionABI function = new FunctionABI("withdrawFunds", false);
            var inputsParameters = new[]{new Parameter("uint256", "_amount"), };
            function.InputParameters = inputsParameters;
            var functionCallEncoder = new FunctionCallEncoder();
            var data = functionCallEncoder.EncodeRequest(function.Sha3Signature, inputsParameters, amount);
            return data;
        }

        /// <summary>
        /// Calls the withdrawFunds function of the "Trust" contract
        /// </summary>
        /// <param name="amount">Amount of ETH the beneficiary is withdrawing</param>
        private async Task CallSmartContractFunctionWithdrawFunds(decimal amount)
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