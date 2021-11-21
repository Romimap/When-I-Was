using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterferenceZone : MonoBehaviour {
    public static int levelBackup = -1;
    public static List<InterferenceZone> collidingZones = new List<InterferenceZone>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D (Collider2D collision) {
        if (collision.tag.Equals("Player")) {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null) {
                if (levelBackup == -1) {
                    levelBackup = playerController.Level;
                    playerController.Level = 0;
                }
                if (!collidingZones.Contains(this)) {
                    collidingZones.Add(this);
                }
            }
        }
    }

    private void OnTriggerExit2D (Collider2D collision) {
        if (collision.tag.Equals("Player")) {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            if (playerController != null) {
                collidingZones.Remove(this);
                if (collidingZones.Count == 0) {
                    playerController.Level = levelBackup;
                    levelBackup = -1;
                }
            }
        }
    }
}
