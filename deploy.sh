#!/bin/bash

# Deploy only the public folder to Vercel
cd "$(dirname "$0")/public"
echo "Deploying from public folder ($(pwd))"
echo "Contents:"
ls -la

# Deploy to Vercel
vercel --prod