using UnityEngine;

public class Locator : MonoBehaviour {
    public static Locator _instance;
    private static bool _instanceSet;

    public static Locator Instance {
        get {
            if (!_instanceSet) {
                _instance = FindObjectOfType<Locator>();

                if (_instance == null) {
                    Debug.LogError("Locator is not present in the scene, singleton failed");
                    return null;
                }
                _instanceSet = true;
            }
            return _instance;
        }
    }

    [Header("Instances")]
    public GameManager GameManagerInstance;
    public SoundManager SoundManagerInstance;
    public UIManager UIManagerInstance;
    public GridManager GridManagerInstance;
    public ParkMatchArea ParkMatchAreaInstance;
    public SaveSystem SaveSystemInstance;

    private void OnDestroy() {
        if (_instance == this) {
            _instanceSet = false;
        }
    }

    public void RegisterGameManager(GameManager manager) {
        GameManagerInstance = manager;
    }

    public void UnregisterGameManager() {
        GameManagerInstance = null;
    }

    public void RegisterMatchManager(MatchManager manager) {
        // MatchManager instance registration if needed
    }

    public void UnregisterMatchManager() {
        // MatchManager unregistration if needed
    }
}