# Post Adaptive Card #

This is a GitHub Action that posts an Adaptive Card in response to an issue or pull request event on GitHub, usually used for notifications about issue and pull request creation.

For more information on adaptive cards see [https://adaptivecards.io/](https://adaptivecards.io/)

## Inputs ##

* `webhook-uri`: URI to the webhook to send the card to. If this is blank, the JSON payload will be written to stdout
* `event-name`: The name of the event, defaults to the GITHUB_EVENT_NAME environment variable
* `event-path`: The path to the JSON payload for the event, defaults to the GITHUB_EVENT_PATH  environment variable

## Example Usage ##

```yaml
name: Post PRs to Teams

on:
  pull_request:
    types: [opened, reopened]

steps:
  name: Notify
  uses: davidwengier/PostAdaptiveCard@v0.1.0
  with:
    webhook-uri: <Webhook URI>
```
