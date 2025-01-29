using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour{
	public GameObject point_Gen_Health;
	public GameObject healthSprite;
	public GameObject canvas;
	public static GameManager instance;
	[SerializeField]private List<GameObject> heals = new List<GameObject>();
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		instance = this;
		AddHealth(4);
	}

	// Update is called once per frame
	void Update()
	{
        

	}

	void AddHealth(int amount){
		for (int i = 0; i < amount; i++){
			GameObject health;
			if (heals.Count == 0){
				health = Instantiate(healthSprite, point_Gen_Health.transform.position, Quaternion.identity);
			}
			else{
				// Desplaza el nuevo corazón hacia la derecha del último corazón
				float healthWidth = healthSprite.GetComponent<RectTransform>().rect.width;
				health = Instantiate(healthSprite, heals.Last().transform.position + new Vector3(healthWidth, 0, 0), Quaternion.identity);
			}
			health.transform.SetParent(canvas.transform);

			heals.Add(health);
		}
	}
    
	public void PlayerReceiveDamage(int damage)
	{
		for (int i = 0; i < damage; i++)
		{
			Destroy(heals.Last());
			heals.RemoveAt(heals.Count - 1);
		}
	}

	public void PlayerDie(){
		foreach (GameObject health in heals){
			Destroy(health);
		}
		heals.Clear();
	}
}