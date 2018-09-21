# ![logo](/logo.png) PushServer
A push notification framework for ASP.NET Core 2. It extends a web application with the capability to manage push subscriptions and send notifications to users.

## Introduction
The process of sending push notifactions to users is similar, idependently of which push provider is used (Web Push, Android device push notifications, Azure Notification Hub).

Registration:
1. Client application requests push subscription from vendor push server
1. Client application submits push subscription details to web application
1. Web application stores the details for later use

Send notification:
1. Client application requests notification delivery from push server, including push subscription details, optionally encrypting the payload (depending on the push provider used)
1. Vendor push server delivers the notification to the device

The following figure illustrates the process:
![push notification process](/diagram.png)

The web application therefore needs to receive/store the subscription information, as well as implement the client requesting the notification delivery.
PushServer is a modular approach for managing push subscriptions and sending notifications in an ASP.NET Core web application.
Different strategies for persisting the subscriptions, as well as differnt push providers can be configured.

## Support
Supported subscription persistance layers:
* [EntityFrameworkCore](https://github.com/aspnet/EntityFrameworkCore)

Supported push providers
* [Web Push (PushAPI)](https://developer.mozilla.org/en-US/docs/Web/API/Push_API)
* [Azure Notification Hub](https://docs.microsoft.com/en-us/azure/notification-hubs/)
## Extensibility
TODO
## Getting Started
TODO
## Demo
https://github.com/tuwrraphael/DigitPushService
