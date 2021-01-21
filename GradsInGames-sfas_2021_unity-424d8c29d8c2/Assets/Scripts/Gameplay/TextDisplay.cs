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




    private IEnumerator SetMapColours()
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

            // If No Characters then just yield and wait for some text to be added
            if (characterCount == 0)
            {
                yield return new WaitForSeconds(0.25f);
                continue;
            }

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
                    c0 = new Color32(255, 0, 0, 255);

                }
                else
                {
                    c0 = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);

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
        }
        yield break;
    
    }

    private IEnumerator DoShowText(string text)
    {
        int currentLetter = 0;
        char[] charArray = text.ToCharArray();

        if (charArray[0] == '$')
        {
            int mapindex = 0;
            //Display Map
            for (int j = 0; j < gameManager._worldMap.GetLength(1); j++)
            {
                while (mapindex < gameManager._worldMap.GetLength(0))
                {
                    _displayText.text += gameManager._worldMap[mapindex, j];
                    mapindex++;
                    yield return _shortWait;
                }
                SetMapColours();

                mapindex = 0;
                _displayText.text += "\n";
            }

        }



        while (currentLetter < charArray.Length)
        {
            _displayText.text += charArray[currentLetter++];
            yield return _shortWait;
        }

        _displayText.text += "\n";
        _displayString = _displayText.text;
        _state = State.Idle;
    }

    private IEnumerator DoAwaitingInput()
    {
        bool on = true;

        while (enabled)
        {
            _displayText.text = string.Format( "{0}> {1}", _displayString, ( on ? "|" : " " ));
            on = !on;
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

    public void Display(string text)
    {
        if (_state == State.Idle)
        {
            StopAllCoroutines();
            _state = State.Busy;
            StartCoroutine(DoShowText(text));
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
