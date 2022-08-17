# DraftHelperSync 1️7️🔄

Sync tool for [MTGAHelper](https://mtgahelper.com).

## What does it do?

It updates your mtgahelper account data with card ratings from 17lands. This way you have the latest information during drafts.

## What does it show?

Currently it shows overall Win% and "Improvement when drawn"-Points.

## How do I do it?

- Download latest [release](https://github.com/LazarQt/LimitedPower.DraftHelperSync/releases)
- Unzip it and edit LimitedPower.DraftHelperSync.dll.config to include your cookie
- Cookie can be found by going to https://mtgahelper.com/my/draftRatings, logging in, pressing F12 to open developer tools, editing ANY card, then looking at request headers from the latest request

![cookie](https://user-images.githubusercontent.com/5879928/184459888-5836866b-d6b6-4d09-a859-4e59a444afd3.gif)

## Configuration Settings

In addition to the cookie there are other settings required to make the synchronization process work.

- Source:
  - 17lands: Data loaded from 17Lands website (https://www.17lands.com/)
- TimeSpanType: 
  - PastDays: Ratings will be downloaded from this very day to a number of days in the past
  - StartDate: Ratings will be downloaded from a particular day onwards
- TimeSpanValue:
  - For "PastDays", you will have to enter the number of days
  - For "StartDate" enter a specific date (Format: "2022-01-30" - year-month-day)
- DraftType: PremierDraft, TradDraft, Sealed, QuickDraft (self explanatory)
- RatingType:
  - AvgPick: Pick order determined by AvgPick data set from 17Lands
  - AbsoluteWin: Pick order determined by absolute WinRate percentage
  - RelativeWin: Pick order determined by WinRate percentage by taking into account only colors from the rated card

Now just execute LimitedPower.DraftHelperSync.exe and watch the magic (pun intended) happen:

![image](https://user-images.githubusercontent.com/5879928/185113065-1f9926c0-a3aa-4b79-aa6c-02a9968931a1.png)

If there are any problems or if you have suggestions feel free to open an issue.

Cheers
