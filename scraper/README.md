# Splitgate application UI scraper

This tool automates the process of opening the Splitgate application, navigating the UI, creating screen captures of the Daily and Weekly Challenges, using Optical Character Recognition (OCR) to convert those into text and then publishing these to an API.

## Build
Instructions are based on a Windows 10 installation running Splitgate via Steam at default installation settings.
Install Python 3.10 (developed using 3.10.4)
Install Python dependencies
- pip3 install pyautogui
- pip3 install pil
Install [Tesseract](https://tesseract-ocr.github.io/tessdoc/Installation.html) for OCR capability.
Script currently assumes installation to: `C:\Program Files\Tesseract-OCR\tesseract.exe`
Install [Splitgate via Steam](https://store.steampowered.com/app/677620/Splitgate/)

**The script will currently only operate correctly at 2560x1440 resolution with the game running in Fullscreen (non-Borderless) mode**

## Run
python sg_scraper.py

## Known Issues
The OCR on the raw images from the Splitgate UI can sometimes produce incorrect data. It is planned to add image preprocessing to improve the OCR performance.

## Roadmap
- Add a requirements.txt
- Add OCR preprocessing
- Update for Beta Season 2 UI changes (post June 2, 2022)
- Updates to allow script to run on any resolution
