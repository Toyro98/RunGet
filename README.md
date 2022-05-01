# RunGet
This was specially made for the Mirror's Edge Speedrun community on Discord. Before I created this, I made a php version that would send a webhook message but you had to open a link and sometimes the mods would forget and it would take ~30 min before the API updated. However, you could only send one run at the time. Basically that method wasn't the best. Instead of having to manually open a link for it to send a message, it will now do it automatically, if the program is running. 

What it does, it looks for new runs every ~5 minutes from an API. If it finds a new run id from any Mirror's Edge game, it will send a embeded message to a discord channel. If it finds multiple new runs, it will send those too.

Before using this and the weblink, there was a discord bot to look for new runs. It was a gamble if it would show a run or not. Now it will get every run that is new from Mirror's Edge and the category extentions.

## Embed Message Example
![](https://raw.githubusercontent.com/Toyro98/RunGet/main/RunGet/src/image/EmbedExample.png)