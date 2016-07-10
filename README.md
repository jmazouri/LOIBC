![Logo](http://jmazouri.github.io/LOIBC/images/loibc_logo.jpg)

Low Orbit Ion Bancannon - An automated spam management bot for Discord

# What does it do?
LOIBC uses various types of heuristics in order to generate a "spam score" for each message that's sent. Then, based on that score, various triggers can be fired in order to delete spam messages, kick/ban users, etc.

# Installation
* Clone or download a zip of the repo
* Install [.NET Core RTM](https://www.microsoft.com/net/core#windows)
* Make sure you have `sqlite3.dll` in the project directory, or your platform's equivalent
* Copy `appsettings.default.json` to `appsettings.json`
  * Set `Discord.BotKey` and `Discord.ClientId` to appropriate values as obtained from the [Discord Developer website](https://discordapp.com/developers/docs/intro)
* Run `dotnet restore`, then `dotnet run` in the project directory
* Access the Web UI to enable/disable channels and view logs
  * Defaults to `http://localhost:8080`

# Planned features
* Server spam statistics
* Chat commands for quick stats and such
* More configuration options, for enabling/disabling heuristics and adjusting weight
* Whatever people end up requesting