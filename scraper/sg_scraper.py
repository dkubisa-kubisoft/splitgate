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
challenges_screen = 'assets/ChallengesScreen_1440p.png'
daily_challenges_screen = 'assets/DailyChallengesScreen_1440p.png'
weekly_challenges_screen = 'assets/WeeklyChallengesScreen_1440p.png'


def RunSplitgate():
    print("Starting Splitgate...")
    user_name = os.environ.get("USERNAME")
    sg_path = Path("C:/Users/" + user_name + "/AppData/Roaming/Microsoft/Windows/\"Start Menu\"/Programs/Steam/Splitgate.url")
    os.system( str(sg_path) )


def CloseSplitgate():
    # Kill the Splitgate process
    print("Exiting Splitgate...")
    subprocess.call(["taskkill","/F","/IM","PortalWars-Win64-Shipping.exe"])


def FindOnScreen(png, region=None):
    try:
        loc = pag.locateOnScreen(png, region=region)
        if loc == None:
            filename = png.split('\\')[-1]
            print(f"Not found on screen: {filename}")
    except:
        print("Not found on screen (exception)")
        return None
    
    return loc


def WaitForPageToLoad(screenImage, region=None):
    loc = None
    while( loc == None):
        time.sleep(0.1)
        loc = FindOnScreen(screenImage, region)
        
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
        # Force focus on Splitgate app by clicking somewhere with no buttons
        pag.moveTo( (500, 0) )
        pag.click( (500, 0) )
        pag.keyDown('esc')
        time.sleep(0.1)
        pag.keyUp('esc')
        time.sleep(0.1)
        play_tab_loc = FindOnScreen(play_tab_selected, region=(650, 0, 250, 100) )
        attempt += 1
        if attempt >= 500:
            print("Unable to find main menu. Exiting.")
            exit()

    print("At main menu")


def SaveDailies():
    print("Saving dailies...")
    GetToMainMenu()

    # Navigate to Challenges page
    pag.moveTo(1178, 31)
    pag.click(1178, 31)
    WaitForPageToLoad(challenges_screen, region=(50, 100, 650, 150))
    
    # Navigate to Daily Challenges page
    pag.moveTo(2247, 567)
    pag.click(2247, 567)
    # Move cursor off of challenges, as it highlights it with moving speckles that interfere with OCR
    pag.moveTo(10, 10)
    WaitForPageToLoad(daily_challenges_screen, region=(30, 80, 350, 200))

    #daily_x = 1990
    daily_y = 595
    daily_width = 765
    daily_height = 70

    pag.screenshot("ss/daily1.png", region = (100, daily_y, daily_width, daily_height))
    pag.screenshot("ss/daily2.png", region = (900, daily_y, daily_width, daily_height))
    pag.screenshot("ss/daily3.png", region = (1690, daily_y, daily_width, daily_height))
    
    print("Daily Challenges images saved.")


def ShouldUpdateWeeklies():
    return datetime.datetime.now(datetime.timezone.utc).weekday() == 3


def SaveWeeklies():
    """ Assumes current state is at the main menu in Splitgate prior to running

        Note: Navigation to Challenges and Weekly Challenge pages currently hard-coded for 1440p locations
    """
 
    # Refresh Weekly Challenges if this script has not been run yet or if it's a Thursday and new
    # Weekly Challenges are now available
    if ShouldUpdateWeeklies() or not os.path.exists('ss/weekly1.png'):
        print("Saving new weeklies...")
        GetToMainMenu()
        # Navigate to Challenges page
        pag.moveTo(1178, 31)
        pag.click(1178, 31)
        WaitForPageToLoad(challenges_screen, region=(50, 100, 650, 150))

        # Navigate to Weekly Challenges page
        pag.moveTo(1518, 544)
        pag.click(1518, 544)
        # Move cursor off of challenges, as it highlights it with moving speckles that interfere with OCR
        pag.moveTo(10, 10)
        WaitForPageToLoad(weekly_challenges_screen, region=(50, 100, 400, 150))
        
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
        print("Weekly Challenges images saved.")
    else:
        print("Weeklies already up-to-date.")


def DailyCheckIn():

    GetToMainMenu()

    # Bring up the menu
    pag.keyDown('esc')
    time.sleep(0.1)
    pag.keyUp('esc')
    
    # Find and press the Reward Center button
    reward_center_loc = WaitForPageToLoad(reward_center_btn, region=(50, 100, 400, 100) )
    print("Clicking on Reward Center")
    pag.moveTo(reward_center_loc)
    pag.click(reward_center_loc)

    daily_loc = WaitForPageToLoad(daily_check_in_btn, region=(100, 300, 450, 150) )
    print("Clicking on Daily Check-In")
    pag.moveTo(daily_loc)
    pag.click(daily_loc)
    
    # TODO: Possibly add a "WaitForPageToLoad" to ensure we're on Check-in page before looking for Claim button
    claim_btn_loc = FindOnScreen(claim_btn, region=(1500, 950, 1000, 300))
    if claim_btn_loc != None:
        print("Clicking on Claim Reward")
        pag.moveTo(claim_btn_loc)
        pag.click(claim_btn_loc)
    else:
        print("No Claim button to click")
    

