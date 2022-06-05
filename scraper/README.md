# Splitgate application UI scraper

This tool automates the process of opening the Splitgate application, navigating the UI, creating screen captures of the Daily and Weekly Challenges, using Optical Character Recognition (OCR) to convert those captures into text, calculating challenge start & end times and then publishing challenge data to the [API](../api/).

## Build
Instructions are based on a Windows 10 installation running Splitgate via Steam at default installation settings.

**The script will currently only operate correctly at 2560x1440 resolution with the game running in Fullscreen (non-Borderless) mode**

- Install Python 3.10 (developed using 3.10.4)
- Install Python dependencies
  - pip3 install pyautogui
  - pip3 install pil
- Install [Tesseract](https://tesseract-ocr.github.io/tessdoc/Installation.html) for OCR capability.
Script currently assumes installation to: `C:\Program Files\Tesseract-OCR\tesseract.exe`
- Install [Splitgate via Steam](https://store.steampowered.com/app/677620/Splitgate/)

## Run

`python sg_scraper.py`

## Roadmap
- Add a requirements.txt
- Remove deprecated SaveCombinedImage() and corresponding PIL dependency in code and README.md
- Update to support any screen resolution
