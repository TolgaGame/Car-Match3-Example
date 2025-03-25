using System.Collections.Generic;
using UnityEngine;

public class ParkMatchArea : MonoBehaviour
{
    public static ParkMatchArea Instance;

    // Assign 4 slot Transforms from the Inspector.
    public Transform[] slots;

    // To track if slots are occupied.
    private bool[] slotOccupied;

    // List to keep track of parked cars.
    public List<GameObject> parkedCars = new List<GameObject>();

    private void Awake() {
        Instance = this;
        // Initially, each slot is empty.
        slotOccupied = new bool[slots.Length];
        for (int i = 0; i < slotOccupied.Length; i++) {
            slotOccupied[i] = false;
        }
    }

    // When a car is sent to the park area, find an available slot and direct the car to the target position.
    public void MoveCarToPark(GameObject car) {
        // Find an available slot.
        for (int i = 0; i < slots.Length; i++) {
            if (!slotOccupied[i]) {
                // If the slot is available, mark it immediately.
                slotOccupied[i] = true;

                CarController controller = car.GetComponent<CarController>();
                if (controller != null) {
                    Debug.Log("Moving to destination: " + slots[i].position);
                    controller.MoveToDestination(slots[i].position);
                }
                parkedCars.Add(car);
                break;
            }
        }

        // Perform match check among the parked cars.
        CheckMatch();
    }

    // Example: After matching, clear the slot.
    private void CheckMatch() {
        Dictionary<Color, List<GameObject>> colorDict = new Dictionary<Color, List<GameObject>>();
        foreach (var car in parkedCars) {
            if (car != null) {
                CarController controller = car.GetComponent<CarController>();
                if (controller != null) {
                    Color carColor = controller.carColor;
                    if (!colorDict.ContainsKey(carColor)) {
                        colorDict[carColor] = new List<GameObject>();
                    }
                    colorDict[carColor].Add(car);
                }
            }
        }

        // If there are 3 or more cars of the same color, clear the matched ones.
        foreach (var kvp in colorDict) {
            if (kvp.Value.Count >= 3) {
                foreach (var matchedCar in kvp.Value) {
                    // To clear the associated slot, you may determine the car's target slot and update the slotOccupied array accordingly.
                    // For simplicity, we simply destroy the car.
                    Destroy(matchedCar);
                    parkedCars.Remove(matchedCar);
                }
                break;
            }
        }
    }
}
