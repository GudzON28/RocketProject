using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class Rocket : MonoBehaviour
{
    private Rigidbody rigidBody;
    private AudioSource audioSource;
    [SerializeField] private float rotationSpeed = 120f;
    [SerializeField] private float launchSpeed = 1100f;
    private State state;
    [SerializeField] private AudioClip flySound;
    [SerializeField] private AudioClip deadSound;
    [SerializeField] private AudioClip finishSound;
    [SerializeField] private ParticleSystem flyParticle;
    [SerializeField] private ParticleSystem deadParticle;
    [SerializeField] private ParticleSystem finishParticle;
    private bool collisionOff = false;
    [SerializeField] private float fuelRocket = 100f;
    [SerializeField] private float fuelConsumption = 7f;
    [SerializeField] private Text fuelText;
    private float fuelTankVolume = 30;

    void Start()
    {
        state = State.Playing;
        rigidBody = GetComponent<Rigidbody>();
        audioSource = rigidBody.GetComponent<AudioSource>();
        fuelText.text = fuelRocket.ToString();
    }

    void Update()
    {
        if (state == State.Playing)
        {
            EmptyTank();
        }

        Rotation();

        if (Debug.isDebugBuild)
        {
            DebugKeys();
        }
    }

    private void EmptyTank()
    {
        if (fuelRocket > 0)
        {
            Launch();
            return;
        }

        fuelText.text = "0";
        audioSource.Pause();
        flyParticle.Stop();
    }

    private void DebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            NextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionOff = !collisionOff;
        }
    }

    private void Launch()
    {
        float adjustment = launchSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Space))
        {
            fuelRocket -= fuelConsumption * Time.deltaTime;
            fuelText.text = fuelRocket.ToString();

            rigidBody.AddRelativeForce(Vector3.up * adjustment);

            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(flySound);
                flyParticle.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                flyParticle.Stop();
            }
        }
    }

    private void Rotation()
    {
        float adjustment = rotationSpeed * Time.deltaTime;

        rigidBody.freezeRotation = true;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * adjustment);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.back * adjustment);
        }

        rigidBody.freezeRotation = false;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (state == State.Dead || state == State.NextLevel)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Battary":
                PlusFuel(fuelTankVolume, collision.gameObject);
                break;
            case "Finish":
                state = State.NextLevel;
                audioSource.Stop();
                audioSource.PlayOneShot(finishSound);
                flyParticle.Stop();
                finishParticle.Play();
                Invoke(nameof(NextLevel), 2f);
                break;
            default:
                if (!collisionOff)
                {
                    state = State.Dead;
                    audioSource.Stop();
                    audioSource.PlayOneShot(deadSound);
                    flyParticle.Stop();
                    deadParticle.Play();
                    Invoke(nameof(FirstLevel), 2f);
                }
                break;
        }
    }

    private void PlusFuel(float fuetTank, GameObject battery)
    {
        battery.GetComponent<BoxCollider>().enabled = false;
        fuelRocket += fuetTank;
        fuelText.text = fuelRocket.ToString();
        Destroy(battery);
    }

    private void NextLevel()
    {
        var currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        var nextLevelIndex = currentLevelIndex + 1;

        if (nextLevelIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextLevelIndex = 1;
        }

        SceneManager.LoadScene(nextLevelIndex);
    }

    private void FirstLevel()
    {
        SceneManager.LoadScene(1);
    }
}
