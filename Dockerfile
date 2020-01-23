FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

LABEL "com.github.actions.name"="Post Adaptive Card"
LABEL "com.github.actions.description"="Posts an Adaptive Card payload to the specified webhook in response to a GitHub action (usually issue or PR creation)"
LABEL "com.github.actions.icon"="zap"
LABEL "com.github.actions.color"="blue"

LABEL "repository"="http://github.com/davidwengier/PostAdaptiveCard"
LABEL "homepage"="http://github.com/davidwengier"
LABEL "maintainer"="David Wengier <adaptivecards@wengier.com>"

COPY PostAdaptiveCard/ ./PostAdaptiveCard/

ADD entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

ENTRYPOINT ["/entrypoint.sh"]
