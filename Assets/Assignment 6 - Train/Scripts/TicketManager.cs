using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TicketManager : MonoBehaviour
{
    public RaycastLogic raycastLogic;

    //Fake Flags
    public bool isFake;
    public bool isDate;
    public bool isDay;
    public bool isMonth;
    public bool isYear;
    public bool isTime;
    public bool isSeat;
    public bool isSeatLetter;
    public bool isSeatNumber;
    public bool isZone;
    public bool isFloor;
    public bool isLocation;
    public bool isFromLocation;
    public bool isToLocation;
    public bool isPencil;
    public bool isLogo;

    //Fake Variables
    public int fakeYear;
    public int fakeMonth;
    public int fakeDay;
    public int fakeDepartureHour;
    public int fakeArriveHour;
    public char fakeSeatLetter;
    public int fakeSeatNumber;
    public char fakeZoneLetter;
    public int fakeFloor;
    public string fakeFromLocation;
    public string fakeToLocation;

    public string[] locationNames = {"Tokyo", "Seoul", "New York", "Bangkok", "Beijing", "Madrid", "Rome"};

    public int calculatedTime;

    private GameObject currentPassenger;

    public TrueValue trueValue;
    private PassengerInfo passengerInfo;

    //Ticket Text
    public TextMeshProUGUI fromLocationText;
    public TextMeshProUGUI toLocationText;
    public TextMeshProUGUI yearText;
    public TextMeshProUGUI monthText; 
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI departureTimeText;
    public TextMeshProUGUI arriveTimeText;
    public TextMeshProUGUI seatNumberText;
    public TextMeshProUGUI seatLetterText;
    public TextMeshProUGUI zoneText;
    public TextMeshProUGUI floorText;

    public RawImage logoImage;

    private Renderer passengerRenderer;

    public bool isMistakenCaught = false;
    public bool isCaught = false;
    public bool isFine = false;
    public bool isMistakeFine = false;

    void Start()
    {
        passengerInfo = GetComponent<PassengerInfo>();

        currentPassenger = gameObject;

        passengerRenderer = currentPassenger.GetComponent<Renderer>();

        int randomFake = Random.Range(0, 2);

        if (randomFake == 0)
        {
            isFake = true;
        }
        else
        {
            isFake = false;
        }

        if (isFake == true)
        {
            int randomAnomaly = Random.Range(0, 7);

            if (randomAnomaly == 0)
            {
                isDate = true;

                int randomDate = Random.Range(0, 3);

                if (randomDate == 0)
                {
                    isDay = true;
                }
                else if (randomDate == 1)
                {
                    isMonth = true;
                }
                else
                {
                    isYear = true;
                }

                RandomDateGeneration();
            }
            else if (randomAnomaly == 1)
            {
                isTime = true;

                fakeDepartureHour = RandomTimeRandomizerLimit(trueValue.hour, 2, 20, 3);

                fakeArriveHour = fakeDepartureHour + 3;
            }
            else if (randomAnomaly == 2)
            {
                isSeat = true;

                int randomSeatValue = Random.Range(0, 2);

                if (randomSeatValue == 0)
                {
                    isSeatLetter = true;
                    fakeSeatLetter = RandomLetterGenerator('D', passengerInfo.trueSeatLetter);
                }
                else
                {
                   isSeatNumber = true;
                   fakeSeatNumber = RandomNumberGenerator(10,  passengerInfo.trueSeatNumber);
                }
            }
            else if (randomAnomaly == 3)
            {
                isZone = true;

                fakeZoneLetter = RandomLetterGenerator('C', passengerInfo.trueZoneLetter);
            }
            else if (randomAnomaly == 4) 
            {
                isFloor = true;

                fakeFloor = RandomNumberGenerator(2, passengerInfo.trueFloor);
            }
            else if (randomAnomaly == 5)
            {
                isLocation = true; 

                int randomLocation = Random.Range(0, 2);

                if (randomLocation == 0)
                {
                    isFromLocation = true;

                    fakeFromLocation = GetRandomLocation();
                }
                else
                {
                    isToLocation = true;

                    fakeToLocation = GetRandomLocation();
                }
            }
            else
            {
                isLogo = true;
            }
        }
    }

    public void ShowTicketUI(GameObject ticketObject)
    {
        ticketObject.SetActive(true);

        yearText.text = trueValue.year.ToString();
        monthText.text = trueValue.month.ToString("D2");
        dayText.text = trueValue.day.ToString("D2");
        departureTimeText.text = trueValue.hour.ToString("D2");
        arriveTimeText.text = trueValue.arrivalHour.ToString("D2");
        seatNumberText.text = passengerInfo.trueSeatNumber.ToString("D2");
        seatLetterText.text = passengerInfo.trueSeatLetter.ToString();
        zoneText.text = passengerInfo.trueZoneLetter.ToString();
        floorText.text = passengerInfo.trueFloor.ToString();
        fromLocationText.text = trueValue.trueFromLocation;
        toLocationText.text = trueValue.trueToLocation;

        logoImage.gameObject.SetActive(true);

        if (isFake == true)
        {
            if (isDate == true)
            {
                if (isYear == true)
                {
                    yearText.text = fakeYear.ToString();
                }
                else if (isMonth == true)
                {
                    monthText.text = fakeMonth.ToString("D2");
                }
                else if (isDay == true)
                {
                    dayText.text = fakeDay.ToString("D2");
                }
            }
            else if (isTime == true)
            {
                departureTimeText.text = fakeDepartureHour.ToString("D2");
                arriveTimeText.text = fakeArriveHour.ToString("D2");
            }
            else if (isLocation == true)
            {
                if (isFromLocation == true)
                {
                    fromLocationText.text = fakeFromLocation;
                }
                else if (isToLocation == true)
                {
                    toLocationText.text = fakeToLocation;
                }
            }
            else if (isSeat)
            {
                if (isSeatNumber == true)
                {
                    seatNumberText.text = fakeSeatNumber.ToString("D2");
                }
                else if (isSeatLetter == true)
                {
                    seatLetterText.text = fakeSeatLetter.ToString();
                }
            }
            else if (isZone == true)
            {
                zoneText.text = fakeZoneLetter.ToString();
            }
            else if (isFloor == true)
            {
                floorText.text = fakeFloor.ToString();
            }
            else if (isLogo == true)
            {
                logoImage.gameObject.SetActive(false);
            }
        }
    }

    public void CheckPassengerStatus()
    {
        gameObject.GetComponent<Collider>().enabled = false;

        if (raycastLogic.hasCheckedFalse)
        {
            passengerRenderer.material.color = Color.red;

            if (isFake)
            {
                isCaught = true;
            }
            else
            {
                isMistakenCaught = true;
            }
        }
        else if (raycastLogic.hasCheckedTrue)
        {
            passengerRenderer.material.color = Color.green;

            if (isFake)
            {
                isMistakeFine = true;
            }
            else
            {
                isFine = true;
            }
        }
    }


    void RandomDateGeneration()
    {
        if (isYear)
        {
            fakeYear = RandomTimeRandomizerUnlimit(trueValue.year, 25);
        }
        else if (isMonth)
        {
            fakeMonth = RandomTimeRandomizerLimit(trueValue.month, 1, 12, 0);
        }
        else if (isDay)
        {
            fakeDay = RandomTimeRandomizerLimit(trueValue.day, 1, 28, 0);
        }
    }

    public int RandomTimeRandomizerUnlimit(int trueTime, int difference)
    {
        int randomTense = Random.Range(0, 2);

        if (randomTense == 0)
        {
            int timeDifference = trueTime - difference;
            int fakePast = Random.Range(timeDifference, trueTime);

            return fakePast;
        }
        else
        {
            int timeDifference = trueTime + difference;
            int fakeFuture = Random.Range(trueTime + 1, timeDifference);

            return fakeFuture;
        }
    }

    public int RandomTimeRandomizerLimit(int trueTime, int start,  int end, int startDifference)
    {
        int randomTense = Random.Range(0, 2);

        if (randomTense == 0)
        {
            int fakePast = Random.Range(start, trueTime - startDifference);
            return fakePast;
        }
        else
        {
            int fakeFuture = Random.Range(trueTime + 1, end + 1);
            return fakeFuture;
        }
    }

    public char RandomLetterGenerator(char endLetter, char trueLetter)
    {
        char randomLetter;

        do
        {
            randomLetter = (char)Random.Range('A', endLetter + 1);
            //Code from https://discussions.unity.com/t/random-char-a-to-z/118119/3

        } while (randomLetter == trueLetter);

        return randomLetter;
    }
    public int RandomNumberGenerator(int endNumber, int trueNumber)
    {
        int randomNumber;

        do
        {
            randomNumber = Random.Range(1, endNumber + 1);

        } while (randomNumber == trueNumber);

        return randomNumber;
    }

    public string GetRandomLocation()
    {
        int randomIndex = Random.Range(0, locationNames.Length);
        string randomLocation = locationNames[randomIndex];

        return randomLocation;
    }
}
