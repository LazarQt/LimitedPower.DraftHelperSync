# LimitedPower.DraftHelperSync

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