def ImageToText(fileName, showDebug=False):

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
    thresholds = range(150, 181, 10)
    if img[1][1][0] < 100: # Completed challenge is more yellow than blue
        thresholds = range(240, 204, -10)
    for thresh in thresholds:
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
            debug.append( (f"Thresh:{thresh}  Min Prob:{min_prob:.0f}%  Text:'{text}'", img_thresh) )
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

    if showDebug:
        fig, axes = plt.subplots(len(debug), 1, figsize=(10,len(debug)*1.25))
        for idx, image in enumerate(debug):
            axes[idx].imshow(cv2.cvtColor(image[1], cv2.COLOR_BGR2RGB))
            axes[idx].set_title(image[0])
            axes[idx].get_xaxis().set_visible(False)
            axes[idx].get_yaxis().set_visible(False)
        plt.tight_layout()
        plt.show()

    return best_text


def DatetimeToString(dt):
    """ Date/time format specified by the PostChallenges API
    """
    return dt.strftime("%Y-%m-%dT%H:%M:%SZ")


def ApiChallenge(type, idx, desc, start_dt, end_dt, stage=0):
    """ JSON format specified by the PostChallenges API
    """
    if stage == 0:
        return {"challengeType": type, "index": idx, "description": desc, "startDateUtc": DatetimeToString(start_dt), "endDateUtc": DatetimeToString(end_dt) }
    else:
        return {"challengeType": type, "index": idx, "description": desc, "startDateUtc": DatetimeToString(start_dt), "endDateUtc": DatetimeToString(end_dt), "stage": stage }


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
    print("===== Daily Challenges =====")
    for idx in range(3):
        description = ImageToText(f'ss/daily{idx + 1}.png')
        print("", description)
        challenges.append( ApiChallenge("daily", idx + 1, description, daily_start_time, daily_end_time) )

    # Weekly Challenges
    # Only send updated Weekly challenges on Thursday (local time) when they refresh
    if ShouldUpdateWeeklies():
        weekly_start_time = daily_start_time
        
        # If this script is run on a day later than UTC Thursday, fix weekly start time
        # 3 = Thursday
        while weekly_start_time.weekday() != 3:
            weekly_start_time -= datetime.timedelta(hours=24)

        weekly_end_time = weekly_start_time + datetime.timedelta(days=7) - datetime.timedelta(microseconds=1)
        
        print("===== Weekly Challenges =====")
        for idx in range(6):
            description = ImageToText(f'ss/weekly{idx + 1}.png')
            print("", description)
            challenges.append( ApiChallenge("weekly", idx + 1, description, weekly_start_time, weekly_end_time) )

    json_obj = {}
    json_obj["challenges"] = challenges

    print("Finished converting challenge images to text.")
    return json_obj


def PostSeasonChallenges():
    
    stages = []

    # Stage 1
    stages.append( [
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
        'Get 300 kills with the AR'
        ] )

    # Stage 2
    stages.append( [
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
        'Win a match with the Highest Score and Most Kills'
        ] )

    # Stage 3
    stages.append( [
        'Win a Match on 10 Different Maps',
        'Play 20 matches of Team SWAT',
        'Get 30 Double Kills',
        'Get 200 Kills with the AR',
        'Get 50 Kills Through Portals',
        'Get 100 Assist Kills',
        'Get 100 kills with the Battle Rifle',
        'Reach Pro level 3',
        'Get 30 kills with the Railgun',
        'Get the Most Kills on your team in 10 Matches',
        'Win a match of 10 different game modes',
        'Get 1 Collateral Kill'
        ] )

    # Stage 4
    stages.append( [
        'Play 10 Matches in 1 Day',
        'Get 10 King Slayer kills',
        'Go through 30 enemy portals',
        'Get 30 kills with the Pistol',
        'Play 15 matches of Sniper Frenzy',
        'Get 5 Triple Kills',
        'Win 3 matches of FFA Brawl',
        'Get the Highest Score on your team in 10 Matches',
        'Get 5 Killection Agency medals',
        'Inflict 30k damage',
        'Get 20 kills with the BFB',
        'Finish the Battle Pass'
    ] )


    # Dates hard-coded for Beta Season 1
    season_start_time = datetime.datetime(2022, 6, 2, 8, 0, 0, 0, tzinfo=datetime.timezone.utc)
    season_end_time = datetime.datetime(2022, 9, 2, 8, 0, 0, 0, tzinfo=datetime.timezone.utc)
    challenges = []
    for stage_idx, stage in enumerate(stages):
        for idx in range(len(stage)):
            challenge_idx = len(challenges)
            stage_number = stage_idx + 1
            challenges.append( ApiChallenge("seasonal", challenge_idx + 1, stage[idx], season_start_time, season_end_time, stage_number) )
    
    challenges_json = {}
    challenges_json["challenges"] = challenges
    PostToApi(challenges_json, preserve_challenge_completions = True)


def main():

    # Create a folder to store captured screenshots
    if not os.path.exists('ss'):
        os.makedirs('ss')
    
    start_time = time.time()
    RunSplitgate()
    DailyCheckIn()
    SaveDailies()
    SaveWeeklies()
    CloseSplitgate()
    print(f"Splitgate open for {time.time() - start_time:.1f} sec")
    challenges = CalculateChallengeData()
    PostToApi(challenges)


# Run main
if __name__ == '__main__':
    main()

    #ImageToText('ss/daily2.png', showDebug=True)

    # Manually run this once every 3 weeks when new Seasonals show up
    #PostSeasonChallenges()
