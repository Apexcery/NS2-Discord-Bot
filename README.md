# NS2 Discord Bot
A docker discord bot that allows you to scrape your player stats from [Observatory](https://observatory.morrolan.ch/).

### Variables
| Key                  | Value                                                                                                                             |
|  ------------------: | :-------------------------------------------------------------------------------------------------------------------------------: |
| `ns2-discord-token`  | The token for your bot that you have created on the [Discord Developer Portal](https://discord.com/developers/applications/)      |
| `ns2-discord-prefix` | The prefix that you want to type into discord for the commands.<br />e.g. `!ns2 ` (with the space) = `!ns2 help`                  |

### Paths
| Path                 | Description                                                                                                                       |
| -------------------: | :-------------------------------------------------------------------------------------------------------------------------------: |
|              '/data' | The folder to store the various data files used by this bot.<br /> e.g. 'profileLinks.json' which keeps track of linked profiles. |
