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
import cv2
import pytesseract
pytesseract.pytesseract.tesseract_cmd = r'C:\Program Files\Tesseract-OCR\tesseract.exe'
import matplotlib.pyplot as plt

play_tab_selected = 'assets/PlayTabSelected_1440p.png'
reward_center_btn = 'assets/RewardCenter_1440p.png'
daily_check_in_btn = 'assets/DailyCheckIn_1440p.png'
claim_btn = 'assets/ClaimReward_1440p.png'


def RunSplitgate():
    print("Starting Splitgate...")
    user_name = os.environ.get("USERNAME")
    sg_path = Path("C:/Users/" + user_name + "/AppData/Roaming/Microsoft/Windows/\"Start Menu\"/Programs/Steam/Splitgate.url")
    os.system( str(sg_path) )


def CloseSplitgate():
    # Kill the Splitgate process
    print("Exiting Splitgate...")
    subprocess.call(["taskkill","/F","/IM","PortalWars-Win64-Shipping.exe"])


def FindOnScreen(png):
    try:
        loc = pag.locateOnScreen(png)
        filename = png.split('\\')[-1]
        if loc == None:
            print(f"Not found on screen: {filename}")
    except:
        print("Not found on screen (exception)")
        return None
    
    return loc
    

def GetToMainMenu():
    """Skip by any pop ups about limited time modes and/or daily streak

    Note: Splitgate Display Mode must be in Fullscreen for this to work. If it's in Borderless Windowed,
    pressing the Esc button moves the mouse to the upper left corner, which triggers
    pyautogui's failsafe to quit out. Esc in Fullscreen moves the mouse to the center of the screen.
    """


    print("Start trying to find main menu")
    play_tab_loc = None
    attempt = 1
    while play_tab_loc == None:
        pag.keyDown('esc')
        time.sleep(0.1)
        pag.keyUp('esc')
        time.sleep(1)
        play_tab_loc = FindOnScreen(play_tab_selected)
        attempt += 1
        if attempt >= 50:
            print("Unable to find main menu. Exiting.")
            exit()

    print("At main menu")


def SaveDailies():
    print("Saving dailies...")

    # Navigate to Challenges page
    time.sleep(1)
    pag.click(1178, 31)
    time.sleep(2)
    # Navigate to Daily Challenges page
    pag.click(2247, 567)
    time.sleep(2)

    #daily_x = 1990
    daily_y = 595
    daily_width = 765
    daily_height = 70

    pag.screenshot("ss/daily1.png", region = (100, daily_y, daily_width, daily_height))
    pag.screenshot("ss/daily2.png", region = (900, daily_y, daily_width, daily_height))
    pag.screenshot("ss/daily3.png", region = (1690, daily_y, daily_width, daily_height))
    
    print("done")


def IsThursday():
    return datetime.datetime.now(datetime.timezone.utc).weekday() == 3


def SaveWeeklies():
    """ Assumes current state is at the main menu in Splitgate prior to running

        Note: Navigation to Challenges and Weekly Challenge pages currently hard-coded for 1440p locations
    """
 
    # Refresh Weekly Challenges if this script has not been run yet or if it's a Thursday and new
    # Weekly Challenges are now available
    if IsThursday() or not os.path.exists('ss/weekly1.png'):
        print("Saving new weeklies...")
        # Navigate to Challenges page
        time.sleep(1)
        pag.click(1178, 31)
        time.sleep(2)
        # Navigate to Weekly Challenges page
        pag.click(1518, 544)
        time.sleep(2)
        
        # Save Weekly challenges from screen
        weekly_y = 575
        weekly_width = 365
        weekly_height = 80
        
        weekly1 = pag.screenshot("ss/weekly1.png", region = (118, weekly_y, weekly_width, weekly_height))
        weekly2 = pag.screenshot("ss/weekly2.png", region = (511, weekly_y, weekly_width, weekly_height))
        weekly3 = pag.screenshot("ss/weekly3.png", region = (904, weekly_y, weekly_width, weekly_height))
        weekly4 = pag.screenshot("ss/weekly4.png", region = (1297, weekly_y, weekly_width, weekly_height))
        weekly5 = pag.screenshot("ss/weekly5.png", region = (1690, weekly_y, weekly_width, weekly_height))
        weekly6 = pag.screenshot("ss/weekly6.png", region = (2083, weekly_y, weekly_width, weekly_height))
        print("done")
    else:
        print("Weeklies already up-to-date.")


