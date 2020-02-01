using UnityEngine;


[System.Serializable]
public class AnimalAI
{
    public FlipCharacter flipCharacter;
    public Transform myAnimalTransform;
    public Transform playerTransform;
    public float targetPosition;
    public float startPosition;
    public Animator animalAnimator;
    public Vector3 velocityDirection;
    public float currentDistance; // current distance of player,animal Vector2.Distance
    public float rangeFallowPlayer; // at wich range can fallow
    public float rangeAtack; // at wich range can attack



    public void IdleFallowAtack()
    {
        flipCharacter.FlipVerification(velocityDirection.x);
        if (currentDistance <= rangeAtack) // A in B area
        {
            Debug.DrawRay(myAnimalTransform.position, Vector2.right * velocityDirection.x * rangeAtack, Color.blue);
            velocityDirection.x = 0; // don't move

            animalAnimator.SetBool("Atack", true); //Atack player
            animalAnimator.SetBool("Walk", false);
        }
        else if (currentDistance <= rangeFallowPlayer) // A in B area
        {
            Debug.DrawRay(myAnimalTransform.position, Vector2.right * velocityDirection.x * rangeFallowPlayer, Color.red);
            FallowPlayerDirection(myAnimalTransform.position.x, playerTransform.position.x); //Fallow Player

            animalAnimator.SetBool("Walk", true); // Walk
            animalAnimator.SetBool("Idle", false);
        }
        else // A out of B area
        {
            velocityDirection.x = 0; // don't move

            animalAnimator.SetBool("Idle", true); // Stay Iddle
            animalAnimator.SetBool("Walk", false);
        }

    }

    public void WalkFallowAtak()
    {
        flipCharacter.FlipVerification(velocityDirection.x);

        if (currentDistance <= rangeAtack)
        {
            Debug.DrawRay(myAnimalTransform.position, Vector2.right * velocityDirection.x * rangeAtack, Color.blue);

            animalAnimator.SetBool("Atack", true); //Atack player
            animalAnimator.SetBool("Walk", false);
        }
        else if (currentDistance < rangeFallowPlayer)//
        {
            Debug.DrawRay(myAnimalTransform.position, Vector2.right * velocityDirection.x * rangeFallowPlayer, Color.yellow);

            FallowPlayerDirection(myAnimalTransform.position.x, playerTransform.position.x); //Fallow Player
            animalAnimator.SetBool("Walk", true);
            animalAnimator.SetBool("Atack", false);
        }
        else
        {
            Debug.DrawRay(myAnimalTransform.position, Vector2.right * velocityDirection.x * rangeFallowPlayer, Color.blue);

            WalkDirection();
            animalAnimator.SetBool("Walk", true); // Walk
            animalAnimator.SetBool("Atack", false);
        }

    }

    void FallowPlayerDirection(float myAnimalPosition, float playerPosition) // used in IdleFallowAtack() and PatrolFallowAtak()
    {
        velocityDirection.x = (myAnimalPosition >= playerPosition) ? -1 : 1; //Player-Left & Animal-Right  else  //Animal-Left & Player-Right
                                                                             //Debug.Log("FallowPlayerDirection");
    }

    void WalkDirection() //used in PatrolFallowAtak()
    {
        if (myAnimalTransform.position.x >= targetPosition && velocityDirection.x != -1)
        {
            velocityDirection.x = -1; //Target-Left & Animal-Right
        }
        else if (myAnimalTransform.position.x <= startPosition && velocityDirection.x != 1)//
        {
            velocityDirection.x = 1; //Animal - Left & Player - Right
        }
    }

}



[System.Serializable]
public class LevelDetails
{
    public Texture LevelPicture;
    public string sceneName;
    public int unlockedLevels;   //which is locked
}

[System.Serializable]
public class AudioInfo
{
    public int ID;
    public AudioClip audioClip;

    public void LoopPlayAudio(AudioSource audioSource)
    {
        audioSource.volume = PlayerPrefs.GetFloat("Volume"); //set volume
        audioSource.loop = true;
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    public void OncePlayAudio(AudioSource audioSource)
    {
        audioSource.volume = PlayerPrefs.GetFloat("Volume"); //set volume
        audioSource.PlayOneShot(audioClip);
    }

}

[System.Serializable]
public class FlipCharacter
{
    public Transform myCharacterTransform;
    bool sampleFlip;

    public void FlipVerification(float velocity)
    {
        if (velocity < 0 && !sampleFlip)
        {
            Flip();
            sampleFlip = true;
        }
        else if (velocity > 0 && sampleFlip)
        {
            Flip();
            sampleFlip = false;
        }
    }
    public void Flip()
    {
        Vector3 theScale = myCharacterTransform.localScale;
        theScale.x = -theScale.x;
        myCharacterTransform.localScale = theScale;
    }
}

[System.Serializable]
public class MoveObjectInteraction
{
    public float X;                                                                    //x axe target
    public float Y;                                                                               //y axe target
    Vector3 targetArrive;
    Vector3 targetStart;
    public int speed;

    public void Init(Transform obj)                                                        //constructor
    {
        targetArrive = new Vector3(obj.position.x + X, obj.position.y + Y);
        targetStart = obj.position;
    }

    public Vector3 MoveAtoB(Transform obj)
    {
        return Vector3.MoveTowards(obj.position, targetArrive, speed * Time.deltaTime);
    }
    public Vector3 MoveBtoA(Transform obj)
    {
        return Vector3.MoveTowards(obj.position, targetStart, speed * Time.deltaTime);
    }

    public Vector3 PushButton(Transform obj, bool btnPressed)
    {
        if (btnPressed)
        {
            return MoveAtoB(obj);
        }
        else
        {
            return MoveBtoA(obj);
        }
    }

}