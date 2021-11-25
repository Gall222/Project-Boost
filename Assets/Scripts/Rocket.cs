using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField]float rotationSpeed = 70f;
    [SerializeField]float thrustSpeed = 70f;

    [SerializeField]AudioClip mainEngin;
    [SerializeField]AudioClip crashSFX;
    [SerializeField]AudioClip endLvlSFX;

    [SerializeField]ParticleSystem enginVFX;
    [SerializeField]ParticleSystem deathVFX;
    [SerializeField]ParticleSystem finishVFX;

    Rigidbody rigidbody;
    AudioSource audioSource;

    bool isCollisionAnabled = true;

    enum State { Alive, Dying, Transcending}
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput(); 
    }

    private void ProcessInput()
    {
        
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
        if (state != State.Alive) { return; }
        RespondToThrust();
        RespondToRotateInput();
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLvl();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            isCollisionAnabled = !isCollisionAnabled;
        }
    }

    private void RespondToThrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            enginVFX.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * thrustSpeed);
        if (!audioSource.isPlaying)
        {
            //audioSource.Play();
            audioSource.PlayOneShot(mainEngin);
            enginVFX.Play();
        }
    }

    private void RespondToRotateInput()
    {
        rigidbody.freezeRotation = true;    //take control of rotation
        if (Input.GetKey(KeyCode.A))
        {
            
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed * Time.deltaTime);
        }
        rigidbody.freezeRotation = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || !isCollisionAnabled) { return; }
        switch (collision.gameObject.tag)
        {
            case "Friendly":

                break;
            case "Fuel":
                break;
            case "Finish":
                FinishingLvl();
                break;
            default:
                PlayerDeathProcces();
                break;
        }
    }

    private void FinishingLvl()
    {
        state = State.Transcending;
        finishVFX.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(endLvlSFX);
        Invoke("LoadNextLvl", 1f);
    }

    private void LoadNextLvl()
    {
        var sceneIndex = SceneManager.GetActiveScene().buildIndex;
        var nexLvlIndex = sceneIndex + 1;
        if (nexLvlIndex == SceneManager.sceneCountInBuildSettings) {
            nexLvlIndex = 0;
        }
            SceneManager.LoadScene(nexLvlIndex);
    }

    private void PlayerDeathProcces()
    {
        state = State.Dying;
        deathVFX.Play();
        audioSource.Stop();
        audioSource.PlayOneShot(crashSFX);
        Invoke("PlayerDeath", 1f);
    }

    private void PlayerDeath()
    {
        //Destroy(gameObject);
        var sceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneIndex); 
    }
}
