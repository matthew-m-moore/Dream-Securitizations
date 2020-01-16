using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.IO.Database.Entities.Securitization;
using System;
using System.Collections.Generic;

namespace Dream.Core.Converters.Database.Securitization
{
    public class AvailableFundsRetrieverDatabaseConverter
    {
        private const string _principalAdvancesRetriever = "Principal Advances";
        private const string _irregularInterestRetriever = "Irregular Interest";
        private const string _allRemainingFundsRetriever = "All Remaining Funds";
        private const string _externalPayerRetriever = "External Payer";

        public static AvailableFundsRetriever ConvertId(
            int? availableFundsRetrievalDetailId, 
            Dictionary<int, AvailableFundsRetrievalDetailEntity> availableFundsRetrievalDetailsDictionary,
            Dictionary<int, string> availableFundsRetrieverTypesDictionary)
        {
            if (!availableFundsRetrievalDetailId.HasValue) return null;

            var availableFundsRetrievalDetails = availableFundsRetrievalDetailsDictionary[availableFundsRetrievalDetailId.Value];
            var availableFundsRetrievalType = availableFundsRetrieverTypesDictionary[availableFundsRetrievalDetails.AvailableFundsRetrievalTypeId];

            switch (availableFundsRetrievalType)
            {
                case _principalAdvancesRetriever:
                    return new PrincipalRemittancesAvailableFundsRetriever(
                        availableFundsRetrievalDetails.AvailableFundsRetrievalValue.GetValueOrDefault());

                case _irregularInterestRetriever:
                    return new IrregularInterestRemittanceAvailableFundsRetriever(
                        availableFundsRetrievalDetails.AvailableFundsRetrievalInteger.GetValueOrDefault(),
                        availableFundsRetrievalDetails.AvailableFundsRetrievalDate.GetValueOrDefault());

                case _allRemainingFundsRetriever:
                    return new AllFundsAvailableFundsRetriever();

                case _externalPayerRetriever:
                    return new ExternalPayerAvailableFundsRetriever();

                default:
                    throw new Exception(string.Format("INTERNAL ERROR: There is no funds retrieval type supported named '{0}'. Please report this error.",
                        availableFundsRetrievalType));
            }
        }

        public static string ConvertToDescription(Type availableFundsRetrieverType)
        {
            if (availableFundsRetrieverType == typeof(PrincipalRemittancesAvailableFundsRetriever))
                return _principalAdvancesRetriever;

            if (availableFundsRetrieverType == typeof(IrregularInterestRemittanceAvailableFundsRetriever))
                return _irregularInterestRetriever;

            if (availableFundsRetrieverType == typeof(AllFundsAvailableFundsRetriever))
                return _allRemainingFundsRetriever;

            if (availableFundsRetrieverType == typeof(ExternalPayerAvailableFundsRetriever))
                return _externalPayerRetriever;

            throw new Exception(string.Format("INTERNAL ERROR: There is no funds retrieval type supported name '{0}'. Please report this error.",
                availableFundsRetrieverType));
        }
    }
}