def DailyCheckIn():

    # Bring up the menu
    pag.keyDown('esc')
    time.sleep(0.1)
    pag.keyUp('esc')
    
    # Find and press the Reward Center button
    reward_center_loc = FindOnScreen(reward_center_btn)
    if reward_center_loc != None:
        print("Clicking on Reward Center")
        pag.moveTo(reward_center_loc)
        time.sleep(1)
        pag.click(reward_center_loc)
        time.sleep(1)

        daily_loc = FindOnScreen(daily_check_in_btn)
        if daily_loc != None:
            print("Clicking on Daily Check-In")
            pag.moveTo(daily_loc)
            time.sleep(1)
            pag.click(daily_loc)
            time.sleep(2)
            claim_btn_loc = FindOnScreen(claim_btn)
            if claim_btn_loc != None:
                pag.moveTo(claim_btn_loc)
                time.sleep(1)
                print("Clicking on Claim Reward")
                pag.click(claim_btn_loc)
            time.sleep(1)
    
    # Be a good neighbor and put the application back in the state we started in.
    GetToMainMenu()


def ImageToText(fileName):

    debug = []
    img = cv2.imread(fileName)    
    debug.append( ("Original", img) )

    # Convert to grayscale. Must be done before thresholding.
    img_gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
    debug.append( ("Grayscale", img_gray) )
    
    # Find & use best threshold based on confidence level
    best_min_prob = 0.0
    best_text = ""
    char_whitelist = '"abcdefghijklmnopqrstuvwxyz ABCDEFGHIJKLMNOPQRSTUVWXYZ,0123456789"'
    for thresh in range(150, 181, 10):
        # Text is white, so invert to black text on white background for thresholding
        img_thresh = cv2.threshold(img_gray, thresh, 255, cv2.THRESH_BINARY_INV)[1]
        d = pytesseract.image_to_data(img_thresh, output_type=pytesseract.Output.DICT,
                config=f'-c tessedit_char_whitelist={char_whitelist} --psm 6')
        try:
            df = [float(i) for i in d['conf']]
            probs = [prob for prob in df if prob >= 0]
            min_prob = min(probs)
            text = pytesseract.image_to_string(img_thresh)
            text = text.replace("\n", " ").rstrip()
            debug.append( (f"Thresh:{thresh}  Min Prob:{min_prob:.0f}  Text:'{text}'", img_thresh) )
            # >1 => If threshold is too high or too low, the system will be very confident with a
            # one word solution, which is never valid
            if min_prob > best_min_prob and len(probs) > 1:
                if text != "":
                    best_text = text
                    best_min_prob = min_prob
                    #print(f'{thresh} : {min_prob}  "{text}"  {probs}')
        except Exception as e:
            #print(f"Exception at threshold {thresh}: {e}")
            pass

    show_debug = False
    if show_debug:
        fig, axes = plt.subplots(len(debug), 1, figsize=(10,len(debug)*1.25))
        for idx, image in enumerate(debug):
            axes[idx].imshow(cv2.cvtColor(image[1], cv2.COLOR_BGR2RGB))
            axes[idx].set_title(image[0])
            axes[idx].get_xaxis().set_visible(False)
            axes[idx].get_yaxis().set_visible(False)
        plt.tight_layout()
        plt.show()

    print(best_text)
    return best_text


