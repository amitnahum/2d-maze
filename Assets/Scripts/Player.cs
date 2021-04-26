// movement related code was taken and modified from Unity official tutorial
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;        //Allows us to use SceneManager
[RequireComponent(typeof(AudioSource))]
//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject
{
    public int collected = 0;                    //How many coins collected.

    public int moved = 0;
    
    public AudioClip collectedSound;     //This is the sound that will play after you collect one
    
    public AudioClip gameOverSound;     //This is the sound that will play after you get game over

    //Start overrides the Start function of MovingObject
    protected override void Start ()
    {
        //Call the Start function of the MovingObject base class.
        base.Start ();
    }
    
    void OnGUI(){
        GUI.Label(new Rect(10, 10, 100, 20), "Collected: " + collected);
        GUI.Label(new Rect(10,40, 100, 20), "Moved: " + moved);
    }


    private void Update ()
    {

        int horizontal = 0;      //Used to store the horizontal move direction.
        int vertical = 0;        //Used to store the vertical move direction.


        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
        horizontal = (int) (Input.GetAxisRaw ("Horizontal"));

        //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
        vertical = (int) (Input.GetAxisRaw ("Vertical"));

        //Check if moving horizontally, if so set vertical to zero.
        if(horizontal != 0)
        {
            vertical = 0;
        }

        //Check if we have a non-zero value for horizontal or vertical
        if(horizontal != 0 || vertical != 0)
        {
            AttemptMove<CellScript> (horizontal, vertical);
        }
    }

    //AttemptMove overrides the AttemptMove function in the base class MovingObject
    //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
    protected override void AttemptMove <T> (int xDir, int yDir)
    {
        //Hit allows us to reference the result of the Linecast done in Move.
        RaycastHit2D hit;

        //If Move returns true, meaning Player was able to move into an empty space.
        bool hasMoved = Move(xDir, yDir, out hit);
        if (hasMoved)
        {
            // if we did move then increment moved 
            moved++;
        }

    }
    
    
    //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
    private void OnTriggerEnter2D (Collider2D other)
    {
        //Check if the tag of the trigger collided with is collectable.
        if (other.CompareTag("collectable"))
        {
            GetComponent<AudioSource>().PlayOneShot(collectedSound); //plays the sound assigned to collectedSound
            collected++;
            Destroy(other.gameObject);
        }

        if (other.CompareTag("LavaTile") && other.GetComponent<LavaTile>().IsActive())
        {
            GetComponent<AudioSource>().PlayOneShot(gameOverSound);
            GameOver();
        }
    }

    //GameOver is invoked, you lost the game 
    private void GameOver()
    {
        Debug.Log("GameOver");
    }


    //Restart reloads the scene when called.
    private void Restart ()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game.
        SceneManager.LoadScene (0);
    }
    
    
}
