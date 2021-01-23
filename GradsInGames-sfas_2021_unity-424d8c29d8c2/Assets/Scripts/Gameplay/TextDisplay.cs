using System.Collections;
using UnityEngine;
using TMPro;

public class TextDisplay : MonoBehaviour
{
    public enum State { Initialising, Idle, Busy }
    [SerializeField]public GameManager gameManager;
    private TMP_Text _displayText;
    private string _displayString;
    private WaitForSeconds _shortWait;
    private WaitForSeconds _longWait;
    private State _state = State.Initialising;

    public bool IsIdle { get { return _state == State.Idle; } }
    public bool IsBusy { get { return _state != State.Idle; } }

    private void Awake()
    {
        _displayText = GetComponent<TMP_Text>();
        _shortWait = new WaitForSeconds(0.01f);
        _longWait = new WaitForSeconds(0.08f);

        _displayText.text = string.Empty;
        _state = State.Idle;
    }

    private void SetMapColours()
    {
    // Force the text object to update right away so we can have geometry to modify right from the start.
        _displayText.ForceMeshUpdate();

        TMP_TextInfo textInfo = _displayText.textInfo;
        int currentCharacter = 0;

        Color32[] newVertexColors;
        Color32 c0 = _displayText.color;

        while (currentCharacter < _displayText.text.Length)
        {
            int characterCount = textInfo.characterCount;

            // Get the index of the material used by the current character.
            int materialIndex = textInfo.characterInfo[currentCharacter].materialReferenceIndex;

            // Get the vertex colors of the mesh used by this text element (character or sprite).
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            // Get the index of the first vertex used by this text element.
            int vertexIndex = textInfo.characterInfo[currentCharacter].vertexIndex;

            // Only change the vertex color if the text element is visible.
            if (textInfo.characterInfo[currentCharacter].isVisible)
            {

                if (_displayText.text[currentCharacter] == 'M')
                {
                    c0 = new Color32(220, 220, 220, 255);

                }
                else if (_displayText.text[currentCharacter] == 'F')
                {
                    c0 = new Color32(0, 255, 0, 255);
                }
                else if (_displayText.text[currentCharacter] == '0')
                {
                    c0 = new Color32(255, 215, 0, 255);
                }
                else if (_displayText.text[currentCharacter] == 'T')
                {
                    c0 = new Color32(139, 69, 19, 255);
                }
                else
                {
                    c0 = new Color32(0, 255, 0, 255);
                }



                newVertexColors[vertexIndex + 0] = c0;
                newVertexColors[vertexIndex + 1] = c0;
                newVertexColors[vertexIndex + 2] = c0;
                newVertexColors[vertexIndex + 3] = c0;

                // New function which pushes (all) updated vertex data to the appropriate meshes when using either the Mesh Renderer or CanvasRenderer.
                _displayText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

                // This last process could be done to only update the vertex data that has changed as opposed to all of the vertex data but it would require extra steps and knowing what type of renderer is used.
                // These extra steps would be a performance optimization but it is unlikely that such optimization will be necessary.
            }
            currentCharacter++;

            //textInfo.characterCount
        }
    }


