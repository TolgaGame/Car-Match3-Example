using UnityEngine;
using System.Collections.Generic;

public class ParkingSpot : MonoBehaviour {
    public Color spotColor; // Expected car color for this parking spot
}

public class MatchManager : MonoBehaviour {
    [Header("Match Settings")]
    public int minMatchCount = 3;
    public float matchAnimationDuration = 0.5f;

    // Unity Methods
    private void Awake() {
        // Register with the Locator system.
        Locator.Instance.RegisterMatchManager(this);
    }

    private void OnDestroy() {
        // Unregister from the Locator system.
        if (Locator.Instance != null) {
            Locator.Instance.UnregisterMatchManager();
        }
    }

    // Match Detection Methods
    public bool CheckMatch(Node[,] grid, int x, int y, Color carColor) {
        int matchCount = 1;
        // Check horizontal matches.
        matchCount += CountMatchesInDirection(grid, x, y, carColor, Vector2Int.right);
        matchCount += CountMatchesInDirection(grid, x, y, carColor, Vector2Int.left);
        // Check vertical matches.
        matchCount += CountMatchesInDirection(grid, x, y, carColor, Vector2Int.up);
        matchCount += CountMatchesInDirection(grid, x, y, carColor, Vector2Int.down);

        return matchCount >= minMatchCount;
    }

    private int CountMatchesInDirection(Node[,] grid, int startX, int startY, Color carColor, Vector2Int direction) {
        int count = 0;
        int currentX = startX + direction.x;
        int currentY = startY + direction.y;

        while (IsValidGridPosition(grid, currentX, currentY)) {
            // Check if the node has a car with matching color.
            // if (grid[currentX, currentY].carColor == carColor) {
            //     count++;
            // } else {
            //     break;
            // }
            currentX += direction.x;
            currentY += direction.y;
        }

        return count;
    }

    private bool IsValidGridPosition(Node[,] grid, int x, int y) {
        return x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1);
    }

    // Match Processing Methods
    public List<GameObject> FindMatches(List<GameObject> cars) {
        Dictionary<Color, List<GameObject>> colorGroups = new Dictionary<Color, List<GameObject>>();
        List<GameObject> matchedCars = new List<GameObject>();

        // Group cars by color.
        foreach (var car in cars) {
            CarController controller = car.GetComponent<CarController>();
            if (controller != null) {
                Color carColor = controller.carColor;
                if (!colorGroups.ContainsKey(carColor)) {
                    colorGroups[carColor] = new List<GameObject>();
                }
                colorGroups[carColor].Add(car);
            }
        }

        // Check for matches in each color group.
        foreach (var group in colorGroups) {
            if (group.Value.Count >= minMatchCount) {
                matchedCars.AddRange(group.Value);
            }
        }

        return matchedCars;
    }

    public void ProcessMatches(List<GameObject> matchedCars) {
        if (matchedCars.Count >= minMatchCount) {
            bool isSpecialMatch = matchedCars.Count >= 4;
            Locator.Instance.GameManagerInstance.AddScore(matchedCars.Count, isSpecialMatch);

            // Animate matched cars.
            foreach (var car in matchedCars) {
                StartCoroutine(AnimateMatch(car));
            }
        }
    }

    // Animation Methods
    private System.Collections.IEnumerator AnimateMatch(GameObject car) {
        float elapsedTime = 0f;
        Vector3 originalScale = car.transform.localScale;
        Vector3 targetScale = originalScale * 1.2f;

        while (elapsedTime < matchAnimationDuration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / matchAnimationDuration;

            // Scale animation.
            car.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);

            // Fade out effect.
            Renderer renderer = car.GetComponent<Renderer>();
            if (renderer != null) {
                Color color = renderer.material.color;
                color.a = Mathf.Lerp(1, 0, t);
                renderer.material.color = color;
            }
            yield return null;
        }
        // Destroy the car after animation.
        Destroy(car);
    }
}