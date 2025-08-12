#!/bin/zsh
# Restore survey and questions files after Unity build

# Ensure target directory exists
mkdir -p ./public/assets/Resources

# Copy survey.json (add more files as needed)
cp -f ./Assets/Resources/survey.json ./public/assets/Resources/survey.json

echo "survey.json restored to public/assets/Resources/"
# Copy survey.html
cp -f ./Assets/Resources/survey.html ./public/survey.html

echo "survey.html restored to public/"
