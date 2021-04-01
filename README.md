# RunGet
This was specially made for Mirror's Edge Speedrun community on Discord. Before, it would send a webhook message but you had to open a link and sometimes the mods would forget and it would take ~30 min before the API updated. And you could only send one run at the time. So instead of having to manually open a link for it to send a message, it will now do it automatically, if the program is running. What it does, it looks for new runs every ~10 minutes from an API. If it finds a new run id, it will send a embeded message to a discord channel. If it finds multiple new runs, it will send those too

Before using the weblink, there was a discord bot to look for new runs. But it was a gamble if it would show a run or not. Now it should get every run that is new from Mirror's Edge and the category extentions

## Embed Message Example
![](https://raw.githubusercontent.com/Toyro98/RunGet/main/ConsoleApi/Image/EmbedExample.png)

## Library Used
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) (v12.0.3)
- [CSharpDiscordWebhook](https://github.com/N4T4NM/CSharpDiscordWebhook) (v2.1)
- [SharpConfig](https://github.com/cemdervis/SharpConfig) (v3.2.9.1)
- [Colorful.Console](https://github.com/tomakita/Colorful.Console) (v1.2.0)

## Todo
- [ ] Learn what async is and use it in the program
- [ ] Better Json Structure
- [ ] Support variables. Example: Training% - **No Tab skip** and Training% - **with Tab skip**
- [ ] Support multiple players in a run (Only for Mirror's Edge Category Extentions)
- [ ] Handle status codes from API
  - speedrun
  - discord
- [ ] Display error messages
  - If there's no internet connection