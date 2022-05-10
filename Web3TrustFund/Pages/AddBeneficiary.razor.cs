

namespace Web3TrustFund.Pages
{
    public partial class AddBeneficiary
    {
        [Inject]
        private MetaMaskService MetaMaskService { get; set; } = default !;
        private DateTime Min { get; set; } = DateTime.Now;
        private string BeneficiaryAddress { get; set; }

        private decimal AmountEthDeposit { get; set; }
        private DateTime? ReleaseDate { get; set; }

        private string? FunctionResult { get; set; }

        private string? RpcResult { get; set; }

        private int TimeUntilRelease { get; set; }


        private async void OnSubmitClicked()
        {
            
            var timeUntilRelease = ConvertToUnixTimestamp(ReleaseDate);
            await CallSmartContractFunctionAddBeneficiary(BeneficiaryAddress, timeUntilRelease, AmountEthDeposit);
           
            
        }

        private static long ConvertToUnixTimestamp(DateTime? date)
        {
            DateTime newDate = (DateTime)date;
            long timeUntilRelease = newDate.Ticks;
            return timeUntilRelease;
        }

        private string GetEncodedFunctionAddBeneficiary(string beneficiaryAddress, long timeUntilRelease)
        {
            FunctionABI function = new FunctionABI("addBeneficiary", false);
            var inputsParameters = new[]{new Parameter("address", "_beneficiary"), new Parameter("uint256", "_timeUntilRelease"), };
            function.InputParameters = inputsParameters;
            var functionCallEncoder = new FunctionCallEncoder();
            var data = functionCallEncoder.EncodeRequest(function.Sha3Signature, inputsParameters, beneficiaryAddress, timeUntilRelease);
            return data;
        }

        private async Task CallSmartContractFunctionAddBeneficiary(string beneficiaryAddress, long timeUntilRelease, decimal amount)
        {
            try
            {
                BigInteger amountToLock = Web3.Convert.ToWei(amount);
                ConfigurationManager config = new();
                string contractAddress = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("AddressSettings")["ContractAddress"];
                string data = GetEncodedFunctionAddBeneficiary(beneficiaryAddress, timeUntilRelease);
                data = data[2..]; //Remove the 0x from the generated string
                var result = await MetaMaskService.SendTransaction(contractAddress, amountToLock, data);
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