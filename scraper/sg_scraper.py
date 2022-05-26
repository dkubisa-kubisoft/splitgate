import datetime
import json
import os
from pathlib import Path
import pyautogui as pag
import requests
from requests.packages.urllib3.exceptions import InsecureRequestWarning
requests.packages.urllib3.disable_warnings(InsecureRequestWarning)
import subprocess
import time
from PIL import Image
from PIL import ImageDraw
import cv2
import pytesseract
pytesseract.pytesseract.tesseract_cmd = r'C:\Program Files\Tesseract-OCR\tesseract.exe'

# All screen locations & sizes based on 1440p resolution
daily_width = 350
daily_height = 48
weekly_width = 350
weekly_height = 77

reward_center_btn = 'assets/RewardCenter_1440p.png'
claim_btn = 'assets/ClaimReward_1440p.png'
daily_check_in_btn = 'assets/DailyCheckIn_1440p.png'


def RunSplitgate():
    print("Starting Splitgate...")
    user_name = os.environ.get("USERNAME")
    sg_path = Path("C:/Users/" + user_name + "/AppData/Roaming/Microsoft/Windows/\"Start Menu\"/Programs/Steam/Splitgate.url")
    os.system( str(sg_path) )


def CloseSplitgate():
    # Kill the Splitgate process
    print("Exiting Splitgate...")
    subprocess.call(["taskkill","/F","/IM","PortalWars-Win64-Shipping.exe"])


def FindButton(buttonPng):
    try:
        btn_loc = pag.locateOnScreen(buttonPng)
        filename = buttonPng.split('\\')[-1]
        if btn_loc == None:
            print(f"Not found on screen: {filename}")
    except:
        print("Not found on screen (exception)")
        return None
    
    return btn_loc
    

def GetToMainMenu():
    """Skip by any pop ups about limited time modes and/or daily streak

    Note: Splitgate Display Mode must be in Fullscreen for this to work. If it's in Borderless Windowed,
    pressing the Esc button moves the mouse to the upper left corner, which triggers
    pyautogui's failsafe to quit out. Esc in Fullscreen moves the mouse to the center of the screen.
    """


    print("Start trying to find main menu")
    reward_center_loc = None
    attempt = 1
    while reward_center_loc == None:
        pag.keyDown('esc')
        time.sleep(0.1)
        pag.keyUp('esc')
        time.sleep(1)
        reward_center_loc = FindButton(reward_center_btn)
        attempt += 1
        if attempt >= 50:
            print("Unable to find main menu. Exiting.")
            exit()

    print("At main menu")
    return reward_center_loc


def SaveDailies():
    print("Saving dailies...")
    #daily_x = 2071
    # OCR on "Play 2 Matches Using Different Armors" gives random words for "Armors" if using 2078
    daily_x = 2076 

    pag.screenshot("ss/daily1.png", region = (daily_x, 200, daily_width, daily_height))
    pag.screenshot("ss/daily2.png", region = (daily_x, 300, daily_width, daily_height))
    pag.screenshot("ss/daily3.png", region = (daily_x, 400, daily_width, daily_height))
    
    print("done")


def IsThursday():
    return datetime.datetime.today().weekday() == 3


def SaveWeeklies():
    """ Assumes current state is at the main menu in Splitgate prior to running

        Note: Navigation to Challenges and Weekly Challenge pages currently hard-coded for 1440p locations
    """
 
    # Refresh Weekly Challenges if this script has not been run yet or if it's a Thursday and new
    # Weekly Challenges are now available
    if IsThursday() or not os.path.exists('ss/weekly1.png'):
        print("Saving new weeklies...")
        # Navigate to Challenges page
        pag.click(1200, 35)
        time.sleep(2)
        # Navigate to Weekly Challenges page
        pag.click(1619, 697)
        time.sleep(2)
        
        # Save Weekly challenges from screen
        weekly_y = 412
        
        weekly1 = pag.screenshot("ss/weekly1.png", region = (98, weekly_y, weekly_width, weekly_height))
        weekly2 = pag.screenshot("ss/weekly2.png", region = (506, weekly_y, weekly_width, weekly_height))
        weekly3 = pag.screenshot("ss/weekly3.png", region = (911, weekly_y, weekly_width, weekly_height))
        weekly4 = pag.screenshot("ss/weekly4.png", region = (1317, weekly_y, weekly_width, weekly_height))
        weekly5 = pag.screenshot("ss/weekly5.png", region = (1720, weekly_y, weekly_width, weekly_height))
        weekly6 = pag.screenshot("ss/weekly6.png", region = (2125, weekly_y, weekly_width, weekly_height))
        print("done")
    else:
        print("Weeklies already up-to-date.")


