#!/bin/bash
TASKS_DIR="docs/TASKS"
KAIZEN_DIR="${TASKS_DIR}/KAIZEN"

PENDING_TASK=$(find "$TASKS_DIR" -maxdepth 1 -name "*.md" -type f | sort | head -n 1)

if [ -n "$PENDING_TASK" ]; then
    echo "Found pending task: $PENDING_TASK"
else
    KAIZEN_TASK=$(find "$KAIZEN_DIR" -maxdepth 1 -name "*.md" -type f | sort | head -n 1)
    if [ -n "$KAIZEN_TASK" ]; then
        echo "Found Kaizen task: $KAIZEN_TASK"
    else
        echo "No pending tasks found. Creating a new Kaizen task."
    fi
fi
