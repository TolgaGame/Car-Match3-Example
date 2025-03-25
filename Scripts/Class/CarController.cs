using UnityEngine;
using UnityEngine.AI;

public enum CarType {
    Normal,
    Special,
    Bomb
}

public class CarController : MonoBehaviour {
    #region Variables

    [Header("Car Properties")]
    public Color carColor;
    public CarType carType = CarType.Normal;
    public float moveSpeed = 5f;
    public float rotationSpeed = 360f;

    [Header("Special Effects")]
    public ParticleSystem matchEffect;
    public ParticleSystem specialEffect;

    private Vector2Int gridPosition;
    private Vector3 targetPosition;
    private bool isMoving;
    private bool isMatched;
    private NavMeshAgent agent;

    #endregion

    #region Unity Methods

    private void Awake() {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start() {
        InitializeSpecialEffects();
    }

    private void Update() {
        if (isMoving) {
            MoveToTarget();
        }
    }

    #endregion

    #region Movement Methods

    public void MoveToDestination(Vector3 destination) {
        targetPosition = destination;
        isMoving = true;
    }

    private void MoveToTarget() {
        // Move towards target position.
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Rotate the car while moving.
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Check if destination is reached.
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f) {
            transform.position = targetPosition;
            isMoving = false;
            transform.rotation = Quaternion.identity;
        }
    }

    #endregion

    #region Match Methods

    public void OnMatch() {
        if (!isMatched) {
            isMatched = true;
            PlayMatchEffects();
            HandleSpecialCarEffects();
        }
    }

    private void PlayMatchEffects() {
        if (matchEffect != null) {
            matchEffect.Play();
        }
    }

    private void HandleSpecialCarEffects() {
        switch (carType) {
            case CarType.Bomb:
                TriggerBombEffect();
                break;
            case CarType.Special:
                // Add special car effects here.
                break;
        }
    }

    private void TriggerBombEffect() {
        // Find and match nearby cars.
        Collider[] nearbyCars = Physics.OverlapSphere(transform.position, 2f);
        foreach (var car in nearbyCars) {
            CarController otherCar = car.GetComponent<CarController>();
            if (otherCar != null && otherCar != this) {
                otherCar.OnMatch();
            }
        }
    }

    #endregion

    #region Input Methods

    private void OnMouseDown() {
        if (!isMatched) {
            Locator.Instance.GridManagerInstance.OnCarClicked(this);
        }
    }

    #endregion

    #region Initialization Methods

    private void InitializeSpecialEffects() {
        if (carType == CarType.Special || carType == CarType.Bomb) {
            if (specialEffect != null) {
                specialEffect.Play();
            }
        }
    }

    #endregion

    #region Properties

    public Vector2Int GridPosition {
        get { return gridPosition; }
        set { gridPosition = value; }
    }

    #endregion
}