def SaveCombinedImage(dailies, weeklies):
    """Construct one combined image consisting of daily and weekly challenges

    This was from the initial prototype. Now that text is being OCR-ed and sent to an API, this
    combined image is no longer used.
    """
    im = Image.new('RGB', (weekly_width + 4, (daily_height*3) + (weekly_height*6) + 16 + 2*11))
    im.paste(dailies[0],  (2, 2))
    im.paste(dailies[1],  (2, daily_height + 2*2))
    im.paste(dailies[2],  (2, daily_height*2 + 2*3))
    im.paste(weeklies[0], (2, daily_height*3 + 2*4))
    im.paste(weeklies[1], (2, daily_height*3 + weekly_height + 2*5))
    im.paste(weeklies[2], (2, daily_height*3 + weekly_height*2 + 2*6))
    im.paste(weeklies[3], (2, daily_height*3 + weekly_height*3 + 2*7))
    im.paste(weeklies[4], (2, daily_height*3 + weekly_height*4 + 2*8))
    im.paste(weeklies[5], (2, daily_height*3 + weekly_height*5 + 2*9))
    
    last_updated = "Last Updated " + datetime.datetime.now().strftime("%a %b %d, %Y at %H:%M EST")
    draw = ImageDraw.Draw(im)
    draw.text((10, daily_height*3 + weekly_height*6 + 2*10), last_updated, (255, 255, 255))
    
    im.save("ss/all.png")


def DailyCheckIn(reward_center_loc):
    print("Clicking on Reward Center")
    pag.click(reward_center_loc)
    time.sleep(1)
    daily_loc = FindButton(daily_check_in_btn)
    if daily_loc != None:
        print("Clicking on Daily Check-In")
        pag.moveTo(daily_loc)
        time.sleep(1)
        pag.click(daily_loc)
        time.sleep(2)
        claim_btn_loc = FindButton(claim_btn)
        if claim_btn_loc != None:
            pag.moveTo(claim_btn_loc)
            time.sleep(1)
            print("Clicking on Claim Reward")
            pag.click(claim_btn_loc)
        time.sleep(2)
    
    # Be a good neighbor and put the application back in the state we started in.
    GetToMainMenu()


def ImageToText(fileName):
    img = cv2.imread(fileName)    
    text = pytesseract.image_to_string(img).replace("\n", " ").rstrip()
    return text


def BetaSeason1Challenges():
    """ Input Season Challenges by hand rather than via OCR for now since they change infrequently
    """

    return ['Score Highest on your team in 100 Matches',
    'Play 300 Matches',
    'Play 50 Matches of Featured Game Modes',
    'Get 2,500 Kills',
    'Win 25 Matches in a Party',
    'Land 7500 Shots',
    'Win 10 Matches in 5 Different Playlists',
    'Play 100 Ranked Matches',
    'Inflict 150,000 Damage',
    'Go Through 50 Friendly Portals',
    'Get 35 King Slayer Kills',
    'Get 25 First Blood Kills',
    'Get 1,000 Kills',
    'Win 30 Matches with the Most Kills',
    'Play 10 Matches in 1 Day',
    'Get 150 Headshot Kills',
    'Win 25 Matches with the Highest Score',
    'Have the Most Assist Kills in 5 Matches',
    'Get a Portal Kill',
    'Get 1500 Score in Team Objective',
    'Get 75 Double Kills',
    'Get 100 Kills Through Portals',
    'Get A Collateral Kill',
    'Get 100 Melee Kills',
    'Get 150 Assist Kills',
    'Get 500 Score in Sniper Frenzy',
    'Get the Most Kills on your team in 15 Matches',
    'Get 50 Revenge Kills',
    'Inflict 50,000 Damage',
    'Win a Match in 6 Different Playlists',
    'Win 25 Ranked Matches',
    'Emote in 25 Matches',
    'Win 75 Matches',
    'Win 3 matches of FFA Brawl',
    'Win a Match on 10 Different Maps',
    'Win a Match with 10 Assist Kills',
    'Play 25 Quick Play Matches',
    'Win 5 Matches with Double the Score of the Opposing Team',
    'Get a Triple Kill',
    'Get 50 Kills with the BFB or Railgun',
    'Play 50 Matches',
    'Win a Match with the Highest Score and Most Kills',
    'Get 75 Kills with a Sniper or Shotgun',
    'Get the Highest Score on your team in 20 Matches',
    'Get 100 Kills with SMG or Battle Rifle',
    'Win a Match of 10 Different Game Modes',
    'Get 300 Kills',
    'Get 100 Assist Kills or Revenge Kills']


