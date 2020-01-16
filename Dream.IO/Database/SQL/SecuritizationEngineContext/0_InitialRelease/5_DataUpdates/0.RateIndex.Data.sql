exec dream.InsertRateIndexGroup 'London Inter-Bank Offer Rates (LIBOR)'
declare @LiborGroupId as int
set @LiborGroupId = (select max(RateIndexGroupId) from dream.RateIndexGroup)

exec dream.InsertRateIndexGroup 'Interest Rate Swaps, Fixed Leg Rate (Swap)'
declare @SwapGroupId as int
set @SwapGroupId = (select max(RateIndexGroupId) from dream.RateIndexGroup)

exec dream.InsertRateIndexGroup 'Constant Maturity Treasury (CMT)'
declare @TreasuryGroupId as int
set @TreasuryGroupId = (select max(RateIndexGroupId) from dream.RateIndexGroup)

exec dream.InsertRateIndex 'LIBOR-O/N', NULL, @LiborGroupId
exec dream.InsertRateIndex 'LIBOR-1W', NULL, @LiborGroupId
exec dream.InsertRateIndex 'LIBOR-1M', 1, @LiborGroupId
exec dream.InsertRateIndex 'LIBOR-2M', 2, @LiborGroupId
exec dream.InsertRateIndex 'LIBOR-3M', 3, @LiborGroupId
exec dream.InsertRateIndex 'LIBOR-6M', 6, @LiborGroupId
exec dream.InsertRateIndex 'LIBOR-1Y', 12, @LiborGroupId
exec dream.InsertRateIndex 'LIBOR-Based Zero Rates', NULL, NULL
exec dream.InsertRateIndex 'LIBOR-Based Discount Factors', NULL, NULL
exec dream.InsertRateIndex 'CMT-1M', 1, @TreasuryGroupId
exec dream.InsertRateIndex 'CMT-3M', 3, @TreasuryGroupId
exec dream.InsertRateIndex 'CMT-6M', 6, @TreasuryGroupId
exec dream.InsertRateIndex 'CMT-1Y', 12, @TreasuryGroupId
exec dream.InsertRateIndex 'CMT-2Y', 24, @TreasuryGroupId
exec dream.InsertRateIndex 'CMT-3Y', 36, @TreasuryGroupId
exec dream.InsertRateIndex 'CMT-5Y', 60, @TreasuryGroupId
exec dream.InsertRateIndex 'CMT-7Y', 84, @TreasuryGroupId
exec dream.InsertRateIndex 'CMT-10Y', 120, @TreasuryGroupId
exec dream.InsertRateIndex 'CMT-20Y', 240, @TreasuryGroupId
exec dream.InsertRateIndex 'CMT-30Y', 360, @TreasuryGroupId
exec dream.InsertRateIndex 'Treasury Zero Rates', NULL, NULL
exec dream.InsertRateIndex 'Treasury Discount Factors', NULL, NULL
exec dream.InsertRateIndex 'Swap-4M', 4, @SwapGroupId
exec dream.InsertRateIndex 'Swap-5M', 5, @SwapGroupId
exec dream.InsertRateIndex 'Swap-6M', 6, @SwapGroupId
exec dream.InsertRateIndex 'Swap-7M', 7, @SwapGroupId
exec dream.InsertRateIndex 'Swap-8M', 8, @SwapGroupId
exec dream.InsertRateIndex 'Swap-9M', 9, @SwapGroupId
exec dream.InsertRateIndex 'Swap-10M', 10, @SwapGroupId
exec dream.InsertRateIndex 'Swap-11M', 11, @SwapGroupId
exec dream.InsertRateIndex 'Swap-18M', 18, @SwapGroupId
exec dream.InsertRateIndex 'Swap-1Y', 12, @SwapGroupId
exec dream.InsertRateIndex 'Swap-2Y', 24, @SwapGroupId
exec dream.InsertRateIndex 'Swap-3Y', 36, @SwapGroupId
exec dream.InsertRateIndex 'Swap-4Y', 48, @SwapGroupId
exec dream.InsertRateIndex 'Swap-5Y', 60, @SwapGroupId
exec dream.InsertRateIndex 'Swap-6Y', 72, @SwapGroupId
exec dream.InsertRateIndex 'Swap-7Y', 84, @SwapGroupId
exec dream.InsertRateIndex 'Swap-8Y', 96, @SwapGroupId
exec dream.InsertRateIndex 'Swap-9Y', 108, @SwapGroupId
exec dream.InsertRateIndex 'Swap-10Y', 120, @SwapGroupId
exec dream.InsertRateIndex 'Swap-11Y', 132, @SwapGroupId
exec dream.InsertRateIndex 'Swap-12Y', 144, @SwapGroupId
exec dream.InsertRateIndex 'Swap-15Y', 180, @SwapGroupId
exec dream.InsertRateIndex 'Swap-20Y', 240, @SwapGroupId
exec dream.InsertRateIndex 'Swap-25Y', 300, @SwapGroupId
exec dream.InsertRateIndex 'Swap-30Y', 360, @SwapGroupId
exec dream.InsertRateIndex 'Swap-40Y', 480, @SwapGroupId
exec dream.InsertRateIndex 'Swap-50Y', 600, @SwapGroupId
exec dream.InsertRateIndex 'None', NULL, NULL