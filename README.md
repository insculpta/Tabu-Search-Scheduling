==================File Structure===========================================
TeampointScheduling 
-TeampointScheduling.cs (source code)
-TeampointScheduling.exe.config (configuration file)
-MySql.Data.dll
-Ubiety.Dns.Core.dll
-README.md


==================Environment setting for C# on Linux======================
Steps:
1. Download and install Mono on Linux:
 (1)Mono: Sponsored by Microsoft, Mono is an open source implementation of Microsoft's .NET Framework based on the ECMA standards for C# and the Common Language Runtime.
 (2)Mono Install page: https://www.mono-project.com/download/stable/#download-lin 

2. Download and install Connector/NET (MySQL) on Unix/Mono:
 (1)Download the mysql-connector-net-version-noinstall.zip and extract the contents to a directory of your choice, for example: ~/connector-net/. (Download page: https://dev.mysql.com/downloads/ connector/net/) on this page select .NET & Mono) 
 (2)In the directory where you unzipped the file, go to folder v4.8, ensure the file MySql.Data.dll is present.
 (3)Register the Connector/NET component, MySql.Data, in the Global Assembly Cache (GAC) by running gacutil command: sudo gacutil -i MySql.Data.dll in mysql/v4.8 directory. 
    Install tutorial page: https: //dev.mysql.com/doc/connector-net/en/connector-net-installation-unix.html 
 
3.Compile: In TeampointScheduling folder, run csc -r:System.dll -r:System.Data.dll -r:MySql.Data.dll TeampointScheduling.cs 
4.Execute: In TeampointScheduling folder, run mono TeampointScheduling.exe


==================Config File===============================================
1.Key & value
 (1)Database info: key = server, port, database, userid, password
 (2)Table name: key = job_table, operative_table, travel_time_table, result_table
 (3)Column name example: key = job_table_jobid (column “jobid” in job table)
 (4)“iteration limit”: key = 12 (default). Iteration limit is for function which try to find out more job that can be assigned in a day.
 (5)“drivelimit_sec”: The driving time upper limit (in seconds) for all the driving tasks. Set 0 if there is no limit.
 (6)“drivelimit_ratio”: The driving time upper limit (in ratio). For each possible job to be assigned, its upper driving time limit = job duration * “drivelimit_ratio”. For example, “drivelimit_ratio” is 12, if a job duration is 20 mins, then the driving time upper limit is 240 mins (4 hours). Ratio could be decimals. Set 0 if there is no limit.
 (7)If both drivelimit parameters are set as 0, the script will take 86400 (24 hours) as the driving time upper limit. If both drivelimit parameters are nonzero, the script will take the smaller one as the driving time upper limit.
 (8)“max_miles” column in schedule_operative is the max driving time an operative can do in a day, the format should be in seconds. 

2.Caution with the format settings
 (1)Do not include database name in table name when editing table names
 (2)In operative table, subitem of column “dates” should be “date”, “startSec”, “endSec”, “max_minutes”) due to the format settings.
 (3)In travel time table, operative location should be recorded with “T-”. For example, operative 143’s initial location should be “T-143” due to the format settings.
 (4)The name of config file must be <ApplicationName>.<ApplicationType>.config. In this folder it should be TeampointScheduling.exe.config
 