def DatetimeToString(dt):
    """ Date/time format specified by the PostChallenges API
    """
    return dt.strftime("%Y-%m-%dT%H:%M:%SZ")


def ApiChallenge(type, idx, desc, start_dt, end_dt):
    """ JSON format specified by the PostChallenges API
    """
    return {"challengeType": type, "index": idx, "description": desc, "startDateUtc": DatetimeToString(start_dt), "endDateUtc": DatetimeToString(end_dt) }


def PostToApi(json_obj, preserve_challenge_completions = False):
    # POST JSON data to API
    print("POSTing to API...")
    
    if preserve_challenge_completions:
        json_obj["suppressCompletionPurge"] = True

    json_str = json.dumps(json_obj)

    api_key = os.environ.get('POST_CHALLENGES_API_KEY')
    url = 'https://splitgate-challenge-api.azurewebsites.net/api/PostChallenges?code=' + api_key
    response = requests.post(url, data=json_str, verify=False)
    if response.status_code == 200:
        print('POST successful (200 Response)')
    else:
        print(f"Error POSTing JSON data: '{response}'")


def CalculateChallengeData():
    """ There is an assumption that this function is run AFTER the Splitgate challenges have updated for the day
    i.e. currently after 4 AM EST
    """

    print("Calculating challenge data...")
    now = datetime.datetime.now(datetime.timezone.utc)
    
    daily_start_time = datetime.datetime(now.year, now.month, now.day, 3, 0, 0, 0, tzinfo=datetime.timezone.utc)
    # Handle case where this script is run between 12AM - 3AM UTC (8PM - 11PM EST)
    if now.hour < 3:
        daily_start_time -= datetime.timedelta(days=1)

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

    json_obj = {}
    json_obj["challenges"] = challenges

    print("done")
    return json_obj


def PostSeasonChallenges():
    
    season_challenges = [
        # Stage 1
        'Reach Pro level 1',
        'Play 300 Matches',
        'Get 3,000 Kills',
        'Unlock a badge',
        'Inflict 150,000 Damage',
        'Get 200 Headshots',
        'Win 30 matches in TDM',
        'Play 50 Ranked Matches',
        'Shut down 100 enemy portals',
        'Get 1 Collateral Kill',
        'Get 50 Revenge kills',
        'Get 300 kills with the AR',
        # Stage 2
        'Get 75 BFB Kills',
        'Play 30 matches in the Featured Playlist',
        'Get 100 kills with the Plasma Rifle',
        'Get 150 Headshot Kills',
        'Win 75 Matches',
        'Have a positive KD/A in 10 matches',
        'Go through 100 friendly portals',
        'Win 50 rounds in Takedown',
        'Unlock 3 badges',
        'Win a match in 7 different playlists',
        'Get a Quad Kill',
        'Win a match with the Highest Score and Most Kills']

    # Dates hard-coded for Beta Season 1
    season_start_time = datetime.datetime(2022, 6, 2, 8, 0, 0, 0, tzinfo=datetime.timezone.utc)
    season_end_time = datetime.datetime(2022, 9, 2, 8, 0, 0, 0, tzinfo=datetime.timezone.utc)
    challenges = []
    for idx in range(len(season_challenges)):
        challenges.append( ApiChallenge("seasonal", idx + 1, season_challenges[idx], season_start_time, season_end_time) )
    
    challenges_json = {}
    challenges_json["challenges"] = challenges
    PostToApi(challenges_json, preserve_challenge_completions = True)


def main():

    # Create a folder to store captured screenshots
    if not os.path.exists('ss'):
        os.makedirs('ss')

    RunSplitgate()
    GetToMainMenu()
    DailyCheckIn()
    SaveDailies()
    GetToMainMenu()
    SaveWeeklies()
    CloseSplitgate()
    challenges = CalculateChallengeData()
    PostToApi(challenges)


# Run main
if __name__ == '__main__':
    main()

    # Manually run this once every 3 weeks when new Seasonals show up
    #PostSeasonChallenges()
