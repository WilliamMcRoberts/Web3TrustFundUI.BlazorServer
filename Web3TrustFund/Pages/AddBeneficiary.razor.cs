

namespace Web3TrustFund.Pages
{
    public partial class AddBeneficiary
    {
        // Metamask wallet injection
        [Inject]
        private MetaMaskService MetaMaskService { get; set; } = default !;

        // Minimum date for release of funds
        private DateTime Min { get; set; } = DateTime.Now;

        // Address that the user sets as the beneficiary
        private string BeneficiaryAddress { get; set; }
        // Amount of ETH the user is depositing in a trust
        private decimal AmountEthDeposit { get; set; }

        // The DateTime of the release date of a created trust
        private DateTime? ReleaseDate { get; set; }

        // The transaction hash of a called function
        private string? FunctionResult { get; set; }

        

        /// <summary>
        /// Calls the addBeneficiary function when "submit" button is clicked after calling the ConvertToUnixTimeStamp function for the release date
        /// </summary>
        private async void OnSubmitClicked()
        {
            
            var timeUntilRelease = ConvertToUnixTimestamp(ReleaseDate);
            await CallSmartContractFunctionAddBeneficiary(BeneficiaryAddress, timeUntilRelease, AmountEthDeposit);
           
            
        }
        /// <summary>
        /// Converts a DateTime date to Unix timestamp in seconds
        /// </summary>
        /// <param name="date">Release date for the funds of a trust</param>
        /// <returns>Release date in seconds</returns>
        private static long ConvertToUnixTimestamp(DateTime? date)
        {
            DateTime newDate = (DateTime)date;
            long timeUntilRelease = newDate.Ticks;
            return timeUntilRelease;
        }

        /// <summary>
        /// Encodes the data for the addBeneficiary function of the "Tust" contract
        /// </summary>
        /// <param name="beneficiaryAddress">Address of the beneficiary of a created trust</param>
        /// <param name="timeUntilRelease">Unix time in seconds for the release date of the funds of a creaed trust</param>
        /// <returns>Data input for the addBeneficiary function</returns>
        private string GetEncodedFunctionAddBeneficiary(string beneficiaryAddress, long timeUntilRelease)
        {
            FunctionABI function = new FunctionABI("addBeneficiary", false);
            var inputsParameters = new[]{new Parameter("address", "_beneficiary"), new Parameter("uint256", "_timeUntilRelease"), };
            function.InputParameters = inputsParameters;
            var functionCallEncoder = new FunctionCallEncoder();
            var data = functionCallEncoder.EncodeRequest(function.Sha3Signature, inputsParameters, beneficiaryAddress, timeUntilRelease);
            return data;
        }

        /// <summary>
        /// Calls the addBeneficiary function of the "Trust" contract
        /// </summary>
        /// <param name="beneficiaryAddress">Address of the beneficiary</param>
        /// <param name="timeUntilRelease">Time in seconds for the release date</param>
        /// <param name="amount">Amount of ETH the user is depositing in the trust</param>
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