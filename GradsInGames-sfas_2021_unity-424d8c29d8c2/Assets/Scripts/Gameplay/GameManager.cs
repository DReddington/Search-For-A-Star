using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private StoryData _data;
    [SerializeField] private int worldSize = 10;
    [SerializeField] private int numberOfTowns = 6;

    public char[,] _worldMap;
    public Vector2Int characterPostion = new Vector2Int(1,1);
    private TextDisplay _output;
    private BeatData _currentBeat;
    private WaitForSeconds _wait;

    private void Awake()
    {
        CreateWorld(worldSize);
        _output = GetComponentInChildren<TextDisplay>();
        _currentBeat = null;
        _wait = new WaitForSeconds(0.5f);
    }

    private void Update()
    {
        if(_output.IsIdle)
        {
            if (_currentBeat == null)
            {
                DisplayBeat(1);
            }
            else
            {
                UpdateInput();
            }
        }
    }

    private void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(_currentBeat != null)
            {
                if (_currentBeat.ID == 1)
                {
                    Application.Quit();
                }
                else
                {
                    DisplayBeat(1);
                }
            }
        }
        else
        {
            KeyCode alpha = KeyCode.Alpha1;
            KeyCode keypad = KeyCode.Keypad1;

            for (int count = 0; count < _currentBeat.Decision.Count; ++count)
            {
                if (alpha <= KeyCode.Alpha9 && keypad <= KeyCode.Keypad9)
                {
                    if (Input.GetKeyDown(alpha) || Input.GetKeyDown(keypad))
                    {
                        ChoiceData choice = _currentBeat.Decision[count];
                        if(_currentBeat.ID == 4)//Map Display CHoice 
                        {
                            if(choice.DisplayText == "North")
                            {
                                characterPostion.y -= 1;
                                if (characterPostion.y < 0) //Boundrie Cheacking
                                    characterPostion.y++;
                            }
                            if (choice.DisplayText == "East")
                            {
                                characterPostion.x ++;
                                if (characterPostion.x == worldSize)
                                    characterPostion.x--;
                            }
                            if (choice.DisplayText == "South")
                            {
                                characterPostion.y += 1;
                                if (characterPostion.y == worldSize)
                                    characterPostion.y--; ;
                            }
                            if (choice.DisplayText == "West")
                            {
                                characterPostion.x --;
                                if (characterPostion.x < 0)
                                    characterPostion.x++; ;
                            }
                            if(choice.DisplayText == "Interact")
                            {
                                //Enter Town Dialouge 
                                if(_worldMap[characterPostion.x, characterPostion.y] == 'T')
                                {
                                    choice.NextID =100;
                                }
                            }
                        }
                        DisplayBeat(choice.NextID);
                        break;
                    }
                }

                ++alpha;
                ++keypad;
            }
        }
    }

    private void DisplayBeat(int id)
    {
        BeatData data = _data.GetBeatById(id);
        StartCoroutine(DoDisplay(data));
        _currentBeat = data;
    }

    private IEnumerator DoDisplay(BeatData data)
    {
        _output.Clear();

        while (_output.IsBusy)
        {
            yield return null;
        }

        _output.Display(data.DisplayText);

        while(_output.IsBusy)
        {
            yield return null;
        }
        
        for (int count = 0; count < data.Decision.Count; ++count)
        {
            ChoiceData choice = data.Decision[count];
            _output.Display(string.Format("{0}: {1}", (count + 1), choice.DisplayText));

            while (_output.IsBusy)
            {
                yield return null;
            }
        }

        if(data.Decision.Count > 0)
        {
            _output.ShowWaitingForInput();
        }
    }
    //MAP CODE
    public void CreateWorld(int WorldSize)
    {
        _worldMap = new char[WorldSize, WorldSize];

        for (int i = 0; i < WorldSize; i++)
        {
            for (int j = 0; j < WorldSize; j++)
            {
                if (i == 0 || i == WorldSize - 1 || j == 0 || j == WorldSize - 1) // Makes border Moutains
                {
                    _worldMap[i, j] = 'M';
                }
                else
                {
                    var terrainChoice = Random.Range(0, 3);
                    _worldMap[i, j] = 'P';
                }
            }
        }
        //Set Towns
        var placedTowns = 0;

        while(placedTowns<numberOfTowns)
        {
            Vector2Int temp = new Vector2Int(Random.Range(0, worldSize), Random.Range(0, worldSize));
            if(_worldMap[temp.x,temp.y] != 'T')
            {
                _worldMap[temp.x, temp.y] = 'T';
            }
            placedTowns++;
        }
        



    }
}
