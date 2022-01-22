# ovktab
A custom ruleset for osu!lazer, that adds an overlay to use VK social network inside the game.

![](OvkScreenshot1.png)

# Installing
1. Download a latest [release](https://github.com/Feodor0090/ovktab/releases/) or build manually.
2. Click "Open osu! folder" in game settings, navigate to `rulesets` folder. On windows you can just go to `%appdata%/osu/rulesets` (if you didn't change folder location).
3. Copy ruleset DLL here.
4. Make sure that [VkNet](https://github.com/vknet/vk) library DLL also placed in rulesets folder (VkNet.dll) - ovktab use it. If not, download from our releases page, or, if you use your own build, find one inside testing project build.
5. Restart the game, enjoy!
6. If you encounter issues, report to this repo.
7. If the game randomly crashes or fails to startup, uninstall this ruleset! It works via multiple hacks, so can easily break the whole game.

# Building
Ruleset:
```
dotnet build osu.Game.Rulesets.OvkTab
```

Test browser:
```
dotnet build osu.Game.Rulesets.OvkTab.Tests
```
You also can build it to get VkNet library.