    private IEnumerator DoShowText(string text)
    {
        int currentLetter = 0;
        char[] charArray = text.ToCharArray();

        if (charArray[0] == '$')
        {
            char[] temp = new char[charArray.Length - 1];

            for (int i = 0; i < charArray.Length -1;i ++)
            {
                temp[i] = charArray[i + 1];
            }
            charArray = temp;

            int mapindex = 0;
            //Display Map
            for (int j = 0; j < gameManager._worldMap.GetLength(1); j++)
            {
                while (mapindex < gameManager._worldMap.GetLength(0))
                {
                    if (gameManager.characterPostion == new Vector2Int(mapindex, j))
                    {
                        _displayText.text += '0';
                    }
                    else
                    {
                        _displayText.text += gameManager._worldMap[mapindex, j];
                    }
                    SetMapColours();

                    mapindex++;
                    yield return 0.005f;
                }
                mapindex = 0;
                _displayText.text += "\n";
            }
            var tempString = "Your current terrain is a ";
            if(gameManager._worldMap[gameManager.characterPostion.x, gameManager.characterPostion.y] == 'T')
            {
                tempString += "town";

            } 
            else if(gameManager._worldMap[gameManager.characterPostion.x, gameManager.characterPostion.y] == 'M')
            {
                tempString += "mountain";

            }
            else if (gameManager._worldMap[gameManager.characterPostion.x, gameManager.characterPostion.y] == 'F')
            {
                tempString += "field";

            }

            while (currentLetter < tempString.Length)
            {
                _displayText.text += tempString[currentLetter++];
                SetMapColours();
                yield return _shortWait;
            }
            _displayText.text += "\n";

        }
        currentLetter = 0;
        while (currentLetter < charArray.Length)
        {
            _displayText.text += charArray[currentLetter++];
            SetMapColours();
            yield return _shortWait;
        }

        _displayText.text += "\n";
        _displayString = _displayText.text;
        _state = State.Idle;
    }

    private IEnumerator DoAwaitingMapInput()
    {
        bool on = true;

        while (enabled)
        {
            //_displayText.text = string.Format("{0}> {1}", _displayString, (on ? "|" : " "));
            on = !on;
            SetMapColours();
            yield return _longWait;
        }
    }
    private IEnumerator DoAwaitingInput()
    {
        bool on = true;

        while (enabled)
        {
            _displayText.text = string.Format( "{0}> {1}", _displayString, ( on ? "|" : " " ));
            on = !on;
            SetMapColours();
            yield return _longWait;
        }
    }

    private IEnumerator DoClearText()
    {
        int currentLetter = 0;
        char[] charArray = _displayText.text.ToCharArray();

        while (currentLetter < charArray.Length)
        {
            if (currentLetter > 0 && charArray[currentLetter - 1] != '\n')
            {
                charArray[currentLetter - 1] = ' ';
            }

            if (charArray[currentLetter] != '\n')
            {
                charArray[currentLetter] = '_';
            }

            _displayText.text = charArray.ArrayToString();
            ++currentLetter;
            yield return null;
        }

        _displayString = string.Empty;
        _displayText.text = _displayString;
        _state = State.Idle;
    }

    private IEnumerator DoUpdateMap()
    {
        //int currentLetter = 0;
        char[] charArray = _displayText.text.ToCharArray();
        //Update the postion of where the charater was to display previous terrain
        //Update the character Postion
        //Update the current tile the player is on 

        charArray[gameManager.previousCharacterPostion.x + gameManager.previousCharacterPostion.y + (gameManager.previousCharacterPostion.y * 10)] =
            gameManager._worldMap[gameManager.previousCharacterPostion.x, gameManager.previousCharacterPostion.y];
        _displayText.text = charArray.ArrayToString();

        SetMapColours();

        charArray[gameManager.characterPostion.x + gameManager.characterPostion.y + (gameManager.characterPostion.y * 10)] =
            '0';
        _displayText.text = charArray.ArrayToString();

        _displayString = _displayText.text;
        SetMapColours();
        _state = State.Idle;
        yield return null;
    }


    public void Display(string text)
    {
        if (_state == State.Idle)
        {
            StopAllCoroutines();
            _state = State.Busy;
            StartCoroutine(DoShowText(text));
        }
    }

    public void ShowWaitingForMapInput()
    {
        if (_state == State.Idle)
        {
            StopAllCoroutines();
            StartCoroutine(DoAwaitingMapInput());
        }
    }

    public void ShowWaitingForInput()
    {
        if (_state == State.Idle)
        {
            StopAllCoroutines();
            StartCoroutine(DoAwaitingInput());
        }
    }

    public void UpdateMap()
    {
        if (_state == State.Idle)
        {
            StopAllCoroutines();
            _state = State.Busy;
            StartCoroutine(DoUpdateMap());
        }
    }
    public void Clear()
    {
        if (_state == State.Idle)
        {
            StopAllCoroutines();
            _state = State.Busy;
            StartCoroutine(DoClearText());
        }
    }
}
