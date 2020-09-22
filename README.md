# NS2 Discord Bot
A docker discord bot that allows you to scrape your player stats from [Observatory](https://observatory.morrolan.ch/).

### Variables
| Key      | Value                                                                                                                                       | Required |
| :------: | :-----------------------------------------------------------------------------------------------------------------------------------------: | :------: |
| `token`  | The token for your bot that you have created on the [Discord Developer Portal](https://discord.com/developers/applications/)                | Yes      |
| `prefix` | The prefix that you want to type into discord for the commands.<br />e.g. `'!ns2 '` (with the space) = `'!ns2 help'`                        | Yes      |

### Paths
| Path       | Description                                                                                                                               | Required |
| :--------: | :---------------------------------------------------------------------------------------------------------------------------------------: | :------: |
| `/appdata` | The folder to store the various data files used by this bot.<br />e.g. The `profileLinks.json` file which keeps track of linked profiles. | Yes      |
