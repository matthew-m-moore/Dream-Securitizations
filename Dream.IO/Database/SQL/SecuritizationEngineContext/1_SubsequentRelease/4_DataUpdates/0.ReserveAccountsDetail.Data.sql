DECLARE @FeesReserveAccountsSetId AS int
DECLARE @ClassAReserveAccountsSetId AS int
DECLARE @ClassBReserveAccountsSetId AS int
DECLARE @ClassB1ReserveAccountsSetId AS int
DECLARE @ClassB2ReserveAccountsSetId AS int
DECLARE @PreferredSharesReserveAccountsSetId AS int
DECLARE @NoteReserveAccountsSetId AS int
DECLARE @EquityReserveAccountsSetId As int

-- Securitization 1000: HERO 2017-1

EXEC dream.InsertReserveAccountsSet
SET @FeesReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 4, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 4, 8
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 5, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 5, 8
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 1, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 1, 8

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 6
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 7
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 8
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 9
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 10
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 11

EXEC dream.InsertReserveAccountsSet
SET @ClassAReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 4, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 4, 4
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 4, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 4, 6
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 5, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 5, 4
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 5, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 5, 6
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 1, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 1, 6

-- Not technically true, but needed for tie-out with integration tests
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 1, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 1, 4

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassAReserveAccountsSetId WHERE TrancheDetailId = 2
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassAReserveAccountsSetId WHERE TrancheDetailId = 3

EXEC dream.InsertReserveAccountsSet
SET @ClassBReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 4, 3
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 4, 4
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 4, 5
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 4, 6
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 5, 3
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 5, 4
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 5, 5
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 5, 6
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 1, 3
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 1, 4
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 1, 5
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 1, 6

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassBReserveAccountsSetId WHERE TrancheDetailId = 4

EXEC dream.InsertReserveAccountsSet
SET @PreferredSharesReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 5, 1
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 5, 2
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 1, 1
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 1, 2

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @PreferredSharesReserveAccountsSetId WHERE TrancheDetailId = 5

-- Securitization 1001: HERO 2017-2

EXEC dream.InsertReserveAccountsSet
SET @FeesReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 15, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 15, 8
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 16, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 16, 8
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 12, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 12, 8

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 17
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 18
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 19
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 20
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 21
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 22

EXEC dream.InsertReserveAccountsSet
SET @ClassAReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 15, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 15, 4
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 15, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 15, 6
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 16, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 16, 4
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 16, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 16, 6
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 12, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 12, 6

-- Not technically true, but needed for tie-out with integration tests
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 12, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 12, 4

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassAReserveAccountsSetId WHERE TrancheDetailId = 13
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassAReserveAccountsSetId WHERE TrancheDetailId = 14

EXEC dream.InsertReserveAccountsSet
SET @ClassBReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 15, 3
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 15, 4
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 15, 5
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 15, 6
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 16, 3
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 16, 4
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 16, 5
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 16, 6
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 12, 3
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 12, 4
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 12, 5
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 12, 6

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassBReserveAccountsSetId WHERE TrancheDetailId = 15

EXEC dream.InsertReserveAccountsSet
SET @PreferredSharesReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 16, 1
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 16, 2
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 12, 1
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 12, 2

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @PreferredSharesReserveAccountsSetId WHERE TrancheDetailId = 16

-- Securitization 1002: HERO 2017-1 Roll-forward

EXEC dream.InsertReserveAccountsSet
SET @FeesReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 25, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 25, 8
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 24, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 24, 8
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 23, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 23, 8

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 28
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 29
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 30
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 31
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 32
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 33

EXEC dream.InsertReserveAccountsSet
SET @ClassAReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 25, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 25, 4
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 25, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 25, 6
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 24, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 24, 4
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 24, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 24, 6
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 23, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 23, 6

-- Not technically true, but needed for tie-out with integration tests
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 23, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 23, 4

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassAReserveAccountsSetId WHERE TrancheDetailId = 26
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassAReserveAccountsSetId WHERE TrancheDetailId = 27

EXEC dream.InsertReserveAccountsSet
SET @ClassBReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 25, 3
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 25, 4
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 25, 5
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 25, 6
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 24, 3
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 24, 4
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 24, 5
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 24, 6
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 23, 3
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 23, 4
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 23, 5
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 23, 6

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassBReserveAccountsSetId WHERE TrancheDetailId = 25

EXEC dream.InsertReserveAccountsSet
SET @PreferredSharesReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 24, 1
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 24, 2
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 23, 1
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 23, 2

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @PreferredSharesReserveAccountsSetId WHERE TrancheDetailId = 24

-- Securitization 1003: HERO 2017-2 Pricing Tie-Out

EXEC dream.InsertReserveAccountsSet
SET @FeesReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 36, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 36, 8
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 35, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 35, 8
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 34, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 34, 8

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 39
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 40
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 41
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 42
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 43
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 44

EXEC dream.InsertReserveAccountsSet
SET @ClassAReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 36, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 36, 4
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 36, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 36, 6
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 35, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 35, 4
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 35, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 35, 6
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 34, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 34, 6

-- Not technically true, but needed for tie-out with integration tests
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 34, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 34, 4

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassAReserveAccountsSetId WHERE TrancheDetailId = 37
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassAReserveAccountsSetId WHERE TrancheDetailId = 38

EXEC dream.InsertReserveAccountsSet
SET @ClassBReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 36, 3
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 36, 4
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 36, 5
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 36, 6
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 35, 3
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 35, 4
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 35, 5
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 35, 6
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 34, 3
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 34, 4
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 34, 5
EXEC dream.InsertReserveAccountsDetail @ClassBReserveAccountsSetId, 34, 6

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassBReserveAccountsSetId WHERE TrancheDetailId = 36

