# Post Adaptive Card #

This is a GitHub Action that posts an Adaptive Card in response to an issue or pull request event on GitHub, usually used for notifications about issue and pull request creation.

For more information on adaptive cards see [https://adaptivecards.io/](https://adaptivecards.io/)

## Inputs ##

* `webhook-uri`: URI to the webhook to send the card to. If this is blank, the JSON payload will be written to stdout
* `event-name`: The name of the event, defaults to the GITHUB_EVENT_NAME environment variable
* `event-path`: The path to the JSON payload for the event, defaults to the GITHUB_EVENT_PATH  environment variable

## Example Usage ##

The following YAML file will create an action that posts notification cards whenever an issue or PR is opened or reopened, to the specified webhook URI.

```yaml
name: Post PRs and issues to Teams

on:
  pull_request:
    types: [opened, reopened]
  issues:
    types: [opened, reopened]

jobs:
  notify:
    runs-on: ubuntu-latest
    
    steps:
      - name: Notify
        uses: davidwengier/PostAdaptiveCard@v1.0.0
        with:
          webhook-uri: <insert your webhook URI here>
```
