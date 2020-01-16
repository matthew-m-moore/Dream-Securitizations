using Dream.Core.BusinessLogic.Coupons;
using Dream.Core.BusinessLogic.PricingStrategies;
using Dream.Core.BusinessLogic.SecuritizationEngine;
using Dream.Core.BusinessLogic.SecuritizationEngine.AvailableFundsLogic;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.Fees;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.InterestPaying;
using Dream.Core.BusinessLogic.SecuritizationEngine.Tranches.ReserveFunds;
using Dream.Core.BusinessLogic.SecuritizationEngine.WaterfallLogic;
using Dream.Core.Converters.Database;
using Dream.Core.Converters.Database.Securitization;
using Dream.Core.Repositories.Database;
using Dream.IO.Database;
using Dream.IO.Database.Contexts;
using Dream.IO.Database.Entities.Securitization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Dream.Core.Savers
{
    public class SecuritizationTrancheDatabaseSaver : DatabaseSaver
    {
        private Securitization _securitization;
        private TypesAndConventionsDatabaseRepository _typesAndConventionsDatabaseRepository;
        private List<SecuritizationNodeEntity> _listOfSecuritizationNodeEntities;

        public Dictionary<string, int> TrancheDetailIdsDictionary { get; private set; }

        public override DbContext DatabaseContext => DatabaseContextRetrieiver.GetSecuritizationEngineContext();

        private Dictionary<string, int> _availableFundsRetrievers;
        public Dictionary<string, int> AvailableFundsRetrievers
        {
            get
            {
                if (_availableFundsRetrievers == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        _availableFundsRetrievers = securitizationEngineContext.AvailableFundsRetrievalTypeEntities
                            .ToDictionary(e => e.AvailableFundsRetrievalTypeDescription,
                                          e => e.AvailableFundsRetrievalTypeId);
                    }
                }

                return _availableFundsRetrievers;
            }
        }

        private Dictionary<string, int> _fundsDistributionRules;
        public Dictionary<string, int> FundsDistributionRules
        {
            get
            {
                if (_fundsDistributionRules == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        _fundsDistributionRules = securitizationEngineContext.FundsDistributionTypeEntities
                            .ToDictionary(e => e.FundsDistributionTypeDescription,
                                          e => e.FundsDistributionTypeId);
                    }
                }

                return _fundsDistributionRules;
            }
        }

        private Dictionary<string, int> _trancheTypes;
        public Dictionary<string, int> TrancheTypes
        {
            get
            {
                if (_trancheTypes == null)
                {
                    using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                    {
                        _trancheTypes = securitizationEngineContext.TrancheTypeEntities
                            .ToDictionary(e => e.TrancheTypeDescription,
                                          e => e.TrancheTypeId);
                    }
                }

                return _trancheTypes;
            }
        }

        public SecuritizationTrancheDatabaseSaver(
            Securitization securitization,
            TypesAndConventionsDatabaseRepository typesAndConventionsDatabaseRepository,
            DateTime cutOffDate,
            string description)
        : base(cutOffDate, description)
        {
            _securitization = securitization;
            _typesAndConventionsDatabaseRepository = typesAndConventionsDatabaseRepository;
            _listOfSecuritizationNodeEntities = new List<SecuritizationNodeEntity>();

            TrancheDetailIdsDictionary = new Dictionary<string, int>();
        }

        public int SaveSecuritizationTrancheStructure()
        {
            var securitizationNodeDataSetEntity = new SecuritizationNodeDataSetEntity
            {
                CutOffDate = _CutOffDate,
                SecuritizationNodeDataSetDescription = _Description
            };

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.SecuritizationNodeDataSetEntities.Add(securitizationNodeDataSetEntity);
                securitizationEngineContext.SaveChanges();
            }

            SaveSecuritizationNodesAndTranches(securitizationNodeDataSetEntity.SecuritizationNodeDataSetId, _securitization.SecuritizationNodes);

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.SecuritizationNodeEntities.AddRange(_listOfSecuritizationNodeEntities);
                securitizationEngineContext.SaveChanges();
            }

            return securitizationNodeDataSetEntity.SecuritizationNodeDataSetId;
        }

        private void SaveSecuritizationNodesAndTranches(int securitizationNodeDataSetId, List<SecuritizationNodeTree> securitizationNodes)
        {
            foreach(var securitizationNode in securitizationNodes)
            {
                var fundsDistributionTypeDescription = DistributionRuleDatabaseConverter
                    .DetermineDescriptionFromDistributionRule(securitizationNode.AvailableFundsDistributionRule.GetType());

                if (securitizationNode.AnyTranches)
                {
                    foreach (var securitizationTranche in securitizationNode.SecuritizationTranches)
                    {
                        SaveSecuritizationTranche(securitizationTranche);

                        var securitizationNodeEntity = new SecuritizationNodeEntity
                        {
                            SecuritizationNodeDataSetId = securitizationNodeDataSetId,
                            SecuritizationNodeName = securitizationNode.SecuritizationNodeName,
                            FundsDistributionTypeId = FundsDistributionRules[fundsDistributionTypeDescription],
                            TrancheDetailId = TrancheDetailIdsDictionary[securitizationTranche.TrancheName]
                        };

                        if (securitizationTranche.PricingStrategy != null && 
                            securitizationTranche.PricingStrategy.GetType() != typeof(DoNothingPricingStrategy))
                        {
                            var pricingStrategyDescription = PricingStrategyDatabaseConverter
                                .DeterminePricingStrategyDescription(securitizationTranche.PricingStrategy.GetType());

                            securitizationNodeEntity.TranchePricingTypeId = _typesAndConventionsDatabaseRepository.PricingTypesReversed[pricingStrategyDescription];

                            securitizationNodeEntity.TranchePricingDayCountConventionId = 
                                _typesAndConventionsDatabaseRepository.DayCountConventionsReversed[securitizationTranche.PricingStrategy.DayCountConvention];

                            securitizationNodeEntity.TranchePricingCompoundingConventionId =
                                _typesAndConventionsDatabaseRepository.CompoundingConventionsReversed[securitizationTranche.PricingStrategy.CompoundingConvention];

                            PricingStrategyDatabaseConverter.PopulateSecuritizationNodeEntity(
                                pricingStrategyDescription, 
                                securitizationNodeEntity, 
                                securitizationTranche.PricingStrategy);
                        }

                        _listOfSecuritizationNodeEntities.Add(securitizationNodeEntity);
                    }
                }

                if (securitizationNode.AnyNodes)
                {
                    foreach (var securitizationChildNode in securitizationNode.SecuritizationNodes)
                    {
                        var securitizationNodeEntity = new SecuritizationNodeEntity
                        {
                            SecuritizationNodeDataSetId = securitizationNodeDataSetId,
                            SecuritizationNodeName = securitizationNode.SecuritizationNodeName,
                            SecuritizationChildNodeName = securitizationChildNode.SecuritizationNodeName,
                            FundsDistributionTypeId = FundsDistributionRules[fundsDistributionTypeDescription]
                        };

                        _listOfSecuritizationNodeEntities.Add(securitizationNodeEntity);

                        SaveSecuritizationNodesAndTranches(securitizationNodeDataSetId, securitizationNode.SecuritizationNodes);
                    }                 
                }
            }
        }

        private void SaveSecuritizationTranche(Tranche securitizationTranche)
        {
            if (TrancheDetailIdsDictionary.ContainsKey(securitizationTranche.TrancheName)) return;

            var reserveAccountsSetId = SaveReserveAccountsSet(securitizationTranche);
            var paymentAvailableFundsRetrievalDetailId = SaveAvailableFundsRetrievalDetail(securitizationTranche.AvailableFundsRetriever);

            var trancheTypeDescription = TrancheTypeDatabaseConverter
                .ConvertType(securitizationTranche.GetType(), securitizationTranche.AbsorbsRemainingAvailableFunds);

            double? trancheBalance = null;
            if (securitizationTranche.CurrentBalance.HasValue) trancheBalance = securitizationTranche.InitialBalance;

            var trancheDetailEntity = new TrancheDetailEntity
            {
                TrancheName = securitizationTranche.TrancheName,
                TrancheBalance = trancheBalance,
                TrancheAccruedPayment = securitizationTranche.AccruedPayment,
                TrancheTypeId = TrancheTypes[trancheTypeDescription],
                
                MonthsToNextPayment = securitizationTranche.MonthsToNextPayment,
                PaymentFrequencyInMonths = securitizationTranche.PaymentFrequencyInMonths,

                PaymentAvailableFundsRetrievalDetailId = paymentAvailableFundsRetrievalDetailId,
                ReserveAccountsSetId = reserveAccountsSetId,

                IncludePaymentShortfall = securitizationTranche.IncludePaymentShortfall,
                IsShortfallPaidFromReserves = securitizationTranche.IsShortfallPaidFromReserves,
            };

            AddInterestPayingTrancheInformation(securitizationTranche, trancheDetailEntity);
            AddFeeTrancheInformation(securitizationTranche, trancheDetailEntity);
            AddReserveTrancheInformation(securitizationTranche, trancheDetailEntity);

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.TrancheDetailEntities.Add(trancheDetailEntity);
                securitizationEngineContext.SaveChanges();
            }

            TrancheDetailIdsDictionary.Add(securitizationTranche.TrancheName, trancheDetailEntity.TrancheDetailId);
            SaveSelfReferentialReserveAccount(securitizationTranche, trancheDetailEntity.ReserveAccountsSetId);

            securitizationTranche.TrancheDetailId = trancheDetailEntity.TrancheDetailId;
        }

        private int? SaveReserveAccountsSet(Tranche securitizationTranche)
        {
            if (!securitizationTranche.ListOfAssociatedReserveAccounts.Any()) return null;

            var listOfReserveAccountDetailEntities = new List<ReserveAccountsDetailEntity>();

            var reserveAccountsSet = new ReserveAccountsSetEntity();
            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.ReserveAccountsSetEntities.Add(reserveAccountsSet);
                securitizationEngineContext.SaveChanges();
            }

            // This filtering mechanism is necessary to prevent stack overflow
            var listOfAssociatedReserveAccountEntries = 
                securitizationTranche.ListOfAssociatedReserveAccounts.Where(a => a.AccountName != securitizationTranche.TrancheName).ToList();

            foreach (var associatedReserveAccountEntry in listOfAssociatedReserveAccountEntries)
            {
                // If the tranche is already saved, this call will just return
                var trancheNodeStruct = _securitization.TranchesDictionary[associatedReserveAccountEntry.AccountName];
                SaveSecuritizationTranche(trancheNodeStruct.Tranche);

                var reserveAccountDetailEntity = new ReserveAccountsDetailEntity
                {
                    ReserveAccountsSetId = reserveAccountsSet.ReserveAccountsSetId,
                    TrancheDetailId = TrancheDetailIdsDictionary[associatedReserveAccountEntry.AccountName],
                    TrancheCashFlowTypeId = _typesAndConventionsDatabaseRepository.TrancheCashFlowTypesReversed[associatedReserveAccountEntry.CashFlowType],
                };

                listOfReserveAccountDetailEntities.Add(reserveAccountDetailEntity);
            }

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.ReserveAccountsDetailEntities.AddRange(listOfReserveAccountDetailEntities);
                securitizationEngineContext.SaveChanges();
            }

            return reserveAccountsSet.ReserveAccountsSetId;
        }

        private void SaveSelfReferentialReserveAccount(Tranche securitizationTranche, int? reserveAccountsSetId)
        {
            if (!reserveAccountsSetId.HasValue) return;

            if (securitizationTranche.ListOfAssociatedReserveAccounts.Any(a => a.AccountName == securitizationTranche.TrancheName))
            {
                var selfReferentialReserveAccountDetailEntities = new List<ReserveAccountsDetailEntity>();
                foreach (var reserveAccountEntry in securitizationTranche.ListOfAssociatedReserveAccounts.Where(a => a.AccountName == securitizationTranche.TrancheName))
                {
                    var reserveAccountDetailEntity = new ReserveAccountsDetailEntity
                    {
                        ReserveAccountsSetId = reserveAccountsSetId.Value,
                        TrancheDetailId = TrancheDetailIdsDictionary[securitizationTranche.TrancheName],
                        TrancheCashFlowTypeId = _typesAndConventionsDatabaseRepository.TrancheCashFlowTypesReversed[reserveAccountEntry.CashFlowType],
                    };

                    selfReferentialReserveAccountDetailEntities.Add(reserveAccountDetailEntity);
                }

                using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                {
                    securitizationEngineContext.ReserveAccountsDetailEntities.AddRange(selfReferentialReserveAccountDetailEntities);
                    securitizationEngineContext.SaveChanges();
                }
            }
        }

        private int SaveAvailableFundsRetrievalDetail(AvailableFundsRetriever availableFundsRetriever)
        {
            var paymentAvailableFundsRetrievalDescription =
                AvailableFundsRetrieverDatabaseConverter.ConvertToDescription(availableFundsRetriever.GetType());

            var availableFundsRetrievalTypeId = AvailableFundsRetrievers[paymentAvailableFundsRetrievalDescription];
            var availableFundsRetrievalDetailEntity = new AvailableFundsRetrievalDetailEntity
            {
                AvailableFundsRetrievalTypeId = availableFundsRetrievalTypeId,
            };

            if (availableFundsRetriever is PrincipalRemittancesAvailableFundsRetriever principalRemittancesAvailableFundsRetriever)
            {
                availableFundsRetrievalDetailEntity.AvailableFundsRetrievalValue = principalRemittancesAvailableFundsRetriever.PrincipalAdvanceRate;
            }

            if (availableFundsRetriever is IrregularInterestRemittanceAvailableFundsRetriever irregularInterestRemittanceAvailableFundsRetriever)
            {
                availableFundsRetrievalDetailEntity.AvailableFundsRetrievalInteger = irregularInterestRemittanceAvailableFundsRetriever.IrregularInterestCollectionFrequencyInMonths;
                availableFundsRetrievalDetailEntity.AvailableFundsRetrievalDate = irregularInterestRemittanceAvailableFundsRetriever.IrregularInterestCollectionStartDate;
            }

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.AvailableFundsRetrievalDetailEntities.Add(availableFundsRetrievalDetailEntity);
                securitizationEngineContext.SaveChanges();
            }

            return availableFundsRetrievalDetailEntity.AvailableFundsRetrievalDetailId;
        }

        private void AddInterestPayingTrancheInformation(Tranche securitizationTranche, TrancheDetailEntity trancheDetailEntity)
        {
            if (securitizationTranche is InterestPayingTranche interestPayingTranche)
            {
                var trancheCouponId = SaveTrancheCoupon(interestPayingTranche);
                var interestAvailableFundsRetrievalDetailId = SaveAvailableFundsRetrievalDetail(interestPayingTranche.InterestAvailableFundsRetriever);

                trancheDetailEntity.TrancheAccruedInterest = interestPayingTranche.AccruedInterest;
                trancheDetailEntity.MonthsToNextInterestPayment = interestPayingTranche.MonthsToNextInterestPayment;
                trancheDetailEntity.InterestPaymentFrequencyInMonths = interestPayingTranche.InterestPaymentFrequencyInMonths;
                trancheDetailEntity.IncludeInterestShortfall = interestPayingTranche.IncludeInterestShortfall;

                trancheDetailEntity.TrancheCouponId = trancheCouponId;
                trancheDetailEntity.InterestAvailableFundsRetrievalDetailId = interestAvailableFundsRetrievalDetailId;
            }
        }

        private void AddFeeTrancheInformation(Tranche securitizationTranche, TrancheDetailEntity trancheDetailEntity)
        {
            if (securitizationTranche is FeeTranche feeTranche)
            {
                var feeGroupDetailId = SaveFeeGroupDetail(feeTranche);
                trancheDetailEntity.FeeGroupDetailId = feeGroupDetailId;

                trancheDetailEntity.PaymentConventionId = _typesAndConventionsDatabaseRepository
                    .PaymentConventionsReversed[feeTranche.FeePaymentConvention];

                trancheDetailEntity.AccrualDayCountConventionId = _typesAndConventionsDatabaseRepository
                    .DayCountConventionsReversed[feeTranche.ProRatingDayCountConvention];

                if (feeTranche is InitialPeriodSpecialAccrualFeeTranche initialPeriodSpecialAccrualFeeTranche)
                {
                    trancheDetailEntity.InitialPaymentConventionId = _typesAndConventionsDatabaseRepository
                        .PaymentConventionsReversed[initialPeriodSpecialAccrualFeeTranche.InitialFeePaymentConvention];

                    trancheDetailEntity.InitialDayCountConventionId = _typesAndConventionsDatabaseRepository
                        .DayCountConventionsReversed[initialPeriodSpecialAccrualFeeTranche.InitialProRatingDayCountConvention];

                    trancheDetailEntity.InitialPeriodEnd = initialPeriodSpecialAccrualFeeTranche.InitialPeriodEndDate;
                }
            }
        }

        private void AddReserveTrancheInformation(Tranche securitizationTranche, TrancheDetailEntity trancheDetailEntity)
        {
            if (securitizationTranche is ReserveFundTranche reserveFundTranche)
            {
                var balanceCapAndFloorSetEntity = new BalanceCapAndFloorSetEntity();
                using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                {
                    securitizationEngineContext.BalanceCapAndFloorSetEntities.Add(balanceCapAndFloorSetEntity);
                    securitizationEngineContext.SaveChanges();
                }

                // Note that the child-class has to come first in this conditional, or else it will be missed
                var listOfBalanceCapAndFloorDetailEntities = new List<BalanceCapAndFloorDetailEntity>();
                if (reserveFundTranche is PercentOfCollateralBalanceCappedReserveFundTranche percentOfCollateralBalanceCappedReserveFundTranche)
                {
                    foreach (var balanceCap in percentOfCollateralBalanceCappedReserveFundTranche.ReserveFundBalanceCapDictionary)
                    {
                        var balanceCapAndFloorDetailEntity = new BalanceCapAndFloorDetailEntity
                        {
                            BalanceCapAndFloorSetId = balanceCapAndFloorSetEntity.BalanceCapAndFloorSetId,
                            BalanceCapOrFloor = ReserveFundTrancheDatabaseConverter.BalanceCap,
                            PercentageOrDollarAmount = ReserveFundTrancheDatabaseConverter.PercentageAmount,
                            EffectiveDate = balanceCap.Key,
                            CapOrFloorValue = balanceCap.Value
                        };

                        listOfBalanceCapAndFloorDetailEntities.Add(balanceCapAndFloorDetailEntity);
                    }

                    foreach (var balanceFloor in percentOfCollateralBalanceCappedReserveFundTranche.ReserveFundBalanceFloorDictionary)
                    {
                        var balanceCapAndFloorDetailEntity = new BalanceCapAndFloorDetailEntity
                        {
                            BalanceCapAndFloorSetId = balanceCapAndFloorSetEntity.BalanceCapAndFloorSetId,
                            BalanceCapOrFloor = ReserveFundTrancheDatabaseConverter.BalanceFloor,
                            PercentageOrDollarAmount = ReserveFundTrancheDatabaseConverter.DollarAmount,
                            EffectiveDate = balanceFloor.Key,
                            CapOrFloorValue = balanceFloor.Value
                        };

                        listOfBalanceCapAndFloorDetailEntities.Add(balanceCapAndFloorDetailEntity);
                    }
                }
                else if (reserveFundTranche is CappedReserveFundTranche cappedReserveFundTranche)
                {
                    foreach (var balanceCap in cappedReserveFundTranche.ReserveFundBalanceCapDictionary)
                    {
                        var balanceCapAndFloorDetailEntity = new BalanceCapAndFloorDetailEntity
                        {
                            BalanceCapAndFloorSetId = balanceCapAndFloorSetEntity.BalanceCapAndFloorSetId,
                            BalanceCapOrFloor = ReserveFundTrancheDatabaseConverter.BalanceCap,
                            PercentageOrDollarAmount = ReserveFundTrancheDatabaseConverter.DollarAmount,
                            EffectiveDate = balanceCap.Key,
                            CapOrFloorValue = balanceCap.Value
                        };

                        listOfBalanceCapAndFloorDetailEntities.Add(balanceCapAndFloorDetailEntity);
                    }
                }

                if (listOfBalanceCapAndFloorDetailEntities.Any())
                    trancheDetailEntity.BalanceCapAndFloorSetId = balanceCapAndFloorSetEntity.BalanceCapAndFloorSetId;

                using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
                {
                    securitizationEngineContext.BalanceCapAndFloorDetailEntities.AddRange(listOfBalanceCapAndFloorDetailEntities);
                    securitizationEngineContext.SaveChanges();
                }
            }
        }

        private int? SaveTrancheCoupon(InterestPayingTranche interestPayingTranche)
        {
            var trancheCouponEntity = new TrancheCouponEntity
            {
                InterestAccrualDayCountConventionId = _typesAndConventionsDatabaseRepository
                    .DayCountConventionsReversed[interestPayingTranche.InterestAccrualDayCountConvention],

                InitialPeriodInterestAccrualDayCountConventionId = _typesAndConventionsDatabaseRepository
                    .DayCountConventionsReversed[interestPayingTranche.InitialPeriodInterestAccrualDayCountConvention],

                InitialPeriodInterestAccrualEndDate = interestPayingTranche.InitialPeriodInterestAccrualEndDate,
            };

            // The else if statement should handle the InverseFloatingRateCoupon as well, since that is a child class of FloatingRateCoupon
            if (interestPayingTranche.Coupon is FixedRateCoupon fixedRateCoupon)
            {
                trancheCouponEntity.TrancheCouponValue = fixedRateCoupon.FixedCouponRate;
            }
            else if (interestPayingTranche.Coupon is FloatingRateCoupon floatingRateCoupon)
            {
                trancheCouponEntity.TrancheCouponRateIndexId = 
                    _typesAndConventionsDatabaseRepository.InterestRateCurveTypesReversed[floatingRateCoupon.InterestRateCurve.Type];

                trancheCouponEntity.TrancheCouponValue = floatingRateCoupon.StartingCouponRate;
                trancheCouponEntity.TrancheCouponFactor = floatingRateCoupon.Factor;
                trancheCouponEntity.TrancheCouponMargin = floatingRateCoupon.Margin;
                trancheCouponEntity.TrancheCouponFloor = floatingRateCoupon.LifeFloor;
                trancheCouponEntity.TrancheCouponCeiling = floatingRateCoupon.LifeCap;
                trancheCouponEntity.TrancheCouponInterimCap = floatingRateCoupon.InterimCap;

                trancheCouponEntity.InterestRateResetFrequencyInMonths = floatingRateCoupon.InterestRateResetFrequencyInMonths;
                trancheCouponEntity.InterestRateResetLookbackMonths = floatingRateCoupon.LookbackMonths;
                trancheCouponEntity.MonthsToNextInterestRateReset = floatingRateCoupon.MonthsToNextRateReset;
            }

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.TrancheCouponEntities.Add(trancheCouponEntity);
                securitizationEngineContext.SaveChanges();
            }

            return trancheCouponEntity.TrancheCouponId;
        }

        private int? SaveFeeGroupDetail(FeeTranche feeTranche)
        {
            var feeGroupDetailEntity = new FeeGroupDetailEntity { FeeGroupName = feeTranche.TrancheName };

            if (feeTranche is PeriodicallyIncreasingFeeTranche periodicallyIncreasingFeeTranche)
            {
                feeGroupDetailEntity.FeeIncreaseRate = periodicallyIncreasingFeeTranche.RateOfIncrease;
                feeGroupDetailEntity.FeeRateUpdateFrequencyInMonths = periodicallyIncreasingFeeTranche.FeeIncreaseFrequencyInMonths;
            }
            else if (feeTranche is BondCountBasedFeeTranche bondCountBasedFeeTranche)
            {
                feeGroupDetailEntity.FeePerUnit = bondCountBasedFeeTranche.AnnualFeePerBond;
                feeGroupDetailEntity.FeeMinimum = bondCountBasedFeeTranche.AnnualMinimumFeeForBonds;
                feeGroupDetailEntity.FeeMaximum = bondCountBasedFeeTranche.AnnualMaximumFeeForBonds;
            }
            else if (feeTranche is PercentOfCollateralBalanceFeeTranche percentOfCollateralBalanceFeeTranche)
            {
                feeGroupDetailEntity.FeeRate = percentOfCollateralBalanceFeeTranche.AnnualPercentageOfCollateralBalance;
                feeGroupDetailEntity.FeeMinimum = percentOfCollateralBalanceFeeTranche.AnnualMinimumFee;
                feeGroupDetailEntity.FeeRateUpdateFrequencyInMonths = percentOfCollateralBalanceFeeTranche.BalanceUpdateFrequencyInMonths;
            }
            else if (feeTranche is PercentOfTrancheBalanceFeeTranche percentOfTrancheBalanceFeeTranche)
            {
                feeGroupDetailEntity.FeeRate = percentOfTrancheBalanceFeeTranche.AnnualPercentageOfTrancheBalance;
                feeGroupDetailEntity.FeeMinimum = percentOfTrancheBalanceFeeTranche.AnnualMinimumFee;
                feeGroupDetailEntity.FeeRollingAverageInMonths = percentOfTrancheBalanceFeeTranche.RollingAverageMonthsForBalanceCalculation;
            }

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.FeeGroupDetailEntities.Add(feeGroupDetailEntity);
                securitizationEngineContext.SaveChanges();
            }

            SaveFeeDetailsAndAssociatedTranches(feeTranche, feeGroupDetailEntity.FeeGroupDetailId);

            return feeGroupDetailEntity.FeeGroupDetailId;
        }

        private void SaveFeeDetailsAndAssociatedTranches(FeeTranche feeTranche, int feeGroupDetailId)
        {
            var listOfFeeDetailEntities = new List<FeeDetailEntity>();

            foreach (var baseFee in feeTranche.BaseAnnualFees)
            {
                var feeDetailEntity = new FeeDetailEntity
                {
                    FeeGroupDetailId = feeGroupDetailId,
                    FeeName = baseFee.Key,
                    FeeAmount = baseFee.Value,
                };

                listOfFeeDetailEntities.Add(feeDetailEntity);
            }

            foreach (var delayedFee in feeTranche.DelayedAnnualFees)
            {
                var feeDetailEntity = new FeeDetailEntity
                {
                    FeeGroupDetailId = feeGroupDetailId,
                    FeeName = delayedFee.Key,
                    FeeAmount = delayedFee.Value.DelayedFeeValue,
                    FeeEffectiveDate = delayedFee.Value.DelayedUntilDate,
                };

                listOfFeeDetailEntities.Add(feeDetailEntity);
            }

            if (feeTranche is PeriodicallyIncreasingFeeTranche periodicallyIncreasingFeeTranche)
            {
                foreach (var increasingFee in periodicallyIncreasingFeeTranche.IncreasingFees)
                {
                    var feeDetailEntity = new FeeDetailEntity
                    {
                        FeeGroupDetailId = feeGroupDetailId,
                        FeeName = increasingFee.Key,
                        FeeAmount = increasingFee.Value,
                        IsIncreasingFee = true,
                    };

                    listOfFeeDetailEntities.Add(feeDetailEntity);
                }
            }

            if (feeTranche is PercentOfTrancheBalanceFeeTranche percentOfTrancheBalanceFeeTranche)
            {
                foreach (var feeAssociatedTrancheName in percentOfTrancheBalanceFeeTranche.AssociatedTrancheNames)
                {
                    // If the tranche is already saved, this call will just return
                    var trancheNodeStruct = _securitization.TranchesDictionary[feeAssociatedTrancheName];
                    SaveSecuritizationTranche(trancheNodeStruct.Tranche);

                    var feeDetailEntity = new FeeDetailEntity
                    {
                        FeeGroupDetailId = feeGroupDetailId,
                        FeeName = feeAssociatedTrancheName,
                        FeeAssociatedTrancheDetailId = TrancheDetailIdsDictionary[feeAssociatedTrancheName]
                    };

                    listOfFeeDetailEntities.Add(feeDetailEntity);
                }
            }

            using (var securitizationEngineContext = DatabaseContext as SecuritizationEngineContext)
            {
                securitizationEngineContext.FeeDetailEntities.AddRange(listOfFeeDetailEntities);
                securitizationEngineContext.SaveChanges();
            }
        }
    }
}
