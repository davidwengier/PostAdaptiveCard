#!/bin/sh -l

cd /app

dotnet run --project PostAdaptiveCard/PostAdaptiveCard.csproj -- \
    --webhook-uri "$WEBHOOK_URI" \
    --event-path "$EVENT_PATH"
