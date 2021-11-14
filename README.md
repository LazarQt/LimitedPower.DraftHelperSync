# LimitedPower.DraftHelperSync

## NEW Feature as of 14th of Nov 2021(!)

If I don't put it up here nobody will read it so that's why this is the first chapter.
There's a configuration file called "LimitedPower.DraftHelperSync.dll.config"
You can open this file with any editor to modify a few important settings:

- timespantype: 
  - PastDays: Ratings will be downloaded from this very day to a number of days in the past
  - StartDate: Ratings will be downloaded from a particular day onwards
- timespanvalue:
  - For "PastDays", you will have to enter the number of days, or a specific date for "StartDate" (Format: "2022-01-30" - year-month-day)
- drafttype: PremierDraft, TradDraft, Sealed (self explanatory)
- ratingtype:
  - AvgPick: Pick order determined by AvgPick data set from 17lands
  - AbsoluteWin: Pick order determined by absolute WinRate percentage
  - RelativeWin: Pick order determined by WinRate percentage by taking into account only colors from the rated card

## What does it do?

It takes a bunch of data from 17lands.com and uploads it to your personal MTGAHelper account.
This means, during draft you have access to "live" data.

## What does it show?

Currently it shows overall Win% and "Improvement when drawn"-Points. Might add some templating so data can be personalized if there's interest.

## How do I do it?

- Download latest Release
![DownloadWhere](https://user-images.githubusercontent.com/5879928/136125776-d0295907-f9bd-4277-b69d-ef9755e8fcc6.png)
- Unzip it and edit LimitedPower.DraftHelperSync.dll.config to include your cookie
- Cookie can be found by going to https://mtgahelper.com/my/draftRatings, logging in, pressing F12 to open developer tools, editing ANY card, then looking at request headers from the latest request:
![Censored](https://user-images.githubusercontent.com/5879928/136126600-fdbcc687-75f7-402a-b0ae-a42025f357a9.png)
- By editing the set property it's possible to select for which set to sync data
- By editing the pastdays property the program will go this many days back to check for 17lands data

Now just execute LimitedPower.DraftHelperSync.exe and watch the magic (ayylmao get it?) happen:

![Done](https://user-images.githubusercontent.com/5879928/136125976-3234c2f0-cb82-4786-ad9e-dca9cab1f842.png)