def DatetimeToString(dt):
    """ Date/time format specified by the PostChallenges API
    """
    return dt.strftime("%Y-%m-%dT%H:%M:%SZ")


def ApiChallenge(type, idx, desc, start_dt, end_dt):
    """ JSON format specified by the PostChallenges API
    """
    return {"challengeType": type, "index": idx, "description": desc, "startDateUtc": DatetimeToString(start_dt), "endDateUtc": DatetimeToString(end_dt) }


def PostToApi():
    """ There is an assumption that this function is run AFTER the Splitgate challenges have updated for the day
    i.e. currently after 4 AM EST
    """

    print("POSTing to API...")
    now = datetime.datetime.now(datetime.timezone.utc)
    
    daily_start_time = datetime.datetime(now.year, now.month, now.day, 8, 0, 0, 0, tzinfo=datetime.timezone.utc)
    daily_end_time = daily_start_time + datetime.timedelta(hours=24) - datetime.timedelta(microseconds=1)
    
    challenges = []

    # Daily Challenges
    for idx in range(3):
        description = ImageToText(f'ss/daily{idx + 1}.png')
        challenges.append( ApiChallenge("daily", idx + 1, description, daily_start_time, daily_end_time) )

    # Weekly Challenges
    # Only send updated Weekly challenges on Thursday (local time) when they refresh
    if IsThursday():
        weekly_start_time = daily_start_time
        # 3 = Thursday
        while weekly_start_time.weekday() != 3:
            weekly_start_time -= datetime.timedelta(hours=24)
        weekly_end_time = weekly_start_time + datetime.timedelta(days=7) - datetime.timedelta(microseconds=1)
        
        for idx in range(6):
            description = ImageToText(f'ss/weekly{idx + 1}.png')
            challenges.append( ApiChallenge("weekly", idx + 1, description, weekly_start_time, weekly_end_time) )

    # Season Challenges
    # This only needed to be sent once for the season. Disabled by default.
    if False:
        season_challenges = BetaSeason1Challenges()
        # Dates hard-coded for Beta Season 1
        season_start_time = datetime.datetime(2022, 1, 1, 8, 0, 0, 0, tzinfo=datetime.timezone.utc)
        season_end_time = datetime.datetime(2022, 6, 2, 8, 0, 0, 0, tzinfo=datetime.timezone.utc)
        for idx in range(len(season_challenges)):
            challenges.append( ApiChallenge("seasonal", idx + 1, season_challenges[idx], season_start_time, season_end_time) )

    json_obj = {}
    json_obj["challenges"] = challenges
    json_str = json.dumps(json_obj)
    #print(json_str)

    # POST JSON data to API
    api_key = os.environ.get('POST_CHALLENGES_API_KEY')
    url = 'https://splitgate-challenge-api.azurewebsites.net/api/PostChallenges?code=' + api_key
    response = requests.post(url, data=json_str, verify=False)
    if response.status_code == 200:
        print('POST successful (200 Response)')
    else:
        print(f"Error POSTing JSON data: '{response}'")
    

def main():

    # Create a folder to store captured screenshots
    if not os.path.exists('ss'):
        os.makedirs('ss')

    RunSplitgate()
    reward_center_loc = GetToMainMenu()
    DailyCheckIn(reward_center_loc)
    SaveDailies()
    SaveWeeklies()
    CloseSplitgate()
    PostToApi()


# Run main
if __name__ == '__main__':
    main()
