#!/bin/bash
branch="auditoria-kaizen-handlers-1801421602915829292"
if [[ $branch =~ ^(feat|feature|fix|audit|auditoria(-kaizen)?)/(.+)$ ]]; then
  echo "Match 1"
else
  echo "No match 1"
fi
if [[ $branch =~ ^auditoria(-kaizen)?-.*$ ]]; then
  echo "Match 2"
else
  echo "No match 2"
fi
