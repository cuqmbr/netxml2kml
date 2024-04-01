# netxml2kml – .netxml to .kml CLI converter & tools

## Features
 
- Convert .netxml files from programs like Kismet or Airodump-ng to easelly viewable on map .kml files
- Save wireless networks with wireless connections (clients) releated to them to the sqlite database
- Concatenate multiple .kml files
- Filter input/output data using sql queries

<!--
## Project preview

<div style="display: flex;">
  <img src="http://drive.google.com/uc?export=view&id=1LsLWYtvKkqxeFwEhixMzFx8mZ6FT-iI6" alt="" width="618" height="300"  border="10" />
</div>                                                                                                                       
-->

## How to install?

### Download all-in-one executable

Download the latest executable suitable for your system at [Releases](https://github.com/cuqmbr/netxml2kml/releases) page

### Compile yourself

1. [Download](https://dotnet.microsoft.com/download) and install .NET SDK >= 6.0
2. Clone this git repository: `$ git clone https://github.com/cuqmbr/netxml2kml.git`
3. Compile (deploy) the app: `[~/netxml2kml]$ dotnet publish -c Release -r <RID> --self-contained true`

Reference [this](https://docs.microsoft.com/en-us/dotnet/core/deploying/) page to learn more about deployment of .NET apps

NOTE: 

- ~ – project root directory
- RID – [runtime identifier](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) of a targeted platform

<!--## How to tweak this project for your own uses?

-->

## Cammand examples

- Convert .netxml to .kml: `$ netxml2kml -i *path_to_netxml_file* -o *path_to_output_file*`
- Add wireless networks with related wireless connections (clients) to database: `$ netxml2kml -di *path_to_netxml_file*`
  - Additionally you can filter input data: `$ netxml2kml -di *path_to_netxml_file* -q *sql_query*`
- Retrieve wireless networks with related wireless connections (clients) from database: `$ netxml2kml -do *path_to_output_file*`
  - You can filter output data similarly to filtering input data: `$ netxml2kml -do *path_to_output_file* -q *sql_query*`
- Concatenate multiple .kml files: `$ netxml2kml -c *pathes_to_kml_files_separated_by_spaces* -o *path_to_output_file*`

## SQL querying reference

This section will help you to understand database structure the program uses ang give you examples of filtering input/output data using SQL queries

### Example SQL queries

Select wireless networks where:

- column name matching some pattern: `SELECT * FROM WirelessNetworks WHERE *column name* LIKE '%*pattern*%'`

- clients with a specific manufacturer has been spotted:
```
SELECT WN.* FROM WirelessNetworks AS WN
  INNER JOIN WirelessConnections AS WCo ON WN.Bssid = WCo.WirelessNetworkBssid
  INNER JOIN WirelessClients AS WCl ON WCl.Mac = WCo.WirelessClientMac
WHERE WCl.Manufacturer LIKE '%*manufacturer*%'
```

Reference [this](https://www.sqlitetutorial.net/) tutorial to learn more about various sql querying techniques

### Database schema

<pre>
/----------------------\                                                                         
|   WirelessNetworks   |                                                                         
|----------------------|                                                                         
|    PK Bssid TEXT     |1-\  /--------------------------------------\     /---------------------\
|      Essid TEXT      |  |  |          WirelessConnection          |     |   WirelessClients   |
|  Manufacturer TEXT   |  |  |--------------------------------------|     |---------------------|
|   Encryption TEXT    |  \-∞|      /- FK WirelessNetworkBssid TEXT |  /-1|     PK Mac TEXT     |
|  FrequencyMhz REAL   |     | PK -|                                |  |  |  Manufacturer TEXT  |
| MaxSignalDbm INTEGER |     |      \- FK WirelessClientMac TEXT    |∞-/  |  FirstSeenDate TEXT |
|   MaxLatitude REAL   |     |            FirstSeenDate TEXT        |     | LastUpdateDate TEXT |
|  MaxLongitude REAL   |     |           LastUpdateDate TEXT        |     \---------------------/
|   MaxAltitude REAL   |     \--------------------------------------/                            
|  FirstSeenDate TEXT  |                                                                         
| LastUpdateDate TEXT  |                                                                         
\----------------------/                                                                         
</pre>

## Data storage folder

Acording to [this](https://jimrich.sk/environment-specialfolder-on-windows-linux-and-os-x/) website program data (database and logs) are stored in netxml2kml folder at:

- Linux: /home/$USER/.local/share
- OSX: /Users/$USER/.local/share
- Windows: C:\Users\%USERNAME%\AppData\Local

## How to contribute?

If you want to add a feature, you should follow these steps:

1. Fork the project
2. Make some changes and commit them with [conventional commit message](https://www.freecodecamp.org/news/how-to-write-better-git-commit-messages/)
3. Submit a PR with a new feature/code

## Find a bug?

If you found an issue or would like to submit an improvement to this project, please submit an issue using the issues tab above. If you would like to submit a PR with a fix, reference the issue you created!

<!--
## Known issues (Work in progress)

- The route management page will not load if database contains a route with an empty departure/arrival city date
-->

## Development status

Released — Project is complete, but might receive some updates
