# Email Sender

This application is an _Azure Function App_ with an HTTP trigger that provides a way to send email.

## API

To send an email post the following JSON the function end-point:

```json
{
    "from": {
        "name": "...",
        "email": "..."
    },
    "replyTo": {
        "name": "...",
        "email": "..."
    },
    "to": [
        {
            "name": "...",
            "email": "..."
        }        
    ],
    "cc": [
        {
            "name": "...",
            "email": "..."
        }        
    ],
    "bcc": [
        {
            "name": "...",
            "email": "..."
        }        
    ],
    "subject": "...",
    "plainTextBody": "...",
    "htmlBody": "..."
}
```

Some of the properties are required and some are optional. See the source code for details.


## Configuration

To test the app locally you need to add the file `local.settings.json` to the `Functions` project. This file is not commited to git and can contain secrets.

Create four entries for each email address that can be used as a sender. Replace the place-holder `EMAIL-ADDRESS` with the actual email address.

```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "EmailSender:Senders:EMAIL-ADDRESS:ServerName": "...",
        "EmailSender:Senders:EMAIL-ADDRESS:Port": 587,
        "EmailSender:Senders:EMAIL-ADDRESS:SocketOptions": "StartTls",
        "EmailSender:Senders:EMAIL-ADDRESS:Password": "..."
    }
}
```

The Azure Functions App application settings have to be configured with the same values:

| Name | Value |
| - | - |
| `EmailSender:Senders:EMAIL-ADDRESS:ServerName` | The host name of the SMTP server. |
| `EmailSender:Senders:EMAIL-ADDRESS:Port` | The port to connect to (e.g. `587`). |
| `EmailSender:Senders:EMAIL-ADDRESS:SocketOptions` | The secure socket options to use for the connection (e.g. `StartTls`). The value should be one of the values of the `SecureSocketOptions` enumeration in the [MailKit](https://github.com/jstedfast/MailKit) library. |
| `EmailSender:Senders:EMAIL-ADDRESS:Password` | The password of the email account. |
