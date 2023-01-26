using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    private Transform ball;
    private Vector3 startMousePos, startBallPos;
    private bool moveTheBall;
    [Range(0f, 1f)]public float maxSpeed;
    [Range(0f, 1f)] public float camSpeed;
    [Range(0f, 50f)] public float pathSpeed;
    private float velocity, camVelocity_x, camVelocity_y;
    private Camera mainCam;
    public Transform path;
    private Rigidbody rb;
    private Collider _collider;
    private Renderer BallRenderer;
    public ParticleSystem CollideParticle;
    public ParticleSystem airEffect;
    public ParticleSystem Dust;

    private int y0, z0;
    [SerializeField] private GameObject[] objectToBeSpawned = new GameObject[3];
    private int objectIndex;
    [SerializeField] private Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        ball = transform;
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        BallRenderer = ball.GetChild(0).GetComponent<Renderer>();

        y0 = -3;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && MenuManager.MenuManagerInstance.GameState)
        {
            moveTheBall = true;

            Plane newPlan = new Plane(Vector3.up, 0f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (newPlan.Raycast(ray, out var distance))
            {
                startMousePos = ray.GetPoint(distance);
                startBallPos = ball.position;
            }
        }

        else if (Input.GetMouseButtonUp(0))
        {
            moveTheBall = false;
        }

        if (moveTheBall)
        {
            Plane newPlan = new Plane(Vector3.up, 0f);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (newPlan.Raycast(ray, out var distance))
            {
                Vector3 mouseNewPos = ray.GetPoint(distance);
                Vector3 MouseNewPos = mouseNewPos - startMousePos;
                Vector3 DesireBallPos = MouseNewPos + startBallPos;

                DesireBallPos.x = Mathf.Clamp(DesireBallPos.x, -1.5f, 1.5f);

                ball.position = new Vector3(Mathf.SmoothDamp(ball.position.x, DesireBallPos.x, ref velocity, maxSpeed), ball.position.y, ball.position.z);
            }
        }

        if (MenuManager.MenuManagerInstance.GameState)
        {
            var pathNewPos = path.position;

            path.position = new Vector3(pathNewPos.x, pathNewPos.y, Mathf.MoveTowards(pathNewPos.z, -100000f, pathSpeed * Time.deltaTime));

            ball.GetChild(0).Rotate(Vector3.right * 1000f * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        var CameraNewPos = mainCam.transform.position;

        if (rb.isKinematic)
            mainCam.transform.position = new Vector3(Mathf.SmoothDamp(CameraNewPos.x, ball.transform.position.x, ref camVelocity_x, camSpeed),
                Mathf.SmoothDamp(CameraNewPos.y, ball.transform.position.y + 3f, ref camVelocity_y, camSpeed), CameraNewPos.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("obstacle"))
        {
            gameObject.SetActive(false);
            MenuManager.MenuManagerInstance.GameState = false;
            MenuManager.MenuManagerInstance.menuElement[2].SetActive(true);
            MenuManager.MenuManagerInstance.menuElement[2].transform.GetChild(0).GetComponent<Text>().text = "You Lose";
            MenuManager.MenuManagerInstance.menuElement[3].GetComponent<Text>().text = PlayerPrefs.GetInt("score").ToString();
            airEffect.Stop();
            Dust.Stop();
        }

        if(other.gameObject.name.Contains("Colorball"))
        {
            PlayerPrefs.SetInt("score", PlayerPrefs.GetInt("score") + 1);
            MenuManager.MenuManagerInstance.menuElement[1].GetComponent<Text>().text = PlayerPrefs.GetInt("score").ToString();
        }    
        
        switch (other.tag)
        {
            case "red":
                other.gameObject.SetActive(false);
                BallRenderer.material = other.GetComponent<Renderer>().material;
                var NewParticle = Instantiate(CollideParticle, transform.position, Quaternion.identity);
                NewParticle.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                break;

            case "green":
                other.gameObject.SetActive(false);
                BallRenderer.material = other.GetComponent<Renderer>().material;
                var NewParticle1 = Instantiate(CollideParticle, transform.position, Quaternion.identity);
                NewParticle1.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                break;

            case "blue":
                other.gameObject.SetActive(false);
                BallRenderer.material = other.GetComponent<Renderer>().material;
                var NewParticle2 = Instantiate(CollideParticle, transform.position, Quaternion.identity);
                NewParticle2.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                break;

            case "yellow":
                other.gameObject.SetActive(false);
                BallRenderer.material = other.GetComponent<Renderer>().material;
                var NewParticle3 = Instantiate(CollideParticle, transform.position, Quaternion.identity);
                NewParticle3.GetComponent<Renderer>().material = other.GetComponent<Renderer>().material;
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("path"))
        {
            rb.isKinematic = _collider.isTrigger = false;
            rb.velocity = new Vector3(0f, 8f, 0f);
            pathSpeed *= 2;

            var airEffectMain = airEffect.main;
            airEffectMain.simulationSpeed *= 2;

            objectIndex = Random.Range(0, 2);
            Vector3 position = new Vector3(0, y0, 460);
            Instantiate(objectToBeSpawned[objectIndex], position, Quaternion.identity, parent);
            y0 -= 1;
        }    
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("path"))
        {
            rb.isKinematic = _collider.isTrigger = true;
            pathSpeed = 30f;
            var airEffectMain = airEffect.main;
            airEffectMain.simulationSpeed /= 2;

            Dust.transform.position = other.contacts[0].point + new Vector3(0f, 0.5f, 0f);
            Dust.GetComponent<Renderer>().material = BallRenderer.material;
            Dust.Play();
        }
    }
}
