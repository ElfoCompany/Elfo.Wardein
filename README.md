# Elfo.Wardein

<p align="center">
  <a href="https://github.com/ElfoCompany/Elfo.Wardein" target="_blank">
    <img width="100"src="https://i.imgur.com/hwz5xQ5.png">
  </a>
</p>

master  | dev
--- | ---
[![CodeFactor](https://www.codefactor.io/repository/github/elfocompany/elfo.wardein/badge/master)](https://www.codefactor.io/repository/github/elfocompany/elfo.wardein/overview/master) | [![CodeFactor](https://www.codefactor.io/repository/github/elfocompany/elfo.wardein/badge/dev)](https://www.codefactor.io/repository/github/elfocompany/elfo.wardein/overview/dev)

> **warden**: a person responsible for the supervision of a particular place or activity or for enforcing the regulations associated with it.
  
  ![Origin](https://i.imgur.com/nSwTbcd.png)

Elfo.Wardein is a product developed by Elfo in order to manage Windows Services installed on a machine.

It is able to manage the status of a Windows Service.

It can monitor the status and restart a service if it goes down for any reason. This check will be performed every x seconds where x is a value configurable by the user.

If it is not able to restart a service after a certain number of times, it can notify a group of users.

It also exposes some APIs in order to start/stop/refresh Windows Services or IIS application pools.


## Installation

The installation is quite simple. The only step required is to copy/paste the distribution folder into the machine/server to be monitored.

In order to create the distribution folder, clone the GitHub repository on your computer, build it and execute this command in a cmd instance.

```cmd
> dotnet publish -r win10-x64 --output bin/dist/win
```

Of course the cmd must point to the Elfo.Wardein solution folder.

The distribution folder will be created here: 

> C:\Users\mirko.bellabarba\source\repos\Elfo.Wardein\Elfo.Wardein\bin\dist\win

Copy/Paste the content of this folder anywhere you need to install Elfo.Wardein.

## Configuration files

Configuration files are placed inside the Asset folder. The Asset folder is created inside the distribution folder.

For develope purpouses you can also place some dummy files inside the Visual Studio solution with the "Copy Always" option enabled. This files are added to the .gitignore, therefore make sure to not commit any of these file on GitHub.

### WardeinConfig.json

NAME  | TYPE | COMMENT
--- | --- | ---
timeSpanFromSeconds | Int | Defines the number of seconds between every check
sendRepeatedNotificationAfterSeconds | Int | Values > 0 => Notification will be sent after `x` seconds have passed since the last sent notification<br>-Value = 0 or -1 => Notifications will be sent always
numberOfNotificationsWithoutRateLimitation | Int | - Values > 0 => `sendRepeatedNotificationAfterSeconds` parameter will be ignored until a certain number of notifications are sent.<br>- Values = 0 or -1 => System is not limited, notification will be sent every time the service is down for `services/maxRetryCount` in a row.
services | Array<Service> | List of services to be checked at every cycle
services/serviceName | String | Name of the service to be checked at every cycle
services/maxRetryCount | Int | Number of service restart to be attempted before sending a notification
services/failMessage | String | Body of the notification to be sent when Elfo.Wardein is not able to restart the service after a specified number of attempts
services/restoredMessage | String | Body of the message to be sent when the service is restarted succesfully by Elfo.Wardein
services/recipientAddress | String | WebHook url or mail of the recipients (separated by ‘;’ in case of multiple recipients
services/notificationType | String | Type of notification to be sent. Currently Elfo.Wardein supports:<br>-	Mail<br>-	Teams
services/sendRepeatedNotificationAfterSeconds | Int [Optional] | Overrides the corrispettive global setting for this service only
services/numberOfNotificationsWithoutRateLimitation | Int [Optional] | Overrides the corrispettive global setting for this service only
persistenceType | String | Where to store Elfo.Wardein data/status. Currently Elfo.Wardein supports:<br>-	JSON
maintenanceModeStatus | Element | Maintenance mode status. This setting is managed by Elfo.Wardein and must not be modified by the user
maintenanceModeStatus/durationInSeconds | Int | Maintenance mode timeout. This setting is managed by Elfo.Wardein and must not be modified by the user
maintenanceModeStatus/isInMaintenanceMode | Bool | True if maintenance mode is enabled. This setting is managed by Elfo.Wardein and must not be modified by the user
maintenanceModeStatus/ maintenanceModeStartDateInUTC | DateTime | Maintenance mode starting date. This setting is managed by Elfo.Wardein and must not be modified by the user

### MailConfig.json

NAME  | TYPE | COMMENT
--- | --- | ---
host | String | SMTP host
port | Int | SMTP port number
enableSSL | Bool | True if the SMTP server equires a SSL connection
useDefaultCredentials | Bool | True if the SMTP server accepts default credentials
fromAddress | String | The address capable of sending emails
fromDisplayName | String | Name of the sender
username | String [Optional] | Username used for the SMTP authentication
password | String [Optional] | Password used for the SMTP authentication

### WardeinDB.json

This is the configuration file where Elfo.Wardein persist its status (if `jsonPersistenceType` is equal to `JSON`). There is no need to open this file and it is suggested to not touch it. If it is not present it will be automatically created by Elfo.Wardein

## Running mode

Elfo.Wardein is capable to run in two different mode

### Running mode: Console

By entering into the Elfo.Wardein folder, you can run it in console mode by double clicking on Elfo.Wardein.exe file:
 
Elfo.Wardein will work until the console window remains open. Therefore, the advice is to use this mode only for debug reason.

### Running Mode: Service

By executing this command from the Elfo.Wardein folder, it is possible to install Elfo.Wardein as a Windows Service. This will ensure that Elfo.Wardein will be started during the server startup phase.

```cmd
> Elfo.Wardein.exe action:install name:Elfo.Wardein.Service description:Elfo.Wardein.Service
```

Moreover, it is possible to manage Elfo.Wardein as all the windows service available on the system and it is the suggested way in order to run the program for long time period.

## Notifications

Elfo.Wardein is capable of notificate the user if the checked services are down.

Currently Elfo.Wardein can send:

- “Fail Notification”: sent every time a service goes down and it is not possible to restore it after a certain number of retries
- “Restore Notification”: sent every time Elfo.Wardein is able to restore succesfully a service.

The Supported notification types are:

- Mail: Requires MailConfig.json configuration files and allows Elfo.Wardein to send a notification through a SMTP server
- Teams: By createing a Microsoft Teams web hooks it is possible to receive these notifications directly in Microsoft Teams.

Notifications can be limited through these parameters:
- services/maxRetryCount
- services/sendRepeatedNotificationAfterSeconds
- services/numberOfNotificationsWithoutRateLimitation

## Maintenance Mode

Elfo.Wardein can be set in manteinance mode.

When this mode is activated, it will remain enabled for a certain number of seconds (default: 300). This number can be changed during the maintenance mode activation.
If the maintenance mode is not stopped after this period of time, it will be automatically disabled by the system.

The maintenance mode can be started/stopped through an API call.
During this period, both Elfo.Wardein APIs (apart from the ones related to the maintenance mode) and services periodic check are stopped.

## APIs specification

Elfo.Wardein expose a self hosted server inside its service.

The APIs available at http://localhost:5000/api are the following ones:

---

### Wardein Status

>	Return a simple message if Elfo.Wardein is available

>	**GET** http://localhost:5000/api/1.0/status

#### REQUEST PARAMETER

None

#### RESPONSE

Return this text if system are available: `Wardein APIs are available`

---

### Restart Windows Service

>	Restarts the specified Windows service if present

>	**GET** http://localhost:5000/api/1.0/ws/restart/{name}

#### REQUEST PARAMETER

NAME  | TYPE  | COMMENT
--- | --- | ---
name  | String  | Name of the Windows Service to be restarted

#### RESPONSE

In case of success: `Service {serviceName} restarted`

Otherwise it will return the exception message.

---

### Stop Windows Service

>	Stops the specified Windows service if present

>	**GET** http://localhost:5000/api/1.0/ws/stop/{name}

#### REQUEST PARAMETER

NAME  | TYPE  | COMMENT
--- | --- | ---
name  | String  | Name of the Windows Service to be stopped

#### RESPONSE

In case of success: `Service {serviceName} stopped`

Otherwise it will return the exception message.

---

### Start Windows Service

>	Starts the specified Windows service if present

>	**GET** http://localhost:5000/api/1.0/ws/start/{name}

#### REQUEST PARAMETER

NAME  | TYPE  | COMMENT
--- | --- | ---
name  | String  | Name of the Windows Service to be started

#### RESPONSE

In case of success: `Service {serviceName} started`

Otherwise it will return the exception message.

---

### Get Windows Service status

>	Returns the specified Windows service status if present

>	**GET** http://localhost:5000/api/1.0/ws/status/{name}

#### REQUEST PARAMETER

NAME  | TYPE  | COMMENT
--- | --- | ---
name  | String  | Name of the Windows Service to query

#### RESPONSE

In case of success: `{WindowsServiceStatus}`

Otherwise it will return the exception message.

---

### Restart IIS Pool

>	Restarts the specified IIS pool if present

>	**GET** http://localhost:5000/api/1.0/pool/refresh/{name}

#### REQUEST PARAMETER

NAME  | TYPE  | COMMENT
--- | --- | ---
name  | String  | Name of the IIS pool to be restarted

#### RESPONSE

In case of success: `ApplicationPool {applicationPoolName} restarted`

Otherwise it will return the exception message.

---

### Stop IIS Pool

>	Stops the specified IIS pool if present

>	**GET** http://localhost:5000/api/1.0/pool/stop/{name}

#### REQUEST PARAMETER

NAME  | TYPE  | COMMENT
--- | --- | ---
name  | String  | Name of the IIS pool to be stopped

#### RESPONSE

In case of success: `ApplicationPool {applicationPoolName} stopped`

Otherwise it will return the exception message.

---

### Start IIS Pool

>	Starts the specified IIS pool if present

>	**GET** http://localhost:5000/api/1.0/pool/start/{name}

#### REQUEST PARAMETER

NAME  | TYPE  | COMMENT
--- | --- | ---
name  | String  | Name of the IIS pool to be started

#### RESPONSE

In case of success: `ApplicationPool {applicationPoolName} started`

Otherwise it will return the exception message.

---

### Get IIS Pool status

>	Returns the specified IIS pool status if present

>	**GET** http://localhost:5000/api/1.0/pool/status/{name}

#### REQUEST PARAMETER

NAME  | TYPE  | COMMENT
--- | --- | ---
name  | String  | Name of the IIS pool to query

#### RESPONSE

In case of success: `{IISPoolStatus}`

Otherwise it will return the exception message.

---

### Restart both Windows Service and IIS Application pool

>	Restarts the specified Windows Service and IIS pool status if present

>	**GET** http://localhost:5000/api/1.0/wspool/restart/{servicename}/{iispoolname} 

#### REQUEST PARAMETER

NAME  | TYPE  | COMMENT
--- | --- | ---
servicename  | String  | Name of the Windows Service to be restarted
iispoolname  | String  | Name of the IIS pool to be restarted

#### RESPONSE

In case of success: `Service {serviceName} restarted, applicationPool {applicationPoolName} restarted`

Otherwise it will return the exception message.

---

### Start maintenance mode

>	Enables Elfo.Wardein maintenance mode

>	**GET** http://localhost:5000/api/1.0/maintenance/start/{durationInSeconds}

#### REQUEST PARAMETER

NAME  | TYPE  | COMMENT
--- | --- | ---
durationInSeconds  | Int  | Maintenance mode timeout in seconds. After this period is elapsed, Elfo.Wardein will disable the maintenance mode  

#### RESPONSE

In case of success: `Maintenance Mode Started`

Otherwise it will return the exception message.

---

### Stop maintenance mode

>	Disables Elfo.Wardein maintenance mode

>	**GET** http://localhost:5000/api/1.0/maintenance/stop

#### REQUEST PARAMETER

None

#### RESPONSE

In case of success: `Maintenance Mode Stopped`

Otherwise it will return the exception message.

---

### Get maintenance mode status

>	Enables Elfo.Wardein maintenance mode

>	**GET** http://localhost:5000/api/1.0/maintenance/status

#### REQUEST PARAMETER

None

#### RESPONSE

In case of manteinance mode started: `Wardein is in manteinance mode`

In case of manteinance mode stopped: `Wardein is not in manteinance mode`

Otherwise it will return the exception message.

---

### Invalidate cached configurations

>	Allows to invalidate cached configurations inside Elfo.Wardein. Usefull if it is necessary to change a configuration file.

>	**GET** http://localhost:5000/api/1.0/configs/invalidate

#### REQUEST PARAMETER

None

#### RESPONSE

In case of manteinance mode started: `Cached configs invalidated`

Otherwise it will return the exception message.

---

### Stop service periodic check

>	Allows to stop the service periodic check without interrupting or stopping the service

>	**GET** http://localhost:5000/api/1.0/polling/stop 

#### REQUEST PARAMETER

None

#### RESPONSE

In case of manteinance mode started: `Periodic check stopped`

Otherwise it will return the exception message.

---

### Restart service periodic check

>	Allows to start or restart the service periodic check when it’s stopped.

>	**GET** http://localhost:5000/api/1.0/polling/restart  

#### REQUEST PARAMETER

None

#### RESPONSE

In case of manteinance mode started: `Periodic check restarted`

Otherwise it will return the exception message.

##	Logging	

Elfo.Wardein is capable of logging what happens inside it in the console windows (when started as a console) or on a file persisted on disk.
The file is available inside the “Logs” folder. 

A new log file will be created every day or each time the current file is larger than 10MB.

Log files are available up to 10 days, then they will be deleted.

All these values can be modified if needed inside the log4net.config file.
