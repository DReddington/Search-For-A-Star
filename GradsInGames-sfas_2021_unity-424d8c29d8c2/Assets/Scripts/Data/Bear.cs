using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bear : MonoBehaviour
{
   public  Vector2Int bearLocation;
    public Vector2Int previousLocation;
    public Bear(Vector2Int startingLocation)
    {
        bearLocation = startingLocation;
    }
    public void BearUpdate(GameManager gameManager)
    {
        previousLocation = bearLocation;
        //Move the bear towards the player if the player is less then 5 away
        //Otherwise chose a random direction and move that direction
        if (PlayerDistance(gameManager.characterPostion) > 5)
        {
            var direction = Random.Range(0, 3);
            if(direction ==0) //North
            {
                bearLocation.y -= 1;
                if (bearLocation.y < 0) //Boundrie Cheacking
                    bearLocation.y++;
            }
            else if (direction == 1)//East
            {
                bearLocation.x++;
                if (bearLocation.x == gameManager.worldSize)
                    bearLocation.x--;
            }
            else if (direction == 2)//South 
            {
                bearLocation.y += 1;
                if (bearLocation.y == gameManager.worldSize)
                    bearLocation.y--; ;
            }
            else if (direction == 3)//West
            {
                bearLocation.x--;
                if (bearLocation.x < 0)
                    bearLocation.x++; ;
            }
        }
        else // The bear is within 5 units of the player so the bear moves towards the player 
        {
            var diffecnce = gameManager.characterPostion - bearLocation;

            if(System.Math.Abs(diffecnce.x) > System.Math.Abs(diffecnce.y)) // Closer on the x axis then the y. If equal then will move on y
            {
                if(diffecnce.x < 0) // negitve X 
                {
                    bearLocation.x--;
                    if (bearLocation.x < 0)
                        bearLocation.x++; ;
                }
                else//Postive X
                {
                    bearLocation.x++;
                    if (bearLocation.x == gameManager.worldSize)
                        bearLocation.x--;
                }
            }
            else
            {
                if (diffecnce.y < 0) // negitve y
                {
                    bearLocation.y -= 1;
                    if (bearLocation.y < 0) //Boundrie Cheacking
                        bearLocation.y++;
                }
                else//Postive y
                {
                    bearLocation.y += 1;
                    if (bearLocation.y == gameManager.worldSize)
                        bearLocation.y--; ;
                }
            }
        }
        //If bear postion is the same as the player postion game over 
    }

    private float PlayerDistance(Vector2Int playerLocation)
    {
        return Vector2Int.Distance(bearLocation, playerLocation);

    }


}
