//Created by Orng.Car,
//pro.rptr@gmail.com,
//description : in-game console system


using UnityEngine;
using System.Collections;
using System;

public class guiConsole : MonoBehaviour
{

    public GUISkin consoleSkin; //Our skin
    public GUIStyle consoleStyle; //Our style
    private bool enableConsole = false; //If false, console is unaviable
    private bool enableHelp = false; //If false, help is unaviable
    private Rect consoleWindow; //Main console window
    private Rect helpWindow; //Help console window

    private ArrayList messages = new ArrayList(); //Array where we store our messages
    private ArrayList commands = new ArrayList(); //Array of system commands
    private ArrayList time = new ArrayList(); //Array of current date

    private string[] split; //We use this variable to cut strings
    private string messageToSend = ""; //Textfield variable
    private string currentTime = ""; //Data variable
    public int maxMessages = 4; //How much messages can save history?
    private Vector2 scrollPosition; //Scrolling
    private Vector2 helpScrollPosition; //Scrolling

    private int scrollingCounter = 0; //Helpful counter, for scrolling system

    private AudioSource consoleSound = null; //Console audio-source to play audio files
    public AudioClip[] audioList = new AudioClip[3]; //Console audio files

    public bool enableSounds = true; //If true, then we can play sounds

    //At start we must add system console commands
    private void Awake()
    {
        if (enableSounds)
        {
            consoleSound = gameObject.AddComponent<AudioSource>();
            consoleSound.loop = false;
            consoleSound.playOnAwake = false;
            consoleSound.volume = 1;
            consoleSound.rolloffMode = AudioRolloffMode.Linear;
            //consoleSound.panLevel = 0;
        }
        commands.Add("setResolution");
        commands.Add("setFPS");
        commands.Add("exit");
        commands.Add("setAA");
        commands.Add("help");
        commands.Add("setLength");
    }


    //Draw our console
    private void OnGUI()
    {
        GUI.skin = consoleSkin;
        if (enableConsole)
        {
            consoleWindow = new Rect(0, 0, Screen.width, Screen.height / 3);
            consoleWindow = GUI.Window(0, consoleWindow, consoleControls, "Console [Beta version]");
        }
        if (enableHelp)
        {
            helpWindow = consoleWindow = new Rect(0, Screen.height / 3, Screen.width, Screen.height / 3);
            helpWindow = GUI.Window(1, helpWindow, helpControls, "Help system [Beta version]");
        }
    }

    //Main console window
    private void consoleControls(int windowID)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < messages.Count; i++)
        {
            GUILayout.Label("" + time[i] + messages[i], consoleStyle);
        }
        GUILayout.EndScrollView();
        messageToSend = GUILayout.TextField(messageToSend);
    }

    private void helpControls(int windowID)
    {
        GUILayout.Label("To close this window use ESC key");
        GUILayout.Label("Console commands :");
        helpScrollPosition = GUILayout.BeginScrollView(helpScrollPosition);
        GUILayout.Label("setResolution width height true/false - Allow to change screen resolution", consoleStyle);
        GUILayout.Label("setFPS number - Allow to change FPS counter", consoleStyle);
        GUILayout.Label("setAA number - Allow to change anisotropic filtering", consoleStyle);
        GUILayout.Label("setLength number - How much strings can console remind?", consoleStyle);
        GUILayout.Label("exit - Immediatly close application", consoleStyle);
        GUILayout.EndScrollView();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            enableHelp = false;
        }
    }

    //Enable/disable console system
    void onOffConsole()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote) && (!enableConsole))
        {
            enableConsole = true;
            messageToSend = ""; //When console is enable, clear previous "non-sendable" message
            if (enableSounds)
            {
                consoleSound.PlayOneShot(audioList[0]);
            }
        }

        else if (Input.GetKeyDown(KeyCode.BackQuote) && (enableConsole))
        {
            enableConsole = false;
            scrollingCounter = 0; //When we closing console, we bring back our default value of scrollingCounter
            enableHelp = false;
            if (enableSounds)
            {
                consoleSound.PlayOneShot(audioList[1]);
            }
        }
    }

    //Add our messages in our array
    private void sendMessages()
    {
        Input.eatKeyPressOnTextFieldFocus = false;
        if ((messageToSend.Length != 0) && (enableConsole) && (Input.GetKeyDown(KeyCode.Return)))
        {
            currentTime = System.DateTime.Now.ToString();
            messages.Add(messageToSend);
            time.Add(currentTime + ", Console message : ");
            consoleCommands();
            messageToSend = "";
        }
        if (messages.Count > maxMessages)
        {
            messages.RemoveAt(0);
            time.RemoveAt(0);
        }
    }

    //Here we adding our console commands
    private void consoleCommands()
    {
        split = messageToSend.Split(new Char[] { ' ', ',', '.', ':', '\t' });

        //FPS editor
        if ((split[0] == commands[1] as string) && (split.Length == 2))
        {
            setFrames(int.Parse(split[1]));
            confirmSound();
        }
        if ((split[0] == commands[0] as string) && (split.Length == 4))
        {
            setResolution(int.Parse(split[1]), int.Parse(split[2]), bool.Parse(split[3]));
            confirmSound();
        }
        //Close application
        if ((split[0] == commands[2] as string) && (split.Length == 1))
        {
            closeApplication();
            confirmSound();
        }
        //AntiAliasing settings
        if ((split[0] == commands[3] as string) && (split.Length == 2))
        {
            antiAliasing(int.Parse(split[1]));
        }
        //Enable/disable help window
        if ((split[0] == commands[4] as string) && (split.Length == 1))
        {
            enableHelp = true;
            confirmSound();
        }
        //Set maximum history
        if ((split[0] == commands[5] as string) && (split.Length == 2))
        {
            setLength(int.Parse(split[1]));
            confirmSound();
        }
        else
        {
            consoleSound.PlayOneShot(audioList[3]);
        }
    }

    //We can scroll now
    private void scrollingMessages()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (scrollingCounter == messages.Count)
            {
                scrollingCounter = 0;
            }
            scrollingCounter++;
            messageToSend = messages[messages.Count - scrollingCounter] as string;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (scrollingCounter <= 1)
            {
                scrollingCounter = messages.Count + 1;
            }
            scrollingCounter--;
            messageToSend = messages[messages.Count - scrollingCounter] as string;
        }
    }

    ///***///***Console commands***///***///

    //Procedure for change screen resolution
    private void setResolution(int h, int w, bool f)
    {
        Screen.SetResolution(h, w, f);
    }

    //Procedure for set application frame rate
    private void setFrames(int frames)
    {
        Application.targetFrameRate = frames;
    }

    //AA filtration
    private void antiAliasing(int i)
    {
        QualitySettings.antiAliasing = i;
    }
    
    //Procedure for close application
    private void closeApplication()
    {
        Application.Quit();
    }

    //Procedure for length console history
    private void setLength(int i)
    {
        maxMessages = i;
    }

    //Play sound if command correct
    private void confirmSound()
    {
        consoleSound.PlayOneShot(audioList[2]);
    }

    private void Update()
    {
        onOffConsole();
        sendMessages();
        scrollingMessages();
    }
}
