# Installing the Service
After you create and build the application, you can install it by running the command-line utility InstallUtil.exe and passing the path to the service's executable file. You can then use the Services Control Manager to start, stop, pause, resume, and configure your service. You can also accomplish many of these same tasks in the Services node in Server Explorer or by using the ServiceController class.

## But further down, is this
- Projects containing Windows services must have installation components for the project and its services.
- This can be easily accomplished from the Properties window. For more information, see How to: Add Installers to Your Service Application.


[Microsoft Article](https://learn.microsoft.com/en-us/dotnet/framework/windows-services/introduction-to-windows-service-applications)

--- 
# Design
## Threading:

- Instead of running all your work on the main thread, you can run tasks by using background worker threads. For more information, see: 
[System.ComponentModel.BackgroundWorker](https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.backgroundworker)


## Set service status
Services report their status to the Service Control Manager so that a user can tell whether a service is functioning correctly. By default, a service that inherits from ServiceBase reports a limited set of status settings, which include SERVICE_STOPPED, SERVICE_PAUSED, and SERVICE_RUNNING. If a service takes a while to start up, it's useful to report a SERVICE_START_PENDING status.

You can implement the SERVICE_START_PENDING and SERVICE_STOP_PENDING status settings by adding code that calls the Windows SetServiceStatus function.

[How to: Set Service Status](https://learn.microsoft.com/en-us/dotnet/framework/windows-services/walkthrough-creating-a-windows-service-application-in-the-component-designer#set-service-status)

## Configuration

Use the ControlService form to have a frontend to create settings.  Settings are stored in an xml file and read by the service at startup.  It should include the following functionality:

- Who the target user is.  This should read from the system somehow and populate users that are available to pick from.  If a user is selected and configured, they should be added to the xml configuration.  "Add new user"
- What time they should be logged out
- How many hours each day that user should be allowed to use the PC running the service

This form must be protected and require administrator rights to run.

## Enforcing disabled login

There are two ideas for how to handle this currently:

- Catch logins with the OnSessionChange event handler - prevent them if time has expired here
- Set accounts disabled at time of forced logout - this however will require regular polling and a way to determine current vs. next day in resetting limits.  More to code, but may be necessary if the above is not successful.

## Runtime
- Warn the user at certain intervals (specify intervals in the settings?)