EXEC dream.InsertReserveAccountsSet
SET @PreferredSharesReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 35, 1
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 35, 2
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 34, 1
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 34, 2

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @PreferredSharesReserveAccountsSetId WHERE TrancheDetailId = 35

-- Securitization 1005: HERO Funding II

EXEC dream.InsertReserveAccountsSet
SET @FeesReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 47, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 47, 8
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 45, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 45, 8

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 48
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 49
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 50
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 51
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 52
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 53
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 54

EXEC dream.InsertReserveAccountsSet
SET @NoteReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @NoteReserveAccountsSetId, 47, 3
EXEC dream.InsertReserveAccountsDetail @NoteReserveAccountsSetId, 47, 4
EXEC dream.InsertReserveAccountsDetail @NoteReserveAccountsSetId, 47, 5
EXEC dream.InsertReserveAccountsDetail @NoteReserveAccountsSetId, 47, 6
EXEC dream.InsertReserveAccountsDetail @NoteReserveAccountsSetId, 45, 3
EXEC dream.InsertReserveAccountsDetail @NoteReserveAccountsSetId, 45, 4
EXEC dream.InsertReserveAccountsDetail @NoteReserveAccountsSetId, 45, 5
EXEC dream.InsertReserveAccountsDetail @NoteReserveAccountsSetId, 45, 6

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @NoteReserveAccountsSetId WHERE TrancheDetailId = 46

EXEC dream.InsertReserveAccountsSet
SET @EquityReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @EquityReserveAccountsSetId, 47, 1
EXEC dream.InsertReserveAccountsDetail @EquityReserveAccountsSetId, 47, 2

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @EquityReserveAccountsSetId WHERE TrancheDetailId = 47

-- Securitization 1005: HERO Sub-Sequential Example

EXEC dream.InsertReserveAccountsSet
SET @FeesReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 58, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 58, 8
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 57, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 57, 8
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 56, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 56, 8
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 55, 7
EXEC dream.InsertReserveAccountsDetail @FeesReserveAccountsSetId, 55, 8

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 64
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 65
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 66
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 67
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 68
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @FeesReserveAccountsSetId WHERE TrancheDetailId = 69

EXEC dream.InsertReserveAccountsSet
SET @ClassAReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 58, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 58, 4
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 58, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 58, 6
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 57, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 57, 4
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 57, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 57, 6
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 56, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 56, 4
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 56, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 56, 6
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 55, 5
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 55, 6

-- Not technically true, but needed for tie-out with integration tests
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 55, 3
EXEC dream.InsertReserveAccountsDetail @ClassAReserveAccountsSetId, 55, 4

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassAReserveAccountsSetId WHERE TrancheDetailId = 59
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassAReserveAccountsSetId WHERE TrancheDetailId = 60
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassAReserveAccountsSetId WHERE TrancheDetailId = 61
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassAReserveAccountsSetId WHERE TrancheDetailId = 62
UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassAReserveAccountsSetId WHERE TrancheDetailId = 63

EXEC dream.InsertReserveAccountsSet
SET @ClassB1ReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 58, 3
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 58, 4
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 58, 5
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 58, 6
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 57, 3
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 57, 4
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 57, 5
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 57, 6
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 56, 3
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 56, 4
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 56, 5
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 56, 6
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 55, 3
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 55, 4
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 55, 5
EXEC dream.InsertReserveAccountsDetail @ClassB1ReserveAccountsSetId, 55, 6

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassB1ReserveAccountsSetId WHERE TrancheDetailId = 58

EXEC dream.InsertReserveAccountsSet
SET @ClassB2ReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @ClassB2ReserveAccountsSetId, 57, 3
EXEC dream.InsertReserveAccountsDetail @ClassB2ReserveAccountsSetId, 57, 4
EXEC dream.InsertReserveAccountsDetail @ClassB2ReserveAccountsSetId, 57, 5
EXEC dream.InsertReserveAccountsDetail @ClassB2ReserveAccountsSetId, 57, 6
EXEC dream.InsertReserveAccountsDetail @ClassB2ReserveAccountsSetId, 56, 3
EXEC dream.InsertReserveAccountsDetail @ClassB2ReserveAccountsSetId, 56, 4
EXEC dream.InsertReserveAccountsDetail @ClassB2ReserveAccountsSetId, 56, 5
EXEC dream.InsertReserveAccountsDetail @ClassB2ReserveAccountsSetId, 56, 6
EXEC dream.InsertReserveAccountsDetail @ClassB2ReserveAccountsSetId, 55, 3
EXEC dream.InsertReserveAccountsDetail @ClassB2ReserveAccountsSetId, 55, 4
EXEC dream.InsertReserveAccountsDetail @ClassB2ReserveAccountsSetId, 55, 5
EXEC dream.InsertReserveAccountsDetail @ClassB2ReserveAccountsSetId, 55, 6

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @ClassB2ReserveAccountsSetId WHERE TrancheDetailId = 57

EXEC dream.InsertReserveAccountsSet
SET @PreferredSharesReserveAccountsSetId = (SELECT MAX(ReserveAccountsSetId) FROM dream.ReserveAccountsSet)

EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 56, 1
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 56, 2
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 55, 1
EXEC dream.InsertReserveAccountsDetail @PreferredSharesReserveAccountsSetId, 55, 2

UPDATE dream.TrancheDetail SET ReserveAccountsSetId = @PreferredSharesReserveAccountsSetId WHERE TrancheDetailId = 56