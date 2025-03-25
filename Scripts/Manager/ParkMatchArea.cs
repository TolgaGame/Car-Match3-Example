using System.Collections.Generic;
using UnityEngine;

public class ParkMatchArea : MonoBehaviour
{
    public static ParkMatchArea Instance;

    // Inspector'dan 4 adet slot Transform'unu atayın.
    public Transform[] slots;

    // Slotların doluluğunu takip etmek için.
    private bool[] slotOccupied;

    // Parka giren arabaları takip ederiz.
    public List<GameObject> parkedCars = new List<GameObject>();

    void Awake()
    {
        Instance = this;
        // Her slot başlangıçta boş.
        slotOccupied = new bool[slots.Length];
        for (int i = 0; i < slotOccupied.Length; i++)
        {
            slotOccupied[i] = false;
        }
    }

    // Araba park alanına gönderildiğinde uygun slotu bulup aracı hedef pozisyona yönlendirir.
    public void MoveCarToPark(GameObject car)
    {
        // Uygun slotu bul.
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slotOccupied[i])
            {
                // Slot boşsa, hemen işaretleyelim.
                slotOccupied[i] = true;

                CarController controller = car.GetComponent<CarController>();
                if (controller != null)
                {
                    Debug.Log("Moving to destination: " + slots[i].position);
                    controller.MoveToDestination(slots[i].position);
                }
                parkedCars.Add(car);
                break;
            }
        }

        // Slotlara eklenen arabalar arasında match kontrolü yap.
        CheckMatch();
    }

    // Örnek: Eşleşme kontrolü sonrası slotu temizleyelim.
    void CheckMatch()
    {
        Dictionary<Color, List<GameObject>> colorDict = new Dictionary<Color, List<GameObject>>();
        foreach (var car in parkedCars)
        {
            if (car != null)
            {
                CarController controller = car.GetComponent<CarController>();
                if (controller != null)
                {
                    Color carColor = controller.carColor;
                    if (!colorDict.ContainsKey(carColor))
                    {
                        colorDict[carColor] = new List<GameObject>();
                    }
                    colorDict[carColor].Add(car);
                }
            }
        }

        // Aynı renkten 3 veya daha fazla araba var ise, eşleşenleri temizleyelim.
        foreach (var kvp in colorDict)
        {
            if (kvp.Value.Count >= 3)
            {
                foreach (var matchedCar in kvp.Value)
                {
                    // İlgili slotu temizlemek için, aracın hedef slotunu belirleyip slotOccupied dizisinde işareti kaldırabilirsiniz.
                    // Burada basitçe yok ediyoruz.
                    Destroy(matchedCar);
                    parkedCars.Remove(matchedCar);
                }
                break;
            }
        }
    }
}
