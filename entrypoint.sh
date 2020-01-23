#!/bin/sh -l

cd /app

dotnet run --project PostAdaptiveCard/PostAdaptiveCard.csproj -- \
    --webhook-uri "$WEBHOOK_URI" \
    --event-name "$EVENT_NAME" \
    --event-path "$EVENT_PATH"