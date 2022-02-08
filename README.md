# LimitedPower.DraftHelperSync

Sync tool for MTGAHelper (https://mtgahelper.com/)

## What does it do?

It updates your mtgahelper account with card ratings from 17lands.
(It can also be configured to use content creator ratings since it takes a while to gather enough data for 17lands to be useful)

## What does it show?

Currently it shows overall Win% and "Improvement when drawn"-Points.
(Not to self: Add templating options?)

## How do I do it?

- Download latest Release
![DownloadWhere](https://user-images.githubusercontent.com/5879928/136125776-d0295907-f9bd-4277-b69d-ef9755e8fcc6.png)
- Unzip it and edit LimitedPower.DraftHelperSync.dll.config to include your cookie
- Cookie can be found by going to https://mtgahelper.com/my/draftRatings, logging in, pressing F12 to open developer tools, editing ANY card, then looking at request headers from the latest request:
![Censored](https://user-images.githubusercontent.com/5879928/136126600-fdbcc687-75f7-402a-b0ae-a42025f357a9.png)
- By editing the set property it's possible to select for which set to sync data

Now just execute LimitedPower.DraftHelperSync.exe and watch the magic (ayylmao get it?) happen:

![Done](https://user-images.githubusercontent.com/5879928/136125976-3234c2f0-cb82-4786-ad9e-dca9cab1f842.png)

There's a configuration file called "LimitedPower.DraftHelperSync.dll.config"
You can open this file with any editor to modify a few important settings:

- Source:
  - 17lands: Data loaded from 17Lands website (https://www.17lands.com/)
  - ContentCreatorOpinion: Data loaded from several content creators
- TimeSpanType: 
  - PastDays: Ratings will be downloaded from this very day to a number of days in the past
  - StartDate: Ratings will be downloaded from a particular day onwards
- TimeSpanValue:
  - For "PastDays", you will have to enter the number of days
  - For "StartDate" enter a specific date (Format: "2022-01-30" - year-month-day)
- DraftType: PremierDraft, TradDraft, Sealed (self explanatory)
- RatingType (Only valid with Source=17lands):
  - AvgPick: Pick order determined by AvgPick data set from 17Lands
  - AbsoluteWin: Pick order determined by absolute WinRate percentage
  - RelativeWin: Pick order determined by WinRate percentage by taking into account only colors from the rated card