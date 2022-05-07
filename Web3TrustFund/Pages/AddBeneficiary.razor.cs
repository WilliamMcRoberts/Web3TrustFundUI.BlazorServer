

namespace Web3TrustFund.Pages
{
    public partial class AddBeneficiary
    {
        [Inject]
        public MetaMaskService MetaMaskService { get; set; } = default !;
        public DateTime Min { get; set; } = DateTime.Now;
        public string BeneficiaryAddress { get; set; }

        public decimal AmountEthDeposit { get; set; }
        public DateTime? ReleaseDate { get; set; }

        public string? FunctionResult { get; set; }

        public string? RpcResult { get; set; }

        public int TimeUntilRelease { get; set; }


        public async void OnSubmitClicked()
        {
            var timeUntilRelease = ConvertToUnixTimestamp(ReleaseDate);
            await CallSmartContractFunctionAddBeneficiary(BeneficiaryAddress, timeUntilRelease, AmountEthDeposit);
        }

        public static long ConvertToUnixTimestamp(DateTime? date)
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

        public async Task CallSmartContractFunctionAddBeneficiary(string beneficiaryAddress, long timeUntilRelease, decimal amount)
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